using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class Item
    {
        public Item(ItemType tp, int texture)
        {
            Type = tp;
            this.texture = texture;
        }
        
        public Item(ItemType tp)
        {
            Type = tp;
        }

        public ItemType Type { get; internal set; }

        private int maxStack = 99;
        private int texture;
        OnUseDeleg onUseDeleg = (Player p, ItemStack s) => { };

        public delegate void OnUseDeleg(Player player, ItemStack stack);

        public virtual int GetMaxStackSize()
        {
            return maxStack;
        }

        public virtual int GetTextureId(ItemStack stack)
        {
            return texture;
        }

        public virtual void OnUse(Player player, ItemStack stack)
        {
            onUseDeleg(player, stack);
        }

        public virtual Item SetMaxStackSize(int sz)
        {
            maxStack = sz;
            return this;
        }

        public virtual Item SetOnUse(OnUseDeleg action)
        {
            onUseDeleg = action;
            return this;
        }
    }

    public class ItemSword : Item
    {
        public ItemSword(ItemType type, int texture) : base(type, texture)
        {}

        private int damage = 0;

        public virtual ItemSword SetDamage(int value)
        {
            damage = value;
            return this;
        }

        public virtual int GetDamage()
        {
            return damage;
        }

        public override void OnUse(Player player, ItemStack stack)
        {
            if (player.CanUseSwords)
            {

            }
        }
    }

    public class ItemStack
    {
        public ItemStack(Item type, int count)
        {
            Type = type;
            Count = count;
        }

        public Item Type { get; internal set; }
        public int Count { get; internal set; }
        public bool IsEmpty
        {
            get
            {
                return Count <= 0;
            }
        }
    }

    public enum ItemType
    {
        GoldCoin,
        Torch,
    }

    public static class Items
    {
        public static Item goldCoin = new Item(ItemType.GoldCoin, 205).SetMaxStackSize(999);
        public static Item torch = new Item(ItemType.Torch, 180).SetOnUse((Player p, ItemStack s) => { p.AddBuff(new Buff(BuffType.Torch, 20)); s.Count -= 1; });
    }
}
