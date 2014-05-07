using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
	public partial class PickUp
	{
		static IList<ReturnAnimation> PickUpAnimations;

		private delegate Animation ReturnAnimation();

		public static void LoadContent(TextureProvider provider)
		{
			PickUpAnimations = new List<ReturnAnimation>(new ReturnAnimation[]{
			
				// PickUp ID 0 - Fireball
				() => {return new Animation(provider.GetTexture("PickUps/crystal"), 7, 50, 0.3f, false);},

			});

		}

	}
}
