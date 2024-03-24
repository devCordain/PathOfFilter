namespace PathOfFilter.Application.Items;

internal interface IEnergyShieldBase : IBaseDefencePercentile
{
    public int MinimumEnergyShield { get; set; }
    public int MaximumEnergyShield { get; set; }
}
