using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ALite
{
    /// <summary>
    /// Interface that describes the DBObject class
    /// </summary>
	public interface IDBObject
	{
		/// <summary>
		/// Property changed event.
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Object deleted event.
		/// </summary>
		event DBObjectDeletedEventHandler DBObjectDeleted;

        /// <summary>
        /// Is the object dirty?
        /// </summary>
		bool IsDirty { get; }

        /// <summary>
        /// Is the object new?
        /// </summary>
		bool IsNew { get; }

        /// <summary>
        /// Is the object deleted?
        /// </summary>
		bool IsDeleted { get; }

        /// <summary>
        /// Save the object to the database.
        /// </summary>
		void Save();

        /// <summary>
        /// Fetch the object from the database.
        /// </summary>
		void Fetch();

		/// <summary>
		/// Deletes the object from the database.
		/// </summary>
		void Delete();

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
