using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite.Core
{
	/// <summary>
	/// Class representing an abstract state machine.  The possible states should be an
	/// enum passed as the T parameter when constructing the object.  State transition
	/// rules are managed using the AddTransition() method.
	/// </summary>
	/// <typeparam name="T">The available states for the state machine, expressed as
	/// an enum.</typeparam>
	public class StateMachine<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		#region Members

		/// <summary>
		/// The current state of the state machine.
		/// </summary>
		private T mState;

		/// <summary>
		/// All possible state transitions.  The key is the starting state; the value is
		/// the set of states that the starting state can transition to.
		/// </summary>
		private Dictionary<T, HashSet<T>> mTransitionTable = new Dictionary<T, HashSet<T>>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the state of the state machine.
		/// </summary>
		public T State
		{
			get { return mState; }
			set { TransitionState(value); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="initialState">The starting state of the state machine.</param>
		public StateMachine(T initialState)
		{
			mState = initialState;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a valid state transition definition to the state machine.
		/// Attempts to transition between states that aren't included as
		/// valid transitions will cause the State setter to throw an
		/// argument exception.
		/// </summary>
		/// <param name="fromState">The starting state.</param>
		/// <param name="toState">The end state.</param>
		public void AddTransition(T fromState, T toState)
		{
			if (!mTransitionTable.ContainsKey(fromState))
			{
				mTransitionTable[fromState] = new HashSet<T>();
			}

			HashSet<T> transitions = mTransitionTable[fromState];

			transitions.Add(toState);
		}

		/// <summary>
		/// Attempts to transition from the current state to the
		/// supplied state.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		private void TransitionState(T state)
		{
			HashSet<T> validTransitions = mTransitionTable[mState];

			if (validTransitions.Contains(state))
			{
				mState = state;
				return;
			}

			throw new ArgumentException("Cannot transition from state " + mState.ToString() + " to state " + state.ToString());
		}

		#endregion
	}
}
