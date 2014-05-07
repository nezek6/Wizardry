using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
	/// <summary>
	/// A concrete variant of a TileMap where the tiles are hexagonally shaped.
	/// </summary>
    public class HexTileMap : TileMap
	{
		#region Data Members

		private int tileRowStep = 0;

		private int tileColStep = 0;

		private int oddRowOffset = 0;

		#endregion

		#region Constructors

		public HexTileMap( Camera camera ):base( camera )
		{
			type = TileMapType.Hex;
		}

		#endregion

		#region Properties

		public override int WidthInPixels
		{
			get
			{
				return (int)Math.Ceiling( NumCols * TileColStep * Scale );
			}
		}

		public override int HeightInPixels
		{
			get
			{
				return (int)Math.Ceiling( NumRows * TileRowStep * Scale );
			}
		}

		public int TileRowStep
		{
			get
			{
				return tileRowStep;
			}
			set
			{
				tileRowStep = value;
			}
		}

		public int TileColStep
		{
			get
			{
				return tileColStep;
			}
			set
			{
				oddRowOffset = (value + 1)/2;
				tileColStep = value;
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

				// Read in the tile step (in pixels)
				rawInput = reader.ReadElementString( "TileStep" ).Trim();
				processBuffer = rawInput.Split( whiteSpace );
				TileRowStep = int.Parse( processBuffer[0] );
				TileColStep = int.Parse( processBuffer[1] );

				// Read in and load the list of textures used by this map
				rawInput = reader.ReadElementString( "TextureList" ).Trim();
				processBuffer = rawInput.Split( whiteSpace, StringSplitOptions.RemoveEmptyEntries );
				LoadTileTextures( content, processBuffer );

				// Read in the layers
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
							newLayer.SetTileTextureIndex(
								row,
								col,
								int.Parse( processBuffer[row * numCols + col] )
								);
						}
					}

					AddLayer(newLayer);
				}
			}
		}

		public override Rectangle GetTileRect( int row, int col, int layerIndex )
		{
			return (
				new Rectangle(
					(int)Math.Ceiling( ( col * TileColStep * Scale ) + ( oddRowOffset * Scale * ( row & 1 ) ) ),
					(int)Math.Ceiling( row * TileRowStep * Scale ),
					(int)Math.Ceiling( TileWidth * Scale ),
					(int)Math.Ceiling( TileHeight * Scale )
					)
				);
		}

		public override List<Tile> GetTilesIntersecting( Rectangle area )
		{
			throw new NotImplementedException();
		}

		public override void SetCameraToWorldBounds()
		{
			int screenWidth = Camera.Game.GraphicsDevice.Viewport.Width;
			int screenHeight = Camera.Game.GraphicsDevice.Viewport.Height;

			Camera.SetBounds(
				new Point( (int)Math.Ceiling( TileWidth * Scale / 2 ),
						   (int)Math.Ceiling( TileHeight * Scale / 2 ) ),
				new Point( WidthInPixels - screenWidth, HeightInPixels - screenHeight )
				);
		}

		/* Returns the (row,col) of the map cell at the given location.
		 * NOTE: The returned point's x value = column and y value = row */
		public override Point WorldLocationToTileRowCol( Vector2 position, out Vector2 localPixel )
		{
			// TODO: Implement this method
			throw new NotImplementedException();
		}

		/* Returns a rectangle that covers the playable world boundaries. */
		public override Rectangle GetWorldRectangle()
		{
			// TODO: Implement this method
			throw new NotImplementedException();
		}

		#endregion
	}
}