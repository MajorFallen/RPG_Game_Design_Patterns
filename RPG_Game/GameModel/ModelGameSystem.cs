using ProOb_RPG.GameOutput;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;
using ProOb_RPG.BattleManagers;

namespace ProOb_RPG.GameModel
{
    internal sealed partial class ModelGameSystem
    {
        private static ModelGameSystem? _instance;

        private bool isRunning = true;

        private static int _height = 19;
        private static int _width = 39;
        private Dungeon _dungeon;

        private DungeonDirector _director;

        public TurnMenager _turnMenager;
        

        public event Action? RequestInput;

        public event Action? OnPlayerAction;

        private ModelGameSystem()
        {
            _turnMenager = new TurnMenager();
        }
        public static ModelGameSystem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ModelGameSystem();
                _instance.Initialize();

            }
            return _instance;
        }
        private void Initialize()
        {
            DungeonBuilder dungeonBuilder = new DungeonBuilder(_height, _width);

            playerPort = new PlayerPort();

            _director = new DungeonDirector();

            UseBuilder(dungeonBuilder);

            _dungeon = dungeonBuilder.GetResult();
        }
        public void UseBuilder(IDungeonBuilder builder)
        {
            _director.ConstructComplexDungeon(builder);
        }
        public void Run()
        {
            Console.CursorVisible = false;
            while (isRunning)
            {
                RequestInput?.Invoke();
            }
        }
        public void EndProgram()
        {
            Environment.Exit(0);
        }


        public void PlayerMoveUp()
        {
            _dungeon.Player.MoveBy(_dungeon, -1, 0);
            _turnMenager.NextTurn();
            OnPlayerAction?.Invoke();
        }
        public void PlayerMoveDown()
        {
            _dungeon.Player.MoveBy(_dungeon, 1, 0);
            _turnMenager.NextTurn();
            OnPlayerAction?.Invoke();
        }
        public void PlayerMoveLeft()
        {
            _dungeon.Player.MoveBy(_dungeon, 0, -1);
            _turnMenager.NextTurn();
            OnPlayerAction?.Invoke();
        }
        public void PlayerMoveRight()
        {
            _dungeon.Player.MoveBy(_dungeon, 0, 1);
            _turnMenager.NextTurn();
            OnPlayerAction?.Invoke();
        }
        public bool PlayerPickUpItem(int x)
        {
            bool result = _dungeon.Player.PickUpItem(_dungeon, x);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool PlayerDropItemFromInventory(int x)
        {
            bool result = _dungeon.Player.DropItemFromInventory(_dungeon, x);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool PlayerDropAllItemsFromInventory()
        {
            bool result = _dungeon.Player.DropAllItemsFromInventory(_dungeon);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool PlayerDropItemFromHand(Entity.Hands hand)
        {
            bool result = _dungeon.Player.DropItemFromHand(_dungeon, hand);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool PlayerEquipItemFromInventory(int x, Entity.Hands hand)
        {
            bool result = _dungeon.Player.EquipItemInHand(hand, x);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool PlayerunEquipItemFromHand(Entity.Hands hand)
        {
            bool result = _dungeon.Player.UnEquipItemInHand(hand);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool SelectItemFromInventory(int x)
        {
            return true;
        }
        public bool SelectItemFromHand(Entity.Hands hand)
        {
            return true;
        }
        public bool UseItemInHand(Entity.Hands hand)
        {
            bool result = _dungeon.Player.UseItem(hand);
            OnPlayerAction?.Invoke();
            return result;
        }
        public bool AttackEntity(int whichEntity, BattleManager.AttackTypes attackType)
        {
            Entity? target = _dungeon.GetTile(_dungeon.Player.GetPosition())!.GetTileEntity(whichEntity);
            if (target == null || !_dungeon.Player.AttackEntity(target, attackType))
                return false;
            return true;
        }


        //private void HandleInput()
        //{
        //    ConsoleKey key = Console.ReadKey(true).Key;
        //    ConsoleKey secondKey;
        //    switch (key)
        //    {
        //        case ConsoleKey.W: _dungeon.Player.MoveBy(_dungeon, -1, 0); break;
        //        case ConsoleKey.S: _dungeon.Player.MoveBy(_dungeon, 1, 0); break;
        //        case ConsoleKey.A: _dungeon.Player.MoveBy(_dungeon, 0, -1); break;
        //        case ConsoleKey.D: _dungeon.Player.MoveBy(_dungeon, 0, 1); break;
        //        case ConsoleKey.E:
        //            do
        //            {
        //                secondKey = Console.ReadKey(true).Key;
        //            } while (secondKey != ConsoleKey.Q && (!(secondKey >= ConsoleKey.D0 && secondKey <= ConsoleKey.D9) || !_dungeon.Player.PickUpItem(_dungeon, secondKey - ConsoleKey.D0)));
        //            break;
        //        case var k when k >= ConsoleKey.D0 && k <= ConsoleKey.D9:
        //            do
        //            {
        //                secondKey = Console.ReadKey(true).Key;
        //            } while (secondKey != ConsoleKey.Q && secondKey != k &&
        //            (secondKey != ConsoleKey.L && secondKey != ConsoleKey.R || !_dungeon.Player.EquipItemInHand(secondKey == ConsoleKey.L ? 0 : 1, k - ConsoleKey.D0)) &&
        //            (secondKey != ConsoleKey.D || !_dungeon.Player.DropItemFromInventory(_dungeon, k - ConsoleKey.D0)));
        //            break;
        //        case var k when k == ConsoleKey.L || k == ConsoleKey.R:
        //            do
        //            {
        //                secondKey = Console.ReadKey(true).Key;
        //            } while (secondKey != ConsoleKey.Q && secondKey != k && (secondKey != ConsoleKey.I || !_dungeon.Player.UnEquipItemInHand(k == ConsoleKey.L ? 0 : 1)));
        //            break;
        //        case ConsoleKey.H:
        //            {
        //                OutputGameSystem.DrawInstruction();
        //                do
        //                {
        //                    secondKey = Console.ReadKey(true).Key;
        //                } while (secondKey != ConsoleKey.H);
        //                break;
        //            }

        //    }
        //}
    }
}