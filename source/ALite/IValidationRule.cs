using System;
using System.Collections.Generic;
using System.Text;
namespace ALite
{
	public interface IValidationRule
	{
		string PropertyName { get; }
		bool Validate(object value, ref string errorMessage);
	}
}
