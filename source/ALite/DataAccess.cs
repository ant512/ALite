using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

[assembly: CLSCompliant(true)]

namespace ALite
{
	/// <summary>
	/// Abstraction layer for SQL Server data access.
	/// </summary>
	[Serializable]
	public class DataAccess : IDisposable, IDataAccess
	{
		#region Members

		[NonSerialized]
		private SqlConnection mConnection;

		[NonSerialized]
		private SqlCommand mCommand;
		private string mProcedure;
		private string mInlineCode;
		private List<SqlParameter> mParameters;

		[NonSerialized]
		private SqlDataReader mDataReader;

		[NonSerialized]
		private SqlTransaction mTransaction;
		private bool mUseTransactions;

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the name of the SQL procedure to execute
        /// </summary>
		public string Procedure
		{
			get { return mProcedure; }
			set
			{
				mProcedure = value;
				mInlineCode = "";
			}
		}

		/// <summary>
		/// Gets or sets the inline SQL code to execute
		/// </summary>
		public string InlineCode
		{
			get { return mInlineCode; }
			set
			{
				mInlineCode = value;
				mProcedure = "";
			}
		}

		/// <summary>
		/// Specify whether or not the object uses transactions when interacting with the DB
		/// </summary>
		public bool UseTransactions
		{
			get { return mUseTransactions; }
			set { mUseTransactions = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for the DataAccess class
		/// </summary>
		/// <param name="connection">The name of the connection in the connection strings section of the config file,
		/// or the string itself.</param>
		/// <param name="isConnectionName">Should be true if the connection parameter contains the name of the connection
		/// string, or false if the parameter contains the connection string itself.</param>
		public DataAccess(string connection, bool isConnectionName)
		{
			if (isConnectionName)
			{
				this.mConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connection].ToString());
			}
			else
			{
				this.mConnection = new SqlConnection(connection);
			}
			this.mCommand = mConnection.CreateCommand();
			this.mParameters = new List<SqlParameter>();
			this.mProcedure = "";
			this.mInlineCode = "";
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
			this.mParameters.Add(new SqlParameter(name, data));
		}

		#endregion

		#region Database Interaction

		/// <summary>
		/// Open the connection to the database
		/// </summary>
		private void Open() 
		{
			mConnection.Open();

			if (mUseTransactions) mTransaction = mConnection.BeginTransaction();

			mCommand.Connection = mConnection;

			if (mUseTransactions) mCommand.Transaction = mTransaction;

			// Choose type of command to run - sproc or SQL code
			if (!String.IsNullOrEmpty(mProcedure))
			{
				// Sproc
				mCommand.CommandType = CommandType.StoredProcedure;
				mCommand.CommandText = mProcedure;
			}
			else
			{
				// Raw SQL code
				mCommand.CommandType = CommandType.Text;
				mCommand.CommandText = mInlineCode;
			}

			// Add parameters
			foreach (SqlParameter item in mParameters)
			{
				mCommand.Parameters.Add(item);
			}
		}

		/// <summary>
		/// Executes the command contained within this object, keeping the connection open
		/// so that the results can be read externally
		/// </summary>
		/// <returns>True if successful; false otherwise</returns>
		public bool Fetch()
		{
			bool success;

			Open();

			try
			{
				mDataReader = mCommand.ExecuteReader();
				success = mDataReader.Read();
			}
			catch
			{
				if (mUseTransactions)
				{
					try
					{
						mTransaction.Rollback();
					}
					catch
					{
						throw;
					}
				}

				throw;
			}

			return success;
		}

		/// <summary>
		/// Executes the command without returning any results
		/// </summary>
		public void Save()
		{
			Open();

			try
			{
				mCommand.ExecuteNonQuery();
			}
			catch
			{
				if (mUseTransactions)
				{
					try
					{
						mTransaction.Rollback();
					}
					catch
					{
						throw;
					}
				}

				throw;
			}
		}

		#endregion

		#region Data Retrieval

		/// <summary>
		/// Check if the results set contains a column with the supplied name.
		/// </summary>
		/// <param name="name">Name of the column to find.</param>
		/// <returns>True if the results set contains the specified column.</returns>
		public bool ContainsColumn(string name)
		{
			for (int i = 0; i < mDataReader.VisibleFieldCount; ++i)
			{
				if (mDataReader.GetName(i).Equals(name))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets a guid from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public Guid GetGuid(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return Guid.Empty;
            }
            else
            {
                return mDataReader.GetGuid(index);
            }
		}

		/// <summary>
		/// Gets a string from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public string GetString(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return "";
            }
            else
            {
                return mDataReader.GetString(index);
            }
		}

		/// <summary>
		/// Gets a 16-bit int from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public Int16 GetInt16(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetInt16(index);
            }
		}

		/// <summary>
		/// Gets a 32-bit int from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public Int32 GetInt32(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetInt32(index);
            }
		}

        /// <summary>
        /// Gets a 64-bit int from the results
        /// </summary>
        /// <param name="ordinal">The name of the field to return</param>
        /// <returns>The requested value</returns>
        public Int64 GetInt64(string ordinal)
        {
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetInt64(index);
            }
        }

		/// <summary>
		/// Gets a datetime from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public DateTime GetDateTime(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return DateTime.MinValue;
            }
            else
            {
                return mDataReader.GetDateTime(index);
            }
		}

		/// <summary>
		/// Gets a byte from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public byte GetByte(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetByte(index);
            }
		}

		/// <summary>
		/// Gets a bool from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public bool GetBoolean(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return false;
            }
            else
            {
                return mDataReader.GetBoolean(index);
            }
		}

		/// <summary>
		/// Gets a double from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public double GetDouble(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetDouble(index);
            }
		}

		/// <summary>
		/// Gets a decimal from the results
		/// </summary>
		/// <param name="ordinal">The name of the field to return</param>
		/// <returns>The requested value</returns>
		public decimal GetDecimal(string ordinal)
		{
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetDecimal(index);
            }
		}

        /// <summary>
        /// Gets a single from the results
        /// </summary>
        /// <param name="ordinal">The name of the field to return</param>
        /// <returns>The requested value</returns>
        public float GetSingle(string ordinal)
        {
            int index = mDataReader.GetOrdinal(ordinal);

            if (mDataReader.IsDBNull(index))
            {
                return 0;
            }
            else
            {
                return mDataReader.GetFloat(index);
            }
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

        /// <summary>
        /// Move to the next row in the record set.
        /// </summary>
        /// <returns>Whether or not the next row was retrieved successfully.</returns>
		public bool MoveToNextRecord()
		{
			return mDataReader.Read();
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

			// Ensure that the transaction is committed to the DB
			if (mUseTransactions)
			{
				try
				{
					mTransaction.Commit();
				}
				catch
				{
					// Major problem - could not commit!
					throw;
				}
			}

			// Clean up all Sql/etc objects
			try
			{
				if (disposing)
				{
					if (mCommand != null) mCommand.Dispose();
					if (mConnection != null) mConnection.Dispose();
					if (mTransaction != null) mTransaction.Dispose();
					if (mDataReader != null) mDataReader.Dispose();
				}
			}
			finally
			{
				mCommand = null;
				mConnection = null;
				mTransaction = null;
				mDataReader = null;
				mParameters = null;
			}
		}

		#endregion
	}
}
