using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using ALite.Core;

namespace ALite.Sql
{
	/// <summary>
	/// Base class for objects that can be persisted to SQL Server.
	/// </summary>
	[Serializable]
	public abstract class SqlBackedObject : PersistedObject<DynamicStore>
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
