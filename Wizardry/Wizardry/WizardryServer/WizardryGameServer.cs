using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using WizardryShared;

namespace WizardryServer
{
	public class WizardryGameServer : Microsoft.Xna.Framework.Game
	{
		// Lobby index indicating "no lobby" 
		private const int NO_LOBBY = -1;

		#region Data Members

		/* XNA GraphicsDevice manager used by the framework. */
		GraphicsDeviceManager graphics;
		
		/* Lidgren Server object enabling client-server communication. */
		NetServer server;

		/* List of lobbies running on the server. */
		GameLobby[] lobbies;
		public readonly object lobbyLock = new object();

		/* The TextureProvider used by sprites to load textures on demand. */
		TextureProvider textureProvider;

		/* Tracks the lobbies that connected players belong to. */
		ConcurrentDictionary<long, int> playerLobbies;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a reference to the NetServer communication object.
		/// </summary>
		public NetServer Server
		{
			get { return server; }
		}

		/// <summary>
		/// Gets a reference to the TextureProvider texture loading object.
		/// </summary>
		public TextureProvider TextureProvider
		{
			get { return textureProvider; }
		}

		#endregion

		/// <summary>
		/// Default constructor for the WizardryGameServer class.
		/// </summary>
		public WizardryGameServer()
		{
			graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
			textureProvider = new TextureProvider( Content );

			// Windows Settings for the XNA window
			Window.Title = "Wizardry Server";
			graphics.PreferredBackBufferWidth = 200;
			graphics.PreferredBackBufferHeight = 1;

			// Set up the lobbies list
			lobbies = new GameLobby[GameSettings.MAX_LOBBIES];
			for ( int i = 0; i < GameSettings.MAX_LOBBIES; ++i )
			{
				lobbies[i] = null;
			}

			playerLobbies = new ConcurrentDictionary<long, int>();

			// Setup the server configuration
			NetPeerConfiguration config = new NetPeerConfiguration( GameSettings.APP_NAME );
			config.Port = GameSettings.SERVER_PORT_NUM;
			config.EnableMessageType( NetIncomingMessageType.DiscoveryRequest );

			// Start the server
			server = new NetServer( config );
			server.Start();
			WriteConsoleMessage( "Server starting!" );

			// Start the Packet Receiver thread
			Thread packets = new Thread( new ThreadStart( this.PacketProcessor ) );
			packets.Start();
		}

		#region XNA Game Overrides

		protected override void LoadContent()
		{
			Spell.LoadContent( TextureProvider );
			Character.LoadContent( TextureProvider );
            PickUp.LoadContent(textureProvider);
			base.LoadContent();

			// Test Lobbies
			StartLobby( "Mini", 1 );
			lobbies[0].MatchDuration = 0.25;
			StartLobby( "Lobby1", 4 );
			StartLobby( "AWsum LobBy", 8 );
			lobbies[StartLobby( "Tiny", 2 )].MatchDuration = 5;
		}

		/// <summary>
		/// Overload of the XNA Game Update() method. Updates the Server every frame.
		/// </summary>
		/// <param name="gameTime"></param>
		protected override void Update( GameTime gameTime )
		{
			base.Update( gameTime );

			// Force draw to not be called this frame
			SuppressDraw();
			Thread.Sleep( 1 );
		}

		/// <summary>
		/// Override of the XNA Game Draw() method. This method MUST remain empty.
		/// </summary>
		protected override void Draw( GameTime gameTime )
		{
			// Do Nothing - The server application should _never_ draw!
		}

		/// <summary>
		/// This method is called automatically when the game is exiting.
		/// </summary>
		protected override void OnExiting( object sender, EventArgs args )
		{
			WriteConsoleMessage( "Server shutting down, goodnight...." );
			base.OnExiting( sender, args );
		}

		#endregion

		#region Methods

		/// <summary>
		/// Writes a timestamped message to the console output.
		/// </summary>
		/// <param name="msg">The message to display.</param>
		public void WriteConsoleMessage( string msg )
		{
			string timestamp = DateTime.Now.ToString( "[HH:mm:ss] " );
			Console.Out.WriteLine( timestamp + msg );
		}

		/// <summary>
		/// Starts a new game lobby.
		/// </summary>
		/// <param name="name">The name of the new lobby.</param>
		/// <param name="size">The maximum player capacity of the new lobby.</param>
		/// <returns>The index of the newly created lobby or -1 if not possible.</returns>
		private int StartLobby( string name, int size )
		{
			lock ( lobbyLock )
			{
				// Look for the next available lobby
				int index;
				for ( index = 0; index < GameSettings.MAX_LOBBIES && lobbies[index] != null; ++index )
				{
					continue;
				}

				// Can't start a new lobby
				if ( index >= GameSettings.MAX_LOBBIES )
				{
					return -1;
				}

				// Create the new lobby
				GameLobby newLobby = new GameLobby( this, index );
				newLobby.Name = name;
				newLobby.MaxPlayers = size;

				// Add it to the list
				lobbies[index] = newLobby;

				// Start it up!
				Thread lobbyThread = new Thread( new ThreadStart( newLobby.Run ) );
				lobbyThread.Start();

				return index;
			}
		}

