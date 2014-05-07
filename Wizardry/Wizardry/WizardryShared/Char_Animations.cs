using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizardryShared
{
	public partial class Character
	{
		private delegate Dictionary<Anim, Animation> ReturnAnimationDic();

		static IList<ReturnAnimationDic> charAnimations;

		public static void LoadContent(TextureProvider provider)
		{
			charAnimations = new List<ReturnAnimationDic>(new ReturnAnimationDic[]{
			
				// ID 0 - Fire Wizard
				() => { return new Dictionary<Anim, Animation> 
					{
						{Anim.WALKFRONT, new Animation(provider.GetTexture("Characters/FireWizard/walkfront"), 6, 90, 2f, false)},

						{Anim.WALKBACK, new Animation(provider.GetTexture("Characters/FireWizard/walkback"), 6, 90, 2f, false)},

						{Anim.WALKLEFT, new Animation(provider.GetTexture("Characters/FireWizard/walkside"), 6, 90, 2f, false)},

						{Anim.WALKRIGHT, new Animation(provider.GetTexture("Characters/FireWizard/walkside"), 6, 90, 2f, true)},

						{Anim.ATTACKFRONT, new Animation(provider.GetTexture("Characters/FireWizard/attackfront"), 1, 90, 2f, false)},

						{Anim.ATTACKBACK, new Animation(provider.GetTexture("Characters/FireWizard/attackback"), 1, 90, 2f, false)},

						{Anim.ATTACKLEFT, new Animation(provider.GetTexture("Characters/FireWizard/attackside"), 1, 90, 2f, false)},

						{Anim.ATTACKRIGHT, new Animation(provider.GetTexture("Characters/FireWizard/attackside"), 1, 90, 2f, true)},

						{Anim.STANDFRONT, new Animation(provider.GetTexture("Characters/FireWizard/standfront"), 1, 90, 2f, false)},

						{Anim.STANDBACK, new Animation(provider.GetTexture("Characters/FireWizard/standback"), 1, 90, 2f, false)},

						{Anim.STANDLEFT, new Animation(provider.GetTexture("Characters/FireWizard/standside"), 1, 90, 2f, false)},

						{Anim.STANDRIGHT, new Animation(provider.GetTexture("Characters/FireWizard/standside"), 1, 90, 2f, true)},

						{Anim.CASTFRONT, new Animation(provider.GetTexture("Characters/FireWizard/castfront"), 4, 90, 2f, false)},

						{Anim.CASTBACK, new Animation(provider.GetTexture("Characters/FireWizard/castfront"), 4, 90, 2f, false)},

						{Anim.CASTLEFT, new Animation(provider.GetTexture("Characters/FireWizard/castfront"), 4, 90, 2f, false)},

						{Anim.CASTRIGHT, new Animation(provider.GetTexture("Characters/FireWizard/castfront"), 4, 90, 2f, true)},

						{Anim.SPECIAL, new Animation(provider.GetTexture("Characters/FireWizard/castfront"), 4, 90, 2f, false)},

					};

				},

				//ID: 1 - Priest
				() => { return new Dictionary<Anim, Animation> 
					{
						{Anim.WALKFRONT, new Animation(provider.GetTexture("Characters/Priest/walkfront"), 6, 90, 2f, false)},

						{Anim.WALKBACK, new Animation(provider.GetTexture("Characters/Priest/walkback"), 6, 90, 2f, false)},

						{Anim.WALKLEFT, new Animation(provider.GetTexture("Characters/Priest/walkside"), 6, 90, 2f, false)},

						{Anim.WALKRIGHT, new Animation(provider.GetTexture("Characters/Priest/walkside"), 6, 90, 2f, true)},

						{Anim.ATTACKFRONT, new Animation(provider.GetTexture("Characters/Priest/attackfront"), 2, 90, 2f, false)},

						{Anim.ATTACKBACK, new Animation(provider.GetTexture("Characters/Priest/attackback"), 2, 90, 2f, false)},

						{Anim.ATTACKLEFT, new Animation(provider.GetTexture("Characters/Priest/attackside"), 2, 90, 2f, false)},

						{Anim.ATTACKRIGHT, new Animation(provider.GetTexture("Characters/Priest/attackside"), 2, 90, 2f, true)},

						{Anim.STANDFRONT, new Animation(provider.GetTexture("Characters/Priest/standfront"), 1, 90, 2f, false)},

						{Anim.STANDBACK, new Animation(provider.GetTexture("Characters/Priest/standback"), 1, 90, 2f, false)},

						{Anim.STANDLEFT, new Animation(provider.GetTexture("Characters/Priest/standside"), 1, 90, 2f, false)},

						{Anim.STANDRIGHT, new Animation(provider.GetTexture("Characters/Priest/standside"), 1, 90, 2f, true)},

						{Anim.CASTFRONT, new Animation(provider.GetTexture("Characters/Priest/castfront"), 2, 90, 2f, false)},

						{Anim.CASTBACK, new Animation(provider.GetTexture("Characters/Priest/castback"), 2, 90, 2f, false)},

						{Anim.CASTLEFT, new Animation(provider.GetTexture("Characters/Priest/castside"), 2, 90, 2f, false)},

						{Anim.CASTRIGHT, new Animation(provider.GetTexture("Characters/Priest/castside"), 2, 90, 2f, true)},

						{Anim.SPECIAL, new Animation(provider.GetTexture("Characters/Priest/castfront"), 2, 90, 2f, false)},
					};

				},

				//ID:2 - Wind Lasher
				() => { return new Dictionary<Anim, Animation> 
					{
						{Anim.WALKFRONT, new Animation(provider.GetTexture("Characters/WindLasher/walkfront"), 6, 90, 2f, false)},

						{Anim.WALKBACK, new Animation(provider.GetTexture("Characters/WindLasher/walkback"), 6, 90, 2f, false)},

						{Anim.WALKLEFT, new Animation(provider.GetTexture("Characters/WindLasher/walkside"), 6, 90, 2f, false)},

						{Anim.WALKRIGHT, new Animation(provider.GetTexture("Characters/WindLasher/walkside"), 6, 90, 2f, true)},

						{Anim.ATTACKFRONT, new Animation(provider.GetTexture("Characters/WindLasher/attackfront"), 5, 90, 2f, false)},

						{Anim.ATTACKBACK, new Animation(provider.GetTexture("Characters/WindLasher/attackback"), 5, 90, 2f, false)},

						{Anim.ATTACKLEFT, new Animation(provider.GetTexture("Characters/WindLasher/attackside"), 5, 90, 2f, false)},

						{Anim.ATTACKRIGHT, new Animation(provider.GetTexture("Characters/WindLasher/attackside"), 5, 90, 2f, true)},

						{Anim.STANDFRONT, new Animation(provider.GetTexture("Characters/WindLasher/standfront"), 1, 90, 2f, false)},

						{Anim.STANDBACK, new Animation(provider.GetTexture("Characters/WindLasher/standback"), 1, 90, 2f, false)},

						{Anim.STANDLEFT, new Animation(provider.GetTexture("Characters/WindLasher/standside"), 1, 90, 2f, false)},

						{Anim.STANDRIGHT, new Animation(provider.GetTexture("Characters/WindLasher/standside"), 1, 90, 2f, true)},

						{Anim.CASTFRONT, new Animation(provider.GetTexture("Characters/WindLasher/attackfront"), 5, 90, 2f, false)},

						{Anim.CASTBACK, new Animation(provider.GetTexture("Characters/WindLasher/attackback"), 5, 90, 2f, false)},

						{Anim.CASTLEFT, new Animation(provider.GetTexture("Characters/WindLasher/attackside"), 5, 90, 2f, false)},

						{Anim.CASTRIGHT, new Animation(provider.GetTexture("Characters/WindLasher/attackside"), 5, 90, 2f, true)},

                        {Anim.SPECIAL, new Animation(provider.GetTexture("Characters/WindLasher/attackfront"), 5, 90, 2f, false)},
					};

				},

				//ID 3 - Beast Mistress
				() => { return new Dictionary<Anim, Animation> 
					{
						{Anim.WALKFRONT, new Animation(provider.GetTexture("Characters/BeastMistress/walkfront"), 4, 90, 2f, false)},

						{Anim.WALKBACK, new Animation(provider.GetTexture("Characters/BeastMistress/walkback"), 4, 90, 2f, false)},

						{Anim.WALKLEFT, new Animation(provider.GetTexture("Characters/BeastMistress/walkside"), 4, 90, 2f, false)},

						{Anim.WALKRIGHT, new Animation(provider.GetTexture("Characters/BeastMistress/walkside"), 4, 90, 2f, true)},

						{Anim.ATTACKFRONT, new Animation(provider.GetTexture("Characters/BeastMistress/attackfront"), 4, 180, 2f, false)},

						{Anim.ATTACKBACK, new Animation(provider.GetTexture("Characters/BeastMistress/attackback"), 4, 180, 2f, false)},

						{Anim.ATTACKLEFT, new Animation(provider.GetTexture("Characters/BeastMistress/attackside"), 4, 180, 2f, false)},

						{Anim.ATTACKRIGHT, new Animation(provider.GetTexture("Characters/BeastMistress/attackside"), 4, 180, 2f, true)},

						{Anim.STANDFRONT, new Animation(provider.GetTexture("Characters/BeastMistress/standfront"), 1, 90, 2f, false)},

						{Anim.STANDBACK, new Animation(provider.GetTexture("Characters/BeastMistress/standback"), 1, 90, 2f, false)},

						{Anim.STANDLEFT, new Animation(provider.GetTexture("Characters/BeastMistress/standside"), 1, 90, 2f, false)},

						{Anim.STANDRIGHT, new Animation(provider.GetTexture("Characters/BeastMistress/standside"), 1, 90, 2f, true)},

						 {Anim.CASTFRONT, new Animation(provider.GetTexture("Characters/BeastMistress/castfront"), 2, 90, 2f, false)},

						{Anim.CASTBACK, new Animation(provider.GetTexture("Characters/BeastMistress/castback"), 2, 90, 2f, false)},

						{Anim.CASTLEFT, new Animation(provider.GetTexture("Characters/BeastMistress/castside"), 2, 90, 2f, false)},

						{Anim.CASTRIGHT, new Animation(provider.GetTexture("Characters/BeastMistress/castside"), 2, 90, 2f, true)},

						{Anim.SPECIAL, new Animation(provider.GetTexture("Characters/BeastMistress/special"), 2, 90, 2f, false)},
					};

				},
		  });

		}

	}
}
