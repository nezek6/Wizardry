using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WizardryShared;

namespace WizardryClient.GameUI
{
    class NamePlate
    {
        private readonly Vector2 BAR_SIZE = new Vector2(45, 10);
        private readonly int VSPACE = 20;

        private ResourceProvider provider;
        private Character character;
        private Rectangle maxHealth;
        private Rectangle currentHealth;
        private Rectangle effectiveHealth;
        private Vector2 namePosition;
        private Texture2D maxHealthTexture;
        private Texture2D currentHealthTexture;
        private Texture2D effectiveHealthTexture;
        private SpriteFont font;

        private Color teamColor;
        private string name;


        public NamePlate(Character character)
        {
            this.provider = GameManager.Instance;
            this.character = character;

            ContentManager content = provider.Game.Content;

            teamColor = GameSettings.TeamColour(character.Team);
            name = character.Name;
            font = content.Load<SpriteFont>("Fonts/namePlateFont");

            maxHealth = new Rectangle(0, 0, (int)BAR_SIZE.X, (int)BAR_SIZE.Y);
            effectiveHealth = new Rectangle(0, 0, (int)BAR_SIZE.X, (int)BAR_SIZE.Y);
            currentHealth = new Rectangle(0, 0, (int)BAR_SIZE.X, (int)BAR_SIZE.Y);
            namePosition = new Vector2();

            maxHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            maxHealthTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.7f) });

            currentHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            currentHealthTexture.SetData<Color>(new[] { Color.White });

            effectiveHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            effectiveHealthTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.5f) });

        }

        public void Update(GameTime gameTime)
        {
			if ( character.Active )
			{
				float scaledFrameHeight = character.CurrentAnimation.FrameHeight * character.CurrentAnimation.Scale;

				namePosition.X = (int)character.WorldPosition.X - (int)provider.Camera.Position.X;
				namePosition.Y = maxHealth.Y = (int)character.WorldPosition.Y - (int)(scaledFrameHeight) - (int)BAR_SIZE.Y - (int)(VSPACE*0.2) - (int)provider.Camera.Position.Y;


				maxHealth.X = (int)character.WorldPosition.X - (int)(BAR_SIZE.X / 2) - (int)provider.Camera.Position.X;
				maxHealth.Y = (int)character.WorldPosition.Y - (int)(scaledFrameHeight) - VSPACE - (int)provider.Camera.Position.Y;

				currentHealth.X = (int)character.WorldPosition.X - (int)(BAR_SIZE.X / 2) - (int)provider.Camera.Position.X;
				currentHealth.Y = (int)character.WorldPosition.Y - (int)(scaledFrameHeight) - VSPACE - (int)provider.Camera.Position.Y;
				currentHealth.Width = (int)(BAR_SIZE.X * (character.CurrentHealth / character.MaxHealth));

				/*
				effectiveHealth.X = (int)character.WorldPosition.X - (int)(BAR_SIZE.X / 2) - (int)provider.Camera.Position.X;
				effectiveHealth.Y = (int)character.WorldPosition.Y - (int)(character.CurrentAnimation.FrameHeight * character.CurrentAnimation.Scale) - VSPACE - (int)provider.Camera.Position.Y;
				*/
			}
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if ( character.Active )
            {
                Vector2 nameOrigin = font.MeasureString("<" + name + ">");
                nameOrigin.X /= 2;
                spriteBatch.DrawString(font, "<" + name + ">", namePosition, teamColor, 0f, nameOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw( maxHealthTexture, maxHealth, null, teamColor, 0, Vector2.Zero, SpriteEffects.None, 0f );
				spriteBatch.Draw( currentHealthTexture, currentHealth, null, teamColor, 0, Vector2.Zero, SpriteEffects.None, 0f );
                //spriteBatch.Draw(effectiveHealthTexture, effectiveHealth, Color.White);
            }
        }

    }
}

