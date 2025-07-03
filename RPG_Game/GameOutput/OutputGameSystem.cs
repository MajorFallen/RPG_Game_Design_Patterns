using ProOb_RPG.Effects;
using ProOb_RPG.GameInput;
using ProOb_RPG.GameModel;
using ProOb_RPG.Entities;
using ProOb_RPG.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.GameInput.InputGameSystem;

namespace ProOb_RPG.GameOutput
{
    internal interface IOutputGameSystem<T> where T : IOutputGameSystem<T>
    {
        void Draw();
    }
    internal sealed class OutputGameSystem : IOutputGameSystem<OutputGameSystem>
    {
        private ModelGameSystem _model;

        private InputGameSystem? _input;

        private static int _windowHeight = 42;
        private static int _windowWidth = 148; // uważać aby nie przekraczała maksymalnej wielkości konsoli (dla Latency Hunter 148 max)
        private static int _firstColumnWidth = 42;
        private static int _secondColumnWidth = 40;
        private static int _thirdColumnWidth = _windowWidth - _firstColumnWidth - _secondColumnWidth - 6;

        // Main Windows
        private Window _map;
        private Window _enemiesOnTile;
        private Window _itemsOnTile;
        private Window _playerStats;
        private Window _playerEffects;
        private Window _coins;
        private Window _hand;
        private Window _inventory;
        private Window _messages;
        private Window _legend;
        private Window _instruction;

        // Stilistic  Windows
        private Window _columnSeparetor;

        public OutputGameSystem(InputGameSystem? input = null)
        {
            _map = new Window(_firstColumnWidth);
            _enemiesOnTile = new Window(_firstColumnWidth);
            _enemiesOnTile.AddToEnd(new StringBuilder("====== Enemies ======"));
            _itemsOnTile = new Window(_firstColumnWidth);
            _itemsOnTile.AddToEnd(new StringBuilder("====== Items ======"));
            _playerStats = new Window(_secondColumnWidth);
            _playerStats.AddToEnd(new StringBuilder("====== Attributes ======"));
            _playerEffects = new Window(_secondColumnWidth);
            _playerEffects.AddToEnd(new StringBuilder("====== Effects ======"));
            _coins = new Window(_secondColumnWidth);
            _coins.AddToEnd(new StringBuilder("====== Coins ======"));
            _hand = new Window(_secondColumnWidth);
            _hand.AddToEnd(new StringBuilder("====== Hand ======"));
            _inventory = new Window(_secondColumnWidth);
            _inventory.AddToEnd(new StringBuilder("====== Inventory ======"));
            _legend = new Window(_thirdColumnWidth);
            _legend.AddToEnd(new StringBuilder("====== Legend ======"));
            _messages = new Window(_thirdColumnWidth, _windowHeight);
            _messages.AddToEnd(new StringBuilder("====== Messages ======"));

            _columnSeparetor = new Window(3);
            for (int i = 0; i < _windowHeight; i++) 
            {
                _columnSeparetor.AddToEnd(new StringBuilder(" | "));
            }

            _instruction = new Window(_windowWidth, _windowHeight);

            _model = ModelGameSystem.GetInstance();

            _input = input;
            if(_input != null)
            {
                _input.OpenInstrucion += DrawInstruction;
                _input.CloseInstrucion += Draw;
                _input.OnMessageThrown += AddMessage;
                _input.OnLegendSet += SetLegend;

            }

            ConnectToModelOutputPort();

            InstructionBuilder builder = new InstructionBuilder();
            _model.UseBuilder(builder);
            SetInstruction(builder.GetResult());
        }
        public void Draw()
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder[] screen = new StringBuilder[_windowHeight];
            for (int k = 0; k < _windowHeight; k++)
                screen[k] = new StringBuilder();
            int i = 0;
            AppendToWindow(screen, ref i, _map);
            AppendToWindow(screen, ref i, _enemiesOnTile);
            AppendToWindow(screen, ref i, _itemsOnTile);
            AddSpacesToWindow(screen, ref i, _firstColumnWidth);
            i = 0;
            AppendToWindow(screen, ref i, _columnSeparetor);
            i = 0;
            AppendToWindow(screen, ref i, _playerStats);
            AppendToWindow(screen, ref i, _playerEffects);
            AppendToWindow(screen, ref i, _coins);
            AppendToWindow(screen, ref i, _hand);
            AppendToWindow(screen, ref i, _inventory);
            AddSpacesToWindow(screen, ref i, _secondColumnWidth);
            i = 0;
            AppendToWindow(screen, ref i, _columnSeparetor);
            i = 0;
            AppendToWindow(screen, ref i, _legend);
            AddSpacesToWindow(screen, ref i, _thirdColumnWidth, 2);
            AppendToWindow(screen, ref i, _messages);
            AddSpacesToWindow(screen, ref i, _thirdColumnWidth);

