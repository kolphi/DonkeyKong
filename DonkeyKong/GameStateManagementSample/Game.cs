#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagementSample.Entities;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace GameStateManagementSample
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class GameStateManagementGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        PlayerEntity player;
        
        SpriteBatch spriteBatch;

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameStateManagementGame()
        {
            Content.RootDirectory = "Content";

          

            graphics = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromTicks(333333);

#if WINDOWS_PHONE
            graphics.IsFullScreen = true;

            // Choose whether you want a landscape or portait game by using one of the two helper functions.
            //InitializeLandscapeGraphics();
            InitializePortraitGraphics();
            
#endif

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

#if WINDOWS_PHONE
            // Hook events on the PhoneApplicationService so we're notified of the application's life cycle
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching += 
                new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>(GameLaunching);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated += 
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated += 
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
#else
            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize(ContentManager content, String texturePath, int screenWidth, int speed)
        {
            // TODO: Add your initialization logic here
            Texture2D playerTexture = content.Load<Texture2D>("grass1");
            //player.Initialize(texture, position);
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

            // TODO: use this.Content to load your game content here
            // Load the player resources 
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            //player.Initialize(Content.Load<Texture2D>("player"), playerPosition); 
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

            // We have different menus for Windows Phone to take advantage of the touch interface
#if WINDOWS_PHONE
            screenManager.AddScreen(new PhoneMainMenuScreen(), null);
#else
            screenManager.AddScreen(new MainMenuScreen(), null);
#endif
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            Debug.WriteLine(GraphicsDevice.Viewport.Height);
          

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Helper method to the initialize the game to be a portrait game.
        /// </summary>
        private void InitializePortraitGraphics()
        {
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
        }

       

        /// <summary>
        /// Helper method to initialize the game to be a landscape game.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
        }

        void GameLaunching(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            AddInitialScreens();
        }

        void GameActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            // Try to deserialize the screen manager
            if (!screenManager.Activate(e.IsApplicationInstancePreserved))
            {
                // If the screen manager fails to deserialize, add the initial screens
                AddInitialScreens();
            }
        }

        void GameDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            // Serialize the screen manager when the game deactivated
            screenManager.Deactivate();
        }
#endif
    }
}
