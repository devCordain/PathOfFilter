namespace PathOfFilter.Application.Contracts;

internal interface IEnergyShieldBase
{
    public int MinimumEnergyShield { get; set; }
    public int MaximumEnergyShield { get; set; }
}
