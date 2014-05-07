using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UIManager;
using UIManager.Controls;
using WizardryShared;

namespace WizardryClient
{
	class LobbyScreen : GameScreen, IPacketReceiver
	{
		#region UI Item Constants
		private readonly Point BACK_BUTTON_POS = new Point( 15, 539 );
		private readonly Point READY_BUTTON_POS = new Point( 305, 539 );
		private readonly Point RED_LIST_POS = new Point( 50, 130 );
		private readonly Point BLUE_LIST_POS = new Point( 450, 130 );
		private readonly Point TEAM_BUTTON_POS = new Point( 200, 455 );
		private readonly Point ROLE_BUTTON_POS = new Point( 410, 455 );
		private readonly int LIST_WIDTH = 300;
		private readonly int LIST_HEIGHT = 300;
		private readonly int STD_BUTTON_WIDTH = 190;
		private readonly int STD_BUTTON_HEIGHT = 46;
		private readonly int BACK_BUTTON_WIDTH = 140;
		private readonly int BACK_BUTTON_HEIGHT = 46;
		private readonly Color TITLE_COLOUR = new Color( 230, 165, 2 );
		private readonly Color TEAM_LIST_TEXT_COLOUR = Color.White;
		private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
		private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
		#endregion

		#region Fields

		// Button to return to the Main Menu
		private Button backButton;

		// Ready button
		private Button readyButton;

		// Switch Team button
		private Button teamButton;

		// Change Role button
		private Button roleButton;

		// Flag indicating whether we're entering a game
		private bool enteringGame = false;

		// Font for the page title
		private SpriteFont titleFont;

		// Font for list headings
		private SpriteFont listHeadingFont;

		// List Views for the teams
		private ListView redTeamList;
		private ListView blueTeamList;

		#endregion

