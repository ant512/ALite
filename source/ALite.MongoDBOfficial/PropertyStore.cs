using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using ALite.Core;

namespace ALite.MongoDBOfficial
{
	/// <summary>
	/// Stores data in an ExpandoObject.  Also maintains an optional
	/// restore point, providing the ability to roll back to a previous
	/// version of the stored data.
	/// </summary>
	[Serializable]
	internal class PropertyStore : IPropertyStore<ExpandoObject>
	{
		#region Members

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

		public ExpandoObject Document
		{
			get { return mDocument; }
			private set { mDocument = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Replace the internal expando data store with the specified object.
		/// Restore point is discarded.
		/// </summary>
		/// <param name="data">Object containing data that will become the new
		/// data repository of this object.</param>
		public void InjectData(ExpandoObject data)
		{
			mDocument = CopyExpando(data);
			mRestorePoint = null;
		}

		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		public void SetRestorePoint()
		{
			mRestorePoint = CopyExpando(mDocument);
		}

		/// <summary>
		/// Sets the specified property to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="name">Name of the property.</param>
		/// <param name="value">The value to store.</param>
		public void SetProperty<T>(string name, T value)
		{
			var doc = mDocument as IDictionary<string, object>;
			doc[name] = value;
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="name">Name of the property.</param>
		/// <returns>The value of the property.</returns>
		public T GetProperty<T>(string name)
		{
			var doc = mDocument as IDictionary<string, object>;

			if (doc.ContainsKey(name)) return (T)doc[name];

			return default(T);
		}

		/// <summary>
		/// Removes the property from the store.
		/// </summary>
		/// <param name="name">The name of the property to remove.</param>
		public void RemoveProperty(string name)
		{
			var doc = mDocument as IDictionary<string, object>;

			if (doc.ContainsKey(name)) doc.Remove(name);
		}

		/// <summary>
		/// Reverts to the last saved restore point.
		/// </summary>
		public void RevertToRestorePoint()
		{
			if (mRestorePoint == null) return;
			mDocument = mRestorePoint;
			mRestorePoint = null;
		}

		/// <summary>
		/// Creates a shallow copy of the supplied expando object.
		/// </summary>
		/// <param name="obj">The object to copy.</param>
		/// <returns>A copy of the object.</returns>
		private static ExpandoObject CopyExpando(ExpandoObject obj)
		{
			var result = new ExpandoObject();

			var source = obj as IDictionary<string, object>;
			var dest = result as IDictionary<string, object>;

			foreach (string key in source.Keys)
			{
				dest.Add(key, source[key]);
			}

			return result;
		}

		#endregion
	}
}
