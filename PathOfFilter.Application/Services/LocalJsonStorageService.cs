using PathOfFilter.Application.Contracts;
using PathOfFilter.Application.Services.Helpers;
using PathOfFilter.Application.Services.Models;
using System.Text.Json;

namespace PathOfFilter.Application.Handlers;
public class LocalJsonStorageService : IStorage
{
    public async Task<List<FilterItem>?> Load(FilterCommand? command)
    {
        if (command == null) return null;

        if (command.Name != "load") return null;

        var filePath = command.Options["src"];

        filePath = filePath.Trim();

        if (!filePath.EndsWith(".filter"))
        {
            Console.WriteLine($"File {filePath} is not supported");
            return null;
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File {filePath} does not exist");
            return null;
        }

        var items = new List<FilterItem>();

        var currentObject = new Dictionary<string, string>();
        using (StreamReader sr = new(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.RemoveFilterComments();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.IsNewFilterItem() && currentObject.Any())
                { 
                    items.AddRange(currentObject.ToFilterItems());
                    currentObject = new Dictionary<string, string>();
                    continue;
                }

                line.AddToCurrentFilterItem(currentObject);
            }
        }

        if (currentObject.Any()) items.AddRange(currentObject.ToFilterItems());

        return items;
    }

    public async Task<List<FilterItem>?> LoadBaseItems()
    {
        var typeDefinitions = LoadItems("Data\\TypeDefinitions", document => document.GetTypeDefinitions());

        if (typeDefinitions == null) return null;

        return LoadItems("Data\\Items", document => document.GetItems(typeDefinitions));
    }

    public async Task Save(ItemFilter? existingFilter, ItemFilter newFilter)
    {
        var cacheBasePath = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        var filterPath = Path.Combine(cacheBasePath, newFilter.Id.ToString());
        var versionPath = Path.Combine(filterPath, newFilter.Version.ToString());

        if (!Directory.Exists(versionPath))
        {
            Directory.CreateDirectory(versionPath);
        }

        var jsonPath = Path.Combine(versionPath, "filter.json");
        var filterFilePath = Path.Combine(versionPath, "filter.filter");

        var filterMetadata = new
        {
            Id = newFilter.Id,
            Name = newFilter.Name,
            Version = newFilter.Version,
            LatestCommand = newFilter.LatestCommand,
            Timestamp = DateTime.UtcNow,
            Items = newFilter.Items
        };

        var jsonContent = JsonSerializer.Serialize(filterMetadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(jsonPath, jsonContent);

        var filterContent = GenerateFilterFile(newFilter);
        await File.WriteAllTextAsync(filterFilePath, filterContent);

        Console.WriteLine($"Saved filter '{newFilter.Name}' version {newFilter.Version} to cache");
    }

    private string GenerateFilterFile(ItemFilter filter)
    {
        var filterContent = new System.Text.StringBuilder();
        filterContent.AppendLine($"# Filter: {filter.Name}");
        filterContent.AppendLine($"# Version: {filter.Version}");
        filterContent.AppendLine($"# Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        filterContent.AppendLine();

        var groupedItems = filter.Items.GroupBy(item => new { item.Show, item.Continue });

        foreach (var group in groupedItems)
        {
            var action = group.Key.Show ? "Show" : "Hide";
            filterContent.AppendLine(action);

            var classItems = group.Where(item => !string.IsNullOrEmpty(item.Category)).ToList();
            if (classItems.Any())
            {
                var classes = string.Join(" ", classItems.Select(item => $"\"{item.Category}\"").Distinct());
                filterContent.AppendLine($"    Class == {classes}");
            }

            var baseTypeItems = group.Where(item => !string.IsNullOrEmpty(item.Name)).ToList();
            if (baseTypeItems.Any())
            {
                var baseTypes = string.Join(" ", baseTypeItems.Select(item => $"\"{item.Name}\"").Distinct());
                filterContent.AppendLine($"    BaseType == {baseTypes}");
            }

            if (group.Key.Continue)
            {
                filterContent.AppendLine("    Continue");
            }

            filterContent.AppendLine();
        }

        return filterContent.ToString();
    }

    public List<ItemFilter> LoadFilterHistory(Guid filterId)
    {
        var cacheBasePath = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        var filterPath = Path.Combine(cacheBasePath, filterId.ToString());

        if (!Directory.Exists(filterPath))
        {
            return new List<ItemFilter>();
        }

        var versions = new List<ItemFilter>();
        var versionDirs = Directory.GetDirectories(filterPath)
            .Select(dir => new DirectoryInfo(dir))
            .Where(dirInfo => int.TryParse(dirInfo.Name, out _))
            .OrderBy(dirInfo => int.Parse(dirInfo.Name));

        foreach (var versionDir in versionDirs)
        {
            var jsonPath = Path.Combine(versionDir.FullName, "filter.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    var jsonContent = File.ReadAllText(jsonPath);
                    var filterData = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                    
                    var filter = new ItemFilter(
                        filterData.GetProperty("Name").GetString() ?? "Unknown",
                        filterData.GetProperty("Items").Deserialize<List<FilterItem>>() ?? new List<FilterItem>()
                    );

                    versions.Add(filter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load filter version {versionDir.Name}: {ex.Message}");
                }
            }
        }

        return versions;
    }

    public ItemFilter? LoadFilterVersion(Guid filterId, int version)
    {
        var cacheBasePath = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        var versionPath = Path.Combine(cacheBasePath, filterId.ToString(), version.ToString());
        var jsonPath = Path.Combine(versionPath, "filter.json");

        if (!File.Exists(jsonPath))
        {
            return null;
        }

        try
        {
            var jsonContent = File.ReadAllText(jsonPath);
            var filterData = JsonSerializer.Deserialize<JsonElement>(jsonContent);
            
            var filter = new ItemFilter(
                filterData.GetProperty("Name").GetString() ?? "Unknown",
                filterData.GetProperty("Items").Deserialize<List<FilterItem>>() ?? new List<FilterItem>()
            );

            return filter;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load filter version {version}: {ex.Message}");
            return null;
        }
    }

    public List<(Guid Id, string Name, int LatestVersion)> ListCachedFilters()
    {
        var cacheBasePath = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        var cachedFilters = new List<(Guid Id, string Name, int LatestVersion)>();

        if (!Directory.Exists(cacheBasePath))
        {
            return cachedFilters;
        }

        var filterDirs = Directory.GetDirectories(cacheBasePath)
            .Select(dir => new DirectoryInfo(dir))
            .Where(dirInfo => Guid.TryParse(dirInfo.Name, out _));

        foreach (var filterDir in filterDirs)
        {
            var filterId = Guid.Parse(filterDir.Name);
            var versionDirs = Directory.GetDirectories(filterDir.FullName)
                .Select(dir => new DirectoryInfo(dir))
                .Where(dirInfo => int.TryParse(dirInfo.Name, out _))
                .OrderByDescending(dirInfo => int.Parse(dirInfo.Name));

            var latestVersionDir = versionDirs.FirstOrDefault();
            if (latestVersionDir != null)
            {
                var jsonPath = Path.Combine(latestVersionDir.FullName, "filter.json");
                if (File.Exists(jsonPath))
                {
                    try
                    {
                        var jsonContent = File.ReadAllText(jsonPath);
                        var filterData = JsonSerializer.Deserialize<JsonElement>(jsonContent);
                        var name = filterData.GetProperty("Name").GetString() ?? "Unknown";
                        var version = int.Parse(latestVersionDir.Name);

                        cachedFilters.Add((filterId, name, version));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to read filter {filterId}: {ex.Message}");
                    }
                }
            }
        }

        return cachedFilters.OrderBy(f => f.Name).ToList();
    }

    private List<T>? LoadItems<T>(string directoryKey, Func<JsonDocument, List<T>> getItemFunc)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), directoryKey);

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory {path} does not exist");
            return null;
        }

        return TryGetItems(path, getItemFunc);
    }

    private List<T> TryGetItems<T>(string directoryPath, Func<JsonDocument, List<T>> getItemsFunc)
    {
        try
        {
            var allItems = new List<T>();

            var jsonFilePaths = Directory.GetFiles(directoryPath, "*.json");

            foreach (var filePath in jsonFilePaths)
            {
                try
                {
                    var jsonContent = File.ReadAllText(filePath);
                    var document = JsonDocument.Parse(jsonContent);

                    allItems.AddRange(getItemsFunc(document));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to read items from {filePath} due to {e.Message}");
                    continue;
                }
            }

            return allItems;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to read items from {directoryPath} due to {e.Message}");
            return null;
        }
    }
}
