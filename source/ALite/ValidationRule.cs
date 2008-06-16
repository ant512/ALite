using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Class representing a single validation rule; used for the built-in basic validation system.
	/// </summary>
	[Serializable]
	public class ValidationRule
	{
		#region Enums

		/// <summary>
		/// List of the types of rule that the object can represent
		/// </summary>
		public enum RuleType
		{
			/// <summary>
			/// Maximum length of a string
			/// </summary>
			MaxLength,

			/// <summary>
			/// Minimum length of a string
			/// </summary>
			MinLength,

			/// <summary>
			/// Object cannot be null
			/// </summary>
			NotNull,

			/// <summary>
			/// Maximum value of an integer
			/// </summary>
			MaxValue,

			/// <summary>
			/// Minimum value of an integer
			/// </summary>
			MinValue
		}

		#endregion

		#region Members

		/// <summary>
		/// The type of validation performed by this rule
		/// </summary>
		private RuleType mType;

		/// <summary>
		/// The value that is considered "valid" for this rule
		/// </summary>
		private object mValidValue;

		/// <summary>
		/// The object that this rule will validate
		/// </summary>
		private object mInstanceToValidate;

		/// <summary>
		/// The property that this rule will validate
		/// </summary>
		private string mPropertyName;

		#endregion

		#region Properties

		/// <summary>
		/// Get the name of the property that this rule will validate
		/// </summary>
		public string PropertyName
		{
			get
			{
				return mPropertyName;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Create a new validation rule.
		/// </summary>
		/// <param name="type">The type of validation rule being created</param>
		/// <param name="validValue">A value representing a valid value for the rule</param>
		/// <param name="instanceToValidate">The object that this rule will validate</param>
		/// <param name="propertyName">The name of the property that this rule will validate</param>
		public ValidationRule(RuleType type, object validValue, object instanceToValidate, string propertyName)
		{
			this.mType = type;
			this.mValidValue = validValue;
			this.mInstanceToValidate = instanceToValidate;
			this.mPropertyName = propertyName;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Check if supplied object is a valid value
		/// </summary>
		/// <param name="value">The value to validate</param>
		/// <returns>True if valid; false if not</returns>
		public bool Validate(object value)
		{
			switch (mType)
			{
				case RuleType.MaxValue:
					return ValidateMaxValue((int)mValidValue, (int)value);
				case RuleType.MinValue:
					return ValidateMinValue((int)mValidValue, (int)value);
				case RuleType.NotNull:
					return (value != null);
				case RuleType.MaxLength:
					return ValidateMaxLength((int)mValidValue, value.ToString());
				case RuleType.MinLength:
					return ValidateMinLength((int)mValidValue, value.ToString());
			}

			return true;
		}

		/// <summary>
		/// Validate the property of the object that this instance is set up to control
		/// </summary>
		/// <returns>True if valid; false if not</returns>
		public bool Validate()
		{
			Type t = mInstanceToValidate.GetType();
			PropertyInfo[] infos = t.GetProperties();

			for (int i = 0; i < infos.Length; i++)
			{
				PropertyInfo info = infos[i];
				Type instanceType = info.GetType();
				
				if (info.Name == mPropertyName)
				{
					switch (mType)
					{
						case RuleType.MaxValue:
							return ValidateMaxValue((int)mValidValue, (int)info.GetValue(mInstanceToValidate, null));
						case RuleType.MinValue:
							return ValidateMinValue((int)mValidValue, (int)info.GetValue(mInstanceToValidate, null));
						case RuleType.NotNull:
							return (info.GetValue(mInstanceToValidate, null) != null);
						case RuleType.MaxLength:
							return ValidateMaxLength((int)mValidValue, info.GetValue(mInstanceToValidate, null).ToString());
						case RuleType.MinLength:
							return ValidateMinLength((int)mValidValue, info.GetValue(mInstanceToValidate, null).ToString());
					}
				}
			}

			return true;
		}

		#region Rule Checking

		/// <summary>
		/// Check that the supplied value is less than or equal to the maximum value
		/// </summary>
		/// <param name="validValue"></param>
		/// <param name="testValue"></param>
		/// <returns></returns>
		private static bool ValidateMaxValue(int validValue, int testValue)
		{
			return (testValue <= validValue);
		}

		/// <summary>
		/// Check that the supplied value is more than or equal to the minimum value
		/// </summary>
		/// <param name="validValue"></param>
		/// <param name="testValue"></param>
		/// <returns></returns>
		private static bool ValidateMinValue(int validValue, int testValue)
		{
			return (testValue >= validValue);
		}

		/// <summary>
		/// Check that the supplied string is shorter than the maximum length or of equal length
		/// </summary>
		/// <param name="validLength"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private static bool ValidateMaxLength(int validLength, string text)
		{
			return (validLength >= text.Length);
		}

		/// <summary>
		/// Check that the supplied string is longer than the minimum length or of equal length
		/// </summary>
		/// <param name="validLength"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private static bool ValidateMinLength(int validLength, string text)
		{
			return (validLength <= text.Length);
		}

		#endregion

		#endregion
	}
}
