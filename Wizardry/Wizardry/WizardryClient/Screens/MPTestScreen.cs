using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using UIManager;
using UIManager.Controls;
using WizardryClient.GameUI;
using WizardryShared;

namespace WizardryClient
{
	class MPTestScreen : GameScreen, IPacketReceiver
	{
		#region UI Item Constants
		private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
		private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
		private readonly int SCOREBOARD_Y_OFFSET = 5;
		private readonly Color CENTER_MSG_COLOUR = new Color( 230, 165, 2 );
		private readonly Color CLOCK_COLOUR = new Color( 230, 165, 2 );
		private readonly string SCORE_SEPARATOR = " | ";
		#endregion

		#region Fields

		// Tracks the player's character
		Character playerChar;

		// Input action that controls the pause screen
		InputAction escapePressed;

		// The player's health/focus
		PlayerFrame playerFrame;

		// The player's action/CD bar
		ActionBar actionBar;

		// The other players' name plates
		NamePlate[] namePlates;

		// The radar
		Radar radar;

		// The font used to draw the scoreboard
		SpriteFont scoreBoardFont;

		// Flag for when the game is over
		bool gameOver = false;

		// Message to display center-screen
		string centerMsg = "";

		// Font for the center-screen message
		SpriteFont msgFont;

		// Input action for when the user presses 'u' (access upgrades screen)
		InputAction uPressed;

		#endregion

		#region Initialization & Deactivation

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public MPTestScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate( bool instancePreserved )
		{
			if ( !instancePreserved )
			{
				ContentManager content = ScreenManager.Game.Content;

				// The escape button handler to open the pause menu
				escapePressed = new InputAction( null, new Keys[] { Keys.Escape }, true );

				// The u button handler to open the upgrades screen
				uPressed = new InputAction( null, new Keys[] { Keys.U }, true );

				// Create world/camera
				Camera camera = new Camera( ScreenManager.Game );
				camera.Autonomous = false;
				GameManager.Instance.Camera = camera;
				IsoTileMap world = new IsoTileMap( camera );
				//world.Cursor.Active = true;
				//world.Cursor.Colour = Color.Blue;
				world.LoadFromFile( content, "Content/Maps/MPMap.map" );
				world.SetCameraToWorldBounds();
				GameManager.Instance.TileMap = world;

				// Set up the game UI
				playerChar = GameManager.Instance.GameState.Characters[GameManager.Instance.MyID];
				playerChar.IsPlayer = true;
				playerFrame = new PlayerFrame( playerChar );
				actionBar = new ActionBar( playerChar );
				radar = new Radar( playerChar );
				radar.Scale = 0.6f;
				radar.Alpha = 0.8f;
				radar.SnapToCorner( Radar.Corner.TopRight );
				
				// NamePlates
				namePlates = new NamePlate[GameManager.Instance.GameState.Characters.Length];
				for ( int i = 0; i < namePlates.Length; ++i )
				{
					namePlates[i] = new NamePlate( GameManager.Instance.GameState.Characters[i] );
				}

				// Load fonts
				scoreBoardFont = content.Load<SpriteFont>( "Fonts/ScoreBoardFont" );
				msgFont = content.Load<SpriteFont>( "Fonts/ScoreBoardFont" );
				
				// Start listening for packets from the server
				GameManager.Instance.RegisterForPackets( this );
			}
		}

		public override void Unload()
		{
			base.Unload();
			GameManager.Instance.UnregisterForPackets( this );
			GameManager.Instance.Client.Disconnect( "" );
		}

		#endregion

		#region HandleInput/Update/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			PlayerIndex playerIndex;
			if ( escapePressed.Evaluate( input, null, out playerIndex ) )
			{
				ShowQuitDialog();
			}
			if ( uPressed.Evaluate( input, null, out playerIndex ) )
			{
				ScreenManager.AddScreen( new UpgradeMenu( GameManager.Instance ), null );
			}
		}

