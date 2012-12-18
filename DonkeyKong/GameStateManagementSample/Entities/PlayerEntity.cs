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


        // Amount of hit points that player has
        public int Health { get; set; }

        public float Rotation { get; set; }

        public PlayerEntity()
        {
            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 3;

            //set rotation
            Rotation = 0.5f;

        }
        
        public override void Initialize(Vector2 position)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 3;

            //set rotation
            Rotation = 0;
            
   
        }
      
        
        public override void Draw(SpriteBatch spriteBatch)
        {
           // spriteBatch.Draw(PlayerTexture, Position,null,
            //Color.White,Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, destinationRect, sourceRect, Color, Rotation, Vector2.Zero, SpriteEffects.None, 0f);
               // spriteBatch.Draw(PlayerTexture, Position, destinationRect, sourceRect);
            }

        }

       
  

    }
}
