using System;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	/// <summary>
	/// Collection of validation delegates.  Used by the DBObject to store all custom validation functions.
	/// </summary>
	class DelegateRuleCollection : DictionaryList<string, DelegateValidationRule>
	{
		/// <summary>
		/// Validate the new value using all validators specified for the given property name.
		/// </summary>
		/// <typeparam name="T">Type of the property to validate.</typeparam>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="errorMessage">Error message returned if the value is invalid.</param>
		/// <param name="oldValue">The current value of the property.</param>
		/// <param name="newValue">The new value of the property.</param>
		/// <returns>True if the new value is valid; false if not.</returns>
		public bool Validate<T>(string propertyName, ref string errorMessage, T oldValue, T newValue)
		{
			// Locate the list of rules for the given property
			List<DelegateValidationRule> rules = this.Values(propertyName);

			if (rules != null)
			{
				// Check the new value against each of the rules
				foreach (DelegateValidationRule rule in rules)
				{
					// Is the value valid?
					if (!rule.DelegateFunction(ref errorMessage, oldValue, newValue))
					{
						return false;
					}
				}
			}

			// New value is valid
			return true;
		}

		/// <summary>
		/// Add a new validation delegate to the list.
		/// </summary>
		/// <param name="rule">Delegate to add to the list.</param>
		public void Add(DelegateValidationRule rule)
		{
			this.Add(new KeyValuePair<string, DelegateValidationRule>(rule.PropertyName, rule));
		}
	}
}
