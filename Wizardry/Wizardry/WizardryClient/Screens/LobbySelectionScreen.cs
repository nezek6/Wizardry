using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UIManager;
using UIManager.Controls;
using WizardryShared;
using System.Collections.Generic;

namespace WizardryClient
{
	class LobbySelectionScreen : GameScreen, IPacketReceiver
	{
		#region UI Item Constants
		private readonly Rectangle BANNER_RECT = new Rectangle( 182, 25, 436, 175 );
		private readonly Point BACK_BUTTON_POS = new Point( 15, 539 );
		private readonly Point JOIN_LOBBY_BUTTON_POS = new Point( 305, 445 );
		private readonly Point REFRESH_BUTTON_POS = new Point( 400, 500 );
		private readonly Point NEW_LOBBY_BUTTON_POS = new Point( 200, 500 );
		private readonly Point LOBBY_LIST_POS = new Point( 250, 90 );
		private readonly int STD_BUTTON_WIDTH = 190;
		private readonly int STD_BUTTON_HEIGHT = 46;
		private readonly int BACK_BUTTON_WIDTH = 140;
		private readonly int BACK_BUTTON_HEIGHT = 46;
		private readonly Color TITLE_COLOUR = new Color( 230, 165, 2 );
		private readonly Color LOBBY_LIST_ITEM_COLOUR = Color.White;
		private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
		private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
		private readonly int LOBBY_LIST_WIDTH = 300;
		private readonly int LOBBY_LIST_HEIGHT = 350;
		#endregion

		#region Internal Data Structures
		private struct LobbyData
		{
			public int id;
			public string name;
			public int maxPlayers;
			public int numPlayers;
		}
		#endregion

		#region Fields

		// Button to return to the Main Menu
		private Button backButton;

		// Button to refresh the lobbies list
		private Button refreshButton;

		// Button to Join the lobby
		private Button joinButton;

		// Button to create a new lobby
		private Button newLobbyButton;

		// Lobby listview
		private List<LobbyData> lobbyData;
		private ListView lobbies;

		// Title font
		private SpriteFont titleFont;

		#endregion

		#region Initialization

