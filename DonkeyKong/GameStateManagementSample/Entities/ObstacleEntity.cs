using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Entities
{
    class ObstacleEntity : GameEntity
    {
        protected Int32 FrameWidth;
        protected Int32 FrameHeight;
        
        public ObstacleEntity()
        {
            // Set the player to be active
            Active = true;

            Position = new Vector2(0, 0);
        }

        public Rectangle BoundingBox
        {
            get
            {

                return new Rectangle(

                    (int)Position.X+10,

                    (int)Position.Y+10,

                    //texture width has to be devided by number of frames to get the size of 1 frame
                    PlayerTexture.Width-30,

                    PlayerTexture.Height-30);

            }

        }
        
        public override void Initialize(Vector2 position)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

        }

        public void initializeTexture(Texture2D entityTexture)
        {
            this.PlayerTexture = entityTexture;
            this.FrameHeight = PlayerTexture.Height;
            this.FrameWidth = PlayerTexture.Width;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(PlayerTexture, Position,null,
            //Color.White,Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, new Vector2(Position.X, Position.Y), Color.White);
                // spriteBatch.Draw(PlayerTexture, Position, destinationRect, sourceRect);
            }

        }

        public virtual void Deactivate()
        {
            this.Active = false;
        }
  


    }
}
