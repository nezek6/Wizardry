using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;
using UIManager;
using WizardryClient.GameUI;
using WizardryShared;

namespace WizardryClient
{
	class TestGameScreen : GameScreen
	{
		#region Fields

		// The Tile Engine
		GameManager manager;
		List<Character> characters = new List<Character>();
		List<PickUp> pickUps = new List<PickUp>();
		Character player;
		ActionBar actionBar;
		PlayerFrame playerFrame;
		List<NamePlate> namePlates;
		Radar radar;
		Song bgMusic;
		InputAction cPress;
		InputAction bPress;
		InputAction escapePressed;
		InputAction uPressed;
		bool drawBB = false;
		Random randomizer = new Random();

		Texture2D scorebg;
		SpriteFont scoreFont;
        SpriteFont gameOverFont;
        bool gameOver = false;

		#endregion

		#region Initialization

		/// <summary>
		/// TestGameScreen Constructor
		/// </summary>
		public TestGameScreen()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void Activate(bool instancePreserved)
		{
			IsoTileMap world;
			Camera camera;

			if (!instancePreserved)
			{
				Color myColor = Color.FromNonPremultiplied(255, 10, 10, 128);
				Console.WriteLine(myColor);
				ContentManager content = ScreenManager.Game.Content;
				manager = GameManager.Instance;
				manager.InMultiplayer = false;

				escapePressed = new InputAction(null, new Keys[] { Keys.Escape }, true);
				uPressed = new InputAction(null, new Keys[] { Keys.U }, true);

				// Called when the screen is first activated.. can initialize here
				camera = new Camera(ScreenManager.Game);
				camera.Autonomous = false;
				manager.Camera = camera;

				world = new IsoTileMap(camera);
				world.Cursor.Active = true;
				world.Cursor.Colour = Color.Blue;
				world.LoadFromFile(content, "Content/Maps/MPMap.map");
				scorebg = content.Load<Texture2D>("UITextures/scorebg");
				scoreFont = content.Load<SpriteFont>("Fonts/GUIFont");
                gameOverFont = content.Load<SpriteFont>("Fonts/GameOverFont");
				world.SetCameraToWorldBounds();
				manager.TileMap = world;

								// Set up a fresh game state
				GameManager.Instance.GameState = new GameState(GameManager.Instance);

				manager.GameState.Characters = new Character[26];

				Character playerChar = new Character(manager);
				manager.GameState.Characters[0] = playerChar;
				playerChar.Initialize(1, new Vector2(350, 350));
				playerChar.IsPlayer = true;
				player = playerChar;
				characters.Add(playerChar);
				actionBar = new ActionBar(player);
				playerFrame = new PlayerFrame(player);
				playerChar.Team = Team.BLUE;
				player.CurrentFocus = 25;
				player.CurrentHealth = 100;
				player.CrystalCount = 500;

				manager.MyID = 0;


				for (int i = 1; i < 26; i++)
				{
					NPC c = new NPC(manager);
					manager.GameState.Characters[i] = c;
					bool canPlace = false;
					Vector2 pos = Vector2.Zero;
					while (!canPlace)
					{
						int xPos = randomizer.Next(32, manager.TileMap.GetWidthInPixels() - 32);
						int yPos = randomizer.Next(32, manager.TileMap.GetHeightInPixels() - 32);
						pos = new Vector2(xPos, yPos);
						if ((pos - manager.GameState.Characters[0].WorldPosition).Length() > 1000)
						{
							canPlace = true;
						}
					}
					c.Initialize(randomizer.Next(0, 4), pos);
				}

				/*
				NPC char1 = new NPC(manager);
				char1.Initialize(0, new Vector2(450, 350));
				characters.Add(char1);
				char1.Name = "Player1";

				NPC char2 = new NPC(manager);
				char2.Initialize(1, new Vector2(550, 350));
				characters.Add(char2);
				char2.Name = "Player2";
				char2.Team = Team.RED;
				char2.CurrentHealth = char2.MaxHealth / 2;

				NPC char3 = new NPC(manager);
				char3.Initialize(2, new Vector2(650, 350));
				characters.Add(char3);
				char3.Name = "Player3";

				NPC char4 = new NPC(manager);
				char4.Initialize(3, new Vector2(750, 350));
				characters.Add(char4);
				char4.Name = "Player4";
				char4.Team = Team.RED;
				char4.CurrentHealth = char2.MaxHealth / 2;
				*/
				/*
				Random randomizer = new Random();
				for (int i = 0; i < 20; i++)
				{

					Character chari = new Character(manager);
					chari.Initialize(3, new Vector2(randomizer.Next(0,4000),randomizer.Next(0, 4000)));
					characters.Add(chari);
					chari.Name = "Player4";
					chari.Team = Team.RED;
					chari.CurrentHealth = char2.MaxHealth;
					Console.WriteLine("ok");
				}
				*/

				//manager.GameState.Characters = new[] { playerChar, char1, char2, char3, char4 };
				namePlates = new List<NamePlate>();
				radar = new Radar(playerChar);
				radar.Scale = 0.5f;
				radar.Alpha = 0.8f;
				radar.SnapToCorner(Radar.Corner.TopRight);

				foreach (Character c in manager.GameState.Characters)
				{
					if (c != player)
					{
						NamePlate np = new NamePlate(c);
						namePlates.Add(np);
					}
				}

				for (int i = 0; i < 120; i++)
				{
					PickUp pu = new PickUp(manager);
					pickUps.Add(pu);
					pu.Initialize(0);
				}

				bgMusic = ScreenManager.Game.Content.Load<Song>("Audio/Demo_FinalFight");
				MediaPlayer.IsRepeating = true;
				MediaPlayer.Play( bgMusic );
				MediaPlayer.Volume = 0.4f;
				cPress = new InputAction(null, new Keys[] { Keys.C }, true);
				bPress = new InputAction(null, new Keys[] { Keys.B }, true);
			}
		}

