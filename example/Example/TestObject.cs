using System;
using System.Collections.Generic;
using System.Text;
using ALite;

namespace Example
{
	class TestObject : DBObject
	{
		#region Members

		private Guid mId;
		private string mName;

		#endregion

		#region Properties

		public Guid Id
		{
			get { return mId; }
			set { SetProperty<Guid>("Id", ref mId, value); }
		}

		public string Name
		{
			get { return mName; }
			set { SetProperty<string>("Name", ref mName, value); }
		}

		#endregion

		#region Constructors

		public TestObject()
		{
			mId = Guid.NewGuid();

			AddRule(ValidationRule.RuleType.MaxLength, 20, "Name");
		}

		#endregion

		#region Methods

		public override DBErrorCode Fetch()
		{
			DataAccess data = new DataAccess();
			DBErrorCode status = DBErrorCode.Ok;

			// Prepare data access object
			data.Procedure = "uspTestObjectSelect";
			data.AddParameter("@ObjectID", mId);

			// Attempt to retrieve data
			if (data.Fetch())
			{
				mName = data.GetString("ObjectName");

				// Call base method
				base.Fetch();
			}
			else
			{
				// Fetch failed; update status
				status = DBErrorCode.Failed;
			}

			data.Dispose();

			return status;
		}

		protected override DBErrorCode Create()
		{
			DataAccess data = new DataAccess();
			DBErrorCode status = DBErrorCode.Ok;

			// Save to database
			data.Procedure = "uspTestObjectCreate";
			data.AddParameter("@ObjectID", mId);
			data.AddParameter("@ObjectName", mName);
			data.Save();

			// Call base method
			base.Create();

			data.Dispose();

			return status;
		}

		protected override DBErrorCode Update()
		{
			DataAccess data = new DataAccess();
			DBErrorCode status = DBErrorCode.Ok;

			// Update database
			data.Procedure = "uspTestObjectUpdate";
			data.AddParameter("@ObjectID", mId);
			data.AddParameter("@ObjectName", mName);
			data.Save();

			// Call base method
			base.Update();

			data.Dispose();

			return status;
		}

		protected override DBErrorCode Delete()
		{
			DataAccess data = new DataAccess();
			DBErrorCode status = DBErrorCode.Ok;

			// Delete from database
			data.Procedure = "uspTestObjectDelete";
			data.AddParameter("@ObjectID", mId);
			data.Save();

			// Call base method
			base.Delete();

			data.Dispose();

			return status;
		}

		#endregion
	}
}