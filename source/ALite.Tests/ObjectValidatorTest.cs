using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALite.ObjectValidator;
using ALite.ObjectValidator.StandardRules;

namespace ALite.Tests
{
	[TestClass]
	public class ObjectValidatorTest
	{
		[TestMethod]
		public void TestNullStringValidation()
		{
			var obj = new Validator();
			obj.AddRule("name", new StringLengthValidationRule(0, 10));

			List<string> errors = new List<string>();

			try
			{
				obj.Validate<string>("name", errors, null);
			}
			catch (NullReferenceException)
			{
				Assert.Fail("String validation fails when string is null.");
			}
		}
	}
}
