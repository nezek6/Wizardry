using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using System;

namespace WizardryShared
{
	abstract public class Sprite
	{
        public enum Anim { STANDFRONT, STANDBACK, STANDLEFT, STANDRIGHT, ATTACKFRONT, ATTACKBACK, ATTACKLEFT, ATTACKRIGHT, WALKFRONT, WALKBACK, WALKLEFT, WALKRIGHT, CASTFRONT, CASTBACK, CASTLEFT, CASTRIGHT, SHIFTLEFT, SHIFTRIGHT, SPECIAL, PROJECTILE }

		#region Member Variables

		protected Dictionary<Anim, Animation> Animations = new Dictionary<Anim, Animation>();

		// This is the sprite's type identifier. Child classes can use it to associate the instance
		// with a specific set of details (eg. animations, role, spell type, behaviour, etc..)
		protected int ID = -1;

		// Current frame of animation
		protected Animation currentAnimation;
		protected Anim currentAnim;

		// State of the sprite
		private bool active;

		//Reference to the provider
		protected ResourceProvider provider;

		//Speed of the sprite
		protected int speed = 5;

		//Draw position
		protected Vector2 drawPosition;

		//World position of the sprite
		protected Vector2 worldPosition;

        //Height of the sprite
        protected int height;

		// Tha alpha level of the sprite (0 = transparent, 1 = fully visible)
		protected float alpha = 1f;

		#endregion

		#region Properties

		// Get the height of sprite
		protected int Height
		{
            get { return height; }
		}

		public Vector2 WorldPosition
		{
			get { return worldPosition; }
			set { worldPosition = value; }	// Might want this later
		}

		public int Speed
		{
			get
			{
				return speed;
			}
			set
			{
				speed = value;
			}
		}

		public bool Active
		{
			get	{ return active; }
			set
			{
				active = value;
				if ( !value )
				{
					ID = -1;
				}
			}
		}

        public ResourceProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                provider = value;
            }
        }

        public Animation CurrentAnimation
        {
            get { return currentAnimation; }
        }

		public Anim CurrentAnimValue
		{
			get { return currentAnim; }
		}

		public float Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		public abstract Rectangle BoundingBox
		{
			get;
		}

		#endregion

		#region Constructor

		//Necessary Constructor
		public Sprite( ResourceProvider provider )
		{
			this.provider = provider;
			this.active = false;
		}


		#endregion
		// Initialize the sprite

		#region Sprite Methods

		public virtual void Initialize( Vector2 position )
		{
			// Set the starting position of the player around the middle of the screen and to the back
			worldPosition = position;

			// Set the player to be active
			active = true;
		}

		public virtual void SetCurrentAnimation( Anim animation )
		{
			currentAnim = animation;
			currentAnimation = Animations[animation];
		}

		#endregion

		// Update the player animation MUST IMPLEMENT
		public abstract void Update( TimeSpan elapsedTime );


		// Draw the sprite MUST IMPLEMENT
		public abstract void Draw( GameTime gameTime, SpriteBatch spriteBatch );

		#region Networking Stuff

		public abstract void WriteToPacket( NetOutgoingMessage packet );

		public abstract void UpdateFromPacket( NetIncomingMessage packet );

		#endregion
	}
}
