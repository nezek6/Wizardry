using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
    public class Tower : Sprite
    {
        Team team;

        //Modifiers
        private float healthModifier = 1;

        private float focusModifier = 1;

        private float damageModifier = 1;

        private int healthPool = 0;

        private int focusPool = 0;

        private int damagePool = 0;

        private int nextHealthUpgrade = 250;

        private int nextFocusUpgrade = 250;

        private int nextDamageUpgrade = 250;

		#region Properties

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

        public int NextHealthUpgrade
        {
            get { return nextHealthUpgrade; }
            set { nextHealthUpgrade = value; }
        }

        public int NextFocusUpgrade
        {
            get { return nextFocusUpgrade; }
            set { nextFocusUpgrade = value; }
        }

        public int NextDamageUpgrade
        {
            get { return nextDamageUpgrade; }
            set { nextDamageUpgrade = value; }
        }

        public int HealthPool
        {
            get { return healthPool; }
            set { healthPool = value; }
        }

        public int FocusPool
        {
            get { return focusPool; }
            set { focusPool = value; }
        }

        public int DamagePool
        {
            get { return damagePool; }
            set { damagePool = value; }
        }

		public override Rectangle BoundingBox
		{
			// TODO: Not implemented
			get { return new Rectangle(); }
		}

		#endregion

		#region Methods

		public Tower (Team team, ResourceProvider provider):base(provider)
        {
            this.team = team;
        }

        public void UpgradeHealth(Character buyer)
        {
            if (buyer.CrystalCount >= 5)
            {
                buyer.CrystalCount -= 5;
                HealthPool += 5;
                if (HealthPool >= NextHealthUpgrade)
                {
                    HealthModifier += 0.05f;
                    NextHealthUpgrade += NextHealthUpgrade * 2;
                    foreach (Character c in provider.GameState.Characters)
                    {
                        if (c.Team == team)
                        {
                            c.HealthModifier = healthModifier;
                            c.MaxHealth *= healthModifier;
                            c.CurrentHealth += 5;
                        }
                    }
                }
            }
        }

        public void UpgradeFocus(Character buyer)
        {
            if (buyer.CrystalCount >= 5)
            {
                buyer.CrystalCount -= 5;
                FocusPool += 5;
                if (FocusPool >= NextFocusUpgrade)
                {
                    FocusModifier += 0.05f;
                    NextFocusUpgrade += NextFocusUpgrade * 2;
                    foreach (Character c in provider.GameState.Characters)
                    {
                        if (c.Team == team)
                        {
                            c.FocusModifier = focusModifier;
                            c.FocusGenRate *= focusModifier;
                        }
                    }
                }
            }
        }

        public void UpgradeDamage(Character buyer)
        {
            if (buyer.CrystalCount >= 5)
            {
                buyer.CrystalCount -= 5;
                DamagePool += 5;
                if (DamagePool >= NextDamageUpgrade)
                {
                    DamageModifier += 0.05f;
                    NextDamageUpgrade += NextDamageUpgrade * 2;
                    foreach (Character c in provider.GameState.Characters)
                    {
                        if (c.Team == team)
                        {
                            c.DamageModifier = damageModifier;
                        }
                    }
                }
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

		#endregion

		#region Networking Stuff

		public override void WriteToPacket( NetOutgoingMessage packet )
        {
			packet.Write( healthModifier );
			packet.Write( focusModifier );
			packet.Write( damageModifier );
			packet.Write( healthPool );
			packet.Write( focusPool );
			packet.Write( damagePool );
			packet.Write( nextHealthUpgrade );
			packet.Write( nextFocusUpgrade );
			packet.Write( nextDamageUpgrade );
        }

        public override void UpdateFromPacket( NetIncomingMessage packet )
        {
			healthModifier = packet.ReadFloat();
			focusModifier = packet.ReadFloat();
			damageModifier = packet.ReadFloat();
			healthPool = packet.ReadInt32();
			focusPool = packet.ReadInt32();
			damagePool = packet.ReadInt32();
			nextHealthUpgrade = packet.ReadInt32();
			nextFocusUpgrade = packet.ReadInt32();
			nextDamageUpgrade = packet.ReadInt32();
		}

		#endregion
	}
}
