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

        private Texture2D sparkTexture; 
        //private Texture2D smoke; 

        private Random random;

        public ParticleController()
        {
            this.particles = new List<Particle>();
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

        private Particle GenerateNewParticle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Vector4 color, float size, int ttl, float sizeVel, float alphaVel, bool isLight, float lightStrength, float lightStrengthVelocity, float lightMax, float lightMaxVel) // генерация новой частички
        {
            Particle particle = new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl, sizeVel, alphaVel, isLight, lightStrength, lightStrengthVelocity, lightMax, lightMaxVel);
            particles.Add(particle);
            return particle;
        }

        public void Update(GameTime gameTime)
        {

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Size <= 0 || particles[particle].TTL <= 0) // если время жизни частички или её размеры равны нулю, удаляем её
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 globalToScreen)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive); // ставим режим смешивания Addictive

            for (int index = 0; index < particles.Count; index++) // рисуем все частицы
            {
                particles[index].Draw(spriteBatch, globalToScreen);
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
            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];
                if (particle.IsLighting)
                {
                    result.Add(new LightSource() {Max = particle.LightMax, Position = new Point((int)particle.Position.X, (int)particle.Position.Y), Strenght = (int)particle.LightStrength });
                }
            }
            return result.ToArray();
        }
    }
}
