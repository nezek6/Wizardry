using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	public class TileLayer
	{
		#region Data Members

		/* The map on which this layer exists. */
		private TileMap parentMap;

		/* The layer's index in the map. */
		private int layerIndex = -1;

		/* The actual grid of tiles that make up this layer. */
		private Tile[,] map;

		/* Determines the visibility of the layer. */
		private float alpha = 1f;  //1 is visible, 0 is transparent

		#endregion

		#region Constructors

		/// <summary>
		/// Basic constructor creating a new TileLayer of width * height tiles.
		/// The tiles are set to having no texture.
		/// </summary>
		/// <param name="width">The width of the layer.</param>
		/// <param name="height">The height of the layer.</param>
		public TileLayer( int width, int height )
		{
			map = new Tile[height, width];

			for ( int row = 0; row < height; row++ )
			{
				for ( int col = 0; col < width; col++ )
				{
					map[row, col] = new Tile();
					map[row, col].TextureId = -1;
				}
			}
		}

		/* Constructor allowing user to create a TileLayer from an existing map. */
		public TileLayer( Tile[,] existingMap )
		{
			map = (Tile[,])existingMap.Clone();
		}

		#endregion

		#region Properties

		/* Gets/Sets the map to which the tile layer is tied.
		 * Note that that the parent map is automatically set when a layer is added to a tile map. */
		public TileMap ParentMap
		{
			get
			{
				return parentMap;
			}
			set
			{
				parentMap = value;
			}
		}

		/* The layer's index in the map.
		 * Note that this is set automatically when a layer is added to a tile map. */
		public int LayerIndex
		{
			get
			{
				return layerIndex;
			}
			set
			{
				layerIndex = value;
			}
		}

		/* The number of columns of tiles in the map. */
		public int NumCols
		{
			get
			{
				return map.GetLength( 1 );
			}
		}

		/* The number of rows of tiles in the map. */
		public int NumRows
		{
			get
			{
				return map.GetLength( 0 );
			}
		}

		/* The width of the entire tile layer in pixels. */
		public int WidthInPixels
		{
			get
			{
				return (int)Math.Ceiling( NumCols * parentMap.TileWidth * parentMap.Scale );
			}
		}

		/* The height of the entire tile layer in pixels. */
		public int HeightInPixels
		{
			get
			{
				return (int)Math.Ceiling( NumRows * parentMap.TileHeight * parentMap.Scale );
			}
		}

		/* Gets/Sets the Alpha level of the layer. Setting the alpha to 0 will make the layer invisible. */
		public float Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = MathHelper.Clamp( value, 0f, 1f );
			}
		}

		/* Gets the layer's tiles matrix. */
		public Tile[,] Tiles
		{
			get
			{
				return map;
			}
		}

		#endregion

		#region Old-Style Accessors, For Grizzled Programmers

		/* Gets the Alpha level of the layer. */
		public float GetAlpha()
		{
			return Alpha;
		}

		/* Gets the Alpha level of the layer. Setting the alpha to 0 will make the layer invisible. */
		public void SetAlpha( float alpha )
		{
			Alpha = alpha;
		}

		/* Gets the map to which the tile layer is tied. */
		public TileMap GetParentMap()
		{
			return ParentMap;
		}

		/* Sets the map to which the tile layer is tied.
		 * Note that this is called automatically when a layer is added to a tile map. */
		public void SetParentMap( TileMap parentMap )
		{
			ParentMap = parentMap;
		}

		/* Gets the number of tiles in the map, horizontally */
		public int GetWidth()
		{
			return NumCols;
		}

		/* Gets the number of tiles in the map, vertically */
		public int GetHeight()
		{
			return NumRows;
		}

		/* Gets the width of the entire tile layer in pixels. */
		public int GetWidthInPixels()
		{
			return WidthInPixels;
		}

		/* Gets the height of the entire tile layer in pixels. */
		public int GetHeightInPixels()
		{
			return HeightInPixels;
		}

		#endregion

		#region Methods

		/* Allows the user to set a specific tile's texture index manually. */
		public void SetTileTextureIndex( int row, int col, int textureIndex )
		{
			map[row, col].TextureId = textureIndex;
		}

		public int GetTileTextureIndex( int row, int col )
		{
			return map[row, col].TextureId;
		}

		public void Draw( SpriteBatch batch )
		{
			if ( parentMap.Type == TileMapType.Square ||
				 parentMap.Type == TileMapType.Square )
			{
				// Turns out the way the engine is set up,
				//  drawing hex or square is the same.
				Draw_SqHex( batch );
			}
			else if ( parentMap.Type == TileMapType.Isometric )
			{
				Draw_Iso( batch );
			}
			else
			{
				// Erroneous type
				throw new Exception( "Map to draw has unrecognized type. Wtfm8." );
			}
		}

		private void Draw_SqHex( SpriteBatch batch )
		{
			for ( int row = 0; row < this.NumRows; row++ )
			{
				for ( int col = 0; col < this.NumCols; col++ )
				{
					int textureIndex = map[row, col].TextureId;

					if ( textureIndex == -1 ) // Skip "no texture" tiles
						continue;

					Rectangle rect = parentMap.GetTileRect( row, col ); // Retrieve this tile's rectangle in the world
					// Adjust drawing for the camera position!
					rect.X -= (int)parentMap.GetCamera().Position.X;
					rect.Y -= (int)parentMap.GetCamera().Position.Y;
					batch.Draw(
						parentMap.GetTextureWithIndex( textureIndex ),
						rect,
						new Color( new Vector4( 1f, 1f, 1f, Alpha ) )
						);
				}
			}
		}

		private void Draw_Iso( SpriteBatch batch )
		{
			int layerHeight = ((IsoTileMap)parentMap).LayerHeight;
			int height = LayerIndex * layerHeight;
			
			// Limit drawing to the tiles on screen
			Rectangle camRect = parentMap.Camera.GetViewRect();
			Point topLeft = parentMap.WorldLocationToTileRowCol( new Vector2( camRect.X, camRect.Y ) );

			Point bottomRight = parentMap.WorldLocationToTileRowCol( new Vector2( camRect.X + camRect.Width, camRect.Y + camRect.Height ) );
			if ( bottomRight.X == -1 && bottomRight.Y == -1 )
			{
				bottomRight.X = parentMap.NumCols - 1;
				bottomRight.Y = parentMap.NumRows - 1;
			}

			topLeft.X = (int)(MathHelper.Max( 0, topLeft.X - 2 ));
			topLeft.Y = (int)(MathHelper.Max( 0, topLeft.Y - 2 ));
			bottomRight.X = (int)(MathHelper.Min( this.NumCols, bottomRight.X + layerHeight + 2 ));
			bottomRight.Y = (int)(MathHelper.Min( this.NumRows, bottomRight.Y + layerHeight + 2 ));

			for ( int row = topLeft.Y; row < bottomRight.Y; row++ )
			{
				for ( int col = topLeft.X; col < bottomRight.X; col++ )
				{
					int textureIndex = map[row, col].TextureId;

					if ( textureIndex == -1 ) // Skip "no texture" tiles
						continue;

					// Calculate this tile's drawing depth
					float drawDepth = ((IsoTileMap)parentMap).GetDrawDepth( row, col, height, 0 );


					Rectangle rect = parentMap.GetTileRect( row, col, LayerIndex ); // Retrieve this tile's rectangle in the world
					// Adjust drawing for the camera position!
					rect.X -= (int)parentMap.GetCamera().Position.X;
					rect.Y -= (int)parentMap.GetCamera().Position.Y;

					batch.Draw(
						parentMap.GetTextureWithIndex( textureIndex ),	// Texture
						rect,											// Destination Rectangle
						null,											// Source Rectangle
						Color.White * Alpha,							// Colour
						0f,												// Rotation
						Vector2.Zero,									// Origin
						SpriteEffects.None,								// Sprite Effects
						drawDepth										// Drawing Depth
						);
				}
			}

		}

		#endregion
	}
}