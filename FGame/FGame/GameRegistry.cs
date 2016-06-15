using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace FGame
{
    public class GameRegistry
    {
        public GameRegistry()
        {
            _textures = new TextureRegistry();
        }
        private class TextureRegistry
        {
            public TextureRegistry()
            {
                _registry = new Dictionary<string, Texture2D>();
            }

            private Dictionary<string, Texture2D> _registry;
            public Texture2D Get(string name)
            {
                Texture2D res = null;
                _registry.TryGetValue(name, out res);
                return res;
            }
            public void Set(string name, Texture2D val)
            {
                _registry[name] = val;
            }
        }

        private TextureRegistry _textures;
        public Texture2D GetTexture(string name)
        {
            return _textures.Get(name);
        }

        public void SetTexture(string name, Texture2D value)
        {
            _textures.Set(name, value);
        }
    }
}