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
        public static Vector2 ToVector2(this Point pnt)
        {
            return new Vector2(pnt.X, pnt.Y);
        }
    }
}
