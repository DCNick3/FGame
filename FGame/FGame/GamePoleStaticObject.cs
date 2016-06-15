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
        public abstract Point Position { get; }
        public abstract Point Size { get; }
        public abstract bool IsObstacle { get; }
        public Rectangle Rectangle { get { return new Rectangle(Position.X, Position.Y, Size.X, Size.Y); } }
        public virtual void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos) { }
        public virtual void Update(GameRegistry registry, GameTime gameTime) { }
    }

    public class GamePoleObjectTile : GamePoleStaticObject
    {
        public GamePoleObjectTile(Point position, int type, bool isObstacle)
        {
            _isObstacle = isObstacle;
            _position = position;
            _type = type;
        }

        private const int _width = 32;
        private const int _height = 32;
        private bool _isObstacle;
        private int _type;
        private Point _position;
        public override bool IsObstacle
        {
            get
            {
                return _isObstacle;
            }
        }

        private Rectangle GetSourceRect()
        {
            int width = 8;
            int x = _type % width;
            int y = _type / width;
            return new Rectangle(x * _width, y * _height, _width, _height);
        }

        public override Point Position
        {
            get
            {
                return _position;
            }
        }

        public override Point Size
        {
            get
            {
                return new Point(_width, _height);
            }
        }

        public override void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D tiles = registry.GetTexture("tiles");
            Rectangle src = GetSourceRect();
            Vector2 pos = Position.ToVector2() - screenPos;
            Color color = _isObstacle ? Color.White * 0.75f : Color.White;
            spriteBatch.Draw(tiles, pos, src, color);
        }
    }

    public class GamePoleObjectChest : GamePoleStaticObject
    {
        public GamePoleObjectChest(Point position)
        {
            _position = position;
        }
        private Point _position;
        private const int _width = 32;
        private const int _height = 32;
        public override bool IsObstacle
        {
            get
            {
                return false;
            }
        }

        public override Point Position
        {
            get
            {
                return _position;
            }
        }

        public override Point Size
        {
            get
            {
                return new Point(_width, _height);
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
            Vector2 pos = Position.ToVector2() - screenPos;
            //Vector2 pos = ScreenCenter - player.Position + new Vector2(chest.GlobPosition.X * _width, chest.GlobPosition.Y * _height - 16);
            Point center = new Point(Position.X * _width + _width / 2, Position.Y * _height + _height / 2 - 16);
            spriteBatch.Draw(chest, pos, src, Color.White);
        }
    }
}
