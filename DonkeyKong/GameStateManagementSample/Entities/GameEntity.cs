using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagementSample.Entities
{

  
    abstract class GameEntity
    {
        // Animation representing the player
        public Texture2D PlayerTexture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Get the width of the player ship
        public int Width
        {
            get { return PlayerTexture.Width; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerTexture.Height; }
        }

        #region basemethods
        public void Initialize()
        {
        }

        public void Update()
        {
        }

        public void Draw()
        {
        }

        #endregion


    }
}
