using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using ObjectValidator;

namespace ALite
{
	/// <summary>
	/// Base class for objects that interact with the database.
	/// </summary>
	[Serializable]
	public abstract class DBObject : IDBObject, INotifyPropertyChanged
	{
		#region Enums

		/// <summary>
		/// Lists all possible statuses for the object.  Primarily used to determine what
		/// to do when Save() is called.
		/// </summary>
		public enum Status
		{
			/// <summary>
			/// Object is newly created and does not exist in the data store.
			/// </summary>
			New,

			/// <summary>
			/// Object is identical to the data in the data store.
			/// </summary>
			Unmodified,

			/// <summary>
			/// Object exists in the data store but its properties have been altered.
			/// </summary>
			Modified,

			/// <summary>
			/// Object has been deleted from the data store.
			/// </summary>
			Deleted
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
		private Status mStatus = Status.New;

		/// <summary>
		/// List of rules that properties are checked against before they are set.
		/// </summary>
		private Validator mValidator = new Validator();

		/// <summary>
		/// Stores all data accessed via the GetProperty() and SetProperty() methods.
		/// </summary>
		private ExpandoObject mDocument = new ExpandoObject();

		/// <summary>
		/// Stores the state of the object after a call to SetRestorePoint().
		/// </summary>
		private ExpandoObject mRestorePoint;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the current status of the object.
		/// </summary>
		public Status State
		{
			get { return mStatus; }
			private set { mStatus = value; }
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
			switch (mStatus)
			{
				case Status.New:
					Create();
					break;
				case Status.Modified:
					Update();
					break;
				case Status.Unmodified:
				case Status.Deleted:
					break;
			}
		}

		/// <summary>
		/// Called by Create().  Should be overridden in subclasses to provide database insertion code.
		/// </summary>
		protected virtual void CreateData() { }

		/// <summary>
		/// Called by Update().  Should be overridden in subclasses to provide database update code.
		/// </summary>
		protected virtual void UpdateData() { }

		/// <summary>
		/// Called by Fetch().  Should be overridden in subclasses to provide database retrieval code.
		/// </summary>
		protected virtual void FetchData() { }

		/// <summary>
		/// Called by Delete().  Should be overridden in subclasses to provide database deletion code.
		/// </summary>
		protected virtual void DeleteData() { }

		/// <summary>
		/// Inserts object into the database.
		/// </summary>
		protected void Create()
		{
			CreateData();
			OnCreated();
		}

		/// <summary>
		/// Updates the object's database representation with the values stored in this object.
		/// </summary>
		protected void Update()
		{
			UpdateData();
			OnUpdated();
		}

		/// <summary>
		/// Fetches the object from the database.
		/// </summary>
		public void Fetch()
		{
			FetchData();
			OnFetched();
		}

		/// <summary>
		/// Deletes the object from the database.
		/// </summary>
		public void Delete()
		{
			DeleteData();
			OnDeleted();
		}

