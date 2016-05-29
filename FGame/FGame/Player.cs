using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FGame
{
    public class Player
    {
        public Player()
        {
            Buffs = new List<Buff>();
            HP = 100;
            MaxHP = 100;
            SkillUsePoint = 100;
            MaxSkillUsePoint = 100;
        }

        public int Type { get; internal set; }
        public Vector2 Position { get; internal set; }
        public int RunDirection { get; internal set; }
        public List<Buff> Buffs { get; internal set; }
        public float HP { get; internal set; }
        public float MaxHP { get; internal set; }
        public float SkillUsePoint { get; internal set; }
        public float MaxSkillUsePoint { get; internal set; }
        public float SkillUsePointPerSec
        {
            get
            {
                return 5f;
            }
        }

        public Color BarColor
        {
            get
            {
                return Type == 1 ? Color.Blue : (Type == 0 ? Color.Green : Color.Black);
            }
        }

        public int LightStrength
        {
            get
            {
                return ((Type == 1 ? 8 : 6) + (Buffs.Where((Buff f) => f.Type == BuffType.Torch).Count() > 0 ? 5 : 0)) * 32;
            }
        }
        public float LightMax
        {
            get
            {
                return ((Buffs.Where((Buff f) => f.Type == BuffType.Torch).Count() > 0 ? 1.5f : 1f));
            }
        }

        public TimeSpan SkillCooldown(int n)
        {
            if (n == 0)
                return TimeSpan.FromSeconds(0.5);
            if (n == 1)
                return TimeSpan.FromSeconds(15);
            else
                return default(TimeSpan);
        }

        public void AddBuff(Buff buff)
        {
            if (Buffs.Where((Buff f) => f.Type == buff.Type).Count() == 0)
                Buffs.Add(buff);
            else
            {
                Buff x = Buffs.Where((Buff f) => f.Type == buff.Type).First();
                if (x.LastedTime < buff.LastedTime)
                    x.LastedTime = buff.LastedTime;
            }
        }

        TimeSpan[] lastSkillUses = new TimeSpan[2];
        public bool UseSkill(int skillN, TimeSpan totalGameTime)
        {
            if (skillN == 0 && SkillUsePoint >= 10 && totalGameTime - lastSkillUses[skillN] >= SkillCooldown(skillN))
            {
                lastSkillUses[skillN] = totalGameTime;
                SkillUsePoint -= 10;
                return true;
            }
            else
            if (skillN == 1 && SkillUsePoint >= 20 && totalGameTime - lastSkillUses[skillN] >= SkillCooldown(skillN))
            {
                lastSkillUses[skillN] = totalGameTime;
                SkillUsePoint -= 20;
                return true;
            }
            else
                return false;
        }

        public void Kill()
        {
            //Game over
        }
        //TODO: Add inventory!
    }
}