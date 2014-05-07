using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	/// <summary>
	/// Enum to specify the type of TileMap, i.e. the shape of the tiles it supports.
	/// </summary>
	public enum TileMapType
	{
		Square,
		Hex,
		Isometric
	}

	/// <summary>
	/// Abstract base class representing a game world made out of layers of tiles.
	/// </summary>
	public abstract class TileMap
	{
		#region Data Members

		/* Describes the map's tileset type. */
		protected TileMapType type;

		/* List of tile layers making up the map. */
		protected List<TileLayer> layers = new List<TileLayer>();

		/* List of the tile textures used by the map. */
		protected List<Texture2D> textures = new List<Texture2D>();

		/* The world camera which defines the boundaries of what is visible on screen. */
		protected Camera camera;

		/* Allows the user to scale the map and "zoom in"-ish. */
		protected float scale = 1.0f;

		/* The width of a single tile (in pixels). */
		protected int tileWidth = 0;

		/* The height of a single tile (in pixels). */
		protected int tileHeight = 0;

		/* The map's name. */
		protected string name = "";

		/* A cursor that can highlight the tile pointed to by the mouse. */
		protected MapCursor cursor;

		#endregion

		#region Constructors

		/// <summary>
		/// Standard TileMap Constructor.
		/// </summary>
		/// <param name="camera">The Camera object representing the player's view of the world.</param>
		public TileMap( Camera camera )
		{
			this.camera = camera;
			this.cursor = new MapCursor( this );
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of this TileMap.
		/// </summary>
		public TileMapType Type
		{
			get
			{
				return type;
			}
		}

		/// <summary>
		/// Gets the list of TileLayers making up the map.
		/// </summary>
		public List<TileLayer> Layers
		{
			get
			{
				return this.layers;
			}
		}

		/// <summary>
		/// Gets the number of columns of tiles in the map. This value is the maximum number of
		/// columns of tiles among the map's layers.
		/// </summary>
		public int NumCols
		{
			get
			{
				int width = -10;

				foreach ( TileLayer layer in layers )
				{
					width = (int)Math.Max( width, layer.NumCols );
				}

				return width;
			}
		}

		/// <summary>
		/// Gets the number of rows of tiles in the map. This value is the maximum number of
		/// rows of tiles among the map's layers.
		/// </summary>
		public int NumRows
		{
			get
			{
				int height = -10;

				foreach ( TileLayer layer in layers )
				{
					height = (int)Math.Max( height, layer.NumRows );
				}

				return height;
			}
		}

		/// <summary>
		/// Gets the width of the map in pixels.
		/// </summary>
		public abstract int WidthInPixels
		{
			get;
		}

		/// <summary>
		/// Gets the height of the map in pixels.
		/// </summary>
		public abstract int HeightInPixels
		{
			get;
		}

		/// <summary>
		/// Gets or sets the Camera object representing the player's view of the world.
		/// </summary>
		public Camera Camera
		{
			get
			{
				return camera;
			}
			set
			{
				camera = value;
			}
		}

		/// <summary>
		/// Gets or sets a value which scales the world's size.
		/// </summary>
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

		/// <summary>
		/// Gets the width of a single tile in pixels.
		/// </summary>
		public virtual int TileWidth
		{
			get
			{
				return tileWidth;
			}
			protected set
			{
				// Outsiders setting this value could break things, so block them
				tileWidth = value;
			}
		}

		/// <summary>
		/// Gets the height of a single tile in pixels.
		/// </summary>
		public virtual int TileHeight
		{
			get
			{
				return tileHeight;
			}
			protected set
			{
				// Outsiders setting this value could break things, so block them
				tileHeight = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the map.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		/// <summary>
		/// Gets the MapCursor object used by this TileMap.
		/// </summary>
		public MapCursor Cursor
		{
			get
			{
				return cursor;
			}
		}

		#endregion

		#region Old-Style Accessors, For Grizzled Programmers

		/// <summary>
		/// Gets the type of this TileMap.
		/// </summary>
		/// <returns>The type of this map.</returns>
		public TileMapType GetMapType()
		{
			return Type;
		}

		/// <summary>
		/// Gets the TileLayer from the list of layers with the specified index.
		/// </summary>
		/// <param name="layerIndex">The index of the layer to get.</param>
		/// <returns>The TileLayer with the specified index.</returns>
		public TileLayer GetLayer( int layerIndex )
		{
			return Layers[layerIndex];
		}

		/// <summary>
		/// Gets the maximum width of the map in columns.
		/// </summary>
		/// <returns>The maximum width of the map in number of columns.</returns>
		public int GetWidthInCols()
		{
			return NumCols;
		}

		/// <summary>
		/// Gets the maximum height of the map in rows.
		/// </summary>
		/// <returns>The maximum height of the map in number of rows.</returns>
		public int GetHeightInRows()
		{
			return NumRows;
		}

		/// <summary>
		/// Gets the width of the map in pixels.
		/// </summary>
		/// <returns>The width of the map in pixels.</returns>
		public int GetWidthInPixels()
		{
			return WidthInPixels;
		}

		/// <summary>
		/// Gets the height of the map in pixels.
		/// </summary>
		/// <returns>The height of the map in pixels.</returns>
		public int GetHeightInPixels()
		{
			return HeightInPixels;
		}

		/// <summary>
		/// Gets the Camera object representing the player's view of the world.
		/// </summary>
		/// <returns>The Camera object representing the player's view of the world.</returns>
		public Camera GetCamera()
		{
			return Camera;
		}

		/// <summary>
		/// Sets the Camera object representing the player's view of the world.
		/// </summary>
		/// <param name="camera">The Camera object to use to represent the player's view of the world.</param>
		public void SetCamera( Camera camera )
		{
			Camera = camera;
		}

		/// <summary>
		/// Gets the scale of the map.
		/// </summary>
		/// <returns>The scale of the map.</returns>
		public float GetScale()
		{
			return Scale;
		}

		/// <summary>
		/// Sets the scale of the map.
		/// </summary>
		/// <param name="scale">The value to set the map's scale to.</param>
		public void SetScale( float scale )
		{
			Scale = scale;
		}

		/// <summary>
		/// Gets the width of a single tile in pixels.
		/// </summary>
		/// <returns>The width of a single tile in pixels.</returns>
		public int GetTileWidth()
		{
			return TileWidth;
		}

		/// <summary>
		/// Gets the height of a single tile in pixels.
		/// </summary>
		/// <returns>The height of a single tile in pixels.</returns>
		public int GetTileHeight()
		{
			return TileHeight;
		}

		/// <summary>
		/// Sets the size of the tiles used by the map.
		/// </summary>
		/// <param name="tileWidth">The width of a tile.</param>
		/// <param name="tileHeight">The height of a tile.</param>
		protected void SetTileSize( int tileWidth, int tileHeight )
		{
			TileWidth = tileWidth;
			TileHeight = tileHeight; 
		}

		/// <summary>
		/// Sets the name of the map.
		/// </summary>
		/// <param name="name">The string to set the map's name to.</param>
		public void SetName( string name )
		{
			Name = name;
		}

		/// <summary>
		/// Gets the name of the map.
		/// </summary>
		/// <returns>The name of the map.</returns>
		public string GetName()
		{
			return Name;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a given layer on top the of tile layers present in the map.
		/// </summary>
		/// <param name="layer">The tile layer to add.</param>
		public void AddLayer( TileLayer layer )
		{
			layer.ParentMap = this;
			layer.LayerIndex = layers.Count;
			layers.Add( layer );
		}

		/// <summary>
		/// Removes the layer at the given index from the map.
		/// </summary>
		/// <param name="layerIndex">The index of the layer to remove.</param>
		public void RemoveLayer( int layerIndex )
		{
			if ( layerIndex > 0 && layerIndex < layers.Count )
			{
				layers.RemoveAt( layerIndex );

				// Readjust layer index data for layers above the one just deleted
				for ( int i = layerIndex; i < Layers.Count; i++ )
				{
					Layers[i].LayerIndex = i;
				}
			}
		}

		/// <summary>
		/// Clears all layer and texture data.
		/// </summary>
		public void Clear()
		{
			layers.Clear();
			textures.Clear();
		}

		/// <summary>
		/// Loads a new tile texture from the specified file(s).
		/// </summary>
		/// <param name="content">The content manager object to use for loading.</param>
		/// <param name="textureNames">One or more textures to load.</param>
		public void LoadTileTextures( ContentManager content, params string[] textureNames )
		{
			Texture2D texture;
			foreach ( string tex in textureNames )
			{
				texture = content.Load<Texture2D>( tex );
				textures.Add( texture );
			}
		}

		/// <summary>
		/// Adds a preloaded texture to the list of textures used by the map. The index
		/// assigned to the texture will be next available index.
		/// </summary>
		/// <param name="texture">The tile texture to add to the list.</param>
		public void AddTileTexture( Texture2D texture )
		{
			textures.Add( texture );
		}

		/// <summary>
		/// Looks up the index of a given texture.
		/// </summary>
		/// <param name="texture">The texture whose index to look up.</param>
		/// <returns>The index of the texture or -1 if the texture isn't loaded.</returns>
		public int IndexOfTexture( Texture2D texture )
		{
			if ( textures.Contains( texture ) )
			{
				return textures.IndexOf( texture );
			}
			return -1;
		}

		/// <summary>
		/// Retrives the texture at the given index.
		/// </summary>
		/// <param name="textureIndex">The index of the texture to retrieve.</param>
		/// <returns>The texture at that index or null if no such texture exists.</returns>
		public Texture2D GetTextureWithIndex( int textureIndex )
		{
			if ( textureIndex >= textures.Count || textureIndex < 0 )
			{
				return null;
			}

			return textures[textureIndex];
		}

		/// <summary>
		/// Helper method that returns a rectangle indicating the area in the world which a given tile occupies.
		/// </summary>
		/// <param name="row">The row coordinate of the tile.</param>
		/// <param name="col">The column coordinate of the tile.</param>
		/// <returns>The rectangular area in the world which the tile occupies.</returns>
		public Rectangle GetTileRect( int row, int col )
		{
			return GetTileRect( row, col, 0 );
		}

		/// <summary>
		/// Draws the world map to the screen using the given SpriteBatch. The SpriteBatch must
		/// be open!
		/// </summary>
		/// <param name="spriteBatch">The open SpriteBatch to use for drawing.</param>
		public void Draw( SpriteBatch spriteBatch )
		{
			foreach ( TileLayer layer in layers )
			{
				layer.Draw( spriteBatch );
			}

			cursor.Draw( spriteBatch );
		}

		/// <summary>
		/// Returns the [row,col] of the map cell at the given location.
		/// </summary>
		/// <param name="position">The position to convert to [row,col] coordinates.</param>
		/// <returns>A Point where the x value = column and y value = row.</returns>
		public Point WorldLocationToTileRowCol( Vector2 position )
		{
			Vector2 oblivion;

			return WorldLocationToTileRowCol( position, out oblivion );
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Populates map data from the given map file.
		/// </summary>
		/// <param name="content">Content manager object to use for loading data.</param>
		/// <param name="fileName">Path to the map file to use.</param>
		public abstract void LoadFromFile( ContentManager content, string fileName );

		/// <summary>
		/// Helper method which returns a rectangle indicating the area in the world which the tile
		/// at the given [row,col] coordinates and layer occupies.
		/// </summary>
		/// <param name="row">The row coordinate of the tile.</param>
		/// <param name="col">The column coordinate of the tile.</param>
		/// <param name="layerIndex">The index of the layer on which the tile lives.</param>
		/// <returns></returns>
		public abstract Rectangle GetTileRect( int row, int col, int layerIndex );

		/// <summary>
		/// Gets a list of all tiles intersecting the given rectangle.
		/// </summary>
		/// <param name="area">The area to check for tiles.</param>
		/// <returns>A list of all the tiles inside the rectangle's area.</returns>
		public abstract List<Tile> GetTilesIntersecting( Rectangle area );

		/// <summary>
		/// Sets the camera boundaries to be able to pan around the entire world without going over.
		/// </summary>
		public abstract void SetCameraToWorldBounds();

		/// <summary>
		/// Returns the [row,col] of the map cell at the given location.
		/// </summary>
		/// <param name="position">The position to convert to [row,col] coordinates.</param>
		/// <param name="localPixel">Will contain the local pixel coordinate of the given position within the tile.</param>
		/// <returns>A Point where the x value = column and y value = row.</returns>
		public abstract Point WorldLocationToTileRowCol( Vector2 position, out Vector2 localPixel );

		/// <summary>
		/// Gets a rectangle that covers the playable world boundaries.
		/// </summary>
		/// <returns>A rectangle that covers the playable area of the world.</returns>
		public abstract Rectangle GetWorldRectangle();

		#endregion
	}
}