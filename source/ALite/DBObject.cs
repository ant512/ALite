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
	/// Base class for objects that interact with the database.
	/// </summary>
	[Serializable] public abstract class DBObject : IDBObject, INotifyPropertyChanged
	{
		#region Enums

		[Flags]
		private enum Status : byte
		{
			IsNew = 0x1,
			IsDirty = 0x2,
			IsDeleted = 0x4
		}

		#endregion

		#region Members

		#region Events

		/// <summary>
		/// Event fired when a property changes value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired when the object is deleted.
		/// </summary>
		public event DBObjectDeletedEventHandler DBObjectDeleted;

		/// <summary>
		/// Object created event.
		/// </summary>
		public event DBObjectCreatedEventHandler DBObjectCreated;

		/// <summary>
		/// Object updated event.
		/// </summary>
		public event DBObjectUpdatedEventHandler DBObjectUpdated;

		/// <summary>
		/// Object fetched event.
		/// </summary>
		public event DBObjectFetchedEventHandler DBObjectFetched;

		#endregion

		/// <summary>
        /// Status of the object as a bitmask; use the Status enum to unpack it.
        /// </summary>
		private Status mStatus = Status.IsNew | Status.IsDirty;

		/// <summary>
		/// List of rules that properties are checked against before they are set.
		/// </summary>
		private ValidationRuleCollection mRules = new ValidationRuleCollection();

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private DelegateRuleCollection mDelegateRules = new DelegateRuleCollection();

		/// <summary>
		/// Stores all data accessed via the GetProperty() and SetProperty() methods.
		/// </summary>
		private Document mDocument = new Document();

		/// <summary>
		/// Stores the state of the object after a call to SetRestorePoint().
		/// </summary>
		private Document mRestorePoint;

		#endregion

		#region Properties

        /// <summary>
        /// Is the object new?
        /// </summary>
		public bool IsNew
		{
			get { return ((mStatus & Status.IsNew) != 0); }
		}

        /// <summary>
        /// Is the object dirty?
        /// </summary>
		public bool IsDirty
		{
			get { return ((mStatus & Status.IsDirty) != 0); }
		}

        /// <summary>
        /// Has the object been deleted?
        /// </summary>
		public bool IsDeleted
		{
			get { return ((mStatus & Status.IsDeleted) != 0); }
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
		protected DBObject() { }

		#endregion

		#region Methods

		#region Data Access

		/// <summary>
		/// Based on the current status of the object, chooses whether to create or update.
		/// </summary>
		public void Save()
		{
			if (IsDirty)
			{
				if (IsNew)
				{
					Create();
				}
				else
				{
					Update();
				}
			}
		}

		protected virtual void CreateData() { }
		protected virtual void UpdateData() { }
		protected virtual void FetchData() { }
		protected virtual void DeleteData() { }

		protected void Create()
		{
			CreateData();
			MarkOld();
			OnCreated();
		}

		protected void Update()
		{
			UpdateData();
			MarkOld();
			OnUpdated();
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden.
		/// </summary>
		/// <returns></returns>
		public void Fetch()
		{
			FetchData();
			MarkOld();
			OnFetched();
		}

		/// <summary>
		/// Marks the object as old; intended to be overriden.
		/// </summary>
		/// <returns></returns>
		public void Delete()
		{
			DeleteData();
			MarkDeleted();
			OnDeleted();
		}

		/// <summary>
		/// Called when the object is created.
		/// </summary>
		private void OnCreated()
		{
			DBObjectCreatedEventHandler handler = DBObjectCreated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is updated.
		/// </summary>
		private void OnUpdated()
		{
			DBObjectUpdatedEventHandler handler = DBObjectUpdated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is deleted.
		/// </summary>
		private void OnDeleted()
		{
			DBObjectDeletedEventHandler handler = DBObjectDeleted;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is fetched.
		/// </summary>
		private void OnFetched()
		{
			DBObjectFetchedEventHandler handler = DBObjectFetched;
			if (handler != null)
			{
				handler(this);
			}
		}

		#endregion

		#region Status

		/// <summary>
		/// Marks the object as dirty.
		/// </summary>
		protected void MarkDirty()
		{
			mStatus |= Status.IsDirty;
		}

		/// <summary>
		/// Marks the object as new.
		/// </summary>
		protected void MarkNew()
		{
			mStatus = Status.IsNew | Status.IsDirty;
		}

		/// <summary>
		/// Marks the object as old.
		/// </summary>
		protected void MarkOld()
		{
			mStatus = (mStatus & Status.IsDeleted);
		}

		/// <summary>
		/// Marks the object as deleted.
		/// </summary>
		protected void MarkDeleted()
		{
			mStatus = Status.IsDeleted;
		}

		#endregion

		#region Restore Point

		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		public void SetRestorePoint()
		{
			mRestorePoint = new Document();

			foreach (string key in mDocument.Keys)
			{
				mRestorePoint.Add(key, mDocument[key]);
			}
		}

		/// <summary>
		/// Reverts to the last saved restore point.
		/// </summary>
		public void RevertToRestorePoint()
		{
			mDocument = mRestorePoint;
			mRestorePoint = null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get a property.  Returns the default value for T if no property is currently set.
		/// </summary>
		/// <typeparam name="T">Type of the property to retrieve.</typeparam>
		/// <param name="propertyName">Name of the property to retrieve</param>
		/// <returns>The current value of the property.</returns>
		protected T GetProperty<T>(string propertyName)
		{
			lock (mDocument)
			{
				if (mDocument.Contains(propertyName)) return (T)mDocument[propertyName];
				return default(T);
			}
		}

		/// <summary>
		/// Set a property and fire a change event.  Throws an exception if any validation rules are violated.
		/// </summary>
		/// <typeparam name="T">Type of the object</typeparam>
		/// <param name="propertyName">Name of the property being changed</param>
		/// <param name="newValue">New value</param>
		protected void SetProperty<T>(string propertyName, T newValue)
		{
			lock (mDocument)
			{
				// Get the existing value from the document
				T oldValue = GetProperty<T>(propertyName);

				// Are we trying to set a null value to null?
				if ((oldValue == null) && (newValue == null))
				{
					return;
				}

				// Prepare a list in which to store validation error messages
				List<string> errorMessages = new List<string>();

				if (!Validate(propertyName, errorMessages, newValue))
				{
					// Validation failed - combine all error messages
					string errorMessage = ConcatenateValidationErrorMessages<T>(errorMessages, propertyName, newValue);

					// Indicate the error by throwing an exception
					throw new ValidationException(errorMessage);
				}

				// Is the value different to the old value?
				if ((oldValue == null) || (!oldValue.Equals((T)newValue)))
				{
					mDocument[propertyName] = newValue;

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

		/// <summary>
		/// Combines a list of error messages into a single string.
		/// </summary>
		/// <typeparam name="T">The type of the property being changed that caused the errors.</typeparam>
		/// <param name="errorMessages">The list of error messages.</param>
		/// <param name="propertyName">The name of the property being changed.</param>
		/// <param name="newValue">The new value being applied to the property.</param>
		/// <returns>A string containing all error messages in a user-friendly format.</returns>
		private string ConcatenateValidationErrorMessages<T>(List<string> errorMessages, string propertyName, T newValue)
		{
			StringBuilder concatErrors = new StringBuilder();
			foreach (string err in errorMessages)
			{
				concatErrors.Append("\n - ");
				concatErrors.Append(err);
			}

			return String.Format("New value '{0}' for property '{1}' violates rules: {2}", newValue.ToString(), propertyName, concatErrors.ToString());
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
	}
}
