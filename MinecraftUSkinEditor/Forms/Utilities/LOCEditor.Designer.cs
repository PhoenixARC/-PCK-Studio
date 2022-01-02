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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.deleteDisplayIDToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // addDisplayIDToolStripMenuItem
            // 
            this.addDisplayIDToolStripMenuItem.Name = "addDisplayIDToolStripMenuItem";
            resources.ApplyResources(this.addDisplayIDToolStripMenuItem, "addDisplayIDToolStripMenuItem");
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
            this.locSort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            resources.ApplyResources(this.locSort, "locSort");
            this.locSort.Controls.Add(this.buttonReplaceAll, 2, 0);
            this.locSort.Controls.Add(this.dataGridViewLocEntryData, 1, 1);
            this.locSort.Controls.Add(this.textBoxReplaceAll, 1, 0);
            this.locSort.Controls.Add(this.treeViewLocEntries, 0, 0);
            this.locSort.ForeColor = System.Drawing.Color.DimGray;
            this.locSort.Name = "locSort";
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.BackColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.buttonReplaceAll, "buttonReplaceAll");
            this.buttonReplaceAll.ForeColor = System.Drawing.Color.White;
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.UseVisualStyleBackColor = false;
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // dataGridViewLocEntryData
            // 
            this.dataGridViewLocEntryData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.dataGridViewLocEntryData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewLocEntryData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewLocEntryData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.locSort.SetColumnSpan(this.dataGridViewLocEntryData, 2);
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewLocEntryData.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dataGridViewLocEntryData, "dataGridViewLocEntryData");
            this.dataGridViewLocEntryData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.dataGridViewLocEntryData.Name = "dataGridViewLocEntryData";
            this.dataGridViewLocEntryData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // textBoxReplaceAll
            // 
            this.textBoxReplaceAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.textBoxReplaceAll.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxReplaceAll, "textBoxReplaceAll");
            this.textBoxReplaceAll.ForeColor = System.Drawing.Color.White;
            this.textBoxReplaceAll.Name = "textBoxReplaceAll";
            // 
            // treeViewLocEntries
            // 
            this.treeViewLocEntries.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.treeViewLocEntries.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewLocEntries.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.treeViewLocEntries, "treeViewLocEntries");
            this.treeViewLocEntries.ForeColor = System.Drawing.Color.White;
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
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "LOCEditor";
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
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
    }
}