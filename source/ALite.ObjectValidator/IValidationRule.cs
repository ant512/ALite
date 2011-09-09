using System;
using System.Collections.Generic;
using System.Text;

namespace ALite.ObjectValidator
{
	/// <summary>
	/// Interface defining the behaviour of a basic validation rule.
	/// </summary>
	public interface IValidationRule
	{
		#region Methods

		/// <summary>
		/// Validates the new value being applied to the property.
		/// </summary>
		/// <param name="value">The new value to validate.</param>
		/// <param name="errorMessages">A list of error messages to be populated by the function.</param>
		/// <returns>True if the value is valid; false if not.</returns>
		bool Validate(object value, List<string> errorMessages);

		#endregion
	}
}
