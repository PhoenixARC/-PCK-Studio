namespace PckStudio.Forms.Editor
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDisplayIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GridContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.addLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.locSort = new System.Windows.Forms.TableLayoutPanel();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.dataGridViewLocEntryData = new System.Windows.Forms.DataGridView();
            this.textBoxReplaceAll = new System.Windows.Forms.TextBox();
            this.treeViewLocKeys = new System.Windows.Forms.TreeView();
            this.ReplaceAllButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.contextMenuStrip1.SuspendLayout();
            this.GridContextMenu.SuspendLayout();
            this.menuStrip.SuspendLayout();
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
            this.addDisplayIDToolStripMenuItem.Click += new System.EventHandler(this.addDisplayIDToolStripMenuItem_Click);
            // 
            // deleteDisplayIDToolStripMenuItem
            // 
            this.deleteDisplayIDToolStripMenuItem.Name = "deleteDisplayIDToolStripMenuItem";
            resources.ApplyResources(this.deleteDisplayIDToolStripMenuItem, "deleteDisplayIDToolStripMenuItem");
            this.deleteDisplayIDToolStripMenuItem.Click += new System.EventHandler(this.deleteDisplayIDToolStripMenuItem_Click);
            // 
            // GridContextMenu
            // 
            this.GridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLanguageToolStripMenuItem,
            this.removeLanguageToolStripMenuItem});
            this.GridContextMenu.Name = "GridContextMenu";
            resources.ApplyResources(this.GridContextMenu, "GridContextMenu");
            this.GridContextMenu.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // addLanguageToolStripMenuItem
            // 
            this.addLanguageToolStripMenuItem.Name = "addLanguageToolStripMenuItem";
            resources.ApplyResources(this.addLanguageToolStripMenuItem, "addLanguageToolStripMenuItem");
            this.addLanguageToolStripMenuItem.Click += new System.EventHandler(this.addLanguageToolStripMenuItem_Click);
            // 
            // removeLanguageToolStripMenuItem
            // 
            this.removeLanguageToolStripMenuItem.Name = "removeLanguageToolStripMenuItem";
            resources.ApplyResources(this.removeLanguageToolStripMenuItem, "removeLanguageToolStripMenuItem");
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // locSort
            // 
            this.locSort.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
            resources.ApplyResources(this.locSort, "locSort");
            this.locSort.Controls.Add(this.dataGridViewLocEntryData, 1, 1);
            this.locSort.Controls.Add(this.textBoxReplaceAll, 1, 0);
            this.locSort.Controls.Add(this.treeViewLocKeys, 0, 0);
            this.locSort.Controls.Add(this.ReplaceAllButton, 2, 0);
            this.locSort.ForeColor = System.Drawing.Color.Black;
            this.locSort.Name = "locSort";
            // 
            // dataGridViewLocEntryData
            // 
            this.dataGridViewLocEntryData.AllowUserToAddRows = false;
            this.dataGridViewLocEntryData.AllowUserToDeleteRows = false;
            this.dataGridViewLocEntryData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.dataGridViewLocEntryData.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewLocEntryData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewLocEntryData.ColumnHeadersVisible = false;
            this.locSort.SetColumnSpan(this.dataGridViewLocEntryData, 2);
            this.dataGridViewLocEntryData.ContextMenuStrip = this.GridContextMenu;
            resources.ApplyResources(this.dataGridViewLocEntryData, "dataGridViewLocEntryData");
            this.dataGridViewLocEntryData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.dataGridViewLocEntryData.Name = "dataGridViewLocEntryData";
            this.dataGridViewLocEntryData.RowHeadersVisible = false;
            this.dataGridViewLocEntryData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // textBoxReplaceAll
            // 
            this.textBoxReplaceAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.textBoxReplaceAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.textBoxReplaceAll, "textBoxReplaceAll");
            this.textBoxReplaceAll.ForeColor = System.Drawing.Color.White;
            this.textBoxReplaceAll.Name = "textBoxReplaceAll";
            // 
            // treeViewLocKeys
            // 
            this.treeViewLocKeys.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.treeViewLocKeys.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewLocKeys.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.treeViewLocKeys, "treeViewLocKeys");
            this.treeViewLocKeys.ForeColor = System.Drawing.Color.White;
            this.treeViewLocKeys.LabelEdit = true;
            this.treeViewLocKeys.Name = "treeViewLocKeys";
            this.locSort.SetRowSpan(this.treeViewLocKeys, 2);
            this.treeViewLocKeys.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLocKeys_AfterSelect);
            this.treeViewLocKeys.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // ReplaceAllButton
            // 
            this.ReplaceAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ReplaceAllButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ReplaceAllButton.BorderRadius = 10;
            this.ReplaceAllButton.BorderSize = 1;
            this.ReplaceAllButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ReplaceAllButton.FlatAppearance.BorderSize = 0;
            this.ReplaceAllButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ReplaceAllButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            resources.ApplyResources(this.ReplaceAllButton, "ReplaceAllButton");
            this.ReplaceAllButton.ForeColor = System.Drawing.Color.White;
            this.ReplaceAllButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.ReplaceAllButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.ReplaceAllButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.ReplaceAllButton.Name = "ReplaceAllButton";
            this.ReplaceAllButton.TextColor = System.Drawing.Color.White;
            this.ReplaceAllButton.UseVisualStyleBackColor = false;
            this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
            // 
            // LOCEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.locSort);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "LOCEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LOCEditor_FormClosing);
            this.Load += new System.EventHandler(this.LOCEditor_Load);
            this.Resize += new System.EventHandler(this.LOCEditor_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.GridContextMenu.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.locSort.ResumeLayout(false);
            this.locSort.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLocEntryData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewLocKeys;
        private System.Windows.Forms.DataGridView dataGridViewLocEntryData;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addDisplayIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDisplayIDToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxReplaceAll;
        private System.Windows.Forms.TableLayoutPanel locSort;
        private System.Windows.Forms.Button buttonReplaceAll;
        private MetroFramework.Controls.MetroContextMenu GridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addLanguageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeLanguageToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton ReplaceAllButton;
    }
}