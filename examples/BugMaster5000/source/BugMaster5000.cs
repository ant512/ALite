using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ALite.MongoDB;
using ALite.ObjectValidator;
using MongoDB;

namespace BugTracker5000
{
	public partial class BugMaster5000 : Form
	{
		private BugCollection mBugs = new BugCollection();
		private DataTable mBugTable = new DataTable();
		private Oid mEditingId;

		public BugMaster5000()
		{
			InitializeComponent();

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

			IEnumerable<Bug> bugList = mBugs.FindBugs(open, closed);

			foreach (Bug bug in bugList)
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

				Oid bugId = new Oid(row.Cells[0].Value.ToString());
				Bug bug = mBugs.First(m => m.Id == bugId);
				bug.Delete();
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
			Bug bug;

			if (mEditingId == null)
			{
				try
				{
					bug = new Bug();
					bug.Description = description;
					bug.IsOpen = true;
					bug.Save();

					mBugs.Add(bug);

					PopulateBugDataTable();
					BugDataGrid.Refresh();
				}
				catch (ValidationException ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			else
			{
				try
				{
					bug = mBugs.First(m => m.Id == mEditingId);
					bug.Description = description;
					bug.Save();

					BugDataGrid.CurrentRow.Cells[1].Value = description;
				}
				catch (ValidationException ex)
				{
					MessageBox.Show(ex.Message);
				}
			}

			CancelEdit();
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

			mEditingId = new Oid(bugId);
		}

		private void BugDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				Oid bugId = new Oid(BugDataGrid[0, e.RowIndex].Value.ToString());
				Bug bug = mBugs.First(m => m.Id == bugId);
				bug.IsOpen = !bug.IsOpen;
				bug.Save();

				PopulateBugDataTable();
				BugDataGrid.Refresh();
			}
		}
	}
}
