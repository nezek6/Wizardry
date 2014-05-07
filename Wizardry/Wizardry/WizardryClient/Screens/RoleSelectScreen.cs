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
	class RoleSelectScreen : GameScreen
	{
		#region UI Item Constants
        private readonly Rectangle PRIEST_IMAGE_RECT = new Rectangle(70, 185, 140, 140);
        private readonly Rectangle FIRE_IMAGE_RECT = new Rectangle(240, 185, 140, 140);
        private readonly Rectangle BM_IMAGE_RECT = new Rectangle(410, 185, 140, 140);
        private readonly Rectangle WIND_IMAGE_RECT = new Rectangle(580, 185, 140, 140);
		private readonly Point BACK_BUTTON_POS = new Point( 15, 539 );
        private readonly Point PRIEST_BUTTON_POS = new Point(70, 335 );
        private readonly Point FIRE_BUTTON_POS = new Point(240, 335 );
        private readonly Point BM_BUTTON_POS = new Point(410, 335 );
        private readonly Point WIND_BUTTON_POS = new Point(580, 335 );
		private readonly int STD_BUTTON_WIDTH = 140;
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

        // Button to select the Priest Role
        private Button priestButton;

        // Button to select the Fire Wizard Role
        private Button fireButton;

        // Button to select the Frost Wizard Role
        private Button bmButton;

        // Button to select the WindLasher Role
        private Button windButton;

        // The Priest Selection Image
        private Texture2D priestImage;

        // The Fire Wizard Selection Image
        private Texture2D fireWizardImage;

        // The Frost Wizard Selection Image
        private Texture2D beastMistressImage;

        // The WindLasher Selection Image
        private Texture2D windLasherImage;

		// Font for the title
		private SpriteFont titleFont;

		#endregion

		#region Initialization

		/// <summary>
		/// MainMenuScreen Constructor
		/// </summary>
		public RoleSelectScreen()
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

                priestImage = content.Load<Texture2D>("Images/priestselect");
                fireWizardImage = content.Load<Texture2D>("Images/wizselect");
                beastMistressImage = content.Load<Texture2D>("Images/bmselect");
                windLasherImage = content.Load<Texture2D>("Images/windselect");
				
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

                // Priest Selection Button
                priestButton = new Button(PRIEST_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                priestButton.Texture = buttonTex;
                priestButton.SetFont(buttonFont,
                    BTNTEXT_INERT_COLOUR,
                    BTNTEXT_HOVER_COLOUR,
                    BTNTEXT_PRIMED_COLOUR,
                    BTNTEXT_INACTIVE_COLOUR);
                priestButton.Text = "Priest";
                priestButton.RegisterCallback(priestButton_OnClick);

                // Fire Wizard Selection Button
                fireButton = new Button(FIRE_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                fireButton.Texture = buttonTex;
                fireButton.SetFont(buttonFont,
                    BTNTEXT_INERT_COLOUR,
                    BTNTEXT_HOVER_COLOUR,
                    BTNTEXT_PRIMED_COLOUR,
                    BTNTEXT_INACTIVE_COLOUR);
                fireButton.Text = "Fire Wizard";
                fireButton.RegisterCallback(fireButton_OnClick);

                // Beast Mistress Selection Button
                bmButton = new Button(BM_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                bmButton.Texture = buttonTex;
                bmButton.SetFont(buttonFont,
                    BTNTEXT_INERT_COLOUR,
                    BTNTEXT_HOVER_COLOUR,
                    BTNTEXT_PRIMED_COLOUR,
                    BTNTEXT_INACTIVE_COLOUR);
                bmButton.Text = "Beast Mistress";
                bmButton.RegisterCallback(bmButton_OnClick);

                // WindLasher Selection Button
                windButton = new Button(WIND_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                windButton.Texture = buttonTex;
                windButton.SetFont(buttonFont,
                    BTNTEXT_INERT_COLOUR,
                    BTNTEXT_HOVER_COLOUR,
                    BTNTEXT_PRIMED_COLOUR,
                    BTNTEXT_INACTIVE_COLOUR);
                windButton.Text = "WindLasher";
                windButton.RegisterCallback(windButton_OnClick);
			}
		}

		#endregion

		#region HandleInput/Draw Overrides

		public override void HandleInput( GameTime gameTime, InputState input )
		{
			// Let the Controls handle their input
			backButton.HandleInputs( gameTime, input );
            priestButton.HandleInputs(gameTime, input);
            fireButton.HandleInputs(gameTime, input);
            bmButton.HandleInputs(gameTime, input);
            windButton.HandleInputs(gameTime, input);
		}

		public override void Draw( GameTime gameTime )
		{
			SpriteBatch sb = ScreenManager.SpriteBatch;

            sb.Begin();
            // Draw the Selection Images
            sb.Draw(priestImage, PRIEST_IMAGE_RECT, Color.White * TransitionAlpha);
            sb.Draw(fireWizardImage, FIRE_IMAGE_RECT, Color.White * TransitionAlpha);
            sb.Draw(beastMistressImage, BM_IMAGE_RECT, Color.White * TransitionAlpha);
            sb.Draw(windLasherImage, WIND_IMAGE_RECT, Color.White * TransitionAlpha);


			// Draw the page title
			string title = "Choose Your Role";
			Vector2 titleSize = titleFont.MeasureString( title );
			int textX = (int)((ScreenManager.GraphicsDevice.Viewport.Width - titleSize.X)/2);
			sb.DrawString( titleFont, title, new Vector2( textX, 20 ), TITLE_COLOUR * TransitionAlpha );
            sb.End();

			// Let the Controls draw themselves
			backButton.Draw( gameTime, TransitionAlpha );
            priestButton.Draw(gameTime, TransitionAlpha);
            fireButton.Draw(gameTime, TransitionAlpha);
            bmButton.Draw(gameTime, TransitionAlpha);
            windButton.Draw(gameTime, TransitionAlpha);

			base.Draw( gameTime );
		}

		#endregion

		#region Button Callbacks

		public void backButton_OnClick( Control sender )
		{
			ExitScreen();
		}

        public void priestButton_OnClick(Control sender)
        {
			ChangeRole( 1 );
            ExitScreen();
        }

        public void fireButton_OnClick(Control sender)
        {
			ChangeRole( 0 );
            ExitScreen();
        }

        public void bmButton_OnClick(Control sender)
        {
			ChangeRole( 3 );
            ExitScreen();
        }

        public void windButton_OnClick(Control sender)
        {
			ChangeRole( 2 );
            ExitScreen();
        }

		private void ChangeRole( int roleID )
		{
			if ( GameManager.Instance.InMultiplayer )
			{
				NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
				msg.Write( (byte)MsgType.ReqClassChange );
				msg.Write( roleID );
				GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.Unreliable );
			}
			else
			{
				// TODO : Implement role changing for singleplayer
			}
		}

		#endregion
	}
}
