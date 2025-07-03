using ProOb_RPG.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;

namespace ProOb_RPG.Items
{
    internal abstract class Potion : Item
    {
        protected abstract Func<EntityEffect> CreateEffect { get; }
        public Potion() : base() { }
        public override bool UseItem(Entity entity)
        {
            this.DestroyItemInEntity(entity);
            return entity.InflictEffect(CreateEffect());
        }

    }

    internal class PotionOfStrength : Potion
    {
        protected override string ItemName => "Potion of Strength";
        private static int EffectDuration => 8;

        protected override Func<EntityEffect> CreateEffect => () => new EffectStrength(EffectDuration);
    }

    internal class PotionOfDragonBreath : Potion
    {
        protected override string ItemName => "Potion of Dragon Breath";
        private static int EffectDuration => -1;

        protected override Func<EntityEffect> CreateEffect => () => new EffectDragonBreath(EffectDuration);
    }

    internal class PotionŻytnia : Potion
    {
        protected override string ItemName => "Żytnia Prosto z Piwniczki";
        private static int EffectDuration => 12;

        protected override Func<EntityEffect> CreateEffect => () => new EffectDrunk(EffectDuration);
    }
    internal class PotionMilk : Potion
    {
        protected override string ItemName => "Milk";

        protected override Func<EntityEffect> CreateEffect => () => new EffectNoEffect();

        public override bool UseItem(Entity entity)
        {
            this.DestroyItemInEntity(entity);
            entity.DispelAllEffects();
            return true;
        }
    }
}
