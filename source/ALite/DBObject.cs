using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
	[Serializable] public abstract class DBObject : IDBObject, INotifyPropertyChanged
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
		/// List of rules that properties are checked against before they are set
		/// </summary>
		private ValidationRuleCollection mRules;

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private DelegateRuleCollection mDelegateRules;

		TransactionData mTransactionData = null;

		#endregion

		#region Properties

        /// <summary>
        /// Is the object new?
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
        /// Is the object dirty?
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
        /// Has the object been deleted?
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
			get { return (mTransactionData != null); }
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Constructor for the DBObject class
        /// </summary>
		protected DBObject()
		{
			mRules = new ValidationRuleCollection();
			mDelegateRules = new DelegateRuleCollection();

			// Mark the object as new
			mStatus = Status.NewStatus | Status.Dirty;
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
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		protected virtual DBErrorCode Update()
		{
			MarkOld();
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		public virtual DBErrorCode Fetch()
		{
			MarkOld();
			return DBErrorCode.Ok;
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden
		/// </summary>
		/// <returns></returns>
		protected virtual DBErrorCode Delete()
		{
			MarkOld();

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
		/// Restores the object to its state at the start of the current transaction.
		/// 
		/// This can throw a TargetInvocationException error at the line containing "info.SetValue".
		/// This occurs if the attempt at resetting the property failed because the setter threw
		/// an exception.  If this happens, check:
		///  - That the value being restored is valid for the given datatype (this can occur if
		///    Commit() was not called in the object's constructor, meaning non-nullable values
		///    are being restored to the default value, ie. null).
		/// 
		/// The TargetInvocationException is caught and wrapped in an UndoException.
		/// </summary>
		protected void RestoreBackedUpState()
		{
			lock (this)
			{
				if (IsTransactionInProgress)
				{
					// Retrieve the old status - this cannot be restored automatically as the property has no public setter
					Status? oldStatus = null;
					if (mTransactionData.ContainsProperty("mStatus")) oldStatus = mTransactionData.RetrieveProperty<Status>("mStatus");

					// Restore all of the old properties that have public setters
					mTransactionData.Restore(this);

					// Restore the old status
					if (oldStatus != null) mStatus = (Status)oldStatus;
				}
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
				// Use shortcut and bypass rules if the object is rolling back to a previous state
				if (IsTransactionInProgress)
				{
					if (mTransactionData.IsRollingBack)
					{
						oldValue = newValue;
						return;
					}
				}

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
						// Remember that the transaction failed
						if (IsTransactionInProgress) mTransactionData.HasTransactionFailed = true;

						// Validation failed - combine all error messages and throw an exception
						StringBuilder concatErrors = new StringBuilder();
						string errorMessage = "";
						foreach (string err in errorMessages)
						{
							concatErrors.Append("\n - ");
							concatErrors.Append(err);
						}

						errorMessage = String.Format("New value '{0}' for property '{1}' violates rules: {2}", newValue.ToString(), propertyName, concatErrors.ToString());

						if (IsTransactionInProgress) mTransactionData.AddErrorMessage(errorMessage);

						throw new ValidationException(errorMessage);
					}

					// Validation succeeded - store the existing value of the property
					if (IsTransactionInProgress) mTransactionData.BackupProperty<T>(propertyName, oldValue);

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
			lock (mRules)
			{
				if (!mRules.Validate<T>(propertyName, errorMessages, value)) valid = false;
			}

			// Validate new value against custom rules
			lock (mDelegateRules)
			{
				if (!mDelegateRules.Validate<T>(propertyName, errorMessages, value)) valid = false;
			}

			return valid;
		}

		/// <summary>
		/// Add an IValidationRule object to the rule list.
		/// </summary>
		/// <param name="rule">The IValidation object to add to the list.</param>
		protected void AddRule(IValidationRule rule)
		{
			lock (mRules)
			{
				mRules.Add(rule);
			}
		}

		/// <summary>
		/// Add a function delegate as a custom rule
		/// </summary>
		/// <param name="propertyName">The function that will validate the property</param>
		/// <param name="delegateFunction">The name of the property that the function validates</param>
		protected void AddRule(Validator delegateFunction, string propertyName)
		{
			lock (mDelegateRules)
			{
				mDelegateRules.Add(delegateFunction, propertyName);
			}
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Begin a new transaction.  If this is not called before the object is modified it will
		/// not be possible to roll back the object to its initial state if any of the modifications
		/// do not work.
		/// </summary>
		public void BeginTransaction()
		{
			mTransactionData = new TransactionData();

			// Store the current status
			mTransactionData.BackupProperty<Status>("mStatus", mStatus);
		}

		/// <summary>
		/// Finish a transaction.
		/// </summary>
		public void EndTransaction()
		{
			mTransactionData = null;
		}

		/// <summary>
		/// Commit the transaction.  Calls OnCommit(), which can contain user code, before it erases all
		/// record of the previous state of the object.
		/// </summary>
		public void Commit()
		{
			lock (this)
			{
				OnCommit();
				if (IsTransactionInProgress) mTransactionData.Reset();
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
				if (IsTransactionInProgress) mTransactionData.IsRollingBack = true;

				RestoreBackedUpState();
				OnRollback();

				if (IsTransactionInProgress) mTransactionData.Reset();
			}
		}

		#endregion

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
