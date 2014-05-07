using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace WizardryShared
{
	public class Animation
	{
		#region Member Variables

		// The image representing the collection of images used for animation
		public Texture2D spriteStrip { get; set; }

		// The scale used to display the sprite strip
		private float scale;

		// The time since we last updated the frame
		private int elapsedTime;

		// The time we display a frame until the next one
		private int frameTime;

		// The number of frames that the animation contains
		private int frameCount;

		// The index of the current frame we are displaying
		private int currentFrame;

		// The color of the frame we will be displaying
        private Color color = Color.White;

		// The area of the image strip we want to display
		private Rectangle sourceRect = new Rectangle();

		// The area where we want to display the image strip in the game
		private Rectangle destinationRect = new Rectangle();

		// Width of a given frame
		private int frameWidth;

		// Height of a given frame
		private int frameHeight;

		// The state of the Animation
		private bool active;

		// Width of a given frame
		private Vector2 position;

		//Is the texture flipped?
		private bool flip;

		//Draw depth
		private float drawDepth;

        //Color Array
        private Color[,] colorArray;

        //Rotation of the sprite
        private float rotation;

        //Bounding box
        private Rectangle boundingBox;

        //Biggest side
        private float biggestSide;

		#endregion

		#region Properties

		public int FrameWidth
		{
			get
			{
				return frameWidth;
			}
			set
			{
				frameWidth = value;
			}
		}



		// Height of a given frame
		public int FrameHeight
		{
			get
			{
				return frameHeight;
			}
			set
			{
				frameHeight = value;
			}
		}



		// The state of the Animation
		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		// Width of a given frame
		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}


		//Color modifier
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}


		//Draw depth
		public float DrawDepth
		{
			get
			{
				return drawDepth;
			}
			set
			{
				drawDepth = value;
			}
		}

		//Scale
		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

        //Draw depth
        public Color[,] ColorArray
        {
            get
            {
               return colorArray;
            }
        }

        //Current frame number
        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }
        }

        //Rotation of image
        public float Rotation
        {
            get
            {
                return rotation;
            }
        }

        //Bounding box
        public Rectangle BoundingBox
        {
            get
            {
                return boundingBox;
            }
             set
            {
                boundingBox = value;
            }
        }


		#endregion

        public Animation(){}

        public Animation(AnimationTexture texture, int frameCount, int frametime, float scale, bool flip)
        {
            Initialize(texture, frameCount, frametime, scale, flip);
        }

		#region Initialize, Update and Draw methods


		public void Initialize(AnimationTexture texture, int frameCount, int frametime, float scale, bool flip)
		{
            colorArray = texture.ColorArray;

			// Keep a local copy of the values passed in
			this.FrameWidth = texture.Texture.Width / frameCount;
			this.FrameHeight = texture.Texture.Height;
			this.frameCount = frameCount;
			this.frameTime = frametime;
			this.scale = scale;

            this.flip = flip;


			spriteStrip = texture.Texture;


			// Set the time to zero
			elapsedTime = 0;
			currentFrame = 0;
            biggestSide = Math.Max((frameWidth * scale), (frameHeight * scale));
            boundingBox = new Rectangle(0, 0, (int)(biggestSide), (int)(biggestSide));


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
			if (elapsedTime > frameTime)
			{
				// Move to the next frame
				currentFrame++;

				// If the currentFrame is equal to frameCount reset currentFrame to zero
				if (currentFrame == frameCount)
				{
					currentFrame = 0;
				}

				// Reset the elapsed time to zero
				elapsedTime = 0;
			}
		}


		// Draw the Animation Strip
		public void Draw( SpriteBatch spriteBatch, Vector2 position, float rotation, float alpha = 1 )
		{
            //Update the bounding box
            boundingBox.X = (int)(position.X - biggestSide/ 2);
            boundingBox.Y = (int)(position.Y - biggestSide / 2);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);


            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale) / 2,
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));


			
			this.position = position;
            this.rotation = rotation;
			/*
			this.position -= cam.Position;
			this.position.X -= FrameWidth * scale / 2;
			this.position.Y -= FrameHeight * scale;
			 */

			// Only draw the animation when we are active
			if (Active)
			{
                if ((rotation >= 0 && rotation < MathHelper.Pi / 2) || (rotation <= 0 && rotation > -MathHelper.Pi / 2))
                {
                    if (flip)
                    {
                        spriteBatch.Draw(
                          spriteStrip,								// Texture
                          Position,	        						// Position
                          sourceRect,								// Source Rectangle (null = full texture)
                          color * alpha,							// Tint colour
                          rotation,									// Rotation
                          new Vector2(FrameWidth / 2, FrameHeight / 2),	// Origin
                          scale,									// Scale
                          SpriteEffects.FlipHorizontally,			// Special effect
                          drawDepth		    						// Depth
                       );
                    }
                    else
                    {
                        spriteBatch.Draw(
                            spriteStrip,							// Texture
                            Position,	        					// Position
                            sourceRect,								// Source Rectangle (null = full texture)
                            color * alpha,							// Tint colour
                            rotation,								// Rotation
                            new Vector2(FrameWidth / 2, FrameHeight / 2),	// Origin
                            scale,										// Scale
                            SpriteEffects.None,							// Special effect
                            drawDepth									// Depth
                         );
                    }
                }
                else
                {
                    if (flip)
                    {
                        spriteBatch.Draw(
                          spriteStrip,								// Texture
                          Position,	        						// Position
                          sourceRect,								// Source Rectangle (null = full texture)
                          color * alpha,							// Tint colour
                          rotation,									// Rotation
                          new Vector2(FrameWidth / 2, FrameHeight / 2),	// Origin
                          scale,									// Scale
                          SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,			// Special effect
                          drawDepth		    						// Depth
                       );
                    }
                    else
                    {
                        spriteBatch.Draw(
                            spriteStrip,							// Texture
                            Position,	        					// Position
                            sourceRect,								// Source Rectangle (null = full texture)
                            color * alpha,							// Tint colour
                            rotation,								// Rotation
                            new Vector2(FrameWidth / 2, FrameHeight / 2),	// Origin
                            scale,										// Scale
                            SpriteEffects.FlipVertically,							// Special effect
                            drawDepth									// Depth
                         );
                    }
                }
			}
		}
		#endregion  
	}
}
