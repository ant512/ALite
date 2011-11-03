using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using ALite.Core;

namespace ALite.Sql
{
	/// <summary>
	/// Abstraction layer for SQL Server data access.
	/// </summary>
	public class DataAccess : IDisposable
	{
		#region Members

		/// <summary>
		/// Connection the the database.
		/// </summary>
		private SqlConnection mConnection;

		/// <summary>
		/// Database command.
		/// </summary>
		private SqlCommand mCommand;

		/// <summary>
		/// Database data reader.
		/// </summary>
		private SqlDataReader mDataReader;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the DataAccess class
		/// </summary>
		/// <param name="connection">The name of the connection in the connection strings section of the config file,
		/// or the string itself.</param>
		/// <param name="isConnectionName">Should be true if the connection parameter contains the name of the connection
		/// string, or false if the parameter contains the connection string itself.</param>
		public DataAccess(string connection, bool isConnectionName)
		{
			if (isConnectionName)
			{
				mConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connection].ToString());
			}
			else
			{
				mConnection = new SqlConnection(connection);
			}

			mCommand = mConnection.CreateCommand();
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the name of the SQL procedure to execute
        /// </summary>
		public string Procedure
		{
			get
			{
				return mCommand.CommandType == CommandType.StoredProcedure ? mCommand.CommandText : string.Empty;
			}

			set
			{
				mCommand.CommandText = value;
				mCommand.CommandType = CommandType.StoredProcedure;
			}
		}

		/// <summary>
		/// Gets or sets the inline SQL code to execute
		/// </summary>
		public string InlineCode
		{
			get
			{
				return mCommand.CommandType == CommandType.Text ? mCommand.CommandText : string.Empty;
			}

			set
			{
				mCommand.CommandText = value;
				mCommand.CommandType = CommandType.Text;
			}
		}

		#endregion

		#region Methods

		#region Parameters

		/// <summary>
		/// Return a specific parameter from the parameter list
		/// </summary>
		/// <param name="name">The name of the parameter to return</param>
		/// <returns>The SqlParameter with the supplied name</returns>
		public SqlParameter Parameters(string name)
		{
			return mCommand.Parameters[name];
		}

		/// <summary>
		/// Adds a parameter to the list
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <param name="data">The value of the parameter</param>
		public void AddParameter(string name, object data)
		{
			mCommand.Parameters.Add(new SqlParameter(name, data));
		}

		#endregion

		#region Database Interaction

		/// <summary>
		/// Fetch data from the data source using either the Procedure or InlineCode member
		/// variable as the instruction.  Keeps the connection open so that subsequent
		/// recordsets can be retrieved if necessary.  Returns the first row of the retrieved
		/// data as a dynamic object.
		/// </summary>
		/// <returns>An expando object containing the first row of the retrieved data.</returns>
		public DynamicStore FetchOne()
		{
			mConnection.Open();
			mDataReader = mCommand.ExecuteReader();
			if (mDataReader.Read())
			{
				return RecordToDynamic(mDataReader);
			}

			return null;
		}

		/// <summary>
		/// Fetch data from the data source using either the Procedure or InlineCode member
		/// variable as the instruction.  Keeps the connection open so that subsequent
		/// recordsets can be retrieved if necessary.  Returns all rows of the retrieved
		/// data as a list of dynamic objects.
		/// </summary>
		/// <returns>An list of expando objects containing the retrieved data.</returns>
		public List<DynamicStore> Fetch()
		{
			mConnection.Open();
			mDataReader = mCommand.ExecuteReader();
			return ToDynamicList(mDataReader);
		}

		/// <summary>
		/// Executes the command without returning any results
		/// </summary>
		public void Save()
		{
			mConnection.Open();
			mCommand.ExecuteNonQuery();
		}

		#endregion

		#region Data Navigation

		/// <summary>
		/// Move to the next record set.
		/// </summary>
		/// <returns>Whether or not the next recordset was retrieved successfully.</returns>
		public bool MoveToNextRecordSet()
		{
			return mDataReader.NextResult();
		}

		#endregion

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Dispose of the object
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose of the object
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			// Ensure that the data reader is shut down
			if (mDataReader != null) mDataReader.Close();

			// Clean up all Sql/etc objects
			try
			{
				if (disposing)
				{
					if (mCommand != null) mCommand.Dispose();
					if (mConnection != null) mConnection.Dispose();
					if (mDataReader != null) mDataReader.Dispose();
				}
			}
			finally
			{
				mCommand = null;
				mConnection = null;
				mDataReader = null;
			}
		}

		#endregion

		#region Expando Objects

		/// <summary>
		/// Converts a datareader row into an expando object.
		/// </summary>
		/// <param name="reader">The reader to extract the data from.</param>
		/// <returns>An expando object representing the datareader row.</returns>
		private static DynamicStore RecordToDynamic(IDataReader reader)
		{
			var output = new DynamicStore();

			for (int i = 0; i < reader.FieldCount; i++)
			{
				output.SetValue(reader.GetName(i), reader.IsDBNull(i) ? null : reader[i]);
			}

			return output;
		}

		/// <summary>
		/// Converts a recordset into a list of expando objects.
		/// </summary>
		/// <param name="reader">The reader to extract the data from.</param>
		/// <returns>A list of expando objects representing the datareader's recordset.</returns>
		private static List<DynamicStore> ToDynamicList(IDataReader reader)
		{
			var result = new List<DynamicStore>();

			while (reader.Read())
			{
				result.Add(RecordToDynamic(reader));
			}

			return result;
		}

		#endregion
	}
}
