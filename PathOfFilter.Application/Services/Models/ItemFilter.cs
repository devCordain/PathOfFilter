namespace PathOfFilter.Application.Services.Models;

public class ItemFilter
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
    public string Action { get; set; }

    public List<FilterItem> Items { get; set; }

    public ItemFilter(ItemFilter? oldFilter, FilterCommand? command, List<FilterItem> baseItems)
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

        if (oldFilter.Version == 0)
        {
            Id = Guid.NewGuid();
            Version = 1;
            Name = oldFilter.Name;
            Items = Update(oldFilter.Items, baseItems);

            ApplyCommand(command); 
            
            return;
        }

        Id = oldFilter.Id;
        Version = oldFilter.Version + 1;
        Items = Update(oldFilter.Items, baseItems);
        ApplyCommand(command);
    }

    public ItemFilter(string name, List<FilterItem> items) 
    {
        Items = items;
        Name = name;
        Action = "load";
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

    private void ApplyCommand(FilterCommand? command)
    {
        if (command == null) 
        {
            Action = "none";
            return;
        }

        Action = command.Name;

        if (command.Name == "rename")
        {
            Name = command.Options["name"];
        }


        return;
    }
}