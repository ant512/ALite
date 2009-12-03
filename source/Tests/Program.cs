using System;
using System.Collections.Generic;
using System.Text;
using ALite;
using System.Threading;
using System.Diagnostics;

namespace Tests
{
	class Program
	{
		static ObjectTest mObj = null;
		static int mNewId = 0;

		static void ThreadTestThread()
		{
			lock (mObj)
			{
				try
				{
					mObj.BeginTransaction();

					mObj.ID = mNewId++;
					mObj.Name = "Joe";
					mObj.ID = mNewId - 10;

					mObj.EndTransaction();

					mObj.Save();
				}
				catch (ValidationException)
				{
					//System.Console.WriteLine(ex.Message);

					if (mObj.HasTransactionFailed)
					{
						foreach (string err in mObj.TransactionErrors)
						{
							System.Console.WriteLine(err);
						}
					}
				}

				System.Console.WriteLine(mObj.ID);
			}
		}

		static void ThreadTest()
		{
			System.Console.WriteLine("\nThreadTest");

			// Thread test
			mObj = new ObjectTest();
			mObj.ID = 20;
			mNewId = 15;

			int threads = 4;

			Thread[] t = new Thread[threads];
			for (int i = 0; i < t.Length; ++i)
			{
				t[i] = new Thread(ThreadTestThread);
			}

			for (int i = 0; i < t.Length; ++i)
			{
				t[i].Start();
			}

			for (int i = 0; i < t.Length; ++i)
			{
				t[i].Join();
			}
		}

		static void CollectionTest()
		{
			System.Console.WriteLine("\nCollectionTest");

			// Collection test
			DBObjectCollection<ObjectTest> collection = new DBObjectCollection<ObjectTest>();
			ObjectTest obj = new ObjectTest();
			obj.ID = 15;
			obj.Name = "Bert";

			collection.Add(obj);

			obj = new ObjectTest();
			obj.ID = 19;
			obj.Name = "Fred";

			collection.Add(obj);

			try
			{

				System.Console.WriteLine("Before transaction");
				foreach (ObjectTest item in collection)
				{
					System.Console.WriteLine(String.Format("Name: {0}, ID: {1}", item.Name, item.ID));
				}

				collection.BeginTransaction();
				collection.RemoveAt(0);
				collection[0].ID = 12;

				System.Console.WriteLine("\nDuring transaction");
				foreach (ObjectTest item in collection)
				{
					System.Console.WriteLine(String.Format("Name: {0}, ID: {1}", item.Name, item.ID));
				}

				collection.Rollback();
				collection.EndTransaction();

				System.Console.WriteLine("\nPost transaction");
				foreach (ObjectTest item in collection)
				{
					System.Console.WriteLine(String.Format("Name: {0}, ID: {1}", item.Name, item.ID));
				}
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);

				System.Console.WriteLine("\nPost exception");
				foreach (ObjectTest item in collection)
				{
					System.Console.WriteLine(String.Format("Name: {0}, ID: {1}", item.Name, item.ID));
				}
			}
		}

		static void DeleteTest()
		{
			System.Console.WriteLine("\nDeleteTest");

			ITest test;
			test = new ChildDeleteTest.ChildTest();
			System.Console.Write(String.Format("{0}: ", test.Name));
			System.Console.WriteLine(test.Test() ? "Passed" : "Failed");

			ObjectTest myObject = new ObjectTest();

			try
			{
				myObject.Name = "joe";
				myObject.Name = "o";
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			try
			{
				myObject.ID = 12;
				myObject.ID = 22;
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			try
			{
				myObject.Date = new DateTime(2009, 2, 2);
				myObject.Date = new DateTime(2002, 2, 2);
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			System.Console.WriteLine(myObject.Name);
			System.Console.WriteLine(myObject.ID.ToString());
		}


		static void Main(string[] args)
		{
			ThreadTest();
			CollectionTest();
			DeleteTest();
			



			System.Console.ReadLine();
		}
	}

	class ObjectTest : DBObject
	{
		public ObjectTest()
		{
			AddRule(new StringLengthValidationRule("Name", 2, 10));
			AddRule(new IntegerBoundsValidationRule("ID", 13, 60));
			AddRule(new DateBoundsValidationRule("Date", new DateTime(2009, 1, 1), new DateTime(2009, 11, 30)));
			AddRule(new DateBoundsValidationRule("Date", new DateTime(2009, 4, 4), new DateTime(2009, 10, 30)));
			AddRule(ValidateID, "ID");

			// Anonymous method for validating ID
			AddRule(delegate(List<string> errorMessages, object value)
			{
				if ((int)value == 12)
				{
					errorMessages.Add("ID cannot be 12");
					return false;
				}
				return true;
			}, "ID");

			Name = "bob";
			ID = 19;
			Date = new DateTime(2009, 5, 5);
		}

		private string mName;
		private int mId;
		private DateTime mDate;

		public string Name
		{
			get { return mName; }
			set { SetProperty<string>("Name", ref mName, value); }
		}

		public int ID
		{
			get { return mId; }
			set { SetProperty<int>("ID", ref mId, value); }
		}

		public DateTime Date
		{
			get { return mDate; }
			set { SetProperty<DateTime>("Date", ref mDate, value); }
		}

		public bool ValidateID(List<string> errorMessages, object value)
		{
			if ((int)value < 14)
			{
				errorMessages.Add("ID cannot be less than 14.");
				return false;
			}

			return true;
		}
	}
}
