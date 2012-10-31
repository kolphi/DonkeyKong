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
        Texture2D SpriteStrip;

        float scale;
        Int32 elapsedTime;
        Int32 FrameTime;
        Int32 FrameCount;
        Int32 CurrentFrame;
        Color color;
        Rectangle sourceRect;
        Rectangle destinationRect;
        Int32 FrameWidth;
        Int32 FrameHeight;

        Boolean Looping;


        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.Position = position;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.FrameCount = frameCount;
            this.SpriteStrip = texture;

            // Set the time to zero
            elapsedTime = 0;
            CurrentFrame = 0;

            // Set the Animation to active by default
            Active = true;
        }


        public void Update(GameTime gameTime)
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
                 CurrentFrame++;
 
          // If the currentFrame is equal to frameCount reset currentFrame to zero 
          if (CurrentFrame == FrameCount)
          {
               CurrentFrame = 0;
               // If we are not looping deactivate the animation
               if (!Looping)
                    Active = false;
          }
          // Reset the elapsed time to zero
          elapsedTime = 0;



        }
    }

}