		/// <summary>
		/// Closes a game lobby.
		/// </summary>
		/// <param name="lobbyID">The ID of the lobby to close.</param>
		public void CloseLobby( int lobbyID )
		{
			/*/ TODO: DELME----
			string n;
			int s;
			double d;
			n = lobbies[lobbyID].Name;
			s = lobbies[lobbyID].MaxPlayers;
			d = lobbies[lobbyID].MatchDuration;
			// -------------*/
			lock ( lobbyLock )
			{
				// Remove players from the lobby
				ICollection<long> keys = playerLobbies.Keys;
				foreach ( long key in keys )
				{
					playerLobbies.TryUpdate( key, -1, lobbyID );
				}

				// Delete the lobby
				lobbies[lobbyID] = null;
			}

			/*/ TODO: DELME------
			int id = StartLobby( n, s );
			lobbies[id].MatchDuration = d;
			//------------------*/
		}

		/// <summary>
		/// Separate thread that processes packets as fast as possible.
		/// </summary>
		private void PacketProcessor()
		{
			while ( true )
			{
				NetIncomingMessage msg;
				while ( ( msg = server.ReadMessage() ) != null )
				{
					switch ( msg.MessageType )
					{
						case NetIncomingMessageType.DiscoveryRequest:
							//
							// Server received a discovery request from a client; send a discovery response (with no extra data attached)
							//
							WriteConsoleMessage( "User Connected!" );
							server.SendDiscoveryResponse( null, msg.SenderEndpoint );
							break;
						case NetIncomingMessageType.VerboseDebugMessage:
						case NetIncomingMessageType.DebugMessage:
						case NetIncomingMessageType.WarningMessage:
						case NetIncomingMessageType.ErrorMessage:
							//
							// Just print diagnostic messages to console
							//
							WriteConsoleMessage( "<<DBG PACKET>>: " + msg.ReadString() );
							break;
						case NetIncomingMessageType.StatusChanged:
							//
							// Someone's status changed
							//
							NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
							if ( status == NetConnectionStatus.Connected )
							{
								WriteConsoleMessage( NetUtility.ToHexString( msg.SenderConnection.RemoteUniqueIdentifier ) + " connected!" );
								playerLobbies.TryAdd( msg.SenderConnection.RemoteUniqueIdentifier, NO_LOBBY );
							}
							else if ( status == NetConnectionStatus.Disconnected )
							{
								WriteConsoleMessage( NetUtility.ToHexString( msg.SenderConnection.RemoteUniqueIdentifier ) + " disconnected!" );
								if ( playerLobbies.ContainsKey( msg.SenderConnection.RemoteUniqueIdentifier ) )
								{
									int playerLobby = playerLobbies[msg.SenderConnection.RemoteUniqueIdentifier];
									if ( playerLobby != NO_LOBBY )
									{
										lock ( lobbyLock )
										{
											lobbies[playerLobby].RemovePlayer( msg.SenderConnection );
										}
									}
								}
								int oblivion;
								playerLobbies.TryRemove( msg.SenderConnection.RemoteUniqueIdentifier, out oblivion );
							}
							break;
						case NetIncomingMessageType.Data:
							//
							// The client sent data to the server
							//
							HandleDataPacket( msg );
							break;
					}
				}
				Thread.Sleep( 1 );
			}
		}

