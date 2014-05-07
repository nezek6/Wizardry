#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UIManager;
using UIManager.Controls;
using WizardryClient;
#endregion

namespace WizardryClient
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class PopUpScreen : GameScreen
    {
        #region Fields

        private string message;
		private Style style;

        private Texture2D backgroundTexture;

        private Button accept;
        private Button cancel;

        #endregion

        #region Events
        public delegate void EventHandler();

		public enum Style
		{
			YesNo,
			OK
		}

        public event EventHandler Accepted;
        public event EventHandler Cancelled;


        Viewport viewport;
        Vector2 viewportSize;
        Vector2 textSize;
        Vector2 textPosition;
        SpriteFont font;

        private readonly int STD_BUTTON_WIDTH = 190;
        private readonly int STD_BUTTON_HEIGHT = 46;
        private readonly Point ACCEPT_BUTTON_POS = new Point( 200, 361 );
        private readonly Point CANCEL_BUTTON_POS = new Point( 400, 361 );
		private readonly Point OK_BUTTON_POS = new Point( 305, 361 );

        private readonly Color BTNTEXT_INERT_COLOUR = new Color( 230, 165, 2 );
        private readonly Color BTNTEXT_HOVER_COLOUR = new Color( 230, 165, 2 );
        private readonly Color BTNTEXT_PRIMED_COLOUR = new Color( 255, 0, 0 );
        private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color( 155, 155, 155 );
        private readonly int HPAD = 32;
        private readonly int VPAD = 16;

        Rectangle backgroundRectangle;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public PopUpScreen( string message, Style style )
        {
            this.message = message;
			this.style = style;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;


                viewport = ScreenManager.GraphicsDevice.Viewport;
                viewportSize = new Vector2(viewport.Width, viewport.Height);
                font = content.Load<SpriteFont>("Fonts/GUIFont");
                textSize = font.MeasureString(message);
                textPosition = (viewportSize - textSize) / 2;

                backgroundRectangle = new Rectangle((int)textPosition.X - HPAD,
													(int)textPosition.Y - VPAD,
													(int)textSize.X + HPAD * 2,
													(int)textSize.Y + VPAD * 2);
				backgroundTexture = content.Load<Texture2D>("UITextures/popbg");


				if ( style == Style.YesNo )
				{
					accept = new Button(ACCEPT_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
					cancel = new Button(CANCEL_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
					accept.Text = "Yes";
					cancel.Text = "No";

					accept.Texture = content.Load<Texture2D>("UITextures/GUIButton");
					cancel.Texture = content.Load<Texture2D>("UITextures/GUIButton");
					accept.SetFont(font,
						BTNTEXT_INERT_COLOUR,
						BTNTEXT_HOVER_COLOUR,
						BTNTEXT_PRIMED_COLOUR,
						BTNTEXT_INACTIVE_COLOUR);
					cancel.SetFont(font,
						BTNTEXT_INERT_COLOUR,
						BTNTEXT_HOVER_COLOUR,
						BTNTEXT_PRIMED_COLOUR,
						BTNTEXT_INACTIVE_COLOUR);

					accept.RegisterCallback(acceptButton_OnClick);
					cancel.RegisterCallback(cancelButton_OnClick);
				}
				else if ( style == Style.OK )
				{
					accept = new Button( OK_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this );
					accept.Texture = content.Load<Texture2D>( "UITextures/GUIButton" );
					accept.SetFont( font,
						BTNTEXT_INERT_COLOUR,
						BTNTEXT_HOVER_COLOUR,
						BTNTEXT_PRIMED_COLOUR,
						BTNTEXT_INACTIVE_COLOUR );
					accept.RegisterCallback( acceptButton_OnClick );
					accept.Text = "OK";
				}

            }
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
			if ( style == Style.YesNo )
			{
				accept.HandleInputs(gameTime, input);
				cancel.HandleInputs(gameTime, input);
			}
			else if ( style == Style.OK )
			{
				accept.HandleInputs( gameTime, input );
			}
            
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();

			if ( style == Style.YesNo )
			{
				accept.Draw(gameTime, TransitionAlpha);
				cancel.Draw(gameTime, TransitionAlpha);
			}
			else if ( style == Style.OK )
			{
				accept.Draw( gameTime, TransitionAlpha );
			}

            base.Draw(gameTime);

        }

		#endregion


        #region Button Callbacks

        public void acceptButton_OnClick(Control sender)
        {
            if (Accepted != null)
            {
                Accepted();
            }
			ExitScreen();
        }

        public void cancelButton_OnClick(Control Sender)
        {
            if (Cancelled != null)
            {
                Cancelled();
            }
			ExitScreen();
        }

        #endregion
    }
}
