using ProOb_RPG.Effects;
using ProOb_RPG.Entities;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.GameModel
{
    internal partial class ModelGameSystem
    {
        public PlayerPort playerPort;

        internal class PlayerPort
        {
            private Player? _player;
            private Dungeon? _dungeon;

            private OutputPort _outputPort;
            //private InputPort _inputPort;

            public OutputPort Output => _outputPort;
            //public InputPort Input => _inputPort;


            public PlayerPort()
            {
                _outputPort = new();
                //_inputPort = new(this);
                ModelGameSystem.GetInstance().OnPlayerAction += UpdateAllOutput;
            }

            public Player? SwitchPlayer(Player player)
            {
                // Odpięcie starego gracza jeśli był
                if (_player != null)
                {
                    _player.onMessageFromPlayerThrown -= _outputPort.UpdateMessage;
                    _player.OnPlayerStatsChanged -= _outputPort.UpdatePlayerStats;
                    _player.OnPlayerEffectsChanged -= _outputPort.UpdatePlayerEffects;
                    _player.OnPlayerCoinsChanged -= _outputPort.UpdatePlayerCoins;
                    _player.OnPlayerHandChanged -= _outputPort.UpdatePlayerHand;
                    _player.OnPlayerInventoryChanged -= _outputPort.UpdateInventory
                    ;
                }

                // Podpięcie nowego gracza
                var tmp = _player;
                _player = player;

                if (_player != null)
                {
                    _player.onMessageFromPlayerThrown += _outputPort.UpdateMessage;
                    _player.OnPlayerStatsChanged += _outputPort.UpdatePlayerStats;
                    _player.OnPlayerEffectsChanged += _outputPort.UpdatePlayerEffects;
                    _player.OnPlayerCoinsChanged += _outputPort.UpdatePlayerCoins;
                    _player.OnPlayerHandChanged += _outputPort.UpdatePlayerHand;
                    _player.OnPlayerInventoryChanged += _outputPort.UpdateInventory;
                }
                UpdateAllOutput();
                return tmp;
            }

            public Dungeon? SwitchDunegon(Dungeon dungeon)
            {
                // Odpięcie starego dungeona jeśli był
                if (_dungeon != null)
                {
                    _dungeon.onMessageFromDungeonThrown -= _outputPort.UpdateMessage;
                    _dungeon.onMapChanged -= _outputPort.UpdateMap;
                    _dungeon.OnTileContentChanged -= _outputPort.UpdateTileContent;
                }

                // Podpięcie nowego dungeona
                var tmp = _dungeon;
                _dungeon = dungeon;

                if (_dungeon != null)
                {
                    _dungeon.onMessageFromDungeonThrown += _outputPort.UpdateMessage;
                    _dungeon.onMapChanged += _outputPort.UpdateMap;
                    _dungeon.OnTileContentChanged += _outputPort.UpdateTileContent;
                }
                UpdateAllOutput();
                return tmp;
            }

            public void UpdateAllOutput()
            {
                if (_dungeon != null)
                    _dungeon.UpdateMapDrawing();
                if (_player != null)
                    _player.UpdatePlayerEvents();
            }

            internal class OutputPort
            {
                public delegate void MapEventHandler(Tile[,] map);
                public delegate void TileContentEventHandler(List<Entity> entities, List<IItem> items);
                public delegate void PlayerStatsEventHandler(Entity.EntityStats baseStats, Entity.EntityStats modifiedStats);
                public delegate void PlayerEffectsEventHandler(List<EntityEffect> effects);
                public delegate void PlayerCoinsEventHandler(Entity.EntityCoins coins);
                public delegate void PlayerHandEventHandler(IItem? left, IItem? right);
                public delegate void PlayerInventoryEventHandler(List<IItem> inventory);
                public delegate void MessageEventHandler(StringBuilder message);

                public event MapEventHandler? OnMapChanged;
                public event TileContentEventHandler? OnTileContentChanged;
                public event PlayerStatsEventHandler? OnPlayerStatsChanged;
                public event PlayerEffectsEventHandler? OnPlayerEffectsChanged;
                public event PlayerCoinsEventHandler? OnPlayerCoinsChanged;
                public event PlayerHandEventHandler? OnPlayerHandChanged;
                public event PlayerInventoryEventHandler? OnPlayerInventoryChanged;
                public event MessageEventHandler? OnMessageUpdated;

                public void UpdateMap(Tile[,] map) => OnMapChanged?.Invoke(map);
                public void UpdateTileContent(List<Entity> entities, List<IItem> items) => OnTileContentChanged?.Invoke(entities, items);
                public void UpdatePlayerStats(Entity.EntityStats baseStats, Entity.EntityStats modifiedStats) => OnPlayerStatsChanged?.Invoke(baseStats, modifiedStats);
                public void UpdatePlayerEffects(List<EntityEffect> effects) => OnPlayerEffectsChanged?.Invoke(effects);
                public void UpdatePlayerCoins(Entity.EntityCoins coins) => OnPlayerCoinsChanged?.Invoke(coins);
                public void UpdatePlayerHand(IItem? left, IItem? right) => OnPlayerHandChanged?.Invoke(left, right);
                public void UpdateInventory(List<IItem> inventory) => OnPlayerInventoryChanged?.Invoke(inventory);
                public void UpdateMessage(StringBuilder message) => OnMessageUpdated?.Invoke(message);
            }

            //internal class InputPort
            //{
            //    private PlayerPort Port { get; set; }

            //    public InputPort(PlayerPort playerPort)
            //    {
            //        Port = playerPort;
            //    }

            //    public void PlayerMoveUp()
            //    {
            //        Port._dungeon.Player.MoveBy(Port._dungeon, -1, 0);
            //        _turnMenager.NextTurn();
            //        OnPlayerAction?.Invoke();
            //    }
            //    public void PlayerMoveDown()
            //    {
            //        _dungeon.Player.MoveBy(_dungeon, 1, 0);
            //        _turnMenager.NextTurn();
            //        OnPlayerAction?.Invoke();
            //    }
            //    public void PlayerMoveLeft()
            //    {
            //        _dungeon.Player.MoveBy(_dungeon, 0, -1);
            //        _turnMenager.NextTurn();
            //        OnPlayerAction?.Invoke();
            //    }
            //    public void PlayerMoveRight()
            //    {
            //        _dungeon.Player.MoveBy(_dungeon, 0, 1);
            //        _turnMenager.NextTurn();
            //        OnPlayerAction?.Invoke();
            //    }
            //    public bool PlayerPickUpItem(int x)
            //    {
            //        bool result = _dungeon.Player.PickUpItem(_dungeon, x);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool PlayerDropItemFromInventory(int x)
            //    {
            //        bool result = _dungeon.Player.DropItemFromInventory(_dungeon, x);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool PlayerDropAllItemsFromInventory()
            //    {
            //        bool result = _dungeon.Player.DropAllItemsFromInventory(_dungeon);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool PlayerDropItemFromHand(Entity.Hands hand)
            //    {
            //        bool result = _dungeon.Player.DropItemFromHand(_dungeon, hand);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool PlayerEquipItemFromInventory(int x, Entity.Hands hand)
            //    {
            //        bool result = _dungeon.Player.EquipItemInHand(hand, x);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool PlayerunEquipItemFromHand(Entity.Hands hand)
            //    {
            //        bool result = _dungeon.Player.UnEquipItemInHand(hand);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //    public bool SelectItemFromInventory(int x)
            //    {
            //        return true;
            //    }
            //    public bool SelectItemFromHand(Entity.Hands hand)
            //    {
            //        return true;
            //    }
            //    public bool UseItemInHand(Entity.Hands hand)
            //    {
            //        bool result = _dungeon.Player.UseItem(hand);
            //        OnPlayerAction?.Invoke();
            //        return result;
            //    }
            //}
        }
    }
}
