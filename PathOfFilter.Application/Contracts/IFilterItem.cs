using PathOfFilter.Application.Models;

namespace PathOfFilter.Application.Contracts;

public interface IFilterItem
{
    public string Name { get; set; }
    public ItemCategory Category { get; set; }
    public Visual? Visual { get; set; }
    public Audio? Audio { get; set; }
    public LevelRange? ItemLevel { get; set; }
    public Rarity? Rarity { get; set; }
    public bool Show { get; set; }
}
