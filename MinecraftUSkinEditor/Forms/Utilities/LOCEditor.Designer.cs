namespace PckStudio
{
    partial class LOCEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LOCEditor));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.locSort = new PckStudio.Forms.MyTablePanel();
			this.buttonReplaceAll = new System.Windows.Forms.Button();
			this.dataGridViewLocEntryData = new System.Windows.Forms.DataGridView();
			this.textBoxReplaceAll = new System.Windows.Forms.TextBox();
			this.treeViewLocEntries = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1.SuspendLayout();
			this.locSort.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewLocEntryData)).BeginInit();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDisplayIDToolStripMenuItem,
            this.renameDisplayIDToolStripMenuItem,
            this.deleteDisplayIDToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
			// 
			// addDisplayIDToolStripMenuItem
			// 
			this.addDisplayIDToolStripMenuItem.Name = "addDisplayIDToolStripMenuItem";
			resources.ApplyResources(this.addDisplayIDToolStripMenuItem, "addDisplayIDToolStripMenuItem");
			this.addDisplayIDToolStripMenuItem.Click += new System.EventHandler(this.addDisplayIDToolStripMenuItem_Click);
			// 
			// renameDisplayIDToolStripMenuItem
			// 
			this.renameDisplayIDToolStripMenuItem.Name = "renameDisplayIDToolStripMenuItem";
			resources.ApplyResources(this.renameDisplayIDToolStripMenuItem, "renameDisplayIDToolStripMenuItem");
			this.renameDisplayIDToolStripMenuItem.Click += new System.EventHandler(this.renameDisplayIDToolStripMenuItem_Click);
			// 
			// deleteDisplayIDToolStripMenuItem
			// 
			this.deleteDisplayIDToolStripMenuItem.Name = "deleteDisplayIDToolStripMenuItem";
			resources.ApplyResources(this.deleteDisplayIDToolStripMenuItem, "deleteDisplayIDToolStripMenuItem");
			this.deleteDisplayIDToolStripMenuItem.Click += new System.EventHandler(this.deleteDisplayIDToolStripMenuItem_Click);
			// 
			// locSort
			// 
			this.locSort.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
			resources.ApplyResources(this.locSort, "locSort");
			this.locSort.Controls.Add(this.buttonReplaceAll, 2, 0);
			this.locSort.Controls.Add(this.dataGridViewLocEntryData, 1, 1);
			this.locSort.Controls.Add(this.textBoxReplaceAll, 1, 0);
			this.locSort.Controls.Add(this.treeViewLocEntries, 0, 0);
			this.locSort.Name = "locSort";
			// 
			// buttonReplaceAll
			// 
			resources.ApplyResources(this.buttonReplaceAll, "buttonReplaceAll");
			this.buttonReplaceAll.ForeColor = System.Drawing.Color.White;
			this.buttonReplaceAll.Name = "buttonReplaceAll";
			this.buttonReplaceAll.UseVisualStyleBackColor = true;
			this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
			// 
			// dataGridViewLocEntryData
			// 
			this.dataGridViewLocEntryData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.locSort.SetColumnSpan(this.dataGridViewLocEntryData, 2);
			resources.ApplyResources(this.dataGridViewLocEntryData, "dataGridViewLocEntryData");
			this.dataGridViewLocEntryData.Name = "dataGridViewLocEntryData";
			this.dataGridViewLocEntryData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
			// 
			// textBoxReplaceAll
			// 
			resources.ApplyResources(this.textBoxReplaceAll, "textBoxReplaceAll");
			this.textBoxReplaceAll.Name = "textBoxReplaceAll";
			// 
			// treeViewLocEntries
			// 
			this.treeViewLocEntries.ContextMenuStrip = this.contextMenuStrip1;
			resources.ApplyResources(this.treeViewLocEntries, "treeViewLocEntries");
			this.treeViewLocEntries.LabelEdit = true;
			this.treeViewLocEntries.Name = "treeViewLocEntries";
			this.locSort.SetRowSpan(this.treeViewLocEntries, 2);
			this.treeViewLocEntries.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeViewLocEntries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			// 
			// LOCEditor
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.locSort);
			this.Name = "LOCEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.Load += new System.EventHandler(this.LOCEditor_Load);
			this.contextMenuStrip1.ResumeLayout(false);
			this.locSort.ResumeLayout(false);
			this.locSort.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewLocEntryData)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewLocEntries;
        private System.Windows.Forms.DataGridView dataGridViewLocEntryData;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addDisplayIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDisplayIDToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxReplaceAll;
        private PckStudio.Forms.MyTablePanel locSort;
        private System.Windows.Forms.Button buttonReplaceAll;
		private System.Windows.Forms.ToolStripMenuItem renameDisplayIDToolStripMenuItem;
	}
}