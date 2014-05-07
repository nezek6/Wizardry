// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TileEngine
{
   public class Spell
    {

        //Rotation of the Spell
        public float Rotation = 0;

        // Image representing the Spell
        public Texture2D Texture;

        // Position of the Center of the Spell
        public Vector2 Position;

        // State of the Projectile
        public bool Active;

        // Represents the viewable boundary of the game
        Viewport viewport;

        // Get the width of the Spell
        public int Width
        {
            get { return Texture.Width; }
        }

        // Get the height of the Spell
        public int Height
        {
            get { return Texture.Height; }
        }

        // Determines how long the spell takes to animate
        float AnimationTime;

        // Determine how much time started since the animation started
        float elapsedTime = 0;


        public void Initialize(Viewport viewport, Texture2D texture)
        {
            Texture = texture;
            this.viewport = viewport;
            Active = false;
            AnimationTime = 4f;
        }

        public void Update(Vector2 position, GameTime gameTime,  KeyboardState currentKeyboardState)
        {
            if (!Active && currentKeyboardState.IsKeyDown(Keys.F1))
            {
                Position = position;
                Active = true;
            }
            if (Active)
            {
                Rotation = (float)Math.PI * 4f * (elapsedTime / AnimationTime);
                
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Console.WriteLine(elapsedTime);
                // Deactivate the spell after the animation time
                if (elapsedTime > AnimationTime)
                {
                    Active = false;
                    elapsedTime = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation,
                new Vector2(Width / 2, Height / 2), MathHelper.Clamp(1.5f-(1f*(elapsedTime*4/AnimationTime)), 0.1f,1.5f), SpriteEffects.None, 0f);
            }
        }
    }
}
