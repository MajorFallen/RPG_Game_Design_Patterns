using ProOb_RPG.Entities;
using ProOb_RPG.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Effects
{
    class EntityEffectsMenager
    {
        Entity _entity;
        private List<EntityEffect> _effects = new List<EntityEffect>();
        private Entity entity;

        

        public EntityEffectsMenager(Entity entity)
        {
            _entity = entity;
        }

        public List<EntityEffect> GetEffects()
        {
            return _effects;
        }
        public void AttachEffect(EntityEffect effect)
        {
            _effects.Add(effect);
            effect.SetHost(this);
        }

        public void DetachEffect(EntityEffect effect)
        {
            _effects.Remove(effect);
        }
        public void DispelAllEffects()
        {
            List<EntityEffect> toDetach = new List<EntityEffect>(_effects);
            foreach (EntityEffect effect in toDetach) 
            {
                effect.DeleteEffect();
            }
            toDetach.Clear();
        }
        public Entity.EntityStats GetModifiedStats(Entity.EntityStats stats)
        {
            foreach (EntityEffect effect in _effects)
            {
                effect.ModifyEntityStats(stats);
            }
            return stats;
        }
    }
}
