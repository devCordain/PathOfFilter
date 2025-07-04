using PathOfFilter.Application.Handlers;
using PathOfFilter.Application.Services.Models;

var _filterHandler = new FilterService(new LocalJsonStorageService());
var input = "";
ItemFilter? itemFilter = null;
Console.WriteLine("Welcome to Path Of Filter");
ShowHelp();

while (true)
{
    try
    {
        input = Console.ReadLine();
        if(input == "exit")
        {
            Console.WriteLine("Bye");
            break;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Awaiting Next Command");
            continue;
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLower();

        switch (command)
        {
            case "history":
                if (itemFilter?.Id != null)
                {
                    _filterHandler.ShowHistory(itemFilter.Id);
                }
                else
                {
                    Console.WriteLine("No active filter. Load a filter first with: load src <path>");
                }
                break;

            case "rollback":
                if (itemFilter?.Id == null)
                {
                    Console.WriteLine("No active filter. Load a filter first with: load src <path>");
                }
                else if (parts.Length < 2)
                {
                    Console.WriteLine("Usage: rollback <version_number>");
                    Console.WriteLine("Tip: Use 'history' to see available versions");
                }
                else if (!int.TryParse(parts[1], out int version))
                {
                    Console.WriteLine($"Invalid version number: {parts[1]}");
                    Console.WriteLine("Usage: rollback <version_number>");
                }
                else
                {
                    var rolledBackFilter = _filterHandler.RollbackToVersion(itemFilter.Id, version);
                    if (rolledBackFilter != null)
                    {
                        itemFilter = rolledBackFilter;
                        FilterVisualiser(itemFilter);
                    }
                }
                break;

            case "list":
                _filterHandler.ListCachedFilters();
                break;

            case "clean":
                if (itemFilter?.Id == null)
                {
                    Console.WriteLine("No active filter. Load a filter first with: load src <path>");
                }
                else
                {
                    int keepCount = 5;
                    if (parts.Length > 1 && int.TryParse(parts[1], out int customKeepCount))
                    {
                        keepCount = customKeepCount;
                    }
                    _filterHandler.CleanOldVersions(itemFilter.Id, keepCount);
                }
                break;

            case "help":
                ShowHelp();
                break;

            default:
                itemFilter = await _filterHandler.Create(itemFilter, input);
                FilterVisualiser(itemFilter);
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine("Tip: Type 'help' for available commands or check your input format");
        Console.WriteLine();
    }

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

void ShowHelp()
{
    Console.WriteLine("Available Commands:");
    Console.WriteLine("==================");
    Console.WriteLine("load src <path>          - Load a .filter file from the specified path");
    Console.WriteLine("rename name <name>       - Rename the current filter");
    Console.WriteLine("history                  - Show version history of the current filter");
    Console.WriteLine("rollback <version>       - Rollback to a specific version");
    Console.WriteLine("list                     - List all cached filters");
    Console.WriteLine("clean [keep_count]       - Clean old versions (default: keep 5 versions)");
    Console.WriteLine("help                     - Show this help message");
    Console.WriteLine("exit                     - Exit the application");
    Console.WriteLine();
}