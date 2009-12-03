using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Maintains all data regarding a DBObject transaction.
	/// </summary>
	class TransactionData
	{
		#region Members

		/// <summary>
		/// Identify if the object is rolling back changes or behaving normally.
		/// </summary>
		bool mIsRollingBack = false;

		/// <summary>
		/// True if the transaction in progress has failed and needs to be rolled back.
		/// </summary>
		bool mHasTransactionFailed = false;

		/// <summary>
		/// List of error messages associated with the current transaction.
		/// </summary>
		List<string> mErrorMessages = new List<string>();

		/// <summary>
		/// Stores backup data for later restoration.
		/// </summary>
		private Dictionary<string, object> mMemento = new Dictionary<string, object>();

		#endregion

		#region Properties

		/// <summary>
		/// Check if the object is currently rolling back changes.
		/// </summary>
		public bool IsRollingBack
		{
			get { return mIsRollingBack; }
			set { mIsRollingBack = value; }
		}

		/// <summary>
		/// Check if the current transaction has failed.
		/// </summary>
		public bool HasTransactionFailed
		{
			get { return mHasTransactionFailed; }
			set { mHasTransactionFailed = value; }
		}

		/// <summary>
		/// Get a list of error messages created during the current transaction.
		/// </summary>
		public List<string> ErrorMessages
		{
			get { return mErrorMessages; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Restores the object to its previous state.
		/// 
		/// This can throw a TargetInvocationException error at the line containing "info.SetValue".
		/// This occurs if the attempt at resetting the property failed because the setter threw
		/// an exception.  If this happens, check:
		///  - That the value being restored does not violate any of the rules applied to the
		///    object (this can occur if the value is set using a member instead of a property)
		///  - That the value being restored is valid for the given datatype (this can occur if
		///    Commit() was not called in the object's constructor, meaning non-nullable values
		///    are being restored to the default value, ie. null).
		/// 
		/// The TargetInvocationException is caught and wrapped in an UndoException.
		/// </summary>
		public void Restore(IDBObject obj)
		{
			// Get type of the object via reflection
			Type type = obj.GetType();
			PropertyInfo[] properties = type.GetProperties();

			// Obtain lock to prevent object changing whilst existing changes are reverted
			lock (mMemento)
			{
				// Loop through all backed up properties
				IEnumerator<string> keys = mMemento.Keys.GetEnumerator();

				while (keys.MoveNext())
				{
					object propertyValue = mMemento[keys.Current];

					// Loop through all properties
					foreach (PropertyInfo item in properties)
					{
						// Is this property writeable and the same as the key?
						if (item.CanWrite)
						{

							// Reset the property to the stored value
							if (item.Name == keys.Current)
							{
								if (keys.Current == "ID")
								{
									System.Diagnostics.Debug.WriteLine(keys.Current);
								}

								try
								{
									item.SetValue(obj, propertyValue, null);
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
			}
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
		public void BackupProperty<T>(string propertyName, T value)
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
		/// Clears the list of backed up properties.
		/// </summary>
		public void ClearBackup()
		{
			lock (mMemento)
			{
				// Flush any existing undo data
				mMemento.Clear();
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
		public T RetrieveProperty<T>(string propertyName)
		{
			lock (mMemento)
			{
				if (mMemento.ContainsKey(propertyName))
				{
					return (T)mMemento[propertyName];
				}

				// Return the closest thing to null we can for this type
				return default(T);
			}
		}

		/// <summary>
		/// Check if the specified property is stored in the backup list.
		/// </summary>
		/// <param name="propertyName">The property to check for.</param>
		/// <returns>True if the property is stored, or false if not.</returns>
		public bool ContainsProperty(string propertyName)
		{
			lock (mMemento)
			{
				return (mMemento.ContainsKey("propertyName"));
			}
		}

		/// <summary>
		/// Reset all transaction data.
		/// </summary>
		public void Reset()
		{
			lock (this)
			{
				ClearBackup();
				ClearErrorMessages();
				mHasTransactionFailed = false;
				mIsRollingBack = false;
			}
		}

		/// <summary>
		/// Add an error message to the list.
		/// </summary>
		/// <param name="message">The message to add.</param>
		public void AddErrorMessage(string message)
		{
			lock (mErrorMessages)
			{
				mErrorMessages.Add(message);
			}
		}

		/// <summary>
		/// Clear the list of error messages.
		/// </summary>
		protected void ClearErrorMessages()
		{
			lock (mErrorMessages)
			{
				mErrorMessages.Clear();
			}
		}

		#endregion
	}
}
