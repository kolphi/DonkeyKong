using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagementSample.Entities
{
    class ParallaxingBackground
    {

        Texture2D backgroundTexture;
        Vector2[] positions;
        float speed;

        public void Initialize(ContentManager content, String texturePath, int screenHeight, float speed)
        {
            // Load the background texture we will be using
            backgroundTexture = content.Load<Texture2D>(texturePath);

            // Set the speed of the background 
            this.speed = speed;

            // If we divide the screen with the texture height then we can determine
            // the number of tiles need. We add 1 to it so that we won't have a gap in the tiling
            positions = new Vector2[screenHeight / backgroundTexture.Height + 3];

            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be side by side to create a tiling effect
                positions[i] = new Vector2(0, i * backgroundTexture.Height);
            }
        } 


        public void Update(float gameSpeed)
        { 
             for (int i = 0; i < positions.Length; i++) 
             {
                  // Update the position of the screen by adding the speed
                  positions[i].Y += gameSpeed;
                  // If the speed has the background moving to the left
                  if (speed <= 0)
                  {
                       // Check the texture is out of view then put that texture at the end of the screen
                      if (positions[i].Y <= -backgroundTexture.Height)
                      {
                          positions[i].Y = backgroundTexture.Height * (positions.Length - 1);
                      }
                  }
                  // If the speed has the background moving to the right
                 else
                 {
                       // Check if the texture is out of view then position it to the start of the screen 
                     if (positions[i].Y >= backgroundTexture.Height * (positions.Length - 1))
                     {
                         positions[i].Y = -backgroundTexture.Height;
                     }
              
                  }
             }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
           
            for (int i = 0; i < positions.Length; i++)
            {
                
                spriteBatch.Draw(backgroundTexture, positions[i], Color.White);
            }
            
        } 

    }
}
