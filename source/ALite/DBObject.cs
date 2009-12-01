using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace ALite
{
	#region Events

	/// <summary>
	/// Event raised when a DBObject is deleted.
	/// </summary>
	/// <param name="sender">The object that raised the event.</param>
	public delegate void DBObjectDeletedEventHandler(object sender);

	#endregion

	/// <summary>
	/// Base class for objects that interact with the database
	/// </summary>
	[Serializable] public abstract class DBObject : IDBObject, INotifyPropertyChanged, IEnlistmentNotification
	{
		#region Enums

		[Flags]
		private enum Status : byte
		{
			NewStatus = 0x1,
			Dirty = 0x2,
			Deleted = 0x4
		}

		#endregion

		#region Members

		#region Events

		/// <summary>
		/// Event fired when a property changes value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired when the object is deleted
		/// </summary>
		public event DBObjectDeletedEventHandler DBObjectDeleted;

		#endregion

		/// <summary>
        /// Status of the object as a bitmask; use the Status enum to unpack it
        /// </summary>
		private Status mStatus;

        /// <summary>
        /// Stores backup data for later restoration
        /// </summary>
		private Dictionary<string, object> mMemento;

		/// <summary>
		/// List of rules that properties are checked against before they are set
		/// </summary>
		private ValidationRuleCollection mRules;

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private DelegateRuleCollection mDelegateRules;

		/// <summary>
		/// Tracks transactional locks.
		/// </summary>
		private TransactionLock mLock = new TransactionLock();

		/// <summary>
		/// Tracks if the object has been enlisted in a transaction or not.
		/// </summary>
		bool mEnlisted = false;

		#endregion

		#region Properties

        /// <summary>
        /// Is the object new?
        /// </summary>
		public bool IsNew
		{
			get { return ((mStatus & Status.NewStatus) != 0); }
		}

        /// <summary>
        /// Is the object dirty?
        /// </summary>
		public bool IsDirty
		{
			get { return ((mStatus & Status.Dirty) != 0); }
		}

        /// <summary>
        /// Has the object been deleted?
        /// </summary>
		public bool IsDeleted
		{
			get { return ((mStatus & Status.Deleted) != 0); }
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Constructor for the DBObject class
        /// </summary>
		protected DBObject()
		{
			mMemento = new Dictionary<string, object>();
			mRules = new ValidationRuleCollection();
			mDelegateRules = new DelegateRuleCollection();

			// Mark the object as new
			mStatus = Status.NewStatus | Status.Dirty;

			// Store the current status
			mMemento.Add("mStatus", mStatus);
		}

		#endregion

		#region Methods

		#region Data Access

		/// <summary>
		/// Based on the current status of the object, chooses whether to create, update or delete
		/// </summary>
		/// <returns>Any errors returned during the save attempt</returns>
		public virtual DBErrorCode Save()
		{
			if (IsDirty)
			{
				if (IsNew)
				{
					if (!IsDeleted)
					{
						return Create();
					}
					else
					{
						// Object is new, so it has not been saved to the database,
						// so we do not want to call the Delete() method.  However,
						// we do want to fire the delete event to ensure that the
						// object is correctly removed from any lists.
						OnDBObjectDeleted();
					}
				}
				else if (IsDeleted)
				{
					return Delete();
				}
				else
				{
					return Update();
				}
			}

			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		protected virtual DBErrorCode Create()
		{
			MarkOld();
			ResetUndo();
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		protected virtual DBErrorCode Update()
		{
			MarkOld();
			ResetUndo();
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		public virtual DBErrorCode Fetch()
		{
			MarkOld();
			ResetUndo();
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		protected virtual DBErrorCode Delete()
		{
			MarkOld();
			ResetUndo();

			// Raise delete event
			OnDBObjectDeleted();

			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Called when the object is deleted
		/// </summary>
		protected void OnDBObjectDeleted()
		{
			DBObjectDeletedEventHandler handler = DBObjectDeleted;
			if (handler != null)
			{
				handler(this);
			}
		}

		#endregion

		#region Status

		#region Framework Methods

		/// <summary>
		/// Marks the object as dirty
		/// </summary>
		public void MarkDirty()
		{
			OnMarkDirty();
			mStatus |= Status.Dirty;
		}

		/// <summary>
		/// Marks the object as new
		/// </summary>
		public void MarkNew()
		{
			OnMarkNew();
			mStatus = Status.NewStatus | Status.Dirty;
		}

		/// <summary>
		/// Marks the object as old
		/// </summary>
		public void MarkOld()
		{
			OnMarkOld();
			mStatus = (mStatus & Status.Deleted);
		}

		/// <summary>
		/// Marks the object as deleted
		/// </summary>
		public void MarkDeleted()
		{
			OnMarkDeleted();
			mStatus |= Status.Deleted | Status.Dirty;
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

		#region Memento Functions

		#region Framework Methods

		/// <summary>
		/// Should be called before properties are altered at the start of a group of property alterations that represent
		/// a single change transaction.
		/// This method calls "OnResetUndo()", which should be overridden if extra functionality is needed.
		/// </summary>
		public void ResetUndo()
		{
			// Call any user code
			OnResetUndo();

			ClearBackedUpState();
		}

		/// <summary>
		/// Clears the list of backed up properties and stores the current
		/// state of the object in the backup list.
		/// </summary>
		protected void ClearBackedUpState()
		{
			// Flush any existing undo data
			mMemento.Clear();

			// Store the current status
			mMemento.Add("mStatus", mStatus);
		}

		/// <summary>
		/// Restores the object to its state at the last call to "ResetUndo().
		/// </summary>
		/// <see cref="DBObject.RestoreProperties"/>
		public void Undo()
		{
			// Call any user code
			OnUndo();

			RestoreBackedUpState();
		}

		/// <summary>
		/// Restores the object to its state at the last call to "ResetUndo().
		/// 
		/// This can throw a TargetInvocationException error at the line containing "info.SetValue".
		/// This occurs if the attempt at resetting the property failed because the setter threw
		/// an exception.  If this happens, check:
		///  - That the value being restored does not violate any of the rules applied to the
		///    object (this can occur if the value is set using a member instead of a property)
		///  - That the value being restored is valid for the given datatype (this can occur if
		///    ResetUndo() was not called in the object's constructor, meaning non-nullable values
		///    are being restored to the default value, ie. null).
		/// 
		/// The TargetInvocationException is caught and wrapped in an UndoException.
		/// </summary>
		protected void RestoreBackedUpState() {

			// Get type of the object via reflection
			Type t = this.GetType();
			PropertyInfo[] infos = t.GetProperties();

			// Obtain lock to prevent object changing whilst existing changes are reverted
			lock (this)
			{
				// Loop through all backed up properties
				IEnumerator<string> keys = mMemento.Keys.GetEnumerator();

				while (keys.MoveNext())
				{
					object propertyValue = mMemento[keys.Current];

					// Loop through all properties
					foreach (PropertyInfo info in infos)
					{
						// Is this property writeable and the same as the key?
						if (info.CanWrite)
						{

							// Reset the property to the stored value
							if (info.Name == keys.Current)
							{
								try
								{
									info.SetValue(this, propertyValue, null);
								}
								catch (TargetInvocationException ex)
								{
									throw new UndoException("Error setting property of object whilst undoing changes.", ex);
								}
								break;
							}
						}
					}
				}

				// Restore the previous status
				if (mMemento.ContainsKey("mStatus"))
				{
					mStatus = (Status)mMemento["mStatus"];
				}
			}

			// Clear the backed up property list
			mMemento.Clear();
		}

		/// <summary>
		/// Backup the supplied value in the memento list.  Does not back up the value
		/// if a value for the property already exists in the memento list, ensuring that
		/// the only stored value is that which existed at the start of the current
		/// transaction.
		/// </summary>
		/// <typeparam name="T">Type of object to store</typeparam>
		/// <param name="propertyName">Name of the property for which the value is appropriate</param>
		/// <param name="value">Current value of the supplied property</param>
		protected void BackupProperty<T>(string propertyName, T value)
		{
			lock (mMemento)
			{
				if (!mMemento.ContainsKey(propertyName))
				{
					mMemento.Add(propertyName, value);
				}
			}
		}

		/// <summary>
		/// Fetch a property from the memento list.  If the value returned is a reference
		/// type it should not be changed.
		/// </summary>
		/// <typeparam name="T">The type of the property to retrieve.</typeparam>
		/// <param name="propertyName">The name of the property to retrieve.</param>
		/// <returns>The object if it exists, or the default value for the parameter
		/// type if not.</returns>
		/// <remarks>In this situation, it would be nice if C# offered C++'s ability to
		/// return const references.</remarks>
		protected T RetrieveBackupProperty<T>(string propertyName)
		{
			if (mMemento.ContainsKey(propertyName))
			{
				return (T)mMemento[propertyName];
			}

			// Return the closest thing to null we can for this type
			return default(T);
		}

		#endregion

		#region Stub Methods

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when ResetUndo() is called.
		/// </summary>
		protected virtual void OnResetUndo() { }

		/// <summary>
		/// Stub method that should be overridden if extra functionality is needed when Undo() is called.
		/// </summary>
		protected virtual void OnUndo() { }

		#endregion

		#endregion

		#region Property Changed

		/// <summary>
		/// Set a property and fire a change event.  Throws an exception if any validation rules are violated.
		/// </summary>
		/// <typeparam name="T">Type of the object</typeparam>
		/// <param name="propertyName">Name of the property being changed</param>
		/// <param name="oldValue">Reference to the value being updated</param>
		/// <param name="newValue">New value</param>
		protected void SetProperty<T>(string propertyName, ref T oldValue, T newValue)
		{
			lock (this)
			{
				// Acquire a lock for the current transaction
				mLock.AcquireLock();

				// Enlist the object in the current transaction, if it has not been enlisted already
				Enlist();

				// Are we trying to set a null value to null?
				if ((oldValue == null) && (newValue == null))
				{
					return;
				}

				// Is the value different to the old value?
				if ((oldValue == null) || (!oldValue.Equals((T)newValue)))
				{
					// Prepare a list in which to store validation error messages
					List<string> errorMessages = new List<string>();

					if (!Validate(propertyName, errorMessages, newValue))
					{

						// Validation failed - combine all error messages and throw an exception
						StringBuilder concatErrors = new StringBuilder();
						foreach (string err in errorMessages)
						{
							concatErrors.Append("\n - ");
							concatErrors.Append(err);
						}

						throw new ValidationException("New value '" + newValue.ToString() + "' for property '" + propertyName + "' violates rules:" + concatErrors.ToString());
					}

					// Validation succeeded - store the existing value of the property
					BackupProperty<T>(propertyName, oldValue);

					// Update the value
					oldValue = newValue;

					// Handle change event
					OnPropertyChanged(propertyName);
				}
			}
		}

		/// <summary>
		/// Called when a property is changed
		/// </summary>
		/// <param name="name">Name of the property that changed</param>
		protected void OnPropertyChanged(string name)
		{
			MarkDirty();

			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		#region Rules

		/// <summary>
		/// Validate the supplied value using all rules.
		/// </summary>
		/// <typeparam name="T">The type of the property to validate.</typeparam>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="errorMessages">Will contain any errors arising from the validation
		/// attempt once the function ends.</param>
		/// <param name="value">Value to validate.</param>
		/// <returns>True if the value is valid; false if not.</returns>
		protected bool Validate<T>(string propertyName, List<string> errorMessages, T value)
		{
			bool valid = true;

			// Validate new value against standard rules
			if (!mRules.Validate<T>(propertyName, errorMessages, value)) valid = false;

			// Validate new value against custom rules
			if (!mDelegateRules.Validate<T>(propertyName, errorMessages, value)) valid = false;

			return valid;
		}

		/// <summary>
		/// Add an IValidationRule object to the rule list.
		/// </summary>
		/// <param name="rule">The IValidation object to add to the list.</param>
		protected void AddRule(IValidationRule rule)
		{
			mRules.Add(rule);
		}

		/// <summary>
		/// Add a function delegate as a custom rule
		/// </summary>
		/// <param name="propertyName">The function that will validate the property</param>
		/// <param name="delegateFunction">The name of the property that the function validates</param>
		protected void AddRule(Validator delegateFunction, string propertyName)
		{
			mDelegateRules.Add(delegateFunction, propertyName);
		}

		#endregion

		#endregion

		#region Transactions

		#region IEnlistmentNotification Members

		public void Commit(Enlistment enlistment)
		{
			ResetUndo();
			mEnlisted = false;
			mLock.ReleaseLock();
			enlistment.Done();
		}

		public void InDoubt(Enlistment enlistment)
		{
			mLock.ReleaseLock();
			mEnlisted = false;
			enlistment.Done();
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Rollback(Enlistment enlistment)
		{
			Undo();
			mLock.ReleaseLock();
			mEnlisted = false;
			enlistment.Done();
		}

		#endregion

		/// <summary>
		/// Enlist the object in the transaction.  Resets the undo system to ensure that it will be rolled back
		/// to the current state in the event of the transaction aborting.
		/// </summary>
		private void Enlist()
		{
			lock (this)
			{
				// Add object to transaction
				if (Transaction.Current != null)
				{
					if (!mEnlisted)
					{
						ResetUndo();
						Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
						mEnlisted = true;
					}
				}
			}
		}

		/// <summary>
		/// Get the value of the specified property.  Returns the latest value for the current transaction, or the last known
		/// good value for other transactions.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="value">The property's current value, passed by reference.</param>
		/// <returns>The value of the property.</returns>
		protected T GetProperty<T>(string propertyName, ref T value) {
			if (Transaction.Current != null) {
				if (mLock.CurrentTransaction != Transaction.Current) {

					// Another transaction is modifying the object - if this property has been changed,
					// fetch the old value from the memento object
					if (mMemento.ContainsKey(propertyName))
					{
						return (T)mMemento[propertyName];
					}
				}
			}

			// Either current transaction is modifying transaction, or property has not changed, so
			// return current value
			return value;
		}

		#endregion
	}

	#region Enums

    /// <summary>
    /// List of possible database errors
    /// </summary>
	public enum DBErrorCode
	{
        /// <summary>
        /// No problems encountered during DB access
        /// </summary>
		Ok = 0,

        /// <summary>
        /// Record already exists
        /// </summary>
		AlreadyExists = 1,

        /// <summary>
        /// DB access failed for a non-specific reason
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Object was not saved to the database
        /// </summary>
        NotSaved = 3,

        /// <summary>
        /// Record does not exist
        /// </summary>
        DoesNotExist = 4,

        /// <summary>
        /// Requested DB operation was not permitted
        /// </summary>
        NotPermitted = 5
	}

	#endregion
}
