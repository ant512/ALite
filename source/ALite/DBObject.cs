using System;
using System.Collections;
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
		#region Delegate Types

		/// <summary>
		/// Template for validation delegates
		/// </summary>
		/// <param name="propertyName">Name of the property being validated</param>
		/// <param name="errorMessage">Error message to return if the value is invalid</param>
		/// <param name="oldValue">The current value of the property</param>
		/// <param name="newValue">The new value of the property</param>
		/// <returns>True if valid, false if not</returns>
		public delegate bool ValidationDelegate(string propertyName, ref string errorMessage, object oldValue, object newValue);

		#endregion

		#region Structs

		[Serializable]
		private struct DelegateValidationRule
		{
			#region Members

			private ValidationDelegate mDelegate;
			private string mPropertyName;

			#endregion

			#region Properties

			public ValidationDelegate Function
			{
				get
				{
					return mDelegate;
				}
			}

			public string PropertyName
			{
				get
				{
					return mPropertyName;
				}
			}

			#endregion

			#region Constructors

			public DelegateValidationRule(ValidationDelegate function, string propertyName)
			{
				mDelegate = function;
				mPropertyName = propertyName;
			}

			#endregion
		}

		#endregion

		#region Enums

		[Flags]
		private enum Status : byte
		{
			New = 0x1,
			Dirty = 0x2,
			Deleted = 0x4
		}

		#endregion

		#region Members

		/// <summary>
        /// Date that the object was created
        /// </summary>
		private DateTime mCreated;

        /// <summary>
        /// Date that the object was updated
        /// </summary>
		private DateTime mUpdated;

        /// <summary>
        /// Status of the object as a bitmask; use the Status enum to unpack it
        /// </summary>
		private Status mStatus;

        /// <summary>
        /// Stores backup data for later restoration
        /// </summary>
		private SortedList mMemento;

		/// <summary>
		/// Event fired when a property changes value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// List of rules that properties are checked against before they are set
		/// </summary>
		private List<ValidationRule> mRules;

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private List<DelegateValidationRule> mDelegateRules;

		/// <summary>
		/// Set to true when the object is reverting to its previous state
		/// </summary>
		private bool mIsUndoing;

		#endregion

		#region Properties

        /// <summary>
        /// Is the object new?
        /// </summary>
		public bool IsNew
		{
			get { return ((mStatus & Status.New) != 0); }
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

        /// <summary>
        /// Date the object was created
        /// </summary>
		public DateTime Created
		{
			get { return mCreated; }
		}

        /// <summary>
        /// Date the object was updated
        /// </summary>
		public DateTime Updated
		{
			get { return mUpdated; }
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Constructor for the DBObject class
        /// </summary>
		protected DBObject()
		{
			mCreated = DateTime.Now;
			mUpdated = DateTime.Now;
			mMemento = new SortedList();
			mRules = new List<ValidationRule>();
			mDelegateRules = new List<DelegateValidationRule>();
			mIsUndoing = false;

			MarkNew();
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
			mStatus = Status.New | Status.Dirty;
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

			// Remember that we are entering the undo phase
			mIsUndoing = true;

			// Loop through all backed up properties
			IEnumerator keys = mMemento.Keys.GetEnumerator();

			while (keys.MoveNext())
			{
				string propertyKey = (string)keys.Current;
				object propertyValue = mMemento[propertyKey];

				// Loop through all properties
				for (int i = 0; i < infos.Length; i++)
				{
					// Is this property the same as the key?
					PropertyInfo info = infos[i];
					if (info.Name == propertyKey)
					{
						// Reset the property to the stored value
						if (info.CanWrite)
						{
							info.SetValue(this, propertyValue, null);
							break;
						}
					}
				}
			}

			// Restore the previous status
			mStatus = (Status)mMemento["mStatus"];

			// Clear the backed up property list
			mMemento.Clear();

			// Call any user code
			OnUndo();

			// Remember that the undo phase has finished
			mIsUndoing = false;
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
			// Are we trying to set a null value to null?
			if ((oldValue == null) && (newValue == null))
			{
				return;
			}

			if ((oldValue == null) || (!oldValue.Equals((T)newValue)))
			{
				// Validate new value against standard rules
				foreach (ValidationRule rule in mRules)
				{
					// Have we found a relevant rule?
					if (rule.PropertyName == propertyName)
					{
						// Is the object valid?
						if (!rule.Validate(newValue))
						{
							// Reset to former value
							throw new ValidationException("New value '" + newValue.ToString() + "' for property '" + propertyName + "' violates basic rule.");
						}
					}
				}

				// Validate new value against custom rules
				string errorMessage = "";

				foreach (DelegateValidationRule rule in mDelegateRules)
				{
					// Have we found a relevant rule?
					if (rule.PropertyName == propertyName)
					{
						// Is the value valid?
						if (!rule.Function(propertyName, ref errorMessage, oldValue, newValue))
						{
							// Reset to former value
							throw new ValidationException("New value '" + newValue.ToString() + "' for property '" + propertyName + "' violates custom rule: " + errorMessage);
						}
					}
				}

				T storedValue = oldValue;

				// Store backup of value if we're not undoing
				if (!mIsUndoing)
				{
					// Only store if a value does not yet exist
					if (!mMemento.ContainsKey(propertyName))
					{
						mMemento.Add(propertyName, storedValue);
					}
				}

				// Update the value
				oldValue = newValue;

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

		/// <summary>
		/// Add a rule to the property validation list
		/// </summary>
		/// <param name="type">Type of rule to add</param>
		/// <param name="validValue">Value to validate against</param>
		/// <param name="propertyName">Name of the property to validate</param>
		protected void AddRule(ValidationRule.RuleType type, int validValue, string propertyName)
		{
			mRules.Add(new ValidationRule(type, validValue, this, propertyName));
		}

		/// <summary>
		/// Add a function delegate as a custom rule
		/// </summary>
		/// <param name="function">The function that will validate the property</param>
		/// <param name="propertyName">The name of the property that the function validates</param>
		protected void AddRule(ValidationDelegate function, string propertyName)
		{
			mDelegateRules.Add(new DelegateValidationRule(function, propertyName));
		}

		#endregion

		#endregion
	}

	#region Interfaces

    /// <summary>
    /// Interface that describes the DBObject class
    /// </summary>
	public interface IDBObject
	{
		/// <summary>
		/// Property changed event
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

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
        /// Date that the object was created
        /// </summary>
		DateTime Created { get; }

        /// <summary>
        /// Date that the object was updated
        /// </summary>
		DateTime Updated { get; }

        /// <summary>
        /// Save the object to the database
        /// </summary>
        /// <returns>The outcome of the save attempt</returns>
		DBErrorCode Save();

        /// <summary>
        /// Fetch the object from the database
        /// </summary>
        /// <returns>The outcome of the fetch attempt</returns>
		DBErrorCode Fetch();

        /// <summary>
        /// Mark the object as old
        /// </summary>
		void MarkOld();

        /// <summary>
        /// Mark the object as new
        /// </summary>
		void MarkNew();

        /// <summary>
        /// Mark the object as deleted
        /// </summary>
		void MarkDeleted();

        /// <summary>
        /// Mark the object as dirty
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
	}

	#endregion

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
