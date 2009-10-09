using System;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	/// <summary>
	/// Collection of validation rule objects.  Used by the DBObject to store all validation objects.
	/// </summary>
	class ValidationRuleCollection : DictionaryList<string, IValidationRule>
	{
		#region Methods

		/// <summary>
		/// Add a new rule to the rule list.
		/// </summary>
		/// <param name="rule">The rule to add to the list.</param>
		public void Add(IValidationRule rule)
		{
			this.Add(new KeyValuePair<string, IValidationRule>(rule.PropertyName, rule));
		}

		/// <summary>
		/// Validate the new value using all rules specified for the given property name.
		/// </summary>
		/// <typeparam name="T">Type of the property to validate.</typeparam>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="errorMessages">List of error messages returned if the value is invalid.</param>
		/// <param name="newValue">The new value for the property.</param>
		/// <returns>True if the new value is valid; false if not.</returns>
		public bool Validate<T>(string propertyName, List<string> errorMessages, T newValue)
		{
			// Locate the list of rules for the current property
			List<IValidationRule> rules = this.Values(propertyName);

			bool valid = true;

			if (rules != null)
			{
				// Validate new value against standard rules
				foreach (ValidationRule rule in rules)
				{
					// Is the value valid?
					if (!rule.Validate(newValue, errorMessages))
					{
						valid = false;
					}
				}
			}

			return valid;
		}

		#endregion
	}
}
