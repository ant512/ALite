using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace ALite
{
	#region Delegates

	/// <summary>
	/// Delegate for handling the list being changed.
	/// </summary>
	/// <param name="sender">DBObjectCollection that fired the event</param>
	/// <param name="e">Event arguments</param>
	public delegate void ListChangedEventHandler(object sender, ListChangedEventArgs e);

	/// <summary>
	/// Delegate for handling the list being cleared.
	/// </summary>
	/// <param name="sender">DBObjectCollection that fired the event</param>
	/// <param name="e">Event arguments</param>
	public delegate void ListClearedEventHandler(object sender, EventArgs e);

	#endregion

	/// <summary>
	/// Interface that describes a collection of IPersistable objects.
	/// </summary>
	public interface IPersistableCollection
	{
		/// <summary>
		/// List changed event handler.
		/// </summary>
		event ListChangedEventHandler ListChanged;

		/// <summary>
		/// List cleared event handler.
		/// </summary>
		event ListClearedEventHandler ListCleared;

		/// <summary>
		/// Event fired when a child is deleted.
		/// </summary>
		event PersistableDeletedEventHandler ChildDeleted;

		/// <summary>
		/// Save the collection.
		/// </summary>
		void Save();

		/// <summary>
		/// Delete the collection.
		/// </summary>
		void Delete();
	}
}
