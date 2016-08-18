using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FGame.ParticleSystem
{
    public struct Particle
    {
        public Texture2D Texture { get; set; }      
        public Vector2 Position { get; set; }       
        public Vector2 Velocity { get; set; }       
        public float Angle { get; set; }            
        public float AngularVelocity { get; set; }    
        public Vector4 Color { get; set; }            
        public float Size { get; set; }
        public float SizeVel { get; set; }		
        public float AlphaVel { get; set; }		
        public int TTL { get; set; }                
        public bool IsLighting { get; set; }
        public float LightStrength { get; set; }
        public float LightStrengthVelocity { get; set; }
        public float LightMax { get; set; }
        public float LightMaxVelocity { get; set; }
        internal bool isUsed;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Vector4 color, float size, int ttl, float sizeVel, float alphaVel, bool isLight, float lightStrength, float lightStrengthVelocity, float lightMax, float lightMaxVel)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            Color = color;
            AngularVelocity = angularVelocity;
            Size = size;
            SizeVel = sizeVel;
            AlphaVel = alphaVel;
            TTL = ttl;
            IsLighting = isLight;
            LightStrength = lightStrength;
            LightStrengthVelocity = lightStrengthVelocity;
            LightMax = lightMax;
            LightMaxVelocity = lightMaxVel;
            isUsed = false;
        }

        public void Update() // цикл обновления
        {
            TTL--; // уменьшаем время жизни

            // Меняем параметры в соответствии с скоростями
            Position += Velocity;
            Angle += AngularVelocity;
            Size += SizeVel;
            LightStrength += LightStrengthVelocity;
            LightMax += LightMaxVelocity;
            if (LightStrength <= 1)
                IsLighting = false;
            if (LightMax <= 0)
                IsLighting = false;

            Color = new Vector4(Color.X, Color.Y, Color.Z, Color.W - AlphaVel); // убавляем цвет. Кстати, цвет записан в Vector4, а не в Color, потому что: Color.R/G/B имеет тип Byte (от 0x00 до 0xFF), чтобы не проделывать лишней трансформации, используем float и Vector4

        }


        public void Draw(SpriteBatch spriteBatch, Vector2 globalToScreen)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2); 

            spriteBatch.Draw(Texture, Position - globalToScreen, sourceRectangle, new Color(Color),
                Angle, origin, Size, SpriteEffects.None, 0); 

        }

    }
}
