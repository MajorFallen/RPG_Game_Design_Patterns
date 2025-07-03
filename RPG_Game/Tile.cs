using ProOb_RPG.Entities;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG
{
    internal class Tile
    {
        public enum TileTypes { Wall, Floor }

        public TileTypes TileType { get; set; }
        private List<Entity> entities;
        private List<IItem> loot;

        public Tile(TileTypes tileType)
        {
            this.TileType = tileType;
            this.entities = new List<Entity>();
            this.loot = new List<IItem>();
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public Entity RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            return entity;
        }
        public void AddItem(IItem item)
        {
            loot.Add(item);
        }
        public IItem RemoveItem(IItem item)
        { 
            loot.Remove(item);
            return item;
        }
        public bool IsWalkable()
        {
            return TileType == TileTypes.Floor;
        }

        public List<IItem> GetTileItemContent()
        {
            return (loot);
        }
        public List<Entity> GetTileEntityContent()
        {
            return (entities);
        }
        public IItem? GetTileItem(int x = 0)
        {
            if (x >= 0 && x < loot.Count )
                return (loot[x]);
            return null;
        }
        public Entity? GetTileEntity(int whichEntity = 0)
        {
            if (whichEntity >= 0 && whichEntity < entities.Count)
                return (entities[whichEntity]);
            return null;
        }
        public char GetChar()
        {
            if (TileType == TileTypes.Wall) return '█';
            if (entities.OfType<Player>().Any()) return 'P';
            if (entities.Count  > 0) return 'E';
            if (loot.Count > 0) return 'L';
            else return ' ';
        }
    }
}
