using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class IsoTileMap : TileMap
	{
		#region Constants

		public const float DEPTH_DAMPENER = 0.0000001f;

		// TODO: This term needs adjusting based on map size
		public const float HEIGHT_TERM_DAMPENER = 0.0001f;  

		#endregion

		#region Data Members

		private int halfTileSize = 0;

		private int quarterTileSize = 0;

		private int layerHeight = 10;

		private Texture2D pickingAid;

		private Texture2D slopeHeightMap;

		private int[,] slopeMap;

		#endregion

		#region Constructors

		public IsoTileMap( Camera camera ):base( camera )
		{
			this.type = TileMapType.Isometric;
		}

		#endregion

		#region Properties

		public override int WidthInPixels
		{
			get
			{
				return (int)Math.Ceiling( ( ( NumCols * TileWidth ) + HalfTileSize ) * Scale );
			}
		}

		public override int HeightInPixels
		{
			get
			{
				if ( NumRows % 2 == 0 )
				{
					// Even number of rows
					return (int)Math.Ceiling( ( ( NumRows / 2 + 1 ) * HalfTileSize + QuarterTileSize ) * Scale );
				}
				else
				{
					// Odd number of rows
					return (int)Math.Ceiling( ( ( NumRows / 2 + 1 ) * HalfTileSize + HalfTileSize  ) * Scale );
				}
			}
		}

		public int HalfTileSize
		{
			get 
			{ 
				return halfTileSize; 
			}
		}

		public int QuarterTileSize
		{
			get 
			{ 
				return quarterTileSize; 
			}
		}

		public override int TileWidth
		{
			get
			{
				return base.TileWidth;
			}
			protected set
			{
				halfTileSize = value / 2;
				quarterTileSize = value / 4;
				base.TileWidth = value;
			}
		}

		public override int TileHeight
		{
			get
			{
				return base.TileHeight;
			}
			protected set
			{
				halfTileSize = value / 2;
				quarterTileSize = value / 4;
				base.TileHeight = value;
			}
		}

		public int LayerHeight
		{
			get
			{
				return layerHeight;
			}
		}

		public Texture2D PickingAid
		{
			get
			{
				return pickingAid;
			}
			set
			{
				pickingAid = value;
			}
		}

		public Texture2D SlopeHeightMap
		{
			get
			{
				return slopeHeightMap;
			}
			set
			{
				slopeHeightMap = value;
			}
		}

		public int[,] SlopeMap
		{
			get
			{
				return slopeMap;
			}
			set
			{
				slopeMap = value;
			}
		}

		#endregion

		#region Overrides of Abstract Methods

		public override void LoadFromFile( ContentManager content, string fileName )
		{
			string rawInput;
			string[] processBuffer;
			int numRows = 0;
			int numCols = 0;
			char[] whiteSpace = new char[] { ' ', '\r', '\t', '\n' };

			// This will create the reader just for this scope
			// So it will get garbage collected as soon as the scope collapses
			using ( XmlReader reader = XmlReader.Create( fileName ) )
			{
				// Read in the map's name
				reader.ReadToFollowing( "Name" );
				Name = reader.ReadElementString( "Name" ).Trim();

				// Read in the map size (in tiles)
				rawInput = reader.ReadElementString( "Size" ).Trim();
				processBuffer = rawInput.Split( whiteSpace );
				numRows = int.Parse( processBuffer[0] );
				numCols = int.Parse( processBuffer[1] );

				// Read in the tile size (in pixels)
				rawInput = reader.ReadElementString( "TileSize" ).Trim();
				processBuffer = rawInput.Split( whiteSpace );
				TileHeight = int.Parse( processBuffer[0] );
				TileWidth = int.Parse( processBuffer[1] );

				// Read in and load the list of textures used by this map
				rawInput = reader.ReadElementString( "TileTextureList" ).Trim();
				processBuffer = rawInput.Split( whiteSpace, StringSplitOptions.RemoveEmptyEntries );
				LoadTileTextures( content, processBuffer );

				// Read in and load the helper textures
				reader.ReadToFollowing( "PickingHelper" );
				rawInput = reader.ReadElementString( "PickingHelper" ).Trim();
				pickingAid = content.Load<Texture2D>( rawInput );
				rawInput = reader.ReadElementString( "Cursor" ).Trim();
				Cursor.Texture = content.Load<Texture2D>( rawInput );
				rawInput = reader.ReadElementString( "SlopeHeightMap" ).Trim();
				slopeHeightMap = content.Load<Texture2D>( rawInput );

				// Read in the slope map
				reader.ReadToFollowing( "SlopeMap" );
				rawInput = reader.ReadElementString( "SlopeMap" ).Trim();
				processBuffer = rawInput.Split( whiteSpace, StringSplitOptions.RemoveEmptyEntries );
				slopeMap = new int[numRows, numCols];
				for ( int row = 0; row < numRows; row++ )
				{
					for ( int col = 0; col < numCols; col++ )
					{
						slopeMap[row, col] = int.Parse( processBuffer[row * numCols + col] );
					}
				}

				// Read in the layers
				int layerCount = 0;
				while ( reader.ReadToFollowing( "Layer" ) )
				{
					TileLayer newLayer = new TileLayer( numCols, numRows );

					// Read the tile texture indices
					reader.ReadToFollowing( "Textures" );
					rawInput = reader.ReadElementString( "Textures" ).Trim();
					processBuffer = rawInput.Split( whiteSpace, StringSplitOptions.RemoveEmptyEntries );
					for ( int row = 0; row < numRows; row++ )
					{
						for ( int col = 0; col < numCols; col++ )
						{
							newLayer.Tiles[row, col].TextureId = int.Parse( processBuffer[row * numCols + col] );
						}
					}

					AddLayer(newLayer);
					layerCount++;
				}
			}
		}

		public override Rectangle GetTileRect( int row, int col, int layerIndex )
		{
			// Adjust layerIndex to where it's actually going to be drawn
			layerIndex -= 1;
			if ( layerIndex < 0 )
			{
				layerIndex = 0;
			}

			int heightMod = layerIndex * HalfTileSize;

			return (
				new Rectangle(
					(int)Math.Ceiling( ( col * TileWidth * Scale ) +
									   ( HalfTileSize * Scale * ( row & 1 ) ) ),
					(int)Math.Ceiling( ( row * QuarterTileSize * Scale ) -
									   ( heightMod * Scale ) ),
					(int)Math.Ceiling( TileWidth * Scale ),
					(int)Math.Ceiling( TileHeight * Scale )
					)
				);
		}

		public override List<Tile> GetTilesIntersecting( Rectangle area )
		{
			// TODO: Implement this method
			throw new NotImplementedException();
		}

		public override void SetCameraToWorldBounds()
		{
			int screenWidth = Camera.Game.GraphicsDevice.Viewport.Width;
			int screenHeight = Camera.Game.GraphicsDevice.Viewport.Height;

			Rectangle worldRect = GetWorldRectangle();

			camera.SetBounds(
				new Point( worldRect.X, worldRect.Y ),
				new Point( worldRect.Width - screenWidth, worldRect.Height - screenHeight )
				);
		}

		/* Returns the (row,col) of the map cell at the given location.
		 * NOTE: The returned point's x value = column and y value = row */
		public override Point WorldLocationToTileRowCol( Vector2 position, out Vector2 localPixel )
		{

			if ( position.X < 0 || position.Y - HalfTileSize < 0 ||
				 position.X >= WidthInPixels || position.Y >= HeightInPixels )
			{
				// The given position is off the map!
				localPixel = new Vector2( -1, -1 );
				return new Point( -1, -1 );
			}
			
			// Figure out the local pixel inside the rough grid location corresponding to the position
			Point pixel = new Point( (int)( position.X % TileWidth ),
							         (int)( position.Y % HalfTileSize ) );
			

			// Create a rectangle to mask out the part of the picking aid texture we want
			Rectangle pixelArea = new Rectangle( pixel.X, pixel.Y, 1, 1 );

			// Retrieve the colour of the pixel on the pickingAid map.
			Color[] pixelColour = new Color[1];
			pickingAid.GetData<Color>( 0, pixelArea, pixelColour, 0, 1 );


			// Initialize mapCell to the location of the cell in a rough grid
			Point mapCell = new Point( (int)( position.X / TileWidth ),
									   ( (int)( position.Y / HalfTileSize ) - 1 ) * 2 );

			// Depending on the pixel's colour, offset mapCell's row/col according to
			// this table:
			//                        Colour | Row (Y) | Col (X)
			//                       --------+---------+---------
			//                        White  |    0    |    0
			//                        Red    |   -1    |   -1
			//                        Yellow |   -1    |    0
			//                        Green  |   +1    |   -1
			//                        Blue   |   +1    |    0
			//
			// Also offset the local pixel location to correspond to the local location
			// within the correct cell.

			if ( pixelColour[0] == Color.Red )			// "Red" is ( R:255 G:0 B:0 )
			{
				mapCell.Y -= 1;
				mapCell.X -= 1;
				pixel.Y += QuarterTileSize;
				pixel.X += HalfTileSize;
			}
			else if ( pixelColour[0] == Color.Yellow )	// "Yellow" is ( R:255 G:255 B:0 )
			{
				mapCell.Y -= 1;
				pixel.Y += QuarterTileSize;
				pixel.X -= HalfTileSize;
			}
			else if ( pixelColour[0] == Color.Lime )	// "Lime" is ( R:0 G:255 B:0 )
			{
				mapCell.Y += 1;
				mapCell.X -= 1;
				pixel.Y	-= QuarterTileSize;
				pixel.X += HalfTileSize;
			}
			else if ( pixelColour[0] == Color.Blue )	// "Blue" is ( R:0 G:0 B:255 )
			{
				mapCell.Y += 1;
				pixel.Y -= QuarterTileSize;
				pixel.X -= HalfTileSize;
			}

			// Final round of error checking
			if ( mapCell.X < 0 || mapCell.X >= NumCols ||
				 mapCell.Y < 0 || mapCell.Y >= NumRows )
			{
				localPixel = new Vector2( -1, -1 );
				return new Point( -1, -1 );
			}

			localPixel = new Vector2( pixel.X, pixel.Y );
			return mapCell;
		}

		/* Returns a rectangle that covers the playable world boundaries. */
		public override Rectangle GetWorldRectangle()
		{
			Rectangle worldRect = new Rectangle();
			worldRect.X = (int)Math.Ceiling( HalfTileSize * Scale );
			worldRect.Y = (int)Math.Ceiling( ( HalfTileSize + QuarterTileSize ) * Scale );
			worldRect.Width = (int)Math.Ceiling( WidthInPixels - ( HalfTileSize * Scale ) );
			worldRect.Height = (int)Math.Ceiling( HeightInPixels - ( QuarterTileSize * Scale ) );

			return worldRect;
		}

		#endregion

		#region IsoTileMap Methods

		/* Returns the height at a specific point in the world.
		 *    - If the point is not on the map, returns int.MinValue.
		 *    - If none of tiles at the corresponding world point have a 
		 *      texture defined, returns a height of -1 
		 */
		public int GetHeightAtWorldPosition( Vector2 position )
		{
			Point cell = WorldLocationToTileRowCol( position );
			int row = cell.Y;
			int col = cell.X;

			// TODO: When adding slopes in, this is where to branch off for slope tiles

			return GetHeightAtTile( row, col );
		}

		/* Returns the height of the tile at the given row and column.
		 *    - If the given [row,col] is non on the map, returns int.MinValue.
		 *    - If none of tiles at the given [row,col] have a texture defined, 
		 *      returns a height of -1 
		 */
		public int GetHeightAtTile( int row, int col )
		{
			// Check if the cell is valid
			if ( row < 0 || col < 0 || row >= NumRows || col >= NumCols )
			{
				return int.MinValue;
			}


			int height = -1;

			for ( int i = Layers.Count - 1; i >= 0; i-- )
			{
				if ( Layers[i].Tiles[row, col].TextureId != -1 )
				{
					height = i * LayerHeight;
					break;
				}
			}

			return height;
		}

		/* Calculates and returns the draw depth for an object drawn in the world. */
		public float GetDrawDepth( int row, int col, int height, int offset )
		{
			// TODO: Adjust this formula to get rid of the "rustling" problem
			float depth = ( ( height * NumRows * NumCols * HEIGHT_TERM_DAMPENER ) +
							( row * NumCols ) + col + offset ) * DEPTH_DAMPENER;

			return 1 - depth;
		}

		/* Calculates and returns the draw depth for an object drawn in the world. */
		public float GetDrawDepth( Vector2 position, int height )
		{
			Point tileLoc = WorldLocationToTileRowCol( position );

			return GetDrawDepth( tileLoc.Y, tileLoc.X, height, 10 );
		}

		#endregion
	}
}