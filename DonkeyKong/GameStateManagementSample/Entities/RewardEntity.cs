using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagementSample.Entities
{
    class RewardEntity : DrawableEntity
    {

        protected Int32 FrameWidth;
        protected Int32 FrameHeight;
        
        public RewardEntity()
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

                    (int)Position.X,

                    (int)Position.Y,

                   
                    PlayerTexture.Width,

                    PlayerTexture.Height);

            }

        }
        
        public override void Initialize(Vector2 position)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Active = true;

        }

        public void InitializeTexture(Texture2D entityTexture)
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
