using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;
using WizardryShared;

namespace WizardryServer
{
	public class GameLobby : ResourceProvider
	{
		#region Resource Provider Requirements

		#region Data Members

		private IsoTileMap tilemap;

		private Game game;

		private Camera camera;

		private GameState gameState;

		#endregion

		#region Properties

		public IsoTileMap TileMap
		{
			get { return tilemap; }
			set { tilemap = value; }
		}

		public Game Game
		{
			get { return game; }
			set { game = value; }
		}

		public Camera Camera
		{
			get { return camera; }
			set { camera = value; }
		}

		public SpriteBatch SpriteBatch
		{
			get { return null; }
			set { /* Do Nothing */ }
		}

		public TextureProvider TextureProvider
		{
			get { return ((WizardryGameServer)Game).TextureProvider; }
		}

		public GameState GameState
		{
			get { return gameState; }
			set { gameState = value; }
		}

		#endregion

		private bool flipFlop = true;
		public void Shoot( int spellID, Vector2 startPoint, Vector2 clickPosition, Vector2 direction, float rotation, Character caster )
		{
			// Security Check
			if ( gameState == null )
			{
				return;
			}

			int nextShot = -1;	// Will hold the index of the next available spell slot
			if ( flipFlop )
			{
				// Get the next available spell starting from the front end of the pool
				for ( int i = 0; i < GameSettings.MAX_SPELLS; ++i )
				{
					if ( !GameState.Spells[i].Active )
					{
						nextShot = i;
						break;
					}
				}
			}
			else
			{
				// Get the next spell starting from the back end of the pool
				for ( int i = GameSettings.MAX_SPELLS - 1; i >= 0; --i )
				{
					if ( !GameState.Spells[i].Active )
					{
						nextShot = i;
						break;
					}
				}
			}
			flipFlop = !flipFlop;

			if ( nextShot >= 0 )
			{
				GameState.Spells[nextShot].StartPoint = startPoint;
				GameState.Spells[nextShot].ClickPosition = clickPosition;
				GameState.Spells[nextShot].Direction = direction;
				GameState.Spells[nextShot].Rotation = rotation;
				GameState.Spells[nextShot].Caster = caster;
				GameState.Spells[nextShot].Initialize( spellID );
				GameState.Spells[nextShot].Active = true;
			}
			
		}

