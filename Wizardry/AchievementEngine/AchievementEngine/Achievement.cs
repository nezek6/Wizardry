using Microsoft.Xna.Framework.Graphics;

namespace AchievementEngine
{
	/// <summary>
	/// This is the abstract base class representing a single achievements.
	/// </summary>
	public abstract class Achievement
	{
		#region Data Members

		/// <summary>
		/// Name of the achievement.
		/// </summary>
		protected string name;

		/// <summary>
		/// Description text associated with the achievement.
		/// </summary>
		protected string description;

		/// <summary>
		/// Whether or not the achievement has been completed/unlocked.
		/// </summary>
		protected bool completed;

		/// <summary>
		/// An image associated with the achievement.
		/// </summary>
		protected Texture2D icon;

		#endregion

		#region Constructor

		/// <summary>
		/// The base (only) constructor.
		/// </summary>
		/// <param name="name">Name of the achievement.</param>
		/// <param name="description">Description text associated with the achievement.</param>
		public Achievement( string name, string description )
		{
			this.name = name;
			this.description = description;
			this.completed = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the achievement's name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Gets the achievement's description text.
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}
		}

		/// <summary>
		/// Gets whether or not the achievement has been completed by the user.
		/// </summary>
		public bool Completed
		{
			get
			{
				return completed;
			}
		}

		/// <summary>
		/// Gets the icon associated with the achievement.
		/// </summary>
		public Texture2D Icon
		{
			get
			{
				return icon;
			}
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Initializes the achievement. This is when the achievement should
		/// register for events with the Achievement Manager.
		/// </summary>
		/// <remarks>This method should only be called by the Achievement Manager!</remarks>
		public abstract void Initialize();

		/// <summary>
		/// Callback method for when an event this achievement has registered for occurs.
		/// </summary>
		/// <remarks>This method should only be called by the Achievement Manager!</remarks>
		/// <param name="e">The achievement that triggered the callback.</param>
		/// <param name="args">Arguments relevant to the achievement.</param>
		public abstract void EventNotify( string e, params object[] args );

		#endregion
	}
}
