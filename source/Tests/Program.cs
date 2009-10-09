using System;
using System.Collections.Generic;
using System.Text;
using ALite;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
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
				myObject.ID = 13;
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
			AddRule(new IntegerBoundsValidationRule("ID", 10, 60));
			AddRule(new DateBoundsValidationRule("Date", new DateTime(2009, 1, 1), new DateTime(2009, 11, 30)));
			AddRule(ValidateID, "ID");

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

		public bool ValidateID(ref string errorMessage, object currentValue, object newValue)
		{
			if ((int)newValue > 20)
			{
				errorMessage = "ID cannot be greater than 20.";
				return false;
			}

			return true;
		}
	}
}
