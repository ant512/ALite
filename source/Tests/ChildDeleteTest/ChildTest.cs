using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.ChildDeleteTest
{
	/// <summary>
	/// Test that the DBObject Save() method fires a DBObjectDeleted event,
	/// which is received by the DBObjectCollection class, which in turn
	/// removes the DBObject from its list.
	/// </summary>
	class ChildTest : ITest
	{
		private string mName = "Child Delete Test";

		public string Name
		{
			get
			{
				return mName;
			}
		}

		public bool Test()
		{
			ChildCollection collection = new ChildCollection();

			for (int i = 0; i < 10; ++i)
			{
				collection.Add(new Child(i));
			}

			Child child = collection[5];
			child.MarkDeleted();
			child.Save();

			return (collection.Count == 9);
		}
	}
}
