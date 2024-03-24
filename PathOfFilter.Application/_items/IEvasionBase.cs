namespace PathOfFilter.Application.Items;

internal interface IEvasionBase : IBaseDefencePercentile
{
    public int MinimumEvasion { get; set; }
    public int MaximumEvasion { get; set; }
}
