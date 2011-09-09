using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ALite;

namespace ALite.Sql
{
	/// <summary>
	/// Interface that describes the DataAccess class
	/// </summary>
	interface IDataAccess
	{
		#region Properties

		/// <summary>
		/// The procedure that should be called by the next Fetch() or Save().  Should automatically
		/// unset the InlineCode member variable.
		/// </summary>
		string Procedure
		{
			get;
			set;
		}

		/// <summary>
		/// Inline code that should be run by the next Fetch() or Save().  Should automatically
		/// unset the Procedure member variable.
		/// </summary>
		string InlineCode
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Fetch data from the data source using either the Procedure or InlineCode member
		/// variable as the instruction.  Keeps the connection open so that subsequent
		/// recordsets can be retrieved if necessary.  Returns all rows of the retrieved
		/// data as a list of dynamic objects.
		/// </summary>
		/// <returns>An list of expando objects containing the retrieved data.</returns>
		List<dynamic> Fetch();

		/// <summary>
		/// Fetch data from the data source using either the Procedure or InlineCode member
		/// variable as the instruction.  Keeps the connection open so that subsequent
		/// recordsets can be retrieved if necessary.  Returns the first row of the retrieved
		/// data as a dynamic object.
		/// </summary>
		/// <returns>An expando object containing the first row of the retrieved data.</returns>
		dynamic FetchOne();

		/// <summary>
		/// Send data to the data source and do not attempt to pull any data back.
		/// </summary>
		void Save();

		/// <summary>
		/// Add a parameter to the internal parameter list.  Parameters are sent to the data source when the Fetch() or
		/// Save() methods are called.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="data">The value of the parameter.</param>
		void AddParameter(string name, object data);

		/// <summary>
		/// Move to the next record set.
		/// </summary>
		/// <returns>True if the next record set was successfully retrieved.</returns>
		bool MoveToNextRecordSet();

		#endregion
	}
}
