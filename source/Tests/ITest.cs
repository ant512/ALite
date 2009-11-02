using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	interface ITest
	{
		string Name
		{
			get;
		}

		bool Test();
	}
}
