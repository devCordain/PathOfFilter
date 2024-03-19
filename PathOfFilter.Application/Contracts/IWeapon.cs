namespace PathOfFilter.Application.Contracts;

internal interface IWeapon
{
    public int MinimumDamage { get; set; }
    public int MaximumDamage { get; set;}
    public double AttackSpeed { get; set; }
    public bool IsTwoHanded { get; set; }
    public double CritChance { get; set; }
}
