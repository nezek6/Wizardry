using Microsoft.Xna.Framework.Input;

namespace DebugAssist
{
	public delegate string DbgMsg();

	public delegate void DbgActionDel();

	public struct DbgKey
	{
		private Keys key;

		private Buttons button;

		public DbgKey( Keys key, Buttons button )
		{
			this.key = key;
			this.button = button;
		}

		public Keys Key
		{
			get
			{
				return key;
			}
			set
			{
				this.key = value;
			}
		}

		public Buttons Button
		{
			get
			{
				return button;
			}
			set
			{
				this.button = value;
			}
		}
	}

	public struct DbgAction
	{
		private DbgKey activator;

		private DbgActionDel action;

		public DbgAction( DbgKey activator, DbgActionDel action )
		{
			this.activator = activator;
			this.action = action;
		}

		public DbgKey Activator
		{
			get
			{
				return activator;
			}
			set
			{
				this.activator = value;
			}
		}

		public DbgActionDel Action
		{
			get
			{
				return action;
			}
			set
			{
				this.action = value;
			}
		}
	}

	public interface IDbgAssistor
	{
		int AddDbgAction( DbgAction action );
		int AddDbgMsg( DbgMsg msg );
		void RemoveDbgAction( int token );
		void RemoveDbgMsg( int token );
	}
}
