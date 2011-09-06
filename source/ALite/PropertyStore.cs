﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace ALite
{
	/// <summary>
	/// Stores data in an ExpandoObject.  Also maintains an optional
	/// restore point, providing the ability to roll back to a previous
	/// version of the stored data.
	/// </summary>
	class PropertyStore
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

		#region Methods

		/// <summary>
		/// Replace the internal expando data store with the specified object.
		/// Restore point is discarded.
		/// </summary>
		/// <param name="data">Object containing data that will become the new
		/// data repository of this object.</param>
		public void InjectData(dynamic data)
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

		public void SetProperty<T>(string name, T value)
		{
			var doc = mDocument as IDictionary<string, object>;

			doc[name] = value;
		}

		public T GetProperty<T>(string name)
		{
			var doc = mDocument as IDictionary<string, object>;

			if (doc.ContainsKey(name)) return (T)doc[name];

			return default(T);
		}

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
			mDocument = mRestorePoint;
			mRestorePoint = null;
		}

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