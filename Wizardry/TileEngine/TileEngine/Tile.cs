namespace TileEngine
{
	/// <summary>
	/// A simple data structure representing a single Tile.
	/// </summary>
	public class Tile
	{
		#region Common Data

		/// <summary>
		/// Gets or sets the Tile's TextureID. A value of -1 indicates no texture.
		/// </summary>
		public int TextureId { set; get; }

		/// <summary>
		/// Gets or sets whether the Tile can be traversed without collision.
		/// </summary>
		public bool Walkable { set; get; }

		#endregion

		#region Square Tileset Specific Data

		/// <summary>
		/// Describes an alpha transition effect for the tile to blend terrain styles together.
		/// This is specific to Square Tilesets only.
		/// 
		/// Not yet implemented.
		/// </summary>
		public short TransitionStyle { set; get; }  // Not implemented yet.

		#endregion

		/// <summary>
		/// Default Tile constructor. Creates a tile with TextureID -1, no collision and no transition style.
		/// </summary>
		public Tile()
		{
			TextureId = -1;			// No texture
			Walkable = true;		// No collision
			TransitionStyle = -1;	// No Transition
		}
	}
}
