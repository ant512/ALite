using System;
using System.Collections.Generic;
using System.Text;
using ALite;
using System.Transactions;
using System.Threading;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			ObjectTest obj = new ObjectTest();
			obj.ID = 15;

			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					obj.ID = 14;
					obj.Name = "Joe";
					obj.ID = 12;

					scope.Complete();
				}
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			System.Console.WriteLine(obj.ID);

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

			// Must be called so that object is ready to be reset to this state
			ResetUndo();
		}

		private string mName;
		private int mId;
		private DateTime mDate;

		public string Name
		{
			get { return GetProperty<string>("Name", ref mName); }
			set { SetProperty<string>("Name", ref mName, value); }
		}

		public int ID
		{
			get { return GetProperty<int>("ID", ref mId); }
			set { SetProperty<int>("ID", ref mId, value); }
		}

		public DateTime Date
		{
			get { return GetProperty<DateTime>("Date", ref mDate); }
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
