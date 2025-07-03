using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG.Items
{
    internal class ItemRingOfPower : Item
    {
        private static int MaxHealthBonus => 20;
        private static int StrengthBonus => 3;
        protected override string ItemName => "Ring of Power";
        public ItemRingOfPower() : base() { }
        public override bool PickUpItem(Dungeon dungeon, Entity entity, IItem item)
        {
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.MaxHealth += MaxHealthBonus;
            stats.Strength += StrengthBonus;
            entity.AddToInventory(item);
            dungeon.RemoveItem(item);
            return true;
        }
        public override bool DropItem(Dungeon dungeon, Entity entity, IItem item)
        {
            if (!base.DropItem(dungeon, entity, item))
                return false;
            EntityStats stats = entity.GetModifiedStatsToChange();
            stats.MaxHealth -= MaxHealthBonus;
            stats.Strength -= StrengthBonus;
            return true;
        }
        public void WhenActionTaken(Entity entity)
        {
            //zabiera życie
        }
    }
    internal class ItemAmogus : Item
    {
        protected override string ItemName => "Amogus";
        public ItemAmogus() : base() { }
    }
    internal class ItemKremowka : Item
    {
        protected override string ItemName => "Kremowka";
        public ItemKremowka() : base() { }
    }
}
