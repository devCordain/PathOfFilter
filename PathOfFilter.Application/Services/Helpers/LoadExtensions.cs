using PathOfFilter.Application.Services.Models;
using System.Text.Json;

namespace PathOfFilter.Application.Services.Helpers;
internal static class LoadExtensions
{
    internal static List<FilterItem> GetItems(this JsonDocument document, List<TypeDefinition> itemTypes)
    {
        var items = new List<FilterItem>();
      
        foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
        {
            try
            {
                items.Add(new FilterItem(
                    item.GetProperty("name").ToString(),
                    document.RootElement.GetProperty("class").ToString(),
                    document.RootElement.GetProperty("types").EnumerateArray().Select(typeName => typeName.ToString()).ToList()
                    ));
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to get item from {item}");
                continue;
            }
        }

        return items;
    }

    internal static List<TypeDefinition> GetTypeDefinitions(this JsonDocument document)
    {
        var itemTypes = new List<TypeDefinition>();

        foreach (var itemType in document.RootElement.EnumerateArray())
        {
            try
            {
                itemTypes.Add(JsonSerializer.Deserialize<TypeDefinition>(itemType));
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to get itemType definition from {itemType}");
                continue;
            }
        }

        return itemTypes;
    }

    internal static string RemoveFilterComments(this string input) => input.Split('#')[0].Trim();
    internal static bool IsNewFilterItem(this string input) => input.StartsWith("Show") || input.StartsWith("Hide");
    internal static List<FilterItem> ToFilterItems(this Dictionary<string,string> input)
    {
        var items = new List<FilterItem>();

        if (!input.ContainsKey("Show") && !input.ContainsKey("Hide")) return items;

        var show = input.ContainsKey("Show");
        var @continue = input.ContainsKey("Continue");
        var classValues = input.ContainsKey("Class") ? input["Class"] : null;
        var baseTypeValues = input.ContainsKey("BaseType") ? input["BaseType"] : null;
        var rarities = input.ContainsKey("Rarity") ? input["Rarity"] : null;

        // Handle Class conditions
        if (classValues != null)
        {
            var classes = classValues.Split(' ');
            if (classes.Length > 1)
            {
                var @operator = classes[0];
                if (@operator == "==")
                {
                    for (var i = 1; i < classes.Length; i++)
                    {
                        items.Add(new FilterItem("", classes[i], new(), show: show, @continue: @continue));
                    }
                }
            }
        }

        // Handle BaseType conditions
        if (baseTypeValues != null)
        {
            var baseTypes = baseTypeValues.Split(' ');
            if (baseTypes.Length > 1)
            {
                var @operator = baseTypes[0];
                if (@operator == "==")
                {
                    for (var i = 1; i < baseTypes.Length; i++)
                    {
                        items.Add(new FilterItem(baseTypes[i], "", new(), show: show, @continue: @continue));
                    }
                }
            }
        }

        // If no specific conditions were parsed, create a generic filter item
        // This prevents losing filter blocks that have conditions we don't fully parse yet
        if (items.Count == 0)
        {
            var description = "Filter Block";
            if (classValues != null) description += $" (Class: {classValues})";
            if (baseTypeValues != null) description += $" (BaseType: {baseTypeValues})";
            
            // Add other common conditions for description
            foreach (var kvp in input)
            {
                if (kvp.Key != "Show" && kvp.Key != "Hide" && kvp.Key != "Continue" && 
                    kvp.Key != "Class" && kvp.Key != "BaseType")
                {
                    description += $" ({kvp.Key}: {kvp.Value})";
                }
            }
            
            items.Add(new FilterItem(description, "FilterBlock", new(), show: show, @continue: @continue));
        }

        return items;
    }

    internal static void AddToCurrentFilterItem(this string input, Dictionary<string, string> currentFilterItem)
    {
        var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return;

        var key = parts[0];
        var value = string.Join(" ", parts, 1, parts.Length - 1);

        // Handle duplicate keys by appending to existing value or creating new entry
        if (currentFilterItem.ContainsKey(key))
        {
            // For conditions that can have multiple values, append them
            currentFilterItem[key] = currentFilterItem[key] + " | " + value;
        }
        else
        {
            currentFilterItem[key] = value;
        }
    }

}
