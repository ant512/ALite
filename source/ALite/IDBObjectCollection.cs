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
		/// Should be called before properties are altered at the start of a group of property alterations that represent
		/// a single change transaction.
		/// </summary>
		void ResetUndo();

		/// <summary>
		/// Restores the state of the object at the last call to "ResetUndo().
		/// </summary>
		void Undo();

		/// <summary>
		/// Event fired when a property changes value
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired when a child object is deleted
		/// </summary>
		event DBObjectDeletedEventHandler DBObjectDeleted;
	}
}
