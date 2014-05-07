using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WizardryShared;

namespace WizardryClient.GameUI
{
	class Radar
	{
		#region Constants
		
		private const float INNER_RAD = 90;
		
		private const float RADAR_SIZE = 200;
		
		private readonly Color PLAYER_COLOUR = Color.LightGreen;

		#endregion

		#region Enums and Structs

		public enum Corner
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		#endregion

		#region Data Members

		private Vector2 position;

		private float scale = 1f;

		private float alpha = 1f;

		private Character boundChar;

		private Texture2D radarTex;

		#endregion

		#region Properties

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public float Scale
		{
			get { return scale; }
			set { scale = value; }
		}

		public float Alpha
		{
			get { return alpha; }
			set 
			{
				alpha = MathHelper.Clamp( value, 0, 1 );
			}
		}

		public Character BoundCharacter
		{
			get { return boundChar; }
		}

		public float Width
		{
			get { return RADAR_SIZE * scale; }
		}

		public float Height
		{
			get { return RADAR_SIZE * scale; }
		}

		#endregion

		#region Methods

		public Radar( Character boundCharacter )
		{
			this.boundChar = boundCharacter;
			radarTex = GameManager.Instance.Game.Content.Load<Texture2D>( "UITextures/Radar" );
		}

		public void SnapToCorner( Corner corner )
		{
			int screenWidth = GameManager.Instance.Game.GraphicsDevice.Viewport.Width;
			int screenHeight = GameManager.Instance.Game.GraphicsDevice.Viewport.Height;

			Vector2 newPos = Vector2.Zero;
			switch ( corner )
			{
				case Corner.TopLeft:
					// Do nothing - Keep at (0,0)
					break;
				case Corner.TopRight:
					newPos.X = screenWidth - Width;
					break;
				case Corner.BottomLeft:
					newPos.Y = screenHeight - Height;
					break;
				case Corner.BottomRight:
					newPos.X = screenWidth - Width;
					newPos.Y = screenHeight - Height; 
					break;
			}
			Position = newPos;
		}

		public void Draw( SpriteBatch sb )
		{
			// Draw the radar texture
			Rectangle drawRect = new Rectangle( 
				(int)position.X, 
				(int)position.Y, 
				(int)(RADAR_SIZE * scale), 
				(int)(RADAR_SIZE * scale) );

			sb.Draw( radarTex, drawRect, Color.White * Alpha );

			// Figure out the center of the radar
			Vector2 radCenter = Position;
			radCenter.X += ((RADAR_SIZE * scale) / 2);
			radCenter.Y += ((RADAR_SIZE * scale) / 2);

			// Draw the other players' dots
			int screenWidth = GameManager.Instance.Game.GraphicsDevice.Viewport.Width;
			int screenHeight = GameManager.Instance.Game.GraphicsDevice.Viewport.Height;
			float axisLen = (new Vector2( screenWidth / 2, screenHeight / 2 )).Length();

			foreach ( Character c in GameManager.Instance.GameState.Characters )
			{
				// Don't consider inactive players or the bound player
				if ( !c.Active || c == boundChar || c.Status == Character.CharStatus.DEAD )
				{
					continue;
				}

				Vector2 dir = c.WorldPosition - boundChar.WorldPosition;
				float dotDist = MathHelper.Clamp( (dir.Length() / axisLen) * (INNER_RAD * Scale), 0, INNER_RAD * Scale );
				dir.Normalize();
				dir *= dotDist;
				DrawingHelpers.DrawPoint(
					sb,
					radCenter + dir,
					GameSettings.TeamColour( c.Team ),
					3 );
			}


			// Draw the player's dot
			DrawingHelpers.DrawPoint( sb, radCenter, PLAYER_COLOUR, 3 );
		}

		#endregion
	}
}
