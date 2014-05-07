using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
	/// <summary>
	/// The Camera controls which portion of the overall world is visible on the screen.
	/// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
	{
		#region Data Members

		/* The boundaries where the camera can move to. */
		private Rectangle bounds;

		/* The camera's position in the world. */
        private Vector2 position = Vector2.Zero;

		/* Toggling this allows the camera position to be controlled directly by the user.
		 * Set to true = user can control the camera with arrow keys/gamepad. */
		private bool autonomous = true;

		/* How fast the camera moves. This is only relevant if in autonomous mode. */
		private float speed = 5.0f;

		#endregion

		#region Constructors

		/// <summary>
		/// Standard constructor for the Camera class.
		/// </summary>
		/// <param name="game">Reference to the game in which this Camera is a component.</param>
		public Camera( Game game ) : base( game )
		{
			bounds = new Rectangle( 0, 0, int.MaxValue, int.MaxValue );			
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets whether the camera is Autonomous (i.e. not tied to another object).
		/// <remarks>When the camera is Autonomous, it can be moved directly using the WASD keys.</remarks>
		/// </summary>
		public bool Autonomous
		{
			get 
			{ 
				return autonomous; 
			}
			set
			{
				autonomous = value;
			}
		}

		/// <summary>
		/// Gets or sets the Camera's movement speed. The speed only matters if the Camera is Autonomous.
		/// </summary>
        public float Speed
        {
            get 
			{ 
				return speed; 
			}
            set 
            {
                // Ensure minimum speed of 1f
                speed = (float)Math.Max(value, 1f);
            }
        }

		/// <summary>
		/// Gets or sets the Camera's position in the world.
		/// </summary>
		public Vector2 Position
		{
			get 
			{ 
				return position; 
			}
			set 
			{ 
				position = value;

				position.X = MathHelper.Clamp( Position.X, bounds.Left, bounds.Right );
				position.Y = MathHelper.Clamp( Position.Y, bounds.Top, bounds.Bottom );
			}
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Centers the camera on the given position.
		/// </summary>
		/// <param name="position">The position to center the camera on.</param>
		public void CenterOn( Vector2 position )
		{
			this.position.X = position.X - ( Game.GraphicsDevice.Viewport.Width / 2 );
			this.position.Y = position.Y - ( Game.GraphicsDevice.Viewport.Height / 2 );

			// Make sure the camera always stays within the bounds
			this.position.X = MathHelper.Clamp( Position.X, bounds.Left, bounds.Right );
			this.position.Y = MathHelper.Clamp( Position.Y, bounds.Top, bounds.Bottom );
		}

		/// <summary>
		/// Sets the boundaries in the world where the camera is allowed to go.
		/// </summary>
		/// <param name="topLeft">The top-leftmost boundary where the camera can go.</param>
		/// <param name="bottomRight">The bottom-rightmost boundary where the camera can go.</param>
		public void SetBounds( Point topLeft, Point bottomRight )
		{
			bounds = new Rectangle( 
				topLeft.X, 
				topLeft.Y,
				bottomRight.X - topLeft.X,
				bottomRight.Y - topLeft.Y
				);

			// Make sure the camera always stays within the bounds
			position.X = MathHelper.Clamp( Position.X, bounds.Left, bounds.Right );
			position.Y = MathHelper.Clamp( Position.Y, bounds.Top, bounds.Bottom );
		}

		/// <summary>
		/// Gets a rectangle representing the visible area in the world.
		/// </summary>
		/// <returns>A rectangle which represents the visible area in the world.</returns>
		public Rectangle GetViewRect()
		{
			return new Rectangle(
				(int)position.X,
				(int)position.Y,
				Game.GraphicsDevice.Viewport.Width,
				Game.GraphicsDevice.Viewport.Height
				);
		}

		/// <summary>
		/// Converts on-screen coordinates to world coordinates based on the Camera's position.
		/// </summary>
		/// <param name="screenCoordinates"></param>
		/// <returns>The world coordinates for the given on-screen coordinates, or (-1,-1) if the given coordinates are not in the world.</returns>
		public Vector2 ScreenToWorldPosition( Vector2 screenCoordinates )
		{
			/* Check if the coordinates given are actually outside of the game window. */
			if ( screenCoordinates.X < 0 || screenCoordinates.Y < 0 ||
				 screenCoordinates.X >= Game.GraphicsDevice.Viewport.Width ||
				 screenCoordinates.Y >= Game.GraphicsDevice.Viewport.Height )
			{
				return new Vector2( -1, -1 );
			}
			
			return screenCoordinates + Position;
		}

		/// <summary>
		/// Override of the base GameComponent's Update() method. 
		/// This will automatically be called if the Camera is attached to a game.
		/// </summary>
		/// <param name="gameTime">A snapshot</param>
		public override void Update(GameTime gameTime)
		{
			if ( autonomous )
			{
				Vector2 motion = Vector2.Zero;
				
				GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
				motion = new Vector2(gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y);
				
				KeyboardState keyState = Keyboard.GetState();
				if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
					motion.Y--;
				if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
					motion.Y++;
				if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
					motion.X++;
				if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
					motion.X--;
				
				if ( motion != Vector2.Zero ) 
				{
					motion.Normalize();
				}
				
				position += motion * Speed;
			}

			// Make sure the camera always stays within the bounds
			position.X = MathHelper.Clamp(Position.X, bounds.Left, bounds.Right);
			position.Y = MathHelper.Clamp(Position.Y, bounds.Top, bounds.Bottom);

			base.Update(gameTime);
		}

		/// <summary>
		/// Checks of any portion of the given rectangle is on the screen.
		/// </summary>
		/// <param name="rectangle">The rectangle to check.</param>
		/// <returns>True if the given rectangle is on the screen and false otherwise.</returns>
		public bool IsRectangleOnScreen( Rectangle rectangle )
		{
			return rectangle.Intersects( GetViewRect() );
		}

		#endregion
	}
}