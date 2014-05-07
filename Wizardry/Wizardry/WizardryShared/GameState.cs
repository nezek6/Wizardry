using System;
using Lidgren.Network;

namespace WizardryShared
{
	public class GameState
	{
		#region Data Members

		private Tower[] towers;

		private Character[] characters;

		private Spell[] spells;

		private PickUp[] pickups;

		private TimeSpan roundClock;

		private int[] scores;

		#endregion

		/// <summary>
		/// Creates a fresh gamestate with all game objects initialized and set to inactive.
		/// </summary>
		public GameState( ResourceProvider provider )
		{
			towers = new Tower[2];
			towers[0] = new Tower(Team.BLUE, provider);
			towers[1] = new Tower(Team.RED, provider);

			characters = new Character[GameSettings.MAX_LOBBY_CAPACITY];
			for ( int i = 0; i < GameSettings.MAX_LOBBY_CAPACITY; ++i )
			{
				characters[i] = new Character( provider );
			}

			spells = new Spell[GameSettings.MAX_SPELLS];
			for ( int i = 0; i < GameSettings.MAX_SPELLS; ++i )
			{
				spells[i] = new Spell( provider );
			}

			pickups = new PickUp[GameSettings.MAX_PICKUPS];
			for ( int i = 0; i < GameSettings.MAX_PICKUPS; ++i )
			{
				pickups[i] = new PickUp( provider );
			}

			roundClock = TimeSpan.Zero;

			scores = new int[2];
			for ( int i = 0; i < scores.Length; ++i )
			{
				scores[i] = 0;
			}
		}

		#region Properties

		public Tower[] Towers
		{
			get { return towers; }
		}

		public Character[] Characters
		{
			get { return characters; }
			set { characters = value; }
		}

		public Spell[] Spells
		{
			get { return spells; }
		}

		public PickUp[] Pickups
		{
			get { return pickups; }
		}

		public TimeSpan RoundClock
		{
			get { return roundClock; }
			set { roundClock = value; }
		}

		public int[] Scores
		{
			get { return scores; }
		}

		#endregion

		#region Networking Stuff

		public void WriteToPacket( NetOutgoingMessage packet )
		{
			// Write the towers to the packet
			for ( int i = 0; i < Towers.Length; ++i )
			{
				Towers[i].WriteToPacket( packet );
			}

			// Write the number of active character records
			int charCount = 0;
			for ( int i = 0; i < GameSettings.MAX_LOBBY_CAPACITY; ++i )
			{
				if ( characters[i].Active )
				{
					++charCount;
				}
			}
			packet.Write( (byte)charCount );

			// Write each active character to the packet
			for ( int i = 0; i < GameSettings.MAX_LOBBY_CAPACITY; ++i )
			{
				if ( characters[i].Active )
				{
					packet.Write( (byte)i );
					characters[i].WriteToPacket( packet );
				}
			}

			// Write the number of active spell records
			int spellCount = 0;
			for ( int i = 0; i < GameSettings.MAX_SPELLS; ++i )
			{
				if ( spells[i].Active )
				{
					++spellCount;
				}
			}
			packet.Write( (int)spellCount );

			// Write each active spell to the packet
			for ( int i = 0; i < GameSettings.MAX_SPELLS; ++i )
			{
				if ( spells[i].Active )
				{
					packet.Write( (int)i );
					spells[i].WriteToPacket( packet );
				}
			}

			// Write the number of active pickup records
			int pickupCount = 0;
			for ( int i = 0; i < GameSettings.MAX_PICKUPS; ++i )
			{
				if ( pickups[i].Active )
				{
					++pickupCount;
				}
			}
			packet.Write( (int)pickupCount );

			// Write each active pickup to the packet
			for ( int i = 0; i < GameSettings.MAX_PICKUPS; ++i )
			{
				if ( pickups[i].Active )
				{
					packet.Write( (int)i );
					pickups[i].WriteToPacket( packet );
				}
			}

			// Write the round clock to the packet
			packet.Write( roundClock.TotalMilliseconds );

			// Write the scores to the packet
			for ( int i = 0; i < scores.Length; ++i )
			{
				packet.Write( scores[i] );
			}
		}

		public void UpdateFromPacket( NetIncomingMessage packet )
		{
			// Update the towers
			for ( int i = 0; i < Towers.Length; ++i )
			{
				Towers[i].UpdateFromPacket( packet );
			}

			// Update the active characters
			int index = -1;
			int charCount = packet.ReadByte();
			for ( int i = 0, lastIndex = 0; i < charCount; ++i )
			{
				index = packet.ReadByte();
				
				// Set characters between active chars to inactive
				for ( int j = lastIndex ; j != index; ++j )
				{
					characters[j].Active = false;
				}
				characters[index].Active = true;
				characters[index].UpdateFromPacket( packet );
				lastIndex = index + 1;
			}

			// Set the rest of the characters (past the last active index) to inactive
			for ( int i = index + 1; i < characters.Length; ++i )
			{
				characters[i].Active = false;
			}


			// Update the active spells
			index = -1;
			int spellCount = packet.ReadInt32();
			for ( int i = 0, lastIndex = 0; i < spellCount; ++i )
			{
				index = packet.ReadInt32();
				for ( int j = lastIndex; j != index; ++j )
				{
					spells[j].Active = false;
				}
				spells[index].Active = true;
				spells[index].UpdateFromPacket( packet );
				lastIndex = index + 1;
			}

			// Set the rest of the spells (past the last active index) to inactive
			for ( int i = index + 1; i < spells.Length; ++i )
			{
				spells[i].Active = false;
			}

			// Update the active pickups
			index = -1;
			int pickupCount = packet.ReadInt32();
			for ( int i = 0, lastIndex = 0; i < pickupCount; ++i )
			{
				index = packet.ReadInt32();
				for ( int j = lastIndex; j != index; ++j )
				{
					pickups[j].Active = false;
				}
				pickups[index].Active = true;
				pickups[index].UpdateFromPacket( packet );
				lastIndex = index + 1;
			}

			// Set the rest of the pickups (past the last active index) to inactive
			for ( int i = index + 1; i < pickups.Length; ++i )
			{
				pickups[i].Active = false;
			}

			// Read the round clock from the packet
			roundClock = TimeSpan.FromMilliseconds( packet.ReadDouble() );

			// Read the scores
			for ( int i = 0; i < scores.Length; ++i )
			{
				scores[i] = packet.ReadInt32();
			}
		}

		#endregion


	}
}
