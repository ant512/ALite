using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ALite
{
	/// <summary>
	/// Thrown if an error occurs when validating a DBObject
	/// </summary>
	[Serializable]
	public class ValidationException : Exception
	{
		/// <summary>
		/// Basic constructor
		/// </summary>
		public ValidationException() : base() { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ValidationException(string message) : base(message) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ValidationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
