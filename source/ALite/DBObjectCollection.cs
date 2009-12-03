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
	public class DBObjectCollection<T> : IList<T>, INotifyPropertyChanged, IDBObjectCollection where T : IDBObject
	{
		#region Enums

		[Flags]
		private enum Status
		{
			NewStatus = 0x1,
			Dirty = 0x2,
			Deleted = 0x4
		}

		#endregion

		#region Members

		#region Status

		/// <summary>
		/// Status of the object as a bitmask; use the Status enum to unpack it
		/// </summary>
		private Status mStatus;

		#endregion

		#region Event System

		/// <summary>
		/// Event fired when a property changes value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired when a property on a child changes value.
		/// </summary>
		public event PropertyChangedEventHandler ChildPropertyChanged;

		/// <summary>
		/// Event fired when a child is deleted.
		/// </summary>
		public event DBObjectDeletedEventHandler ChildDeleted;

		/// <summary>
		/// Event fired when a child's property is changed but the validation fails.
		/// </summary>
		public event PropertyValidationFailedEventHandler ChildPropertyValidationFailed;

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
		/// All data used by the current transaction.
		/// </summary>
		private TransactionData mTransactionData;

		/// <summary>
		/// Internal list of DBObjects.
		/// </summary>
		private IList<T> mInternalList;

		#endregion

		#region Event Handlers

		/// <summary>
		/// Fired when the list is changed
		/// </summary>
		/// <param name="e"></param>
		protected void OnListChanged(ListChangedEventArgs e)
		{
			MarkListDirty();

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
		protected void OnListCleared(EventArgs e)
		{
			MarkListNew();

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
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
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
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index));
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
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
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
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, mInternalList.IndexOf(item)));
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
				OnListCleared(new EventArgs());
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
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index));
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

		#region Properties

		/// <summary>
		/// Returns true if any object is new
		/// </summary>
		public bool IsNew
		{
			get
			{
				lock (this)
				{
					return ((mStatus & Status.NewStatus) != 0);
				}
			}
		}

		/// <summary>
		/// Returns true if any object is dirty
		/// </summary>
		public bool IsDirty
		{
			get
			{
				lock (this)
				{
					return ((mStatus & Status.Dirty) != 0);
				}
			}
		}

		/// <summary>
		/// Returns true if any object is deleted
		/// </summary>
		public bool IsDeleted
		{
			get
			{
				lock (this)
				{
					return ((mStatus & Status.Deleted) != 0);
				}
			}
		}

		/// <summary>
		/// Get a list of transaction errors if the object is running a transaction.
		/// </summary>
		public List<string> TransactionErrors
		{
			get
			{
				if (IsTransactionInProgress)
				{
					return mTransactionData.ErrorMessages;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Returns true if a transaction is in progress and has encountered errors.
		/// </summary>
		public bool HasTransactionFailed
		{
			get
			{
				if (IsTransactionInProgress)
				{
					return mTransactionData.HasTransactionFailed;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Returns true if a transaction is in progress.
		/// </summary>
		public bool IsTransactionInProgress
		{
			get
			{
				lock (this)
				{
					return (mTransactionData != null);
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for the DBObjectCollection class
		/// </summary>
		public DBObjectCollection()
		{
			mInternalList = new List<T>();
			ChildPropertyChanged += new PropertyChangedEventHandler(HandleChildPropertyChanged);
			ChildDeleted += new DBObjectDeletedEventHandler(HandleChildDeleted);
			ChildPropertyValidationFailed += new PropertyValidationFailedEventHandler(HandleChildPropertyValidationFailed);
			MarkNew();
		}

		#endregion

		#region Methods

		#region Data Access

		/// <summary>
		/// Save all objects in the collection individually by using their own save methods
		/// Also removes any deleted objects from the collection
		/// </summary>
		/// <returns>Always returns true</returns>
		public virtual bool Save()
		{
			int i = 0;
			while (i < this.Count)
			{
				IDBObject obj = (IDBObject)this[i];
				obj.Save();
				if (obj.IsDeleted)
				{
					this.Remove((T)obj);
				}
				else
				{
					i++;
				}
			}

			return true;
		}

		/// <summary>
		/// Clears the list; intended to be overriden
		/// </summary>
		/// <returns>Always returns true</returns>
		public virtual bool Fetch()
		{
			return true;
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

		#region Status

		#region List Status

		/// <summary>
		/// Marks the list as dirty
		/// </summary>
		private void MarkListDirty()
		{
			mStatus |= Status.Dirty;
		}

		/// <summary>
		/// Marks the list as new
		/// </summary>
		private void MarkListNew()
		{
			mStatus = Status.NewStatus | Status.Dirty;
		}

		/// <summary>
		/// Marks the list as old
		/// </summary>
		private void MarkListOld()
		{
			mStatus = (mStatus & Status.Deleted);
		}

		/// <summary>
		/// Marks the list as deleted
		/// </summary>
		private void MarkListDeleted()
		{
			mStatus |= Status.Deleted | Status.Dirty;
		}

		#endregion

		#region Child Status

		#region Framework Methods

		/// <summary>
		/// Marks all objects as deleted
		/// </summary>
		public void MarkDeleted()
		{
			MarkListDeleted();

			foreach (IDBObject item in this)
			{
				item.MarkDeleted();
			}

			OnMarkDeleted();
		}

		/// <summary>
		/// Marks all objects as new
		/// </summary>
		public void MarkNew()
		{
			MarkListNew();

			foreach (IDBObject item in this)
			{
				item.MarkNew();
			}

			OnMarkNew();
		}

		/// <summary>
		/// Marks all objects as old
		/// </summary>
		public void MarkOld()
		{
			MarkListOld();

			foreach (IDBObject item in this)
			{
				item.MarkOld();
			}

			OnMarkOld();
		}

		/// <summary>
		/// Marks all objects as dirty
		/// </summary>
		public void MarkDirty()
		{
			MarkListDirty();

			foreach (IDBObject item in this)
			{
				item.MarkDirty();
			}

			OnMarkDirty();
		}

		#endregion

		#region Stub Methods

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when MarkDirty() is called.
		/// </summary>
		protected virtual void OnMarkDirty() { }

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when MarkNew() is called.
		/// </summary>
		protected virtual void OnMarkNew() { }

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when MarkOld() is called.
		/// </summary>
		protected virtual void OnMarkOld() { }

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when MarkDeleted() is called.
		/// </summary>
		protected virtual void OnMarkDeleted() { }

		#endregion

		#endregion

		#endregion

		#region Memento Functions

		#region Framework Methods

		/// <summary>
		/// Should be called before properties are altered at the start of a group of property alterations that represent
		/// a single change transaction.
		/// </summary>
		public void ResetUndo()
		{
			foreach (IDBObject item in this)
			{
				item.Commit();
			}
		}

		/// <summary>
		/// Restores the state of the object at the last call to "ResetUndo().
		/// </summary>
		public void RestoreBackedUpState()
		{
			if (IsTransactionInProgress)
			{

				// Retrieve the old status - this cannot be restored automatically as the property has no public setter
				Status? oldStatus = null;
				if (mTransactionData.ContainsProperty("mStatus")) oldStatus = mTransactionData.RetrieveProperty<Status>("mStatus");

				// Restore all items to their previous state.  Also removes the items - this ensures that
				// any items that should not be in this list (they were added during the transaction) are
				// correctly removed (events, etc), and that items appear in their original order.
				while (mInternalList.Count > 0)
				{
					mInternalList[0].Rollback();
					mInternalList.RemoveAt(0);
				}

				// Restore the list to its previous state
				List<T> originalList = mTransactionData.RetrieveProperty<List<T>>("mOriginalList");

				// Ensure all items in this new list are rolled back to their previous state - this
				// handles the situation in which objects deleted from the object do not appear in the first list
				foreach (T item in originalList)
				{
					item.Rollback();
					mInternalList.Add(item);
				}

				// Restore the old status
				if (oldStatus != null) mStatus = (Status)oldStatus;
			}
		}

		#endregion

		#region Stub Methods

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when Commit() is called.
		/// </summary>
		protected virtual void OnCommit() { }

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when Rollback() is called.
		/// </summary>
		protected virtual void OnRollback() { }

		#endregion

		#endregion

		#region Property Changed

		/// <summary>
		/// Set a property and fire a change event
		/// </summary>
		/// <typeparam name="TY">Type of the object</typeparam>
		/// <param name="propertyName">Name of the property being changed</param>
		/// <param name="oldValue">Reference to the value being updated</param>
		/// <param name="newValue">New value</param>
		protected void SetProperty<TY>(string propertyName, ref TY oldValue, TY newValue)
		{
			lock (this)
			{
				if (!oldValue.Equals((TY)newValue))
				{
					oldValue = newValue;
					OnPropertyChanged(propertyName);
				}
			}
		}

		/// <summary>
		/// Called when a property is changed
		/// </summary>
		/// <param name="name">Name of the property that has changed</param>
		protected void OnPropertyChanged(string name)
		{
			MarkDirty();

			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		/// <summary>
		/// Called when a child's property is changed
		/// </summary>
		/// <param name="sender">Name of the property that has changed</param>
		/// <param name="e">Event arguments</param>
		private void HandleChildPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			MarkListDirty();
		}

		private void HandleChildPropertyValidationFailed(object sender)
		{
			Rollback();
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
			child.DBObjectDeleted -= this.ChildDeleted;
			child.PropertyChanged -= this.ChildPropertyChanged;
			child.PropertyValidationFailed -= this.ChildPropertyValidationFailed;
		}

		/// <summary>
		/// Add all events to the child
		/// </summary>
		/// <param name="child">Child to remove events from</param>
		private void AddChildEvents(T child)
		{
			child.DBObjectDeleted += this.ChildDeleted;
			child.PropertyChanged += this.ChildPropertyChanged;
			child.PropertyValidationFailed += this.ChildPropertyValidationFailed;
		}

		#endregion

		#region Transactions

		public void BeginTransaction()
		{
			mTransactionData = new TransactionData();

			// Create an exact copy of the current list that we can restore to if items
			// are added/deleted/reordered from the current list during this transaction
			List<T> originalList = new List<T>();

			foreach (T item in mInternalList)
			{
				// Inform the item that a transaction is beginning
				item.BeginTransaction();
				originalList.Add(item);
			}

			// Back up the list
			mTransactionData.BackupProperty<List<T>>("mOriginalList", originalList);
		}

		public void EndTransaction()
		{
			if (IsTransactionInProgress)
			{
				foreach (T item in mInternalList)
				{
					// Inform the item that the transaction is ending
					item.EndTransaction();
				}

				mTransactionData = null;
			}
		}

		/// <summary>
		/// Commit the transaction.  Calls OnCommit(), which can contain user code, before it erases all
		/// record of the previous state of the object.
		/// </summary>
		public void Commit()
		{
			lock (this)
			{
				if (IsTransactionInProgress)
				{
					OnCommit();

					foreach (T item in mInternalList)
					{
						item.Commit();
					}

					mTransactionData.Reset();
				}
			}
		}

		/// <summary>
		/// Rollback the transaction.  Restores all changed public properties to their values
		/// at the start of the transaction, and calls OnRollback() to run any user code.
		/// </summary>
		public void Rollback()
		{
			lock (this)
			{
				if (IsTransactionInProgress)
				{
					mTransactionData.IsRollingBack = true;

					RestoreBackedUpState();
					OnRollback();

					mTransactionData.IsRollingBack = false;
				}
			}
		}

		#endregion

		#endregion
	}
}
