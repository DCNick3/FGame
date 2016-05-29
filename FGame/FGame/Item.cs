using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class Item
    {
        public ItemType Type { get; internal set; }
        public virtual int GetMaxStackSize()
        {
            return 99;
        }
    }

    public class ItemStack
    {
        public Item Type { get; internal set; }
        public int Count { get; internal set; }
    }

    public enum ItemType
    {
        GoldCoin,
    }
}
