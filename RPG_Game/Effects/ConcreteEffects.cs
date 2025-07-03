using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;

namespace ProOb_RPG.Effects
{
    internal class EffectNoEffect : EntityEffect
    {
        public override string EffectName => "Nothing";
        public override string[] EffectDescription => new string[] { $"Nothing" };

        public EffectNoEffect(int duration = -1) : base(duration) { }
    }
    internal class EffectStrength : EntityEffect
    {
        public override string EffectName => "Strength";
        public override string[] EffectDescription => new string[] { $"(+{StrengthBonus}) Strength" };
        private static int StrengthBonus => 10;

        public EffectStrength(int duration = -1) : base(duration) { }
        public override void ModifyEntityStats(Entity.EntityStats stats)
        {
            stats.Strength += StrengthBonus;
        }
    }

    internal class EffectDragonBreath : EntityEffect
    {
        public override string EffectName => "Dragon Breath";
        public override string[] EffectDescription => new string[]
        {
            $"(+{StrengthBonus}) Strength",
            $"(+{DexterityBonus}) Dexterity",
            $"({MaxHealthBonus}) Max Health",
        };

        private static int StrengthBonus => 1;
        private static int DexterityBonus => 2;
        private static int MaxHealthBonus => -10;

        public EffectDragonBreath(int duration = -1) : base(duration) { }
        public override void ModifyEntityStats(Entity.EntityStats stats)
        {
            stats.Strength += StrengthBonus;
            stats.Dexterity += DexterityBonus;
            stats.MaxHealth += MaxHealthBonus;
        }
    }

    internal class EffectDrunk : EntityEffect
    {
        public override string EffectName => "Drunk";
        public override string[] EffectDescription => new string[] { $"(+{CurrentLuckBonus}) Luck (Falloff: 1)" };
        private static int LuckBonusMultiplier => 1;
        private int CurrentLuckBonus => Duration * LuckBonusMultiplier;

        public EffectDrunk(int duration = -1) : base(duration) 
        {
            duration = Math.Max(duration, 2);
        }
        public override void ModifyEntityStats(Entity.EntityStats stats)
        {
            stats.Luck += CurrentLuckBonus;
        }
    }
}