		public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen )
		{
			// Clear the center message
			centerMsg = "";

			if ( ! gameOver )
			{
				if ( (!otherScreenHasFocus) && (playerChar.Status != Character.CharStatus.DEAD) )
				{
					// Update the Camera
					GameManager.Instance.Camera.Update( gameTime );

					// Update only my character
					playerChar.Update( gameTime.ElapsedGameTime );
				}

				if ( playerChar.Status == Character.CharStatus.DEAD )
				{
					centerMsg += "You have been killed!\n(Respawning Soon..)";
				}
			
				// Send character updates to the server for my character
				SendPlayerUpdate();
			}

			base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;
			GameManager.Instance.Camera.CenterOn( playerChar.WorldPosition );

			sb.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );
			
			GameManager.Instance.TileMap.Draw( sb );
			
			foreach ( Character c in GameManager.Instance.GameState.Characters )
			{
				if ( c.Status != Character.CharStatus.DEAD && c.Status != Character.CharStatus.OFFSCREEN )
				{
					c.Draw( gameTime, sb );
				}
			}

			foreach ( Spell s in GameManager.Instance.GameState.Spells )
			{
				s.Draw( gameTime, sb );
			}

			foreach ( PickUp p in GameManager.Instance.GameState.Pickups )
			{
				p.Draw( gameTime, sb );
			}

			sb.End();

			// TODO: Repair the playerframes so that this is not necessary
			sb.Begin();

			// Update and draw the player frame
			playerFrame.Update( gameTime );
			playerFrame.Draw( gameTime, sb );

			// Update and draw the player's actionbar
			actionBar.Update( gameTime );
			actionBar.Draw( gameTime, sb );

			// Update and draw the nameplates
			for ( int i = 0; i < namePlates.Length; ++i )
			{
				if ( GameManager.Instance.GameState.Characters[i].Active &&
					 i != GameManager.Instance.MyID &&
					 GameManager.Instance.GameState.Characters[i].Status != Character.CharStatus.DEAD &&
					 GameManager.Instance.GameState.Characters[i].Status != Character.CharStatus.OFFSCREEN )
				{
					namePlates[i].Update( gameTime );
					namePlates[i].Draw( gameTime, sb );
				}
			}

			// Draw the Radar
			radar.Draw( sb );

			// Draw the Scoreboard
			DrawScoreBoard( sb );

			// Draw the center message if one exists
			if ( !string.IsNullOrEmpty( centerMsg ) )
			{
				int screenHeight = ScreenManager.GraphicsDevice.Viewport.Height;
				int screenWidth = ScreenManager.GraphicsDevice.Viewport.Width;
				Vector2 textSize = msgFont.MeasureString( centerMsg );
				Vector2 texPos = new Vector2();
				texPos.X = ((screenWidth - textSize.X) / 2);
				texPos.Y = (screenHeight / 3) - (textSize.Y / 2);

				sb.DrawString( msgFont, centerMsg, texPos, CENTER_MSG_COLOUR );
			}

			sb.End();

			base.Draw( gameTime );
		}

		private void DrawScoreBoard( SpriteBatch sb )
		{
			int screenWidth = ScreenManager.GraphicsDevice.Viewport.Width;
			Vector2 drawPos = new Vector2();

			// Draw the Clock
			string timeStr = "Time Left: " + GameManager.Instance.GameState.RoundClock.ToString( @"m\:ss" );
			Vector2 timeStrSize = scoreBoardFont.MeasureString( timeStr );
			drawPos.X = (int)((screenWidth - timeStrSize.X) / 2);
			drawPos.Y = SCOREBOARD_Y_OFFSET;
			sb.DrawString( scoreBoardFont, timeStr, drawPos, CLOCK_COLOUR );

			// Draw the separator
			Vector2 sepSize = scoreBoardFont.MeasureString( SCORE_SEPARATOR );
			drawPos.X = (int)((screenWidth - sepSize.X) / 2);
			drawPos.Y = drawPos.Y + timeStrSize.Y - SCOREBOARD_Y_OFFSET;
			sb.DrawString( scoreBoardFont, SCORE_SEPARATOR, drawPos, CLOCK_COLOUR );

			// Draw the red team's score
			string redScore = GameManager.Instance.GameState.Scores[(int)Team.RED].ToString();
			Vector2 redScoreSize = scoreBoardFont.MeasureString( redScore );
			Vector2 redScorePos = drawPos;
			redScorePos.X -= redScoreSize.X;
			sb.DrawString( scoreBoardFont, redScore, redScorePos, GameSettings.TeamColour( Team.RED ) );

			// Draw the blue team's score
			string blueScore = GameManager.Instance.GameState.Scores[(int)Team.BLUE].ToString();
			drawPos.X += sepSize.X;
			sb.DrawString( scoreBoardFont, blueScore, drawPos, GameSettings.TeamColour( Team.BLUE ) );
		}

		private void EndGame( Nullable<Team> winner )
		{
			gameOver = true;
	
			// Set up the message to display to the user
			string endMsg = "";
			if ( winner == null )
			{
				endMsg += "Game Over!\nThe game ended in a tie.";
			}
			else
			{
				endMsg += (winner == playerChar.Team) ? "Victory!\n" : "Defeat :(\n";
				
				switch ( winner )
				{
					case Team.RED:
						endMsg += "The Red Team Wins!";
						break;
					case Team.BLUE:
						endMsg += "The Blue Team Wins!";
						break;
				}
			}

			// Popup the message
			PopUpScreen gameOverScreen = new PopUpScreen( endMsg, PopUpScreen.Style.OK );
			gameOverScreen.Accepted += Quit;
			ScreenManager.AddScreen( gameOverScreen, null );
		}

		private void Quit()
		{
			LoadingScreen.Load( ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen() );
		}

		#endregion

		#region Button/UI Callbacks

		private void ShowQuitDialog()
		{
			const string message = "Are you sure you want to go back to Main Menu?";
			PopUpScreen confirmExitMessageBox = new PopUpScreen( message, PopUpScreen.Style.YesNo );

			confirmExitMessageBox.Accepted += Quit;
			confirmExitMessageBox.Cancelled += () => { };

			ScreenManager.AddScreen( confirmExitMessageBox, null );
		}

		#endregion

		#region Packet Handling

		public void ReceivePacket( NetIncomingMessage msg )
		{
			MsgType type = (MsgType)msg.ReadByte();
			switch ( type )
			{
				case MsgType.GSUpdate:
					// Retain my position - unless overriden by the server
					Vector2 myPos = playerChar.WorldPosition;
					float myFocus = playerChar.CurrentFocus;
					GameManager.Instance.GameState.UpdateFromPacket( msg );
					if ( ! playerChar.PositionOverride )
					{
						playerChar.WorldPosition = myPos;
					}
					if ( ! playerChar.FocusOverride )
					{
						playerChar.CurrentFocus = myFocus;
					}
					break;
				case MsgType.InfoGameOver:
					// Game's over
					int winner = msg.ReadInt32();
					if ( winner == -1 )
					{
						EndGame( null );
					}
					else
					{
						EndGame( (Team)winner );
					}
					break;
			}
		}

		private void SendPlayerUpdate()
		{
			if ( playerChar.Active )
			{
				NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
				msg.Write( (byte)MsgType.CharUpdate );
				msg.Write( (byte)GameManager.Instance.MyID );
				msg.Write( playerChar.WorldPosition.X );
				msg.Write( playerChar.WorldPosition.Y );
				msg.Write( (int)playerChar.CurrentAnimValue );
				msg.Write( (int)playerChar.CurrentFocus );
				GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.ReliableOrdered );
			}
		}

		#endregion
	}
}
