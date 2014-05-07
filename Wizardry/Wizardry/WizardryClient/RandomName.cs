using System;

namespace WizardryClient
{
	class RandomName
	{
		private const string CONSONANTS = "bcdfghjklmnpqrstvwxz";

		private const string VOWELS = "aeiouy";

		private const int SIZE = 5;

		private static Random rand = new Random();

		public static string Generate()
		{
			string name = "";

			for ( int i = 0; i < SIZE; ++i )
			{
				if ( (i & 1) == 0 )
				{
					//Even
					name += CONSONANTS[rand.Next( CONSONANTS.Length )];
				}
				else
				{
					//Odd
					name += VOWELS[rand.Next( VOWELS.Length )];
				}
			}

			return char.ToUpper( name[0] ) + name.Substring( 1 );
		}
	}
}
