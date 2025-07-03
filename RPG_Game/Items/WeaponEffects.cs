using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG.Items
{
    internal class WeaponEffectSharpness : WeaponDecorator
    {
        private static int DamageBonus => 1;
        protected override string EffectName => "Sharpness";
        public WeaponEffectSharpness(IItem wrappee, int effectLevel) : base(wrappee, effectLevel) { }
        public override int GetItemDamage()
        {
            return base.GetItemDamage() + DamageBonus * (EffectLevel + 1);
        }
    }
    internal class WeaponEffectSussy : WeaponDecorator
    {
        private static int WisdomBonus => -1;
        private static int LuckBonus => 1;
        protected override string EffectName => "Sussy";
        public WeaponEffectSussy(IItem wrappee, int effectLevel) : base(wrappee, effectLevel) { }
        public override bool PickUpItem(Dungeon dungeon, Entity entity, IItem item)
        {
            if (!base.PickUpItem(dungeon, entity, item))
                return false;
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.Wisdom += WisdomBonus;
            stats.Luck += LuckBonus;
            return true;
        }
        public override bool DropItem(Dungeon dungeon, Entity entity, IItem item)
        {
            if (!base.DropItem(dungeon, entity, item))
                return false;
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.Wisdom -= WisdomBonus;
            stats.Luck -= LuckBonus;
            return true;
        }
    }
    internal class WeaponEffectGains : WeaponDecorator
    {
        private static int StrengthBonus => 3;
        private static int WisdomBonus => -2;
        protected override string EffectName => "Gains";
        public WeaponEffectGains(IItem wrappee, int effectLevel) : base(wrappee, effectLevel) { }
        public override bool EquipItemInHand(Entity entity, IItem item, Hands hand)
        {
            if (!base.EquipItemInHand(entity, item, hand))
                return false;
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.Wisdom += WisdomBonus;
            stats.Strength += StrengthBonus;
            return true;
        }
        public override bool UnEquipItemInHand(Entity entity, Hands hand)
        {
            if (!base.UnEquipItemInHand(entity, hand))
                return false;
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.Wisdom -= WisdomBonus;
            stats.Strength -= StrengthBonus;
            return true;
        }
    }
}
