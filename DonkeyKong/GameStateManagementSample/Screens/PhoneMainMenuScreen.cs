#region File Description
//-----------------------------------------------------------------------------
// PhoneMainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using GameStateManagementSample.Screens;

namespace GameStateManagementSample
{
    class PhoneMainMenuScreen : PhoneMenuScreen
    {


       public bool musicState = true;

        public PhoneMainMenuScreen()
            : base("")

        {
            // Create a button to start the game
            Button playButton = new Button("Play");
            playButton.Tapped += playButton_Tapped;
            MenuButtons.Add(playButton);

            // Create two buttons to toggle sound effects and music. This sample just shows one way
            // of making and using these buttons; it doesn't actually have sound effects or music
            //BooleanButton sfxButton = new BooleanButton("Sound Effects", true);
            //sfxButton.Tapped += sfxButton_Tapped;
            //MenuButtons.Add(sfxButton);

            Button instructionsButton = new Button("Instructions");
            instructionsButton.Tapped += instructionsButton_Tapped;
            MenuButtons.Add(instructionsButton);

            BooleanButton musicButton = new BooleanButton("Music", true);
            musicButton.Tapped += musicButton_Tapped;
            MenuButtons.Add(musicButton);
            //musicState = musicButton.getValue();
            
        }

        void playButton_Tapped(object sender, EventArgs e)
        {
            
            // When the "Play" button is tapped, we load the GameplayScreen
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(musicState));
        }

        //void sfxButton_Tapped(object sender, EventArgs e)
        //{
        //    BooleanButton button = sender as BooleanButton;

        //    // In a real game, you'd want to store away the value of 
        //    // the button to turn off sounds here. :)
        //}

        void instructionsButton_Tapped(object sender, EventArgs e)
        {
            // When the "Play" button is tapped, we load the GameplayScreen
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BackgroundInstructions(), new InstructionsScreen());
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;
            musicState = button.getValue();

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)
            Debug.WriteLine("MUSICSTATE" + musicState);
           // Debug.WriteLine(button.getValue());
            
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            base.OnCancel();
        }
    }
}
