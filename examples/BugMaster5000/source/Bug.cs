using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALite.Core;
using ALite.MongoDB;
using ALite.ObjectValidator;
using ALite.ObjectValidator.StandardRules;
using MongoDB;

namespace BugTracker5000
{
	public class Bug : MongoBackedObject
	{
		public string Description
		{
			get { return GetProperty<string>("Description"); }
			set { SetProperty<string>("Description", value); }
		}

		public bool IsOpen
		{
			get { return GetProperty<bool>("IsOpen"); }
			set { SetProperty<bool>("IsOpen", value); }
		}

		public Bug(Oid id)
			: base("bug", "bugs")
		{
			Id = id;
			AddRules();
		}

		public Bug()
			: base("bug", "bugs")
		{
			AddRules();
		}

		public Bug(Document document)
			: base("bug", "bugs", document)
		{
			AddRules();
		}

		private void AddRules()
		{
			AddRule("Description", new StringLengthValidationRule(3, 100));
		}
	}
}
