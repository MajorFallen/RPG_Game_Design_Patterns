using ProOb_RPG.BattleManagers;
using ProOb_RPG.Entities;

namespace ProOb_RPG.Items
{
    internal interface IItem : IRPGObject
    {
        public string GetItemName();

        public bool PickUpItem(Dungeon dungeon, Entity entity, IItem item);
        public bool DropItem(Dungeon dungeon, Entity entity, IItem item);
        public bool EquipItemInHand(Entity entity, IItem item, Entity.Hands hand);
        public bool UnEquipItemInHand(Entity entity, Entity.Hands hand);
        public bool UseItem(Entity entity);
        public bool DestroyItemInEntity(Entity entity);
        public bool InternalDestroyItemInEntity(IItem item, Entity entity);
        public int GetItemDamage();
        public (int, int) AcceptVisitor(IItemVisitor visitor);
        public (int, int) InternalAcceptVisitor(IItem item, IItemVisitor visitor);
    }

    internal abstract class Item : RPGObject, IItem
    {
        protected abstract string ItemName { get; }
        public Item() : base(){}
        public string GetItemName()
        {
            return ItemName;
        }
        public override void MoveBy(Dungeon dungeon, int dy, int dx)
        {
            (int y, int x) newPosition = (Position.Item1 + dy, Position.Item2 + dx);

            if (dungeon.IsWalkable(newPosition))
            {
                dungeon.UpdateItemPosition(this, Position, newPosition);
                Position = newPosition;
            }
        }
        public virtual bool PickUpItem(Dungeon dungeon, Entity entity, IItem item)
        {
            entity.AddToInventory(item);
            dungeon.RemoveItem(item);
            return true;
        }
        public virtual bool DropItem(Dungeon dungeon, Entity entity, IItem item)
        {
            if (!dungeon.AddItem(item, entity.GetPosition()))
                return false;
            entity.RemoveItemFromInventory(item);
            return true;
        }
        public virtual bool EquipItemInHand(Entity entity, IItem item, Entity.Hands hand)
        {
            if (!entity.UnEquipItemInHand(hand))
                return false;
            entity.RemoveItemFromInventory(item);
            if (hand == Entity.Hands.Left)
                entity.AddItemToLeftHand(item);
            else
                entity.AddItemToRightHand(item);
            return true;
        }
        public virtual bool UnEquipItemInHand(Entity entity, Entity.Hands hand)
        {
            IItem? item;
            if (hand == Entity.Hands.Left)
                item = entity.RemoveItemFromLeftHand();
            else
                item = entity.RemoveItemFromRightHand();
            if (item != null) 
                entity.AddToInventory(item);
            return true;
        }
        public virtual int GetItemDamage()
        {
            return 0;
        }
        public virtual bool UseItem(Entity entity)
        {
            return false;
        }
        public virtual bool DestroyItemInEntity(Entity entity)
        {
            return InternalDestroyItemInEntity(this, entity);
        }
        public virtual bool InternalDestroyItemInEntity(IItem item, Entity entity)
        {
            return entity.RemoveItem(item);
        }

        public (int, int) AcceptVisitor(IItemVisitor visitor)
        {
            return InternalAcceptVisitor(this, visitor);
        }

        public virtual (int, int) InternalAcceptVisitor(IItem item, IItemVisitor visitor)
        {
            return visitor.VisitNonWeaponItem(item);
        }
    }

    internal abstract class Weapon : Item
    {
        protected abstract int Damage { get; }
        protected abstract int WeaponMaxHealth { get; }
        protected IWeaponBehaviorStrategy Behavior { get; private set; } 
        protected int WeaponHealth { get; set; }
        public Weapon(IWeaponBehaviorStrategy behavior) : base() 
        {
            WeaponHealth = WeaponMaxHealth;
            Behavior = behavior;
        }
        public override int GetItemDamage()
        {
            return Damage;
        }
        public override (int, int) InternalAcceptVisitor(IItem item, IItemVisitor visitor)
        {
            return Behavior.AcceptWeaponVisitor(item, visitor);
        }
    }

