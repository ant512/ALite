using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace ALite.Core
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
		/// Check if the specified value exists.
		/// </summary>
		/// <param name="name">The value to find.</param>
		/// <returns>True if the value exists.</returns>
		public bool HasValue(string name)
		{
			return mData.ContainsKey(name);
		}

		/// <summary>
		/// Gets the value for the specified name.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="name">The name of the value.</param>
		/// <returns>The value.</returns>
		public T GetValue<T>(string name)
		{
			if (HasValue(name)) return (T)mData[name];
			return default(T);
		}

		/// <summary>
		/// Sets the value for the specified name.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="name">The name of the value.</param>
		/// <param name="value">The value.</param>
		public void SetValue<T>(string name, T value)
		{
			mData[name] = value;
		}

		/// <summary>
		/// Removes the value from the store.
		/// </summary>
		/// <param name="name">The name of the value to remove.</param>
		public void RemoveValue(string name)
		{
			if (mData.ContainsKey(name)) mData.Remove(name);
		}

		public object Clone()
		{
			var result = new DynamicStore();

			foreach (string key in mData.Keys)
			{
				result.SetValue(key, mData[key]);
			}

			return result;
		}

		#endregion
	}
}
