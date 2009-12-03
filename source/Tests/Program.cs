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
		static ObjectTest mObj;
		static int mNewId;

		static void ThreadTest()
		{
			lock (mObj)
			{
				try
				{
					mObj.BeginTransaction();

					mObj.ID = mNewId++;
					mObj.Name = "Joe";
					mObj.ID = mNewId - 10;
				}
				catch (ValidationException)
				{
					//System.Console.WriteLine(ex.Message);
				}

				if (mObj.HasTransactionFailed)
				{
					foreach (string err in mObj.TransactionErrors)
					{
						System.Console.WriteLine(err);
					}

					mObj.Rollback();
				}
				else
				{
					mObj.Commit();
				}

				mObj.EndTransaction();

				System.Console.WriteLine(mObj.ID);
			}
		}

		static void Main(string[] args)
		{
			mObj = new ObjectTest();
			mObj.ID = 20;
			mNewId = 15;

			//ThreadTest();

			int threads = 2;

			Thread[] t = new Thread[threads];
			for (int i = 0; i < t.Length; ++i)
			{
				t[i] = new Thread(ThreadTest);
			}

			for (int i = 0; i < t.Length; ++i)
			{
				t[i].Start();
			}


			/*
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

			myObject.Undo();

			System.Console.WriteLine(myObject.Name);
			System.Console.WriteLine(myObject.ID.ToString());
			 * */

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

			Commit();
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
