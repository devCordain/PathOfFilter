using PathOfFilter.Application.Contracts;
using PathOfFilter.Application.Models;

namespace PathOfFilter.Application.Items.Weapons.Wands
{
    internal class SageWand : IFilterItem, IWeapon
    {
        public string Name { get => "Sage Wand"; set => throw new NotImplementedException(); }
        public ItemCategory Category { get => ItemCategory.Wand; set => throw new NotImplementedException(); }
        public Visual? Visual { get => null; set => throw new NotImplementedException(); }
        public Audio? Audio { get => null   ; set => throw new NotImplementedException(); }
        public LevelRange? ItemLevel { get => null; set => throw new NotImplementedException(); }
        public Rarity? Rarity { get => null; set => throw new NotImplementedException(); }
        public bool Show { get => true; set => throw new NotImplementedException(); }
        public int MinimumDamage { get => 23; set => throw new NotImplementedException(); }
        public int MaximumDamage { get => 42; set => throw new NotImplementedException(); }
        public double AttackSpeed { get => 1.2; set => throw new NotImplementedException(); }
        public bool IsTwoHanded { get => false; set => throw new NotImplementedException(); }
        public double CritChance { get => 8; set => throw new NotImplementedException(); }
    }
}
