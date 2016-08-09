using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public static class Extensions
    {
        public static Point ToPoint(this Vector2 vect)
        {
            return new Point((int)vect.X, (int)vect.Y);
        }
        public static System.Drawing.Point ToDrawingPoint(this Vector2 vect)
        {
            return new System.Drawing.Point((int)vect.X, (int)vect.Y);
        }
        public static Vector2 ToVector2(this Point pnt)
        {
            return new Vector2(pnt.X, pnt.Y);
        }
        public static Vector2 ToVector2(this System.Drawing.Point pnt)
        {
            return new Vector2(pnt.X, pnt.Y);
        }
        public static FloatRectangle ToFloatRect(this Rectangle rect)
        {
            return new FloatRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
