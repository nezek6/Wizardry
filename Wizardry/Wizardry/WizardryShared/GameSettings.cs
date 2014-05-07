using Microsoft.Xna.Framework;

namespace WizardryShared
{
	/// <summary>
	/// The teams in multiplayer.
	/// </summary>
	public enum Team {RED, BLUE}

	/// <summary>
	/// Contains static constants which represent the game's parameters.
	/// </summary>
	public class GameSettings
	{
		// The port number used for wizardry communication
		public const int SERVER_PORT_NUM = 23; // 13370;

		// The server URL (blank = use LAN discovery)
		public const string SERVER_URL = ""; //"solari.dyndns-web.com"; // "";

		// The application name (used by lidgren)
		public const string APP_NAME = "wizardry";

		// The maximum number of players that can ever be in one lobby
		public const int MAX_LOBBY_CAPACITY = 8;

		// The maximum number of spells that can exist in a single game at a given time
		public const int MAX_SPELLS = 500;

		// The maximum number of pickups that can exist in a single game at a given time
		public const int MAX_PICKUPS = 200;

		// The maximum length of a lobby's name
		public const int MAX_LOBBY_NAME_LEN = 30;

		// The maximum number of lobbies that can exist at any one time
		public const int MAX_LOBBIES = 10;

		// The default duration for a Multiplayer Match
		public const double DEFAULT_MP_MATCH_DURATION = 15;

		// The time for a character to respawn, in seconds
		public const double RESPAWN_TIME = 5;

		/// <summary>
		/// Retrieves the Colour for a given team.
		/// </summary>
		/// <param name="team">The team.</param>
		/// <returns>The Colour for the given team.</returns>
		public static Color TeamColour( Team team )
		{
			switch ( team )
			{
				case Team.RED:
					return Color.Red;
				case Team.BLUE:
					return Color.Cyan;
				default:
					return Color.Gray;
			}
		}
	}

	/// <summary>
	/// Defines the protocol for communication between the server and client. The first byte
	/// of any data packet sent will correspond to one of these values.
	/// </summary>
	public enum MsgType : byte
	{
		Error,
		ReqLobbyList,		// Request the list of lobbies
		InfoLobbyList,		// Returned lobby list
		ReqJoinLobby,		// Request to join lobby
		ReqStartLobby,		// Request to start a new lobby
		InfoJoinedLobby,	// Indicates the lobby joined
		GSUpdate,			// Updated gamestate packet (sent from server to client)
		CharUpdate,			// A character update (sent from client to server)
		ReqSpellCast,		// A spell cast request made to the server
		ReqClassChange,		// A class change request
		ReqTeamChange,		// A team change request
		ReqReady,			// Client declaring "ready"
		InfoGameStart,		// Signal to clients that the game is starting
		ReqLeaveLobby,		// Signal from the client that it's leaving the lobby
		InfoGameOver,		// Signal from the server that the game ended
		ReqUpgrade,			// Request an upgrade purchase
		ReqCreateLobby,		// Request to create a new lobby

		Debug				// A debug string
	}

	public enum MsgErrorCode : byte
	{
		None,				// No error
		LobbyFull,			// The requested lobby to join is full
		InvalidLobby,		// The requested lobby to join doesn't exist
		CannotCreateLobby	// Unable to create a new lobby
	}
}
