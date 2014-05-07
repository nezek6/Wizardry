using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace WizardryShared
{
	public partial class Character
	{

		private struct CharDetails
		{
			public readonly float maxHealth;

			public readonly int speed;

			public readonly int[] spells;

			public CharDetails
				(
					float maxHealth,
					int speed,
					int[] spells
				)
			{
				this.maxHealth = maxHealth;
				this.speed = speed;
				this.spells = spells;
			}
		}


		static readonly IList<CharDetails> Details = new ReadOnlyCollection<CharDetails>(new[]{
			
			// Char ID 0 - FireWizard
			new CharDetails(
				100,
				4,
				new[]{0,3,10}
				),

			// Char ID 1 - Priest
			new CharDetails(
				100,
				4,
				new[]{1,5,18}
				),

			// Char ID 2 - WindLasher
			new CharDetails(
				100,
				4,
				new[]{2,15,13}
				),
			
				  // Char ID 3 - BeastMistress
			new CharDetails(
				100,
				4,
				new[]{7,8,11}
				),

		});
	}
}
