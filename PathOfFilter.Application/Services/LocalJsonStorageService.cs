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

        var fileName = command.Options["src"];
        var items = new List<FilterItem>();



        // TODO implement loading
        return items;

    }

    public async Task<List<FilterItem>?> LoadBaseItems()
    {
        var itemTypes = LoadItems("ItemTypes", document => document.GetItemTypeDefinitions());

        if (itemTypes == null) return null;

        return LoadItems("BaseItems", document => document.GetItems(itemTypes));
    }

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
