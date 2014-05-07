using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AchievementEngine;
using System;

namespace AchievementTestGame
{
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;

		KeyboardState prevKbState;
		MouseState prevMouseState;

		int damageDealt = 0;

		public Game1()
		{
			graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
		}


		protected override void Initialize()
		{
			// Set up the achievements
			AchievementManager.Instance.AddAchievement( new Achi1() );
			AchievementManager.Instance.AddAchievement( new Achi2() );
			AchievementManager.Instance.AddAchievement( new Achi3() );
			AchievementManager.Instance.Initialize();

			prevKbState = Keyboard.GetState();
			prevMouseState = Mouse.GetState();

			base.Initialize();
		}


		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch( GraphicsDevice );
			font = Content.Load<SpriteFont>( "gamefont" );
		}


		protected override void Update( GameTime gameTime )
		{
			KeyboardState kbState = Keyboard.GetState();
			MouseState mouseState = Mouse.GetState();

			// Allows the game to exit
			if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || 
				 kbState.IsKeyDown( Keys.Escape )
				)
				this.Exit();

			
			if ( kbState.IsKeyDown( Keys.M ) && !prevKbState.IsKeyDown( Keys.M ) )
			{
				AchievementManager.Instance.RaiseEvent( "MKeyPressed", null );
			}

			if ( kbState.IsKeyDown( Keys.Space ) && !prevKbState.IsKeyDown( Keys.Space ) )
			{
				AchievementManager.Instance.RaiseEvent( "SpaceBarPressed", null );
			}

			if ( mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released )
			{
				DealDamage();
			}



			prevKbState = kbState;
			prevMouseState = mouseState;

			base.Update( gameTime );
		}


		protected override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.Black );

			spriteBatch.Begin();


			Vector2 drawPos = new Vector2( 20, 20 );

			foreach( Achievement achi in AchievementManager.Instance.Achievements )
			{
				spriteBatch.DrawString( font, "Name: " + achi.Name, drawPos, Color.HotPink );
				drawPos.Y += font.LineSpacing;
				spriteBatch.DrawString( font, "Desc: " + achi.Description, drawPos, Color.HotPink );
				drawPos.Y += font.LineSpacing;
				spriteBatch.DrawString( font, "Achieved: " + achi.Completed.ToString(), drawPos, Color.HotPink );
				drawPos.Y += font.LineSpacing;
				spriteBatch.DrawString( font, "--------------------------------------", drawPos, Color.HotPink );
				drawPos.Y += font.LineSpacing;
			}

			spriteBatch.DrawString( font, "T. Damage Dealt: " + damageDealt.ToString(), drawPos, Color.HotPink );


			spriteBatch.End();

			base.Draw( gameTime );
		}


		private void DealDamage()
		{
			Random rand = new Random();
			int damage = rand.Next( 50, 100 );
			damageDealt += damage;
			AchievementManager.Instance.RaiseEvent( "DamageDealt", damage );
		}
	}
}
