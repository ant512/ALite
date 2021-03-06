﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALite.Core;
using MongoDB;

namespace ALite.MongoDBSamus
{
	/// <summary>
	/// Base class for objects that interact with the database.
	/// </summary>
	[Serializable]
	public class MongoBackedObject : PersistedObject<Document>
	{
		#region Properties

		/// <summary>
		/// The name of the database that the object is stored in.
		/// </summary>
		protected string DatabaseName
		{
			get;
			private set;
		}

		/// <summary>
		/// The name of the collection that the object is stored in.
		/// </summary>
		protected string CollectionName
		{
			get;
			private set;
		}

		/// <summary>
		/// The ID of the object.
		/// </summary>
		public Oid Id
		{
			get { return GetProperty<Oid>("_id"); }
			set { Fetch(value); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MongoBackedObject class.
		/// </summary>
		/// <param name="databaseName">The name of the database that stores the object.</param>
		/// <param name="collectionName">The name of the collection that stores the object.</param>
		public MongoBackedObject(string databaseName, string collectionName)
			: this(databaseName, collectionName, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MongoBackedObject class.
		/// </summary>
		/// <param name="databaseName">The name of the database that stores the object.</param>
		/// <param name="collectionName">The name of the collection that stores the object.</param>
		/// <param name="document">The object's data fetched from MongoDB.</param>
		public MongoBackedObject(string databaseName, string collectionName, Document document)
			: base(new PropertyStore())
		{
			DatabaseName = databaseName;
			CollectionName = collectionName;
			
			if (document != null) InjectData(document);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Inserts or updates the document's data.
		/// </summary>
		private void Upsert()
		{
			if (State != ModificationState.New && State != ModificationState.Modified) return;

			// Prepare the database connection
			Mongo db = new Mongo();
			db.Connect();
			db[DatabaseName][CollectionName].Save(Properties.Document);
			db.Disconnect();
		}

		/// <summary>
		/// Inserts the object's document into MongoDB.
		/// </summary>
		protected override void CreateData()
		{
			Upsert();
		}

		/// <summary>
		/// Deletes the object's document from MongoDB.
		/// </summary>
		protected override void DeleteData()
		{
			Mongo db = new Mongo();
			db.Connect();

			lock (Properties)
			{
				db[DatabaseName][CollectionName].Remove(Properties.Document);
			}

			db.Disconnect();
		}

		/// <summary>
		/// Fetches the object's data from MongoDB.
		/// </summary>
		protected override void FetchData()
		{
			Document doc = FetchDocument(Properties.Document, DatabaseName, CollectionName);

			if (doc != null)
			{
				Properties.InjectData(doc);
			}
			else
			{
				Properties.InjectData(new Document());
			}
		}

		/// <summary>
		/// Fetch the object based on its ID.
		/// </summary>
		/// <param name="id">The ID of the object to retrieve.</param>
		protected void Fetch(Oid id)
		{
			var query = new Document();
			query.Add("_id", id);

			Properties.InjectData(query);

			Fetch();
		}

		/// <summary>
		/// Updates the stored data.
		/// </summary>
		protected override void UpdateData()
		{
			Upsert();
		}

		/// <summary>
		/// Fetch the object based on the specified document.
		/// </summary>
		/// <param name="query">Document prototype to retrieve.</param>
		/// <param name="collectionName">The name of the MongoDB collection that stores the document.</param>
		/// <param name="databaseName">The name of the MongoDB database that stores the document.</param>
		/// <returns>The matching document, or null if no match was found.</returns>
		protected static Document FetchDocument(Document query, string databaseName, string collectionName)
		{
			Mongo db = new Mongo();
			db.Connect();

			Document retrieved = db[databaseName][collectionName].FindOne(query);

			db.Disconnect();

			return retrieved;
		}

		#endregion
	}
}
