using ProOb_RPG.Entities;
using ProOb_RPG.GameModel;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.BattleManagers
{
    internal static class BattleManager
    {
        public enum AttackTypes { Basic, Sneak, Magic}
        private static (int,int,int) CalculateAttack(Entity attacker, AbstractWeaponVisitor visitor)
        { 
            IItem? leftItem = attacker.GetItemFromLeftHand(), rightItem = attacker.GetItemFromRightHand();
            int leftDMG = 0, rightDMG = 0;
            int leftDef = 0, rightDef = 0;
            if (leftItem != null)
                (leftDMG, leftDef) = leftItem.AcceptVisitor(visitor);
            if (rightItem != null)
            {
                (rightDMG, rightDef) = rightItem.AcceptVisitor(visitor);
                if (rightItem == leftItem)
                    rightDMG = 0;
            }
            return (leftDMG, rightDMG, leftDef + rightDef);
        }
        public static void AttackAction(Entity attacker, Entity defender, AttackTypes attackType)
        {
            AbstractWeaponVisitor visitor;
            switch (attackType)
            {
                case AttackTypes.Basic:
                    {
                        visitor = new BasicAttackWeaponVisitor(attacker);
                        break;
                    }
                case AttackTypes.Sneak:
                    {
                        visitor = new BasicAttackWeaponVisitor(attacker);
                        break;
                    }
                case AttackTypes.Magic:
                    {
                        visitor = new BasicAttackWeaponVisitor(attacker);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), $"Unsupported attack type: {attackType}");
            }

            (int AttDMG1, int AttDMG2, int AttDef) = CalculateAttack(attacker, visitor);
            (int DefDMG1, int DefDMG2, int DefDef) = CalculateAttack(defender, new BasicAttackWeaponVisitor(defender));
            defender.InflictDamage(AttDMG1 - DefDef);
            defender.InflictDamage(AttDMG2 - DefDef);
            if (defender.ModifiedStats.Health > 0) 
            {
                attacker.InflictDamage(DefDMG1 - AttDef);
                attacker.InflictDamage(DefDMG2 - AttDef);
            }
        }
    }
}
