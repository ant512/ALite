using System;
using System.Collections.Generic;
using System.Text;

namespace ALite.ObjectValidator.StandardRules
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the length of a string against minimum and
	/// maximum values.
	/// </summary>
	[Serializable]
	public class StringLengthValidationRule : IValidationRule
	{
		#region Constants

		/// <summary>
		/// Message returned when the string being validated is too long.
		/// </summary>
		private const string StringTooLongMessage = "String is longer than maximum length of {0} chars.";

		/// <summary>
		/// Message returned when the string being validated is too short.
		/// </summary>
		private const string StringTooShortMessage = "String is shorter than minimum length of {0} chars.";

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the StringLengthValidationRule class.
		/// </summary>
		/// <param name="minLength">The mimimum valid length of the string.</param>
		/// <param name="maxLength">The maximum valid length of the string.</param>
		public StringLengthValidationRule(int minLength, int maxLength)
		{
			MaxLength = maxLength;
			MinLength = minLength;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the maximum length of a valid string.
		/// </summary>
		public int MaxLength { get; private set; }

		/// <summary>
		/// Gets the minimum length of a valid string.
		/// </summary>
		public int MinLength { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Validate the string.
		/// </summary>
		/// <param name="value">The string to validate.</param>
		/// <param name="errorMessages">List of error messages output if an error occurs.</param>
		/// <returns>True if the validation passed; false if not.</returns>
		public bool Validate(object value, List<string> errorMessages)
		{
			bool valid = true;

			if (!ValidateMaxLength((string)value))
			{
				errorMessages.Add(string.Format(StringTooLongMessage, MaxLength));
				valid = false;
			}

			if (!ValidateMinLength((string)value))
			{
				errorMessages.Add(string.Format(StringTooShortMessage, MinLength));
				valid = false;
			}

			return valid;
		}

		/// <summary>
		/// Check that the supplied string is shorter than the maximum length or of equal length.
		/// </summary>
		/// <param name="text">The string to validate.</param>
		/// <returns>True if the string is valid; false if not.</returns>
		private bool ValidateMaxLength(string text)
		{
			if (text == null) return true;
			return MaxLength >= text.Length;
		}

		/// <summary>
		/// Check that the supplied string is longer than the minimum length or of equal length.
		/// </summary>
		/// <param name="text">The string to validate.</param>
		/// <returns>True if the string is valid; false if not.</returns>
		private bool ValidateMinLength(string text)
		{
			if (MinLength > 0 && text == null) return false;
			if (text == null) return true;
			return MinLength <= text.Length;
		}

		#endregion
	}
}
