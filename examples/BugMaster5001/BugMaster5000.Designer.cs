namespace BugTracker5000
{
	partial class BugMaster5000
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.BugDataGrid = new System.Windows.Forms.DataGridView();
			this.BugGridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuStrip = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ClosedBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BugEntryTextBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.BugDataGrid)).BeginInit();
			this.BugGridContextMenu.SuspendLayout();
			this.TopMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// BugDataGrid
			// 
			this.BugDataGrid.AllowUserToAddRows = false;
			this.BugDataGrid.AllowUserToDeleteRows = false;
			this.BugDataGrid.AllowUserToResizeRows = false;
			this.BugDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.BugDataGrid.CausesValidation = false;
			this.BugDataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.BugDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.BugDataGrid.ContextMenuStrip = this.BugGridContextMenu;
			this.BugDataGrid.Cursor = System.Windows.Forms.Cursors.Default;
			this.BugDataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.BugDataGrid.Location = new System.Drawing.Point(13, 27);
			this.BugDataGrid.Name = "BugDataGrid";
			this.BugDataGrid.RowHeadersVisible = false;
			this.BugDataGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.BugDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.BugDataGrid.Size = new System.Drawing.Size(558, 255);
			this.BugDataGrid.TabIndex = 1;
			this.BugDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.BugDataGrid_CellContentClick);
			this.BugDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.BugDataGrid_CellDoubleClick);
			// 
			// BugGridContextMenu
			// 
			this.BugGridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DeleteToolStripMenuItem});
			this.BugGridContextMenu.Name = "BugGridContextMenu";
			this.BugGridContextMenu.Size = new System.Drawing.Size(117, 26);
			// 
			// DeleteToolStripMenuItem
			// 
			this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
			this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.DeleteToolStripMenuItem.Text = "Delete";
			this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
			// 
			// TopMenuStrip
			// 
			this.TopMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.viewToolStripMenuItem});
			this.TopMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.TopMenuStrip.Name = "TopMenuStrip";
			this.TopMenuStrip.Size = new System.Drawing.Size(583, 24);
			this.TopMenuStrip.TabIndex = 2;
			this.TopMenuStrip.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExitToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
			this.toolStripMenuItem1.Text = "File";
			// 
			// ExitToolStripMenuItem
			// 
			this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
			this.ExitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
			this.ExitToolStripMenuItem.Text = "Exit";
			this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenBugsToolStripMenuItem,
            this.ClosedBugsToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// OpenBugsToolStripMenuItem
			// 
			this.OpenBugsToolStripMenuItem.Checked = true;
			this.OpenBugsToolStripMenuItem.CheckOnClick = true;
			this.OpenBugsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.OpenBugsToolStripMenuItem.Name = "OpenBugsToolStripMenuItem";
			this.OpenBugsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.OpenBugsToolStripMenuItem.Text = "Open Bugs";
			this.OpenBugsToolStripMenuItem.Click += new System.EventHandler(this.OpenBugsToolStripMenuItem_Click);
			// 
			// ClosedBugsToolStripMenuItem
			// 
			this.ClosedBugsToolStripMenuItem.CheckOnClick = true;
			this.ClosedBugsToolStripMenuItem.Name = "ClosedBugsToolStripMenuItem";
			this.ClosedBugsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.ClosedBugsToolStripMenuItem.Text = "Closed Bugs";
			this.ClosedBugsToolStripMenuItem.Click += new System.EventHandler(this.ClosedBugsToolStripMenuItem_Click);
			// 
			// BugEntryTextBox
			// 
			this.BugEntryTextBox.Location = new System.Drawing.Point(13, 288);
			this.BugEntryTextBox.Name = "BugEntryTextBox";
			this.BugEntryTextBox.Size = new System.Drawing.Size(558, 20);
			this.BugEntryTextBox.TabIndex = 3;
			this.BugEntryTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BugEntryTextBox_KeyPress);
			// 
			// BugMaster5000
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(583, 320);
			this.Controls.Add(this.BugEntryTextBox);
			this.Controls.Add(this.TopMenuStrip);
			this.Controls.Add(this.BugDataGrid);
			this.Name = "BugMaster5000";
			this.Text = "BugMaster5000";
			((System.ComponentModel.ISupportInitialize)(this.BugDataGrid)).EndInit();
			this.BugGridContextMenu.ResumeLayout(false);
			this.TopMenuStrip.ResumeLayout(false);
			this.TopMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView BugDataGrid;
		private System.Windows.Forms.MenuStrip TopMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem OpenBugsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ClosedBugsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip BugGridContextMenu;
		private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
		private System.Windows.Forms.TextBox BugEntryTextBox;

	}
}

