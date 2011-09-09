using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using ALite;
using ObjectValidator;

namespace ALite.Sql
{
	/// <summary>
	/// Base class for objects that interact with the database.
	/// </summary>
	[Serializable]
	public abstract class SqlBackedObject : PersistedObject<ExpandoObject>
	{
		#region Constructors

		public SqlBackedObject()
			: base(new PropertyStore())
		{
		}

		#endregion
	}
}
