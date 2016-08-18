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
using FGame;

namespace FGame.ParticleSystem
{
    public class ParticleController
    {
        private Particle[] _pool;
        private const int _poolAllocSize = 16;
        //public List<Particle> particles;

        private Texture2D sparkTexture; 
        //private Texture2D smoke; 

        private Random random;

        public int ActiveParticles { get; internal set; }

        public ParticleController()
        {
            _pool = new Particle[_poolAllocSize];
            random = new Random();
        }

        public void LoadContent(ContentManager Manager)
        {
            sparkTexture = Manager.Load<Texture2D>("spark");
            //smoke = Manager.Load<Texture2D>("smoke");
        }

        public void FireballFlySparks(Vector2 position)
        {
            if (random.Next(2) == 1)
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), (float)random.NextDouble() * 0.5f);
                float angle = 0;
                float angleVel = (float)(Math.PI / 20);
                Vector4 color = new Vector4(1.0f, 0.5f, 0.5f, 0.5f);
                float size = 2f;
                int ttl = 80;
                float sizeVel = 0;
                float alphaVel = .01f;
                bool isLight = true;
                float light = 60.0f;
                float lightVel = -1.0f;
                float lightMax = 1f;
                float lightMaxVel = -0.015f;


                GenerateNewParticle(sparkTexture, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel, isLight, light, lightVel, lightMax, lightMaxVel);
            }
        }

        public void PlayerWizardManaSparksManaCast(Vector2 position, int manaCount)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), (float)random.NextDouble() * 0.5f);
                float angle = 0;
                float angleVel = (float)(Math.PI * 2d * random.NextDouble() / 45.0);
                Vector4 color = new Vector4(0.5f, 0.5f, 1f, 0.5f);
                float size = 3f;
                int ttl = 240;
                float sizeVel = 0;
                float alphaVel = .003f;
                bool isLight = true;
                float light = 60f;
                float lightVel = -0.3f;
                float lightMax = 0.5f;
                float lightMaxVel = -0.0005f;


                GenerateNewParticle(sparkTexture, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel, isLight, light, lightVel, lightMax, lightMaxVel);
            }
        }

        public void PlayerWizardManaSparks(Vector2 position)
        {
            if (random.Next(20) == 1)
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), (float)random.NextDouble() * 0.5f);
                float angle = 0;
                float angleVel = (float)(Math.PI * random.NextDouble() / 45.0);
                Vector4 color = new Vector4(0.5f, 0.5f, 1f, 0.5f);
                float size = 3f;
                int ttl = 240;
                float sizeVel = 0;
                float alphaVel = .003f;
                bool isLight = true;
                float light = 20f;
                float lightVel = -0.2f;
                float lightMax = 0.5f;
                float lightMaxVel = -0.0005f;


                GenerateNewParticle(sparkTexture, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel, isLight, light, lightVel, lightMax, lightMaxVel);
            }
        }

        public void FireballDestroySparks(Vector2 position)
        {
            for (int a = 0; a < 40; a++)
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), (float)random.NextDouble() * 0.5f);
                float angle = 0;
                float angleVel = (float)(Math.PI * 2d * random.NextDouble() / 45.0);
                Vector4 color = new Vector4(1.0f, 0.5f, 0.5f, 0.5f);
                float size = 2f;
                int ttl = 80;
                float sizeVel = 0;
                float alphaVel = .01f;
                bool isLight = true;
                float light = 640f;
                float lightVel = -12.0f;
                float lightMax = 1f;
                float lightMaxVel = -0.015f;


                GenerateNewParticle(sparkTexture, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel, isLight, light, lightVel, lightMax, lightMaxVel);
            }
        }

        private void GenerateNewParticle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Vector4 color, float size, int ttl, float sizeVel, float alphaVel, bool isLight, float lightStrength, float lightStrengthVelocity, float lightMax, float lightMaxVel) // генерация новой частички
        {
            Particle particle = new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl, sizeVel, alphaVel, isLight, lightStrength, lightStrengthVelocity, lightMax, lightMaxVel);
            PushParticle(particle);
        }

        private void PushParticle(Particle particle)
        {
            particle.isUsed = true;
            Particle _particle;
            int n = 0;
            retry:
            do
            {
                _particle = _pool[n];
                if (!_particle.isUsed)
                    break;
                n++;
            } while (n < _pool.Length);
            if (_particle.isUsed)
            {
                PoolAlloc();
                goto retry;
            }
            _pool[n] = particle;
        }

        private void PoolAlloc()
        {
            Particle[] newArray = new Particle[_pool.Length + _poolAllocSize];
            Array.Copy(_pool, newArray, _pool.Length);
            _pool = newArray;
        }

        public void Update(GameTime gameTime)
        {
            int activeParticles = 0;
            for (int i = 0; i < _pool.Length; i++)
            {
                Particle particle = _pool[i];
                if (particle.isUsed)
                {
                    activeParticles++;
                    particle.Update();
                    if (particle.Size <= 0 || particle.TTL <= 0)
                    {
                        particle.isUsed = false;
                    }
                }

                _pool[i] = particle;
            }
            ActiveParticles = activeParticles;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 globalToScreen, Vector2 screenSize)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive); // ставим режим смешивания Addictive

            for (int i = 0; i < _pool.Length; i++) // рисуем все частицы
            {
                Particle particle = _pool[i];
                if (particle.isUsed)
                {
                    FloatRectangle screenRect = new FloatRectangle(globalToScreen, screenSize);
                    if (screenRect.Contains(particle.Position))
                        particle.Draw(spriteBatch, globalToScreen);

                }
                _pool[i] = particle;
            }

            spriteBatch.End();
        }

        public Vector2 AngleToV2(float angle, float length)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(angle) * length;
            direction.Y = -(float)Math.Sin(angle) * length;
            return direction;
        }

        public LightSource[] GetLightSources()
        {
            List<LightSource> result = new List<LightSource>();
            for (int i = 0; i < _pool.Length; i++)
            {
                var particle = _pool[i];
                if (particle.isUsed && particle.IsLighting)
                {
                    result.Add(new LightSource() {Max = particle.LightMax, Position = new Point((int)particle.Position.X, (int)particle.Position.Y), Strenght = (int)particle.LightStrength });
                }
            }
            return result.ToArray();
        }
    }
}
