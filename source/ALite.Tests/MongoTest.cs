using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ALite;
using ALite.MongoDB;
using ObjectValidator;
using ObjectValidator.StandardRules;
using MongoDB;

namespace ALite.Tests
{
	[TestClass]
	public class MongoTest
	{
		class MongoDBObjectTest : MongoBackedObject
		{
			public string Description
			{
				get { return GetProperty<string>("Description"); }
				set { SetProperty<string>("Description", value); }
			}

			public MongoDBObjectTest() : base("test", "test") { }
		}

		[TestMethod]
		public void GetSetTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			Assert.AreEqual("this is some text", testObject.Description);
		}

		[TestMethod]
		public void SaveTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.Save();

			Assert.AreNotEqual(null, testObject.Id);
		}

		[TestMethod]
		public void LoadTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.Save();

			Oid id = testObject.Id;

			testObject = new MongoDBObjectTest();
			testObject.Id = id;

			Assert.AreEqual("this is some text", testObject.Description);
		}

		[TestMethod]
		public void UpdateTest()
		{
			// Create the original object
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.Save();

			Oid id = testObject.Id;

			// Fetch the object from the DB
			testObject = new MongoDBObjectTest();
			testObject.Id = id;

			// Change and update the object
			testObject.Description = "more text";
			testObject.Save();

			// Fetch the object from the DB again
			testObject = new MongoDBObjectTest();
			testObject.Id = id;

			Assert.AreEqual("more text", testObject.Description);
		}

		[TestMethod]
		public void DeleteTest()
		{
			// Create the original object
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.Save();

			Oid id = testObject.Id;

			// Fetch the object from the DB and delete it
			testObject = new MongoDBObjectTest();
			testObject.Id = id;

			testObject.Delete();

			// Attempt to fetch the object again
			testObject = new MongoDBObjectTest();
			testObject.Id = id;

			Assert.IsNull(testObject.Description);
		}

		[TestMethod]
		public void NewStateTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			Assert.AreEqual(ModificationState.New, testObject.State);

			testObject.Save();

			Assert.AreEqual(ModificationState.Unmodified, testObject.State);
		}

		[TestMethod]
		public void DeletedStateTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";
			testObject.Save();
			testObject.Delete();

			Assert.AreEqual(ModificationState.Deleted, testObject.State);
		}

		[TestMethod]
		public void ModifiedStateTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";
			testObject.Save();

			Assert.AreEqual(ModificationState.Unmodified, testObject.State);

			testObject.Description = "more text";
			Assert.AreEqual(ModificationState.Modified, testObject.State);
		}

		[TestMethod]
		public void RestorePointTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.SetRestorePoint();
			testObject.Description = "more text";

			testObject.RevertToRestorePoint();

			Assert.AreEqual("this is some text", testObject.Description);
		}

		[TestMethod]
		public void RevertToRestoreCalledTwiceTest()
		{
			MongoDBObjectTest testObject = new MongoDBObjectTest();
			testObject.Description = "this is some text";

			testObject.SetRestorePoint();
			testObject.Description = "more text";

			testObject.RevertToRestorePoint();
			testObject.RevertToRestorePoint();

			Assert.AreEqual("this is some text", testObject.Description);
		}

		[TestMethod]
		public void StringLengthTest()
		{
			StringLengthValidationRule rule = new StringLengthValidationRule(5, 10);
			List<string> errors = new List<string>();

			rule.Validate("test", errors);

			Assert.AreEqual(1, errors.Count);
		}
	}
}
