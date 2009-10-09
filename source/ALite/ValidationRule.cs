using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
	/// <summary>
	/// Base validation rule class.  Provides basic functionality for subclasses
	/// wanting to implement IValidationRule.
	/// </summary>
	[Serializable]
	public abstract class ValidationRule : IValidationRule
	{
		#region Members

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
		/// Constructor.
		/// </summary>
		/// <param name="propertyName">Name of the property to validate.</param>
		public ValidationRule(string propertyName)
		{
			mPropertyName = propertyName;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Validates the supplied value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="errorMessage">List of error messages populated if the validation fails.</param>
		/// <returns>True if the value is valid; false if not.</returns>
		public abstract bool Validate(object value, List<string> errorMessages);

		#endregion
	}
}
