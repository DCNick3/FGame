using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{
    public abstract class GamePoleStaticObject
    {
        public abstract Vector2 Position { get; }
        public abstract Vector2 Size { get; }
        public abstract bool IsObstacle { get; }
        public FloatRectangle Rectangle { get { return new FloatRectangle(Position.X, Position.Y, Size.X, Size.Y); } }
        public virtual void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos) { }
        public virtual void Update(GameRegistry registry, GameTime gameTime) { }
        public int Layer { get; protected set; }
        /* Layers:
         * 0 to 5   - tiles
         * 6 to 10  - tile effects
         * 11 to 15 - chests, tables etc
         * */
    }

    public class GamePoleObjectTile : GamePoleStaticObject
    {
        public GamePoleObjectTile(Vector2 position, int type, bool isObstacle, int layer)
        {
            _isObstacle = isObstacle;
            _position = position;
            _type = type;
            Layer = layer;
        }

        private const int _width = 32;
        private const int _height = 32;
        private bool _isObstacle;
        private int _type;
        private Vector2 _position;
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

        public override Vector2 Position
        {
            get
            {
                return _position;
            }
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
            Vector2 origin = Size / 2f;
            spriteBatch.Draw(tiles, pos, src, color, 0f, origin, 1f, SpriteEffects.None, 0);
        }
    }

    public class GamePoleObjectChest : GamePoleStaticObject
    {
        public GamePoleObjectChest(Vector2 position, int layer, int type)
        {
            _position = position;
            Layer = layer;
        }
        private Vector2 _position;
        private const int _width = 32;
        private const int _height = 32;
        public override bool IsObstacle
        {
            get
            {
                return false;
            }
        }

        public override Vector2 Position
        {
            get
            {
                return _position;
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
            return new Rectangle(Type * _width, (AnimationFrame) * (_height + 16), _width, _height + 16);
        }

        public int Type { get; set; }
        public bool IsOpen { get; set; }
        public int AnimationFrame { get; set; }
        public override void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D chest = registry.GetTexture("chest");
            Rectangle src = GetSourceRect();
            Vector2 pos = Position - screenPos;
            Vector2 origin = Size / 2f;
            spriteBatch.Draw(chest, pos, src, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
        }
    }
}