		public bool CheckCollision( Sprite sprite1, Sprite sprite2, bool pixelPerfect )
		{
			Animation anim1 = sprite1.CurrentAnimation;
			Animation anim2 = sprite2.CurrentAnimation;

			Rectangle box1 = sprite1.BoundingBox;
			Rectangle box2 = sprite2.BoundingBox;

			if ( !box1.Intersects( box2 ) )
			{
				return false;
			}
			else if ( !pixelPerfect )
			{
				return true;
			}

			Matrix mat1 = Matrix.CreateTranslation( -anim1.FrameWidth / 2, -anim1.FrameHeight, 0 ) * Matrix.CreateRotationZ( anim1.Rotation )
				* Matrix.CreateScale( anim1.Scale ) * Matrix.CreateTranslation( sprite1.WorldPosition.X, sprite1.WorldPosition.Y, 0 );

			Matrix mat2 = Matrix.CreateTranslation( -anim2.FrameWidth / 2, -anim2.FrameHeight / 2, 0 ) * Matrix.CreateRotationZ( anim2.Rotation )
				* Matrix.CreateScale( anim2.Scale ) * Matrix.CreateTranslation( sprite2.WorldPosition.X, sprite2.WorldPosition.Y, 0 );

			Color[,] tex1 = anim1.ColorArray;
			Color[,] tex2 = anim2.ColorArray;

			//Matrix which expresses the coordinates of the first textures in terms of the second texture's local coordinate
			Matrix mat1to2 = mat1 * Matrix.Invert( mat2 );

			int width1 = anim1.FrameWidth;
			int height1 = anim1.FrameHeight;
			int width2 = anim2.FrameWidth;
			int height2 = anim2.FrameHeight;

			int frameOffset1 = anim1.CurrentFrame * width1;
			int frameOffset2 = anim2.CurrentFrame * width2;

			//Check if any non-transparent pixel of the first texture overlaps with non-transparent pixel of the second texture
			for ( int x1 = frameOffset1; x1 < frameOffset1 + width1; x1++ )
			{
				for ( int y1 = 0; y1 < height1; y1++ )
				{
					Vector2 pos1 = new Vector2( x1 - frameOffset1, y1 );
					Vector2 pos2 = Vector2.Transform( pos1, mat1to2 );

					int x2 = (int)pos2.X + frameOffset2;
					int y2 = (int)pos2.Y;
					if ( ( x2 >= 0 ) && ( x2 < width2 ) )
					{
						if ( ( y2 >= 0 ) && ( y2 < height2 ) )
						{
							if ( tex1[x1, y1].A > 0 )
							{
								if ( tex2[x2, y2].A > 0 )
								{
									//There must be an overlap, return the position of the collision
									Vector2 screenPos = Vector2.Transform( pos1, mat1 );
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		#endregion

		#region Data Members

		/* The number of times to update per second. */
		public const int FPS = 60;
		public const double FRAME_TIME = 1f / FPS;

		/* Reference to the game server. */
		private WizardryGameServer wizServer;

		/* The lobby's name, as shown in the available lobbies list. */
		private string name = "NewLobby";

		/* The maximum number of players that can be held in the lobby. */
		private int maxPlayers = GameSettings.MAX_LOBBY_CAPACITY;

		/* Duration of a match (in minutes). */
		private double matchDuration = GameSettings.DEFAULT_MP_MATCH_DURATION;

		/* Tracks the time of the last game update to control update framerate. */
		private DateTime prevUpdateTime;

		/* Message queue used by the server to forward messages to lobbies. */
		/* Left public as an optimization to speed up access time (less indirection). */
		public ConcurrentQueue<NetIncomingMessage> messageQueue;

		/* The lobby's ID. */
		private int myID;

		/* The list of players connected to the server. */
		private int numConnected;
		private NetConnection[] players;
		private object playerListLock = new object();

		/* Whether or not the game has started. */
		private bool gameStarted = false;

		/* Ready flags for all the players. */
		private bool[] readyFlags;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Lobby's name, as shown in the available lobbies list.
		/// </summary>
		public string Name
		{
			get { return name; }
			set 
			{
				if ( value.Length <= GameSettings.MAX_LOBBY_NAME_LEN )
				{
					name = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of players that the lobby can hold.
		/// </summary>
		public int MaxPlayers
		{
			get { return maxPlayers; }
			set
			{
				maxPlayers = (int)MathHelper.Clamp( value, 1, GameSettings.MAX_LOBBY_CAPACITY );
			}
		}

		/// <summary>
		/// Gets the number of players currently in the lobby.
		/// </summary>
		public int NumPlayers
		{
			get { return numConnected; }
		}

		/// <summary>
		/// Duration of a match in minutes.
		/// </summary>
		public double MatchDuration
		{
			get { return matchDuration; }
			set { matchDuration = value; }
		}

		/// <summary>
		/// Gets whether or not the actual game has started.
		/// </summary>
		public bool GameStarted
		{
			get { return gameStarted; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Default constructor for GameLobby objects.
		/// </summary>
		/// <param name="wizServer">A reference to the Game Server object.</param>
		public GameLobby( WizardryGameServer wizServer, int id )
		{
			messageQueue = new ConcurrentQueue<NetIncomingMessage>();

			this.game = wizServer;
			this.wizServer = wizServer;
			myID = id;

			numConnected = 0;
			players = new NetConnection[GameSettings.MAX_LOBBY_CAPACITY];
			for ( int i = 0; i < GameSettings.MAX_LOBBY_CAPACITY; ++i )
			{
				players[i] = null;
			}
			gameState = new GameState( this );

			camera = new TileEngine.Camera( wizServer );
			tilemap = new IsoTileMap( camera );
			tilemap.LoadFromFile( wizServer.Content, "Content/Maps/MPMap.map" );

			prevUpdateTime = DateTime.MinValue;
		}

		/// <summary>
		/// Runnable method that is the entry point for the lobby thread.
		/// </summary>
		public void Run()
		{
			wizServer.WriteConsoleMessage( "Starting New Lobby: " + Name + " (" + MaxPlayers + " players)" );

			// Create the ready flags
			readyFlags = new bool[maxPlayers];
			for ( int i = 0; i < readyFlags.Length; ++i )
			{
				readyFlags[i] = false;
			}

			while ( true )
			{
				// The current Frame's time
				DateTime curTime = DateTime.Now;
				TimeSpan timeDiff;

				// Process next packet
				NetIncomingMessage msg;
				if ( messageQueue.TryDequeue( out msg ) )
				{
					HandlePacket( msg );
				}

				// If the game isn't started yet, skip the game loop
				if ( !gameStarted )
				{
					// See if everyone's ready yet
					int numReady = 0;
					for ( int i = 0; i < readyFlags.Length; ++i )
					{
						if ( readyFlags[i] )
						{
							++numReady;
						}
					}

					if ( numReady == MaxPlayers )
					{
						// All the players are accounted for and ready
						SetupGame();
						gameStarted = true;
					}
					else
					{
						// Some players are not ready yet
						// Check if it's time to send a gamestate packet
						timeDiff = curTime - prevUpdateTime;
						if ( timeDiff >= TimeSpan.FromSeconds( FRAME_TIME ) )
						{
							// Broadcast the gamestate to all connected players
							NetOutgoingMessage gsUpdateMsg = wizServer.Server.CreateMessage();
							gsUpdateMsg.Write( (byte)MsgType.GSUpdate );
							gameState.WriteToPacket( gsUpdateMsg );
							BroadcastPacket( gsUpdateMsg );

							// Record this frame's update time
							prevUpdateTime = curTime;
						}

						continue;
					}
				}

				// Check if it's time to update the game
				timeDiff = curTime - prevUpdateTime;
				if ( timeDiff >= TimeSpan.FromSeconds( FRAME_TIME ) )
				{
					// Tick the clock
					gameState.RoundClock = gameState.RoundClock - timeDiff;

					// If the clock has expired, end the game!
					if ( gameState.RoundClock <= TimeSpan.Zero )
					{
						break;
					}

					// Update Spells
					foreach ( Spell spell in gameState.Spells )
					{
						if ( spell.Active )
						{
							spell.Update( timeDiff );
						}
					}

					// Update pickups
					foreach ( PickUp pickup in GameState.Pickups )
					{
						if ( pickup.Active )
						{
							pickup.Update( timeDiff );
						}
					}

					// Process Collisions
					foreach ( Character character in GameState.Characters )
					{
						if ( !character.Active )
						{
							continue;
						}

						foreach ( Spell spell in GameState.Spells )
						{
							if ( spell.Active && (spell.Caster != character) )
							{
								if ( CheckCollision( character, spell, true ) )
								{
									spell.Hit( timeDiff, character );
								}
							}
						}

						foreach ( PickUp pickup in GameState.Pickups )
						{
							if ( pickup.Active && pickup.CurrentStatus == PickUp.PickUpStatus.SPAWNED )
							{
								if ( CheckCollision( character, pickup, false ) )
								{
									pickup.OnHit( timeDiff, pickup, character );
								}
							}
						}

					}

					// Check for deaths
					foreach ( Character character in GameState.Characters )
					{
						if ( character.Active && 
							 character.CurrentHealth <= 0 && 
							 character.Status != Character.CharStatus.DEAD )
						{
							// Character died
							character.Status = Character.CharStatus.DEAD;
							
							if ( character.Team == Team.RED )
							{
								GameState.Scores[(int)Team.BLUE]++;
							}
							else if ( character.Team == Team.BLUE )
							{
								GameState.Scores[(int)Team.RED]++;
							}

							// Cast a respawner
							Shoot( 19, character.WorldPosition, Vector2.Zero, Vector2.Zero, 0, character );
						}
					}

					// Broadcast the gamestate to all connected players
					NetOutgoingMessage gsUpdateMsg = wizServer.Server.CreateMessage();
					gsUpdateMsg.Write( (byte)MsgType.GSUpdate );
					gameState.WriteToPacket( gsUpdateMsg );
					BroadcastPacket( gsUpdateMsg );

					// Record this frame's update time
					prevUpdateTime = curTime;
				}

				if ( messageQueue.Count == 0 )
				{
					// Done processing messages for now, don't hog the processor with a busy-loop
					Thread.Sleep( 1 );
				}
			}

			wizServer.WriteConsoleMessage( "Lobby <" + Name + ">'s game ended." );
			EndGame();
			wizServer.CloseLobby( myID );
		}

		public int AddPlayer( NetConnection connection, string playerName )
		{
			lock ( playerListLock )
			{
				for ( int i = 0; i < MaxPlayers; ++i )
				{
					if ( players[i] == null )
					{
						++numConnected;
						players[i] = connection;
						gameState.Characters[i].Initialize( (NetRandom.Instance.NextInt() % 4), new Vector2( 350, 350 ) );
						gameState.Characters[i].Name = playerName;

						// Choose which team to add the player to
						int redCount = 0;
						int blueCount = 0;
						for ( int j = 0; j < MaxPlayers; ++j )
						{
							if ( gameState.Characters[j].Active && j != i )
							{
								if ( gameState.Characters[j].Team == Team.RED )
								{
									++redCount;
								}
								else if ( gameState.Characters[j].Team == Team.BLUE )
								{
									++blueCount;
								}
							}
						}
						if ( blueCount < redCount )
						{
							gameState.Characters[i].Team = Team.BLUE;
						}
						else
						{
							gameState.Characters[i].Team = Team.RED;
						}

						// Send the player the "Joined Lobby" confirmation
						NetOutgoingMessage msg = wizServer.Server.CreateMessage();
						msg.Write( (byte)MsgType.InfoJoinedLobby );
						msg.Write( (byte)i );
						msg.Write( Name );
						gameState.WriteToPacket( msg );
						wizServer.Server.SendMessage( msg, connection, NetDeliveryMethod.ReliableOrdered );
						string toCon = NetUtility.ToHexString( connection.RemoteUniqueIdentifier ) + " has joined lobby " + Name;
						wizServer.WriteConsoleMessage( toCon );
						return i;
					}
				}
			}
			return -1;
		}

		public void RemovePlayer( NetConnection player )
		{
			lock ( playerListLock )
			{
				for ( int i = 0; i < MaxPlayers; ++i )
				{
					if ( players[i] != null && players[i].RemoteUniqueIdentifier == player.RemoteUniqueIdentifier )
					{
						string toCon = NetUtility.ToHexString( player.RemoteUniqueIdentifier ) + " has left lobby " + Name;
						wizServer.WriteConsoleMessage( toCon );
						gameState.Characters[i].Active = false;
						players[i] = null;
						readyFlags[i] = false;
						--numConnected;
					}
				}
			}
		}

		private void HandlePacket( NetIncomingMessage packet )
		{
			MsgType type = (MsgType)packet.ReadByte();
			switch ( type )
			{
				case MsgType.CharUpdate:
					ApplyPlayerUpdate( packet );
					break;
				case MsgType.ReqSpellCast:
					int spellID = packet.ReadInt32();
					Vector2 start = Vector2.Zero;
					start.X = packet.ReadFloat();
					start.Y = packet.ReadFloat();
                    Vector2 click = Vector2.Zero;
                    click.X = packet.ReadFloat();
					click.Y = packet.ReadFloat();
					Vector2 dir = Vector2.Zero;
					dir.X = packet.ReadFloat();
					dir.Y = packet.ReadFloat();
					float angle = packet.ReadFloat();
					Shoot( spellID, start, click, dir, angle, RUIDToCharacter( packet.SenderConnection.RemoteUniqueIdentifier ) );
					break;
				case MsgType.ReqClassChange:
					int roleID = packet.ReadInt32();
					(RUIDToCharacter( packet.SenderConnection.RemoteUniqueIdentifier )).RoleID = roleID;
					break;
				case MsgType.ReqTeamChange:
					Team team = (Team)packet.ReadInt32();
					(RUIDToCharacter( packet.SenderConnection.RemoteUniqueIdentifier )).Team = team;
					break;
				case MsgType.ReqReady:
					bool ready = packet.ReadBoolean();
					readyFlags[RUIDToPlayerIndex( packet.SenderConnection.RemoteUniqueIdentifier )] = ready;
					break;
				case MsgType.ReqUpgrade:
					int charID = packet.ReadByte();
					int upgrade = packet.ReadByte();
					Character c = GameState.Characters[charID];
					switch ( upgrade )
					{
						case 0:
							GameState.Towers[(int)c.Team].UpgradeHealth( c );
							break;
						case 1:
							GameState.Towers[(int)c.Team].UpgradeFocus( c );
							break;
						case 2:
							GameState.Towers[(int)c.Team].UpgradeDamage( c );
							break;
					}
					break;
			}
		}

		private void ApplyPlayerUpdate( NetIncomingMessage packet )
		{
			int index = packet.ReadByte();
			Vector2 playerPos = Vector2.Zero;
			playerPos.X = packet.ReadFloat();
			playerPos.Y = packet.ReadFloat();
			GameState.Characters[index].WorldPosition = playerPos;
			GameState.Characters[index].SetCurrentAnimation( (Sprite.Anim)packet.ReadInt32() );
			GameState.Characters[index].CurrentFocus = packet.ReadFloat();
		}

		private void SetupGame()
		{
			// Set the clock
			gameState.RoundClock = TimeSpan.FromMinutes( MatchDuration );

			// Setup 100 crystals
			for ( int i = 0; i < 100; ++i )
			{
				GameState.Pickups[i].Initialize( 0 );
			}

			// Send game start signals
			NetOutgoingMessage msg = wizServer.Server.CreateMessage();
			msg.Write( (byte)MsgType.InfoGameStart );
			BroadcastPacket( msg );
			wizServer.WriteConsoleMessage( "Starting the game." );
		}

		private void EndGame()
		{
			int redIndex = (int)Team.RED;
			int blueIndex = (int)Team.BLUE;

			int winningTeam = -1;

			// Determine who won the game
			if ( gameState.Scores[redIndex] > gameState.Scores[blueIndex] )
			{
				winningTeam = redIndex;
			}
			else if ( gameState.Scores[redIndex] < gameState.Scores[blueIndex] )
			{
				winningTeam = blueIndex;
			}

			// Send the GameOver signal to all clients
			NetOutgoingMessage msg = wizServer.Server.CreateMessage();
			msg.Write( (byte)MsgType.InfoGameOver );
			msg.Write( winningTeam );
			BroadcastPacket( msg );
		}

		private void BroadcastPacket( NetOutgoingMessage packet )
		{
			lock ( playerListLock )
			{
				if ( numConnected > 0 )
				{
					List<NetConnection> allPlayers = new List<NetConnection>();
					foreach ( NetConnection player in players )
					{
						if ( player != null )
						{
							allPlayers.Add( player );
						}
					}
					wizServer.Server.SendMessage( packet, allPlayers, NetDeliveryMethod.ReliableOrdered, 0 );
				}
			}
		}

		private Character RUIDToCharacter( long ruid )
		{
			lock ( playerListLock )
			{
				for ( int i = 0; i < players.Length; ++i )
				{
					if ( players[i] != null && players[i].RemoteUniqueIdentifier == ruid )
					{
						return GameState.Characters[i];
					}
				}
			}

			return null;
		}

		private int RUIDToPlayerIndex( long ruid )
		{
			lock ( playerListLock )
			{
				for ( int i = 0; i < players.Length; ++i )
				{
					if ( players[i] != null && players[i].RemoteUniqueIdentifier == ruid )
					{
						return i;
					}
				}
			}

			return -1;
		}

		#endregion
	}
}
