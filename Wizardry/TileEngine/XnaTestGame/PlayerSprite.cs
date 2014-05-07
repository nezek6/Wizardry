using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace XnaTestGame
{
    class PlayerSprite : DrawableGameComponent
    {

        Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        // Animation representing the player
        public Animation CurrentAnimation;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;

        //Reference to the parent game
        Game1 parentGame;

        //Speed of the sprite
        int Speed = 5;

        //Draw position
        public Vector2 drawPosition;

        //World position of the sprite
        public Vector2 worldPosition;

        // Get the width of the player ship
        public int Width
        {
            get { return CurrentAnimation.FrameWidth; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return CurrentAnimation.FrameHeight; }
        }

        //Necessary Constructor
        public PlayerSprite(Game1 game)
            : base(game)
        {
            parentGame = game;
        }



        // Initialize the player
        public void Initialize(String animationDir, Vector2 position)
        {

            string[] filePaths = Directory.GetFiles("Content/" + animationDir);

            int i = 0;
            foreach (string file in filePaths)
            {
                Texture2D animTexture = parentGame.Content.Load<Texture2D>(animationDir + "/" + Path.GetFileNameWithoutExtension(file));
                Animation anim = new Animation();
                if (file.ToLower().Contains("back"))
                {
                    Animations.Add("walkBack", anim);
                    CurrentAnimation = anim;
                    anim.Initialize(animTexture, Vector2.Zero, 6, 30, Color.White, 2f, true);
                }

                if (file.ToLower().Contains("front"))
                {
                    Animations.Add("walkFront", anim);
                    anim.Initialize(animTexture, Vector2.Zero, 6, 30, Color.White, 2f, true);
                }

                if (file.ToLower().Contains("side"))
                {
                    Animations.Add("walkLeft", anim);
                    anim.Initialize(animTexture, Vector2.Zero, 6, 30, Color.White, 2f, true);
                    anim = new Animation();
                    Animations.Add("walkRight", anim);
                    anim.Initialize(animTexture, Vector2.Zero, 6, 30, Color.White, 2f, true);
                    anim.Flip = true;
                }

                if (file.ToLower().Contains("stand"))
                {
                    Animations.Add("stand", anim);
                    anim.Initialize(animTexture, Vector2.Zero, 3, 30, Color.White, 2f, false);
                }

                if (file.ToLower().Contains("attack"))
                {
                    Animations.Add("attack", anim);
                    anim.Initialize(animTexture, Vector2.Zero, 15, 30, Color.White, 2f, true);
                }

                i++;
            }


            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;


            // Set the player to be active
            Active = true;


            // Set the player health
            Health = 100;

            worldPosition = position;
        }


        // Update the player animation
        public override void Update(GameTime gameTime)
        {
            Vector2 motion = Vector2.Zero;

            CurrentAnimation = Animations["stand"];

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.I) || keyState.IsKeyDown(Keys.W))
            {
                motion.Y--;
                CurrentAnimation = Animations["walkBack"];
            }
            if (keyState.IsKeyDown(Keys.K) || keyState.IsKeyDown(Keys.S))
            {
                motion.Y++;
                CurrentAnimation = Animations["walkFront"];
            }
            if (keyState.IsKeyDown(Keys.L) || keyState.IsKeyDown(Keys.D))
            {
                motion.X++;
                CurrentAnimation = Animations["walkRight"];
            }
            if (keyState.IsKeyDown(Keys.J) || keyState.IsKeyDown(Keys.A))
            {
                motion.X--;
                CurrentAnimation = Animations["walkLeft"];
            }

            if (keyState.IsKeyDown(Keys.J) || keyState.IsKeyDown(Keys.F))
            {
                CurrentAnimation = Animations["attack"];
            }

            if (motion != Vector2.Zero)
            {
                motion.Normalize();
            }

            Vector2 nextPosition = Position + (motion * Speed);

            Position = nextPosition;
            worldPosition = Position;
            

            int height = parentGame.tileMap.GetHeightAtWorldPosition(worldPosition);

            // Adjust the draw depth
            float drawDepth = parentGame.tileMap.GetDrawDepth(worldPosition, height);
            CurrentAnimation.drawDepth = drawDepth;

            //Update the animation
            CurrentAnimation.Update(gameTime);

            //Console.WriteLine(Position);

			parentGame.camera.CenterOn(Position);
            

            base.Update(gameTime);
        }

        // Draw the player
        public override void Draw(GameTime gameTime)
        {
            CurrentAnimation.Draw(parentGame.spriteBatch, Position, parentGame.camera);
        }
    }
}