    internal abstract class TwoHandedWeapon : Weapon
    {
        public TwoHandedWeapon(IWeaponBehaviorStrategy behavior) : base(behavior) { }
        public override bool EquipItemInHand(Entity entity, IItem item, Entity.Hands hand)
        {
            if(!entity.UnEquipItemInHand(Entity.Hands.Left) || !entity.UnEquipItemInHand(Entity.Hands.Right))
                return false;
            entity.RemoveItemFromInventory(item);
            entity.AddItemToLeftHand(item);
            entity.AddItemToRightHand(item);
            return true;
        }
        public override bool UnEquipItemInHand(Entity entity, Entity.Hands hand)
        {
            IItem? item;
            item = entity.RemoveItemFromLeftHand();
            entity.RemoveItemFromRightHand();
            if (item != null)
                entity.AddToInventory(item);
            return true;
        }
    }

    internal abstract class WeaponDecorator : IItem
    {
        protected IItem Wrappee { get; set; }
        protected abstract string EffectName { get; }
        public static int _maxEfffectLevel = 5;
        private int _effectLevel;
        protected int EffectLevel
        {
            get { return _effectLevel; }
            set
            {
                _effectLevel = Math.Min(value, 5);
                switch (EffectLevel)
                {
                    case 0:
                        EffectLevelString = "";
                        break;
                    case 1:
                        EffectLevelString = " I";
                        break;
                    case 2:
                        EffectLevelString = " II";
                        break;
                    case 3:
                        EffectLevelString = " III";
                        break;
                    case 4:
                        EffectLevelString = " IV";
                        break;
                    case 5:
                        EffectLevelString = " V";
                        break;
                }
            }
        }
        protected string? EffectLevelString { get; set; }
        protected WeaponDecorator(IItem wrappee, int effectLevel = 0)
        {
            Wrappee = wrappee;
            EffectLevel = effectLevel;
        }

        public string GetItemName()
        {
            return Wrappee.GetItemName() + $" ({EffectName}{EffectLevelString})";
        }
        public virtual void MoveBy(Dungeon dungeon, int dy, int dx)
        {
            Wrappee.MoveBy(dungeon, dy, dx);
        }
        public (int, int) GetPosition()
        {
            return Wrappee.GetPosition();
        }
        public void SetPosition((int, int) position)
        {
            Wrappee.SetPosition(position);
        }
        public virtual bool PickUpItem(Dungeon dungeon, Entity entity, IItem item)
        {
            return Wrappee.PickUpItem(dungeon, entity, item);
        }
        public virtual bool DropItem(Dungeon dungeon, Entity entity, IItem item)
        {
            return Wrappee.DropItem(dungeon, entity, item);    
        }
        public virtual bool EquipItemInHand(Entity entity, IItem item, Entity.Hands hand)
        {
            return Wrappee.EquipItemInHand(entity, item, hand);
        }
        public virtual bool UnEquipItemInHand(Entity entity, Entity.Hands hand)
        {
            return Wrappee.UnEquipItemInHand(entity, hand);
        }
        public virtual int GetItemDamage()
        {
            return Wrappee.GetItemDamage();
        }

        public virtual bool UseItem(Entity entity)
        {
            return Wrappee.UseItem(entity);
        }
        public virtual bool DestroyItemInEntity(Entity entity)
        {
            return Wrappee.InternalDestroyItemInEntity(this, entity);
        }
        public virtual bool InternalDestroyItemInEntity(IItem item, Entity entity) 
        {
            return Wrappee.InternalDestroyItemInEntity(item, entity);
        }

        public (int,int) AcceptVisitor(IItemVisitor visitor)
        {
            return Wrappee.InternalAcceptVisitor(this, visitor);
        }

        public (int, int) InternalAcceptVisitor(IItem item, IItemVisitor visitor)
        {
            return Wrappee.InternalAcceptVisitor(item, visitor);
        }
    }

}