            foreach(StringBuilder sb in screen)
            {
                Console.WriteLine(sb);
            }
        }
        public void DrawInstruction()
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder[] screen = new StringBuilder[_windowHeight];
            for (int k = 0; k < _windowHeight; k++)
                screen[k] = new StringBuilder();
            int i = 0;
            AppendToWindow(screen, ref i, _instruction);
            AddSpacesToWindow(screen, ref i, _windowWidth);
            foreach (StringBuilder sb in screen)
            {
                Console.WriteLine(sb);
            }
        }


        public void UpdateMap(Tile[,] map)
        {
            _map.ChangeAll(new List<StringBuilder>(), 0);
            for (int i = 0; i < map.GetLength(0); i++)
            {

                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    sb.Append(map[i, j].GetChar());
                }
                _map.AddToEnd(sb);
            }
            Draw();
        }
        public void UpdateTileContent(List<Entity> entities, List<IItem> items)
        {
            List<StringBuilder> entityList = new List<StringBuilder>();
            List<StringBuilder> itemList = new List<StringBuilder>();
            int i = 0;
            foreach (Entity ent in entities)
            {
                entityList.Add(new StringBuilder($"({i}) {ent.GetEntityName()}"));
                i++;
            }
            i = 0; 
            foreach (IItem item in items)
            {
                itemList.Add(new StringBuilder($"({i}) {item.GetItemName()}"));
                i++;
            }
            _enemiesOnTile.ChangeAll(entityList, 1);
            _itemsOnTile.ChangeAll(itemList, 1);
            Draw();
        }
        public void UpdatePlayerStats(Entity.EntityStats baseStats, Entity.EntityStats modifiedStats)
        {
            List<StringBuilder> statList = new List<StringBuilder>();
            foreach (var stat in modifiedStats.statDictionary)
            {
                int baseStat = baseStats.statDictionary[stat.Key];
                int modifiedStat = stat.Value;
                statList.Add(new StringBuilder(stat.Key.ToString() + ": " + stat.Value + (modifiedStat == baseStat ? "" : modifiedStat > baseStat ? $" (+{modifiedStat - baseStat})" : $" ({modifiedStat - baseStat})")));
            }
            _playerStats.ChangeAll(statList, 1);
            Draw();
        }
        public void UpdatePlayerEffects(List<EntityEffect> effects)
        {
            List<StringBuilder> effectsList = new List<StringBuilder>();
            if (effects.Count == 0)
                effectsList.Add(new StringBuilder("No Effects"));
            else
            {
                foreach (var effect in effects)
                {
                    effectsList.Add(new StringBuilder($"{effect.EffectName} " + (effect.Duration == -1 ? "(Infinite)" : $"(Duration: {effect.Duration})")));
                    foreach (var line in effect.EffectDescription)
                    {
                        effectsList.Add(new StringBuilder($"  {line}"));
                    }
                }
            }
            _playerEffects.ChangeAll(effectsList, 1);
            Draw();
        }
        public void UpdateCoins(Entity.EntityCoins coins)
        {
            List<StringBuilder> coinList = new List<StringBuilder>();
            foreach (var coin in coins.coinDictionary)
            {
                coinList.Add(new StringBuilder(coin.Key.ToString() + ": " + coin.Value));
            }
            _coins.ChangeAll(coinList, 1);
            Draw();
        }
        public void UpdateHand(IItem? left, IItem? right)
        {
            List<StringBuilder> handList =
            [
                new StringBuilder("L: " + (left != null ? left!.GetItemName() : "")),
                new StringBuilder("R: " + (right != null ? right!.GetItemName() : "")) ,
            ];
            _hand.ChangeAll(handList, 1);
            Draw();
        }
        public void Updateinventory(List<IItem> inventory)
        {
            List<StringBuilder> itemList = new List<StringBuilder>();
            int i = 0;
            foreach (IItem item in inventory)
            {
                itemList.Add(new StringBuilder($"({i}) {item.GetItemName()}"));
                i++;
            }
            _inventory.ChangeAll(itemList, 1);
            Draw();
        }
        public void AddMessage(StringBuilder message)
        {
            string timePrefix = $"[{DateTime.Now:HH:mm:ss}] ";
            message.Insert(0, timePrefix);
            _messages.AddInMiddle(message, 1);
            Draw();
        }
        public void SetInstruction(List<StringBuilder> instruction)
        {
            _instruction.ChangeAll(instruction);
            Draw();
        }
        public void SetLegend(List<StringBuilder> legend)
        {
            _legend.ChangeAll(legend, 1);
            Draw();
        }


        private void AppendToWindow(StringBuilder[] screen, ref int i, Window window)
        {
            foreach (StringBuilder sb in window.Content)
            {
                if (i == _windowHeight) return;
                screen[i++].Append(sb);
            }
        }
        private void AddSpacesToWindow(StringBuilder[] window, ref int i, int width, int height = int.MaxValue)
        {
            for (int k = i; k < _windowHeight; k++)
            {
                if (height == 0)
                    return;
                // Dodaj spacje do StringBuildera na tej pozycji
                window[k].Append(' ', width);
                height--;
                i++;
            }
        }

        private void ConnectToModelOutputPort()
        {
            ModelGameSystem.PlayerPort.OutputPort port = _model.playerPort.Output;

            port.OnMapChanged += UpdateMap;
            port.OnTileContentChanged += UpdateTileContent;
            port.OnPlayerStatsChanged += UpdatePlayerStats;
            port.OnPlayerEffectsChanged += UpdatePlayerEffects;
            port.OnPlayerCoinsChanged += UpdateCoins;
            port.OnPlayerHandChanged += UpdateHand;
            port.OnPlayerInventoryChanged += Updateinventory;
            port.OnMessageUpdated += AddMessage;

            _model.playerPort.UpdateAllOutput();
        }

    }

    internal class Window
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxHeight { get; set; }
        public List<StringBuilder> Content { get; set; }
        public Window(int width, int maxHeight = int.MaxValue) 
        {
            Width = width;
            MaxHeight = maxHeight;
            Content = new List<StringBuilder>();
        }

        public void AddToLine(StringBuilder sb, int i) 
        {
            if(i >= Content.Count)
            {
                for (int j = Content.Count; j <= i; j++) 
                {
                    AddToEnd(new StringBuilder());
                }
            }
            Content[i].Append(sb);
        }
        public void AddToEnd(StringBuilder sb) 
        {
            Content.Add(FormatText(sb, Width));
        }
        public void AddInMiddle(StringBuilder sb, int i)
        {
            if (Content.Count == MaxHeight)
                Content.RemoveAt(Content.Count - 1);
            Content.Insert(i,FormatText(sb, Width));
        }
        public void ChangeAll(List<StringBuilder> list, int i = 0)
        {
            // Jeśli indeks jest poza zakresem, ustaw go na koniec listy
            if (i > Content.Count)
                i = Content.Count;

            // Tworzymy kopię listy, aby uniknąć referencji do oryginału
            var copiedList = list.Select(sb => FormatText(new StringBuilder(sb.ToString()), Width)).ToList();

            // Usuwamy elementy od indeksu i do końca
            Content.RemoveRange(i, Content.Count - i);

            // Dodajemy całą skopiowaną listę od indeksu i
            Content.InsertRange(i, copiedList);
        }

        private StringBuilder FormatText(StringBuilder sb, int length)
        {
            if (sb.Length > length)
            {
                sb.Length = length; // Ucina nadmiarowe znaki
            }
            else
            {
                sb.Append(' ', length - sb.Length); // Dopełnia spacjami do pełnej długości
            }
            return sb;
        }
    }
}
