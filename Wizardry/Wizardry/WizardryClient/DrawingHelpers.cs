using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryClient
{
	public class DrawingHelpers
	{
		// The underlying texture for drawing primitives
		private static Texture2D raw;

		/// <summary>
		/// Initializes the DrawingHelpers. This must be called before any of the primitive drawing
		/// tools are used.
		/// </summary>
		/// <param name="graphicsDevice">The Game's graphics device.</param>
		public static void Initialize( GraphicsDevice graphicsDevice )
		{
			raw = new Texture2D( GameManager.Instance.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color );
			raw.SetData<Color>( new[] { Color.White } );
		}

		/// <summary>
		/// Draws a point with specified size and colour to the screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use for drawing. The SpriteBatch must be open.</param>
		/// <param name="point">The point to draw.</param>
		/// <param name="colour">The colour of the point.</param>
		/// <param name="size">How large to make the point.</param>
		/// <param name="adjustCamera">Whether or not to adjust the point's location for the world camera.</param>
		public static void DrawPoint( SpriteBatch spriteBatch, Point point, Color colour, int size, bool adjustCamera = false )
		{
			// Make size odd
			size = ( ( size & 1 ) != 0 ) ? size : size + 1;

			if ( adjustCamera && GameManager.Instance.Camera != null )
			{
				point.X = point.X - (int)GameManager.Instance.Camera.Position.X;
				point.Y = point.Y - (int)GameManager.Instance.Camera.Position.Y;
			}

			spriteBatch.Draw(
				raw,
				new Rectangle( point.X - ( size / 2 ), point.Y - ( size / 2 ), size, size ),
				null,
				colour,
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0 );
		}

		/// <summary>
		/// Draws a point with the specified size and colour to the screen.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use for drawing. The SpriteBatch must be open.</param>
		/// <param name="point">The point to draw.</param>
		/// <param name="colour">The colour of the point.</param>
		/// <param name="size">How large to make the point.</param>
		/// <param name="adjustCamera">Whether or not to adjust the point's location for the world camera.</param>
		public static void DrawPoint( SpriteBatch spriteBatch, Vector2 point, Color colour, int size, bool adjustCamera = false )
		{
			DrawPoint( spriteBatch, new Point( (int)point.X, (int)point.Y ), colour, size, adjustCamera );
		}

		/// <summary>
		/// Draws an arbitary line between two points with the given width and colour.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use for drawing. The SpriteBatch must be open.</param>
		/// <param name="point1">The starting point of the line.</param>
		/// <param name="point2">The ending point of the line.</param>
		/// <param name="width">The width of the line.</param>
		/// <param name="colour">The colour of the line.</param>
		/// <param name="adjustCamera">Whether or not to adjust the line's location for the world camera.</param>
		public static void DrawLine( SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, float width, Color colour, bool adjustCamera = false  )
		{
			if ( adjustCamera && GameManager.Instance.Camera != null )
			{
				point1 -= GameManager.Instance.Camera.Position;
				point2 -= GameManager.Instance.Camera.Position;
			}

			float angle = (float)Math.Atan2( point2.Y - point1.Y, point2.X - point1.X );
			float length = Vector2.Distance( point1, point2 );

			spriteBatch.Draw( 
				raw, 
				point1, 
				null, 
				colour,
				angle, 
				Vector2.Zero, 
				new Vector2( length, width ),
				SpriteEffects.None, 
				0 );
		}

		/// <summary>
		/// Draws the specified rectangle to the screen, filled in with the specified colour.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use for drawing. The SpriteBatch must be open.</param>
		/// <param name="rectangle">The rectangle to draw.</param>
		/// <param name="colour">The colour of the rectangle.</param>
		/// <param name="adjustCamera">Whether or not to adjust the rectangle's location for the world camera.</param>
		public static void DrawFilledRectangle( SpriteBatch spriteBatch, Rectangle rectangle, Color colour, bool adjustCamera = false )
		{
			if ( adjustCamera && GameManager.Instance.Camera != null )
			{
				rectangle.X = rectangle.X - (int)GameManager.Instance.Camera.Position.X;
				rectangle.Y = rectangle.Y - (int)GameManager.Instance.Camera.Position.Y;
			}

			spriteBatch.Draw(
				raw,
				rectangle,
				null,
				colour,
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0 );
		}

		/// <summary>
		/// Draws the specified rectangle to the screen without filling it in.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch to use for drawing. The SpriteBatch must be open.</param>
		/// <param name="rectangle">The rectangle to draw.</param>
		/// <param name="width">The width of the lines making up the rectangle.</param>
		/// <param name="colour">The colour of the rectangle.</param>
		/// <param name="adjustCamera">Whether or not to adjust the rectangle's location for the world camera.</param>
		public static void DrawWireRectangle( SpriteBatch spriteBatch, Rectangle rectangle, float width, Color colour, bool adjustCamera = false )
		{
			if ( adjustCamera && GameManager.Instance.Camera != null )
			{
				rectangle.X = rectangle.X - (int)GameManager.Instance.Camera.Position.X;
				rectangle.Y = rectangle.Y - (int)GameManager.Instance.Camera.Position.Y;
			}

			// Figure out corners
			Vector2 topLeft = new Vector2( rectangle.X, rectangle.Y );
			Vector2 topRight = new Vector2( rectangle.X + rectangle.Width, rectangle.Y );
			Vector2 botLeft = new Vector2( rectangle.X, rectangle.Y + rectangle.Height );
			Vector2 botRight = new Vector2( rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height );

			// Draw top line
			DrawLine( spriteBatch, topLeft, topRight, width, colour );
			
			// Draw right side
			DrawLine( spriteBatch, topRight, botRight, width, colour );

			// Draw bottom line
			DrawLine( spriteBatch, botRight, botLeft, width, colour );

			// Draw left side
			DrawLine( spriteBatch, botLeft, topLeft, width, colour );
		}
	}
}
