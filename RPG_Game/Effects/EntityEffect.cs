using ProOb_RPG.GameModel;
using ProOb_RPG.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Effects
{
    internal abstract class EntityEffect : ITurnObserver
    {
        public abstract string EffectName { get; }
        public abstract string[] EffectDescription { get; }
        public int Duration { get; private set; }
        private EntityEffectsMenager? _host;

        public EntityEffect(int duration = -1)
        {
            Duration = duration;
            ModelGameSystem.GetInstance()._turnMenager.AttachTurnObserver(this);
        }
        public void SetHost(EntityEffectsMenager host)
        {
            _host = host;
        }

        public virtual void UpdateTurn(ITurnMenager turnMenager)
        {
            if (Duration == -1)
                return;
            Duration--;
            if (Duration == 0)
                DeleteEffect();
        }

        public virtual void DeleteEffect()
        {
            ModelGameSystem.GetInstance()._turnMenager.DetachTurnObserver(this);
            _host!.DetachEffect(this);
        }

        public virtual void ModifyEntityStats(Entity.EntityStats stats)
        {
            return; 
        }

    }
}
