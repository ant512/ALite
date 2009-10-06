using System;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	class DelegateRuleCollection
	{
		private List<DelegateValidationRule> mDelegateList;

		public DelegateRuleCollection()
		{
			mDelegateList = new List<DelegateValidationRule>();
		}

		public void Add(DelegateValidationRule rule)
		{
			mDelegateList.Add(rule);
		}

		public void Clear()
		{
			mDelegateList.Clear();
		}

		public bool Validate<T>(string propertyName, ref string errorMessage, T oldValue, T newValue)
		{
			foreach (DelegateValidationRule rule in mDelegateList)
			{
				// Have we found a relevant rule?
				if (rule.PropertyName == propertyName)
				{
					// Is the value valid?
					if (!rule.DelegateFunction(ref errorMessage, oldValue, newValue))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
