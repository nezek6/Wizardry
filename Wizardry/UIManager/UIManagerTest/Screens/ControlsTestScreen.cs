using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UIManager;
using UIManager.Controls;

namespace UIManagerTest
{
    class ControlsTestScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

		float pauseAlpha;
		InputAction pauseAction;

		Button testBtn1;
		Button testBtn2;
        ListView testlistView;
        Selector testSelector;

		Color bgColour = Color.CornflowerBlue;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsTestScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

			pauseAction = new InputAction(
			   new Buttons[] { Buttons.Start, Buttons.Back },
			   new Keys[] { Keys.Escape },
			   true );
        }


        /// <summary>
        /// Screen initialization.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(200);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();


				// Load Controls
				
				Texture2D buttonTex = content.Load<Texture2D>( "ControlStuff/TestBtn" );
				SpriteFont buttonFont = content.Load<SpriteFont>( "ControlStuff/btnText" );

				testBtn1 = new Button( new Point( 50, 50 ), 100, 100, this );
				testBtn1.Texture = buttonTex;
				testBtn1.SetFont( buttonFont, Color.Black, Color.Red, Color.Yellow, Color.LightGray );
				testBtn1.Text = "TestBtn1";
				testBtn1.RegisterCallback( testBtn1_OnClick );

				testBtn2 = new Button( new Point( 200, 50 ), 150, 75, this );
				testBtn2.Texture = buttonTex;
				testBtn2.Text = "Button 2";
				testBtn2.SetFont( buttonFont, Color.Black, Color.Red, Color.Yellow, Color.LightGray );
				testBtn2.RegisterCallback( testBtn2_OnClick );

                testlistView = new ListView(4, new Point(400, 75), 150, 200, this);
                testlistView.Texture = buttonTex;
                testlistView.Font = buttonFont;
                testlistView.ArrowTexture = buttonTex;
                testlistView.ArrowFont = buttonFont;
                testlistView.AddElement("A");
                testlistView.AddElement("B");
                testlistView.AddElement("C");
                testlistView.AddElement("D");
                testlistView.AddElement("E");
                testlistView.AddElement("F");
                testlistView.AddElement("G");
                testlistView.AddElement("H");
                testlistView.AddElement("A");
                testlistView.RemoveElement("A");
                testlistView.RemoveElement("G");
                testlistView.Clear();
                testlistView.AddElement("A");
                testlistView.AddElement("B");
                testlistView.AddElement("C");
                testlistView.AddElement("D");
                testlistView.AddElement("E");
                testlistView.AddElement("F");
                testlistView.AddElement("G");
                testlistView.AddElement("H");
                testlistView.AddElement("A");

                testSelector = new Selector(new Point(50, 300), 150, 200, this);
                testSelector.Font = buttonFont;
                testSelector.ArrowTexture = buttonTex;
                testSelector.ArrowFont = buttonFont;
                testSelector.AddElement("A");
                testSelector.AddElement("B");
                testSelector.AddElement("C");
                testSelector.AddElement("D");
                testSelector.AddElement("E");
                testSelector.AddElement("F");
                testSelector.AddElement("G");
                testSelector.AddElement("H");
                testSelector.AddElement("A");
                testSelector.RemoveElement("A");
                testSelector.RemoveElement("G");
                testSelector.Clear();
                testSelector.AddElement("A");
                testSelector.AddElement("B");
                testSelector.AddElement("C");
                testSelector.AddElement("D");
                testSelector.AddElement("E");
                testSelector.AddElement("F");
                testSelector.AddElement("G");
                testSelector.AddElement("H");
                testSelector.AddElement("Asds");

            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
				
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;
            PlayerIndex player;

            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
				// Game Paused
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
				// Tell the buttons to handle their inputs 
				testBtn1.HandleInputs( gameTime, input );
				testBtn2.HandleInputs( gameTime, input );
                testlistView.HandleInputs(gameTime, input);
                testSelector.HandleInputs(gameTime, input);
			}
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               bgColour, 0, 0);


			// Tell the controls to draw themselves
			testBtn1.Draw( gameTime, 1f );
			testBtn2.Draw( gameTime, 0.5f );
            testlistView.Draw(gameTime, 1f);
            testSelector.Draw(gameTime, 1f);
			

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

		// Callback for the first button, toggles the background colour
		public void testBtn1_OnClick( Control sender )
		{
			if ( bgColour == Color.CornflowerBlue )
			{
				bgColour = Color.Crimson;
			}
			else
			{
				bgColour = Color.CornflowerBlue;
			}
		}

		// Callback for the second button, toggles whether button 1 is active or not
		public void testBtn2_OnClick( Control sender )
		{
			testBtn1.Active = ! testBtn1.Active;
		}



        #endregion
    }
}
