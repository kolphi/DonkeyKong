using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Screens
{
    class GameOverScreen : PhoneMenuScreen
    {
       
        public GameOverScreen(Int32 scorePoints)
            : base("")
        {

            // Create the "Menu" and "Exit" buttons for the screen
            //Button pauseButton = new Button(scorePoints.ToString());
            ////pauseButton.Tapped += menuButton_Tapped;
            //MenuButtons.Add(pauseButton);

            Label lbl = new Label("Your score: " + scorePoints.ToString());
            LabelList.Add(lbl);

            // Create the "Menu" and "Exit" buttons for the screen
            Button menuButton = new Button("Menu");
            menuButton.Tapped += menuButton_Tapped;
            MenuButtons.Add(menuButton);

            Button exitButton = new Button("Exit");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);
        }

        void menuButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        void exitButton_Tapped(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        protected override void OnCancel()
        {
            ExitScreen();
            base.OnCancel();
        }
    }
}
