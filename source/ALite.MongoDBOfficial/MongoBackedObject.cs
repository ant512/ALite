using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using ALite.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ALite.MongoDBOfficial
{
	[Serializable]
	public abstract class MongoBackedObject : PersistedObject<DynamicStore>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlBackedObject class.
		/// </summary>
		public MongoBackedObject()
			: base(new PropertyStore())
		{
			Id = ObjectId.GenerateNewId();
		}

		#endregion

		#region Properties

		[BsonId]
		public ObjectId Id
		{
			get { return GetProperty<ObjectId>("Id"); }
			set { SetProperty<ObjectId>("Id", value); }
		}

		#endregion
	}
}
