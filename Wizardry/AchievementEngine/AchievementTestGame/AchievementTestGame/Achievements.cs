using AchievementEngine;
using System;

namespace AchievementTestGame
{
	public class Achi1 : Achievement
	{
		public Achi1():base(
			"M-Maniac",
			"Press the M key 10 times."
			)
		{
		}

		public override void Initialize()
		{
			AchievementManager.Instance.RegisterForEvent( "MKeyPressed", this );
		}


		int count = 0;


		public override void EventNotify( string e, params object[] args )
		{
			Console.WriteLine( "Achi1 Notified" );
			count++;
			if ( count >= 10 )
			{
				this.completed = true;
				Console.WriteLine( "Achi1 Unlocked!" );
				AchievementManager.Instance.UnregisterFromEvent( "MKeyPressed", this );
			}
		}

	}

	public class Achi2 : Achievement
	{
		public Achi2():base(
			"\"Space\"-d Out!",
			"Press the spacebar 4 times."
			)
		{
		}

		public override void Initialize()
		{
			AchievementManager.Instance.RegisterForEvent( "SpaceBarPressed", this );
		}


		int count = 0;

		public override void EventNotify( string e, params object[] args )
		{
			Console.WriteLine( "Achi2 Notified" );
			count++;
			if ( count >= 4 )
			{
				this.completed = true;
				Console.WriteLine( "Achi2 Unlocked!" );
				AchievementManager.Instance.UnregisterFromEvent( "SpaceBarPressed", this );
			}
		}

	}

	public class Achi3 : Achievement
	{
		public Achi3():base(
			"Joe Damage-io",
			"Deal 1000 damage total."
			)
		{
		}

		public override void Initialize()
		{
			AchievementManager.Instance.RegisterForEvent( "DamageDealt", this );
		}

		int damage = 0;

		public override void EventNotify( string e, params object[] args )
		{
			Console.WriteLine( "Achi3 Notified" );
			damage += (int)args[0];
			if ( damage >= 1000 )
			{
				this.completed = true;
				Console.WriteLine( "Achi3 Unlocked!" );
				AchievementManager.Instance.UnregisterFromEvent( "DamageDealt", this );
			}
		}
	}
}
