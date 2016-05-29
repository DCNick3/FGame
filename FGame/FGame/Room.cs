using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FGame
{
    public class Room
    {
        public Rectangle Position { get; private set; }
        public Chest Chest { get; internal set; }
        public List<Room> ConnectsTo { get; private set; }
        public Point Center
        {
            get
            {
                return Position.Center;//new Point(Position.X + Position.Width / 2, Position.Y + Position.Height / 2);
            }
        }
        public Room(Rectangle pos)
        {
            Position = pos;
            ConnectsTo = new List<Room>();
        }
    }
}
