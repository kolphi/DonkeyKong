using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Entities
{
    class WaterEntity : AnimationEntity
    {
        
        public WaterEntity()
        {
            // Set the player to be active
            Active = true;

            Position = new Vector2(0, 0);
        }

        public override void Initialize(Vector2 position)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {            
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, destinationRect, sourceRect, Color);
            }

        }


    }
    
}
