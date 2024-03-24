using PathOfFilter.Application.Handlers;
using PathOfFilter.Application.Services.Models;

var _filterHandler = new FilterService(new LocalJsonStorageService());
var input = "";
ItemFilter? itemFilter = null;
Console.WriteLine("Welcome to Path Of Filter");
while (true)
{
    input = Console.ReadLine();
    if(input == "exit")
    {
        Console.WriteLine("Bye");
        break;
    }

    itemFilter = await _filterHandler.Create(itemFilter, input);

    FilterVisualiser(itemFilter);
    Console.WriteLine("Awaiting Next Command");
}

void FilterVisualiser(ItemFilter? filter)
{
    if (filter == null)
    {
        Console.WriteLine("No filter available");
    }

    var groupedItems = filter?.Items.GroupBy(x => x.Category) ?? [];
    foreach (var itemGroup in groupedItems)
    {
        var item = itemGroup.FirstOrDefault();

        var displayString = $"{item.Category}: {string.Join(", ", itemGroup.Select(x => x.Name))}";
        Console.WriteLine(displayString + "\n");
    }
}