using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class Tile
    {
        public Tile(int id, bool isObstacle)
        {
            Id = id;
            IsObstacle = isObstacle;
        }

        public int Id { get; set; }
        public bool IsObstacle { get; set; }
    }
}
