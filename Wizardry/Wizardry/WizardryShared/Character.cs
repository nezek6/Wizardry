using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WizardryShared
{
	public partial class Character : Sprite
	{
        public int Score = 0;

		public enum CharStatus { NORMAL, DEAD, IMMUNE, OFFSCREEN }

		enum FaceDirection { FRONT, BACK, LEFT, RIGHT }

		public const float MAX_HEALTH = 100;

		public const float MAX_FOCUS = 100;

		#region Member Variables

		private const int HEIGHT_THRESHOLD = 6;

		// Amount of hit points that player has
		private float currentHealth;

		//Max amount of hitpoints
		private float maxHealth;

		private float focusGenRate = 0.015f;

		// Amount of focus player has
		private float currentFocus;

		//Modifiers
		private float healthModifier = 1;

		private float focusModifier = 1;

		private float damageModifier = 1;

		//Spells
		private int[] spells = new int[10];

		private double[] cooldowns = new double[4];
		private double[] cdCounter = new double[4];

		//Facing Direction
		private FaceDirection facing = FaceDirection.FRONT;

		//Is this the player
		private bool isPlayer = false;

		//Character's team
		private Team team = Team.RED;

		//Character's crystal count
		private int crystalCount = 0;

		//Character' status
		CharStatus status = CharStatus.NORMAL;

		//Character's name
		private string name = "Bob";

		//Is the server overriding position?
		private bool positionOverride = false;

		// Is the server overriding focus?
		private bool focusOverride = false;

		#endregion

		#region Properties

		public int RoleID
		{
			get { return ID; }
			set
			{
				if ( ID != value )
				{
					ID = value;

					//Set the characteristics
					spells = Details[ID].spells;
					maxHealth = Details[ID].maxHealth;

					//Set the cooldowns
					int i = 0;
					foreach (int slot in spells)
					{
						cooldowns[i] = Spell.Details[slot].cooldown;
						i++;
					}

					// Set the animations
					Animations = charAnimations[ID]();
				}
			}
		}

		public float HealthModifier
		{
			get { return healthModifier; }
			set
			{
				healthModifier = value;
			}
		}

		public float FocusModifier
		{
			get { return focusModifier; }
			set
			{
				focusModifier = value;
			}
		}

		public float DamageModifier
		{
			get { return damageModifier; }
			set
			{
				damageModifier = value;
			}
		}

		public float CurrentHealth
		{
			get { return currentHealth; }
			set
			{
				if (value >= maxHealth)
				{
					currentHealth = maxHealth;
				}
				else
				{
					currentHealth = value;
				}
			}
		}

		public float CurrentFocus
		{
			get { return currentFocus; }
			set
			{
				if (value >= MAX_FOCUS)
				{
					currentFocus = MAX_FOCUS;
				}
				else
				{
					currentFocus = value;
				}
			}
		}

		public float MaxHealth
		{
			get { return maxHealth; }
			set { maxHealth = value; }
		}

		public float FocusGenRate
		{
			get { return focusGenRate; }
			set { focusGenRate = value; }
		}

		public int CrystalCount
		{
			get { return crystalCount; }
			set
			{
				//Console.WriteLine("Crystal Grabbed"); 
				crystalCount = value;
				//Console.WriteLine(CrystalCount);
			}
		}

		public bool IsPlayer
		{
			get { return isPlayer; }
			set { isPlayer = value; }
		}

		public Team Team
		{
			get { return team; }
			set { team = value; }
		}

		public double[] Cooldowns
		{
			get { return cooldowns; }
		}

		public double[] CdCounter
		{
			get { return cdCounter; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool PositionOverride
		{
			get { return positionOverride; }
			set { positionOverride = value; }
		}

		public bool FocusOverride
		{
			get { return focusOverride; }
			set { focusOverride = value; }
		}

		public CharStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		public override Rectangle BoundingBox
		{
			get 
			{ 
				Rectangle ret = currentAnimation.BoundingBox;
				ret.X = (int)(WorldPosition.X - (ret.Width / 2));
				ret.Y = (int)(WorldPosition.Y - ret.Height);
				return ret;
			}
		}

		#endregion

		//Necessary Constructor
		public Character(ResourceProvider provider)
			: base(provider)
		{
		}

		#region Override of Sprite methods

		// Initialize the player
		public void Initialize(int ID, Vector2 position)
		{
			base.Initialize(position);

			RoleID = ID;

			speed = Details[RoleID].speed;
			maxHealth = Details[RoleID].maxHealth;
			currentHealth = maxHealth;
			currentFocus = 0;
			currentAnimation = Animations[Anim.STANDFRONT];
			facing = FaceDirection.FRONT;
		}

		#endregion

		public void Shoot( int spellSlot, Vector2 clickPos )
		{
			if ( cdCounter[spellSlot] <= 0 )
			{
				cdCounter[spellSlot] = cooldowns[spellSlot];
				Vector2 direction = new Vector2();
				Vector2 startPoint = worldPosition - new Vector2(0, currentAnimation.FrameHeight * currentAnimation.Scale / 2);
				direction = clickPos - startPoint;
				direction.Normalize();
				double rotation = Math.Atan2(direction.Y, direction.X);
				provider.Shoot( spells[spellSlot], startPoint, clickPos, direction, (float)rotation, this );
			}

			if (facing.Equals(FaceDirection.BACK))
			{
				if ( spellSlot == 0 )
				{ 
					SetCurrentAnimation( Anim.ATTACKBACK );
				}
				else 
				{
					SetCurrentAnimation( Anim.CASTBACK );
				}
			}
			else if (facing.Equals(FaceDirection.RIGHT))
			{
				if ( spellSlot == 0 )
				{
					SetCurrentAnimation( Anim.ATTACKRIGHT );
				}
				else
				{
					SetCurrentAnimation( Anim.CASTRIGHT );
				}
			}
			else if (facing.Equals(FaceDirection.LEFT))
			{
				if ( spellSlot == 0 )
				{
					SetCurrentAnimation( Anim.ATTACKLEFT );
				}
				else
				{
					SetCurrentAnimation( Anim.CASTLEFT );
				}
			}
			else
			{
				if ( spellSlot == 0 )
				{
					SetCurrentAnimation( Anim.ATTACKFRONT );
				}
				else
				{
					SetCurrentAnimation( Anim.CASTFRONT );
				}
			}
		}

		#region Update and Draw

		// Update the player animation
		public override void Update( TimeSpan elapsedTime )
		{
			Vector2 motion = Vector2.Zero;

			//Current Standing Animation
			if (facing.Equals(FaceDirection.BACK))
			{
				SetCurrentAnimation(Anim.STANDBACK);
			}
			else if (facing.Equals(FaceDirection.RIGHT))
			{
				SetCurrentAnimation(Anim.STANDRIGHT);
			}
			else if (facing.Equals(FaceDirection.LEFT))
			{
				SetCurrentAnimation(Anim.STANDLEFT);
			}
			else
			{
				SetCurrentAnimation(Anim.STANDFRONT);
			}

			if (isPlayer)
			{
				KeyboardState keyState = Keyboard.GetState();
				MouseState mouseState = Mouse.GetState();

				if (keyState.IsKeyDown(Keys.I) || keyState.IsKeyDown(Keys.W))
				{
					motion.Y--;
					SetCurrentAnimation(Anim.WALKBACK);
					facing = FaceDirection.BACK;
				}
				if (keyState.IsKeyDown(Keys.K) || keyState.IsKeyDown(Keys.S))
				{
					motion.Y++;
					SetCurrentAnimation(Anim.WALKFRONT);
					facing = FaceDirection.FRONT;
				}
				if (keyState.IsKeyDown(Keys.L) || keyState.IsKeyDown(Keys.D))
				{
					motion.X++;
					SetCurrentAnimation(Anim.WALKRIGHT);
					facing = FaceDirection.RIGHT;
				}
				if (keyState.IsKeyDown(Keys.J) || keyState.IsKeyDown(Keys.A))
				{
					motion.X--;
					SetCurrentAnimation(Anim.WALKLEFT);
					facing = FaceDirection.LEFT;
				}

				if (keyState.IsKeyDown(Keys.J) || keyState.IsKeyDown(Keys.F))
				{


				}

				if ( mouseState.LeftButton == ButtonState.Pressed )
				{

					Vector2 clickPos = new Vector2();
					clickPos.X = mouseState.X;
					clickPos.Y = mouseState.Y;
					clickPos += provider.Camera.Position;
					Shoot(0, clickPos);
				}

				if ( mouseState.RightButton == ButtonState.Pressed )
				{
                    if (currentFocus >= 100)
                    {
                        currentFocus -= 100;
                        Vector2 clickPos = new Vector2();
                        clickPos.X = mouseState.X;
                        clickPos.Y = mouseState.Y;
                        clickPos += provider.Camera.Position;
                        Shoot(1, clickPos);
                    }
				}

				if (keyState.IsKeyDown(Keys.Space))
				{
					Vector2 clickPos = new Vector2();
					clickPos.X = mouseState.X;
					clickPos.Y = mouseState.Y;
					clickPos += provider.Camera.Position;
					Shoot(2, clickPos);
				}
			}

			for ( int i = 0; i < 4; i++ )
			{
				if (cdCounter[i] > 0)
				{
					cdCounter[i] -= elapsedTime.TotalSeconds;
				}
				else
				{
					cdCounter[i] = 0;
				}
			}

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
			}

			Vector2 nextposition = worldPosition + (motion * speed);
			int nextHeight = provider.TileMap.GetHeightAtWorldPosition(nextposition);

			//worldPosition = nextposition;

			//TOFIX: Stange overflow bug
			//if (Math.Abs(nextHeight - height) <= HEIGHT_THRESHOLD)
			if (nextHeight - height <= HEIGHT_THRESHOLD)
			{
				worldPosition = nextposition;
				height = nextHeight;
			}

			CurrentFocus += (float)(focusGenRate * elapsedTime.TotalSeconds * 60 * focusModifier);
		}

		// Draw the player
		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if ( Active && status!=CharStatus.OFFSCREEN )
			{
				// Adjust the draw depth
				float drawDepth = provider.TileMap.GetDrawDepth(worldPosition, height);
				currentAnimation.DrawDepth = drawDepth;

				//Account for draw position shift
				drawPosition = worldPosition;
				drawPosition -= provider.Camera.Position;
				//drawPosition.X -= Width * currentAnimation.Scale / 2;
				drawPosition.Y -= (currentAnimation.FrameHeight * currentAnimation.Scale) / 2;
				drawPosition.Y -= (height / 10) * 32;

				//Draw the animation
				currentAnimation.Draw(spriteBatch, drawPosition, 0, alpha);

				//Update the animation
				currentAnimation.Update(gameTime);


				//DEBUG POINTS: Keep for debugging purposes
				/*
				Texture2D raw = new Texture2D(provider.Game.GraphicsDevice, 1, 1);
				raw.SetData(new Color[] { Color.White });
				spriteBatch.Draw(raw, new Rectangle((int)((worldPosition.X) - 5 - provider.Camera.Position.X), (int)((worldPosition.Y) - currentAnimation.FrameHeight*currentAnimation.Scale / 2 - 5 - provider.Camera.Position.Y), 10, 10), Color.Red);
				spriteBatch.Draw(raw, new Rectangle((int)((worldPosition.X) - 5 - provider.Camera.Position.X), (int)((worldPosition.Y) - 5 - provider.Camera.Position.Y), 10, 10), Color.Blue);
				*/

			}
		}
		#endregion

		#region Networking Stuff

		public override void WriteToPacket(NetOutgoingMessage packet)
		{
			packet.Write( (int)RoleID );

			packet.Write( worldPosition.X );
			packet.Write( worldPosition.Y );
			packet.Write( positionOverride );
			positionOverride = false;
			packet.Write( (int)currentAnim );
			packet.Write( (byte)status );
			packet.Write( currentHealth );
			packet.Write( currentFocus );
			packet.Write( focusOverride );
			focusOverride = false;
			packet.Write( speed );
			packet.Write( (byte)Team );
			packet.Write( Name );
			packet.Write( crystalCount );
		}

		public override void UpdateFromPacket(NetIncomingMessage packet)
		{
			RoleID = packet.ReadInt32();
			worldPosition.X = packet.ReadFloat();
			worldPosition.Y = packet.ReadFloat();
			positionOverride = packet.ReadBoolean();
			SetCurrentAnimation((Anim)packet.ReadInt32());
			status = (CharStatus)packet.ReadByte();
			currentHealth = packet.ReadFloat();
			currentFocus = packet.ReadFloat();
			focusOverride = packet.ReadBoolean();
			speed = packet.ReadInt32();
			Team = (Team)packet.ReadByte();
			Name = packet.ReadString();
			CrystalCount = packet.ReadInt32();
		}

		#endregion
	}
}
