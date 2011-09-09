using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace ALite
{
	/// <summary>
	/// Collection of IDBObject objects
	/// </summary>
	[Serializable]
	public class PersistedObjectCollection<T> : IList<T>, IPersistableCollection where T : IPersistable
	{
		#region Members

		#region Event System

		/// <summary>
		/// Event fired when a child is deleted.
		/// </summary>
		public event PersistableDeletedEventHandler ChildDeleted;

		/// <summary>
		/// List changed event handler.
		/// </summary>
		public event ListChangedEventHandler ListChanged;

		/// <summary>
		/// List cleared event handler.
		/// </summary>
		public event ListClearedEventHandler ListCleared;

		#endregion

		/// <summary>
		/// Internal list of DBObjects.
		/// </summary>
		private IList<T> mInternalList = new List<T>();

		#endregion

		#region Event Handlers

		/// <summary>
		/// Fired when the list is changed
		/// </summary>
		/// <param name="e"></param>
		protected void RaiseListChangedEvent(ListChangedEventArgs e)
		{
			ListChangedEventHandler handler = ListChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Fired when the list is cleared
		/// </summary>
		/// <param name="e"></param>
		protected void RaiseListClearedEvent(EventArgs e)
		{
			ListClearedEventHandler handler = ListCleared;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion

		#region IList<T> Members

		/// <summary>
		/// Get the index of the specified item
		/// </summary>
		/// <param name="item">Item to find the index of</param>
		/// <returns>The index of the item</returns>
		public int IndexOf(T item)
		{
			return mInternalList.IndexOf(item);
		}

		/// <summary>
		/// Insert an item at the specified index
		/// </summary>
		/// <param name="index">Index to insert at</param>
		/// <param name="item">Item to insert</param>
		public void Insert(int index, T item)
		{
			AddChildEvents(item);
			mInternalList.Insert(index, item);
			RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		/// <summary>
		/// Remove the item at the specified index
		/// </summary>
		/// <param name="index">Index of the item to remove</param>
		public void RemoveAt(int index)
		{
			lock (this)
			{
				T item = mInternalList[index];
				if (mInternalList.Remove(item))
				{
					RemoveChildEvents(item);
					RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index));
				}
			}
		}

		/// <summary>
		/// Get or set the item at the specified index
		/// </summary>
		/// <param name="index">Index of the item to retrieve</param>
		/// <returns>The item at the specified index</returns>
		public T this[int index]
		{
			get { return mInternalList[index]; }
			set
			{
				AddChildEvents(value);
				mInternalList[index] = value;
				RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Add an item to the end of the list
		/// </summary>
		/// <param name="item">Them item to add</param>
		public void Add(T item)
		{
			AddChildEvents(item);
			mInternalList.Add(item);
			RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, mInternalList.IndexOf(item)));
		}

		/// <summary>
		/// Remove all items from the list
		/// </summary>
		public void Clear()
		{
			lock (this)
			{
				// Remove all event handlers from objects
				foreach (T item in mInternalList)
				{
					RemoveChildEvents(item);
				}

				mInternalList.Clear();
				RaiseListClearedEvent(new EventArgs());
			}
		}

		/// <summary>
		/// Check if the list contains the specified item
		/// </summary>
		/// <param name="item">The item to check the existence of</param>
		/// <returns>True if the item is stored within the list</returns>
		public bool Contains(T item)
		{
			return mInternalList.Contains(item);
		}

		/// <summary>
		/// Disabled because list events remain wired up.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			//CopyTo(array, arrayIndex);
			throw new Exception("Not implemented");
		}

		/// <summary>
		/// Get the number of items in the list
		/// </summary>
		public int Count
		{
			get { return mInternalList.Count; }
		}

		/// <summary>
		/// Returns true if the list is read-only
		/// </summary>
		public bool IsReadOnly
		{
			get { return IsReadOnly; }
		}

		/// <summary>
		/// Remove the specified item from the list
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			lock (this)
			{
				int index = mInternalList.IndexOf(item);
				if (mInternalList.Remove(item))
				{
					RemoveChildEvents(item);
					RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index));
					return true;
				}
				else
					return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Get an enumerator for the list
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return mInternalList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Get an enumerator for the list
		/// </summary>
		/// <returns>An enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)mInternalList).GetEnumerator();
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public PersistedObjectCollection()
		{
			ChildDeleted += new PersistableDeletedEventHandler(HandleChildDeleted);
		}

		#endregion

		#region Methods

		#region Data Access

		/// <summary>
		/// Save all objects in the collection individually by calling their save methods.
		/// </summary>
		public void Save()
		{
			foreach (T obj in mInternalList)
			{
				obj.Save();
			}
		}

		/// <summary>
		/// Delete all objects in the collection by calling their delete methods.
		/// </summary>
		public void Delete()
		{
			while (mInternalList.Count > 0)
			{
				mInternalList[0].Delete();
			}
		}

		#endregion

		#region Sorting

		/// <summary>
		/// Sort the list
		/// </summary>
		public void Sort()
		{
			ArrayList.Adapter((IList)mInternalList).Sort();
		}

		#endregion

		#region List Changed

		/// <summary>
		/// Called when a child is deleted.  Removes all event listeners
		/// from the child and removes the child from the list.
		/// </summary>
		/// <param name="sender">The child that has been deleted</param>
		private void HandleChildDeleted(object sender)
		{
			T child = (T)sender;
			Remove(child);
		}

		#endregion

		#region Child Events

		/// <summary>
		/// Remove all events from the child
		/// </summary>
		/// <param name="child">Child to remove events from</param>
		private void RemoveChildEvents(T child)
		{
			child.PersistableObjectDeleted -= this.ChildDeleted;
		}

		/// <summary>
		/// Add all events to the child
		/// </summary>
		/// <param name="child">Child to remove events from</param>
		private void AddChildEvents(T child)
		{
			child.PersistableObjectDeleted += this.ChildDeleted;
		}

		#endregion

		#endregion
	}
}