		public LobbySelectionScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate( bool instancePreserved )
		{
			if ( !instancePreserved )
			{
				ContentManager content = ScreenManager.Game.Content;

				// Set up the title
				titleFont = content.Load<SpriteFont>( "Fonts/PageTitle" );

				// Set up the Buttons
				Texture2D buttonTex = content.Load<Texture2D>( "UITextures/GUIButton" );
				SpriteFont buttonFont = content.Load<SpriteFont>( "Fonts/GUIFont" );

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

				// Refresh Lobbies List button
				refreshButton = new Button( REFRESH_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				refreshButton.Texture = buttonTex;
				refreshButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				refreshButton.Text = "Refresh List";
				refreshButton.RegisterCallback( refreshButton_OnClick );

				// Join Selected Lobby Button
				joinButton = new Button( JOIN_LOBBY_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				joinButton.Texture = buttonTex;
				joinButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				joinButton.Text = "Join Selected Lobby";
				joinButton.RegisterCallback( joinButton_OnClick );
				joinButton.Active = false;

				// New Lobby Button
				newLobbyButton = new Button( NEW_LOBBY_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				newLobbyButton.Texture = buttonTex;
				newLobbyButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				newLobbyButton.Text = "Create New Lobby";
				newLobbyButton.RegisterCallback( newButton_OnClick );

				// Set up lobby list
				lobbyData = new List<LobbyData>();
				lobbies = new ListView( 8, LOBBY_LIST_POS, LOBBY_LIST_WIDTH, LOBBY_LIST_HEIGHT, this );
				lobbies.Font = content.Load<SpriteFont>( "Fonts/GUIFontBold" ); ;
				lobbies.Texture = content.Load<Texture2D>( "UITextures/ListViewItemTex" );
				lobbies.ArrowTexture = content.Load<Texture2D>( "UITextures/ArrowUp" );
				lobbies.ItemTextColour = LOBBY_LIST_ITEM_COLOUR;

				// Open the network connection
				GameManager.Instance.Client.Start();
				if ( string.IsNullOrEmpty( GameSettings.SERVER_URL ) )
				{
					//GameManager.Instance.Client.DiscoverLocalPeers( GameSettings.SERVER_PORT_NUM );
					string s = System.IO.File.ReadAllText( "Content/host.txt" );
					GameManager.Instance.Client.Connect( s, GameSettings.SERVER_PORT_NUM );
					//Console.WriteLine( s );
				}
				else
				{
					GameManager.Instance.Client.Connect( GameSettings.SERVER_URL, GameSettings.SERVER_PORT_NUM );
				}
				GameManager.Instance.RegisterForPackets( this );
			}
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			backButton.HandleInputs( gameTime, input );
			refreshButton.HandleInputs( gameTime, input );
			joinButton.HandleInputs( gameTime, input );
			lobbies.HandleInputs( gameTime, input );
			newLobbyButton.HandleInputs( gameTime, input );
		}

		public override void Update( GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen )
		{
			base.Update( gameTime, otherScreenHasFocus, coveredByOtherScreen );
			refreshButton.Active = (GameManager.Instance.Client.ConnectionStatus == NetConnectionStatus.Connected);
			newLobbyButton.Active = refreshButton.Active;
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;

			// Draw the page title
			string title = "Select Lobby";
			Vector2 titleSize = titleFont.MeasureString( title );
			int textX = (int)( ( ScreenManager.GraphicsDevice.Viewport.Width - titleSize.X ) / 2 );
			sb.Begin();
			sb.DrawString( titleFont, title, new Vector2( textX, 20 ), TITLE_COLOUR * TransitionAlpha );
			sb.End();

			// Let the Controls draw themselves
			backButton.Draw( gameTime, TransitionAlpha );
			refreshButton.Draw( gameTime, TransitionAlpha );
			joinButton.Draw( gameTime, TransitionAlpha );
			lobbies.Draw( gameTime, TransitionAlpha );
			newLobbyButton.Draw( gameTime, TransitionAlpha );

			base.Draw( gameTime );
		}

		public override void Deactivate()
		{
			GameManager.Instance.UnregisterForPackets( this );
			base.Deactivate();
		}

		#endregion

		#region Button Callbacks

		private void backButton_OnClick( Control sender )
		{
			GameManager.Instance.UnregisterForPackets( this );
			GameManager.Instance.Client.Disconnect( "" );
			ExitScreen();
		}

		private void joinButton_OnClick( Control sender )
		{
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqJoinLobby );
			msg.Write( (byte)lobbyData[lobbies.SelectedIndex].id );
			msg.Write( RandomName.Generate() );	// TODO : Replace random names
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
		}

		private void newButton_OnClick( Control sender )
		{
			ScreenManager.AddScreen( new CreateLobbyScreen(), null );
		}

		private void refreshButton_OnClick( Control sender )
		{
			RequestLobbiesList();
		}

		#endregion

		#region Packet Handling

		public void ReceivePacket( NetIncomingMessage msg )
		{
			MsgType type = (MsgType)msg.ReadByte();
			switch ( type )
			{
				case MsgType.InfoLobbyList:
					lobbies.Clear();
					lobbyData.Clear();
					int numLobbies = msg.ReadByte();
					for ( int i = 0; i < numLobbies; ++i )
					{
						LobbyData newLobby = new LobbyData();
						newLobby.id = msg.ReadByte();
						newLobby.name = msg.ReadString();
						newLobby.maxPlayers = msg.ReadByte();
						newLobby.numPlayers = msg.ReadByte();
						lobbyData.Add( newLobby );
						lobbies.AddElement( newLobby.name + "  (" + newLobby.numPlayers + "/" + newLobby.maxPlayers + ")" );
						Console.WriteLine( "Lobby: " + newLobby );
					}
					joinButton.Active = true;
					break;
				case MsgType.InfoJoinedLobby:
					GameManager.Instance.InMultiplayer = true;
					GameManager.Instance.MyID = msg.ReadByte();
					GameManager.Instance.MPLobbyName = msg.ReadString();
					GameManager.Instance.GameState = new GameState( GameManager.Instance );
					GameManager.Instance.GameState.UpdateFromPacket( msg );
					ScreenManager.AddScreen( new LobbyScreen(), null );
					break;
				case MsgType.Error:
					HandleError( (MsgErrorCode)msg.ReadByte() );
					break;
			}
		}

		private void RequestLobbiesList()
		{
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqLobbyList );
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
		}

		private void HandleError( MsgErrorCode err )
		{
			string msg = "";
			switch ( err )
			{
				case MsgErrorCode.CannotCreateLobby:
					msg += "Can't create any more new lobbies yet.";
					break;
				case MsgErrorCode.LobbyFull:
					msg += "That lobby is full.";
					break;
				case MsgErrorCode.InvalidLobby:
					msg += "That lobby does not exist anymore.";
					break;
			}

			PopUpScreen popup = new PopUpScreen( msg, PopUpScreen.Style.OK );
			ScreenManager.AddScreen( popup, null );
		}

		#endregion
	}
}
