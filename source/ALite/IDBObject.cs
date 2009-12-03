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

		event PropertyValidationFailedEventHandler PropertyValidationFailed;

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
		/// Returns true if a transaction is currently in progress.
		/// </summary>
		/// <returns></returns>
		bool IsTransactionInProgress { get; }

		/// <summary>
		/// Returns true if a transaction is in progress and has encountered errors.
		/// </summary>
		bool HasTransactionFailed { get; }

		/// Get a list of transaction errors if the object is running a transaction.
		/// </summary>
		List<string> TransactionErrors { get; }

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
		/// Commit any changes made to the object.
        /// </summary>
		void Commit();

        /// <summary>
		/// Rollback any changes made to the object.
        /// </summary>
		void Rollback();

		/// <summary>
		/// Begin a transaction.
		/// </summary>
		void BeginTransaction();

		/// <summary>
		/// End a transaction.
		/// </summary>
		void EndTransaction();
	}
}
