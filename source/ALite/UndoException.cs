using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ALite
{
	/// <summary>
	/// Thrown if an error occurs when the undo system is running
	/// </summary>
	[Serializable]
	public class UndoException : Exception
	{
		/// <summary>
		/// Basic constructor
		/// </summary>
		public UndoException() : base() { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public UndoException(string message) : base(message) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected UndoException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public UndoException(string message, Exception innerException) : base(message, innerException) { }
	}
}
