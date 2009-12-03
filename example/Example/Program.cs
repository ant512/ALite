using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
	class Program
	{
		public static TestObject test;

		static void Main(string[] args)
		{
			Console.WriteLine("ALite Test");
			Console.WriteLine("Creating object...");

			// Create a test object
			test = new TestObject();
			test.Id = Guid.NewGuid();
			test.Name = "New Test Object";

			UndoTest();
			RuleTest();
			StatusTest();

			Console.WriteLine("All done!");
			WaitForUser();
		}

		static void StatusTest()
		{
			Console.WriteLine("".PadLeft(80, '-'));
			Console.WriteLine("Rule Test\n---------");

			Console.WriteLine("\nCurrent object status...");

			Console.WriteLine("Dirty: " + test.IsDirty.ToString());
			Console.WriteLine("Deleted: " + test.IsDeleted.ToString());
			Console.WriteLine("New: " + test.IsNew.ToString());

			Console.WriteLine("\nMaking object old...");
			test.MarkOld();

			Console.WriteLine("Dirty: " + test.IsDirty.ToString());
			Console.WriteLine("Deleted: " + test.IsDeleted.ToString());
			Console.WriteLine("New: " + test.IsNew.ToString());

			Console.WriteLine("\nMaking object deleted...");
			test.MarkDeleted();

			Console.WriteLine("Dirty: " + test.IsDirty.ToString());
			Console.WriteLine("Deleted: " + test.IsDeleted.ToString());
			Console.WriteLine("New: " + test.IsNew.ToString());

			Console.WriteLine("\nMaking object new...");
			test.MarkNew();

			Console.WriteLine("Dirty: " + test.IsDirty.ToString());
			Console.WriteLine("Deleted: " + test.IsDeleted.ToString());
			Console.WriteLine("New: " + test.IsNew.ToString());

			WaitForUser();
		}

		static void RuleTest()
		{
			Console.WriteLine("".PadLeft(80, '-'));
			Console.WriteLine("Rule Test\n---------");

			Console.WriteLine("\nAttempting to set a property to an invalid value...");

			// Attempt to set name to an illegal value
			try
			{
				test.Name = "This is an illegal value as name must be less than 20 chars";
			}
			catch (ALite.ValidationException e)
			{
				Console.WriteLine(e.Message.ToString());
			}

			WaitForUser();
		}

		static void UndoTest()
		{
			Console.WriteLine("".PadLeft(80, '-'));
			Console.WriteLine("Undo Test\n---------");

			Console.WriteLine("\nCurrent object properties:");

			// Print current values
			Console.WriteLine(" - Id: " + test.Id.ToString());
			Console.WriteLine(" - Name: " + test.Name.ToString());

			Console.WriteLine("\nCreating transaction");

			// Create a transaction
			test.BeginTransaction();

			Console.WriteLine("\nSetting new values...");

			// Update values
			test.Id = Guid.NewGuid();
			test.Name = "New name";

			Console.WriteLine("\nCurrent object properties:");

			// Print new values
			Console.WriteLine(" - Id: " + test.Id.ToString());
			Console.WriteLine(" - Name: " + test.Name.ToString());

			Console.WriteLine("\nUndoing changes...");

			// Undo changes
			test.Rollback();

			Console.WriteLine("\nCurrent object properties:");

			// Print current values
			Console.WriteLine(" - Id: " + test.Id.ToString());
			Console.WriteLine(" - Name: " + test.Name.ToString());

			test.EndTransaction();

			WaitForUser();
		}

		static void WaitForUser()
		{
			// Wait for user
			Console.WriteLine("\nPress Return to contimue");
			Console.ReadLine();
		}
	}
}
