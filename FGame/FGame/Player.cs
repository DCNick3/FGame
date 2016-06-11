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
            Inventory = new ItemStack[40];
            HP = 100;
            MaxHP = 100;
            SkillUsePoint = 100;
            MaxSkillUsePoint = 100;
            SwordColor = Color.DarkGreen;
        }

        private bool _canMove = true;
        private bool _isSwordCasted;
        private float _swordPos = 0;
        private const float _swordSpeedC = 0.03F;
        private float _swordSpeed = 0;
        private int _maxSword = 48;
        public Color SwordColor { get; set; }
        public int SwordDirection { get; set; }
        public int GetSwordLength(GameTime gameTime)
        {
            if (_isSwordCasted)
            {
                _swordPos += _swordSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_swordPos >= _maxSword)
                {
                    _swordSpeed = -_swordSpeed;
                }
                if (_swordPos <= 0)
                {
                    _swordSpeed = 0;
                    _swordPos = 0;
                    _isSwordCasted = false;
                }
                return (int)_swordPos;
            }
            else
                return 0;
        }
        public bool CanMove
        {
            get
            {
                return _canMove && !_isSwordCasted;
            }
        }
        public bool IsSwordCasted { get { return _isSwordCasted; } }
        internal bool isSpeedHack = false;
        public float Speed { get { return isSpeedHack ? 0.48F : 0.12F; } }
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
        public bool CanUseSwords
        {
            get
            {
                return Type == 0;
            }
        }
        public ItemStack[] Inventory { get; private set; }

        private int hudSlot;
        private int invSlot;

        public int SelectedSlot
        {
            get
            {
                if (IsInInventory)
                    return invSlot;
                else
                    return hudSlot;
            }
            set
            {
                if (IsInInventory)
                    invSlot = value;
                else
                    hudSlot = value;
            }
        }

        public bool IsInInventory { get; set; }

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
        public bool UseSkill(int skillN, GameTime gameTime)
        {
            if (skillN == 0 && SkillUsePoint >= 10 && gameTime.TotalGameTime - lastSkillUses[skillN] >= SkillCooldown(skillN))
            {
                lastSkillUses[skillN] = gameTime.TotalGameTime;
                SkillUsePoint -= 10;
                return true;
            }
            else
            if (skillN == 1 && SkillUsePoint >= 20 && gameTime.TotalGameTime - lastSkillUses[skillN] >= SkillCooldown(skillN))
            {
                lastSkillUses[skillN] = gameTime.TotalGameTime;
                SkillUsePoint -= 20;
                return true;
            }
            else
                return false;
        }

        public void ValidateInventory()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] != null && Inventory[i].IsEmpty)
                    Inventory[i] = null;
            }
        }

        public bool AddItem(ItemStack item)
        {
            int s = 0;
            while (s < 40 && Inventory[s] != null) { s++; }
            if (s == 40) return false;
            Inventory[s] = item;
            return true;
        }

        public void CastSword(int length, float speed, int swordDirection)
        {
            _isSwordCasted = true;
            _maxSword = length;
            _swordPos = 0;
            _swordSpeed = speed;
            SwordDirection = RunDirection;
        }

        public void Kill()
        {
            //Game over
        }

        internal void UnCastSword()
        {
            _isSwordCasted = false;
            _swordSpeed = 0;
            _swordPos = 0;
        }
    }

    public class SwordDrawInfo
    {
        public SwordDrawInfo()
        {

        }

        public double Rotation { get; set; }
        public int Length { get; set; }
    }
}