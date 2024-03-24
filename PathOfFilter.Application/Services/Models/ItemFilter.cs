namespace PathOfFilter.Application.Services.Models;

public class ItemFilter
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
    public string LatestCommand { get; set; }

    public List<FilterItem> Items { get; set; }

    public ItemFilter(ItemFilter? oldFilter, List<FilterItem>? loadedItems, FilterCommand? command, List<FilterItem> baseItems)
    {
        if (oldFilter == null)
        {
            Id = Guid.NewGuid();
            Name = "New Filter";
            Version = 1;
            Items = baseItems;

            ApplyCommand(command);

            return;
        }

        Id = oldFilter.Id;
        Version = oldFilter.Version;
        Items = Update(oldFilter.Items, baseItems);
        LatestCommand = ApplyCommand(command);
    }

    public ItemFilter(string name, List<FilterItem> items) 
    {
        Items = items;
        Name = name;
        LatestCommand = "load";
    }


    private List<FilterItem> Update(List<FilterItem> oldItems, List<FilterItem> baseItems)
    {
        var items = new List<FilterItem>();

        foreach (var baseItem in baseItems)
        {
            var subItems = oldItems.Where(x => x.Name == baseItem.Name);

            if (subItems.Any())
            {
                foreach (var subItem in subItems)
                {
                    // Todo Implement

                }
                
                items.AddRange(subItems);
                continue;
            }

            items.Add(baseItem);
        }


        return items;
    }

    private string ApplyCommand(FilterCommand? command)
    {
        if (command == null) return LatestCommand;
        
        if (command.Name == "rename")
        {
            Name = command.Options["name"];
            Version++;
        }



        return command.ToString();
    }
}