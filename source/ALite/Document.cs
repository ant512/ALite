using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	/// <summary>
	/// The Document class is used by all DBObjects as a store for any data that should be persisted to
	/// the database.
	/// </summary>
	public class Document : System.Collections.DictionaryBase, ICloneable
	{
		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Document() { }

		#endregion

		#region Properties

		/// <summary>
		/// Get or set the value of the specified key.
		/// </summary>
		/// <param name="key">The key of the value to retrieve.</param>
		/// <returns>The value of the key.</returns>
		public object this[string key]
		{
			get
			{
				return Dictionary[key];
			}
			set
			{
				Dictionary[key] = value;
			}
		}

		/// <summary>
		/// Gets the collection of all keys.
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return (Dictionary.Keys);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a new key/value pair to the document.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		public void Add(string key, object value)
		{
			Dictionary.Add(key, value);
		}

		/// <summary>
		/// Check if the document contains the specified key.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns>True if the document contains the key; false if not.</returns>
		public bool Contains(string key)
		{
			return Dictionary.Contains(key);
		}

		/// <summary>
		/// Removes the key from the document.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			Dictionary.Remove(key);
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a clone of the document.
		/// </summary>
		/// <returns>A clone of the document.</returns>
		public object Clone()
		{
			Document clone = new Document();

			foreach (string key in Dictionary.Keys)
			{
				clone[key] = Dictionary[key];
			}

			return clone;
		}

		#endregion

		#endregion
	}
}
