using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Entities
{
    internal class EnemyOrc : Entity
    {
        protected override string EntityName => "Orc";
        public EnemyOrc() : base(new EntityStats(200,20,6,8,20,4,8), new EntityCoins()) 
        {
            IItem item = new WeaponLongSword();
            this.AddItemToLeftHand(item);
        }

    }
}
