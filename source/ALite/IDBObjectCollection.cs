using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace ALite
{
	/// <summary>
	/// Interface that describes the DBObjectCollection class
	/// </summary>
	public interface IDBObjectCollection
	{
		/// <summary>
		/// Save the collection.
		/// </summary>
		void Save();

		/// <summary>
		/// Delete the collection.
		/// </summary>
		void Delete();
	}
}
