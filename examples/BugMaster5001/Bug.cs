using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ALite.MongoDBOfficial;

namespace BugTracker5000
{
	public class Bug : MongoBackedObject
	{
		private const string ServerName = "mongodb://localhost";
		private const string DatabaseName = "bug2";
		private const string CollectionName = "bugs";

		public string Description {
			get { return GetProperty<string>("Description"); }
			set { SetProperty<string>("Description", value); }
		}

		public bool IsOpen {
			get { return GetProperty<bool>("IsOpen"); }
			set { SetProperty<bool>("IsOpen", value); }
		}

		public Bug()
		{
			AddRules();
		}

		public Bug(ObjectId id)
		{
			Id = id;

			AddRules();
		}

		private void AddRules()
		{
			AddRule("Description", new ALite.ObjectValidator.StandardRules.StringLengthValidationRule(4, 100));
		}

		private void Upsert()
		{
			var server = MongoServer.Create(ServerName);
			var db = server.GetDatabase(DatabaseName);
			var collection = db.GetCollection(CollectionName);
			collection.Save(this);
		}

		protected override void CreateData()
		{
			Upsert();
		}

		protected override void UpdateData()
		{
			Upsert();
		}

		protected override void FetchData()
		{
			base.FetchData();
		}

		protected override void DeleteData()
		{
			var server = MongoServer.Create(ServerName);
			var db = server.GetDatabase(DatabaseName);
			var collection = db.GetCollection(CollectionName);
			collection.Remove(new QueryDocument("Id", Id));
		}
	}
}