		public override void Unload()
		{
			MediaPlayer.Stop();
			base.Unload();
		}

		#endregion

		#region HandleInput/Update/Draw Overrides

		public override void HandleInput(GameTime gameTime, InputState input)
		{
			PlayerIndex playerIndex;
			// Any UI elements that use the InputState can be handled here
			if (escapePressed.Evaluate(input, null, out playerIndex))
			{
				PopOutExit();
			}

			if (uPressed.Evaluate(input, null, out playerIndex))
			{
				DisplayUpgrades();
			}

			if (cPress.Evaluate(input, null, out playerIndex))
			{
				player.RoleID = ((player.RoleID + 1) % 4);
			}

			if (bPress.Evaluate(input, null, out playerIndex))
			{
				drawBB = !drawBB;
			}
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{

			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (gameOver)
            {
                return;
            }

			// This will ensure we can't keep moving around and shooting while popups are open
			if (otherScreenHasFocus)
			{
				return;
			}

            if (manager.GameState.Characters[0].CurrentHealth <= 0)
            {
                gameOver = true;
            }

			// Updating done here
			manager.Camera.Update(gameTime);

			foreach (Character c in manager.GameState.Characters)
			{
				if (c != characters[0])
				{
				   // if (c.CurrentHealth <= 0) c.Active = false;
					/*
					if (c.Name == "Player4")
					{
						c.Shoot(0, characters[0].WorldPosition);
						c.WorldPosition += (characters[0].WorldPosition - c.WorldPosition) / 300;
					}
					 */
				}
				c.Update(gameTime.ElapsedGameTime);
			}



			foreach (Spell s in manager.GameState.Spells)
			{
				s.Update(gameTime.ElapsedGameTime);
				if (s.Active)
				{
					foreach (Character c in manager.GameState.Characters)
					{
						if (s.Caster != c)
						{
							if (manager.CheckCollision(c, s, true))
							{
								//Console.WriteLine(c.CurrentHealth);
								s.Hit(gameTime.ElapsedGameTime, c);
								/*
								if (c.Name == "Player4" && s.Caster == characters[0])
								{
									Vector2 repulsion = characters[0].WorldPosition - characters[4].WorldPosition;
									repulsion.Normalize();
									characters[4].WorldPosition -= repulsion * 100;
								}
								 */
							}
						}
					}
				}
			}

			foreach (PickUp pu in pickUps)
			{
				pu.Update(gameTime.ElapsedGameTime);
				foreach (Character c in manager.GameState.Characters)
				{
					if (manager.CheckCollision(c, pu, false))
					{
						if (c.IsPlayer && pu.CurrentStatus.Equals(PickUp.PickUpStatus.SPAWNED))
						{
							pu.OnHit(gameTime.ElapsedGameTime, pu, c);
						}
					}
				}
			}

			foreach (NamePlate np in namePlates)
			{
				np.Update(gameTime);
			}

			actionBar.Update(gameTime);
			playerFrame.Update(gameTime);

			foreach (Character c in manager.GameState.Characters)
			{
				if (c.Status == Character.CharStatus.DEAD)
				{
					c.Status = Character.CharStatus.NORMAL;
					bool canPlace = false;
					Vector2 pos = Vector2.Zero;
					while (!canPlace)
					{
						int xPos = randomizer.Next(0, manager.TileMap.GetWidthInPixels());
						int yPos = randomizer.Next(0, manager.TileMap.GetHeightInPixels());
						pos = new Vector2(xPos, yPos);
						if ((pos - manager.GameState.Characters[0].WorldPosition).Length() > 1000)
						{
							canPlace = true;
						}
					}
					c.Initialize(randomizer.Next(0, 4), pos);
				}
			}

		}

		public override void Draw(GameTime gameTime)
		{
			manager.Camera.CenterOn(player.WorldPosition);

			SpriteBatch sb = manager.SpriteBatch;

			sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			manager.TileMap.Draw(sb);

			foreach (Character c in manager.GameState.Characters)
			{
				c.Draw(gameTime, sb);
				if (drawBB && c.Active)
				{
					DrawingHelpers.DrawPoint(sb, new Point((int)c.WorldPosition.X, (int)c.WorldPosition.Y), Color.Magenta, 3, true);
					DrawingHelpers.DrawWireRectangle(sb, c.CurrentAnimation.BoundingBox, 3f, GameSettings.TeamColour(c.Team));
				}
			}

			foreach (Spell s in manager.GameState.Spells)
			{
				s.Draw(gameTime, sb);
				if (drawBB && s.Active)
				{
					DrawingHelpers.DrawWireRectangle(sb, s.CurrentAnimation.BoundingBox, 3f, Color.Purple);
				}
			}

			foreach (PickUp pu in pickUps)
			{
				pu.Draw(gameTime, sb);
				if (drawBB && pu.Active && pu.CurrentStatus == PickUp.PickUpStatus.SPAWNED)
				{
					DrawingHelpers.DrawWireRectangle(sb, pu.CurrentAnimation.BoundingBox, 2f, Color.Gray);
				}
			}

			sb.End();

			sb.Begin();

			foreach (NamePlate np in namePlates)
			{
				np.Draw(gameTime, sb);
			}

			actionBar.Draw(gameTime, sb);
			playerFrame.Draw(gameTime, sb);
			radar.Draw(sb);

			sb.Draw(scorebg, new Rectangle(400, 20, 120, 40), null, Color.White, 0, new Vector2(60, 20), SpriteEffects.None, 0);
			sb.DrawString(scoreFont, manager.GameState.Characters[0].Score.ToString(), new Vector2(450, 35), Color.Black, 0, scoreFont.MeasureString("100") / 2, 2f, SpriteEffects.None, 0);

            if (gameOver)
            {
                sb.DrawString(scoreFont, "GAME OVER", new Vector2(400, 300), Color.Red, 0, scoreFont.MeasureString("GAMEOVER") / 2, 2f, SpriteEffects.None, 0);
            }

			sb.End();

			base.Draw(gameTime);
		}

		#endregion

		public void PopOutExit()
		{

			const string message = "Are you sure you want to go back to Main Menu?";

			PopUpScreen confirmExitMessageBox = new PopUpScreen(message, PopUpScreen.Style.YesNo);

			confirmExitMessageBox.Accepted += () =>
			{
				LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
			};

			confirmExitMessageBox.Cancelled += () => { };

			ScreenManager.AddScreen(confirmExitMessageBox, null);
		}


		public void DisplayUpgrades()
		{

			UpgradeMenu upgradeMenu = new UpgradeMenu(manager);

			upgradeMenu.Accepted += () => { };

			upgradeMenu.Cancelled += () => { };

			ScreenManager.AddScreen(upgradeMenu, null);
		}

	}

}
