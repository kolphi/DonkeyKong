using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagementSample.Entities
{
    class AnimationEntity : GameEntity 
    {
       
        protected float scale;
        protected Int32 elapsedTime;
        protected Int32 FrameTime;
        protected Int32 FrameCount;
        protected Int32 currentFrame;
        protected Color Color;
        protected Rectangle sourceRect;
        protected Rectangle destinationRect;
        protected Int32 FrameWidth;
        protected Int32 FrameHeight;

        protected Boolean Looping;


        public virtual void Initialize(Vector2 position)
        {
            this.boundingBox = new BoundingBox();
            this.Position = position;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default
            Active = true;
        }

        public virtual Rectangle BoundingBox
        {
            get
            {

                return new Rectangle(

                    (int)Position.X,

                    (int)Position.Y,

                    //texture width has to be devided by number of frames to get the size of 1 frame
                    PlayerTexture.Width/FrameCount,

                    PlayerTexture.Height);

            }

        }

        public virtual void InitializeAnimation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, float scale, bool looping)
        {

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Keep a local copy of the values passed in
            this.PlayerTexture = texture;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.FrameCount = frameCount;
            this.FrameTime = frametime;
            this.Color = color;
            this.scale = scale;
            this.Looping = looping;

            
           
        }


        public virtual void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (!Active)
                return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time we need to switch frames
            if (elapsedTime > FrameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero 
                if (currentFrame == FrameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (!Looping)
                        Active = false;
                }
                // Reset the elapsed time to zero
                elapsedTime = 0;



            }
               // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
             sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
             destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale) / 2, (int)(FrameWidth * scale), (int)(FrameHeight * scale));

        }


        // Draw the Animation Strip 
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(PlayerTexture, destinationRect, sourceRect, Color);
                
            }
        }

        public virtual void Deactivate()
        {
            this.Active = false;
        }


        public bool isActive()
        {
            return Active;
        }



    }
}
