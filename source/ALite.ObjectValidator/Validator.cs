using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite.ObjectValidator
{
	/// <summary>
	/// Central class within the library.  To use the library, create an instance of this class,
	/// add rules to it, and call its Validate() method to verify data.
	/// </summary>
	[Serializable]
	public class Validator
	{
		#region Members

		/// <summary>
		/// List of rules that properties are checked against before they are set
		/// </summary>
		private ValidationRuleCollection mRules = new ValidationRuleCollection();

		/// <summary>
		/// List of delegates that function as custom rules
		/// </summary>
		private DelegateRuleCollection mDelegateRules = new DelegateRuleCollection();

		#endregion

		#region Methods

		/// <summary>
		/// Validate the supplied value using all rules.
		/// </summary>
		/// <typeparam name="T">The type of the property to validate.</typeparam>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="errorMessages">Will contain any errors arising from the validation
		/// attempt once the function ends.</param>
		/// <param name="value">Value to validate.</param>
		/// <returns>True if the value is valid; false if not.</returns>
		public bool Validate<T>(string propertyName, List<string> errorMessages, T value)
		{
			bool valid = true;

			// Validate new value against standard rules
			if (!mRules.Validate<T>(propertyName, errorMessages, value)) valid = false;

			// Validate new value against custom rules
			if (!mDelegateRules.Validate<T>(propertyName, errorMessages, value)) valid = false;

			return valid;
		}

		/// <summary>
		/// Add an IValidationRule object to the rule list.
		/// </summary>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <param name="rule">The IValidation object to add to the list.</param>
		public void AddRule(string propertyName, IValidationRule rule)
		{
			mRules.Add(propertyName, rule);
		}

		/// <summary>
		/// Add a function delegate as a custom rule.
		/// </summary>
		/// <param name="propertyName">The function that will validate the property.</param>
		/// <param name="delegateFunction">The name of the property that the function validates.</param>
		public void AddRule(string propertyName, ValidatorDelegate delegateFunction)
		{
			mDelegateRules.Add(propertyName, delegateFunction);
		}

		#endregion
	}
}
