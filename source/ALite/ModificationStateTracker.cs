using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite
{
	#region Enums

	/// <summary>
	/// Lists all possible modification states of an object.
	/// </summary>
	public enum ModificationState
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
	class ModificationStateTracker
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ModificationStateTracker()
		{
			State = ModificationState.New;
		}

		/// <summary>
		/// Constructor.  Creates a state tracker with an initial state.
		/// </summary>
		/// <param name="initialState">The initial state for the new tracker.</param>
		public ModificationStateTracker(ModificationState initialState)
		{
			State = initialState;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the state of the object.
		/// </summary>
		public ModificationState State
		{
			get;
			private set;
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
		public void TransitionState(ModificationState newState)
		{
			switch (newState)
			{
				case ModificationState.Deleted:
					State = ModificationState.Deleted;
					break;

				case ModificationState.Modified:
					switch (State)
					{
						case ModificationState.Modified:
						case ModificationState.New:
							break;
						case ModificationState.Unmodified:
							State = ModificationState.Modified;
							break;
						case ModificationState.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;

				case ModificationState.New:
					throw new ArgumentException("Objects cannot become new again.");

				case ModificationState.Unmodified:
					switch (State)
					{
						case ModificationState.Modified:
						case ModificationState.New:
							State = ModificationState.Unmodified;
							break;
						case ModificationState.Unmodified:
							break;
						case ModificationState.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;
			}
		}

		#endregion
	}
}
