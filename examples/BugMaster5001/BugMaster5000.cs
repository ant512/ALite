using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace BugTracker5000
{
	public partial class BugMaster5000 : Form
	{
		private DataTable mBugTable = new DataTable();
		private BsonObjectId mEditingId;

		private MongoServer mServer = null;
		private MongoDatabase mDB = null;
		private MongoCollection mCollection = null;

		public BugMaster5000()
		{
			InitializeComponent();

			mServer = MongoServer.Create("mongodb://localhost");
			mDB = mServer.GetDatabase("bug2");
			mCollection = mDB.GetCollection("bugs");

			BugDataGrid.AutoGenerateColumns = false;

			BugDataGrid.Columns.Add("Id", "Id");
			BugDataGrid.Columns.Add("Description", "Description");

			DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
			checkBoxColumn.HeaderText = "Open";
			checkBoxColumn.Name = "Open";
			BugDataGrid.Columns.Add(checkBoxColumn);

			CreateBugDataTable();
			PopulateBugDataTable();

			BugDataGrid.DataSource = mBugTable;
			BugDataGrid.Columns[0].DataPropertyName = "Id";
			BugDataGrid.Columns[1].DataPropertyName = "Description";
			BugDataGrid.Columns[2].DataPropertyName = "Open";

			BugDataGrid.Columns[0].Visible = false;
			BugDataGrid.Columns[1].Width = 400;
		}

		private void CreateBugDataTable()
		{
			mBugTable.Columns.Add("Id");
			mBugTable.Columns.Add("Description");
			mBugTable.Columns.Add("Open");
		}

		private void PopulateBugDataTable()
		{
			mBugTable.Clear();

			bool open = OpenBugsToolStripMenuItem.Checked;
			bool closed = ClosedBugsToolStripMenuItem.Checked;

			// MongoDB doesn't support queries complex enough to search for the correct
			// subset of bugs, so we use a lambda after returning the entire DB set.
			foreach (var bug in mCollection.FindAllAs<Bug>().Where(m => ((m.IsOpen && open) || (!m.IsOpen && closed))))
			{
				DataRow row = mBugTable.NewRow();
				row["Id"] = bug.Id.ToString().Replace("\"", "");
				row["Description"] = bug.Description;
				row["Open"] = bug.IsOpen;
				mBugTable.Rows.Add(row);
			}
		}

		private void OpenBugsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PopulateBugDataTable();
			BugDataGrid.Refresh();
		}

		private void ClosedBugsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PopulateBugDataTable();
			BugDataGrid.Refresh();
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}

		private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Delete selected bugs
			foreach (DataGridViewRow row in BugDataGrid.SelectedRows)
			{
				if (row.IsNewRow) continue;

				string bugId = row.Cells[0].Value.ToString();

				var query = Query.EQ("_id", new ObjectId(bugId));

				mCollection.Remove(query);
			}

			PopulateBugDataTable();
			BugDataGrid.Refresh();
		}

		private void CancelEdit()
		{
			mEditingId = null;
			BugEntryTextBox.Text = "";
			BugDataGrid.ForeColor = Color.Black;
			BugDataGrid.Enabled = true;
		}

		private void BugEntryTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 27)
			{
				CancelEdit();
			}

			if (e.KeyChar != 13) return;

			string description = BugEntryTextBox.Text;

			// Update or create the bug
			Bug bug = null;

			if (mEditingId == null)
			{
				try
				{
					bug = new Bug();

					bug.Description = description;
					bug.IsOpen = true;

					bug.Save();
				}
				catch (ALite.ObjectValidator.ValidationException ex)
				{
					MessageBox.Show(ex.Message);
				}

				PopulateBugDataTable();
				BugDataGrid.Refresh();
			}
			else
			{
				var query = Query.EQ("_id", mEditingId);
				bug = mCollection.FindOneAs<Bug>(query);
				bug.Description = description;

				bug.Save();

				BugDataGrid.CurrentRow.Cells[1].Value = description;
			}

			if (bug.State == ALite.Core.ModificationState.Unmodified)
			{
				CancelEdit();
			}
		}

		private void BugDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore clicks outside of a cell
			if (e.RowIndex < 0) return;

			BugDataGrid.Enabled = false;
			BugDataGrid.ForeColor = Color.Gray;

			// Get the content of the clicked row
			DataGridViewRow row = BugDataGrid.Rows[e.RowIndex];

			BugEntryTextBox.Text = row.Cells[1].Value.ToString();

			string bugId = row.Cells[0].Value.ToString();

			mEditingId = new BsonObjectId(bugId);
		}

		private void BugDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				var query = Query.EQ("_id", new ObjectId(BugDataGrid[0, e.RowIndex].Value.ToString()));

				var bug = mCollection.FindOneAs<Bug>(query);
				bug.IsOpen = !bug.IsOpen;
				bug.Save();

				PopulateBugDataTable();
				BugDataGrid.Refresh();
			}
		}
	}
}
