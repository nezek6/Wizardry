using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WizardryShared;

namespace WizardryClient.GameUI
{
    class ActionBar
    {
        private readonly int NUM_BUTTONS = 3;
        private readonly Vector2 BUTTON_SIZE = new Vector2(40, 40);
        private readonly int HPAD = 15;
        private readonly int SPACING = 10;
        private readonly int VPAD = 15;
        private readonly string[] letters = {"LC","RC","SP"};

        private ResourceProvider provider;
        private Character character;
        private Rectangle[] buttons;
        private Rectangle[] cds;
        private Texture2D buttonTexture;
        private Texture2D coolDownTexture;
        private Color textColor;
        private SpriteFont font;

        public ActionBar(Character character)
        {
            this.provider = GameManager.Instance;
            this.character = character;

            buttons = new Rectangle[NUM_BUTTONS];
            cds = new Rectangle[NUM_BUTTONS];

            ContentManager content = provider.Game.Content;

            font = content.Load<SpriteFont>("Fonts/GUIFont");
            textColor = Color.White;
           
            buttonTexture = content.Load<Texture2D>("UITextures/popbg");
            coolDownTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            coolDownTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.5f) });

            int xOffset = HPAD;
            int yOffset = provider.Game.GraphicsDevice.Viewport.Height - VPAD - (int)BUTTON_SIZE.Y;

            for (int i = 0; i < NUM_BUTTONS; i++)
            {
                buttons[i] = new Rectangle(xOffset, yOffset, (int)BUTTON_SIZE.X, (int)BUTTON_SIZE.Y);
                cds[i] = new Rectangle(xOffset, yOffset, (int)BUTTON_SIZE.X, (int)BUTTON_SIZE.Y);
                xOffset += (int)BUTTON_SIZE.X + SPACING;
            }
        }

        public void Update(GameTime gameTime)
        {

            for (int i = 0; i < NUM_BUTTONS; i++)
            {
                cds[i].Height = (int)(buttons[i].Height * (character.CdCounter[i] / character.Cooldowns[i]));
             }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int i = 0; i < NUM_BUTTONS; i++)
            {

                spriteBatch.Draw(buttonTexture, buttons[i], Color.White);
                spriteBatch.Draw(coolDownTexture, cds[i], Color.White);
                spriteBatch.DrawString(font, letters[i], new Vector2(buttons[i].X + BUTTON_SIZE.X * 0.15f, buttons[i].Y + BUTTON_SIZE.Y * 0.15f), textColor);
                /*
                spriteBatch.Draw(
                coolDownTexture,
                cds[i],
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);*/

            }
        }

    }
}

