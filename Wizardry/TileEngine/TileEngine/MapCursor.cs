using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
	/// <summary>
	/// A visual cursor which automatically aligns itself to the tile grid of a map.
	/// </summary>
	public class MapCursor
	{
		#region Data Members

		/* Reference to the map which the cursor is highlighting. */
		protected TileMap parentMap;

		/* Switch to turn the cursor on/off. */
		protected bool isActive = false;

		/* The texture used by the cursor. */
		protected Texture2D texture = null;

		/* The cursor's colour. Defaults to red. */
		protected Color cursorColour = Color.Red;

		/* Switch to turn the height shadow on/off. Only applicable for Isometric maps. */
		bool heightShadow = true;

		/* Alpha controlling how transparent the height shadow is.*/
		float heightShadowAlpha = 0.3f;

		#endregion

		#region Constructor

		/// <summary>
		/// Standard MapCursor constructor.
		/// </summary>
		/// <param name="parentMap">The map which the cursor is tied to.</param>
		public MapCursor( TileMap parentMap )
		{
			this.parentMap = parentMap;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the texture used by the cursor.
		/// </summary>
		public Texture2D Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
			}
		}

		/// <summary>
		/// Switch to the the cursor on or off.
		/// </summary>
		public bool Active
		{
			get
			{
				return isActive;
			}
			set
			{
				isActive = value;
			}
		}

		/// <summary>
		/// Gets or sets the cursor's colour. The default colour is red.
		/// </summary>
		public Color Colour
		{
			get
			{
				return cursorColour;
			}
			set
			{
				cursorColour = value;
			}
		}

		/// <summary>
		/// Switch to turn the cursor's height shadow on or off.
		/// </summary>
		public bool HeightShadow
		{
			get
			{
				return heightShadow;
			}
			set
			{
				heightShadow = value;
			}
		}

		/// <summary>
		/// Gets or sets the alpha value which controls the transparency of the cursor's
		/// height shadow.
		/// </summary>
		public float HeightShadowAlpha
		{
			get
			{
				return heightShadowAlpha;
			}
			set
			{
				heightShadowAlpha = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Draws the cursor to the screen using the given SpriteBatch. The SpriteBatch must
		/// be open!
		/// </summary>
		/// <param name="spriteBatch">The open SpriteBatch object to use for drawing.</param>
		public void Draw( SpriteBatch spriteBatch )
		{
			if ( isActive )
			{
				// Convert the mouse's location to a tile location
				Vector2 mousePos = new Vector2( Mouse.GetState().X, Mouse.GetState().Y );
				Vector2 mouseWorld = parentMap.Camera.ScreenToWorldPosition( mousePos );
				Point mouseCell = parentMap.WorldLocationToTileRowCol( mouseWorld );

				int row = mouseCell.Y;
				int col = mouseCell.X;

				// Get the drawing rectangle for the tile
				Rectangle drawRect = parentMap.GetTileRect( row, col );

				// Adjust the drawing rectangle for the camera's position
				drawRect.X -= (int)parentMap.GetCamera().Position.X;
				drawRect.Y -= (int)parentMap.GetCamera().Position.Y;

				// Draw the height shadow if it's active
				if ( parentMap.Type == TileMapType.Isometric && heightShadow )
				{
					// Figure out the height at this spot
					int height = ((IsoTileMap)parentMap).GetHeightAtTile( row, col );

					// Don't bother drawing a shadow for things at ground level
					if ( height > 0 )
					{
						Rectangle shadowDrawRect = drawRect;
						shadowDrawRect.Y -= height / ((IsoTileMap)parentMap).LayerHeight * 
											((IsoTileMap)parentMap).HalfTileSize;

						spriteBatch.Draw( Texture, shadowDrawRect, Colour * HeightShadowAlpha );
					}
				}

				spriteBatch.Draw( Texture, drawRect, Colour );
			}
		}

		#endregion
	}
}
