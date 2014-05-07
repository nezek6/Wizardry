using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UIManager;
using UIManager.Controls;
using WizardryShared;
using Lidgren.Network;

namespace WizardryClient
{

    class UpgradeMenu : GameScreen
    {
        #region Fields

        string message;

        ResourceProvider provider;

        Texture2D backgroundTexture;
        Texture2D crystalTexture;

        private Button upg1;
        private Button upg2;
        private Button upg3;
        private Button upg4;
        private Button upg5;
        private Button upg6;

        private Vector2 typeTextOrigin;

        private Button accept;
        private Button cancel;

        #endregion

        #region Events
        public delegate void EventHandler();

        public event EventHandler Accepted;
        public event EventHandler Cancelled;


        Viewport viewport;
        Vector2 viewportSize;
        Vector2 textSize;
        Vector2 textPosition;
        SpriteFont font;

        private readonly int STD_BUTTON_WIDTH = 190;
        private readonly int STD_BUTTON_HEIGHT = 46;

        private static readonly int UPG_BUTTON_WIDTH = 46;
        private static readonly int UPG_BUTTON_HEIGHT = 46;


        private static readonly int ROW1 = 150;
        private static readonly int ROW2 = 350;
        private static readonly int COL1 = (800 / 4) * 1 - UPG_BUTTON_WIDTH;
        private static readonly int COL2 = (800 / 4) * 2 - UPG_BUTTON_WIDTH;
        private static readonly int COL3 = (800 / 4) * 3 - UPG_BUTTON_WIDTH;


        private readonly Point UPG1_BUTTON_POS = new Point(COL1, ROW1);
        private readonly Point UPG2_BUTTON_POS = new Point(COL2, ROW1);
        private readonly Point UPG3_BUTTON_POS = new Point(COL3, ROW1);
        private readonly Point UPG4_BUTTON_POS = new Point(COL1, ROW2);
        private readonly Point UPG5_BUTTON_POS = new Point(COL2, ROW2);
        private readonly Point UPG6_BUTTON_POS = new Point(COL3, ROW2);


        private readonly Vector2 UPG1_TYPETEXT_POS = new Vector2(COL1-5, ROW1 - UPG_BUTTON_HEIGHT/2);
        private readonly Vector2 UPG2_TYPETEXT_POS = new Vector2(COL2, ROW1 - UPG_BUTTON_HEIGHT/2);
        private readonly Vector2 UPG3_TYPETEXT_POS = new Vector2(COL3-10, ROW1 - UPG_BUTTON_HEIGHT/2);

        private readonly Vector2 UPG1_POOLTEXT_POS = new Vector2(COL1 + UPG_BUTTON_WIDTH + 5, ROW1);
        private readonly Vector2 UPG2_POOLTEXT_POS = new Vector2(COL2 + UPG_BUTTON_WIDTH + 5, ROW1);
        private readonly Vector2 UPG3_POOLTEXT_POS = new Vector2(COL3 + UPG_BUTTON_WIDTH + 5, ROW1);

        private readonly Vector2 UPG1_VALTEXT_POS = new Vector2(COL1, ROW1 + UPG_BUTTON_HEIGHT + 10);
        private readonly Vector2 UPG2_VALTEXT_POS = new Vector2(COL2, ROW1 + UPG_BUTTON_HEIGHT + 10);
        private readonly Vector2 UPG3_VALTEXT_POS = new Vector2(COL3, ROW1 + UPG_BUTTON_HEIGHT + 10);

        private readonly Point ACCEPT_BUTTON_POS = new Point(300, 500);
        private readonly Point CANCEL_BUTTON_POS = new Point(400, 500);

        private readonly Color BTNTEXT_INERT_COLOUR = new Color(230, 165, 2);
        private readonly Color BTNTEXT_HOVER_COLOUR = new Color(230, 165, 2);
        private readonly Color BTNTEXT_PRIMED_COLOUR = new Color(255, 0, 0);
        private readonly Color BTNTEXT_INACTIVE_COLOUR = new Color(155, 155, 155);
        private static readonly int HPAD = 25;
        private static readonly int VPAD = 25;

        Rectangle backgroundRectangle = new Rectangle(HPAD, VPAD, 750, 550);


        #endregion

        #region Initialization

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public UpgradeMenu(ResourceProvider p)
        {
            IsPopup = true;
            provider = p;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            crystalTexture = p.Game.Content.Load<Texture2D>("Images/diamond");

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
                textSize = font.MeasureString("Upgrades");
                textPosition = ((viewportSize - textSize) / 2) - new Vector2(0,250);

                typeTextOrigin = font.MeasureString("HEALTH")/2;

                upg1 = new Button(UPG1_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);
                upg2 = new Button(UPG2_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);
                upg3 = new Button(UPG3_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);
                upg4 = new Button(UPG4_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);
                upg5 = new Button(UPG5_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);
                upg6 = new Button(UPG6_BUTTON_POS, UPG_BUTTON_WIDTH, UPG_BUTTON_HEIGHT, this);

                accept = new Button(ACCEPT_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                cancel = new Button(CANCEL_BUTTON_POS, STD_BUTTON_WIDTH, STD_BUTTON_HEIGHT, this);
                accept.Text = "Return";
                cancel.Text = "No";
                upg1.Text = " ";

                backgroundTexture = content.Load<Texture2D>("UITextures/upgradebg");

                upg1.Texture = content.Load<Texture2D>("UITextures/healthbutton");
                upg2.Texture = content.Load<Texture2D>("UITextures/focusbutton");
                upg3.Texture = content.Load<Texture2D>("UITextures/dmgbutton");
                upg4.Texture = content.Load<Texture2D>("UITextures/GUIButton");
                upg5.Texture = content.Load<Texture2D>("UITextures/GUIButton");
                upg6.Texture = content.Load<Texture2D>("UITextures/GUIButton");

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
                upg1.RegisterCallback(upg1_OnClick);
                upg2.RegisterCallback(upg2_OnClick);
                upg3.RegisterCallback(upg3_OnClick);
            }
        }


