﻿using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ALite
{
	#region Delegates

	/// <summary>
	/// Event raised when a persistable object is deleted.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableDeletedEventHandler(IPersistable sender);

	/// <summary>
	/// Event raised when a persistable object is created.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableCreatedEventHandler(IPersistable sender);

	/// <summary>
	/// Event raised when a persistable object is updated.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableUpdatedEventHandler(IPersistable sender);

	/// <summary>
	/// Event raised when a persistable object is fetched.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableFetchedEventHandler(IPersistable sender);

	#endregion

	/// <summary>
	/// Interface that describes the persistable objects.
	/// </summary>
	public interface IPersistable : IRevertable, INotifyPropertyChanged
	{
		/// <summary>
		/// Object deleted event.
		/// </summary>
		event PersistableDeletedEventHandler PersistableObjectDeleted;

		/// <summary>
		/// Gets the modification state of the object.
		/// </summary>
		ModificationState State { get; }

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
	}
}
