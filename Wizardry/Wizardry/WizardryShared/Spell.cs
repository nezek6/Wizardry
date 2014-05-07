using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WizardryShared
{
    public partial class Spell : Sprite
    {
        #region Member Variables

        private const int HEIGHT_THRESHOLD = 6;

        // Distance the spell can travel
        private double maxDistance = 400;

        // Distance the spell has travelled
        private float distance;

        //Origin of the spell
        private Vector2 startPoint;

        //Direction of the spell
        private Vector2 direction;

        //Rotation
        float rotation;

        //As the spell been activated
        private double coolDown;

        //Keeps track of tick times which are useful for some spells
        private double tickTracker = -0.00000000001;

        //Boolean which causes draw depth to be ignored
        private bool ignoreDrawDepth = false;

        //Character which cast the spell
        private Character caster;

        private float userData;

        private Vector2 clickPosition;

        private float clickDistance;

        //Necessary Constructor
        public Spell(ResourceProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Properties

        public int SpellID
        {
            get { return ID; }
            set
            {
				if ( ID != value )
				{
					ID = value;
					currentAnimation = spellAnimations[ID]();
				}
            }
        }

        public double MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public Character Caster
        {
            get { return caster; }
            set { caster = value; }
        }

        public Vector2 ClickPosition
        {
            get { return clickPosition; }
            set
            {
                clickPosition = value;
                //ASSUMPTION: Startposition has already been set.
                clickDistance = (clickPosition - startPoint).Length();
            }
        }

		public override Rectangle BoundingBox
		{
			get
			{
				Rectangle ret = currentAnimation.BoundingBox;
				ret.X = (int)( WorldPosition.X - ( ret.Width / 2 ) );
				ret.Y = (int)( WorldPosition.Y - ( ret.Height / 2 ) );
				return ret;
			}
		}

        #endregion

        #region Override of Sprite methods

        // Initialize the player
        public void Initialize(int spellID)
        {
            SpellID = spellID;
            worldPosition = startPoint;
            distance = 0;
            speed = Details[spellID].speed;
            maxDistance = Details[spellID].maxDistance;
            coolDown = Details[spellID].cooldown;
            tickTracker = 0;
        }

        #endregion

        #region Update and Draw

        // Update the player animation
        public override void Update( TimeSpan elapsedTime )
        {
            if ( Active )
            {
                if (distance < maxDistance)
                {
                    Details[SpellID].updateBehaviour( elapsedTime, this );

                    height = provider.TileMap.GetHeightAtWorldPosition( worldPosition );
				}
                else
                {
                    Active = false;
                }
            }
        }

        // Draw the player
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Account for draw position shift
            if ( Active )
            {
                currentAnimation.DrawDepth = 0;
                if (!Details[SpellID].ignoreDrawDepth)
                {
                    float drawDepth = provider.TileMap.GetDrawDepth(worldPosition + new Vector2(currentAnimation.FrameWidth / 2, currentAnimation.FrameHeight * 1.15f), height);
                    currentAnimation.DrawDepth = drawDepth;
                }

                drawPosition = worldPosition;
                drawPosition -= provider.Camera.Position;
                currentAnimation.Draw(spriteBatch, drawPosition, rotation, alpha);
                //Update the animation
				currentAnimation.Update(gameTime);
            }
        }

        public void Hit( TimeSpan elapsedTime, Character target )
        {
			if ( SpellID >= 0 && SpellID < Details.Count )
			{
				Details[SpellID].hitBehaviour(elapsedTime, this, target);
			}
        }

        #endregion

        #region Networking Stuff

        public override void WriteToPacket(NetOutgoingMessage packet)
        {
            packet.Write((int)SpellID);
            packet.Write(worldPosition.X);
            packet.Write(worldPosition.Y);
            packet.Write((float)rotation);
        }

        public override void UpdateFromPacket(NetIncomingMessage packet)
        {
            SpellID = packet.ReadInt32();
            worldPosition.X = packet.ReadFloat();
            worldPosition.Y = packet.ReadFloat();
            rotation = packet.ReadFloat();
        }

        #endregion
    }
}
