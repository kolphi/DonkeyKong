using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagementSample.Entities
{
    class PlayerEntity : AnimationEntity
    {


        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;


        
        public virtual void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, null,
                 Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

       
  

    }
}
