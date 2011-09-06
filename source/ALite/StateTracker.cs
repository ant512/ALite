using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite
{
	#region Enums

	/// <summary>
	/// Lists all possible statuses for the object.  Primarily used to determine what
	/// to do when Save() is called.
	/// </summary>
	public enum DBObjectState
	{
		/// <summary>
		/// Object is newly created and does not exist in the data store.
		/// </summary>
		New,

		/// <summary>
		/// Object is identical to the data in the data store.
		/// </summary>
		Unmodified,

		/// <summary>
		/// Object exists in the data store but its properties have been altered.
		/// </summary>
		Modified,

		/// <summary>
		/// Object has been deleted from the data store.
		/// </summary>
		Deleted
	}

	#endregion

	/// <summary>
	/// State machine that describes all possible states of a DBObject.
	/// </summary>
	class StateTracker
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public StateTracker()
		{
			State = DBObjectState.New;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the state of the object.  Typically TransitionState()
		/// should be used instead of the setter to enforce the transition rules.
		/// </summary>
		public DBObjectState State
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Transition from the current state to the specified state.  Protects against
		/// illegal transitions, such as any state to "New" (only new, unsaved objects
		/// are new) or "Deleted" to any state (deleted objects cannot be modified).
		/// Throws an ArgumentException if an illegal transition is attempted.
		/// </summary>
		/// <param name="newState">The new state to switch to.</param>
		public void TransitionState(DBObjectState newState)
		{
			switch (newState)
			{
				case DBObjectState.Deleted:
					State = DBObjectState.Deleted;
					break;

				case DBObjectState.Modified:
					switch (State)
					{
						case DBObjectState.Modified:
						case DBObjectState.New:
							break;
						case DBObjectState.Unmodified:
							State = DBObjectState.Modified;
							break;
						case DBObjectState.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;

				case DBObjectState.New:
					throw new ArgumentException("Objects cannot become new again.");

				case DBObjectState.Unmodified:
					switch (State)
					{
						case DBObjectState.Modified:
						case DBObjectState.New:
							State = DBObjectState.Unmodified;
							break;
						case DBObjectState.Unmodified:
							break;
						case DBObjectState.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;
			}
		}

		#endregion
	}
}
