using System;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	class ValidationRuleCollection
	{
		private Dictionary<string, List<IValidationRule>> mRules;

		public ValidationRuleCollection()
		{
			mRules = new Dictionary<string, List<IValidationRule>>();
		}

		public void Add(IValidationRule rule)
		{
			// Locate an existing list of rules for this property
			List<IValidationRule> ruleList;

			if (mRules.ContainsKey(rule.PropertyName))
			{
				ruleList = mRules[rule.PropertyName];
			}
			else
			{
				ruleList = new List<IValidationRule>();
				mRules.Add(rule.PropertyName, ruleList);
			}

			// Add the new rule to the list
			ruleList.Add(rule);
		}

		public void Clear()
		{
			mRules.Clear();
		}

		public bool Validate<T>(string propertyName, ref string errorMessage, T newValue)
		{
			if (mRules.ContainsKey(propertyName))
			{
				List<IValidationRule> ruleList = mRules[propertyName];

				// Validate new value against standard rules
				foreach (ValidationRule rule in ruleList)
				{
					// Have we found a relevant rule?
					if (rule.PropertyName == propertyName)
					{
						// Is the object valid?
						if (!rule.Validate(newValue, ref errorMessage))
						{
							return false;
						}
					}
				}
			}

			return true;
		}
	}
}
