using PathOfFilter.Application.Contracts;
using PathOfFilter.Application.Services.Models;

namespace PathOfFilter.Application.Handlers;
public class FilterService
{
    private readonly IStorage _storage;

    public FilterService(IStorage storage)
    {
        _storage = storage;
    }

    public async Task<ItemFilter?> Create(ItemFilter? existingFilter, string? input)
    {
        try
        {
            var command = FilterCommand.TryCreate(input);
            if (command == null) return existingFilter;

            // Special handling for load command
            if (command.Name.ToLower() == "load")
            {
                return await HandleLoadCommand(existingFilter, command);
            }

            var baseItems = await _storage.LoadBaseItems();
            if (baseItems == null) 
            {
                Console.WriteLine("Failed to load base items. Please check your data directory.");
                return existingFilter;
            }

            var newFilter = new ItemFilter(existingFilter, null, command, baseItems);
            await _storage.Save(existingFilter, newFilter);
            return newFilter;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing command: {ex.Message}");
            Console.WriteLine("Type 'help' for available commands");
            return existingFilter;
        }
    }

    private async Task<ItemFilter?> HandleLoadCommand(ItemFilter? existingFilter, FilterCommand command)
    {
        try
        {
            var loadedFilterItems = await _storage.Load(command);
            if (loadedFilterItems == null) 
            {
                Console.WriteLine("Failed to load filter. Please check the file path and format.");
                return existingFilter;
            }

            var baseItems = await _storage.LoadBaseItems();
            if (baseItems == null) 
            {
                Console.WriteLine("Failed to load base items. Please check your data directory.");
                return existingFilter;
            }

            // If there's already an active filter, ask user what to do
            if (existingFilter != null)
            {
                Console.WriteLine($"You already have an active filter: '{existingFilter.Name}' (Version {existingFilter.Version})");
                Console.WriteLine("Do you want to:");
                Console.WriteLine("1. Replace the current filter with the new one");
                Console.WriteLine("2. Keep the current filter and cancel the load");
                Console.WriteLine("Enter 1 or 2:");
                
                var choice = Console.ReadLine();
                if (choice != "1")
                {
                    Console.WriteLine("Load cancelled. Current filter unchanged.");
                    return existingFilter;
                }
                Console.WriteLine("Loading new filter...");
            }

            // Create new filter from loaded items
            var newFilter = new ItemFilter(null, loadedFilterItems, command, baseItems);
            
            // Set a proper name based on the file path
            if (command.Options.ContainsKey("src"))
            {
                var fileName = Path.GetFileNameWithoutExtension(command.Options["src"]);
                newFilter.Name = fileName;
            }

            await _storage.Save(null, newFilter);
            Console.WriteLine($"Successfully loaded filter: '{newFilter.Name}'");
            return newFilter;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading filter: {ex.Message}");
            Console.WriteLine("Please check the file path and format.");
            return existingFilter;
        }
    }

    public void ShowHistory(Guid filterId)
    {
        var history = _storage.LoadFilterHistory(filterId);
        
        if (!history.Any())
        {
            Console.WriteLine("No history found for this filter.");
            return;
        }

        Console.WriteLine($"Filter History ({history.Count} versions):");
        Console.WriteLine("Version | Name");
        Console.WriteLine("--------|-----");
        
        for (int i = 0; i < history.Count; i++)
        {
            var version = history[i];
            Console.WriteLine($"{i + 1,7} | {version.Name}");
        }
    }

    public ItemFilter? RollbackToVersion(Guid filterId, int version)
    {
        var targetFilter = _storage.LoadFilterVersion(filterId, version);
        
        if (targetFilter == null)
        {
            Console.WriteLine($"Version {version} not found for filter {filterId}");
            return null;
        }

        Console.WriteLine($"Rolled back to version {version} of filter '{targetFilter.Name}'");
        return targetFilter;
    }

    public void ListCachedFilters()
    {
        var cachedFilters = _storage.ListCachedFilters();
        
        if (!cachedFilters.Any())
        {
            Console.WriteLine("No cached filters found.");
            return;
        }

        Console.WriteLine("Cached Filters:");
        Console.WriteLine("ID                                   | Name                    | Latest Version");
        Console.WriteLine("-------------------------------------|-------------------------|---------------");
        
        foreach (var (id, name, latestVersion) in cachedFilters)
        {
            Console.WriteLine($"{id} | {name,-23} | {latestVersion}");
        }
    }

    public void CleanOldVersions(Guid filterId, int keepCount = 5)
    {
        var cacheBasePath = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        var filterPath = Path.Combine(cacheBasePath, filterId.ToString());

        if (!Directory.Exists(filterPath))
        {
            Console.WriteLine("Filter not found in cache.");
            return;
        }

        var versionDirs = Directory.GetDirectories(filterPath)
            .Select(dir => new DirectoryInfo(dir))
            .Where(dirInfo => int.TryParse(dirInfo.Name, out _))
            .OrderByDescending(dirInfo => int.Parse(dirInfo.Name))
            .Skip(keepCount)
            .ToList();

        if (!versionDirs.Any())
        {
            Console.WriteLine($"No old versions to clean (keeping {keepCount} versions).");
            return;
        }

        foreach (var versionDir in versionDirs)
        {
            try
            {
                versionDir.Delete(true);
                Console.WriteLine($"Deleted version {versionDir.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete version {versionDir.Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"Cleaned {versionDirs.Count} old versions, keeping {keepCount} most recent.");
    }
}
