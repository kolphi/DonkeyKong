using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Media;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Screens
{
    class Label
    {
        /// <summary>
        /// The text displayed in the button.
        /// </summary>
        public string Text = "";

        /// <summary>
        /// The position of the top-left corner of the button.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The size of the button.
        /// </summary>
        public Vector2 Size = new Vector2(250, 75);

        public Label(string text)
        {
            Text = text;
        }

        public void Draw(GameScreen screen)
        {

            // Grab some common items from the ScreenManager
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            SpriteFont font = screen.ScreenManager.Font;

            spriteBatch.DrawString(font, Text, Position, Microsoft.Xna.Framework.Color.Yellow);

        }
    }
}
