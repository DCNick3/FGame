using System;
using Microsoft.Xna.Framework;

namespace FGame
{
    public class Buff
    {
        public const float PoisonDmgPS = 1.0f;

        public Buff(BuffType type, int secs)
        {
            Type = type;
            LastedTime = TimeSpan.FromSeconds(secs);
        }
        public BuffType Type { get; internal set; }
        public TimeSpan LastedTime { get; internal set; }

        public string GetStringTime()
        {
            if (LastedTime.TotalSeconds <= 99)
            {
                return (int)Math.Ceiling(LastedTime.TotalSeconds) + "s";
            }
            if (LastedTime.TotalMinutes <= 99)
            {
                return (int)Math.Ceiling(LastedTime.TotalSeconds) + "m";
            }
            return "MANY";
        }
    }

    public enum BuffType
    {
        Torch,
        Poison,
    }
}