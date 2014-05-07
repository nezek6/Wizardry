using System.Collections.Generic;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;
using WizardryShared;

namespace WizardryClient
{
    public class GameManager : ResourceProvider
    {
        #region Member Variables

        private IsoTileMap tilemap;

        private Game game;

        private Camera camera;

        private SpriteBatch spriteBatch;

		private GameState gameState;

		private NetClient client;

		private List<IPacketReceiver> packetReceivers;

		// The player's ID as set by the lobby when joined
		private int myID;

		private bool inMultiplayer = false;

		private string lobbyName;
		
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
			get { return spriteBatch; }
			set { spriteBatch = value; }
		}

        public TextureProvider TextureProvider
        {
            get { return ((WizardryGameClient)Game).TextureProvider; }
        }

		public GameState GameState
		{
			get { return gameState; }
			set { gameState = value; }
		}
		
		public NetClient Client
		{
			get { return client; }
		}

		public int MyID
		{
			get { return myID; }
			set { myID = value; }
		}
		
		public bool InMultiplayer
		{
			get { return inMultiplayer; }
			set { inMultiplayer = value; }
		}

		public string MPLobbyName
		{
			get { return lobbyName; }
			set { lobbyName = value; }
		}

        #endregion

        #region Constructors

        /// <summary>
        /// Default GameManager constructor. Hidden to enforce singleton pattern.
        /// </summary>
        private GameManager()
        {
			packetReceivers = new List<IPacketReceiver>();

			NetPeerConfiguration config = new NetPeerConfiguration( GameSettings.APP_NAME );
			config.EnableMessageType( NetIncomingMessageType.DiscoveryResponse );
			client = new NetClient( config );
        }

        #endregion

        #region Singleton Design Members

        /// <summary>
        /// The singleton instance of the manager.
        /// </summary>
        private static GameManager instance;

        /// <summary>
        /// Provides access to the singleton instance of the GameManager.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        #endregion

        #region Required Methods

		private bool flipFlop = true;
        public void Shoot( int spellID, Vector2 startPoint, Vector2 clickPosition, Vector2 direction, float rotation, Character caster )
        {
			// Security Check
			if ( gameState == null )
			{
				return;
			}

			if ( inMultiplayer )
			{
				SendSpellCastRequest( spellID, startPoint, clickPosition, direction, rotation );
				return;
			}


			// Cast the spell directly
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

            if (!box1.Intersects(box2))
            {
                return false;
            }
            else if (!pixelPerfect)
            {
                return true;
            }

            Matrix mat1 = Matrix.CreateTranslation(-anim1.FrameWidth/2, -anim1.FrameHeight, 0) * Matrix.CreateRotationZ(anim1.Rotation)
                * Matrix.CreateScale(anim1.Scale) * Matrix.CreateTranslation(sprite1.WorldPosition.X, sprite1.WorldPosition.Y, 0);

            Matrix mat2 = Matrix.CreateTranslation(-anim2.FrameWidth / 2, -anim2.FrameHeight/2, 0) * Matrix.CreateRotationZ(anim2.Rotation)
                * Matrix.CreateScale(anim2.Scale) * Matrix.CreateTranslation(sprite2.WorldPosition.X, sprite2.WorldPosition.Y, 0);

            Color[,] tex1 = anim1.ColorArray;
            Color[,] tex2 = anim2.ColorArray;

            //Matrix which expresses the coordinates of the first textures in terms of the second texture's local coordinate
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);

            int width1 = anim1.FrameWidth;
            int height1 = anim1.FrameHeight;
            int width2 = anim2.FrameWidth;
            int height2 = anim2.FrameHeight;

            int frameOffset1 = anim1.CurrentFrame * width1;
            int frameOffset2 = anim2.CurrentFrame * width2;

            //Check if any non-transparent pixel of the first texture overlaps with non-transparent pixel of the second texture
            for (int x1 = frameOffset1; x1 < frameOffset1 + width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1 - frameOffset1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X + frameOffset2;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    //There must be an overlap, return the position of the collision
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
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

		#region Network Handling

		public void HandleIncomingPackets()
		{
			NetIncomingMessage msg;
			while ( ( msg = client.ReadMessage() ) != null )
			{
				switch ( msg.MessageType )
				{
					case NetIncomingMessageType.DiscoveryResponse:
						// just connect to first server discovered
						client.Connect( msg.SenderEndpoint );
						break;
					case NetIncomingMessageType.Data:
						HandleDataPacket( msg );
						break;
					case NetIncomingMessageType.DebugMessage:
						System.Console.WriteLine( msg.ReadString() );
						break;
				}
			}
		}

		private void HandleDataPacket( NetIncomingMessage msg )
		{
			MsgType type = (MsgType)msg.PeekByte();
			switch ( type )
			{
				case MsgType.Debug:
					msg.ReadByte();
					string output = "[Dbg Msg From Server]: " + msg.ReadString();
					System.Console.WriteLine( output );
					break;
				default:
					int numReceivers = packetReceivers.Count;
					for ( int i = 0; i < numReceivers; ++i )
					{
						packetReceivers[i].ReceivePacket( msg );
						msg.Position = 0;	// Reset the message for the next guy
					}
					break;
			}
		}

		/// <summary>
		/// Interface allowing package receivers to register to receive and process data packets that
		/// can't be handled by the game manager.
		/// </summary>
		/// <param name="receiver">The receiver to register.</param>
		public void RegisterForPackets( IPacketReceiver receiver )
		{
			packetReceivers.Add( receiver );
		}

		/// <summary>
		/// Unregisters a package receiver from receiving packets.
		/// </summary>
		/// <param name="receiver">The receiver to unregister.</param>
		public void UnregisterForPackets( IPacketReceiver receiver )
		{
			packetReceivers.Remove( receiver );
		}

		/// <summary>
		/// Sends a request to the server to cast a spell.
		/// </summary>
		/// <param name="spellID">The ID of the spell to cast.</param>
		/// <param name="startPoint">The spell's starting point.</param>
		/// <param name="direction">The spell's direction.</param>
		/// <param name="rotation">The spell's rotation.</param>
		private void SendSpellCastRequest( int spellID, Vector2 startPoint, Vector2 clickPos, Vector2 direction, float rotation )
		{
			NetOutgoingMessage msg = client.CreateMessage();
			msg.Write( (byte)MsgType.ReqSpellCast );
			msg.Write( spellID );
			msg.Write( (float)startPoint.X );
			msg.Write( (float)startPoint.Y );
            msg.Write( (float)clickPos.X );
            msg.Write( (float)clickPos.Y );
			msg.Write( (float)direction.X );
			msg.Write( (float)direction.Y );
			msg.Write( (float)rotation );
			client.SendMessage( msg, NetDeliveryMethod.Unreliable );
		}
		
		#endregion

		
	}
}
