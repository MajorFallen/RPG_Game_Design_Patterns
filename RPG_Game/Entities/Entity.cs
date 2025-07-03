using ProOb_RPG.BattleManagers;
using ProOb_RPG.Effects;
using ProOb_RPG.GameModel;
using ProOb_RPG.GameOutput;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Entities
{
    internal abstract class Entity : RPGObject
    {
        public class EntityStats
        {
            public enum EntityStatsTypes { MaxHealth, Health, Strength, Dexterity, Luck, Aggression, Wisdom, Armor }

            public event Action? OnStatsChanged;
            public event Action? OnEntityDeath;
            private void NotifyChange() => OnStatsChanged?.Invoke();
            private void NotifyDeath() => OnEntityDeath?.Invoke();

            public Dictionary<EntityStatsTypes, int> statDictionary = new Dictionary<EntityStatsTypes, int>();
            public int MaxHealth
            {
                get { return statDictionary[EntityStatsTypes.MaxHealth]; }
                set
                {
                    statDictionary[EntityStatsTypes.MaxHealth] = value; NotifyChange();
                    if (value < Health)
                        Health = MaxHealth;
                }
            }
            public int Health
            {
                get { return (statDictionary.ContainsKey(EntityStatsTypes.Health) ? statDictionary[EntityStatsTypes.Health] : int.MinValue); }
                set { statDictionary[EntityStatsTypes.Health] = value; NotifyChange(); NotifyDeath(); }
            }
            public int Strength
            {
                get { return statDictionary[EntityStatsTypes.Strength]; }
                set { statDictionary[EntityStatsTypes.Strength] = value; NotifyChange(); }
            }
            public int Dexterity
            {
                get { return statDictionary[EntityStatsTypes.Dexterity]; }
                set { statDictionary[EntityStatsTypes.Dexterity] = value; NotifyChange(); }
            }
            public int Luck
            {
                get { return statDictionary[EntityStatsTypes.Luck]; }
                set { statDictionary[EntityStatsTypes.Luck] = value; NotifyChange(); }
            }
            public int Aggression
            {
                get { return statDictionary[EntityStatsTypes.Aggression]; }
                set { statDictionary[EntityStatsTypes.Aggression] = value; NotifyChange(); }
            }
            public int Wisdom
            {
                get { return statDictionary[EntityStatsTypes.Wisdom]; }
                set { statDictionary[EntityStatsTypes.Wisdom] = value; NotifyChange(); }
            }
            public int Armor
            {
                get { return statDictionary[EntityStatsTypes.Armor]; }
                set { statDictionary[EntityStatsTypes.Armor] = value; NotifyChange(); }
            }

            public EntityStats(int maxHealth, int strength, int dexterity, int luck, int aggression, int wisdom, int armor)
            {
                MaxHealth = maxHealth;
                Health = maxHealth;
                Strength = strength;
                Dexterity = dexterity;
                Luck = luck;
                Aggression = aggression;
                Wisdom = wisdom;
                Armor = armor;
                NotifyChange();
            }
            public EntityStats(EntityStats stats)
            {
                MaxHealth = stats.MaxHealth;
                Health = stats.Health;
                Strength = stats.Strength;
                Dexterity = stats.Dexterity;
                Luck = stats.Luck;
                Aggression = stats.Aggression;
                Wisdom = stats.Wisdom;
                Armor = stats.Armor;
                NotifyChange();
            }
        }
        public class EntityCoins
        {
            public Dictionary<CoinsClass.CoinTypes, int> coinDictionary = new Dictionary<CoinsClass.CoinTypes, int>();

            public event Action? OnCoinsChanged;
            private void NotifyChange() => OnCoinsChanged?.Invoke();

            public int Bronze
            {
                get { return coinDictionary[CoinsClass.CoinTypes.Bronze]; }
                set { coinDictionary[CoinsClass.CoinTypes.Bronze] = value; NotifyChange(); }
            }
            public int Silver
            {
                get { return coinDictionary[CoinsClass.CoinTypes.Silver]; }
                set { coinDictionary[CoinsClass.CoinTypes.Silver] = value; NotifyChange(); }
            }
            public int Gold
            {
                get { return coinDictionary[CoinsClass.CoinTypes.Gold]; }
                set { coinDictionary[CoinsClass.CoinTypes.Gold] = value; NotifyChange(); }
            }
            public EntityCoins(int bronze = 0, int silver = 0, int gold = 0)
            {
                Bronze = bronze;
                Silver = silver;
                Gold = gold;
                NotifyChange();
            }
            public EntityCoins(EntityCoins coins)
            {
                Bronze = coins.Bronze;
                Silver = coins.Silver;
                Gold = coins.Gold;
                NotifyChange();
            }
        }

        public event Action? onEntityDeath;

        public enum Hands { Left, Right }
        protected EntityEffectsMenager _effectsMenager;
        private EntityStats _modifiedStats;
        protected abstract string EntityName { get; }
        public EntityStats BaseStats { get; private set; }
        public virtual EntityStats ModifiedStats
        {
            get { return _effectsMenager.GetModifiedStats(new EntityStats(_modifiedStats)); }
            set { _modifiedStats = value; }
        }
        public virtual EntityCoins Coins { get; set; }
        protected List<IItem> inventory;
        protected IItem? rightHand;
        protected IItem? leftHand;
        public Entity(EntityStats stats, EntityCoins coins) : base()
        {
            BaseStats = new EntityStats(stats);
            _modifiedStats = new EntityStats(BaseStats);
            inventory = new List<IItem>();
            rightHand = null;
            leftHand = null;
            Coins = new EntityCoins(coins);

            

            _effectsMenager = new EntityEffectsMenager(this);

            _modifiedStats.OnEntityDeath += () => onEntityDeath?.Invoke();
        }
        public EntityStats GetModifiedStatsToChange()
        {
            return _modifiedStats;
        }
        public List<IItem> GetInventory()
        {
            return inventory;
        }
        public string GetEntityName()
        {
            return EntityName;
        }
        public virtual bool InflictDamage(int damage)
        {
            _modifiedStats.Health -= Math.Max(damage, 0);
            return true;
        }
        public virtual bool InflictEffect(EntityEffect effect)
        {
            _effectsMenager.AttachEffect(effect);
            return true;
        }
        public virtual bool DispelAllEffects()
        {
            _effectsMenager.DispelAllEffects();
            return true;
        }
        public override void MoveBy(Dungeon dungeon, int dy, int dx)
        {
            (int, int) newPosition = (Position.Item1 + dy, Position.Item2 + dx);
            (int, int) oldPosition = Position;

            if (dungeon.IsWalkable(newPosition))
            {
                Position = newPosition;
                dungeon.UpdateEntityPosition(this, oldPosition, newPosition);
            }

        }
        public virtual bool AttackEntity(Entity target, BattleManager.AttackTypes attackType)
        {
            BattleManager.AttackAction(this, target, attackType);
            return true;
        }
        public virtual bool RemoveItem(IItem item)
        {
            if (inventory.Remove(item))
                return true;
            if (leftHand == item)
                leftHand = null;
            else if (rightHand == item)
                rightHand = null;
            else
                return false;
            return true;
        }
        public virtual bool PickUpItem(Dungeon dungeon, int item_number = 0)
        {
            IItem? item = dungeon.GetTile(Position)!.GetTileItem(item_number);
            if (item == null || !item.PickUpItem(dungeon, this, item))
                return false;
            return true;
        }
        public virtual bool DropItemFromInventory(Dungeon dungeon, int whichItemInInventory)
        {
            if (whichItemInInventory >= inventory.Count)
                return false;
            IItem item = inventory[whichItemInInventory];
            return item.DropItem(dungeon, this, item);
        }
        public virtual bool DropAllItemsFromInventory(Dungeon dungeon)
        {
            IItem item;
            bool isAllItemsDroped = true;
            for (int i = inventory.Count - 1; i >= 0; i--)
            {
                item = inventory[i];
                if (!item.DropItem(dungeon, this, item))
                    isAllItemsDroped = false;
            }
            return isAllItemsDroped;
        }
        public virtual bool DropItemFromHand(Dungeon dungeon, Hands hand)
        {
            IItem? item = hand == Hands.Left ? leftHand : rightHand;
            if (item == null)
                return true;
            if (!UnEquipItemInHand(hand))
                return false;
            int x = inventory.IndexOf(item);
            return DropItemFromInventory(dungeon, x);
        }
        public virtual void AddToInventory(IItem item)
        {
            inventory.Add(item);
        }
        public virtual IItem? RemoveItemFromInventory(IItem item)
        {
            inventory.Remove(item);
            return item;
        }
        public virtual bool EquipItemInHand(Hands hand, int whichItemInInventory)
        {
            if (whichItemInInventory >= inventory.Count)
                return false;

            return inventory[whichItemInInventory].EquipItemInHand(this, inventory[whichItemInInventory], hand);
        }
        public virtual IItem? AddItemToLeftHand(IItem item)
        {
            return leftHand = item;
        }
        public virtual IItem? AddItemToRightHand(IItem item)
        {
            return rightHand = item;
        }

        public virtual bool UnEquipItemInHand(Hands hand)
        {
            if (hand == Hands.Left && leftHand != null)
                return leftHand.UnEquipItemInHand(this, hand);
            if (hand == Hands.Right && rightHand != null)
                return rightHand.UnEquipItemInHand(this, hand);
            else return true;
        }
        public virtual IItem? RemoveItemFromLeftHand()
        {
            IItem? item = leftHand;
            leftHand = null;
            return item;
        }
        public virtual IItem? RemoveItemFromRightHand()
        {
            IItem? item = rightHand;
            rightHand = null;
            return item;
        }
        public virtual IItem? GetItemFromLeftHand()
        {
            return leftHand;
        }
        public virtual IItem? GetItemFromRightHand()
        {
            return rightHand;
        }
        public virtual bool UseItem(Hands hand)
        {
            if (hand == Hands.Left && leftHand != null)
                return leftHand.UseItem(this);
            if (hand == Hands.Right && rightHand != null)
                return rightHand.UseItem(this);
            else return false;
        }

    }

    


}
