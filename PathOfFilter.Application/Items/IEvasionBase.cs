namespace PathOfFilter.Application.Items;

internal interface IEvasionBase : IInherentQuality
{
    public int MinimumEvasion { get; set; }
    public int MaximumEvasion { get; set; }
}
