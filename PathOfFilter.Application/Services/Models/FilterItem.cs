namespace PathOfFilter.Application.Services.Models;

public class FilterItem
{
    public string Name { get; set; }
    public string Category { get; set; }
    public List<string> ItemTypes { get; set; }
    public Visual? Visual { get; set; }
    public Audio? Audio { get; set; }
    public LevelRange? ItemLevel { get; set; }
    public LevelRange? ZoneLevel { get; set; }
    public Rarity? Rarity { get; set; }
    public bool Show { get; set; }
    public bool Continue {  get; set; }

    public FilterItem(string name, string category, List<string> itemTypes, Visual? visual = null, Audio? audio = null, LevelRange? itemLevel = null, LevelRange? zoneLevel = null, Rarity? rarity = null, bool show = true, bool @continue = false)
    {
        Name = name;
        Category = category;
        ItemTypes = itemTypes;
        Visual = visual;
        Audio = audio;
        ItemLevel = itemLevel;
        ZoneLevel = zoneLevel;
        Rarity = rarity;
        Show = show;
        Continue = @continue;
    }
}