		#region Initialization

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public LobbyScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate( bool instancePreserved )
		{
			if ( !instancePreserved )
			{
				ContentManager content = ScreenManager.Game.Content;

				titleFont = content.Load<SpriteFont>( "Fonts/PageTitle" );
				listHeadingFont = content.Load<SpriteFont>( "Fonts/ListHeading" );

				// Set up the Buttons
				Texture2D buttonTex = content.Load<Texture2D>( "UITextures/GUIButton" );
				SpriteFont buttonFont = content.Load<SpriteFont>( "Fonts/GUIFont" );

				// Ready Button
				readyButton = new Button( READY_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				readyButton.Texture = buttonTex;
				readyButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				readyButton.Text = "Ready!";
				readyButton.RegisterCallback( readyButton_OnClick );

				// Back Button
				backButton = new Button( BACK_BUTTON_POS, BACK_BUTTON_WIDTH, BACK_BUTTON_HEIGHT, this );
				backButton.Texture = buttonTex;
				backButton.SetFont( buttonFont, 
					BTNTEXT_INERT_COLOUR, 
					BTNTEXT_HOVER_COLOUR, 
					BTNTEXT_PRIMED_COLOUR, 
					BTNTEXT_INACTIVE_COLOUR );
				backButton.Text = "Back";
				backButton.RegisterCallback( backButton_OnClick );

				// Switch Team Button
				teamButton = new Button( TEAM_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				teamButton.Texture = buttonTex;
				teamButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				teamButton.Text = "Change Team";
				teamButton.RegisterCallback( teamButton_OnClick );

				// Switch Role Button
				roleButton = new Button( ROLE_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				roleButton.Texture = buttonTex;
				roleButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				roleButton.Text = "Change Role";
				roleButton.RegisterCallback( roleButton_OnClick );

				// Set up the team lists
				Texture2D listItemTex = content.Load<Texture2D>( "UITextures/ListViewItemTex" );
				SpriteFont listItemFont = content.Load<SpriteFont>( "Fonts/GUIFontBold" );

				// Red team list
				redTeamList = new ListView( 8, RED_LIST_POS, LIST_WIDTH, LIST_HEIGHT, this );
				redTeamList.Texture = listItemTex;
				redTeamList.Font = listItemFont;
				redTeamList.ItemTextColour = TEAM_LIST_TEXT_COLOUR;

				// Blue team list
				blueTeamList = new ListView( 8, BLUE_LIST_POS, LIST_WIDTH, LIST_HEIGHT, this );
				blueTeamList.Texture = listItemTex;
				blueTeamList.Font = listItemFont;
				blueTeamList.ItemTextColour = TEAM_LIST_TEXT_COLOUR;

				GameManager.Instance.RegisterForPackets( this );
			}
		}

		public override void Unload()
		{
			base.Unload();
			GameManager.Instance.UnregisterForPackets( this );
			
			if ( ! enteringGame )
			{
				// Signal that we're leaving the lobby
				NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
				msg.Write( (byte)MsgType.ReqLeaveLobby );
				GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
			}
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			backButton.HandleInputs( gameTime, input );
			teamButton.HandleInputs( gameTime, input );
			roleButton.HandleInputs( gameTime, input );
			readyButton.HandleInputs( gameTime, input );
			//redTeamList.HandleInputs( gameTime, input );
			//blueTeamList.HandleInputs( gameTime, input );
		}

		public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen )
		{
			base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );
			PopulateTeamLists();
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;

			sb.Begin();

			// Draw the page title
			string title = GameManager.Instance.MPLobbyName;
			Vector2 size = titleFont.MeasureString( title );
			int textX = (int)((ScreenManager.GraphicsDevice.Viewport.Width - size.X) / 2);
			int textY = 20;
			sb.DrawString( titleFont, title, new Vector2( textX, textY ), TITLE_COLOUR * TransitionAlpha );

			// Draw the headings for the lists
			string redTeam = "Red Team";
			size = listHeadingFont.MeasureString( redTeam );
			textX = (int)((LIST_WIDTH - size.X) / 2);
			textY = (int)(RED_LIST_POS.Y - size.Y);
			sb.DrawString( listHeadingFont, redTeam, new Vector2( RED_LIST_POS.X + textX, textY ), 
				GameSettings.TeamColour( Team.RED ) * TransitionAlpha );

			string blueTeam = "Blue Team";
			size = listHeadingFont.MeasureString( blueTeam );
			textX = (int)((LIST_WIDTH - size.X) / 2);
			textY = (int)(BLUE_LIST_POS.Y - size.Y);
			sb.DrawString( listHeadingFont, blueTeam, new Vector2( BLUE_LIST_POS.X + textX, textY ),
				GameSettings.TeamColour( Team.BLUE ) * TransitionAlpha );

			sb.End();

			// Let the Controls draw themselves
			backButton.Draw( gameTime, TransitionAlpha );
			teamButton.Draw( gameTime, TransitionAlpha );
			roleButton.Draw( gameTime, TransitionAlpha );
			readyButton.Draw( gameTime, TransitionAlpha );
			redTeamList.Draw( gameTime, TransitionAlpha );
			blueTeamList.Draw( gameTime, TransitionAlpha );
			

			base.Draw( gameTime );
		}

		private void PopulateTeamLists()
		{
			redTeamList.Clear();
			blueTeamList.Clear();
			foreach ( Character c in GameManager.Instance.GameState.Characters )
			{
				if ( !c.Active )
				{
					continue;
				}

				string toAdd = c.Name;
				switch ( c.RoleID )
				{
					case 0:
						toAdd += " (Wiz)";
						break;
					case 1:
						toAdd += " (Prst)";
						break;
					case 2:
						toAdd += " (Wnd)";
						break;
					case 3:
						toAdd += " (BM)";
						break;
				}
				if ( c.Team == Team.RED )
				{
					redTeamList.AddElement( toAdd );
				}
				else if ( c.Team == Team.BLUE )
				{
					blueTeamList.AddElement( toAdd );
				}


			}
		}

		#endregion

		#region Button Callbacks

		public void readyButton_OnClick( Control sender )
		{
			// Signal the server of our readiness status
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqReady );
			msg.Write( (bool)true );
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
			
			readyButton.Active = false;
		}

		public void teamButton_OnClick( Control sender )
		{
			Team current = GameManager.Instance.GameState.Characters[GameManager.Instance.MyID].Team;
			Team switchTo;
			if ( current == Team.RED )
			{
				switchTo = Team.BLUE;
			}
			else
			{
				switchTo = Team.RED;
			}

			// Signal the server of our readiness status
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqTeamChange );
			msg.Write( (int)switchTo );
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
		}

		public void roleButton_OnClick( Control sender )
		{
			ScreenManager.AddScreen( new RoleSelectScreen(), null );
		}

		public void backButton_OnClick( Control sender )
		{
			ExitScreen();
		}

		#endregion

		#region IPacketReceiver Members

		public void ReceivePacket( NetIncomingMessage msg )
		{
			MsgType type = (MsgType)msg.ReadByte();
			switch ( type )
			{
				case MsgType.InfoGameStart:
					enteringGame = true;
					LoadingScreen.Load( ScreenManager, true, null, new MPTestScreen() );
					break;
				case MsgType.GSUpdate:
					GameManager.Instance.GameState.UpdateFromPacket( msg );
					break;
			}
		}

		#endregion
	}
}
