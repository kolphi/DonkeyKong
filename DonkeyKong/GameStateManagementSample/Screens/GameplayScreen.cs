#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using System.Diagnostics;
using GameStateManagementSample.Entities;
using Microsoft.Devices.Sensors;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using GameStateManagementSample.Screens;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        
        Int32 gameScore;
        
        Int16 screenWidth = 480;
        Int16 screenHeight = 800;
        float rotationValue;
        const Int16 fixedSpeed = 1;
        float dynamicSpeed;

        //for limiting creation of new objects
        Int16 bananaCounter;
        Int16 barrelCounter;
        Int16 puddleCounter;
        
        //for slowing animation of objects other than player
        Int16 animationCounter;

        //graphics for HUD
        Texture2D headUpDisplayTexture;
        Texture2D heart1LiveTexture;
        Texture2D heart2LiveTexture;
        Texture2D heartfullLiveTexture;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        //init vectors for bananas and obstacles
        List<BananaEntity> bananaVector = new List<BananaEntity>();
        List<BarrelEntity> barrelVector = new List<BarrelEntity>();
        List<WaterEntity> waterVector = new List<WaterEntity>();
        

        //time management
        private float time_between_frame;
        private float time_last_frame;

        //banana texture
        Texture2D bananaTexture;
        Texture2D barrelTexture;
        Texture2D puddleTexture;

        Random random;

        float pauseAlpha;

        InputAction pauseAction;

        ParallaxingBackground grassTexture;
        ParallaxingBackground cloudTexture;
        Texture2D mainBackground;

        PlayerEntity player;

        Motion MotionSensor;

        //rotation movement flags
        Boolean moveRight;
        Boolean moveLeft;

        //Vector2 cloudsPosition = Vector2.Zero;
        //float cloudSpeed = 0.5f;

        SpriteBatch spriteBatch;

        Song playMusic;
        PhoneMainMenuScreen phm = new PhoneMainMenuScreen();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            
            random = new Random((int)screenHeight/2);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //init game start score with 0
            gameScore = 0;

            //init motion sensor
            MotionSensor = new Motion();

            //set time boarder for player sprite animation update
            time_between_frame = 0.1f;
            time_last_frame = 0f;

           
            //set dynamicSpeed initial value
            dynamicSpeed = fixedSpeed;

            //init player
            player = new PlayerEntity();

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("menufont");

                //grassTexture = content.Load<Texture2D>("grass1");
                //cloudsTexture = content.Load<Texture2D>("clouds");

                //backgrounds
                cloudTexture = new ParallaxingBackground();
                grassTexture = new ParallaxingBackground();

                mainBackground = content.Load<Texture2D>("water");
                grassTexture.Initialize(content, "grass2", 800, -(dynamicSpeed));
                cloudTexture.Initialize(content, "clouds", 800, -(dynamicSpeed*3));

                //set HUD textures
                headUpDisplayTexture = content.Load<Texture2D>("HUD2");
                heart1LiveTexture = content.Load<Texture2D>("Heart_1");
                heart2LiveTexture = content.Load<Texture2D>("Heart_2");
                heartfullLiveTexture= content.Load<Texture2D>("Heart_3");

                //Setup sensors
                if (MotionSensor != null)
                {
                    MotionSensor.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(MotionSensor_CurrentValueChanged);
                    MotionSensor.Start();
                }

                //setup input touch gestures
                TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Flick;

                Texture2D playerTexture = content.Load<Texture2D>("donkey_walk_raster4");
               
                Texture2D playerTextureBurstMode = content.Load<Texture2D>("donkey_movement_raster");
                //set banana texture
                bananaTexture = content.Load<Texture2D>("bananas_single_raster2");
                barrelTexture = content.Load<Texture2D>("barrels2");
                puddleTexture = content.Load<Texture2D>("small_water");


                //set player position to the bottom
                Vector2 playerPosition = new Vector2(screenWidth / 2,
                screenHeight/2-200);

                player.Initialize(playerPosition);
                player.InitializeAnimation(playerTexture, playerTexture.Width/20, playerTexture.Height, 20, 1, Color.White, 1, true);

                
                if (phm.musicState == false)
                {                    
                    playMusic = content.Load<Song>("backMusic");
                    PlaySong(playMusic);
                }

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }

        private void PlaySong(Song playMusic)
        {
            try
            {
                // Play the music
                MediaPlayer.Play(playMusic);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        void MotionSensor_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            //movement detected - rotate the player
            rotationValue = e.SensorReading.Attitude.Roll;

       //     Debug.WriteLine("Rotation Value: " + rotationValue);
      //      Debug.WriteLine("Quaternion Value: " + e.SensorReading.Attitude.Quaternion);
       //     Debug.WriteLine("RollValue: " + e.SensorReading.Attitude.Roll);
           

            if(rotationValue >= -0.5 && rotationValue <= 1.9){
  //              player.Rotation= rotationValue+90;
            }

            if (rotationValue > 0.2)
            {
                moveLeft = false;
                moveRight = true;
            }
            else if (rotationValue < -0.2)
            {

                moveLeft = true;
                moveRight = false;
            }
            else
            {
                moveLeft = false;
                moveRight = false;
            }
           
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            // check whether gestures are available
            while (TouchPanel.IsGestureAvailable)
            {
                // read the next gesture
                var gesture = TouchPanel.ReadGesture();

                // has the user tapped the screen?
                if (gesture.GestureType == GestureType.DoubleTap)
                {
                    Debug.WriteLine("-----DoubleTap detected");
                }
                else if(gesture.GestureType == GestureType.Flick)
                {
                    Debug.WriteLine("-----Flick detected");
                }
               
            }


            //check if player health is > 0
            if (player.Health < 1)
            {
                showGameOverExit();
            }
 
           dynamicSpeed += gameTime.ElapsedGameTime.Milliseconds/ 100000.0f;

            //testing arguments
           Debug.WriteLine("SpeedVal :" + dynamicSpeed);

            time_last_frame += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (time_last_frame >= time_between_frame)
            {
                //   player.Update(gameTime);
                time_last_frame = 0f;
                player.Update(gameTime);
                Debug.WriteLine("Tick");

                //adding banana
                if (bananaCounter > 20)
                {
                   
                        BananaEntity b = new BananaEntity();
                        b.Initialize(new Vector2(random.Next(400) + 40, 800));
                        b.InitializeAnimation(bananaTexture, bananaTexture.Width / 8, bananaTexture.Height, 8, 1, Color.White, 1, true);
                        bananaVector.Add(b);
                        bananaCounter = 0;
                    
                }

                //adding barrel
                if (barrelCounter > 40)
                {
                    
                        BarrelEntity o = new BarrelEntity();
                        o.Initialize(new Vector2(random.Next(400) + 40, 800));
                        o.InitializeAnimation(barrelTexture, barrelTexture.Width / 8, barrelTexture.Height, 8, 1, Color.White, 1, true);
                        barrelVector.Add(o);
                        barrelCounter = 0;
                    
                }

                //adding puddle
                if (puddleCounter > 60)
                {
                    WaterEntity w = new WaterEntity();
                    w.Initialize(new Vector2(random.Next(350) + 30, 800));
                    w.InitializeAnimation(puddleTexture, puddleTexture.Width / 3, puddleTexture.Height, 3, 1, Color.White, 1, true);
                    waterVector.Add(w);
                    puddleCounter = 0;
                }

                 //slowing animation of objects - only update after 2 ticks
                if (animationCounter > 2)
                {
                    //update bananas
                    foreach (BananaEntity ba in bananaVector)
                    {
                        //trigger update

                        ba.Update(gameTime);
                    }
                    

                    //update barrels
                    foreach (BarrelEntity bar in barrelVector)
                    {
                        bar.Update(gameTime);
                    }
                    

                    //update puddles
                    foreach (WaterEntity wa in waterVector)
                    {
                        wa.Update(gameTime);
                    }

                    //resetting animation counter
                    animationCounter = 0;

                }

                animationCounter++;

                bananaCounter++;
                barrelCounter++;
                puddleCounter++;
                
            }

            checkCollisions();

            foreach (BananaEntity b in bananaVector)
            {

                if (b.Position.Y < 0)
                {
                    b.Deactivate();
                    //  bananaVector.Remove(b);

                }
                else
                {
                    b.Update(gameTime);
                    b.Position.Y -= dynamicSpeed;
                }
            }


            //barrel positions
            foreach (BarrelEntity ba in barrelVector)
            {

                if (ba.Position.Y < 0)
                {
                    ba.Deactivate();
                }
                else
                {
                    ba.Update(gameTime);
                    ba.Position.Y -= dynamicSpeed;
                }
            }

            //puddle positions
            foreach (WaterEntity w in waterVector)
            {

                if (w.Position.Y < 0)
                {
                    w.Deactivate();
                }
                else
                {
                    w.Update(gameTime);
                    w.Position.Y -= dynamicSpeed;
                }
            }
           
           
            //player movement
            if (moveRight)
            {
                player.Position.X += 2f;
            }

            if (moveLeft)
            {
                player.Position.X -= 2f;
            }

            //destroy player entity if it falls of the ground
            if (player.Position.X > ScreenManager.Game.GraphicsDevice.Viewport.Width-20 || player.Position.X < 20)
            {
                //deactivate player entity
                player.Active = false;

                //play Animation

                showGameOverExit();
                
            }

           // player.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, false);




            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                grassTexture.Update(-dynamicSpeed);
                cloudTexture.Update(-3*dynamicSpeed);

                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2,
                    200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);

                
            }
        }

        private void showGameOverExit()
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BackgroundOverScreen(), new GameOverScreen());
        }

        private void checkCollisions()
        {
            Rectangle PlayerBoundingBox = player.BoundingBox;

            //check for collisions with bananas
            foreach (BananaEntity b in bananaVector)
            {
                if (b.isActive()){
               
                if (PlayerBoundingBox.Intersects(b.BoundingBox))
                {
                    //add points to scoreboard
                    gameScore += b.ScorePoints;
                    // deactivate to destroy it later
                    b.Deactivate();
                    //b.Active = false;
                   
                    
                    Debug.WriteLine("Collision: player and banana at " + b.Position.Y + ";" +b.Position.X);
                }
                }

            }

            foreach (BarrelEntity bar in barrelVector)
            {
                if (bar.isActive())
                {

                    if (PlayerBoundingBox.Intersects(bar.BoundingBox))
                    {
                        //decrease player health
                        player.Health -= 1;

                        //deactivate obstacle
                        bar.Deactivate();

                    }
                }
            }


            foreach (WaterEntity water in waterVector)
            {
                if (PlayerBoundingBox.Intersects(water.BoundingBox))
                {
                    showGameOverExit();
                    //DrawHud(spriteBatch, ScreenManager.GraphicsDevice.Viewport);
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - playerPosition;
                    direction.Normalize();
                    movement += direction;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 8f;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            // Our player and enemy are both actually just text strings.
            spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            //backgrounds
            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
            grassTexture.Draw(spriteBatch);

            //draw player
            player.Draw(spriteBatch);
           
            //draw bananas
            foreach (BananaEntity b in bananaVector)
            {

                b.Draw(spriteBatch);
                
            }

            //draw barrels 
            foreach (BarrelEntity o in barrelVector)
            {
                o.Draw(spriteBatch);
            }

            foreach (WaterEntity w in waterVector)
            {
                w.Draw(spriteBatch);
            }


            //draw clouds
            cloudTexture.Draw(spriteBatch);

            spriteBatch.Draw(headUpDisplayTexture, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(270, 20), Color.Yellow);

            //draw HUD
            DrawHud(spriteBatch, viewport);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawHud(SpriteBatch spriteBatch, Viewport viewport)
        {
           // Rectangle backgroundHud = new Rectangle(0, 0, screenWidth, screenHeight);
          //  Texture2D text = new Texture2D(backgroundHud, backgroundHud.Width, backgroundHud.Height);
            spriteBatch.Draw(headUpDisplayTexture, new Vector2(0, 0), Color.White);
            //spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(viewport.Width/2, 30), Color.Black);
            spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(270, 20), Color.Yellow);
         
            //draw hearts
            if (player.Health == 3)
            {
                spriteBatch.Draw(heartfullLiveTexture, new Vector2(0, 15), Color.White);
            }
            else if (player.Health == 2)
            {
                spriteBatch.Draw(heart2LiveTexture, new Vector2(0, 15), Color.White);

            }
            else if (player.Health == 1)
            {
                spriteBatch.Draw(heart1LiveTexture, new Vector2(0, 15), Color.White);

            }
            
        }


        #endregion
    }
}
