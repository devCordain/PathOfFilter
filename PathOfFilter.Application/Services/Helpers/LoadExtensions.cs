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
        //Todo implement
        return new List<FilterItem>();
    }

    internal static void AddToCurrentFilterItem(this string intpu, Dictionary<string, string> currentFilterItem)
    {
        //Todo implement
    }

}
