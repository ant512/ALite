using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALite.Core;
using ALite.MongoDB;
using ALite.ObjectValidator;
using MongoDB;

namespace BugTracker5000
{
	public class BugCollection : PersistedObjectCollection<Bug>
	{
		public BugCollection()
		{
			Fetch();
		}

		public void Fetch()
		{
			Mongo db = new Mongo();
			db.Connect();

			ICursor cursor = db["bug"]["bugs"].FindAll();

			foreach (Document document in cursor.Documents)
			{
				Add(new Bug(document));
			}

			db.Disconnect();
		}

		public IEnumerable<Bug> FindBugs(bool open, bool closed)
		{
			foreach (Bug bug in this)
			{
				if ((bug.IsOpen && open) || (!bug.IsOpen && closed))
				{
					yield return bug;
				}
			}
		}
	}
}
