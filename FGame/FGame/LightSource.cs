using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class LightSource
    {
        public Point Position { get; set; }
        public int Strenght { get; set; }
        public float Max { get; set; } = 1f;
    }
}
