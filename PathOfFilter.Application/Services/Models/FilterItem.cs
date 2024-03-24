namespace PathOfFilter.Application.Services.Models;

public class FilterItem
{
    public string Name { get; set; }
    public string Category { get; set; }
    public List<string> ItemTypes { get; set; }
    public Visual? Visual { get; set; }
    public Audio? Audio { get; set; }
    public LevelRange? ItemLevel { get; set; }
    public Rarity? Rarity { get; set; }
    public bool Show { get; set; }

    public FilterItem(string name, string category, List<string> itemTypes, Visual? visual = null, Audio? audio = null, LevelRange? itemLevel = null, Rarity? rarity = null, bool show = true)
    {
        Name = name;
        Category = category;
        ItemTypes = itemTypes;
        Visual = visual;
        Audio = audio;
        ItemLevel = itemLevel;
        Rarity = rarity;
        Show = show;
    }
}
