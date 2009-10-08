using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Class representing a single validation rule.  Validates the value of an integer against minimum and
	/// maximum values
	/// </summary>
	[Serializable]
	public class IntegerBoundsValidationRule : ValidationRule
	{
		#region Members

		/// <summary>
		/// The minimum valid value of the integer
		/// </summary>
		private int mMinValue;

		/// <summary>
		/// The maximum valid value of the integer
		/// </summary>
		private int mMaxValue;

		#endregion

		#region Properties

		/// <summary>
		/// The minimum valid value of the integer
		/// </summary>
		public int MinValue
		{
			get
			{
				return mMinValue;
			}
		}
		
		/// <summary>
		/// The minimum valid value of the integer
		/// </summary>
		public int MaxValue
		{
			get
			{
				return mMaxValue;
			}
		}
		
		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName">The name of the property to validate</param>
		/// <param name="minValue">The minimum value of the integer</param>
		/// <param name="maxValue">The maximum value of the integer</param>
		public IntegerBoundsValidationRule(string propertyName, int minValue, int maxValue) : base(propertyName)
		{
			mMaxValue = maxValue;
			mMinValue = minValue;
		}

		#endregion

		#region Methods
		
		/// <summary>
		/// Validate the integer
		/// </summary>
		/// <param name="value">The integer to validate</param>
		/// <param name="errorMessage">Error message output if an error occurs</param>
		/// <returns>True if the validation passed; false if not</returns>
		public override bool Validate(object value, ref string errorMessage)
		{
			if (!ValidateMaxValue((int)value))
			{
				errorMessage = String.Format("Integer is larger than maximum value of {0}.", mMaxValue);
				return false;
			}

			if (!ValidateMinValue((int)value))
			{
				errorMessage = String.Format("Integer is smaller than minimum value of {0}.", mMinValue);
				return false;
			}
			
			return true;
		}

		/// <summary>
		/// Check that the supplied integer is smaller than or equal to the maximum value
		/// </summary>
		/// <param name="value">The integer to validate</param>
		/// <returns>True if the integer is valid; false if not</returns>
		private bool ValidateMaxValue(int value)
		{
			return (mMaxValue >= value);
		}

		/// <summary>
		/// Check that the supplied integer is larger than or equal to the minimum value
		/// </summary>
		/// <param name="value">The integer to validate</param>
		/// <returns>True if the integer is valid; false if not</returns>
		private bool ValidateMinValue(int value)
		{
			return (mMinValue <= value);
		}

		#endregion
	}
}