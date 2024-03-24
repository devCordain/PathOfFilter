namespace PathOfFilter.Application.Items;

internal interface IArmourBase : IBaseDefencePercentile
{
    public int MinimumArmour { get; set; }
    public int MaximumArmour { get; }
}
