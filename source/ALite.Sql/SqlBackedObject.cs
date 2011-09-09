using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using ALite.Core;

namespace ALite.Sql
{
	/// <summary>
	/// Base class for objects that can be persisted to SQL Server.
	/// </summary>
	[Serializable]
	public abstract class SqlBackedObject : PersistedObject<ExpandoObject>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlBackedObject class.
		/// </summary>
		public SqlBackedObject()
			: base(new PropertyStore())
		{
		}

		#endregion
	}
}
