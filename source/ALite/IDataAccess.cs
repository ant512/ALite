using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ALite
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
		/// Fetch data from the data source using either the Procedure or InlineCode member variable as the instruction.
		/// Note that data can be sent to the data source as part of the Fetch routine.  Should automatically move to the
		/// first record set and read in the first row.
		/// </summary>
		/// <returns>True if data was successfully returned.</returns>
		bool Fetch();

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
		/// Get a guid from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the guid.</param>
		/// <returns>The guid.</returns>
		Guid GetGuid(string ordinal);

		/// <summary>
		/// Get a string from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the string.</param>
		/// <returns>The string.</returns>
		string GetString(string ordinal);

		/// <summary>
		/// Get an int16 from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the int16.</param>
		/// <returns>The int16.</returns>
		Int16 GetInt16(string ordinal);

		/// <summary>
		/// Get an int32 from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the int32.</param>
		/// <returns>The int32.</returns>
		Int32 GetInt32(string ordinal);

		/// <summary>
		/// Get an int64 from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the int64.</param>
		/// <returns>The int64.</returns>
		Int64 GetInt64(string ordinal);

		/// <summary>
		/// Get a datetime from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the datetime.</param>
		/// <returns>The datetime.</returns>
		DateTime GetDateTime(string ordinal);

		/// <summary>
		/// Get a byte from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the byte.</param>
		/// <returns>The byte.</returns>
		byte GetByte(string ordinal);

		/// <summary>
		/// Get a bool from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the bool.</param>
		/// <returns>The bool.</returns>
		bool GetBoolean(string ordinal);

		/// <summary>
		/// Get a double from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the double.</param>
		/// <returns>The double.</returns>
		double GetDouble(string ordinal);

		/// <summary>
		/// Get a decimal from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the decimal.</param>
		/// <returns>The decimal.</returns>
		decimal GetDecimal(string ordinal);

		/// <summary>
		/// Get a single from the retrieved data.
		/// </summary>
		/// <param name="ordinal">The name of the field containing the single.</param>
		/// <returns>The single.</returns>
		float GetSingle(string ordinal);

		/// <summary>
		/// Move to the next record set.
		/// </summary>
		/// <returns>True if the next record set was successfully retrieved.</returns>
		bool MoveToNextRecordSet();

		/// <summary>
		/// Move to the next record within the current record set.
		/// </summary>
		/// <returns>True if the next record was successfully retrieved.</returns>
		bool MoveToNextRecord();

		#endregion
	}
}
