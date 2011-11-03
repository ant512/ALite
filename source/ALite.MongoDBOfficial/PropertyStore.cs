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
	internal class PropertyStore : IPropertyStore<DynamicStore>
	{
		#region Members

		/// <summary>
		/// Stores all data accessed via the GetProperty() and SetProperty() methods.
		/// </summary>
		private DynamicStore mDocument = new DynamicStore();

		/// <summary>
		/// Stores the state of the object after a call to SetRestorePoint().
		/// </summary>
		private DynamicStore mRestorePoint;

		#endregion

		#region Properties

		public DynamicStore Document
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
		public void InjectData(DynamicStore data)
		{
			mDocument = data.Clone() as DynamicStore;
		}

		/// <summary>
		/// Stores the current state of the object for future restoral.
		/// </summary>
		public void SetRestorePoint()
		{
			mRestorePoint = mDocument.Clone() as DynamicStore;
		}

		/// <summary>
		/// Sets the specified property to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="name">Name of the property.</param>
		/// <param name="value">The value to store.</param>
		public void SetProperty<T>(string name, T value)
		{
			mDocument.SetValue<T>(name, value);
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="name">Name of the property.</param>
		/// <returns>The value of the property.</returns>
		public T GetProperty<T>(string name)
		{
			return mDocument.GetValue<T>(name);
		}

		/// <summary>
		/// Removes the property from the store.
		/// </summary>
		/// <param name="name">The name of the property to remove.</param>
		public void RemoveProperty(string name)
		{
			mDocument.RemoveValue(name);
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

		#endregion
	}
}
