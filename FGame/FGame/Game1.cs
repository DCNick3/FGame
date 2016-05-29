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

namespace FGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;
        Texture2D tileTexture;
        Texture2D fireballTexture;
        Texture2D chestTexture;
        Texture2D stuffTexture;
        Texture2D whitePixel;
        SpriteFont font;
        Effect smoothLightEffect;
        int tileWidth = 32;
        int tileHeight = 32;
        int chestWidth = 32;
        int chestHeight = 32;
        int stuffWidth = 24;
        int stuffHeight = 24;
        int stuffPerLine = 16;
        Player player;
        int playerTextureWidth = 32;
        int playerTextureHeight = 32;
        int screenWidth = 800;
        int screenHeight = 600;
        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;
        TimeSpan runFreq = TimeSpan.FromSeconds(0.3);
        TimeSpan lastRunUpdate = new TimeSpan(0);
        int playerRunStep = 1;
        bool runStepDirection = false;
        float playerMoveSpeed = 2;
        bool enableSmoothLightning = true;
        bool cheatFullBright = false;
        bool debugInfo = false;
        bool cheatSpeedHack = false;
        bool enableDevCheats = true;
        Random rnd = new Random();
        GamePole gamePole;
        List<Fireball> fireballs = new List<Fireball>();
        LightSource[] lightSources;
        TimeSpan fireballStepFreq = TimeSpan.FromSeconds(0.1);
        TimeSpan lastFireballStepUpdate = new TimeSpan(0);
        TimeSpan fireballMoveFreq = TimeSpan.FromSeconds(0.002);
        TimeSpan lastFireballMoveUpdate = new TimeSpan(0);
        TimeSpan fireballSpawnCooldown = TimeSpan.FromSeconds(0.5);
        TimeSpan lastFireballSpawn = new TimeSpan(0);
        TimeSpan chestAnimationSpeed = TimeSpan.FromSeconds(0.2);
        TimeSpan lastChestAnimation = new TimeSpan(0);

        /*
         * TODO: Block
         * Add smooth light (Done a bit)
         * Add bauble hint
         * Add chunked-world
         * Normal onKeyPress (done)
         * Tree world generating algorithm
         * Add abstraction to GamePole and Chunk!
         */

        Vector2 ScreenCenter {
            get
            {
                return new Vector2(graphics.PreferredBackBufferWidth / 2.0F, graphics.PreferredBackBufferHeight / 2.0F);
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            player = new Player();
            player.Type = 1;
            RegenPole();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("players");
            tileTexture = Content.Load<Texture2D>("tiles");
            fireballTexture = Content.Load<Texture2D>("fireballs");
            chestTexture = Content.Load<Texture2D>("chest");
            smoothLightEffect = Content.Load<Effect>("light");
            stuffTexture = Content.Load<Texture2D>("stuff");
            font = Content.Load<SpriteFont>("font");
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        private bool KeyPress(Keys key)
        {
            return lastKeyboardState != null && lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            bool isMoving = false;
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                PlayerMove(3);
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                PlayerMove(2);
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                PlayerMove(1);
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                PlayerMove(0);
                isMoving = true;
            }

            if (KeyPress(Keys.Q))
            {
                if (player.Type == 1 && player.UseSkill(0, gameTime.TotalGameTime))
                {
                    fireballs.Add(new Fireball() { X = (int)player.Position.X, Y = (int)player.Position.Y, Direction = player.RunDirection, Speed = 3, Step = 0 });
                }
            }

            if (keyboardState.IsKeyDown(Keys.OemTilde))
            {
                RegenPole();
            }

            if (gameTime.TotalGameTime - lastRunUpdate > runFreq)
            {
                if (isMoving)
                    if (runStepDirection)
                    {
                        if (playerRunStep < 2)
                            playerRunStep++;
                        else
                        {
                            runStepDirection = !runStepDirection;
                            playerRunStep--;
                        }
                    }
                    else
                    {
                        if (playerRunStep > 0)
                            playerRunStep--;
                        else
                        {
                            runStepDirection = !runStepDirection;
                            playerRunStep++;
                        }
                    }
                else
                {
                    playerRunStep = 1;
                }
                lastRunUpdate = gameTime.TotalGameTime;
            }
            UpdateFireballs(gameTime);
            lightSources = GetLightSources();

            Rectangle playerRect = new Rectangle((int)player.Position.X, (int)player.Position.Y, playerTextureWidth, playerTextureHeight);
            if (KeyPress(Keys.E))
                foreach (var chest in gamePole.Chests)
                {
                    Rectangle chestRect = new Rectangle(chest.GlobPosition.X * chestWidth, chest.GlobPosition.Y * chestHeight, chestWidth, chestHeight);
                    if (chestRect.Intersects(playerRect) && !chest.IsOpen)
                    {
                        chest.OpenStart(player);
                        break;
                    }
                }

            if (gameTime.TotalGameTime - lastChestAnimation > chestAnimationSpeed)
            {
                foreach (var chest in gamePole.Chests)
                {
                    if (chest.AnimationFrame != 0 && chest.AnimationFrame != 3)
                    {
                        chest.AnimationFrame++;
                        if (chest.AnimationFrame == 3)
                        {
                            chest.OpenEnd(player);
                        }
                    }
                }
                lastChestAnimation = gameTime.TotalGameTime;
            }

            for (int i = 0; i < player.Buffs.Count; i++)
            {
                var buff = player.Buffs[i];
                if (buff.LastedTime <= TimeSpan.Zero)
                {
                    player.Buffs.Remove(buff);
                    i--;
                    continue;
                }
                buff.LastedTime -= gameTime.ElapsedGameTime;
            }

            if (KeyPress(Keys.T))
            {
                if (player.Type == 1 && player.UseSkill(1, gameTime.TotalGameTime))
                    player.AddBuff(new Buff(BuffType.Torch, 10));
            }

            if (player.HP < 0)
                player.Kill();

            if (player.SkillUsePoint < player.MaxSkillUsePoint)
                player.SkillUsePoint += player.SkillUsePointPerSec * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (player.SkillUsePoint > player.MaxSkillUsePoint)
                player.SkillUsePoint = player.MaxSkillUsePoint;

            if (KeyPress(Keys.OemComma))
                enableSmoothLightning = !enableSmoothLightning;

            if (player.Buffs.Where((Buff f) => f.Type == BuffType.Poison).Count() > 0)
            {
                player.HP -= (float)(Buff.PoisonDmgPS * gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (enableDevCheats && KeyPress(Keys.Z))
            {
                //Healing Potion
                player.HP += 50;
                if (player.HP > player.MaxHP)
                    player.HP = player.MaxHP;
            }

            if (enableDevCheats && KeyPress(Keys.X))
            {
                //Healing Potion
                player.SkillUsePoint += 50;
                if (player.SkillUsePoint > player.MaxSkillUsePoint)
                    player.SkillUsePoint = player.MaxSkillUsePoint;
            }

            if (enableDevCheats && KeyPress(Keys.C))
            {
                cheatFullBright = !cheatFullBright;
                enableSmoothLightning = !cheatFullBright;
            }

            if (enableDevCheats && KeyPress(Keys.F3))
            {
                debugInfo = !debugInfo;
            }

            if (enableDevCheats && KeyPress(Keys.V))
            {
                cheatSpeedHack = !cheatSpeedHack;
                if (cheatSpeedHack)
                    playerMoveSpeed *= 3;
                else
                    playerMoveSpeed /= 3;
            }

            if (enableDevCheats && KeyPress(Keys.B))
            {
                MouseState s = Mouse.GetState();
                player.Position += new Vector2(s.X, s.Y) - ScreenCenter;
            }

            base.Update(gameTime);
        }

        private Point[] GetCollidingTiles(Rectangle rect)
        {
            //TODO: optimize!~
            //TODO: Negative coord bug
            List<Point> result = new List<Point>();
            for (int x = gamePole.MinX; x < gamePole.MaxX; x++)
                for (int y = gamePole.MinY; y < gamePole.MaxY; y++)
                {
                    Rectangle r = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                    if (r.Intersects(rect))
                        result.Add(new Point(x, y));
                }
            return result.ToArray();
        }

        private void PlayerMove(int direction)
        {
            player.RunDirection = direction;
            Vector2 newPos = player.Position;
            switch (direction)
            {
                case 0:
                    newPos.Y += playerMoveSpeed;
                    break;
                case 1:
                    newPos.X -= playerMoveSpeed;
                    break;
                case 2:
                    newPos.X += playerMoveSpeed;
                    break;
                case 3:
                    newPos.Y -= playerMoveSpeed;
                    break;
            }
            Rectangle pr = new Rectangle((int)newPos.X + 4, (int)newPos.Y + 4, playerTextureWidth - 8, playerTextureHeight - 4);
            bool canMove = true;
            foreach (var coll in GetCollidingTiles(pr))
                canMove &= !gamePole.GetTileAt(new Point(coll.X, coll.Y)).IsObstacle;
            if (canMove)
                player.Position = newPos;
        }

        private void RegenPole()
        {
            gamePole = GamePole.Generate(/*64, 64, 30, 30,*/ rnd, this);
            Point p = gamePole.GetRandomFreePole(rnd);
            player.Position = new Vector2(p.X * playerTextureWidth, p.Y * playerTextureHeight);
        }

        public void UpdateFireballs(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastFireballStepUpdate > fireballStepFreq)
            {
                foreach (var fireball in fireballs)
                {
                    if (fireball.Step < 3)
                        fireball.Step++;
                    else
                        fireball.Step = 0;
                }
                lastFireballStepUpdate = gameTime.TotalGameTime;
            }

            if (gameTime.TotalGameTime - lastFireballMoveUpdate > fireballMoveFreq)
            {
                List<Fireball> td = new List<Fireball>();
                foreach (var fireball in fireballs)
                {
                    Vector2 newPos = new Vector2(fireball.X, fireball.Y);
                    switch (fireball.Direction)
                    {
                        case 0:
                            newPos.Y += fireball.Speed;
                            break;
                        case 1:
                            newPos.X -= fireball.Speed;
                            break;
                        case 2:
                            newPos.X += fireball.Speed;
                            break;
                        case 3:
                            newPos.Y -= fireball.Speed;
                            break;
                    }

                    Rectangle pr = new Rectangle((int)newPos.X + 4, (int)newPos.Y + 4, 32 - 8, 32 - 8);
                    bool canMove = true;
                    foreach (var coll in GetCollidingTiles(pr))
                        canMove &= !gamePole.GetTileAt(new Point(coll.X, coll.Y)).IsObstacle;
                    if (canMove)
                    {
                        fireball.X = (int)newPos.X;
                        fireball.Y = (int)newPos.Y;
                    }
                    else
                    {
                        td.Add(fireball);
                    }
                }
                fireballs.RemoveAll((Fireball f) => td.IndexOf(f) != -1);


                lastFireballMoveUpdate = gameTime.TotalGameTime;
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RenderTarget2D gamePoleImg = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            GraphicsDevice.SetRenderTarget(gamePoleImg);
            GraphicsDevice.Clear(Color.Black);
            
            //smoothLightEffect.Parameters["screenSize"].SetValue(new Vector2(screenWidth, screenHeight));
            //smoothLightEffect.Parameters["liteSource"].SetValue(new Vector4[] { new Vector4(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) * 0.5f, 1.0f, (player.Type == 1 ? 8 : 6) * 32f) });

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone/*, smoothLightEffect*/);

            DrawTiles(lightSources);
            DrawChests(lightSources);
            DrawFireballs();
            DrawPlayer();
            //spriteBatch.Draw(lightnessMap, Vector2.Zero, new Color(255, 255, 255, 127));


            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            if (enableSmoothLightning)
            {
                smoothLightEffect.Parameters["screenSize"].SetValue(new Vector2(screenWidth, screenHeight));
                Vector4[] liteSources = (from ls in lightSources select new Vector4(new Vector2(ls.Position.X, ls.Position.Y) - player.Position + ScreenCenter, ls.Strenght, ls.Max)).ToArray();
                Vector4[] arr = new Vector4[5];

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = i < liteSources.Length ? liteSources[i] : new Vector4(0);
                }
                smoothLightEffect.Parameters["liteSource"].SetValue(arr);
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, smoothLightEffect);
            }
            else
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(gamePoleImg, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //Draw Info Stuff
            DrawHealthbar(player.HP, player.MaxHP);
            DrawSkillUsePointBar(player.SkillUsePoint, player.MaxSkillUsePoint, player.BarColor);
            DrawBuffs(player);
            if (debugInfo)
            {
                Vector2 chnkIdIrr = ((player.Position / new Vector2(tileWidth, tileHeight) + new Vector2(gamePole.MinX, gamePole.MinY)) / new Vector2(Chunk.width, Chunk.height));
                string dbgnfo = "Pos: " + (player.Position).ToString()
                    + "\nIsRunningSlowly: " + gameTime.IsRunningSlowly
                    + "\n Chunk: " + new Vector2((int)chnkIdIrr.X, (int)chnkIdIrr.Y);
                string[] lines = dbgnfo.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string ln = lines[i];
                    Vector2 sz = font.MeasureString(ln);
                    spriteBatch.DrawString(font, ln, new Vector2(screenWidth - sz.X, i * (sz.Y + 1)), Color.White);
                }
            }

            spriteBatch.End();
            
            gamePoleImg.Dispose();

            base.Draw(gameTime);
        }

        private void DrawBuffs(Player player)
        {
            for (int i = 0; i < player.Buffs.Count; i++)
            {
                DrawBuff(player.Buffs[i], new Vector2(i * (1.5f * stuffWidth) + stuffWidth / 2f, 40));
            }
        }

        private void DrawHealthbar(float hP, float maxHP)
        {
            Rectangle pos = new Rectangle(10, 10, 100, 20);
            Rectangle rectCont = new Rectangle(pos.X - 1, pos.Y - 1, pos.Width + 2, pos.Height + 2);
            Rectangle rectRed = new Rectangle(pos.X, pos.Y, (int)(pos.Width * (hP / maxHP)), pos.Height);
            spriteBatch.Draw(whitePixel, rectCont, Color.White);
            spriteBatch.Draw(whitePixel, rectRed, Color.Red);
            spriteBatch.DrawString(font, hP.ToString("###"), new Vector2(pos.X + pos.Width / 2f, pos.Y + pos.Height / 2f), Color.Black, 0, font.MeasureString(hP.ToString("###")) / 2f, 1f, SpriteEffects.None, 0);
        }

        private void DrawSkillUsePointBar(float skillUsePoints, float maxSkillUsePoints, Color color)
        {
            Rectangle pos = new Rectangle(120, 10, 100, 20);
            Rectangle rectCont = new Rectangle(pos.X - 1, pos.Y - 1, pos.Width + 2, pos.Height + 2);
            Rectangle rectRed = new Rectangle(pos.X, pos.Y, (int)(pos.Width * (skillUsePoints / maxSkillUsePoints)), pos.Height);
            spriteBatch.Draw(whitePixel, rectCont, Color.White);
            spriteBatch.Draw(whitePixel, rectRed, color);
            spriteBatch.DrawString(font, skillUsePoints.ToString("###"), new Vector2(pos.X + pos.Width / 2f, pos.Y + pos.Height / 2f), Color.Black, 0, font.MeasureString(skillUsePoints.ToString("###")) / 2f, 1f, SpriteEffects.None, 0);
        }

        private int GetDistance(Point a, Point b)
        {
            return (int)Math.Sqrt(Math.Pow(Math.Abs(a.X - b.X), 2) + Math.Pow(Math.Abs(a.Y - b.Y), 2));
        }

        private float GetLightLevel(Point p, LightSource[] sources)
        {
            if (!enableSmoothLightning & !cheatFullBright)
            {
                float max = 0;
                foreach (var source in sources)
                {
                    int dist = GetDistance(p, source.Position);
                    if (dist > source.Strenght)
                        continue;
                    float x = (source.Max * ((source.Strenght - dist) / (float)(source.Strenght)));
                    if (x > max)
                        max = x;
                }
                return max;
            }
            return 1f;
        }

        private void DrawChests(LightSource[] lightSources)
        {
            foreach (var chest in gamePole.Chests)
            {
                Vector2 pos = ScreenCenter - player.Position + new Vector2(chest.GlobPosition.X * chestWidth, chest.GlobPosition.Y * chestHeight - 16);
                Point center = new Point(chest.GlobPosition.X * chestWidth + chestWidth / 2, chest.GlobPosition.Y * chestHeight + chestHeight / 2 - 16);
                float lightness = GetLightLevel(center, lightSources);
                spriteBatch.Draw(chestTexture, pos, new Rectangle(chest.Type * chestWidth, (chest.AnimationFrame) * (chestHeight + 16), chestWidth, chestHeight + 16), Color.White * lightness);
            }
        }

        private void DrawFireballs()
        {
            int fireballWidth = 75;
            int fireballHeight = 76;
            foreach (var fireball in fireballs)
            {
                Vector2 pos = ScreenCenter - player.Position + new Vector2(fireball.X, fireball.Y);
                spriteBatch.Draw(fireballTexture, new Rectangle((int)pos.X, (int)pos.Y, 32, 32), new Rectangle(fireballWidth * fireball.Step, fireballHeight * fireball.Direction, fireballWidth, fireballHeight), Color.White);
            }
        }

        private LightSource[] GetLightSources()
        {
            List<LightSource> result = new List<LightSource>();

            result.Add(new LightSource() { Position = new Point((int)player.Position.X + playerTextureWidth / 2, (int)player.Position.Y + playerTextureHeight / 2), Strenght = player.LightStrength, Max = (int)(player.LightMax) });
            foreach (var fireball in fireballs)
                result.Add(new LightSource() { Position = new Point(fireball.X, fireball.Y), Strenght = 10 * 32 });
            //smt moar?


            result.Sort((LightSource a, LightSource b) => GetDistance(new Point((int)player.Position.X, (int)player.Position.Y), a.Position).CompareTo(GetDistance(new Point((int)player.Position.X, (int)player.Position.Y), b.Position)));
            return result.ToArray();
        }

        private void DrawPlayer()
        {
            Vector2 plOffset = Vector2.Zero;
            if (player.Type == 1)
                plOffset.X = 192;
            if (player.Type == 2)
                plOffset.Y = 128;
            spriteBatch.Draw(playerTexture, //new Rectangle((int)playerPosition.X, (int)playerPosition.Y, playerTextureWidth, playerTextureHeight)
                ScreenCenter
                , new Rectangle(playerTextureWidth * playerRunStep + (int)plOffset.X, playerTextureHeight * player.RunDirection + (int)plOffset.Y, playerTextureWidth, playerTextureHeight), player.Buffs.Where((Buff b) => b.Type == BuffType.Poison).Count() > 0 ? Color.LawnGreen : Color.White);  
        }

        private void DrawTiles(LightSource[] lightSources)
        {
            for (int x = gamePole.MinX; x < gamePole.MaxX; x++)
                for (int y = gamePole.MinY; y < gamePole.MaxY; y++)
                {
                    Vector2 pps = (new Vector2(x, y) * tileWidth - player.Position) / 2;
                    Tile t = gamePole.GetTileAt(new Point(x, y));
                    if (t != null && t.Id != 0 && pps.X < screenWidth && pps.Y < screenHeight)
                    {
                        Point center = new Point(x * tileWidth + tileWidth / 2, y * tileHeight + tileHeight / 2);
                        float lightness = GetLightLevel(center, lightSources);
                        if (t.IsObstacle)
                            lightness = (lightness * 0.75f);
                        spriteBatch.Draw(tileTexture, ScreenCenter - player.Position + new Vector2(x * tileWidth, y * tileHeight), GetTilePos(t.Id), Color.White * lightness);
                    }
                }
        }

        private Rectangle GetTilePos(int n)
        {
            int width = 8;
            int x = n % width;
            int y = n / width;
            return new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        private Rectangle GetStuffCoord(int stuffId)
        {
            int x = stuffId % stuffPerLine;
            int y = stuffId / stuffPerLine;
            return new Rectangle(x * stuffWidth, y * stuffHeight, stuffWidth, stuffHeight);
        }

        private void DrawBuff(Buff buff, Vector2 pos)
        {
            int borderId = 240;
            float scale = 32f / stuffWidth;
            Rectangle borderRect = GetStuffCoord(borderId);
            Vector2 buffImgPos = new Vector2(5, 4.5f);
            Rectangle buffImgRect = GetStuffCoord(GetBuffStuffId(buff.Type));
            //Rectangle buffImgDestRect = new Rectangle((int)(pos + buffImgPos).X, (int)(pos + buffImgPos).Y);

            spriteBatch.Draw(stuffTexture, new Rectangle((int)pos.X, (int)pos.Y, 32, 32), borderRect, Color.White);
            spriteBatch.Draw(stuffTexture, pos + buffImgPos / scale, buffImgRect, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, buff.GetStringTime(), pos + new Vector2(16f, 36f), Color.White, 0f, font.MeasureString(buff.GetStringTime()) / 2f, 1f, SpriteEffects.None, 0);
        }

        private int GetBuffStuffId(BuffType type)
        {
            switch (type)
            {
                case BuffType.Torch:
                    return 180;
                case BuffType.Poison:
                    return 66;
                default:
                    return 0;
            }
        }
    }
}
 