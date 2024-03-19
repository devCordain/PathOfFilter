using PathOfFilter.Application.Contracts;
using PathOfFilter.Application.Items.Weapons.Wands;

namespace PathOfFilter.Application.Models;

public class ItemFilter
{
    public int Version { get; set; }

    public ItemFilter(ItemFilter? oldFilter, string? command)
    {
        Version = oldFilter.Version++;
        Items = oldFilter.Items;
        Items = Parse(command);
    }

    public ItemFilter(string? command)
    {
        Version = 1;
        Items = BaseItems();
        Items = Parse(command);
    }

    private List<IFilterItem> Parse(string? command) => Items;

    public DateTime CreatedAt { get; set; }
    public List<IFilterItem> Items { get; set; }
    private static List<IFilterItem> BaseItems() => new List<IFilterItem>() { 
        new SageWand()
    };
}