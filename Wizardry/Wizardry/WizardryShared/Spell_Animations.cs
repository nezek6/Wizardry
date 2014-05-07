using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
	public partial class Spell
	{
		static IList<ReturnAnimation> spellAnimations;

		private delegate Animation ReturnAnimation();

		public static void LoadContent( TextureProvider provider )
		{
			spellAnimations = new List<ReturnAnimation>(new ReturnAnimation[]{
			
				// Spell ID 0 - Fireball
				() => {return new Animation(provider.GetTexture("Spells/fireball"), 1, 90, 1.50f, false);},

				// Spell ID 1 - Holy Bolt
				() => {return new Animation(provider.GetTexture("Spells/holybolt"), 1, 90, 1.25f, false);},

				// Spell ID 2 - WindLash
				() => {return new Animation(provider.GetTexture("Spells/windlash3"), 1, 90, 1f, false);},

				// Spell ID 3 - Explosion Field
				() => {return new Animation(provider.GetTexture("Spells/explosion"), 12, 90, 0.0f, false);},

				// Spell ID 4 - Explosion
				() => {return new Animation(provider.GetTexture("Spells/explosion"), 12, 90, 0.8f, false);},

				// Spell ID 5 - Holy Nova
				() => {return new Animation(provider.GetTexture("Spells/explosion"), 12, 90, 0.0f, false);},

				// Spell ID 6 - Holy Nova Bolt
				() => {return new Animation(provider.GetTexture("Spells/holybolt"), 1, 90, 1.25f, false);},

				// Spell ID 7 - Claw
				() => {return new Animation(provider.GetTexture("Spells/claw2"), 4, 75, 0.5f, false);},

				// Spell ID 8 - Stampede
				() => {return new Animation(provider.GetTexture("Spells/claw"), 4, 150, 0f, false);},

				// Spell ID 9 - Stampede Unit
				() => {return new Animation(provider.GetTexture("Spells/stampede"), 3, 90, 2f, false);},

                // Spell ID 10 - Fire Shift
				() => {return new Animation(provider.GetTexture("Spells/fireball"), 1, 90, 2f, false);},

                // Spell ID 11 - Claw Leap
				() => {return new Animation(provider.GetTexture("Spells/clawleap"), 1, 90, 1f, false);},

                // Spell ID 12 - Holy Wings
				() => {return new Animation(provider.GetTexture("Spells/holywings"), 1, 90, 1f, false);},

                // Spell ID 13 - Wind Spin
				() => {return new Animation(provider.GetTexture("Spells/windspin"), 4, 50, 2f, false);},

                // Spell ID 14 - Tornado
				() => {return new Animation(provider.GetTexture("Spells/tornado"), 6, 30, 0.85f, false);},

                // Spell ID 15 - Ninja Storm
				() => {return new Animation(provider.GetTexture("Spells/wlulti"), 10, 225, 2f, false);},

				// Spell ID 16 - Ninja Storm Spin
				() => {return new Animation(provider.GetTexture("Spells/windspin"), 4, 50, 2f, false);},

				// Spell ID 17 - Ninja Storm Tornado
				() => {return new Animation(provider.GetTexture("Spells/tornado2"), 6, 30, 2.7f, false);},

				// Spell ID 18 - Holy Shield
				() => {return new Animation(provider.GetTexture("Spells/holyshield"), 1, 30, 0.4f, false);},

				// Spell ID 19 - Respawner
				() => {return new Animation(provider.GetTexture("Spells/explosion"), 12, 90, 0.0f, false);},
			});

		}

	}
}
