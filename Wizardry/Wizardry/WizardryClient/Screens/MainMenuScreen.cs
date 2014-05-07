using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using UIManager;
using UIManager.Controls;
using WizardryShared;

namespace WizardryClient
{
	class MainMenuScreen : GameScreen
	{
		#region UI Item Constants
		//private readonly Rectangle BANNER_RECT = new Rectangle( 182, 25, 436, 175 );
		private readonly Rectangle BANNER_RECT = new Rectangle( 111, 25, 578, 175 );
		private readonly Point SP_BUTTON_POS = new Point( 305, 223 );
		private readonly Point MP_BUTTON_POS = new Point( 305, 292 );
		private readonly Point OPT_BUTTON_POS = new Point( 305, 361 );
		private readonly Point ACHI_BUTTON_POS = new Point( 305, 430 );
		private readonly Point EXIT_BUTTON_POS = new Point( 330, 530 );
		private readonly int STD_BUTTON_WIDTH = 190;
		private readonly int STD_BUTTON_HEIGHT = 46;
		private readonly int EXIT_BUTTON_WIDTH = 140;
		private readonly int EXIT_BUTTON_HEIGHT = 46;
		private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
		private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
		private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
        private readonly Vector2 ALTAR_POS_1 = new Vector2(100, 361);
        private readonly Vector2 ALTAR_POS_2 = new Vector2(700, 361);
		#endregion

		#region Fields

		// The Wizardry Scroll Logo
		private Texture2D banner;

		// Button to go to Single Player
		private Button spButton;

		// Button to go to Multiplayer
		private Button mpButton;

		// Button to go to the Options menu
		private Button optionsButton;

		// Button to go to the Achievements page
		private Button achiButton;

		// Button to exit the game
		private Button exitButton;

        // Altars to draw
        private Animation altar1;
        private Animation altar2;

		// TODO: Delme (Demo Code)
		Song demoBGMusic;

		#endregion

		#region Initialization

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public MainMenuScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate( bool instancePreserved )
		{
			if ( !instancePreserved )
			{
				ContentManager content = ScreenManager.Game.Content;

				banner = content.Load<Texture2D>( "Images/wizlogo3" );

				// Set up the Buttons
				Texture2D buttonTex = content.Load<Texture2D>( "UITextures/GUIButton" );
				SpriteFont buttonFont = content.Load<SpriteFont>( "Fonts/GUIFont" );

				// Single Player Button
				spButton = new Button( SP_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				spButton.Texture = buttonTex;
				spButton.SetFont( buttonFont, 
					BTNTEXT_INERT_COLOUR, 
					BTNTEXT_HOVER_COLOUR, 
					BTNTEXT_PRIMED_COLOUR, 
					BTNTEXT_INACTIVE_COLOUR );
				spButton.Text = "Single Player";
				spButton.RegisterCallback( spButton_OnClick );

				// Multiplayer Button
				mpButton = new Button( MP_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				mpButton.Texture = buttonTex;
				mpButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				mpButton.Text = "Multiplayer";
				mpButton.RegisterCallback( mpButton_OnClick );

				// Options Button
				optionsButton = new Button( OPT_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				optionsButton.Texture = buttonTex;
				optionsButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				optionsButton.Text = "Options";
				optionsButton.RegisterCallback( optionsButton_OnClick );

				// Achievements Button
				achiButton = new Button( ACHI_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
				achiButton.Texture = buttonTex;
				achiButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				achiButton.Text = "Achievements";
				achiButton.Active = false;
				achiButton.RegisterCallback( achiButton_OnClick );

				// Exit Button
				exitButton = new Button( EXIT_BUTTON_POS, EXIT_BUTTON_WIDTH, EXIT_BUTTON_HEIGHT, this );
				exitButton.Texture = buttonTex;
				exitButton.SetFont( buttonFont,
					BTNTEXT_INERT_COLOUR,
					BTNTEXT_HOVER_COLOUR,
					BTNTEXT_PRIMED_COLOUR,
					BTNTEXT_INACTIVE_COLOUR );
				exitButton.Text = "Exit";
				exitButton.RegisterCallback( exitButton_OnClick );

                altar1 = new Animation(new AnimationTexture(ScreenManager.Game.Content.Load<Texture2D>("Images/altar"), null), 3, 60, 1f, false);
                altar2 = new Animation(new AnimationTexture(ScreenManager.Game.Content.Load<Texture2D>("Images/altar"), null), 3, 60, 1f, false);

				// TODO: Delme (Demo Code)
				demoBGMusic = ScreenManager.Game.Content.Load<Song>( "Audio/Demo_FightingWinds" );
				MediaPlayer.IsRepeating = true;
				MediaPlayer.Play( demoBGMusic );
				MediaPlayer.Volume = 0.4f;
			}
		}

		public override void Unload()
		{
			MediaPlayer.Stop();
			base.Deactivate();
			base.Unload();
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			spButton.HandleInputs( gameTime, input );
			mpButton.HandleInputs( gameTime, input );
			optionsButton.HandleInputs( gameTime, input );
			achiButton.HandleInputs( gameTime, input );
			exitButton.HandleInputs( gameTime, input );
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;

			sb.Begin();
			// Draw the Wizardry Banner
			sb.Draw( banner, BANNER_RECT, Color.White * TransitionAlpha );
            altar1.Update(gameTime);
            altar2.Update(gameTime);
            altar1.Draw(sb, ALTAR_POS_1, 0f, TransitionAlpha );
            altar2.Draw(sb, ALTAR_POS_2, 0f, TransitionAlpha );
			sb.End();

			// Let the Controls draw themselves
			spButton.Draw( gameTime, TransitionAlpha );
			mpButton.Draw( gameTime, TransitionAlpha );
			optionsButton.Draw( gameTime, TransitionAlpha );
			achiButton.Draw( gameTime, TransitionAlpha );
			exitButton.Draw( gameTime, TransitionAlpha );

			base.Draw( gameTime );
		}

		#endregion

		#region Button Callbacks

		public void spButton_OnClick( Control sender )
		{
			LoadingScreen.Load( ScreenManager, true, null, new TestGameScreen() );
		}

		public void mpButton_OnClick( Control sender )
		{
			ScreenManager.AddScreen( new LobbySelectionScreen(), null );
		}

		public void optionsButton_OnClick( Control sender )
		{
			ScreenManager.AddScreen( new OptionsMenuScreen(), null );
		}

		public void achiButton_OnClick( Control sender )
		{
            ScreenManager.AddScreen(new RoleSelectScreen(), null);
		}

		public void exitButton_OnClick( Control sender )
		{
			ScreenManager.Game.Exit();
		}

		#endregion
	}
}
