using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.Items.CoinsClass;
using ProOb_RPG.Entities;

namespace ProOb_RPG.Items
{
    internal class CoinsClass : Item
    {
        protected override string ItemName => $"{Amount} {CoinType} Coins";
        public enum CoinTypes { Bronze, Silver, Gold }

        private CoinTypes CoinType { get; set; }

        private int Amount { get; set; }
        public CoinsClass(CoinTypes coinType, int amount) : base()
        {
            CoinType = coinType;
            Amount = amount;
        }
        public override bool PickUpItem(Dungeon dungeon, Entity entity, IItem item)
        {
            switch(CoinType)
            {
                case CoinTypes.Gold:
                    entity.Coins.Gold += Amount;
                    break;
                case CoinTypes.Silver:
                    entity.Coins.Silver += Amount;
                    break;
                case CoinTypes.Bronze:
                    entity.Coins.Bronze += Amount;
                    break;
            }
            dungeon.RemoveItem(item);
            return true;
        }
    }
}
