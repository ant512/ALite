using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ALite
{
	/// <summary>
	/// Thrown if an error occurs when attempting to start a DBObject transaction.
	/// </summary>
	[Serializable]
	public class TransactionInitialisationException : Exception
	{
		/// <summary>
		/// Basic constructor.
		/// </summary>
		public TransactionInitialisationException() : base() { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">Error message</param>
		public TransactionInitialisationException(string message) : base(message) { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected TransactionInitialisationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public TransactionInitialisationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
