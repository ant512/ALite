using System;
using System.Collections.Generic;
using System.Text;

namespace ALite.ObjectValidator.StandardRules
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the value of an integer against minimum and
	/// maximum values.
	/// </summary>
	[Serializable]
	public class IntegerBoundsValidationRule : IValidationRule
	{
		#region Constants

		/// <summary>
		/// Message returned when the integer being validated is too large.
		/// </summary>
		private const string IntegerTooLargeMessage = "Integer is larger than maximum value of {0}.";

		/// <summary>
		/// Message returned when the integer being validated is too small.
		/// </summary>
		private const string IntegerTooSmallMessage = "Integer is smaller than minimum value of {0}.";

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the IntegerBoundsValidationRule class.
		/// </summary>
		/// <param name="minValue">The minimum value of the integer.</param>
		/// <param name="maxValue">The maximum value of the integer.</param>
		public IntegerBoundsValidationRule(int minValue, int maxValue)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the minimum valid value of the integer.
		/// </summary>
		public int MinValue { get; private set; }

		/// <summary>
		/// Gets the minimum valid value of the integer.
		/// </summary>
		public int MaxValue { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Validate the integer.
		/// </summary>
		/// <param name="value">The integer to validate.</param>
		/// <param name="errorMessages">List of error messages output if an error occurs.</param>
		/// <returns>True if the validation passed; false if not.</returns>
		public bool Validate(object value, List<string> errorMessages)
		{
			bool valid = true;

			if (!ValidateMaxValue((int)value))
			{
				errorMessages.Add(string.Format(IntegerTooLargeMessage, MaxValue));
				valid = false;
			}

			if (!ValidateMinValue((int)value))
			{
				errorMessages.Add(string.Format(IntegerTooSmallMessage, MinValue));
				valid = false;
			}

			return valid;
		}

		/// <summary>
		/// Check that the supplied integer is smaller than or equal to the maximum value.
		/// </summary>
		/// <param name="value">The integer to validate.</param>
		/// <returns>True if the integer is valid; false if not.</returns>
		private bool ValidateMaxValue(int value)
		{
			return MaxValue >= value;
		}

		/// <summary>
		/// Check that the supplied integer is larger than or equal to the minimum value.
		/// </summary>
		/// <param name="value">The integer to validate.</param>
		/// <returns>True if the integer is valid; false if not.</returns>
		private bool ValidateMinValue(int value)
		{
			return MinValue <= value;
		}

		#endregion
	}
}
