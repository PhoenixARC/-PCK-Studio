namespace PckStudio.Forms.Editor
{
    partial class GameRuleFileEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameRuleFileEditor));
            this.GrfTreeView = new System.Windows.Forms.TreeView();
            this.MessageContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.addGameRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeGameRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrfParametersTreeView = new System.Windows.Forms.TreeView();
            this.DetailContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionLvlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.compressionTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionTypeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.MessageContextMenu.SuspendLayout();
            this.DetailContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GrfTreeView
            // 
            this.GrfTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.GrfTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GrfTreeView.ContextMenuStrip = this.MessageContextMenu;
            this.GrfTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.GrfTreeView.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.GrfTreeView.Location = new System.Drawing.Point(0, 0);
            this.GrfTreeView.Name = "GrfTreeView";
            this.GrfTreeView.Size = new System.Drawing.Size(223, 312);
            this.GrfTreeView.TabIndex = 0;
            this.GrfTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.GrfTreeView_AfterSelect);
            this.GrfTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrfTreeView_KeyDown);
            // 
            // MessageContextMenu
            // 
            this.MessageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGameRuleToolStripMenuItem,
            this.removeGameRuleToolStripMenuItem});
            this.MessageContextMenu.Name = "MessageContextMenu";
            this.MessageContextMenu.Size = new System.Drawing.Size(178, 48);
            // 
            // addGameRuleToolStripMenuItem
            // 
            this.addGameRuleToolStripMenuItem.Name = "addGameRuleToolStripMenuItem";
            this.addGameRuleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.addGameRuleToolStripMenuItem.Text = "Add Game Rule";
            this.addGameRuleToolStripMenuItem.Click += new System.EventHandler(this.addGameRuleToolStripMenuItem_Click);
            // 
            // removeGameRuleToolStripMenuItem
            // 
            this.removeGameRuleToolStripMenuItem.Name = "removeGameRuleToolStripMenuItem";
            this.removeGameRuleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.removeGameRuleToolStripMenuItem.Text = "Remove Game Rule";
            this.removeGameRuleToolStripMenuItem.Click += new System.EventHandler(this.removeGameRuleToolStripMenuItem_Click);
            // 
            // GrfParametersTreeView
            // 
            this.GrfParametersTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.GrfParametersTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GrfParametersTreeView.ContextMenuStrip = this.DetailContextMenu;
            this.GrfParametersTreeView.Dock = System.Windows.Forms.DockStyle.Right;
            this.GrfParametersTreeView.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.GrfParametersTreeView.Location = new System.Drawing.Point(227, 0);
            this.GrfParametersTreeView.Name = "GrfParametersTreeView";
            this.GrfParametersTreeView.Size = new System.Drawing.Size(223, 312);
            this.GrfParametersTreeView.TabIndex = 1;
            this.GrfParametersTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.GrfDetailsTreeView_NodeMouseDoubleClick);
            this.GrfParametersTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GrfDetailsTreeView_KeyDown);
            // 
            // DetailContextMenu
            // 
            this.DetailContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem1,
            this.removeToolStripMenuItem});
            this.DetailContextMenu.Name = "DetailContextMenu";
            this.DetailContextMenu.Size = new System.Drawing.Size(118, 48);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem1.Text = "Add";
            this.addToolStripMenuItem1.Click += new System.EventHandler(this.addDetailContextMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(25, 88);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(73, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Game Rule";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            this.metroLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(252, 88);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(75, 19);
            this.metroLabel2.TabIndex = 0;
            this.metroLabel2.Text = "Parameters";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.compressionLvlToolStripMenuItem,
            this.compressionTypeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(25, 60);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(450, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Menu;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // compressionLvlToolStripMenuItem
            // 
            this.compressionLvlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.compressionLvlToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Menu;
            this.compressionLvlToolStripMenuItem.Name = "compressionLvlToolStripMenuItem";
            this.compressionLvlToolStripMenuItem.Size = new System.Drawing.Size(106, 20);
            this.compressionLvlToolStripMenuItem.Text = "Compression Lvl";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "None",
            "Compressed",
            "Compressed + RLE",
            "Compressed + RLE + CRC"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.Text = "None";
            // 
            // compressionTypeToolStripMenuItem
            // 
            this.compressionTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compressionTypeComboBox});
            this.compressionTypeToolStripMenuItem.Name = "compressionTypeToolStripMenuItem";
            this.compressionTypeToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
            this.compressionTypeToolStripMenuItem.Text = "Compression Type";
            // 
            // compressionTypeComboBox
            // 
            this.compressionTypeComboBox.Items.AddRange(new object[] {
            "Zlib",
            "Deflate",
            "XMem"});
            this.compressionTypeComboBox.Name = "compressionTypeComboBox";
            this.compressionTypeComboBox.Size = new System.Drawing.Size(121, 23);
            // 
            // metroPanel1
            // 
            this.metroPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroPanel1.Controls.Add(this.GrfParametersTreeView);
            this.metroPanel1.Controls.Add(this.GrfTreeView);
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(25, 110);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(450, 312);
            this.metroPanel1.TabIndex = 4;
            this.metroPanel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            this.metroPanel1.Resize += new System.EventHandler(this.metroPanel1_Resize);
            // 
            // GRFEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 450);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 450);
            this.Name = "GRFEditor";
            this.Padding = new System.Windows.Forms.Padding(25, 60, 25, 25);
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "GRF Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.OnLoad);
            this.MessageContextMenu.ResumeLayout(false);
            this.DetailContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.metroPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView GrfTreeView;
        private System.Windows.Forms.TreeView GrfParametersTreeView;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroContextMenu MessageContextMenu;
        private MetroFramework.Controls.MetroContextMenu DetailContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addGameRuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeGameRuleToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private System.Windows.Forms.ToolStripMenuItem compressionLvlToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem compressionTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox compressionTypeComboBox;
    }
}