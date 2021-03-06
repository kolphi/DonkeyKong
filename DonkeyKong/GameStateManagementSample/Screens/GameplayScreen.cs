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
using System.Windows.Threading;
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

       private ContentManager content;
      private  SpriteFont gameFont;
        
       private Int32 gameScore;

       private Int16 screenWidth = 480;
       private Int16 screenHeight = 800;
       private float rotationValue;
       private const Int16 fixedSpeed = 1;
       private float dynamicSpeed;

        //enum for current animation
        enum MoveState { WALK = 1, RIGHT, LEFT, JUMP};
        MoveState moveState;

        //for limiting creation of new objects
        private Int16 bananaCounter;
        private Int16 barrelCounter;
        private Int16 puddleCounter;
        private Int16 heartCounter;
        
        //for slowing animation of objects other than player
        private Int16 animationCounter;

        //graphics for HUD
        private Texture2D headUpDisplayTexture;
        private Texture2D heart1LiveTexture;
        private Texture2D heart2LiveTexture;
        private Texture2D heartfullLiveTexture;

        private Texture2D redDotTexture;
        private Texture2D greenDotTexture;


        private Vector2 playerPosition = new Vector2(100, 100);
        private Vector2 enemyPosition = new Vector2(100, 100);

        //init vectors for bananas and obstacles and rewards
        private List<BananaEntity> bananaVector = new List<BananaEntity>();
        private List<BarrelEntity> barrelVector = new List<BarrelEntity>();
        private List<ObstacleEntity> puddleVector = new List<ObstacleEntity>();
        private List<RewardEntity> heartVector = new List<RewardEntity>();
        

        //time management
        private float time_between_frame;
        private float time_last_frame;

        //banana texture
        private Texture2D bananaTexture;
        private Texture2D barrelTexture;
        private Texture2D puddleTexture;
        private Texture2D heartTexture;

        private Random random;

        float pauseAlpha;

        private InputAction pauseAction;

        //textures
        private ParallaxingBackground grassTexture;
        private ParallaxingBackground cloudTexture;
        private Texture2D mainBackground;

        //player variables
        protected PlayerEntity player;

        private Texture2D playerTexture;
        private Texture2D playerTextureWalkLeft;
        private Texture2D playerTextureWalkRight;
        private Texture2D playerTextureBurstMode;


        protected Motion MotionSensor;

        //rotation movement flags
        private Boolean moveRight;
        private Boolean moveLeft;
        private Boolean moveNormal;

        //RushMode Variables
        private Boolean rushMode;
        private Boolean cooldownActive;
        private Int16 cooldownCounter;
        private Int16 rushModeCounter;

        //Vector2 cloudsPosition = Vector2.Zero;
        //float cloudSpeed = 0.5f;

        private SpriteBatch spriteBatch;

        //sound variables
        private Song playMusic;
        private Boolean MusicState;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool musicState)
        {
            //setting cooldown and rushmode counter
            rushModeCounter = 11;
            cooldownCounter = 30;

            //setup input touch gestures
            TouchPanel.DisplayHeight = 800;
            TouchPanel.DisplayWidth = 480;
            TouchPanel.DisplayOrientation = DisplayOrientation.Portrait;
            TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Tap;

            var touchTimer = new DispatcherTimer();
            touchTimer.Interval = TimeSpan.FromMilliseconds(50);
            touchTimer.Tick += new EventHandler(Read_Gestures);
            touchTimer.Start();


            //setup random mechanism - seed is screenheiht/2
            random = new Random((int)screenHeight/2);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            MusicState = musicState;

            //init rushMode variables
            rushMode = false;
            cooldownActive = false;

            //init game start score with 0
            gameScore = 0;

            //init move state with WALK
            moveState = MoveState.WALK;

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
                redDotTexture = content.Load<Texture2D>("redDot");
                greenDotTexture = content.Load<Texture2D>("greenDot");
                heartfullLiveTexture= content.Load<Texture2D>("Heart_3");

                //Setup sensors
                if (MotionSensor != null)
                {
                    MotionSensor.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(MotionSensor_CurrentValueChanged);
                    MotionSensor.Start();
                }

               
                //set playerTextures
                playerTexture = content.Load<Texture2D>("donkey_walk_raster4");
                playerTextureWalkLeft = content.Load<Texture2D>("donkey_walk_left");
                playerTextureWalkRight = content.Load<Texture2D>("donkey_walk_right");
                playerTextureBurstMode = content.Load<Texture2D>("donkey_movement_raster");
                
                
                //set obstacle&reward texture
                bananaTexture = content.Load<Texture2D>("bananas_single_raster2");
                barrelTexture = content.Load<Texture2D>("barrels2");
                puddleTexture = content.Load<Texture2D>("puddle2");
                heartTexture = content.Load<Texture2D>("heart");

                //set player position to the bottom
                Vector2 playerPosition = new Vector2(screenWidth / 2,
                screenHeight/2-250);

                //start animation is MoveState.WALK
                player.Initialize(playerPosition);
                player.InitializeAnimation(playerTexture, playerTexture.Width/20, playerTexture.Height, 20, 1, Color.White, 1, true);

                
                //enable music if status is set to ON
                if (MusicState == true)
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

        /// <summary>
        /// Open media player for background music
        /// </summary>
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

        /// <summary>
        /// Method to get sensor changes and values
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">sensor event</param>
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
                moveNormal = true;
            }
           
        }
        /// <summary>
        /// Reads the touch gestures and performs actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Read_Gestures(object sender, EventArgs e)
        {
            TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Tap;

            
            // check whether gestures are available
            while (TouchPanel.IsGestureAvailable)
            {
                // read the next gesture
                GestureSample gesture = TouchPanel.ReadGesture();

                switch (gesture.GestureType)
                {
                    case GestureType.DoubleTap:
                        Debug.WriteLine("-----DoubleTap detected");

                        if (!cooldownActive && !rushMode)
                        {
                            Debug.WriteLine("-----rushModeActivated");
                            rushMode = true;
                            rushModeCounter = 11;
                            player.InitializeAnimation(playerTextureBurstMode, playerTextureBurstMode.Width / 11, playerTextureBurstMode.Height, 11, 1, Color.White, 1, false);
                            moveState = MoveState.JUMP;
                        }

                        break;
                        
                    case GestureType.Tap:
                        Debug.WriteLine("----Tap detected");

                        break;
                }

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
           

            //check if player health is > 0
            if (player.Health < 1)
            {
                showGameOverExit();
            }

           
                dynamicSpeed += gameTime.ElapsedGameTime.Milliseconds / 100000.0f;
           

            //testing arguments
           Debug.WriteLine("SpeedVal :" + dynamicSpeed);

            time_last_frame += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (time_last_frame >= time_between_frame)
            {
                //   player.Update(gameTime);
                time_last_frame = 0f;
                player.Update(gameTime);

                Debug.WriteLine("Tick");


                //check if rush mode is active
                if (rushMode)
                {
                    rushModeCounter--;
                    //check if rush mode has finished
                    if (rushModeCounter < 1)
                    {
                        //set rush mode off, enable cooldown
                        rushMode = false;
                        cooldownActive = true;
                        Debug.WriteLine("---- RushModeDeactivated  -> cooldownActivated");
                        //set move state
                        moveState = MoveState.WALK;
                        //change player animation back to walk
                        player.InitializeAnimation(playerTexture, playerTexture.Width / 20, playerTexture.Height, 20, 1, Color.White, 1, true);
                            
                    }

                }

                //check if cooldown for rushmode is active
                if (cooldownActive)
                {
                    cooldownCounter--;

                    if (cooldownCounter < 1)
                    {
                        Debug.WriteLine("----Cooldown deactivated");
                        //reset variables
                        cooldownActive = false;
                       
                        cooldownCounter = 30;
                    }

                 
                }


                //adding banana
                if (bananaCounter > 20)
                {
                   
                        BananaEntity b = new BananaEntity();
                        b.Initialize(new Vector2(random.Next(360) + 60, 800));
                        b.InitializeAnimation(bananaTexture, bananaTexture.Width / 8, bananaTexture.Height, 8, 1, Color.White, 1, true);
                        bananaVector.Add(b);
                        bananaCounter = 0;
                    
                }

                //adding heart
                if (heartCounter > 150)
                {

                    RewardEntity heart = new RewardEntity();
                    heart.Initialize(new Vector2(random.Next(360) + 60, 800));
                    heart.InitializeTexture(heartTexture);
                    heartVector.Add(heart);
                    heartCounter = 0;

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
                    ObstacleEntity w = new ObstacleEntity();
                    w.Initialize(new Vector2(random.Next(350) + 30, 800));
                    w.InitializeTexture(puddleTexture);
                    puddleVector.Add(w);
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
                    
                    //resetting animation counter
                    animationCounter = 0;

                }

                animationCounter++;
                bananaCounter++;
                barrelCounter++;
                puddleCounter++;
                heartCounter++;
                
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
                    if (rushMode)
                    {
                        b.Position.Y -= (dynamicSpeed + 2);
                    }
                    else
                    {
                        b.Position.Y -= dynamicSpeed;
                    }
                }
            }

            //barrel positions
            foreach (BarrelEntity barrel in barrelVector)
            {

                if (barrel.Position.Y < 0)
                {
                    barrel.Deactivate();
                }
                else
                {
                    barrel.Update(gameTime);
                    if (rushMode)
                    {
                        barrel.Position.Y -= (dynamicSpeed + 2);
                    }
                    else
                    {
                        barrel.Position.Y -= dynamicSpeed;
                    }
                }
            }

            //puddle positions
            foreach (RewardEntity heart in heartVector)
            {

                if (heart.Position.Y < 0)
                {
                    heart.Deactivate();
                }
                else
                {
                 //   heart.Update(gameTime);
                    if (rushMode)
                    {
                        heart.Position.Y -= (dynamicSpeed + 2);
                    }
                    else
                    {
                        heart.Position.Y -= dynamicSpeed;
                    }
                }
            }


            //puddle positions
            foreach (ObstacleEntity w in puddleVector)
            {

                if (w.Position.Y < 0)
                {
                    w.Deactivate();
                }
                else
                {
                   // w.Update(gameTime);
                    if (rushMode)
                    {
                        w.Position.Y -= (dynamicSpeed+2);
                    }
                    else
                    {
                        w.Position.Y -= dynamicSpeed;
                    }
                }
            }

            //check if rush mode is active, otherways no move will be performed
            if (moveState != MoveState.JUMP)
            {
                //player movement
                if (moveRight)
                {
                    if (moveState != MoveState.RIGHT)
                    {
                        //change animation to walking
                        player.InitializeAnimation(playerTextureWalkRight, playerTextureWalkRight.Width / 18, playerTextureWalkRight.Height, 18, 1, Color.White, 1, true);

                        moveState = MoveState.RIGHT;
                    }
                    player.Position.X += 2f;
                }

                else if (moveLeft)
                {
                    if (moveState != MoveState.LEFT)
                    {
                        //change animation to walking
                        player.InitializeAnimation(playerTextureWalkLeft, playerTextureWalkLeft.Width / 18, playerTextureWalkLeft.Height, 18, 1, Color.White, 1, true);

                        moveState = MoveState.LEFT;
                    }
                    player.Position.X -= 2f;
                }

                else if (moveNormal)
                {
                    if (moveState != MoveState.WALK)
                    {
                        player.InitializeAnimation(playerTexture, playerTexture.Width / 20, playerTexture.Height, 20, 1, Color.White, 1, true);
                        moveState = MoveState.WALK;
                    }
                }

            }

            //destroy player entity if it falls of the ground
            if (player.Position.X > ScreenManager.Game.GraphicsDevice.Viewport.Width-30 || player.Position.X < 30)
            {
                //deactivate player entity
                player.Active = false;
                //GameOver
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
                if (rushMode)
                {
                    grassTexture.Update(- (dynamicSpeed +2));
                    cloudTexture.Update(-3 * (dynamicSpeed +2));
                }
                else
                {
                    grassTexture.Update(-(dynamicSpeed));
                    cloudTexture.Update(-3 * (dynamicSpeed));
                }


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
            MediaPlayer.Stop();
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BackgroundOverScreen(), new GameOverScreen(gameScore));
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
                   
                    
                    //Debug.WriteLine("Collision: player and banana at " + b.Position.Y + ";" +b.Position.X);
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

            foreach (RewardEntity heart in heartVector)
            {
               
                    if (PlayerBoundingBox.Intersects(heart.BoundingBox))
                    {
                        //decrease player health
                        if (player.Health < 3)
                        {
                            player.Health += 1;
                        }
                        //deactivate obstacle
                        heart.Deactivate();

                    }
                
            }

            // collision with water in rush mode not available
            if (!rushMode)
            {
                foreach (ObstacleEntity water in puddleVector)
                {
                    if (PlayerBoundingBox.Intersects(water.BoundingBox))
                    {
                        showGameOverExit();
                        //DrawHud(spriteBatch, ScreenManager.GraphicsDevice.Viewport);
                    }
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

            //draw barrels 
            foreach (RewardEntity h in heartVector)
            {
                h.Draw(spriteBatch);
            }


            foreach (ObstacleEntity w in puddleVector)
            {
                w.Draw(spriteBatch);
            }

            //draw player
            player.Draw(spriteBatch);
           


            //draw clouds
            cloudTexture.Draw(spriteBatch);

            spriteBatch.Draw(headUpDisplayTexture, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(160, 20), Color.Yellow);

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

            //draw RushModeState
            if (cooldownActive)
            {
                spriteBatch.Draw(redDotTexture, new Vector2(320, 10), Color.White);

            }
            else
            {
                spriteBatch.Draw(greenDotTexture, new Vector2(320, 10), Color.White);
            }

            //spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(viewport.Width/2, 30), Color.Black);
            spriteBatch.DrawString(gameFont, "Score: " + gameScore, new Vector2(270, 50), Color.Yellow);
         
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
