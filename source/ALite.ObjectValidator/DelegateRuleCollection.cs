﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ALite.ObjectValidator
{
	#region Delegate Types

	/// <summary>
	/// Template for validation delegates.
	/// </summary>
	/// <param name="errorMessages">List of error message populated if the value is invalid.</param>
	/// <param name="value">The new value of the property.</param>
	/// <returns>True if valid, false if not.</returns>
	public delegate bool ValidatorDelegate(List<string> errorMessages, object value);

	#endregion

	/// <summary>
	/// Collection of validation delegates.  Used by the DBObject to store all custom validation functions.
	/// </summary>
	class DelegateRuleCollection : Dictionary<string, ValidatorDelegate>
	{
		#region Methods

		/// <summary>
		/// Validate the new value using all validators specified for the given property name.
		/// </summary>
		/// <typeparam name="T">Type of the property to validate.</typeparam>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="errorMessages">List of error messages populated if the value is invalid.</param>
		/// <param name="newValue">The new value of the property.</param>
		/// <returns>True if the new value is valid; false if not.</returns>
		public bool Validate<T>(string propertyName, List<string> errorMessages, T newValue)
		{
			bool valid = true;

			// Are there any rules for this property?
			if (this.ContainsKey(propertyName))
			{
				// Locate all rules
				ValidatorDelegate rule = this[propertyName];

				// Extract all method invocations from delegate - must do this so
				// all delegates can run and any errors can be remembered.
				foreach (ValidatorDelegate subrule in rule.GetInvocationList())
				{
					if (!subrule(errorMessages, newValue))
					{
						valid = false;
					}
				}
			}

			// New value is valid
			return valid;
		}

		/// <summary>
		/// Add a rule.
		/// </summary>
		/// <param name="propertyName">The property to validate.</param>
		/// <param name="rule">The delegate to perform the validation.</param>
		public void Add(ValidatorDelegate rule, string propertyName)
		{
			// Do we already have a delegate for this rule?
			if (this.ContainsKey(propertyName))
			{
				// Use multicasting to add new function to existing delegate
				this[propertyName] += rule;
			}
			else
			{
				// Add a new rule to the dictionary
				base.Add(propertyName, rule);
			}
		}

		#endregion
	}
}
