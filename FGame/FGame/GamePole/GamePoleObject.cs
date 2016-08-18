using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public abstract class GamePoleObject
    {
        public GamePoleObject()
        {
            UUID = Guid.NewGuid();
        }

        private Vector2 _pos;
        public Vector2 Position
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                Moved = true;
            }
        }
        internal bool Moved { set; get; }
        public abstract Vector2 Size { get; }
        public abstract bool IsObstacle { get; }
        public FloatRectangle Rectangle { get { return new FloatRectangle(Position.X, Position.Y, Size.X, Size.Y); } }
        public virtual void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos) { }
        public virtual void Update(GameRegistry registry, GameTime gameTime) { }
        public virtual void Interact(Player player) { }
        public virtual void Collide(Player player) { }
        public int Layer { get; protected set; }
        public Guid UUID { get; private set; }

        public void __hackSetLayer(int val)
        {
            Layer = val;
        }
        /* Layers:
         * 0 to 5   - tiles
         * 6 to 10  - tile effects
         * 11 to 15 - chests, tables etc
         * */

        public override string ToString()
        {
            return GetType() + ", " + UUID + ", " + Position;
        }
    }

    public class GamePoleObjectTile : GamePoleObject
    {
        public GamePoleObjectTile(Vector2 position, int type, bool isObstacle, int layer)
        {
            _isObstacle = isObstacle;
            Position = position;
            _type = type;
            Layer = layer;
        }

        private const int _width = 32;
        private const int _height = 32;
        private bool _isObstacle;
        private int _type;
        public override bool IsObstacle
        {
            get
            {
                return _isObstacle;
            }
        }
        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private Rectangle GetSourceRect()
        {
            int width = 8;
            int x = _type % width;
            int y = _type / width;
            return new Rectangle(x * _width, y * _height, _width, _height);
        }

        public override Vector2 Size
        {
            get
            {
                return new Vector2(_width, _height);
            }
        }

        public override void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D tiles = registry.GetTexture("tiles");
            Rectangle src = GetSourceRect();
            Vector2 pos = Position - screenPos;
            Color color = _isObstacle ? Color.White * 0.75f : Color.White;
            Vector2 origin = Vector2.Zero;
            spriteBatch.Draw(tiles, pos, src, color, 0f, origin, 1f, SpriteEffects.None, 0);
        }

        public override void Update(GameRegistry registry, GameTime gameTime)
        {
        }

        public override string ToString()
        {
            return base.ToString() + " Type: " + Type;
        }
    }

    public class GamePoleObjectChest : GamePoleObject
    {
        public GamePoleObjectChest(Vector2 position, int layer, int type)
        {
            Position = position;
            Layer = layer;
            Type = type;
        }
        private TimeSpan animationFrameLength = TimeSpan.FromSeconds(0.2);
        private TimeSpan lastAnimationFrame;
        private bool isOpening = false;
        private Player openingBy = null;
        private const int _width = 32;
        private const int _height = 48;
        public override bool IsObstacle
        {
            get
            {
                return false;
            }
        }

        public override Vector2 Size
        {
            get
            {
                return new Vector2(_width, _height);
            }
        }

        private Rectangle GetSourceRect()
        {
            return new Rectangle(Type * _width, (AnimationFrame) * (_height), _width, _height);
        }

        public int Type { get; set; }
        public bool IsOpen { get; set; }
        public int AnimationFrame { get; set; }
        public override void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D chest = registry.GetTexture("chest");
            Rectangle src = GetSourceRect();
            Vector2 pos = Position - screenPos;
            Vector2 origin = Vector2.Zero;
            spriteBatch.Draw(chest, pos, src, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
        }

        public override void Update(GameRegistry registry, GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastAnimationFrame >= animationFrameLength && isOpening)
            {
                lastAnimationFrame = gameTime.TotalGameTime;

                if (AnimationFrame < 3)
                    AnimationFrame++;
                else
                {
                    isOpening = false;
                    openingBy.AddItem(new ItemStack(Items.goldCoin, 2));
                }
            }
        }

        public override void Interact(Player player)
        {
            isOpening = true;
            openingBy = player;
        }
    }
}
