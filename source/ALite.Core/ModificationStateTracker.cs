using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite.Core
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
	[Serializable]
	class ModificationStateTracker : StateMachine<ModificationState>
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ModificationStateTracker() : this(ModificationState.New) { }

		/// <summary>
		/// Constructor.  Creates a state tracker with an initial state.
		/// </summary>
		/// <param name="initialState">The initial state for the new tracker.</param>
		public ModificationStateTracker(ModificationState initialState)
			: base(initialState)
		{
			AddTransition(ModificationState.New, ModificationState.Deleted);
			AddTransition(ModificationState.New, ModificationState.Unmodified);
			AddTransition(ModificationState.Modified, ModificationState.Unmodified);
			AddTransition(ModificationState.Modified, ModificationState.Deleted);
			AddTransition(ModificationState.Unmodified, ModificationState.Modified);
			AddTransition(ModificationState.Unmodified, ModificationState.Deleted);
		}

		#endregion
	}
}
