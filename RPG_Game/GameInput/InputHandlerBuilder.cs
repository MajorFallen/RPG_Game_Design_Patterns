using ProOb_RPG.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG.GameInput
{
    internal class InputHandlerBuilder : AbstractBuilder<InputGameSystem.IInputHandler>
    {
        private bool _isItemExist = false;
        private bool _isUsableItemExist = false;
        private bool _isPlayerExist = false;

        List<InputGameSystem.IInputHandler> _inputHandlerList = new List<InputGameSystem.IInputHandler>();
        public override void Reset()
        {
            _inputHandlerList = new List<InputGameSystem.IInputHandler>();
        }

        public override InputGameSystem.IInputHandler GetResult()
        {
            _result = _inputHandlerList[0];
            _inputHandlerList.RemoveAt(0);
            InputGameSystem.IInputHandler last = _result;
            foreach (InputGameSystem.IInputHandler h in _inputHandlerList)
            {
                last = last.SetNext(h);
            }
            return base.GetResult();
        }

        public override IDungeonBuilder AddPlayer(EntityStats entityStats)
        {
            _inputHandlerList.Insert(0,new InputGameSystem.MovementInputHandler());
            _inputHandlerList.Insert(0, new InputGameSystem.OpenInstructionInputHandler());
            _inputHandlerList.Insert(0, new InputGameSystem.GameQuitInputHandler());
            _inputHandlerList.Add(new InputGameSystem.NotImplementedInputHandler());

            return this;
        }

        public override IDungeonBuilder AddEnemies(int numberOfEnemies)
        {
            _inputHandlerList.Insert(0, new InputGameSystem.AttackEntityInputHandler());
            return this;
        }

        public override IDungeonBuilder AddItems(int numberOfItems)
        {
            WhenItemAdded();
            return this;
        }

        public override IDungeonBuilder AddWeapons(int numberOfWeapons)
        {
            WhenItemAdded();
            return base.AddWeapons(numberOfWeapons);
        }

        public override IDungeonBuilder AddModifiedWeapons(int numberOfModifiedWeapons, int maxStacks)
        {
            WhenItemAdded();
            return base.AddModifiedWeapons(numberOfModifiedWeapons, maxStacks);
        }

        public override IDungeonBuilder AddCoins(int numberOfCoinsStacks)
        {
            WhenItemAdded();
            return base.AddCoins(numberOfCoinsStacks);
        }

        public override IDungeonBuilder AddPotions(int numberOfPotions)
        {
            WhenItemAdded();
            return base.AddPotions(numberOfPotions);
        }

        private void WhenItemAdded()
        {
            if (_isItemExist == true) return;
            _inputHandlerList.Insert(0, new InputGameSystem.ItemPickUpInputHandler());
            _inputHandlerList.Insert(0, new InputGameSystem.ItemDropInputHandler());
            _inputHandlerList.Insert(0, new InputGameSystem.ItemInInventoryManagementInputHandler());
            _isItemExist = true;
        }
    }
}
