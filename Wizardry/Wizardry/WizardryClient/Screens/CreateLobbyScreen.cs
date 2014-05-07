using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using UIManager;
using UIManager.Controls;
using Lidgren.Network;
using WizardryShared;

namespace WizardryClient
{
	class CreateLobbyScreen : GameScreen
	{
		#region UI Item Constants
		private readonly Point BACK_BUTTON_POS = new Point( 15, 539 );
		private readonly Point CREATE_BUTTON_POS = new Point( 305, 450 );
		private readonly Vector2 MATCH_DUR_SEL_LABEL_POS = new Vector2( 155, 200 );
		private readonly Point MATCH_DUR_SEL_POS = new Point( 465, 195 );
		private readonly Vector2 NUMPLAYERS_SEL_LABEL_POS = new Vector2( 155, 280 );
		private readonly Point NUMPLAYERS_SEL_POS = new Point( 465, 275 );
		private readonly int SEL_HEIGHT = 30;
		private readonly int SEL_WIDTH = 100;
		private readonly int STD_BUTTON_WIDTH = 190;
		private readonly int STD_BUTTON_HEIGHT = 46;
		private readonly int BACK_BUTTON_WIDTH = 140;
		private readonly int BACK_BUTTON_HEIGHT = 46;
		private readonly Color TITLE_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
		private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
		#endregion

		#region Fields

		// Button to return to the Main Menu
		private Button backButton;

		// Font for the title
		private SpriteFont titleFont;

		// Font for item labels
		private SpriteFont labelFont;

		// Match Duration Selector
		private Selector matchDurSel;

		// Number of Players Selector
		private Selector numPlayersSel;

		// Create Button 
		private Button createButton;

		#endregion

		#region Initialization

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public CreateLobbyScreen()
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
				labelFont = content.Load<SpriteFont>( "Fonts/GUIFontBold" );

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

				// Create Button
				createButton = new Button( CREATE_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				createButton.Texture = buttonTex;
				createButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				createButton.Text = "Create Lobby";
				createButton.RegisterCallback( createButton_OnClick );

				// Match Duration Selector
				matchDurSel = new Selector( MATCH_DUR_SEL_POS, SEL_WIDTH, SEL_HEIGHT, this );
				matchDurSel.ArrowTexture = content.Load<Texture2D>( "UITextures/ArrowLeft" );
				matchDurSel.Font = labelFont;
				matchDurSel.TextColour = Color.White;
				matchDurSel.AddElement( "5" );
				matchDurSel.AddElement( "10" );
				matchDurSel.AddElement( "15" );
				matchDurSel.AddElement( "20" );
				matchDurSel.AddElement( "30" );
				matchDurSel.SelectedIndex = 2;

				// NumPlayers Selector
				numPlayersSel = new Selector( NUMPLAYERS_SEL_POS, SEL_WIDTH, SEL_HEIGHT, this );
				numPlayersSel.ArrowTexture = content.Load<Texture2D>( "UITextures/ArrowLeft" );
				numPlayersSel.Font = labelFont;
				numPlayersSel.TextColour = Color.White;
				numPlayersSel.AddElement( "1" );
				numPlayersSel.AddElement( "2" );
				numPlayersSel.AddElement( "4" );
				numPlayersSel.AddElement( "6" );
				numPlayersSel.AddElement( "8" );
				numPlayersSel.SelectedIndex = 2;
				

			}
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			backButton.HandleInputs( gameTime, input );
			matchDurSel.HandleInputs( gameTime, input );
			numPlayersSel.HandleInputs( gameTime, input );
			createButton.HandleInputs( gameTime, input );
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;

			sb.Begin();

			// Draw a shaded rectangle
			Rectangle r = new Rectangle( 75, 75, 
				ScreenManager.GraphicsDevice.Viewport.Width - 150, 
				ScreenManager.GraphicsDevice.Viewport.Height - 150 );
			DrawingHelpers.DrawFilledRectangle( sb, r, Color.Black * 0.5f * TransitionAlpha );

			// Draw the page title
			string title = "Create New Lobby";
			Vector2 titleSize = titleFont.MeasureString( title );
			int textX = (int)( ( ScreenManager.GraphicsDevice.Viewport.Width - titleSize.X ) / 2 );
			sb.DrawString( titleFont, title, new Vector2( textX, 20 ), TITLE_COLOUR * TransitionAlpha );
			

			// Draw the Match Duration label
			sb.DrawString( labelFont, "Match Duration (Mins):", MATCH_DUR_SEL_LABEL_POS, TITLE_COLOUR * TransitionAlpha );
			
			// Draw the Number of Players label
			sb.DrawString( labelFont, "Number of Players:", NUMPLAYERS_SEL_LABEL_POS, TITLE_COLOUR * TransitionAlpha );

			sb.End();

			// Let the Controls draw themselves
			backButton.Draw( gameTime, TransitionAlpha );
			matchDurSel.Draw( gameTime, TransitionAlpha );
			numPlayersSel.Draw( gameTime, TransitionAlpha );
			createButton.Draw( gameTime, TransitionAlpha );

			base.Draw( gameTime );
		}

		#endregion

		#region Button Callbacks

		public void backButton_OnClick( Control sender )
		{
			ExitScreen();
		}

		public void createButton_OnClick( Control sender )
		{
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqCreateLobby );

			string name = RandomName.Generate(); // TODO: Replace random names
			msg.Write( name + "'s Lobby" );
			msg.Write( int.Parse( numPlayersSel.SelectedItem ) );
			msg.Write( double.Parse( matchDurSel.SelectedItem ) );
			msg.Write( name );	
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );

			ExitScreen();
		}

		#endregion
	}
}
