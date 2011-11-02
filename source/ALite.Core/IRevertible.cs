using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite.Core
{
	/// <summary>
	/// Defines the properties of an object that can store a state and
	/// revert to that state at a future point.
	/// </summary>
	public interface IRevertible
	{
		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		void SetRestorePoint();

		/// <summary>
		/// Reverts to the last saved restore point.
		/// </summary>
		void RevertToRestorePoint();
	}
}
