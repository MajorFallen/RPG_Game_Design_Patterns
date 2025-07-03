using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Items
{
    internal class WeaponLongSword : Weapon
    {
        protected override int Damage => 70;
        protected override int WeaponMaxHealth => 100;
        protected override string ItemName => "Long Sword";

        public WeaponLongSword() : base(WeaponTags.BehaviorDictionary[WeaponTags.Behavior.Dexterity]) {}
    }
    internal class WeaponGreatSword : TwoHandedWeapon
    {
        protected override int Damage => 90;
        protected override int WeaponMaxHealth => 150;
        protected override string ItemName => "Great Sword";
        public WeaponGreatSword() : base(WeaponTags.BehaviorDictionary[WeaponTags.Behavior.Strength]) { }
    }
    internal class WeaponFireWand : Weapon
    {
        protected override int Damage => 6;
        protected override int WeaponMaxHealth => 20;
        protected override string ItemName => "Fire Wand";
        public WeaponFireWand() : base(WeaponTags.BehaviorDictionary[WeaponTags.Behavior.Magic]) { }
    }
}
