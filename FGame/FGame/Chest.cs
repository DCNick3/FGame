using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace FGame
{
    public class Chest //: GameObject 
    {
        public Chest(Game1 game) //: base(game) 
        { }

        public void OpenStart(Player player)
        {
            IsOpen = true;
            AnimationFrame = 1;
        }

        public void OpenEnd(Player player)
        {
            //TODO: Move inventory!
        }

        //TODO: Add inventory!
        public int Type { get; set; }
        public bool IsOpen { get; set; }
        public int AnimationFrame { get; set; }
        public Point Position { get; set; }
    }
}
