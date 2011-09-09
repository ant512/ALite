using System;
using System.Collections.Generic;
using System.Text;

namespace ALite.ObjectValidator
{
	/// <summary>
	/// Class for storing multiple values against a single key in a dictionary.  Dictionary
	/// internally stores TValue items in a list against each TKey.
	/// </summary>
	/// <typeparam name="TKey">Type of the key.</typeparam>
	/// <typeparam name="TValue">Type of the values stored.</typeparam>
	class DictionaryList<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
	{
		#region Members

		/// <summary>
		/// Dictionary used for storing lists of items
		/// </summary>
		private Dictionary<TKey, List<TValue>> mDictionary;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public DictionaryList()
		{
			mDictionary = new Dictionary<TKey, List<TValue>>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the list of items for the specified key.
		/// </summary>
		/// <param name="key">The key for which the list should be returned.</param>
		/// <returns>The list of items if it exists, or null if it does not.</returns>
		public List<TValue> Values(TKey key)
		{
			if (!mDictionary.ContainsKey(key)) return null;
			return mDictionary[key];
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		/// <summary>
		/// Add a key/value pair.
		/// </summary>
		/// <param name="item">A KeyValuePair object containing the key and value to add.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			// Attempt to find an existing list for the supplied key
			List<TValue> list;
			if (mDictionary.ContainsKey(item.Key))
			{
				list = mDictionary[item.Key];
			}
			else
			{
				list = new List<TValue>();
				mDictionary.Add(item.Key, list);
			}

			list.Add(item.Value);
		}

		/// <summary>
		/// Empties all data in the collection.
		/// </summary>
		public void Clear()
		{
			mDictionary.Clear();
		}

		/// <summary>
		/// Check if the collection contains the supplied key/value pair.
		/// </summary>
		/// <param name="item">The key/value pair to search for.</param>
		/// <returns>True if the pair exists; false if not.</returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			// Abort if the key does not exist
			if (!mDictionary.ContainsKey(item.Key)) return false;

			// Locate the list containing the item
			List<TValue> list = mDictionary[item.Key];

			// Locate the item in the list
			foreach (TValue listItem in list)
			{
				if (listItem.Equals(item.Value))
				{

					// Item found
					return true;
				}
			}

			// Item does not exist
			return false;
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Get the total number of values in the collection.
		/// </summary>
		public int Count
		{
			get
			{
				int itemCount = 0;

				foreach (TKey key in mDictionary.Keys)
				{
					itemCount += mDictionary[key].Count;
				}

				return itemCount;
			}
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Remove the key/value pair from the collection if it exists.
		/// </summary>
		/// <param name="item">The key/value pair to remove.</param>
		/// <returns>True if the item was removed; false if not.</returns>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			// Abort if list does not contain key
			if (!mDictionary.ContainsKey(item.Key)) return false;

			// Locate the relevant list
			List<TValue> list = mDictionary[item.Key];
			TValue listItem;

			// Locate the relevant item
			for (int i = 0; i < list.Count; ++i)
			{
				listItem = list[i];

				if (listItem.Equals(item.Value))
				{
					// Found the item
					list.Remove(listItem);
					return true;
				}
			}

			// Item does not exist
			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			//return mDictionary.GetEnumerator();
			throw new NotImplementedException("Not implemented");
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Get an enumerator.
		/// </summary>
		/// <returns>An enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mDictionary.GetEnumerator();
		}

		#endregion
	}
}
