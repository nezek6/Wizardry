﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
	public class SquareTileMap : TileMap
	{
		#region Constructors

		public SquareTileMap( Camera camera ):base( camera )
		{
			type = TileMapType.Square;
		}

		#endregion

		#region Properties

		public override int WidthInPixels
		{
			get
			{
				return (int)( NumCols * TileWidth * Scale );
			}
		}

		public override int HeightInPixels
		{
			get
			{
				return (int)( NumRows * TileHeight * Scale );
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

					AddLayer( newLayer );
				}
			}
		}

		public override Rectangle GetTileRect( int row, int col, int layerIndex )
		{
			return (
				new Rectangle(
					(int)Math.Ceiling( col * TileWidth * Scale ),
					(int)Math.Ceiling( row * TileHeight * Scale ),
					(int)Math.Ceiling( TileWidth * Scale ),
					(int)Math.Ceiling( TileHeight * Scale )
					)
				);
		}

		public override List<Tile> GetTilesIntersecting( Rectangle area )
		{
			// Figure out the index of the leftmost tile contained in the area
			int leftIndex = area.Left * NumCols / WidthInPixels;
			if ( leftIndex < 0 )
			{
				leftIndex = 0;
			}

			// Figure out the index of the rightmost tile contained in the area
			int rightIndex = area.Right * NumCols / WidthInPixels;
			if ( rightIndex >= NumCols )
			{
				rightIndex = NumCols - 1;
			}

			// Figure out the index of the topmost tile contained in the area
			int topIndex = area.Top * NumRows / HeightInPixels;
			if ( topIndex < 0 )
			{
				topIndex = 0;
			}

			// Figure out the index of the bottom tile contained in the area
			int bottomIndex = area.Bottom * NumRows / HeightInPixels;
			if ( bottomIndex >= NumRows )
			{
				bottomIndex = NumRows - 1;
			}


			// Build a list with the coordinates of all the tiles contained in the area
			List<Tile> tiles = new List<Tile>();

			foreach ( TileLayer layer in Layers )
			{
				for ( int i = leftIndex; i <= rightIndex; i++ )
				{
					for ( int j = topIndex; j <= bottomIndex; j++ )
					{
						tiles.Add( layer.Tiles[i,j] );
					}
				}
			}

			return tiles;
		}

		public override void SetCameraToWorldBounds()
		{
			int screenWidth = Camera.Game.GraphicsDevice.Viewport.Width;
			int screenHeight = Camera.Game.GraphicsDevice.Viewport.Height;

			camera.SetBounds(
				new Point( 0, 0 ),
				new Point( WidthInPixels - screenWidth, HeightInPixels - screenHeight )
				);
		}

		/* Returns the (row,col) of the map cell at the given location.
		 * NOTE: The returned point's x value = column and y value = row */
		public override Point WorldLocationToTileRowCol( Vector2 position, out Vector2 localPixel )
		{
			// TODO: Implement this method
			throw new NotImplementedException(); ;
		}

		/* Returns a rectangle that covers the playable world boundaries. */
		public override Rectangle GetWorldRectangle()
		{
			// TODO: Implement this method
			return new Rectangle( 0, 0, WidthInPixels, HeightInPixels );
		}

		#endregion
	}
}