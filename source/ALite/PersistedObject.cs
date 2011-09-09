using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using ALite.ObjectValidator;

namespace ALite
{
	/// <summary>
	/// Base class for objects that interact with the database.
	/// </summary>
	[Serializable]
	public abstract class PersistedObject<PropertyStoreType> : IPersistable
	{
		#region Events

		/// <summary>
		/// Event fired when a property changes value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event fired when the object is deleted.
		/// </summary>
		public event PersistableDeletedEventHandler PersistableObjectDeleted;

		/// <summary>
		/// Object created event.
		/// </summary>
		public event PersistableCreatedEventHandler PersistableObjectCreated;

		/// <summary>
		/// Object updated event.
		/// </summary>
		public event PersistableUpdatedEventHandler PersistableObjectUpdated;

		/// <summary>
		/// Object fetched event.
		/// </summary>
		public event PersistableFetchedEventHandler PersistableObjectFetched;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the current status of the object.
		/// </summary>
		public ModificationState State
		{
			get { return StateTracker.State; }
		}

		protected IPropertyStore<PropertyStoreType> Properties
		{
			get;
			private set;
		}

		/// <summary>
		/// List of rules that properties are checked against before they are set.
		/// </summary>
		private Validator Validator
		{
			get;
			set;
		}

		private ModificationStateTracker StateTracker {
			get;
			set;
		}

		#endregion

		#region Constructors

		public PersistedObject(IPropertyStore<PropertyStoreType> propertyStore)
		{
			Properties = propertyStore;
			Validator = new Validator();
			StateTracker = new ModificationStateTracker();
		}

		#endregion

		#region Methods

		#region Data Access

		/// <summary>
		/// Based on the current status of the object, chooses whether to create or update.
		/// </summary>
		public void Save()
		{
			switch (State)
			{
				case ModificationState.New:
					Create();
					break;
				case ModificationState.Modified:
					Update();
					break;
				case ModificationState.Unmodified:
				case ModificationState.Deleted:
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
			StateTracker.TransitionState(ModificationState.Unmodified);
			RaiseCreatedEvent();
		}

		/// <summary>
		/// Updates the object's database representation with the values stored in this object.
		/// </summary>
		protected void Update()
		{
			UpdateData();
			StateTracker.TransitionState(ModificationState.Unmodified);
			RaiseUpdatedEvent();
		}

		/// <summary>
		/// Fetches the object from the database.
		/// </summary>
		public void Fetch()
		{
			FetchData();
			StateTracker.TransitionState(ModificationState.Unmodified);
			RaiseFetchedEvent();
		}

		/// <summary>
		/// Deletes the object from the database.
		/// </summary>
		public void Delete()
		{
			DeleteData();
			StateTracker.TransitionState(ModificationState.Deleted);
			RaiseDeletedEvent();
		}

		/// <summary>
		/// Called when the object is created.
		/// </summary>
		protected void RaiseCreatedEvent()
		{
			PersistableCreatedEventHandler handler = PersistableObjectCreated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is updated.
		/// </summary>
		protected void RaiseUpdatedEvent()
		{
			PersistableUpdatedEventHandler handler = PersistableObjectUpdated;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is deleted.
		/// </summary>
		protected void RaiseDeletedEvent()
		{
			PersistableDeletedEventHandler handler = PersistableObjectDeleted;
			if (handler != null)
			{
				handler(this);
			}
		}

		/// <summary>
		/// Called when the object is fetched.
		/// </summary>
		protected void RaiseFetchedEvent()
		{
			PersistableFetchedEventHandler handler = PersistableObjectFetched;
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
		protected void InjectData(PropertyStoreType data)
		{
			Properties.InjectData(data);

			// Reset status.  Since we are replacing the internal data store
			// with an entirely new data store, we don't know what state the
			// store is in.  We presume it has been fetched anew from the
			// database and so the object is unmodified.
			StateTracker = new ModificationStateTracker(ModificationState.Unmodified);
		}

		#endregion

		#region Restore Point

		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		public void SetRestorePoint()
		{
			// Ensure that the state is backed up in the restore point
			Properties.SetProperty("mState", State);
			Properties.SetRestorePoint();

			// We don't need the state to be in the property store, so we can remove it
			Properties.RemoveProperty("mState");
			OnSetRestorePoint();
		}

		/// <summary>
		/// Reverts to the last saved restore point.
		/// </summary>
		public void RevertToRestorePoint()
		{
			OnRevertToRestorePoint();
			Properties.RevertToRestorePoint();

			// Restore the backed up state
			StateTracker = new ModificationStateTracker(Properties.GetProperty<ModificationState>("mState"));

			// We no longer need the state to be in the property store
			Properties.RemoveProperty("mState");
		}

		/// <summary>
		/// Called when SetRestorePoint() runs.
		/// </summary>
		protected virtual void OnSetRestorePoint() { }

		/// <summary>
		/// Called when RevertToRestorePoint() runs.
		/// </summary>
		protected virtual void OnRevertToRestorePoint() { }

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
			lock (Properties)
			{
				return Properties.GetProperty<T>(propertyName);
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
			lock (Properties)
			{
				if (State == ModificationState.Deleted)
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
					Properties.SetProperty<T>(propertyName, newValue);
					StateTracker.TransitionState(ModificationState.Modified);
					RaisePropertyChangedEvent(propertyName);
				}
			}
		}

		/// <summary>
		/// Called when a property is changed.
		/// </summary>
		/// <param name="name">Name of the property that changed.</param>
		protected void RaisePropertyChangedEvent(string name)
		{
			lock (Properties)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(name));
				}
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

			string valueString = newValue == null ? "null" : newValue.ToString();

			return String.Format("New value '{0}' for property '{1}' violates rules: {2}", valueString, propertyName, concatErrors.ToString());
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
			return Validator.Validate<T>(propertyName, errorMessages, value);
		}

		/// <summary>
		/// Add an IValidationRule object to the rule list.
		/// </summary>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="rule">The IValidation object to add to the list.</param>
		protected void AddRule(string propertyName, IValidationRule rule)
		{
			Validator.AddRule(propertyName, rule);
		}

		/// <summary>
		/// Add a function delegate as a custom rule
		/// </summary>
		/// <param name="propertyName">The name of the property that the function validates.</param>
		/// <param name="delegateFunction">The function that will validate the property.</param>
		protected void AddRule(string propertyName, ValidatorDelegate delegateFunction)
		{
			Validator.AddRule(propertyName, delegateFunction);
		}

		#endregion

		#endregion
	}
}
