namespace PckStudio.Forms.Editor
{
    partial class ModelEditor
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
            this.modelTreeView = new System.Windows.Forms.TreeView();
            this.modelContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelTreeView
            // 
            this.modelTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.modelTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modelTreeView.ContextMenuStrip = this.modelContextMenu;
            this.modelTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelTreeView.ForeColor = System.Drawing.SystemColors.Window;
            this.modelTreeView.Location = new System.Drawing.Point(20, 60);
            this.modelTreeView.Name = "modelTreeView";
            this.modelTreeView.PathSeparator = ".";
            this.modelTreeView.Size = new System.Drawing.Size(335, 395);
            this.modelTreeView.TabIndex = 0;
            this.modelTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.modelTreeView_BeforeSelect);
            // 
            // modelContextMenu
            // 
            this.modelContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.editToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.modelContextMenu.Name = "modelContextMenu";
            this.modelContextMenu.Size = new System.Drawing.Size(118, 70);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Visible = false;
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Visible = false;
            // 
            // ModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 475);
            this.Controls.Add(this.modelTreeView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(375, 475);
            this.Name = "ModelEditor";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "ModelEditor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.modelContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView modelTreeView;
        private MetroFramework.Controls.MetroContextMenu modelContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    }
}