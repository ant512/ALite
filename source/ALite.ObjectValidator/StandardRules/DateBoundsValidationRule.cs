using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite.ObjectValidator.StandardRules
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the value of an date against minimum and
	/// maximum values.
	/// </summary>
	[Serializable]
	public class DateBoundsValidationRule : IValidationRule
	{
		#region Constants

		/// <summary>
		/// Message returned when the date being validated is too large.
		/// </summary>
		private const string DateTooLargeMessage = "Date is larger than maximum value of {0}.";

		/// <summary>
		/// Message returned when the date being validated is too small.
		/// </summary>
		private const string DateTooSmallMessage = "Date is smaller than minimum value of {0}.";

		#endregion

		#region Properties

		/// <summary>
		/// The minimum valid value of the date.
		/// </summary>
		public DateTime MinValue
		{
			get;
			private set;
		}

		/// <summary>
		/// The minimum valid value of the date.
		/// </summary>
		public DateTime MaxValue
		{
			get;
			private set;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minValue">The minimum value of the date.</param>
		/// <param name="maxValue">The maximum value of the date.</param>
		public DateBoundsValidationRule(DateTime minValue, DateTime maxValue)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Validate the date.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <param name="errorMessages">List of error message output if an error occurs.</param>
		/// <returns>True if the validation passed; false if not.</returns>
		public bool Validate(object value, List<string> errorMessages)
		{
			bool valid = true;

			if (!ValidateMaxValue((DateTime)value))
			{
				errorMessages.Add(String.Format(DateTooLargeMessage, MaxValue));
				valid = false;
			}

			if (!ValidateMinValue((DateTime)value))
			{
				errorMessages.Add(String.Format(DateTooSmallMessage, MinValue));
				valid = false;
			}

			return valid;
		}

		/// <summary>
		/// Check that the supplied date is smaller than or equal to the maximum value.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <returns>True if the date is valid; false if not.</returns>
		private bool ValidateMaxValue(DateTime value)
		{
			return (MaxValue >= value);
		}

		/// <summary>
		/// Check that the supplied date is larger than or equal to the minimum value.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <returns>True if the date is valid; false if not.</returns>
		private bool ValidateMinValue(DateTime value)
		{
			return (MinValue <= value);
		}

		#endregion
	}
}
