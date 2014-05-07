using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using UIManager;
using UIManager.Controls;

namespace WizardryClient
{
	class OptionsMenuScreen : GameScreen
	{
		#region UI Item Constants
		private readonly Point BACK_BUTTON_POS = new Point( 15, 539 );
		private readonly Vector2 MUSIC_SEL_LABEL_POS = new Vector2( 255, 200 );
		private readonly Point MUSIC_SEL_POS = new Point( 405, 195 );
		private readonly int MUSIC_SEL_HEIGHT = 30;
		private readonly int MUSIC_SEL_WIDTH = 100;
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

		// Music On/Off Selector
		private Selector musicSel;

		#endregion

		#region Initialization

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public OptionsMenuScreen()
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

				// Music Selector
				musicSel = new Selector( MUSIC_SEL_POS, MUSIC_SEL_WIDTH, MUSIC_SEL_HEIGHT, this );
				musicSel.ArrowTexture = content.Load<Texture2D>( "UITextures/ArrowLeft" );
				musicSel.Font = labelFont;
				musicSel.AddElement( "On" );
				musicSel.AddElement( "Off" );
				musicSel.TextColour = Color.White;
				if ( MediaPlayer.IsMuted )
				{
					musicSel.SelectedIndex = 1;
				}
				musicSel.RegisterCallback( musicSel_OnSelectionChanged );
			}
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			backButton.HandleInputs( gameTime, input );
			musicSel.HandleInputs( gameTime, input );
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
			string title = "Options";
			Vector2 titleSize = titleFont.MeasureString( title );
			int textX = (int)( ( ScreenManager.GraphicsDevice.Viewport.Width - titleSize.X ) / 2 );
			sb.DrawString( titleFont, title, new Vector2( textX, 20 ), TITLE_COLOUR * TransitionAlpha );
			

			// Draw the Music label
			sb.DrawString( labelFont, "Music:", MUSIC_SEL_LABEL_POS, TITLE_COLOUR * TransitionAlpha );
			
			sb.End();

			// Let the Controls draw themselves
			backButton.Draw( gameTime, TransitionAlpha );
			musicSel.Draw( gameTime, TransitionAlpha );

			base.Draw( gameTime );
		}

		#endregion

		#region Button Callbacks

		public void backButton_OnClick( Control sender )
		{
			ExitScreen();
		}

		public void musicSel_OnSelectionChanged( Control sender )
		{
			MediaPlayer.IsMuted = ( (sender as Selector).SelectedIndex != 0 );
		}

		#endregion
	}
}
