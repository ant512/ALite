using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ALite
{
	/// <summary>
	/// Thrown if an error occurs when interacting with the database
	/// </summary>
	[Serializable]
	public class DataAccessException : Exception
	{
		/// <summary>
		/// Basic constructor
		/// </summary>
		public DataAccessException() : base() { }
 
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
   		public DataAccessException(string message) : base(message) { }
 
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
   		protected DataAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
	}
}
