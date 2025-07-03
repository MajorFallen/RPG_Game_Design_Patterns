using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG
{
    internal interface IRPGObject
    {
        void MoveBy(Dungeon dungeon, int dy, int dx);
        (int x, int y) GetPosition();
        void SetPosition((int, int) position);
    }

    internal abstract class RPGObject : IRPGObject
    {
        protected (int, int) Position { get; set; }

        public RPGObject()
        {
            Position = (-1,-1);
        }

        public abstract void MoveBy(Dungeon dungeon, int dy, int dx);
        public (int, int) GetPosition()
        {
            return Position;
        }
        public void SetPosition((int,int) position)
        {
            this.Position = position;
        }
    }
}
