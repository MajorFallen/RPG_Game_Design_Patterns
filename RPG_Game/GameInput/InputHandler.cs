using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;

namespace ProOb_RPG.GameInput
{
    internal partial class InputGameSystem
    {
        internal interface IInputHandler
        {
            IInputHandler SetNext(IInputHandler handler);

            bool Handle(InputGameSystem system, ConsoleKey key);

            List<StringBuilder> CreateLegend(InputGameSystem system);
        }

        internal abstract class AbstractInputHandler : IInputHandler
        {
            private IInputHandler? _nextHandler;


            public virtual bool Handle(InputGameSystem system, ConsoleKey key)
            {
                if (_nextHandler != null)
                {
                    return _nextHandler.Handle(system, key);
                }
                return false;
            }

            public IInputHandler SetNext(IInputHandler handler)
            {
                _nextHandler = handler;
                return handler;
            }

            public virtual List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                if (_nextHandler != null)
                {
                    return _nextHandler.CreateLegend(system);
                }
                return new List<StringBuilder>();
            }

            protected virtual void NotImplementedkeyBind(InputGameSystem system, ConsoleKey key)
            {
                system.OnMessageThrown?.Invoke(new StringBuilder($"Key ({key}) is not binded to any possible action."));
            }
        }

        internal class MovementInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                var action = system._keyConfig.GetMapping(key);
                if (action != null)
                {
                    switch (action)
                    {
                        case KeyConfig.KeyMapping.MoveUp:
                            system._model.PlayerMoveUp();
                            return true;
                        case KeyConfig.KeyMapping.MoveDown:
                            system._model.PlayerMoveDown();
                            return true;
                        case KeyConfig.KeyMapping.MoveLeft:
                            system._model.PlayerMoveLeft();
                            return true;
                        case KeyConfig.KeyMapping.MoveRight:
                            system._model.PlayerMoveRight();
                            return true;
                    }
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                StringBuilder x = new StringBuilder();
                foreach(KeyConfig.KeyMapping action in new KeyConfig.KeyMapping[] { KeyConfig.KeyMapping.MoveUp, KeyConfig.KeyMapping.MoveDown, KeyConfig.KeyMapping.MoveLeft, KeyConfig.KeyMapping.MoveRight})
                {
                    x.Append(system._keyConfig.GetKey(action));
                }
                list.Add(new StringBuilder($"({x}) - movement"));
                return list;
            }
        }

        internal class ItemPickUpInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                var action = system._keyConfig.GetMapping(key);
                if (action != null && action == KeyConfig.KeyMapping.PickUpMode)
                {
                    ConsoleKey secondKey;
                    while (true)
                    {
                        system.OnLegendSet?.Invoke(new List<StringBuilder>()
                        {
                            new StringBuilder($"(0-9) - Pick Up Item"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Quit Pick up Mode"),
                        });
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction == KeyConfig.KeyMapping.QuitFromMode)
                            break;
                        else if (secondKey >= ConsoleKey.D0 && secondKey <= ConsoleKey.D9)
                        {
                            system._model.PlayerPickUpItem(secondKey - ConsoleKey.D0);
                        }
                        else
                            NotImplementedkeyBind(system, secondKey);
                    }
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.PickUpMode)}) - Enter Pickup Mode"));
                return list;
            }
        }

        internal class ItemDropInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                var action = system._keyConfig.GetMapping(key);
                if (action != null && action == KeyConfig.KeyMapping.DropMode)
                {
                    ConsoleKey secondKey;
                    while (true)
                    {
                        system.OnLegendSet?.Invoke(new List<StringBuilder>()
                        {
                            new StringBuilder($"(0-9) - Drop Item From Inventory"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.LeftHand)}) - Drop Item From Left hand"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.RightHand)}) - Drop Item From Right hand"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.Inventory)}) - Drop All Items in Your Inventory"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Quit Drop Mode"),
                        });
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction == KeyConfig.KeyMapping.QuitFromMode)
                            break;
                        if (secondKey >= ConsoleKey.D0 && secondKey <= ConsoleKey.D9)
                        {
                            system._model.PlayerDropItemFromInventory(secondKey - ConsoleKey.D0);
                        }
                        else if (secondAction == KeyConfig.KeyMapping.LeftHand)
                        {
                            system._model.PlayerDropItemFromHand(Entity.Hands.Left);
                        }
                        else if (secondAction == KeyConfig.KeyMapping.RightHand)
                        {
                            system._model.PlayerDropItemFromHand(Entity.Hands.Right);
                        }
                        else if (secondAction == KeyConfig.KeyMapping.Inventory)
                        {
                            system._model.PlayerDropAllItemsFromInventory();
                        }
                        else
                            NotImplementedkeyBind(system, secondKey);
                    }
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.DropMode)}) - Enter Drop Mode"));
                return list;
            }
        }

        internal class ItemInInventoryManagementInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                ConsoleKey secondKey;
                if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
                {
                    while (true)
                    {
                        system.OnLegendSet?.Invoke(new List<StringBuilder>()
                        {
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.DropMode)}) - Drop Selected Item"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.LeftHand)}) - Equip Selected Item to Left Hand"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.RightHand)}) - Equip Selected Item to Right Hand"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Unselect Item"),
                        });
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction != null)
                        {
                            if (secondAction == KeyConfig.KeyMapping.QuitFromMode)
                                break;
                            if (secondAction == KeyConfig.KeyMapping.DropMode)
                            {
                                system._model.PlayerDropItemFromInventory(key - ConsoleKey.D0);
                                break;
                            }
                            if (secondAction == KeyConfig.KeyMapping.LeftHand)
                            {
                                system._model.PlayerEquipItemFromInventory(key - ConsoleKey.D0, Entity.Hands.Left);
                                break;
                            }
                            if (secondAction == KeyConfig.KeyMapping.RightHand)
                            {
                                system._model.PlayerEquipItemFromInventory(key - ConsoleKey.D0, Entity.Hands.Right);
                                break;
                            }
                        }
                        NotImplementedkeyBind(system, secondKey);
                    }
                    return true;
                }
                var action = system._keyConfig.GetMapping(key);
                if (action != null && (action == KeyConfig.KeyMapping.LeftHand || action == KeyConfig.KeyMapping.RightHand))
                {
                    Entity.Hands hand = (action == KeyConfig.KeyMapping.LeftHand ? Entity.Hands.Left : Entity.Hands.Right);
                    while (true)
                    {
                        system.OnLegendSet?.Invoke(new List<StringBuilder>()
                        {
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.DropMode)}) - Drop Selected Item"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.Inventory)}) - Unequip Selected Item"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.UseItem)}) - Use Selected Item"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Unselect Item"),
                        });
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction != null)
                        {
                            if (secondAction == KeyConfig.KeyMapping.QuitFromMode)
                                break;
                            if (secondAction == KeyConfig.KeyMapping.DropMode)
                            {
                                system._model.PlayerDropItemFromHand(hand);
                                break;
                            }
                            if (secondAction == KeyConfig.KeyMapping.Inventory)
                            {
                                system._model.PlayerunEquipItemFromHand(hand);
                                break;
                            }
                            if (secondAction == KeyConfig.KeyMapping.UseItem)
                            {
                                system._model.UseItemInHand(hand);
                                break;
                            }
                        }
                        NotImplementedkeyBind(system, secondKey);
                    }
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"(0-9) - Manage Item in Inventory"));
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.LeftHand)}) - Manage Item in Left Hand"));
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.RightHand)}) - Manage Item in Right Hand"));
                return list;
            }
        }
        internal class AttackEntityInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                ConsoleKey secondKey;
                var action = system._keyConfig.GetMapping(key);
                if (action != null && action == KeyConfig.KeyMapping.AttackEntity)
                {
                    ConsoleKey thirdKey;
                    while (true)
                    {
                        system.OnLegendSet?.Invoke(new List<StringBuilder>()
                        {
                            new StringBuilder($"(0-9) - Choose Target"),
                            new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Quit Fight Mod"),
                        });
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction != null && secondAction == KeyConfig.KeyMapping.QuitFromMode)
                        {
                            break;
                        }
                        if (secondKey >= ConsoleKey.D0 && secondKey <= ConsoleKey.D9)
                        {
                            while (true)
                            {
                                system.OnLegendSet?.Invoke(new List<StringBuilder>()
                                {
                                    new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.BasicAttack)}) - Basic Attack"),
                                    new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.SneakAttack)}) - Sneak Attack"),
                                    new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.MagicAttack)}) - Magic Attack"),
                                    new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitFromMode)}) - Quit Fight Mod"),
                                });
                                thirdKey = Console.ReadKey(true).Key;
                                var thirdAction = system._keyConfig.GetMapping(thirdKey);
                                if (thirdAction != null)
                                {
                                    if (thirdAction == KeyConfig.KeyMapping.QuitFromMode)
                                        break;
                                    if (thirdAction == KeyConfig.KeyMapping.BasicAttack)
                                    {
                                        system._model.AttackEntity(secondKey - ConsoleKey.D0, BattleManagers.BattleManager.AttackTypes.Basic);
                                        break;
                                    }
                                    if (thirdAction == KeyConfig.KeyMapping.SneakAttack)
                                    {
                                        system._model.AttackEntity(secondKey - ConsoleKey.D0, BattleManagers.BattleManager.AttackTypes.Sneak);
                                        break;
                                    }
                                    if (thirdAction == KeyConfig.KeyMapping.MagicAttack)
                                    {
                                        system._model.AttackEntity(secondKey - ConsoleKey.D0, BattleManagers.BattleManager.AttackTypes.Magic);
                                        break;
                                    }
                                }
                                NotImplementedkeyBind(system, thirdKey);
                            }
                        }
                        NotImplementedkeyBind(system, secondKey);
                    }
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.AttackEntity)}) - Enter Fight Mode"));
                return list;
            }
        }

        internal class OpenInstructionInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                var action = system._keyConfig.GetMapping(key);
                if (action != null && action == KeyConfig.KeyMapping.Instruction)
                {
                    system.OpenInstrucion?.Invoke();
                    ConsoleKey secondKey;
                    while (true)
                    {
                        secondKey = Console.ReadKey(true).Key;
                        var secondAction = system._keyConfig.GetMapping(secondKey);
                        if (secondAction == KeyConfig.KeyMapping.Instruction || secondAction == KeyConfig.KeyMapping.QuitFromMode)
                        {
                            break;
                        }
                    }
                    system.CloseInstrucion?.Invoke();
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.Instruction)}) - Open Game Instruction"));
                return list;
            }
        }

        internal class GameQuitInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                var action = system._keyConfig.GetMapping(key);
                if (action != null && action == KeyConfig.KeyMapping.QuitGame)
                {
                    system._model.EndProgram();
                    return true;
                }
                return base.Handle(system, key);
            }
            public override List<StringBuilder> CreateLegend(InputGameSystem system)
            {
                List<StringBuilder> list = base.CreateLegend(system);
                list.Add(new StringBuilder($"({system._keyConfig.GetKey(KeyConfig.KeyMapping.QuitGame)}) - Quit Game"));
                return list;
            }
        }

        internal class NotImplementedInputHandler : AbstractInputHandler
        {
            public override bool Handle(InputGameSystem system, ConsoleKey key)
            {
                NotImplementedkeyBind(system, key);
                return base.Handle(system, key);
            }
        }
    }
}
