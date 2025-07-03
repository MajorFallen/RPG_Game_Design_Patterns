using ProOb_RPG.Entities;
using ProOb_RPG.GameModel;
using ProOb_RPG.GameOutput;
using ProOb_RPG.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG
{
    internal class Dungeon
    {
        readonly private int height;
        readonly private int width;

        private Tile[,] Map { get; set; }

        public Player Player { get; set; }
        private List<Entity> entities;
        private List<IItem> items;

        public event ModelGameSystem.PlayerPort.OutputPort.MessageEventHandler? onMessageFromDungeonThrown;
        public event ModelGameSystem.PlayerPort.OutputPort.MapEventHandler? onMapChanged;
        public event ModelGameSystem.PlayerPort.OutputPort.TileContentEventHandler? OnTileContentChanged;

        public Dungeon(int height, int width)
        {
            this.height = height;
            this.width = width;
            this.Map = new Tile[height + 2, width + 2];
            this.entities = new List<Entity>();
            this.items = new List<IItem>();
        }
        public bool SetTile(int y, int x, Tile tile)
        {
            if (x >= 0 && x <= width + 1 && y >= 0 && y <= height + 1)
            {
                Map[y, x] = tile;
                return true;
            }
            return false;
        }
        public Tile? GetTile((int y, int x) position)
        {
            if (position.x >= 0 && position.x <= width + 1 && position.y >= 0 && position.y <= height + 1)
            {
                return Map[position.y, position.x];
            }
            return null;
        }
        //public void DrawEverything()
        //{
        //    const int inventoryMapGap = 2;
            
        //    StringBuilder[] screen = new StringBuilder[screenHeight];
        //    for (int k = 0; k < screenHeight; k++)
        //    {
        //        screen[k] = new StringBuilder();
        //    }
        //    int i = 0;
        //    while (i < height + 2)
        //    {
        //        screen[i].Append(string.Concat(Enumerable.Range(0, MapDrawing.GetLength(1)).Select(j => MapDrawing[i, j])).PadRight(width + 2 + inventoryMapGap));
        //        i++;
        //    }
        //    screen[i++].Append(' ', width + 2 + inventoryMapGap);
        //    screen[i++].Append(" Tile Items:".PadRight(width + 2 + inventoryMapGap));

        //    //Tile Content

        //    (int y, int x) = Player.GetPosition();
        //    List<IItem> itemsList = Map[y, x].GetTileItemContent();
        //    int j = 0;
        //    foreach (IItem item in itemsList)
        //    {
        //        screen[i++].Append($"({j++}) {item.GetItemName()}".PadRight(width + 2 + inventoryMapGap));
        //    }
        //    while (i < screenHeight)
        //    {
        //        screen[i++].Append(' ', width + 2 + inventoryMapGap);
        //    }

        //    //Player Stats

        //    i = 0;
        //    int playerStatsSize = screenWidth - inventoryMapGap - width - 2;
        //    screen[i++].Append("==== Attributes =====".PadRight(playerStatsSize));

        //    foreach (PropertyInfo prop in typeof(EntityStats).GetProperties())
        //    {
        //        string stat = $"{prop.Name}: {prop.GetValue(Player.ModifiedStats)}";
        //        if ((int)prop.GetValue(Player.BaseStats) != (int)prop.GetValue(Player.ModifiedStats))
        //            stat = stat + (((int)prop.GetValue(Player.BaseStats) < (int)prop.GetValue(Player.ModifiedStats)) ? " (+" : " (") + $"{(int)prop.GetValue(Player.ModifiedStats) - (int)prop.GetValue(Player.BaseStats)}" + ")";
        //        screen[i++].Append(stat.PadRight(playerStatsSize));
        //    }

        //    //Player Coins

        //    screen[i++].Append("======= Coins ========".PadRight(playerStatsSize));
        //    screen[i++].Append($"Golden Coins: {Player.GoldenCoins}".PadRight(playerStatsSize));
        //    screen[i++].Append($"Silver Coins: {Player.SilverCoins}".PadRight(playerStatsSize));
        //    screen[i++].Append($"Gold Bars: {Player.GoldBars}".PadRight(playerStatsSize));

        //    //Player Equipment

        //    screen[i++].Append("====== Equiped =======".PadRight(playerStatsSize));
        //    IItem? leftHand = Player.GetItemFromLeftHand();
        //    IItem? rightHand = Player.GetItemFromRightHand();
        //    screen[i++].Append(("L: " + (leftHand == null ? "None" : $"{leftHand.GetItemName()}")).PadRight(playerStatsSize));
        //    screen[i++].Append(("R: " + (rightHand == null ? "None" : $"{rightHand.GetItemName()}")).PadRight(playerStatsSize));

        //    //Player Inventory

        //    itemsList = Player.GetInventory();
        //    j = 0;
        //    screen[i++].Append("===== Inventory ======".PadRight(playerStatsSize));
        //    foreach (IItem item in itemsList)
        //    {
        //        screen[i++].Append($"({j++}) {item.GetItemName()}".PadRight(playerStatsSize));
        //    }
        //    while (i < screenHeight)
        //    {
        //        screen[i++].Append(' ', playerStatsSize);
        //    }

        //    //Drawing Screen

        //    Console.SetCursorPosition(0, 0);
        //    foreach (StringBuilder line in screen)
        //    {
        //        Console.WriteLine(line);
        //    }
        //}
        public void UpdateMapDrawing()
        {
            onMapChanged?.Invoke(Map);
            (int y, int x) = Player.GetPosition();
            OnTileContentChanged?.Invoke(Map[y, x].GetTileEntityContent(), Map[y, x].GetTileItemContent());
        }
        public void UpdateEntityPosition(Entity entity, (int y,int x) oldPosition, (int y ,int x) newPosition)
        {
            // Usunięcie obiektu ze starego Tile
            Map[oldPosition.y, oldPosition.x].RemoveEntity(entity);

            // Dodanie obiektu do nowego Tile
            Map[newPosition.y, newPosition.x].AddEntity(entity);

            // Aktualizacja mapy wizualnej
            UpdateMapDrawing();
            //MapDrawing[oldY, oldX] = Map[oldY, oldX].GetChar();
            //MapDrawing[newY, newX] = Map[newY, newX].GetChar();
        }
        public void UpdateItemPosition(IItem item, (int y, int x) oldPosition, (int y, int x) newPosition)
        {

            // Usunięcie obiektu ze starego Tile
            Map[oldPosition.y, oldPosition.x].RemoveItem(item);

            // Dodanie obiektu do nowego Tile
            Map[newPosition.y, newPosition.x].AddItem(item);

            // Aktualizacja mapy wizualnej
            UpdateMapDrawing();
            //MapDrawing[oldY, oldX] = Map[oldY, oldX].GetChar();
            //MapDrawing[newY, newX] = Map[newY, newX].GetChar();
        }

        public bool IsWalkable((int y,int x) position)
        {
            return Map[position.y, position.x].IsWalkable();
        }
        public bool AddPlayer(Player player, (int y, int x) position)
        {
            if (!IsWalkable(position))
                return false;
            player.SetPosition(position);
            Player = player;
            Map[position.y, position.x].AddEntity(player);
            return true;
        }
        public bool AddEntity(Entity entity, (int y, int x) position)
        {
            (int y, int x) = position;
            if (!IsWalkable(position))
                return false;
            entity.SetPosition(position);
            entities.Add(entity);
            Map[y, x].AddEntity(entity);
            UpdateMapDrawing();
            entity.onEntityDeath += () => RemoveEntity(entity);
            return true;
        }
        public bool AddItem(IItem item, (int x, int y) position)
        {
            (int y, int x) = position;
            if (!IsWalkable(position))
                return false;
            item.SetPosition(position);
            items.Add(item);
            Map[y, x].AddItem(item);
            UpdateMapDrawing();
            return true;
        }
        public IItem RemoveItem(IItem item)
        {
            items.Remove(item);
            (int y, int x) = item.GetPosition();
            Map[y, x].RemoveItem(item);
            UpdateMapDrawing();
            return item;
        }
        public Entity RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            (int y, int x) = entity.GetPosition();
            Map[y, x].RemoveEntity(entity);
            UpdateMapDrawing();
            return entity;
        }
        public IItem GetItem(IItem item)
        {
            (int y, int x) = item.GetPosition();
            return Map[y, x].RemoveItem(item);
        }
        List<StringBuilder> ConvertToList(char[,] X)
        {
            int rows = X.GetLength(0);
            int cols = X.GetLength(1);
            var result = new List<StringBuilder>(rows);

            for (int i = 0; i < rows; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(X[i, j]);
                }
                result.Add(sb);
            }

            return result;
        }
    }
}
