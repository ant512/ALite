using System;
using System.Collections.Generic;
using System.Text;

namespace ALite
{
	#region Delegate Types

	/// <summary>
	/// Template for validation delegates
	/// </summary>
	/// <param name="errorMessage">Error message to return if the value is invalid</param>
	/// <param name="oldValue">The current value of the property</param>
	/// <param name="newValue">The new value of the property</param>
	/// <returns>True if valid, false if not</returns>
	public delegate bool Validator(ref string errorMessage, object currentValue, object newValue);

	#endregion

	#region Structs

	[Serializable]
	public struct DelegateValidationRule
	{
		#region Members

		private Validator mDelegate;
		private string mPropertyName;

		#endregion

		#region Properties

		public Validator DelegateFunction
		{
			get
			{
				return mDelegate;
			}
		}

		public string PropertyName
		{
			get
			{
				return mPropertyName;
			}
		}

		#endregion

		#region Constructors

		public DelegateValidationRule(Validator delegateFunction, string propertyName)
		{
			mDelegate = delegateFunction;
			mPropertyName = propertyName;
		}

		#endregion
	}

	#endregion
}
