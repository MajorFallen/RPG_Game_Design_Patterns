using ProOb_RPG.Entities;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.BattleManagers
{
    internal interface IItemVisitor
    {
        public (int,int) VisitNonWeaponItem(IItem item);
        public (int, int) VisitStrengthBasedWeapon(IItem item);
        public (int, int) VisitDexterityBasedWeapon(IItem item);
        public (int, int) VisitMagicBasedWeapon(IItem item);
    }

    internal abstract class AbstractWeaponVisitor : IItemVisitor
    {
        protected Entity _entity;
        protected Entity.EntityStats stats;
        public AbstractWeaponVisitor(Entity entity)
        {
            _entity = entity;
            stats = _entity.ModifiedStats;
        }

        public abstract (int, int) VisitDexterityBasedWeapon(IItem item);
        public abstract (int, int) VisitMagicBasedWeapon(IItem item);
        public abstract (int, int) VisitStrengthBasedWeapon(IItem item);
        public virtual (int, int) VisitNonWeaponItem(IItem item)
        {
            return (0, 0);
        }
    }

    internal class BasicAttackWeaponVisitor : AbstractWeaponVisitor
    {
        public BasicAttackWeaponVisitor(Entity entity) : base(entity) { }
        public override (int, int) VisitStrengthBasedWeapon(IItem item)
        {
            return (item.GetItemDamage() + stats.Strength + stats.Aggression, stats.Armor + stats.Strength + stats.Luck);
        }
        public override (int, int) VisitDexterityBasedWeapon(IItem item)
        {
            return (item.GetItemDamage() + stats.Dexterity + stats.Luck, stats.Armor + stats.Dexterity + stats.Luck);
        }

        public override (int, int) VisitMagicBasedWeapon(IItem item)
        {
            return (1, stats.Armor + stats.Dexterity + stats.Luck);
        }

        public override (int, int) VisitNonWeaponItem(IItem item)
        {
            (int, int) x = base.VisitNonWeaponItem(item);
            x.Item2 += (stats.Armor + stats.Dexterity);
            return x;
        }
    }

    internal class SneakAttackWeaponVisitor : AbstractWeaponVisitor
    {
        public SneakAttackWeaponVisitor(Entity entity) : base(entity) { }
        public override (int, int) VisitStrengthBasedWeapon(IItem item)
        {
            return ((item.GetItemDamage() + stats.Strength + stats.Aggression)/2, stats.Armor + stats.Strength);
        }
        public override (int, int) VisitDexterityBasedWeapon(IItem item)
        {
            return ((item.GetItemDamage() + stats.Dexterity + stats.Luck)*2, stats.Armor + stats.Dexterity);
        }

        public override (int, int) VisitMagicBasedWeapon(IItem item)
        {
            return (1, stats.Armor);
        }

        public override (int, int) VisitNonWeaponItem(IItem item)
        {
            (int, int) x = base.VisitNonWeaponItem(item);
            x.Item2 += stats.Armor;
            return x;
        }
    }

    internal class MagicAttackWeaponVisitor : AbstractWeaponVisitor
    {
        public MagicAttackWeaponVisitor(Entity entity) : base(entity) { }
        public override (int, int) VisitStrengthBasedWeapon(IItem item)
        {
            return (1, stats.Armor + stats.Luck);
        }
        public override (int, int) VisitDexterityBasedWeapon(IItem item)
        {
            return (1, stats.Armor + stats.Luck);
        }

        public override (int, int) VisitMagicBasedWeapon(IItem item)
        {
            return (item.GetItemDamage() + stats.Wisdom*2, stats.Armor + stats.Wisdom*2);
        }

        public override (int, int) VisitNonWeaponItem(IItem item)
        {
            (int, int) x = base.VisitNonWeaponItem(item);
            x.Item2 += (stats.Armor + stats.Luck);
            return x;
        }
    }
}
