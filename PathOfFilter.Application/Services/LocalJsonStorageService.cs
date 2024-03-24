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

    //Todo implement
    public async Task Save(ItemFilter? existingFilter, ItemFilter newFilter)
    {
        return;
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
