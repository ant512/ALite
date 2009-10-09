using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the length of a string against minimum and
	/// maximum values.
	/// </summary>
	[Serializable]
	public class StringLengthValidationRule : ValidationRule
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

		#region Members

		/// <summary>
		/// Maximum length of a valid string.
		/// </summary>
		private int mMaxLength;

		/// <summary>
		/// Minimum length of a valid string.
		/// </summary>
		private int mMinLength;

		#endregion

		#region Properties

		/// <summary>
		/// The maximum length of a valid string.
		/// </summary>
		public int MaxLength
		{
			get
			{
				return mMaxLength;
			}
		}
		
		/// <summary>
		/// The minimum length of a valid string.
		/// </summary>
		public int MinLength
		{
			get
			{
				return mMinLength;
			}
		}
		
		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="minLength">The mimimum valid length of the string.</param>
		/// <param name="maxLength">The maximum valid length of the string.</param>
		public StringLengthValidationRule(string propertyName, int minLength, int maxLength) : base(propertyName)
		{
			mMaxLength = maxLength;
			mMinLength = minLength;
		}

		#endregion

		#region Methods
		
		/// <summary>
		/// Validate the string.
		/// </summary>
		/// <param name="value">The string to validate.</param>
		/// <param name="errorMessage">Error message output if an error occurs.</param>
		/// <returns>True if the validation passed; false if not.</returns>
		public override bool Validate(object value, ref string errorMessage)
		{
			if (!ValidateMaxLength((string)value))
			{
				errorMessage = String.Format(StringTooLongMessage, mMaxLength);
				return false;
			}

			if (!ValidateMinLength((string)value))
			{
				errorMessage = String.Format(StringTooShortMessage, mMinLength);
				return false;
			}
			
			return true;
		}

		/// <summary>
		/// Check that the supplied string is shorter than the maximum length or of equal length.
		/// </summary>
		/// <param name="text">The string to validate.</param>
		/// <returns>True if the string is valid; false if not.</returns>
		private bool ValidateMaxLength(string text)
		{
			return (mMaxLength >= text.Length);
		}

		/// <summary>
		/// Check that the supplied string is longer than the minimum length or of equal length.
		/// </summary>
		/// <param name="text">The string to validate.</param>
		/// <returns>True if the string is valid; false if not.</returns>
		private bool ValidateMinLength(string text)
		{
			return (mMinLength <= text.Length);
		}

		#endregion
	}
}
