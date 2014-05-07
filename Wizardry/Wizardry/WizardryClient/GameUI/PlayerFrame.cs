using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WizardryShared;

namespace WizardryClient.GameUI
{
    class PlayerFrame
    {
        private static readonly float SCALE = 0.5f;

        private static readonly int VSPACE = 5;
        private static readonly int HSPACE = 5;

        private ResourceProvider provider;

        private Character character;
        private Rectangle maxHealth = new Rectangle((int)(130 * SCALE) + HSPACE, (int)(6 * SCALE) + VSPACE, (int)(293 * SCALE), (int)(54 * SCALE));
        private Rectangle currentHealth = new Rectangle((int)(130 * SCALE) + HSPACE, (int)(6 * SCALE) + VSPACE, (int)(293 * SCALE), (int)(54 * SCALE));
        private Rectangle effectiveHealth = new Rectangle((int)(130 * SCALE) + HSPACE, (int)(6 * SCALE) + VSPACE, (int)(293 * SCALE), (int)(54 * SCALE));
        private Rectangle maxFocus = new Rectangle((int)(130 * SCALE) + HSPACE, (int)(64 * SCALE) + VSPACE, (int)(293 * SCALE), (int)(54 * SCALE));
        private Rectangle currentFocus = new Rectangle((int)(130 * SCALE) + HSPACE, (int)(64 * SCALE) + VSPACE, (int)(293 * SCALE), (int)(54 * SCALE));
        private Rectangle roleIcon = new Rectangle((int)(13 * SCALE) + HSPACE, (int)(8 * SCALE) + VSPACE, (int)(104 * SCALE), (int)(104 * SCALE));
        private Rectangle roleBg = new Rectangle((int)(8 * SCALE) + HSPACE, (int)(4 * SCALE) + VSPACE, (int)(115 * SCALE), (int)(115 * SCALE));
        private Rectangle frame = new Rectangle(0 + HSPACE, 0 + VSPACE, (int)(500*SCALE), (int)(123 * SCALE));
        private Rectangle crystal = new Rectangle((int)(444 * SCALE) + HSPACE, (int)(13 * SCALE) + VSPACE, (int)(43 * SCALE), (int)(43 * SCALE));
        private Rectangle crystalbg = new Rectangle((int)(438 * SCALE) + HSPACE, (int)(5 * SCALE) + VSPACE, (int)(55 * SCALE), (int)(112 * SCALE));

        private Texture2D maxHealthTexture;
        private Texture2D currentHealthTexture;
        private Texture2D effectiveHealthTexture;
        private Texture2D maxFocusTexture;
        private Texture2D currentFocusTexture;
        private Texture2D[] roleIconTextures = new Texture2D[10];
        private Texture2D roleBgTexture;
        private Texture2D frameTexture;
        private Texture2D crystalTexture;

        private SpriteFont font;
        private SpriteFont font2;

        private Color teamColor;
        private string name;
        private int currentRole = 0;

        private Vector2 healthStringPos;
        private Vector2 healthStringOrigin;
        private Vector2 focusStringPos;
        private Vector2 focusStringOrigin;
        private Vector2 crystalCountPos;
        private Vector2 crystalCountOrigin;

        public PlayerFrame(Character character)
        {
            this.provider = GameManager.Instance;
            this.character = character;

            ContentManager content = provider.Game.Content;

            name = character.Name;
            font = content.Load<SpriteFont>("Fonts/GUIFont");
            font2 = content.Load<SpriteFont>("Fonts/playerFrameFont");

            maxHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            maxHealthTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.3f) });

            currentHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            currentHealthTexture.SetData<Color>(new[] { new Color(0.1f, 0.7f, 0.1f, 1f) });

            effectiveHealthTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            effectiveHealthTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.1f) });

            maxFocusTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            maxFocusTexture.SetData<Color>(new[] { new Color(0.1f, 0.1f, 0.1f, 0.3f) });

            currentFocusTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            currentFocusTexture.SetData<Color>(new[] { new Color(255, 215, 0f, 1f) });

            crystalTexture = content.Load<Texture2D>("Images/diamond");

                roleIconTextures[0] = content.Load<Texture2D>("Images/wizardicon");
   
                roleIconTextures[1] = content.Load<Texture2D>("Images/priesticon");
    
                roleIconTextures[2] = content.Load<Texture2D>("Images/wlicon");

                roleIconTextures[3] = content.Load<Texture2D>("Images/bmicon");

            roleBgTexture = new Texture2D(provider.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            roleBgTexture.SetData<Color>(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });

            frameTexture = content.Load<Texture2D>("UITextures/CharFrame");

            healthStringPos= new Vector2(maxHealth.X + maxHealth.Width/2, maxHealth.Y + maxHealth.Height/2+2);
            healthStringOrigin = font.MeasureString("HEALTH") * 0.5f;

            focusStringPos = new Vector2(maxFocus.X + maxFocus.Width/2, maxFocus.Y + maxFocus.Height/2+2);
            focusStringOrigin = font.MeasureString("FOCUS") * 0.5f;

            crystalCountPos = new Vector2((int)(466*SCALE), (int)(91*SCALE));
            crystalCountOrigin = font.MeasureString("55") * 0.5f;


        }

        public void Update(GameTime gameTime)
        {
            currentHealth.Width = (int)(maxHealth.Width * (character.CurrentHealth / character.MaxHealth));
            currentFocus.Width = (int)(maxFocus.Width * (character.CurrentFocus / Character.MAX_FOCUS));

            /*
            effectiveHealth.X = (int)character.WorldPosition.X - (int)(BAR_SIZE.X / 2) - (int)provider.Camera.Position.X;
            effectiveHealth.Y = (int)character.WorldPosition.Y - (int)(character.CurrentAnimation.FrameHeight * character.CurrentAnimation.Scale) - VSPACE - (int)provider.Camera.Position.Y;
            */

            ContentManager content = provider.Game.Content;
            if (character.RoleID == 0)
            {
                currentRole = 0;
            }
            else if (character.RoleID == 1)
            {
                currentRole = 1;
            }
            else if (character.RoleID == 2)
            {
                currentRole = 2;
            }
            else if (character.RoleID == 3)
            {
                currentRole = 3;
            }


        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (character.Active)
            {
                teamColor = GameSettings.TeamColour(character.Team);
                teamColor.A = 100;


                spriteBatch.Draw(maxHealthTexture, maxHealth, Color.Red);
                spriteBatch.Draw(currentHealthTexture, currentHealth, Color.White);
                //spriteBatch.Draw(effectiveHealthTexture, effectiveHealth, Color.White);
                spriteBatch.Draw(maxFocusTexture, maxFocus, Color.Red);
                spriteBatch.Draw(currentFocusTexture, currentFocus, Color.White);
                spriteBatch.Draw(roleBgTexture, roleBg, teamColor);
                spriteBatch.Draw(roleIconTextures[currentRole], roleIcon, Color.White);
                DrawingHelpers.DrawFilledRectangle(spriteBatch, crystalbg, new Color(0, 0, 0) * 0.5f);
                spriteBatch.Draw(crystalTexture, crystal, Color.White);
                spriteBatch.Draw(frameTexture, frame, Color.White);

                spriteBatch.DrawString(font, "HEALTH", healthStringPos, Color.Black, 0f, healthStringOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "FOCUS", focusStringPos, Color.Black, 0f, focusStringOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font2, character.CrystalCount.ToString(), crystalCountPos, Color.White, 0f, crystalCountOrigin, 1f, SpriteEffects.None, 0f);


            }
        }

    }
}

