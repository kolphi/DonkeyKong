using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Entities
{
    class BananaEntity : AnimationEntity
    {
        
        // Amount of hit points that player has
        public int ScorePoints { get; set; }

       

        public BananaEntity()
        {
            // Set the banana to be active
            Active = true;

            Position = new Vector2(0, 0);
        }
        
        public override void Initialize(Vector2 position)
        {
            // Set the starting position of the banana
            Position = position;

            // Set the banana to be active
            Active = true;

            // Set the banana points
            ScorePoints = 3;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(PlayerTexture, Position,null,
            //Color.White,Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, destinationRect, sourceRect, Color);
                // spriteBatch.Draw(PlayerTexture, Position, destinationRect, sourceRect);
            }

        }


        public void setScore(Int16 score){
            this.ScorePoints = score;
        }

        
    }
}
