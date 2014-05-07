using System;
using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
    public partial class PickUp : Sprite
    {
        public enum PickUpStatus { SPAWNED, RESPAWNING }

        #region Member Variables

        private const int HEIGHT_THRESHOLD = 6;
		
        //Rotation
        float rotation;

        //As the PickUp been activated
        private double respawnTime;

        //Keeps track of tick times which are useful for some PickUps
        private double respawnTimer = 0;

        private HitBehaviour onHit;

        private PickUpStatus currentStatus;

        private Vector2 fixedLocation = new Vector2(-1, -1);

        //Necessary Constructor
        public PickUp(ResourceProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Properties

		public int PickupID
		{
			get { return ID; }
			set
			{
				if ( ID != value )
				{
					ID = value;
					currentAnimation = PickUpAnimations[ID]();
				}
			}
		}

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public HitBehaviour OnHit
        {
            get
            {
                return onHit;
            }
        }

        public PickUpStatus CurrentStatus
        {
            get { return currentStatus; }
        }

        public Vector2 FixedLocation
        {
            get
            {
                return fixedLocation;
            }
            set
            {
                fixedLocation = value;
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

        // Initialize the pickup
        public void Initialize( int ID )
        {
            this.PickupID = ID;

            respawnTime = Details[ID].respawnTime;
            respawnTimer = 0;

            onHit = Details[ID].hitBehaviour;

            currentStatus = PickUpStatus.RESPAWNING;

            // Set the pickup to be active
            Active = true;
        }

        #endregion

        #region Update and Draw

        // Update the pickup
        public override void Update( TimeSpan elapsedTime )
        {
            if ( Active && currentStatus == PickUpStatus.RESPAWNING )
            {
                if ( respawnTimer <= 0 )
                {
                    respawnTimer = 0;
					Details[PickupID].spawnBehaviour( elapsedTime, this );
                    currentStatus = PickUpStatus.SPAWNED;
                }
                else
                {
					respawnTimer -= elapsedTime.TotalSeconds;
                }
            }

            //base.Update(gameTime);
        }

        // Draw the pickup
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Account for draw position shift
            if ( Active && currentStatus.Equals( PickUpStatus.SPAWNED ) )
            {
                float drawDepth = provider.TileMap.GetDrawDepth(worldPosition + new Vector2(currentAnimation.FrameWidth / 2, currentAnimation.FrameHeight * 1.15f), height);
                currentAnimation.DrawDepth = drawDepth;

                //Console.WriteLine(worldPosition);
                drawPosition = worldPosition;
                drawPosition -= provider.Camera.Position;
                currentAnimation.Draw(spriteBatch, drawPosition, rotation, alpha);
                //Update the animation
                currentAnimation.Update(gameTime);
            }
        }

        #endregion

		#region Networking Stuff

		public override void WriteToPacket( NetOutgoingMessage packet )
		{
			packet.Write( PickupID );
			packet.Write( worldPosition.X );
			packet.Write( worldPosition.Y );
			packet.Write( (byte)CurrentStatus );
		}

		public override void UpdateFromPacket( NetIncomingMessage packet )
		{
			PickupID = packet.ReadInt32();
			worldPosition.X = packet.ReadFloat();
			worldPosition.Y = packet.ReadFloat();
			currentStatus = (PickUpStatus)packet.ReadByte();
		}

		#endregion
    }
}
