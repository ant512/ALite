using System;
using System.Collections.Generic;
using System.Text;
using ALite;

namespace Tests.ChildDeleteTest
{
	class Child : DBObject
	{
		private int mID;

		public Child(int id)
		{
			mID = id;
		}
	}
}
