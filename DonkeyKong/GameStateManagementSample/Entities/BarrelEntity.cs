using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Entities
{
    class BarrelEntity : AnimationEntity
    {

        public int ScorePoints { get; set; }

        public BarrelEntity()
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

            // Set the barrel points
            ScorePoints = 1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {            
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, destinationRect, sourceRect, Color);
            }

        }

        public void setScore(Int16 score)
        {
            this.ScorePoints = score;
        }

    }
}
