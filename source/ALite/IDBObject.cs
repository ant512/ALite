using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace ALite
{
    /// <summary>
    /// Interface that describes the DBObject class
    /// </summary>
	public interface IDBObject
	{
		/// <summary>
		/// Property changed event
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Object deleted event
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
        /// Save the object to the database
        /// </summary>
        /// <returns>The outcome of the save attempt</returns>
		DBErrorCode Save();

        /// <summary>
        /// Fetch the object from the database
        /// </summary>
        /// <returns>The outcome of the fetch attempt</returns>
		DBErrorCode Fetch();

        /// <summary>
        /// Mark the object as old
        /// </summary>
		void MarkOld();

        /// <summary>
        /// Mark the object as new
        /// </summary>
		void MarkNew();

        /// <summary>
        /// Mark the object as deleted
        /// </summary>
		void MarkDeleted();

        /// <summary>
        /// Mark the object as dirty
        /// </summary>
		void MarkDirty();

        /// <summary>
		/// Should be called before properties are altered at the start of a group of property alterations that represent
		/// a single change transaction.
        /// </summary>
		void ResetUndo();

        /// <summary>
		/// Restores the state of the object at the last call to "ResetUndo().
        /// </summary>
		void Undo();
	}
}
