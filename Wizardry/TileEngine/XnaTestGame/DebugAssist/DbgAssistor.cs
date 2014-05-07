using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DebugAssist
{
	public class DbgAssistor : DrawableGameComponent, IDbgAssistor
	{
		#region Data Members

		private SpriteFont dbgFont;

		private Color dbgColour;

		private string title;

		private Vector2 position;

		private DbgKey dbgSwitch;

		private SpriteBatch sb;

		private int tokenCounter;

		private Dictionary<int, DbgMsg> messages;

		private Dictionary<int, DbgAction> actions;

		private KeyboardState prevKBState;

		private GamePadState prevGPState;

		#endregion

		#region Constructor

		public DbgAssistor( Game game ):base( game )
		{
			dbgFont = null;
			dbgColour = Color.HotPink;
			title = "Debug Info:";
			position = new Vector2( 3, 3 );
			dbgSwitch = new DbgKey( Keys.OemTilde, Buttons.Start );
			tokenCounter = 0;
			messages = new Dictionary<int,DbgMsg>();
			actions = new Dictionary<int,DbgAction>();
		}

		#endregion

		#region Properties

		public SpriteFont DbgFont
		{
			get { return dbgFont; }
			set { dbgFont = value; }
		}

		public Color DbgColour
		{
			get { return dbgColour; }
			set { dbgColour = value; }
		}

		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public DbgKey DbgSwitch
		{
			get { return dbgSwitch; }
			set { dbgSwitch = value; }
		}

		#endregion

		#region Methods

		public void SetSpritebatch( SpriteBatch spriteBatch )
		{
			sb = spriteBatch;
		}

		public int AddDbgMsg( DbgMsg msg )
		{
			messages.Add( tokenCounter, msg );
			return tokenCounter++;
		}

		public int AddDbgAction( DbgAction action )
		{
			actions.Add( tokenCounter, action );
			return tokenCounter++;
		}

		public void RemoveDbgMsg( int token )
		{
			messages.Remove( token );
		}

		public void RemoveDbgAction( int token )
		{
			actions.Remove( token );
		}

		#endregion

		#region GameComponent Overrides

		public override void Initialize()
		{
			prevKBState = Keyboard.GetState();
			prevGPState = GamePad.GetState( PlayerIndex.One );
			base.Initialize();
		}

		public override void Update( GameTime gameTime )
		{
			KeyboardState KBState = Keyboard.GetState();
			GamePadState GPState = GamePad.GetState( PlayerIndex.One );

			if ( ( KBState.IsKeyDown( dbgSwitch.Key ) && prevKBState.IsKeyUp( dbgSwitch.Key ) ) ||
				 ( GPState.IsButtonDown( dbgSwitch.Button ) && prevGPState.IsButtonUp( dbgSwitch.Button ) ) )
			{
				Visible = !Visible;
			}

			foreach( DbgAction action in actions.Values )
			{
				if ( ( KBState.IsKeyDown( action.Activator.Key ) &&
					   prevKBState.IsKeyUp( action.Activator.Key ) ) ||
				     ( GPState.IsButtonDown( action.Activator.Button ) &&
					   prevGPState.IsButtonUp( action.Activator.Button ) ) 
				   )
				{
					action.Action();
				}
			}

			prevGPState = GPState;
			prevKBState = KBState;

			base.Update( gameTime );
		}

		public override void Draw( GameTime gameTime )
		{
			// If no font has been set, nothing to draw
			if ( dbgFont == null || sb == null )
			{
				return;
			}

			string dbgString = title + "\n\n";

			foreach( DbgMsg msg in messages.Values )
			{
				dbgString += ( msg() + "\n" );
			}

			sb.DrawString( dbgFont, dbgString, position, dbgColour );

 			base.Draw(gameTime);
		}

		#endregion
	}
}
