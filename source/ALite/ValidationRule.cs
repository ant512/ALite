using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ALite
{
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
		
		public ValidationRule(string propertyName)
		{
			mPropertyName = propertyName;
		}

		#endregion

		#region Methods

		public abstract bool Validate(object value, ref string errorMessage);

		#endregion
	}
}
