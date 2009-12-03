using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace ALite
{
	/// <summary>
	/// Interface that describes the DBObjectCollection class
	/// </summary>
	public interface IDBObjectCollection
	{
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
		/// Save the IDBObjectCollection
		/// </summary>
		/// <returns></returns>
		bool Save();

		/// <summary>
		/// Populate the collection with data
		/// </summary>
		/// <returns></returns>
		bool Fetch();

		/// <summary>
		/// Mark all children in the object as deleted
		/// </summary>
		void MarkDeleted();

		/// <summary>
		/// Mark all children in the object as new
		/// </summary>
		void MarkNew();

		/// <summary>
		/// Mark all children in the object as old
		/// </summary>
		void MarkOld();

		/// <summary>
		/// Mark all children in the object as dirty
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

		/// <summary>
		/// Event fired when a property changes value
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;
	}
}
