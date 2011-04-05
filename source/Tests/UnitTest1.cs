using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALite;
using ObjectValidator;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		private class TestObject : DBObject
		{
			public int Id
			{
				get
				{
					return GetProperty<int>("Id");
				}
				set
				{
					SetProperty("Id", value);
				}
			}

			public string Name
			{
				get
				{
					return GetProperty<string>("Name");
				}
				set
				{
					SetProperty("Name", value);
				}
			}

			public TestObject(int id, string name)
			{
				Id = id;
				Name = name;

				AddRule("Id", new ObjectValidator.StandardRules.IntegerBoundsValidationRule(0, 10));
				AddRule("Name", new ObjectValidator.StandardRules.StringLengthValidationRule(3, 8));
				AddRule("Name", delegate(List<string> errorMessages, object value)
				{
					if ((string)value == "Bert")
					{
						errorMessages.Add("Name cannot be \"Bert\"");
						return false;
					}
					return true;
				});
			}
		}

		[TestMethod]
		public void TestNewStatus()
		{
			var obj = new TestObject(2, "Bob");

			Assert.AreEqual(true, obj.IsDirty);
			Assert.AreEqual(true, obj.IsNew);
			Assert.AreEqual(false, obj.IsDeleted);
		}

		[TestMethod]
		public void TestOldStatus()
		{
			var obj = new TestObject(2, "Bob");
			obj.Save();

			Assert.AreEqual(false, obj.IsDirty);
			Assert.AreEqual(false, obj.IsNew);
			Assert.AreEqual(false, obj.IsDeleted);
		}

		[TestMethod]
		public void TestDeletedStatus()
		{
			var obj = new TestObject(2, "Bob");
			obj.Delete();

			Assert.AreEqual(false, obj.IsDirty);
			Assert.AreEqual(false, obj.IsNew);
			Assert.AreEqual(true, obj.IsDeleted);
		}

		[TestMethod]
		public void TestDirtyStatus()
		{
			var obj = new TestObject(2, "Bob");
			obj.Save();

			obj.Id = 5;

			Assert.AreEqual(true, obj.IsDirty);
			Assert.AreEqual(false, obj.IsNew);
			Assert.AreEqual(false, obj.IsDeleted);
		}

		[TestMethod]
		public void TestProperties()
		{
			var obj = new TestObject(2, "Bob");
			obj.Id = 5;
			obj.Name = "Joe";

			Assert.AreEqual(5, obj.Id);
			Assert.AreEqual("Joe", obj.Name);
		}

		[TestMethod]
		public void TestRestorePoint()
		{
			var obj = new TestObject(2, "Bob");

			obj.SetRestorePoint();

			obj.Id = 5;
			obj.Name = "Joe";

			obj.RevertToRestorePoint();

			Assert.AreEqual(2, obj.Id);
			Assert.AreEqual("Bob", obj.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(ValidationException))]
		public void TestIdStandardRule()
		{
			var obj = new TestObject(2, "Bob");

			obj.Id = 34;
		}

		[TestMethod]
		[ExpectedException(typeof(ValidationException))]
		public void TestNameStandardRule()
		{
			var obj = new TestObject(2, "Bob");

			obj.Name = "A";
		}

		[TestMethod]
		[ExpectedException(typeof(ValidationException))]
		public void TestNameDelegateRule()
		{
			var obj = new TestObject(2, "Bob");

			obj.Name = "Bert";
		}
	}
}
