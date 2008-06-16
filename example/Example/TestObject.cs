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
			DBErrorCode status = DBErrorCode.Ok;
			
			using (DataAccess data = new DataAccess())
			{
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
			}

			return status;
		}

		protected override DBErrorCode Create()
		{
			DBErrorCode status = DBErrorCode.Ok;

			using (DataAccess data = new DataAccess())
			{
				// Save to database
				data.Procedure = "uspTestObjectCreate";
				data.AddParameter("@ObjectID", mId);
				data.AddParameter("@ObjectName", mName);
				data.Save();

				// Call base method
				base.Create();
			}

			return status;
		}

		protected override DBErrorCode Update()
		{
			DBErrorCode status = DBErrorCode.Ok;

			using (DataAccess data = new DataAccess())
			{
				// Update database
				data.Procedure = "uspTestObjectUpdate";
				data.AddParameter("@ObjectID", mId);
				data.AddParameter("@ObjectName", mName);
				data.Save();

				// Call base method
				base.Update();
			}

			return status;
		}

		protected override DBErrorCode Delete()
		{
			DBErrorCode status = DBErrorCode.Ok;

			using (DataAccess data = new DataAccess())
			{
				// Delete from database
				data.Procedure = "uspTestObjectDelete";
				data.AddParameter("@ObjectID", mId);
				data.Save();

				// Call base method
				base.Delete();
			}

			return status;
		}

		#endregion
	}
}