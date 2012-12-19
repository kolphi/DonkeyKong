using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;

namespace GameStateManagementSample.Screens
{
    class InstructionsScreen : PhoneMenuScreen
    {
        public InstructionsScreen() : base("")
        {
            Button backButton = new Button("Back");
            backButton.Tapped += menuButton_Tapped;
            MenuButtons.Add(backButton);


        }

        void menuButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        
    }
}