        #endregion

        #region HandleInput/Draw Overrides

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {

            upg1.HandleInputs(gameTime, input);
            upg2.HandleInputs(gameTime, input);
            upg3.HandleInputs(gameTime, input);
            upg4.HandleInputs(gameTime, input);
            upg5.HandleInputs(gameTime, input);
            upg6.HandleInputs(gameTime, input);

            accept.HandleInputs(gameTime, input);
			//cancel.HandleInputs(gameTime, input);
        }

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

            spriteBatch.Draw(crystalTexture, new Rectangle(40,40,35,35), Color.White);
            int crystalCount = provider.GameState.Characters[GameManager.Instance.MyID].CrystalCount;
            spriteBatch.DrawString(font, crystalCount.ToString() , new Vector2(80,40), Color.Black, 0, Vector2.Zero, 1.2f, SpriteEffects.None,0);

            // Draw the message box text.
            spriteBatch.DrawString(font, "Upgrades", textPosition, Color.Black);

            spriteBatch.DrawString(font, "Health", UPG1_TYPETEXT_POS, Color.Black);
            spriteBatch.DrawString(font, "Focus", UPG2_TYPETEXT_POS, Color.Black);
            spriteBatch.DrawString(font, "Damage", UPG3_TYPETEXT_POS, Color.Black);

            Character buyer = provider.GameState.Characters[GameManager.Instance.MyID];
            Tower tower =   provider.GameState.Towers[(int)buyer.Team];
            spriteBatch.DrawString(font, (tower.HealthPool).ToString() + " / " + (tower.NextHealthUpgrade).ToString(), UPG1_POOLTEXT_POS, Color.Black);
            spriteBatch.DrawString(font, (tower.FocusPool).ToString() + " / " + (tower.NextFocusUpgrade).ToString(), UPG2_POOLTEXT_POS, Color.Black);
            spriteBatch.DrawString(font, (tower.DamagePool).ToString() + " / " + (tower.NextDamageUpgrade).ToString(), UPG3_POOLTEXT_POS, Color.Black);

            spriteBatch.Draw(crystalTexture, UPG1_POOLTEXT_POS + new Vector2(60, -3), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(crystalTexture, UPG2_POOLTEXT_POS + new Vector2(60, -3), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(crystalTexture, UPG3_POOLTEXT_POS + new Vector2(60, -3), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

			spriteBatch.DrawString( font, (tower.HealthModifier - 1).ToString( "#0%" ), UPG1_VALTEXT_POS, Color.Black );
			spriteBatch.DrawString( font, (tower.FocusModifier - 1).ToString( "#0%" ), UPG2_VALTEXT_POS, Color.Black );
			spriteBatch.DrawString( font, (tower.DamageModifier - 1).ToString( "#0%" ), UPG3_VALTEXT_POS, Color.Black );


            spriteBatch.End();

            upg1.Draw(gameTime, TransitionAlpha);
            upg2.Draw(gameTime, TransitionAlpha);
            upg3.Draw(gameTime, TransitionAlpha);
            /*
            upg4.Draw(gameTime, TransitionAlpha);
            upg5.Draw(gameTime, TransitionAlpha);
            upg6.Draw(gameTime, TransitionAlpha);
            */
            accept.Draw(gameTime, TransitionAlpha);
          //  cancel.Draw(gameTime, TransitionAlpha);

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

        public void upg1_OnClick(Control Sender)
        {
			if ( GameManager.Instance.InMultiplayer )
			{
				SignalServer( 0 );
			}
			else
			{
				Character buyer = provider.GameState.Characters[GameManager.Instance.MyID];
				provider.GameState.Towers[(int)buyer.Team].UpgradeHealth(buyer);
			}
        }

        public void upg2_OnClick(Control Sender)
        {
			if ( GameManager.Instance.InMultiplayer )
			{
				SignalServer( 1 );
			}
			else
			{
				Character buyer = provider.GameState.Characters[GameManager.Instance.MyID];
				provider.GameState.Towers[(int)buyer.Team].UpgradeFocus(buyer);
			}
        }

        public void upg3_OnClick(Control Sender)
        {
			if ( GameManager.Instance.InMultiplayer )
			{
				SignalServer( 2 );
			}
			else
			{
				Character buyer = provider.GameState.Characters[GameManager.Instance.MyID];
				provider.GameState.Towers[(int)buyer.Team].UpgradeDamage(buyer);
			}
        }

		private void SignalServer( byte upgrade )
		{
			NetOutgoingMessage msg = GameManager.Instance.Client.CreateMessage();
			msg.Write( (byte)MsgType.ReqUpgrade );
			msg.Write( (byte)GameManager.Instance.MyID );
			msg.Write( (byte)upgrade );
			GameManager.Instance.Client.SendMessage( msg, NetDeliveryMethod.ReliableOrdered );
		}

        #endregion

    }
}
