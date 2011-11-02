using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ALite.ObjectValidator
{
	/// <summary>
	/// Thrown if an error occurs when validating a DBObject.
	/// </summary>
	[Serializable]
	public class ValidationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the ValidationException class.
		/// </summary>
		public ValidationException() : base() { }

		/// <summary>
		/// Initializes a new instance of the ValidationException class.
		/// </summary>
		/// <param name="message">Error message.</param>
		public ValidationException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the ValidationException class.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Initializes a new instance of the ValidationException class.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ValidationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