		/// <summary>
		/// Handles a single incoming data packet from a client.
		/// </summary>
		/// <param name="msg">The packet to handle.</param>
		private void HandleDataPacket( NetIncomingMessage msg )
		{
			MsgType type = (MsgType)msg.PeekByte();
			switch ( type )
			{
				case MsgType.Debug:
					msg.ReadByte();
					string output = "[Dbg Msg From " + NetUtility.ToHexString( msg.SenderConnection.RemoteUniqueIdentifier ) + "]: "
						+ msg.ReadString();
					WriteConsoleMessage( output );
					break;
				case MsgType.ReqLobbyList:
					SendLobbyList( msg.SenderConnection );
					break;
				case MsgType.ReqJoinLobby:
					msg.ReadByte();
					int lobbyID = msg.ReadByte();
					string playerName = msg.ReadString();
					MsgErrorCode err;
					if ( ! AddPlayerToLobby( msg.SenderConnection, lobbyID, playerName, out err ) )
					{
						NetOutgoingMessage errMsg = server.CreateMessage();
						errMsg.Write( (byte)MsgType.Error );
						errMsg.Write( (byte)err );
						server.SendMessage( errMsg, msg.SenderConnection, NetDeliveryMethod.Unreliable );
					}
					else
					{
						playerLobbies[msg.SenderConnection.RemoteUniqueIdentifier] = lobbyID;
					}
					break;
				case MsgType.ReqLeaveLobby:
					if ( playerLobbies.ContainsKey( msg.SenderConnection.RemoteUniqueIdentifier ) )
					{
						int playerLobby = playerLobbies[msg.SenderConnection.RemoteUniqueIdentifier];
						if ( playerLobby != NO_LOBBY )
						{
							lock ( lobbyLock )
							{
								lobbies[playerLobby].RemovePlayer( msg.SenderConnection );
							}
						}
					}
					break;
				case MsgType.ReqCreateLobby:
					msg.ReadByte();
					string lName = msg.ReadString();
					int maxCap = msg.ReadInt32();
					double matchDur = msg.ReadDouble();
					string pName = msg.ReadString();
					ServiceLobbyCreateRequest( msg.SenderConnection, lName, pName, maxCap, matchDur );
					break;
				default:
					int pLobby = playerLobbies[msg.SenderConnection.RemoteUniqueIdentifier];
					if ( pLobby != NO_LOBBY )
					{
						lobbies[pLobby].messageQueue.Enqueue( msg );
					}
					break;
			}
		}

		/// <summary>
		/// Handler to service a Create Lobby Request.
		/// </summary>
		/// <param name="playerConn">The requesting player's net connection object.</param>
		/// <param name="lobbyName">The name of the new lobby.</param>
		/// <param name="playerName">The name of the player.</param>
		/// <param name="playerCapacity">The player capacity of the lobby.</param>
		/// <param name="duration">The match duration to create the lobby with.</param>
		private void ServiceLobbyCreateRequest( NetConnection playerConn, string lobbyName, string playerName, int playerCapacity, double duration )
		{
			int lobbyID = StartLobby( lobbyName, playerCapacity );
			if ( lobbyID == NO_LOBBY )
			{
				NetOutgoingMessage errMsg = server.CreateMessage();
				errMsg.Write( (byte)MsgType.Error );
				errMsg.Write( (byte)MsgErrorCode.CannotCreateLobby );
				server.SendMessage( errMsg, playerConn, NetDeliveryMethod.ReliableUnordered );
			}
			else
			{
				lobbies[lobbyID].MatchDuration = duration;
				MsgErrorCode err;
				AddPlayerToLobby( playerConn, lobbyID, playerName, out err );
				playerLobbies[playerConn.RemoteUniqueIdentifier] = lobbyID;
			}
		}

		/// <summary>
		/// Replies to a lobby list request made by a client.
		/// </summary>
		/// <param name="client">The client to send the reply to.</param>
		private void SendLobbyList( NetConnection client )
		{
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write( (byte)MsgType.InfoLobbyList );
			lock ( lobbyLock )
			{
				byte numLobbies = 0;
				for ( int i = 0; i < GameSettings.MAX_LOBBIES; ++i )
				{
					if ( lobbies[i] != null && !lobbies[i].GameStarted )
					{
						++numLobbies;
					}
				}
				msg.Write( numLobbies );
				for ( byte i = 0; i < GameSettings.MAX_LOBBIES; ++i )
				{
					if ( lobbies[i] != null && !lobbies[i].GameStarted )
					{
						msg.Write( i );
						msg.Write( lobbies[i].Name );
						msg.Write( (byte)(lobbies[i].MaxPlayers) );
						msg.Write( (byte)(lobbies[i].NumPlayers) );
					}
				}
			}
			server.SendMessage( msg, client, NetDeliveryMethod.Unreliable );
		}

		/// <summary>
		/// Attempts to join a player to a lobby. The MsgErrorCode parameter will be set if the
		/// operation is not succesful.
		/// </summary>
		/// <param name="lobbyID">The ID of the lobby to join.</param>
		/// <param name="playerName">The player's name.</param>
		/// <param name="error">Out parameter set to the appropriate error code when an error occurs.</param>
		/// <returns></returns>
		private bool AddPlayerToLobby( NetConnection playerConn, int lobbyID, string playerName, out MsgErrorCode error )
		{
			bool success = true;
			error = MsgErrorCode.None;

			lock ( lobbyLock )
			{
				if ( lobbies[lobbyID] == null )
				{
					// Lobby doesn't exist
					error = MsgErrorCode.InvalidLobby;
					success = false;
				}
				else if ( lobbies[lobbyID].AddPlayer( playerConn, playerName ) == -1 )
				{
					// Lobby is full
					error = MsgErrorCode.LobbyFull;
					success = false;
				}
			}
			return success;
		}

		#endregion
	}
}
