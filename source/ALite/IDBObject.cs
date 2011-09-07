using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ALite
{
	#region Delegates

	/// <summary>
	/// Event raised when a DBObject is deleted.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void DBObjectDeletedEventHandler(object sender);

	/// <summary>
	/// Event raised when a DBObject is created.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void DBObjectCreatedEventHandler(object sender);

	/// <summary>
	/// Event raised when a DBObject is updated.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void DBObjectUpdatedEventHandler(object sender);

	/// <summary>
	/// Event raised when a DBObject is fetched.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void DBObjectFetchedEventHandler(object sender);

	#endregion

    /// <summary>
    /// Interface that describes the DBObject class
    /// </summary>
	public interface IDBObject : IRevertable, INotifyPropertyChanged
	{
		/// <summary>
		/// Object deleted event.
		/// </summary>
		event DBObjectDeletedEventHandler DBObjectDeleted;

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
