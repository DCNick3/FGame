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
        Texture2D slotTexture;
        Texture2D swordTexture;
        Texture2D whitePixel;
        SpriteFont font14;
        SpriteFont font11;
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
        int swordWidth = 32;
        int swordHeight = 32;
        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;
        MouseState mouseState;
        MouseState lastMouseState;
        TimeSpan runFreq = TimeSpan.FromSeconds(0.3);
        TimeSpan lastRunUpdate = new TimeSpan(0);
        int playerRunStep = 1;
        bool runStepDirection = false;
        bool enableSmoothLightning = true;
        bool cheatFullBright = false;
        bool debugInfo = false;
        bool enableDevCheats = true;
        bool firstRun = true;
        bool isMoving = false;
        float targetFPS = 60.0F;
        double r = 0;
        ItemStack holdingItem = null;
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
         * Add smooth light (done)
         * Add bauble hint
         * Add chunked-world (done)
         * Normal onKeyPress (done)
         * Tree world generating algorithm (done)
         * Add abstraction to GamePole and Chunk!
         * Add auto-loading for chunks 
         * Add item moving ability ib inventory (done)
         * Add abstraction: Tiles & Chests are StaticObject's 
         * Add light flikering
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
            player.Type = 0;
            player.AddItem(new ItemStack(Items.torch, 12));
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

            TargetElapsedTime = TimeSpan.FromSeconds(1F / targetFPS);

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
            slotTexture = Content.Load<Texture2D>("slot");
            swordTexture = Content.Load<Texture2D>("sword");
            font14 = Content.Load<SpriteFont>("font14");
            font11 = Content.Load<SpriteFont>("font11");
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
            isMoving = false;
            UpdateFireballs(gameTime);
            lightSources = GetLightSources();


            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();
            if (firstRun)
            {
                firstRun = false;
                return;
            }

            if (!player.IsInInventory)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    PlayerMove(3, gameTime);
                    isMoving = true;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    PlayerMove(2, gameTime);
                    isMoving = true;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    PlayerMove(1, gameTime);
                    isMoving = true;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    PlayerMove(0, gameTime);
                    isMoving = true;
                }

                if (KeyPress(Keys.Q))
                {
                    if (player.Type == 1 && player.UseSkill(0, gameTime))
                    {
                        fireballs.Add(new Fireball() { X = (int)player.Position.X, Y = (int)player.Position.Y, Direction = player.RunDirection, Speed = 3, Step = 0 });
                    }
                    if (player.CanUseSwords && player.UseSkill(0, gameTime))
                    {
                        player.CastSword(48, 0.3F, player.RunDirection);
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.OemTilde))
            {
                RegenPole();
            }

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
                if (player.Type == 1 && player.UseSkill(1, gameTime))
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
                player.isSpeedHack = !player.isSpeedHack;
            }

            if (enableDevCheats && KeyPress(Keys.B))
            {
                MouseState s = Mouse.GetState();
                player.Position += new Vector2(s.X, s.Y) - ScreenCenter;
            }

            if (enableDevCheats && KeyPress(Keys.N))
            {
                player.CastSword(10000000, 1F, player.RunDirection);
            }

            if (enableDevCheats && KeyPress(Keys.M))
            {
                player.UnCastSword();
            }

            if (KeyPress(Keys.Escape))
            {
                player.IsInInventory = !player.IsInInventory;
            }

            if (KeyPress(Keys.F4))
            {
                graphics.ToggleFullScreen();
            }

            if (!player.IsInInventory)
            {
                if (holdingItem != null)
                {
                    player.AddItem(holdingItem);
                    holdingItem = null;
                }
                if (KeyPress(Keys.D1))
                    player.SelectedSlot = 0;
                if (KeyPress(Keys.D2))
                    player.SelectedSlot = 1;
                if (KeyPress(Keys.D3))
                    player.SelectedSlot = 2;
                if (KeyPress(Keys.D4))
                    player.SelectedSlot = 3;
                if (KeyPress(Keys.D5))
                    player.SelectedSlot = 4;
                if (KeyPress(Keys.D6))
                    player.SelectedSlot = 5;
                if (KeyPress(Keys.D7))
                    player.SelectedSlot = 6;
                if (KeyPress(Keys.D8))
                    player.SelectedSlot = 7;
                if (KeyPress(Keys.D9))
                    player.SelectedSlot = 8;

                if (KeyPress(Keys.D0))
                    player.SelectedSlot = 9;

                if (KeyPress(Keys.A))
                {
                    player.SelectedSlot--;
                    if (player.SelectedSlot < 0)
                        player.SelectedSlot += 10;
                }

                if (KeyPress(Keys.D))
                {
                    player.SelectedSlot++;
                    player.SelectedSlot %= 10;
                }
            }
            else
            {
                if (KeyPress(Keys.Up))
                {
                    player.SelectedSlot -= 10;
                    if (player.SelectedSlot < 0)
                        player.SelectedSlot += 40;
                    player.SelectedSlot %= 40;
                }
                if (KeyPress(Keys.Down))
                {
                    player.SelectedSlot += 10;
                    if (player.SelectedSlot < 0)
                        player.SelectedSlot += 40;
                    player.SelectedSlot %= 40;
                }
                if (KeyPress(Keys.Left))
                {
                    player.SelectedSlot -= 1;
                    if (player.SelectedSlot < 0)
                        player.SelectedSlot += 40;
                    player.SelectedSlot %= 40;
                }
                if (KeyPress(Keys.Right))
                {
                    player.SelectedSlot += 1;
                    if (player.SelectedSlot < 0)
                        player.SelectedSlot += 40;
                    player.SelectedSlot %= 40;
                }
                if (KeyPress(Keys.W))
                {
                    ItemStack s = player.Inventory[player.SelectedSlot];
                    if (holdingItem == null || s == null || s.Type != holdingItem.Type)
                    {
                        player.Inventory[player.SelectedSlot] = holdingItem;
                        holdingItem = s;
                    }
                    else
                    {
                        int add = holdingItem.Count;
                        if (add + player.Inventory[player.SelectedSlot].Count > holdingItem.Type.GetMaxStackSize())
                            add = holdingItem.Type.GetMaxStackSize() - player.Inventory[player.SelectedSlot].Count;
                        player.Inventory[player.SelectedSlot].Count += add;
                        holdingItem.Count -= add;
                        if (holdingItem.Count <= 0)
                            holdingItem = null;
                    }
                }
                if (KeyPress(Keys.Q))
                {
                    ItemStack s = player.Inventory[player.SelectedSlot];
                    if (s != null)
                    {
                        if (holdingItem == null)
                        {
                            holdingItem = new ItemStack(s.Type, 1);
                            s.Count--;
                        }
                        else if (holdingItem.Type == s.Type && holdingItem.Count + 1 <= holdingItem.Type.GetMaxStackSize())
                        {
                            holdingItem.Count++;
                            s.Count--;
                        }
                        if (s.Count <= 0)
                            player.Inventory[player.SelectedSlot] = null;
                    }
                }
            }

            if (KeyPress(Keys.S))
            {
                ItemStack cs = player.Inventory[player.SelectedSlot];
                if (cs != null)
                {
                    cs.Type.OnUse(player, cs);
                    player.ValidateInventory();
                }
            }

            if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {//LClickStart

            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {//LClickLong

            }

            if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
            {//LClickEnd

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

        private void PlayerMove(int direction, GameTime gameTime)
        {
            player.RunDirection = direction;
            if (!player.CanMove) return;
            Vector2 newPos = player.Position;
            switch (direction)
            {
                case 0:
                    newPos.Y += player.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    break;
                case 1:
                    newPos.X -= player.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    break;
                case 2:
                    newPos.X += player.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    break;
                case 3:
                    newPos.Y -= player.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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
            DrawSword(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) / 2f, player.SwordDirection, player.GetSwordLength(gameTime), player.SwordColor);

            //DrawSword(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) / 2, r, 48, Color.Blue);
            //DrawSword(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) / 2, r + Math.PI, 48, Color.Blue);
            //DrawSword(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) / 2, r + Math.PI * 1.5, 48, Color.Green);
            //DrawSword(ScreenCenter + new Vector2(playerTextureWidth, playerTextureHeight) / 2, r + Math.PI * 0.5, 48, Color.Green);
            //r += Math.PI / 20;
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
            DrawInventory(player);

            if (debugInfo)
            {

                int x = (int)Math.Floor(player.Position.X / tileWidth / Chunk.width);
                int y = (int)Math.Floor(player.Position.Y / tileHeight / Chunk.height);

                Vector2 chnkIdIrr = new Vector2(x, y);
                string dbgnfo = " Pos: " + (player.Position).ToString()
                    + "\n IsRunningSlowly: " + gameTime.IsRunningSlowly
                    + "\n Chunk: " + new Vector2((int)chnkIdIrr.X, (int)chnkIdIrr.Y)
                    + "\n FPS: " + (int)(1.0F / gameTime.ElapsedGameTime.TotalSeconds);
                string[] lines = dbgnfo.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string ln = lines[i];
                    Vector2 sz = font14.MeasureString(ln);
                    spriteBatch.DrawString(font14, ln, new Vector2(screenWidth - sz.X, i * (sz.Y + 1)), Color.White);
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
            spriteBatch.DrawString(font14, hP.ToString("###"), new Vector2(pos.X + pos.Width / 2f, pos.Y + pos.Height / 2f), Color.Black, 0, font14.MeasureString(hP.ToString("###")) / 2f, 1f, SpriteEffects.None, 0);
        }

        private void DrawSkillUsePointBar(float skillUsePoints, float maxSkillUsePoints, Color color)
        {
            Rectangle pos = new Rectangle(120, 10, 100, 20);
            Rectangle rectCont = new Rectangle(pos.X - 1, pos.Y - 1, pos.Width + 2, pos.Height + 2);
            Rectangle rectRed = new Rectangle(pos.X, pos.Y, (int)(pos.Width * (skillUsePoints / maxSkillUsePoints)), pos.Height);
            spriteBatch.Draw(whitePixel, rectCont, Color.White);
            spriteBatch.Draw(whitePixel, rectRed, color);
            spriteBatch.DrawString(font14, skillUsePoints.ToString("###"), new Vector2(pos.X + pos.Width / 2f, pos.Y + pos.Height / 2f), Color.Black, 0, font14.MeasureString(skillUsePoints.ToString("###")) / 2f, 1f, SpriteEffects.None, 0);
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
            foreach (var chunk in gamePole.chunks)
            {
                if (chunk != null)
                    foreach (var chest in chunk.Chests)
                    {
                        Vector2 pps = (new Vector2(chest.GlobPosition.X * chestWidth, chest.GlobPosition.Y * chestHeight) - player.Position) / 2;
                        if (pps.X < screenWidth && pps.Y < screenHeight)
                        {
                            Vector2 pos = ScreenCenter - player.Position + new Vector2(chest.GlobPosition.X * chestWidth, chest.GlobPosition.Y * chestHeight - 16);
                            Point center = new Point(chest.GlobPosition.X * chestWidth + chestWidth / 2, chest.GlobPosition.Y * chestHeight + chestHeight / 2 - 16);
                            float lightness = GetLightLevel(center, lightSources);
                            spriteBatch.Draw(chestTexture, pos, new Rectangle(chest.Type * chestWidth, (chest.AnimationFrame) * (chestHeight + 16), chestWidth, chestHeight + 16), Color.White * lightness);
                        }
                    }
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
            //Optimize~
            int mX = (int)Math.Floor((player.Position.X - screenWidth) / tileWidth);
            int mY = (int)Math.Floor((player.Position.Y- screenHeight) / tileHeight);
            if (mX < gamePole.MinX)
                mX = gamePole.MinX;
            if (mY < gamePole.MinY)
                mY = gamePole.MinY;
            int mxX = (int)Math.Floor((player.Position.X + screenWidth) / tileWidth);
            int mxY = (int)Math.Floor((player.Position.Y + screenHeight) / tileHeight);
            for (int x = mX; x < mxX; x++)
                for (int y = mY; y < mxY; y++)
                {
                    Vector2 pps = (new Vector2(x, y) * tileWidth - player.Position) / 2;
                    pps = new Vector2(Math.Abs(pps.X), Math.Abs(pps.Y));
                    Tile t = gamePole.GetTileAt(new Point(x, y));
                    if (t != null && t.Id != 0 /*&& pps.X < screenWidth && pps.Y < screenHeight*/)
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

        private Texture2D GetStuffTexture(int id)
        {
            return stuffTexture;
        }

        private void DrawBuff(Buff buff, Vector2 pos)
        {
            int borderId = 240;
            float scale = 32f / stuffWidth;
            Rectangle borderRect = GetStuffCoord(borderId);
            Vector2 buffImgPos = new Vector2(5, 4.5f);
            int buffId = GetBuffStuffId(buff.Type);
            Rectangle buffImgRect = GetStuffCoord(buffId);
            Texture2D brdTxt = GetStuffTexture(borderId);
            Texture2D bffTxt = GetStuffTexture(buffId);
            //Rectangle buffImgDestRect = new Rectangle((int)(pos + buffImgPos).X, (int)(pos + buffImgPos).Y);

            spriteBatch.Draw(brdTxt, new Rectangle((int)pos.X, (int)pos.Y, 32, 32), borderRect, Color.White);
            spriteBatch.Draw(bffTxt, pos + buffImgPos / scale, buffImgRect, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font14, buff.GetStringTime(), pos + new Vector2(16f, 36f), Color.White, 0f, font14.MeasureString(buff.GetStringTime()) / 2f, 1f, SpriteEffects.None, 0);
        }

        private void DrawItemStack(ItemStack stack, Vector2 pos)
        {
            int itmTxtId = stack.Type.GetTextureId(stack);
            Texture2D stff = GetStuffTexture(itmTxtId);
            Rectangle itmSrcPos = GetStuffCoord(itmTxtId);
            string cnt = stack.Count.ToString();

            spriteBatch.Draw(stff, pos, itmSrcPos, Color.White);
            if (stack.Count != 1)
                spriteBatch.DrawString(font11, cnt,  pos + new Vector2(stuffWidth / 2f, 1.3f * stuffHeight), Color.White, 0, font11.MeasureString(cnt) / 2, 1f, SpriteEffects.None, 0);
        }

        private void DrawInventory(Player player)
        {
            int irmpl = 10;
            ItemStack[] itm = player.Inventory;
            float xsz = irmpl * 50f + 48f;
            Vector2 offset = new Vector2(screenWidth - xsz, 10);

            for (int y = 0; y < (player.IsInInventory ? 4 : 1); y++)
            {
                for (int x = 0; x < irmpl; x++)
                {
                    ItemStack stk = itm[x + y * irmpl];
                    Vector2 pos = offset + new Vector2(x * 50, y * 50 + (y == 0 ? 0 : 15f));
                    Color c;
                    switch (x)
                    {
                        case 0:
                            c = Color.DarkBlue;
                            break;
                        case 1:
                            c = Color.DarkRed;
                            break;
                        case 2:
                            c = Color.DarkOrange;
                            break;
                        case 3:
                            c = Color.DarkGreen;
                            break;
                        case 4:
                            c = Color.DarkMagenta;
                            break;
                        case 5:
                            c = Color.DarkGoldenrod;
                            break;
                        case 6:
                            c = Color.DarkCyan;
                            break;
                        case 7:
                            c = Color.DarkViolet;
                            break;
                        case 8:
                            c = Color.DarkTurquoise;
                            break;
                        case 9:
                            c = Color.DarkKhaki;
                            break;
                        default:
                            c = Color.Black;
                            break;
                    }

                    if (!player.IsInInventory && y == 0 && player.SelectedSlot == x)
                        spriteBatch.Draw(slotTexture, new Rectangle((int)pos.X - 12, (int)pos.Y - 10, 52, 52), c * 1.5f);
                    else
                        if (player.IsInInventory && y == player.SelectedSlot / 10 && x == player.SelectedSlot % 10)
                        spriteBatch.Draw(slotTexture, new Rectangle((int)pos.X - 12, (int)pos.Y - 10, 52, 52), c * 1.5f);
                    else
                        spriteBatch.Draw(slotTexture, new Rectangle((int)pos.X - 10, (int)pos.Y - 8, 48, 48), c);
                    if (stk != null)
                        DrawItemStack(stk, pos - new Vector2(0, 3));
                    if (y == 0)
                    {
                        string n = ((x + 1) % 10).ToString();
                        if (!player.IsInInventory && y == 0 && player.SelectedSlot == x)
                            spriteBatch.DrawString(font14, n, pos + new Vector2(48f / 2f - 10, 50f), Color.White, 0, font14.MeasureString(n) / 2f, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.DrawString(font11, n, pos + new Vector2(48f / 2f - 10, 50f), Color.White, 0, font11.MeasureString(n) / 2f, 1f, SpriteEffects.None, 0);
                    }
                }
            }

            if (player.IsInInventory && holdingItem != null)
            {
                DrawItemStack(holdingItem, offset + new Vector2(xsz - 48f, 0f));
            }
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

        private void DrawSword(Vector2 position, int direction, int length, Color color)
        {
            //DLRU
            if (length == 0) return;
            int c1 = (int)Math.Ceiling((float)length / swordHeight) - 1;
            int rem = length % swordHeight;
            if (rem == 0) rem = swordHeight;
            int xm = swordWidth;
            int ym = swordHeight;
            float rot = 0f;
            Vector2 origin = new Vector2(swordWidth / 2F, swordHeight);
            switch (direction)
            {
                case 0:
                    ym *= 1;
                    xm *= 0;
                    rot = (float)Math.PI;
                    break;
                case 1:
                    ym *= 0;
                    xm *= -1;
                    rot = 1.5f * (float)Math.PI;
                    break;
                case 2:
                    ym *= 0;
                    xm *= 1;
                    rot = 0.5f * (float)Math.PI;
                    break;
                case 3:
                    ym *= -1;
                    xm *= 0;
                    rot = 0f;
                    break;
            }
            for (int i = 0; i < c1; i++)
            {
                spriteBatch.Draw(swordTexture, new Rectangle((int)(position.X + xm * i - xm / swordWidth * (swordHeight - rem)), (int)(position.Y + ym * i - ym / swordHeight * (swordHeight - rem)), swordWidth, (i == 0 ? rem : swordHeight)), new Rectangle(swordWidth, 0, swordWidth, (i == 0 ? rem : swordHeight)), color, rot, origin, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(swordTexture, new Rectangle((int)(position.X + xm * c1 - xm / swordWidth * (swordHeight - rem)), (int)(position.Y + ym * c1 - ym / swordHeight * (swordHeight - rem)), swordWidth, (c1 == 0 ? rem : swordHeight)), new Rectangle(0, 0, swordWidth, (c1 == 0 ? rem : swordHeight)), color, rot, origin, SpriteEffects.None, 0);
        }

        private void DrawSword(Vector2 position, double direction, int length, Color color)
        {
            //DLRU
            if (length == 0) return;
            int c1 = (int)Math.Ceiling((float)length / swordHeight) - 1;
            int rem = length % swordHeight;
            if (rem == 0) rem = swordHeight;
            Vector2 m = Vector2.Zero;
            m.X = (float)(Math.Sin(direction) * swordWidth);
            m.Y = -(float)(Math.Cos(direction) * swordHeight);
            float rot = (float)direction;
            Vector2 origin = new Vector2(swordWidth / 2F, swordHeight);
            /*switch (direction)
            {
                case 0:
                    ym *= 1;
                    xm *= 0;
                    rot = (float)Math.PI;
                    break;
                case 1:
                    ym *= 0;
                    xm *= -1;
                    rot = 1.5f * (float)Math.PI;
                    break;
                case 2:
                    ym *= 0;
                    xm *= 1;
                    rot = 0.5f * (float)Math.PI;
                    break;
                case 3:
                    ym *= -1;
                    xm *= 0;
                    rot = 0f;
                    break;
            }*/
            for (int i = 0; i < c1; i++)
            {
                spriteBatch.Draw(swordTexture, new Rectangle((int)(position.X + m.X * i - m.X / swordWidth * (swordHeight - rem)), (int)(position.Y + m.Y * i - m.Y / swordHeight * (swordHeight - rem)), swordWidth, (i == 0 ? rem : swordHeight)), new Rectangle(swordWidth, 0, swordWidth, (i == 0 ? rem : swordHeight)), color, rot, origin, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(swordTexture, new Rectangle((int)(position.X + m.X * c1 - m.X / swordWidth * (swordHeight - rem)), (int)(position.Y + m.Y * c1 - m.Y / swordHeight * (swordHeight - rem)), swordWidth, (c1 == 0 ? rem : swordHeight)), new Rectangle(0, 0, swordWidth, (c1 == 0 ? rem : swordHeight)), color, rot, origin, SpriteEffects.None, 0);
        }
    }
}
 