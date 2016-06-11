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

        public List<Particle> particles;

        private Texture2D dot; // текстура точки
        private Texture2D smoke; // текстура дыма

        private Random random;

        public ParticleController()
        {
            this.particles = new List<Particle>();
            random = new Random();
        }

        public void LoadContent(ContentManager Manager)
        {
            dot = Manager.Load<Texture2D>("spark");
            smoke = Manager.Load<Texture2D>("smoke");

        }

        public void EngineRocket(Vector2 position) // функци€, котора€ будет генерировать частицы
        {
            /*for (int a = 0; a < 2; a++) // создаем 2 частицы дыма дл€ трейла
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), 0.6f);
                float angle = 0;
                float angleVel = 0;
                Vector4 color = new Vector4(1f, 1f, 1f, 1f);
                float size = 1f;
                int ttl = 40;
                float sizeVel = 0;
                float alphaVel = 0;


                GenerateNewParticle(smoke, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel);
            }*/

            for (int a = 0; a < 20; a++) // создаем 1 искру дл€ трейла
            {
                Vector2 velocity = AngleToV2((float)(Math.PI * 2d * random.NextDouble()), .2f);
                float angle = 0;
                float angleVel = 0;
                Vector4 color = new Vector4(1.0f, 0.5f, 0.5f, 0.5f);
                float size = 1f;
                int ttl = 80;
                float sizeVel = 0;
                float alphaVel = .01f;


                GenerateNewParticle(dot, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel);
            }

            /*for (int a = 0; a < 10; a++) // создаем 10 дыма, но на практике Ч реактивна€ стру€ дл€ трейла
            {
                Vector2 velocity = Vector2.Zero;
                float angle = 0;
                float angleVel = 0;
                Vector4 color = new Vector4(1.0f, 0.5f, 0.5f, 1f);
                float size = 0.1f + 1.8f * (float)random.NextDouble();
                int ttl = 10;
                float sizeVel = -.05f;
                float alphaVel = .01f;


                GenerateNewParticle(smoke, position, velocity, angle, angleVel, color, size, ttl, sizeVel, alphaVel);
            }*/
        }

        private Particle GenerateNewParticle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Vector4 color, float size, int ttl, float sizeVel, float alphaVel) // генераци€ новой частички
        {
            Particle particle = new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl, sizeVel, alphaVel);
            particles.Add(particle);
            return particle;
        }

        public void Update(GameTime gameTime)
        {

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Size <= 0 || particles[particle].TTL <= 0) // если врем€ жизни частички или еЄ размеры равны нулю, удал€ем еЄ
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive); // ставим режим смешивани€ Addictive

            for (int index = 0; index < particles.Count; index++) // рисуем все частицы
            {
                particles[index].Draw(spriteBatch);
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

        public IEnumerable<LightSource> GetLightSources()
        {
            throw new NotImplementedException();
        }
    }
}
