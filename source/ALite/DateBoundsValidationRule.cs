using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the value of an date against minimum and
	/// maximum values.
	/// </summary>
	[Serializable]
	public class DateBoundsValidationRule : ValidationRule
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

		#region Members

		/// <summary>
		/// The minimum valid value of the date.
		/// </summary>
		private DateTime mMinValue;

		/// <summary>
		/// The maximum valid value of the date.
		/// </summary>
		private DateTime mMaxValue;

		#endregion

		#region Properties

		/// <summary>
		/// The minimum valid value of the date.
		/// </summary>
		public DateTime MinValue
		{
			get
			{
				return mMinValue;
			}
		}

		/// <summary>
		/// The minimum valid value of the date.
		/// </summary>
		public DateTime MaxValue
		{
			get
			{
				return mMaxValue;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="minValue">The minimum value of the date.</param>
		/// <param name="maxValue">The maximum value of the date.</param>
		public DateBoundsValidationRule(string propertyName, DateTime minValue, DateTime maxValue)
			: base(propertyName)
		{
			mMaxValue = maxValue;
			mMinValue = minValue;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Validate the date.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <param name="errorMessage">Error message output if an error occurs.</param>
		/// <returns>True if the validation passed; false if not.</returns>
		public override bool Validate(object value, ref string errorMessage)
		{
			if (!ValidateMaxValue((DateTime)value))
			{
				errorMessage = String.Format(DateTooLargeMessage, mMaxValue);
				return false;
			}

			if (!ValidateMinValue((DateTime)value))
			{
				errorMessage = String.Format(DateTooSmallMessage, mMinValue);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check that the supplied date is smaller than or equal to the maximum value.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <returns>True if the date is valid; false if not.</returns>
		private bool ValidateMaxValue(DateTime value)
		{
			return (mMaxValue >= value);
		}

		/// <summary>
		/// Check that the supplied date is larger than or equal to the minimum value.
		/// </summary>
		/// <param name="value">The date to validate.</param>
		/// <returns>True if the date is valid; false if not.</returns>
		private bool ValidateMinValue(DateTime value)
		{
			return (mMinValue <= value);
		}

		#endregion
	}
}
