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

	/// <summary>
	/// Stores validation delegates against their property names.
	/// </summary>
	[Serializable]
	public struct DelegateValidationRule
	{
		#region Members

		/// <summary>
		/// Validation delegate that performs validation on the associated property name.
		/// </summary>
		private Validator mDelegate;

		/// <summary>
		/// Name of the property to validate.
		/// </summary>
		private string mPropertyName;

		#endregion

		#region Properties

		/// <summary>
		/// Get the delegate function.
		/// </summary>
		public Validator DelegateFunction
		{
			get
			{
				return mDelegate;
			}
		}

		/// <summary>
		/// Get the name of the property to validate.
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
		/// Constructor.
		/// </summary>
		/// <param name="delegateFunction">The function that will validate the specified property.</param>
		/// <param name="propertyName">The name of the property to validate.</param>
		public DelegateValidationRule(Validator delegateFunction, string propertyName)
		{
			mDelegate = delegateFunction;
			mPropertyName = propertyName;
		}

		#endregion
	}

	#endregion
}
