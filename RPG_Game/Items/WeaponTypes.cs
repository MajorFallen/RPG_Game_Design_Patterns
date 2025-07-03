using ProOb_RPG.BattleManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.Items
{
    internal interface IWeaponBehaviorStrategy
    {
        public (int, int) AcceptWeaponVisitor(IItem item, IItemVisitor visitor);
    }
    internal abstract class AbstractWeaponBehaviorStrategy : IWeaponBehaviorStrategy
    {
        public abstract (int, int) AcceptWeaponVisitor(IItem item, IItemVisitor visitor);
    }
    internal class WeaponBehaviorStrengthBased : AbstractWeaponBehaviorStrategy
    {
        public override (int, int) AcceptWeaponVisitor(IItem item, IItemVisitor visitor)
        {
            return visitor.VisitStrengthBasedWeapon(item);
        }
    }
    internal class WeaponBehaviorDexterityBased : AbstractWeaponBehaviorStrategy
    {
        public override (int, int) AcceptWeaponVisitor(IItem item, IItemVisitor visitor)
        {
            return visitor.VisitDexterityBasedWeapon(item);
        }
    }
    internal class WeaponBehaviorMagicBased : AbstractWeaponBehaviorStrategy
    {
        public override (int, int) AcceptWeaponVisitor(IItem item, IItemVisitor visitor)
        {
            return visitor.VisitMagicBasedWeapon(item);
        }
    }
    internal static class WeaponTags
    {
        public enum Behavior {Strength, Dexterity, Magic}
        public static Dictionary<Behavior, IWeaponBehaviorStrategy> BehaviorDictionary;
        static WeaponTags()
        {
            BehaviorDictionary = new Dictionary<Behavior, IWeaponBehaviorStrategy>
            {
                { Behavior.Strength, new WeaponBehaviorStrengthBased() },
                { Behavior.Dexterity, new WeaponBehaviorDexterityBased() },
                { Behavior.Magic, new WeaponBehaviorMagicBased() }
            };
        }
    }
}
