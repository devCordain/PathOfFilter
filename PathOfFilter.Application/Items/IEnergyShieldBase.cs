namespace PathOfFilter.Application.Items;

internal interface IEnergyShieldBase : IInherentQuality
{
    public int MinimumEnergyShield { get; set; }
    public int MaximumEnergyShield { get; set; }
}
