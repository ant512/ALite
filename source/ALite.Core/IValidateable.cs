using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite.Core
{
	#region Enums

	/// <summary>
	/// All possible ways in which the object can validate its properties.
	/// </summary>
	public enum ValidationTimeType
	{
		/// <summary>
		/// Validation of new property values is performed when the properties are set.
		/// </summary>
		ValidatesOnPropertyChange = 0,

		/// <summary>
		/// Validation of new property values is performed when the properties are set,
		/// but an exception isn't raised for any violations until Save() is called.
		/// </summary>
		ValidatesOnSave = 1
	}

	#endregion

	/// <summary>
	/// Interface defining properties of an object that can be validated.
	/// </summary>
	public interface IValidateable
	{
		/// <summary>
		/// Gets or sets the validator object, which contains a list of rules
		/// that properties are checked against before they are set.
		/// </summary>
		ValidationTimeType ValidationTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list of all current validation errors that have
		/// arisen from setting properties.
		/// </summary>
		Dictionary<string, List<string>> ValidationErrors
		{
			get;
			set;
		}
	}
}