		/// <summary>
		/// Called when the object is created.
		/// </summary>
		protected void OnCreated()
		{
			TransitionState(Status.Unmodified);

			DBObjectCreatedEventHandler handler = DBObjectCreated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is updated.
		/// </summary>
		protected void OnUpdated()
		{
			TransitionState(Status.Unmodified);

			DBObjectUpdatedEventHandler handler = DBObjectUpdated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is deleted.
		/// </summary>
		protected void OnDeleted()
		{
			TransitionState(Status.Deleted);

			DBObjectDeletedEventHandler handler = DBObjectDeleted;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is fetched.
		/// </summary>
		protected void OnFetched()
		{
			TransitionState(Status.Unmodified);

			DBObjectFetchedEventHandler handler = DBObjectFetched;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Replace the internal expando data store with the specified object.
		/// </summary>
		/// <param name="data">Object containing data that will become the new
		/// data repository of this object.</param>
		protected void InjectData(dynamic data)
		{
			mDocument = data;

			// Reset status.  Since we are replacing the internal data store
			// with an entirely new data store, we don't know what state the
			// store is in.  We presume it has been fetched anew from the
			// database and so the object is unmodified.
			mStatus = Status.Unmodified;

			// We have to scrap the restore point because it too is no longer
			// relevant if we have replaced the data store.
			mRestorePoint = null;
		}

		#endregion

		#region Restore Point

		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		public void SetRestorePoint()
		{
			mRestorePoint = new ExpandoObject();

			var source = mDocument as IDictionary<string, object>;
			var dest = mRestorePoint as IDictionary<string, object>;

			foreach (string key in source.Keys)
			{
				dest.Add(key, source[key]);
			}

			// Ensure that the restore point contains the current state of the object
			dest.Add("mStatus", mStatus);

			OnSetRestorePoint();
		}

		/// <summary>
		/// Reverts to the last saved restore point.
		/// </summary>
		public void RevertToRestorePoint()
		{
			OnRevertToRestorePoint();

			mDocument = mRestorePoint;
			mRestorePoint = null;

			var dictionary = mDocument as IDictionary<string, object>;

			// Ensure we revert to the status of the document as it was when
			// we created the restore point.
			mStatus = (Status)dictionary["mStatus"];

			// We no longer need the backed-up status
			dictionary.Remove("mStatus");
		}

		/// <summary>
		/// Called when SetRestorePoint() runs.
		/// </summary>
		protected virtual void OnSetRestorePoint() { }

		/// <summary>
		/// Called when RevertToRestorePoint() runs.
		/// </summary>
		protected virtual void OnRevertToRestorePoint() { }

		/// <summary>
		/// Get a value from the restore point data.
		/// </summary>
		/// <typeparam name="T">Type of the value to return.</typeparam>
		/// <param name="propertyName">Name of the property to return.</param>
		/// <returns>The value of the property in the restore point, if available.</returns>
		protected T GetRestorePointValue<T>(string propertyName)
		{
			// Give up if we don't have a restore point to get a value from
			if (mRestorePoint == null) return default(T);

			var dictionary = mRestorePoint as IDictionary<string, object>;

			return (T)dictionary[propertyName];
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
				var dictionary = mDocument as IDictionary<string, object>;
				if (dictionary.Keys.Contains(propertyName)) return (T)dictionary[propertyName];
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
				if (mStatus == Status.Deleted)
				{
					throw new ArgumentException("Cannot alter deleted objects.");
				}

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
					throw new ObjectValidator.ValidationException(errorMessage);
				}

				// Is the value different to the old value?
				if ((oldValue == null) || (!oldValue.Equals((T)newValue)))
				{
					var dictionary = mDocument as IDictionary<string, object>;

					dictionary[propertyName] = newValue;

					// Handle change event
					OnPropertyChanged(propertyName);
				}
			}
		}

		/// <summary>
		/// Transition from the current state to the specified state.  Protects against
		/// illegal transitions, such as any state to "New" (only new, unsaved objects
		/// are new) or "Deleted" to any state (deleted objects cannot be modified).
		/// Throws an ArgumentException if an illegal transition is attempted.
		/// </summary>
		/// <param name="newState">The new state to switch to.</param>
		private void TransitionState(Status newState) {
			switch (newState)
			{
				case Status.Deleted:
					mStatus = Status.Deleted;
					break;

				case Status.Modified:
					switch (mStatus)
					{
						case Status.Modified:
						case Status.New:
							break;
						case Status.Unmodified:
							mStatus = Status.Modified;
							break;
						case Status.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;

				case Status.New:
					throw new ArgumentException("Objects cannot become new again.");

				case Status.Unmodified:
					switch (mStatus)
					{
						case Status.Modified:
						case Status.New:
							mStatus = Status.Unmodified;
							break;
						case Status.Unmodified:
							break;
						case Status.Deleted:
							throw new ArgumentException("Cannot alter deleted objects.");
					}
					break;
			}
		}

		/// <summary>
		/// Called when a property is changed
		/// </summary>
		/// <param name="name">Name of the property that changed</param>
		protected void OnPropertyChanged(string name)
		{
			TransitionState(Status.Modified);

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
			return mValidator.Validate<T>(propertyName, errorMessages, value);
		}

		/// <summary>
		/// Add an IValidationRule object to the rule list.
		/// </summary>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="rule">The IValidation object to add to the list.</param>
		protected void AddRule(string propertyName, IValidationRule rule)
		{
			mValidator.AddRule(propertyName, rule);
		}

		/// <summary>
		/// Add a function delegate as a custom rule
		/// </summary>
		/// <param name="propertyName">The name of the property that the function validates.</param>
		/// <param name="delegateFunction">The function that will validate the property.</param>
		protected void AddRule(string propertyName, ValidatorDelegate delegateFunction)
		{
			mValidator.AddRule(propertyName, delegateFunction);
		}

		#endregion

		#endregion
	}
}
