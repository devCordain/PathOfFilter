using PathOfFilter.Application.Models;
using PathOfFilter.Application.Services;

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

    itemFilter = await CommandHandler.CreateFilter(itemFilter, input);

    FilterVisualiser(itemFilter);
    Console.WriteLine("Awaiting Next Command");
}

void FilterVisualiser(ItemFilter filter)
{
    foreach (var item in filter.Items)
    {
        Console.WriteLine(item.Name);
    }
}