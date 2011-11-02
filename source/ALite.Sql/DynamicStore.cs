using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace ALite.Sql
{
	/// <summary>
	/// Dynamic object that stores property values.
	/// </summary>
	[Serializable]
	public class DynamicStore : DynamicObject, ISerializable, ICloneable
	{
		#region Members

		/// <summary>
		/// Dictionary that stores all data.
		/// </summary>
		private Dictionary<string, object> mData = new Dictionary<string, object>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the DynamicStore class.
		/// </summary>
		public DynamicStore()
		{
		}

		/// <summary>
		/// Initializes a new instance of the DynamicStore class.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DynamicStore(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry entry in info)
			{
				mData.Add(entry.Name, entry.Value);
			}
		}

		#endregion

		#region Methods

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return mData.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			mData.Add(binder.Name, value);
			return true;
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, object> pair in mData)
			{
				info.AddValue(pair.Key, pair.Value);
			}
		}

		/// <summary>
		/// Check if the specified property exists.
		/// </summary>
		/// <param name="property">The property to find.</param>
		/// <returns>True if the property exists.</returns>
		public bool HasProperty(string property)
		{
			return mData.ContainsKey(property);
		}

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="property">The name of the property.</param>
		/// <returns>The value of the property.</returns>
		public T GetProperty<T>(string property)
		{
			if (HasProperty(property)) return (T)mData[property];
			return default(T);
		}

		/// <summary>
		/// Sets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="property">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		public void SetProperty<T>(string property, T value)
		{
			mData[property] = value;
		}

		public void RemoveProperty(string property)
		{
			if (mData.ContainsKey(property)) mData.Remove(property);
		}

		public object Clone()
		{
			var result = new DynamicStore();

			foreach (string key in mData.Keys)
			{
				result.SetProperty(key, mData[key]);
			}

			return result;
		}

		#endregion
	}
}
