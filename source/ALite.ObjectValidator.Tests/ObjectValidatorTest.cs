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

		[TestMethod]
		public void TestStringLengthValidation()
		{
			var obj = new Validator();
			obj.AddRule("name", new StringLengthValidationRule(3, 10));

			var errors = new List<string>();

			Assert.IsFalse(obj.Validate<string>("name", errors, "Jo"), "String is too short.");
			Assert.IsFalse(obj.Validate<string>("name", errors, "This name is too long"), "String is too long.");
			Assert.IsTrue(obj.Validate<string>("name", errors, "Bob"), "String should be valid.");
		}

		[TestMethod]
		public void TestIntegerBoundsValidation()
		{
			var obj = new Validator();
			obj.AddRule("value", new IntegerBoundsValidationRule(3, 8));

			var errors = new List<string>();

			Assert.IsFalse(obj.Validate<int>("value", errors, 2), "Integer is too small.");
			Assert.IsFalse(obj.Validate<int>("value", errors, 9), "Integer is too large.");
			Assert.IsTrue(obj.Validate<int>("value", errors, 5), "Integer should be valid.");
		}

		[TestMethod]
		public void TestMissingValidator()
		{
			var obj = new Validator();

			var errors = new List<string>();

			Assert.IsTrue(obj.Validate<int>("value", errors, 2), "All values should be allowed if no rules apply.");
		}

		[TestMethod]
		public void TestDelegateValidation()
		{
			var obj = new Validator();
			obj.AddRule("name", (value, errors) =>
			{
				if ((string)value == "Bob")
				{
					errors.Add("Name cannot be 'Bob'");
					return false;
				}
				return true;
			});

			var errorMessages = new List<string>();

			Assert.IsFalse(obj.Validate<string>("name", errorMessages, "Bob"), "Name cannot be 'Bob'");
			Assert.IsTrue(obj.Validate<string>("name", errorMessages, "Joe"), "Name can be Joe.");
		}
	}
}
