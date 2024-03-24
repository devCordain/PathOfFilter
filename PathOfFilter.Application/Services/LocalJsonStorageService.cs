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

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            var currentObject = new Dictionary<string, string>();
            while ((line = sr.ReadLine()) != null)
            {
                line = line.WithoutFilterComments();

                // If the line is not empty after removing comments
                if (string.IsNullOrWhiteSpace(line))
                {
                    // Split into objects based on "Show" or "Hide" keywords
                    if (line.StartsWith("Show") || line.StartsWith("Hide"))
                    {
                        // If currentObject is not empty, add it to extractedData
                        if (currentObject.Count > 0)
                        {
                            //items.Add(currentObject);
                        }
                        // Start a new object
                        currentObject = new Dictionary<string, string>();
                    }

                    // Add the line to currentObject
                    //currentObject.Add(line);
                }
            }

            // Add the last object to extractedData
            if (currentObject.Count > 0)
            {
                //items.Add(currentObject);
            }
        }

        // TODO implement loading
        return items;

    }

    public async Task<List<FilterItem>?> LoadBaseItems()
    {
        var itemTypes = LoadItems("Data\\TypeDefinitions", document => document.GetTypeDefinitions());

        if (itemTypes == null) return null;

        return LoadItems("Data\\Items", document => document.GetItems(itemTypes));
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
