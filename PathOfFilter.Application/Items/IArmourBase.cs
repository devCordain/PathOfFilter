namespace PathOfFilter.Application.Items;

internal interface IArmourBase : IInherentQuality
{
    public int MinimumArmour { get; set; }
    public int MaximumArmour { get; }
}
