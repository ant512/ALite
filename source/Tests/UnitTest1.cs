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

		private class ExceptionThrowingObject : DBObject
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

			public ExceptionThrowingObject(int id, string name)
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

			protected override void CreateData()
			{
				throw new NotImplementedException("Creating data");
			}

			protected override void UpdateData()
			{
				throw new NotImplementedException("Updating data");
			}

			protected override void DeleteData()
			{
				throw new NotImplementedException("Deleting data");
			}

			protected override void FetchData()
			{
				Name = "Fetched";
				Id = 4;
				MarkOld();
			}
		}

		private class TestObjectCollection : DBObjectCollection<TestObject>
		{

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
		public void TestIdStandardRule()
		{
			var obj = new TestObject(2, "Bob");

			try
			{
				obj.Id = 34;
				Assert.Fail("Exception should be thrown by the rule system.");
			}
			catch (ValidationException)
			{
			}
		}

		[TestMethod]
		public void TestNameStandardRule()
		{
			var obj = new TestObject(2, "Bob");

			try
			{
				obj.Name = "A";
				Assert.Fail("Exception should be thrown by the rule system.");
			}
			catch (ValidationException)
			{
			}
		}

		[TestMethod]
		public void TestNameDelegateRule()
		{
			var obj = new TestObject(2, "Bob");

			try
			{
				obj.Name = "Bert";
				Assert.Fail("Exception should be thrown by the rule system.");
			}
			catch (ValidationException)
			{
			}
		}

		[TestMethod]
		public void TestCreateExceptionThrowCreate()
		{
			var obj = new ExceptionThrowingObject(2, "Bob");

			try
			{
				obj.Save();
				Assert.Fail("Exception should be thrown by the rule system.");
			}
			catch (NotImplementedException)
			{
			}
		}

		[TestMethod]
		public void TestUpdateExceptionThrowCreate()
		{
			var obj = new ExceptionThrowingObject(2, "Bob");
			obj.Fetch();
			obj.Id = 2;

			try
			{
				obj.Save();
				Assert.Fail("Exception should be thrown by the rule system.");
			}
			catch (NotImplementedException)
			{
			}
		}

		[TestMethod]
		public void TestFetch()
		{
			var obj = new ExceptionThrowingObject(2, "Bob");
			obj.Fetch();

			Assert.AreEqual(4, obj.Id);
		}

		[TestMethod]
		public void TestCollectionSave()
		{
			var list = new TestObjectCollection();

			for (int i = 0; i < 5; ++i)
			{
				list.Add(new TestObject(i, String.Format("Object {0}", i)));
			}

			foreach (var obj in list)
			{
				Assert.AreEqual(true, obj.IsNew);
			}

			list.Save();

			foreach (var obj in list)
			{
				Assert.AreEqual(false, obj.IsNew);
			}
		}

		[TestMethod]
		public void TestCollectionDelete()
		{
			var list = new TestObjectCollection();

			for (int i = 0; i < 5; ++i)
			{
				list.Add(new TestObject(i, String.Format("Object {0}", i)));
			}

			list.Delete();

			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void TestCollectionDeleteOne()
		{
			var list = new TestObjectCollection();

			for (int i = 0; i < 5; ++i)
			{
				list.Add(new TestObject(i, String.Format("Object {0}", i)));
			}

			list[1].Delete();

			Assert.AreEqual(4, list.Count);
		}
	}
}
