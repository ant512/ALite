using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ALite.Core
{
	#region Delegates

	/// <summary>
	/// Event raised when a persistable object is deleted.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableDeletedEventHandler(object sender);

	/// <summary>
	/// Event raised when a persistable object is created.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableCreatedEventHandler(object sender);

	/// <summary>
	/// Event raised when a persistable object is updated.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableUpdatedEventHandler(object sender);

	/// <summary>
	/// Event raised when a persistable object is fetched.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void PersistableFetchedEventHandler(object sender);

	#endregion

	/// <summary>
	/// Interface that describes the persistable objects.
	/// </summary>
	public interface IPersistable : IRevertible, IValidateable, INotifyPropertyChanged
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
