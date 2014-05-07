using System;
using System.IO;
using System.Collections.Generic;

namespace AchievementEngine
{
	/// <summary>
	/// Describes the current state of the AchievementManager engine.
	/// </summary>
	enum ManagerState
	{
		Uninitialized,
		Initializing,
		Started
	}

	/// <summary>
	/// Singleton class that provides the user with an interface to manage all achievement-related
	/// activities.
	/// </summary>
	public class AchievementManager
	{
		#region Data Members

		/// <summary>
		/// List of tracked achievements.
		/// </summary>
		private List<Achievement> achis;

		/// <summary>
		/// The current state of the manager.
		/// </summary>
		private ManagerState state;

		/// <summary>
		/// Dictionary where the key is the event name and the value is a list of achievements
		/// that are registered for the event.
		/// </summary>
		private Dictionary<string, List<Achievement>> eventReg;

		/// <summary>
		/// Queue of achievements waiting to be unregistered from events.
		/// </summary>
		private List<Tuple<string, Achievement>> unregQueue;

		#endregion

		#region Constructors

		/// <summary>
		/// Default AchievementManager constructor. Hidden to enforce singleton pattern.
		/// </summary>
		private AchievementManager()
		{
			achis = new List<Achievement>();
			state = ManagerState.Uninitialized;
			eventReg = new Dictionary<string,List<Achievement>>();
			unregQueue = new List<Tuple<string,Achievement>>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of achievements being tracked by the engine (Read-only property).
		/// </summary>
		public List<Achievement> Achievements
		{
			get
			{
				return achis;
			}
		}

		#endregion

		#region Singleton Design Members

		/// <summary>
		/// The singleton instance of the manager.
		/// </summary>
		private static AchievementManager instance;

		/// <summary>
		/// Provides access to the singleton instance of the AchievementManager.
		/// </summary>
		public static AchievementManager Instance
		{
			get
			{
				if ( instance == null )
				{
					instance = new AchievementManager();
				}
				return instance;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds an achievement to be tracked by the Achievement Manager.
		/// </summary>
		/// <remarks>All achievements must be added before calling Initialize()!</remarks>
		/// <param name="achievement">The achievement to add.</param>
		public void AddAchievement( Achievement achievement )
		{
			// Check to make sure we're still in the raw uninitialized state.
			if ( state != ManagerState.Uninitialized )
			{
				throw new Exception( "Cannot add an achievement at this time." );
			}

			achis.Add( achievement );
		}

		/// <summary>
		/// Initializes the Achievement Engine. Note that all achievements must
		/// be added BEFORE calling this method!
		/// </summary>
		public void Initialize()
		{
			// Check to make sure the engine hasn't already been initialized.
			if ( state != ManagerState.Uninitialized )
			{
				throw new Exception( "The achievement engine has already been initialized." );
			}

			// Flag that we're initializing
			state = ManagerState.Initializing;

			foreach( Achievement achi in achis )
			{
				achi.Initialize();
			}

			// Flag that the engine is ready
			state = ManagerState.Started;
		}

		/// <summary>
		/// Registers an achievement to be notified when an event occurs. This method
		/// should only be called from within the Initialize method of an achievement!
		/// </summary>
		/// <param name="e">The event to register for.</param>
		/// <param name="caller">A reference to the achievement registering.</param>
		public void RegisterForEvent( string e, Achievement caller )
		{
			// Can only register for events while the engine is initializing!
			if ( state != ManagerState.Initializing )
			{
				throw new Exception( "Cannot register for an achievement event at this time." );
			}

			if ( ! eventReg.ContainsKey( e ) )
			{
				// Event doesn't exist yet, create it!
				eventReg.Add( e, new List<Achievement>() );
			}

			eventReg[e].Add( caller );
		}

		/// <summary>
		/// Unregisters an achievement from being notified when an event occurs.
		/// </summary>
		/// <param name="e">The event to unregister from.</param>
		/// <param name="caller">A reference to the achievement unregistering.</param>
		public void UnregisterFromEvent( string e, Achievement caller )
		{
			if ( eventReg.ContainsKey( e ) )
			{
				// Queue up to unregister the achievement. The actual
				//   unregistering will occur the next time RaiseEvent
				//   is called. This is to avoid the eventReg dictionary
				//   from being modified while being iterated through
				//   during a RaiseEvent call.
				unregQueue.Add( new Tuple<string,Achievement>(e, caller) );
			}
		}

		/// <summary>
		/// Raises an achievement event.
		/// </summary>
		/// <param name="e">The achievement event to raise.</param>
		/// <param name="args">Any relevant arguments.</param>
		public void RaiseEvent( string e, params object[] args )
		{
			// Flush out any achievements waiting to be unregistered
			if ( unregQueue.Count > 0 )
			{
				foreach( Tuple<string,Achievement> tup in unregQueue )
				{
					eventReg[tup.Item1].Remove( tup.Item2 );
				}
				unregQueue.Clear();
			}

			// Now let's call the callbacks for any achievements registered to this event
			if ( eventReg.ContainsKey( e ) )
			{
				foreach( Achievement achi in eventReg[e] )
				{
					achi.EventNotify( e, args );
				}
			}
		}

		/// <summary>
		/// Loads achievements game save data from a specified file.
		/// </summary>
		/// <remarks>
		/// If the file does not exist, it will be created.
		/// </remarks>
		/// <param name="filePath">Path to the saved data file.</param>
		public void LoadSaveGame( string filePath )
		{
			FileStream fs = new FileStream( filePath, FileMode.OpenOrCreate );
			BinaryReader br = new BinaryReader( fs );
			LoadSaveGame( br );
			br.Close();
		}

		/// <summary>
		/// Loads achievements game save data from a binary stream reader. The
		/// stream must be already open and pointing to the beginning of achievement
		/// data.
		/// </summary>
		/// <param name="reader">Open binary reader object pointing to achievement data.</param>
		public void LoadSaveGame( BinaryReader reader )
		{
			// TODO : Implement AchievementManager loading
			throw new NotImplementedException();
		}

		/// <summary>
		/// Saves current achievement progress to the specified file.
		/// </summary>
		/// <remarks>
		/// If the file does not exist, it will be created.
		/// </remarks>
		/// <param name="filePath">Path to the saved data file.</param>
		public void SaveGame( string filePath )
		{
			FileStream fs = new FileStream( filePath, FileMode.Create );
			BinaryWriter bw = new BinaryWriter( fs );
			SaveGame( bw );
			bw.Close();
		}

		/// <summary>
		/// Saves current achievement progress to a binary stream. The stream must be open.
		/// </summary>
		/// <param name="writer">Open binary writer object.</param>
		public void SaveGame( BinaryWriter writer )
		{
			// TODO : Implement AchievementManager saving
			throw new NotImplementedException();
		}

		#endregion
	}
}
