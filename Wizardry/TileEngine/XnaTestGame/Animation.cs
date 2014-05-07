
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace XnaTestGame
{
	class Animation
	{
		// The image representing the collection of images used for animation
		public Texture2D spriteStrip;


		// The scale used to display the sprite strip
		float scale;


		// The time since we last updated the frame
		int elapsedTime;


		// The time we display a frame until the next one
		int frameTime;


		// The number of frames that the animation contains
		int frameCount;


		// The index of the current frame we are displaying
		int currentFrame;


		// The color of the frame we will be displaying
		Color color;


		// The area of the image strip we want to display
		Rectangle sourceRect = new Rectangle();


		// The area where we want to display the image strip in the game
		Rectangle destinationRect = new Rectangle();

		// Position where the image should be drawn
		Vector2 drawPosition;


		// Width of a given frame
		public int FrameWidth;


		// Height of a given frame
		public int FrameHeight;


		// The state of the Animation
		public bool Active;


		// Determines if the animation will keep playing or deactivate after one run
		public bool Looping;


		// Width of a given frame
		public Vector2 Position;

		//Is the texture flipped?
		public bool Flip;

		//Draw depth
		public float drawDepth;


		public void Initialize(Texture2D texture, Vector2 position, int frameCount,
int frametime, Color color, float scale, bool looping)
		{
			// Keep a local copy of the values passed in
			this.color = color;
			this.FrameWidth = texture.Width / frameCount;
			this.FrameHeight = texture.Height;
			this.frameCount = frameCount;
			this.frameTime = frametime;
			this.scale = scale;


			Looping = looping;
			Position = position;
			spriteStrip = texture;


			// Set the time to zero
			elapsedTime = 0;
			currentFrame = 0;


			// Set the Animation to active by default
			Active = true;
		}

		public void Update(GameTime gameTime)
		{
			// Do not update the game if we are not active
			if (Active == false)
				return;


			// Update the elapsed time
			elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;


			// If the elapsed time is larger than the frame time
			// we need to switch frames
			if (elapsedTime > frameTime * 3)
			{
				// Move to the next frame
				currentFrame++;


				// If the currentFrame is equal to frameCount reset currentFrame to zero
				if (currentFrame == frameCount)
				{
					currentFrame = 0;
					// If we are not looping deactivate the animation
					if (Looping == false)
					{
						// Active = false;
					}
				}



				// Reset the elapsed time to zero
				elapsedTime = 0;
			}


			// Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
			sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
			if (!Looping)
			{
				sourceRect = new Rectangle(0, 0, FrameWidth, FrameHeight);
			}


			// Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
			destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
			(int)Position.Y - (int)(FrameHeight * scale) / 2,
			(int)(FrameWidth * scale),
			(int)(FrameHeight * scale));


		}


		// Draw the Animation Strip
		public void Draw(SpriteBatch spriteBatch, Vector2 position, Camera cam)
		{

			Position = position;
			Position -= cam.Position;
			Position.X -= FrameWidth*scale/2;
			Position.Y -= FrameHeight*scale;

			// Only draw the animation when we are active
			if (Active)
			{
				if (Flip)
				{
					spriteBatch.Draw(
					  spriteStrip,			    // Texture
					  Position,	        	    // Position
					  sourceRect,				// Source Rectangle (null = full texture)
					  color,			        // Tint colour
					  0,						// Rotation
					  Vector2.Zero,			    // Origin
					  scale,						// Scale
					  SpriteEffects.FlipHorizontally,		// Special effect
					  drawDepth		    	// Depth
				   );
				}
				else
				{
						// spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
				   spriteBatch.Draw(
					   spriteStrip,			    // Texture
					   Position,	        	// Position
					   sourceRect,				// Source Rectangle (null = full texture)
					   color,			        // Tint colour
					   0,						// Rotation
					   Vector2.Zero,			// Origin
						scale,						// Scale
					   SpriteEffects.None,		// Special effect
					   drawDepth				// Depth
					);
				}
			}
		}

	}
}
