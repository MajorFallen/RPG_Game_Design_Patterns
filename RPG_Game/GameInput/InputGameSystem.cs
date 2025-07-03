using ProOb_RPG.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.GameInput.InputGameSystem;

namespace ProOb_RPG.GameInput
{



    internal interface IInputGameSystem
    {

    }

    internal partial class InputGameSystem : IInputGameSystem
    {
        internal class KeyConfig
        {
            public enum KeyMapping { MoveUp, MoveDown, MoveLeft, MoveRight, PickUpMode, DropMode, QuitFromMode, LeftHand, RightHand, Inventory, Instruction, QuitGame, UseItem, AttackEntity, BasicAttack, SneakAttack, MagicAttack }

            public Dictionary<KeyMapping, ConsoleKey> KeyMap { get; private set; }

            public KeyConfig()
            {
                KeyMap = new Dictionary<KeyMapping, ConsoleKey>
                {
                    { KeyMapping.MoveUp, ConsoleKey.W },
                    { KeyMapping.MoveDown, ConsoleKey.S },
                    { KeyMapping.MoveLeft, ConsoleKey.A },
                    { KeyMapping.MoveRight, ConsoleKey.D },
                    { KeyMapping.QuitFromMode, ConsoleKey.Q },
                    { KeyMapping.PickUpMode, ConsoleKey.E },
                    { KeyMapping.DropMode, ConsoleKey.G },
                    { KeyMapping.LeftHand, ConsoleKey.L },
                    { KeyMapping.RightHand, ConsoleKey.R },
                    { KeyMapping.Inventory, ConsoleKey.I },
                    { KeyMapping.Instruction, ConsoleKey.H },
                    { KeyMapping.QuitGame, ConsoleKey.Escape },
                    { KeyMapping.UseItem, ConsoleKey.F },
                    { KeyMapping.AttackEntity, ConsoleKey.T },
                    { KeyMapping.BasicAttack, ConsoleKey.D1 },
                    { KeyMapping.SneakAttack, ConsoleKey.D2 },
                    { KeyMapping.MagicAttack, ConsoleKey.D3 }
                };
            }
            public void RebindKey(KeyMapping action, ConsoleKey newKey)
            {
                KeyMap[action] = newKey;
            }
            public ConsoleKey? GetKey(KeyMapping action)
            {
                return KeyMap.TryGetValue(action, out var key) ? key : null;
            }
            public KeyMapping? GetMapping(ConsoleKey key)
            {
                foreach (var kvp in KeyMap)
                {
                    if (kvp.Value == key)
                        return kvp.Key;
                }
                return null;
            }
        }
        public enum PossibleActions { Movement, EnterItemPickUpMode, EnterItemDropMode, EquipItem }

        private ModelGameSystem _model;

        private KeyConfig _keyConfig;

        private IInputHandler _inputHandler;

        private List<StringBuilder> _legend;

        public delegate void MessageEventHandler(StringBuilder message);
        public delegate void LegendEventHandler(List<StringBuilder> legend);

        public event Action? OpenInstrucion;
        public event Action? CloseInstrucion;
        public event MessageEventHandler? OnMessageThrown;
        public event LegendEventHandler? OnLegendSet;

        public InputGameSystem()
        {
            _model = ModelGameSystem.GetInstance();
            _model.RequestInput += WaitForInput;

            _keyConfig = new KeyConfig();

            InputHandlerBuilder builder = new InputHandlerBuilder();
            _model.UseBuilder(builder);
            _inputHandler = builder.GetResult();
            _legend = _inputHandler.CreateLegend(this);
        }
        private void WaitForInput()
        {
            bool isActionTaken = false;
            ConsoleKey key;
            while(!isActionTaken)
            {
                OnLegendSet?.Invoke(_legend);
                key = Console.ReadKey(true).Key;
                isActionTaken = _inputHandler.Handle(this, key);
            }
        }
    }



    
}
