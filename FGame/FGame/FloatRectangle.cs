using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class FloatRectangle
    {
        private Vector2 position;
        private float size;

        public FloatRectangle() : this(0,0,0,0)
        { }

        public FloatRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public FloatRectangle(Vector2 position, Vector2 size) : this(position.X, position.Y, size.X, size.Y)
        {}

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        //public bool Intersects(FloatRectangle rect)
        //{
         //   return !(
           //     rect.X > X + Width  ||
             //   rect.Y > Y + Height ||
               // rect.Y + rect.Height < Y ||
                //rect.X + rect.Width  < X
                //); 
        //}

        public bool Intersects(FloatRectangle rect)
        {
            return rect.X < this.X + this.Width && this.X < rect.X + rect.Width && rect.Y < this.Y + this.Height && this.Y < rect.Y + rect.Height;
        }

        public bool IsInside(FloatRectangle rect)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= X && point.X < X + Width
                && point.Y >= Y && point.Y < Y + Height;
        }
    }
}
