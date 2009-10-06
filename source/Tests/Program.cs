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
				myObject.Name = "o";
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			try
			{
				myObject.ID = 22;
			}
			catch (ValidationException ex)
			{
				System.Console.WriteLine(ex.Message);
			}

			System.Console.ReadLine();
		}
	}

	class ObjectTest : DBObject
	{
		public ObjectTest()
		{
			AddRule(new StringLengthValidationRule("Name", 2, 10));
			AddRule(new IntegerBoundsValidationRule("ID", 10, 60));
			AddRule(ValidateID, "ID");

			mName = "bob";
			mId = 40;
		}

		private string mName;
		private int mId;

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
