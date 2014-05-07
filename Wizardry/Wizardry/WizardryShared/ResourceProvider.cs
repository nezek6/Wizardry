using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace WizardryShared
{
	public interface ResourceProvider
	{
		#region Properties

		IsoTileMap TileMap
		{
			get;
			set;
		}

		Game Game
		{
			get;
			set;
		}

		Camera Camera
		{
			get;
			set;
		}

		SpriteBatch SpriteBatch
		{
			get;
			set;
		}

		GameState GameState
		{
			get;
			set;
		}

		TextureProvider TextureProvider
		{
			get;
		}

		#endregion

		#region Interface Methods

		void Shoot( int spellID, Vector2 startPoint, Vector2 clickPosition, Vector2 direction, float rotation, Character caster );

		bool CheckCollision( Sprite sprite1, Sprite sprite2, bool pixelPerfect );

		#endregion
	}
}
