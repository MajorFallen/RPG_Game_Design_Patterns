using ProOb_RPG.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG.GameOutput
{
    internal class InstructionBuilder : AbstractBuilder<List<StringBuilder>>
    {
        private bool _isItemExist;
        private bool IsItemExist
        {
            get { return _isItemExist; }
            set
            {
                bool oldValue = _isItemExist;
                _isItemExist = value;
                if (_isItemExist && !oldValue)
                    AddBasicItemInstruction();
            }

        }
        List<StringBuilder> _instruction;

        public InstructionBuilder()
        {
            IsItemExist = false;
            _instruction = new List<StringBuilder>();
            Reset();
        }
        public override void Reset()
        {
            _instruction = new List<StringBuilder>();
        }
        public override IDungeonBuilder BuildEmptyDungeon()
        {
            _instruction.Add(new StringBuilder("====== Game Instruction ======"));
            return this;
        }
        public override IDungeonBuilder BuildFilledDungeon()
        {
            _instruction.Add(new StringBuilder("====== Game Instruction ======"));
            return this;
        }
        public override IDungeonBuilder AddPaths()
        {
            _instruction.Add(new StringBuilder($"There are corridors in the dungeon, travel through them to places of your destination."));
            return this;
        }
        public override IDungeonBuilder AddRooms(int numberOfRooms)
        {
            _instruction.Add(new StringBuilder($"There are {numberOfRooms} rooms in the dungeon which have many space in it."));
            return this;
        }
        public override IDungeonBuilder AddCentralRoom(int yLength, int xLength)
        {
            _instruction.Add(new StringBuilder($"There is one big room ({yLength}x{xLength}) in the middle of the dungeon which have many space in it."));
            return this;
        }
        public override IDungeonBuilder AddPlayer(EntityStats entityStats)
        {
            _instruction.Add(new StringBuilder($"You as a player spawn at the top of the dungeon."));
            _instruction.Add(new StringBuilder($"You can move with WSAD keys."));
            _instruction.Add(new StringBuilder($"Your character have their own attributes with starting values:"));
            return this;
        }
        public override IDungeonBuilder AddItems(int numberOfItems)
        {
            IsItemExist = true;
            _instruction.Add(new StringBuilder($"There are placed {numberOfItems} not useable items."));
            return this;
        }
        public override IDungeonBuilder AddPotions(int numberOfPotions)
        {
            IsItemExist = true;
            _instruction.Add(new StringBuilder($"There are placed {numberOfPotions} potions."));
            _instruction.Add(new StringBuilder($"They can be drinked with (Not implemented) and gives you some funny effects"));
            return this;
        }
        public override IDungeonBuilder AddCoins(int numberOfCoinsStacks)
        {
            IsItemExist = true;
            _instruction.Add(new StringBuilder($"There are placed {numberOfCoinsStacks} Coin Sacs that contains some coins (who could expect that)."));
            _instruction.Add(new StringBuilder($"They can contain one of three type of coins: bronze, silver, gold. Depend of type the sac it contains various amount of coins."));
            _instruction.Add(new StringBuilder($"After being picked up coins are added to your 'Coin Inventory'."));
            return this;
        }
        public override IDungeonBuilder AddWeapons(int numberOfWeapons)
        {
            IsItemExist = true;
            _instruction.Add(new StringBuilder($"There are placed {numberOfWeapons} weapons."));

            return this;
        }
        public override IDungeonBuilder AddModifiedWeapons(int numberOfModifiedWeapons, int maxStacks)
        {
            IsItemExist = true;
            _instruction.Add(new StringBuilder($"There are placed {numberOfModifiedWeapons} modified weapons."));
            _instruction.Add(new StringBuilder($"They have some effect placed on them. Effects can modify the attributes of the weapon or player"));
            return this;
        }
        public override IDungeonBuilder AddEnemies(int numberOfEnemies)
        {
            _instruction.Add(new StringBuilder($"Through the dungeon there are {numberOfEnemies} monsters."));
            _instruction.Add(new StringBuilder($"They look very dangerous, but in reality they are very friendly and kind"));
            return this;
        }
        private void AddBasicItemInstruction()
        {
            _instruction.Add(new StringBuilder($"Through the dungeon there are placed many items."));
            _instruction.Add(new StringBuilder($"They can be picked up with (E) + (*) keys where * represent the indenx of the item on the ground."));
            _instruction.Add(new StringBuilder($"They can be equiped to your hands with (*) + (L) or (R) where * represent the index of the item in the inventory."));
        }
        public override List<StringBuilder> GetResult()
        {
            List<StringBuilder> result = _instruction;
            Reset();
            return result;
        }
    }
}
