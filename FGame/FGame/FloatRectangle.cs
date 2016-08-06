using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public class FloatRectangle
    {
        public FloatRectangle() : this(0,0,0,0)
        { }

        public FloatRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public bool Intersects(FloatRectangle rect)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(Vector2 point)
        {
            return point.X > X && point.X < X + Width && point.Y > Y && point.Y < Y + Height;
        }

        public bool IsInside(FloatRectangle rect)
        {
            throw new NotImplementedException();
        }
    }
}
