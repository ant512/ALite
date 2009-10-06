using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace ALite
{
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

        /// <summary>
        /// Status of the object as a bitmask; use the Status enum to unpack it
        /// </summary>
		private Status mStatus;

        /// <summary>
        /// Stores backup data for later restoration
        /// </summary>
		private Dictionary<string, object> mMemento;

		/// <summary>
		/// Event fired when a property changes value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// List of rules that properties are checked against before they are set
		/// </summary>
		private ValidationRuleCollection mRules;

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private DelegateRuleCollection mDelegateRules;

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
			return DBErrorCode.Ok;
		}

		#endregion

		#region Status

		#region Framework Methods

		/// <summary>
		/// Marks the object as dirty
		/// </summary>
		public void MarkDirty()
		{
			mStatus |= Status.Dirty;
			OnMarkDirty();
		}

		/// <summary>
		/// Marks the object as new
		/// </summary>
		public void MarkNew()
		{
			mStatus = Status.NewStatus | Status.Dirty;
			OnMarkNew();
		}

		/// <summary>
		/// Marks the object as old
		/// </summary>
		public void MarkOld()
		{
			mStatus = (mStatus & Status.Deleted);
			OnMarkOld();
		}

		/// <summary>
		/// Marks the object as deleted
		/// </summary>
		public void MarkDeleted()
		{
			mStatus |= Status.Deleted | Status.Dirty;
			OnMarkDeleted();
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
			// Flush any existing undo data
			mMemento.Clear();

			// Store the current status
			mMemento.Add("mStatus", mStatus);

			// Call any user code
			OnResetUndo();
		}

		/// <summary>
		/// Restores the object to its state at the last call to "ResetUndo().
		/// </summary>
		public void Undo()
		{
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
								info.SetValue(this, propertyValue, null);
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

			// Call any user code
			OnUndo();
		}

		/// <summary>
		/// Backup the supplied value in the memento list.  Does not back up the value
		/// if a value for the property already exists in the memento list.
		/// </summary>
		/// <typeparam name="T">Type of object to store</typeparam>
		/// <param name="propertyName">Name of the property for which the value is appropriate</param>
		/// <param name="value">Current value of the supplied property</param>
		protected void BackupValue<T>(string propertyName, T value)
		{
			lock (mMemento)
			{
				if (!mMemento.ContainsKey(propertyName))
				{
					mMemento.Add(propertyName, value);
				}
			}
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
				// Are we trying to set a null value to null?
				if ((oldValue == null) && (newValue == null))
				{
					return;
				}

				if ((oldValue == null) || (!oldValue.Equals((T)newValue)))
				{
					string errorMessage = "";

					// Validate new value against standard rules
					if (!mRules.Validate<T>(propertyName, ref errorMessage, newValue))
					{
						// Reset to former value
						throw new ValidationException("New value '" + newValue.ToString() + "' for property '" + propertyName + "' violates rule: " + errorMessage);
					}

					// Validate new value against custom rules
					if (!mDelegateRules.Validate<T>(propertyName, ref errorMessage, oldValue, newValue))
					{
						// Reset to former value
						throw new ValidationException("New value '" + newValue.ToString() + "' for property '" + propertyName + "' violates rule: " + errorMessage);
					}		

					// Store the existing value of the property
					BackupValue<T>(propertyName, oldValue);

					// Update the value
					oldValue = newValue;
				}

				// Handle change event
				OnPropertyChanged(propertyName);
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
			mDelegateRules.Add(new DelegateValidationRule(delegateFunction, propertyName));
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
