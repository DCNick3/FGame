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
        {
            Inventory = new ItemStack[] { new ItemStack(Items.goldCoin, 2) };
        }

        public void OpenStart(Player player)
        {
            IsOpen = true;
            AnimationFrame = 1;
        }

        public void OpenEnd(Player player)
        {
            for (int i = 0; i < Inventory.Length; i++)
                player.AddItem(Inventory[i]);
            Inventory = new ItemStack[0];
        }
        
        public int Type { get; set; }
        public bool IsOpen { get; set; }
        public int AnimationFrame { get; set; }
        public Point Position { get; set; }
        public Point ChunkPosition { get; set; }
        public ItemStack[] Inventory { get; set; }
        public Point GlobPosition
        {
            get
            {
                return new Point(ChunkPosition.X * Chunk.width + Position.X, ChunkPosition.Y * Chunk.height + Position.Y);
            }
        }
    }
}
