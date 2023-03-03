namespace PckStudio.Forms.Editor
{
	partial class BehaviourEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaviourEditor));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.metroContextMenu1 = new MetroFramework.Controls.MetroContextMenu(this.components);
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addNewEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addNewPositionOverrideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flag1Checkbox = new MetroFramework.Controls.MetroCheckBox();
			this.flag2Checkbox = new MetroFramework.Controls.MetroCheckBox();
			this.zUpDown = new System.Windows.Forms.NumericUpDown();
			this.yUpDown = new System.Windows.Forms.NumericUpDown();
			this.zLabel = new MetroFramework.Controls.MetroLabel();
			this.yLabel = new MetroFramework.Controls.MetroLabel();
			this.xUpDown = new System.Windows.Forms.NumericUpDown();
			this.xLabel = new MetroFramework.Controls.MetroLabel();
			this.metroContextMenu1.SuspendLayout();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.yUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.treeView1.ContextMenuStrip = this.metroContextMenu1;
			this.treeView1.ForeColor = System.Drawing.Color.White;
			this.treeView1.Location = new System.Drawing.Point(20, 84);
			this.treeView1.Margin = new System.Windows.Forms.Padding(0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(186, 176);
			this.treeView1.TabIndex = 13;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			this.treeView1.MouseHover += new System.EventHandler(this.treeView1_MouseHover);
			// 
			// metroContextMenu1
			// 
			this.metroContextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.removeToolStripMenuItem});
			this.metroContextMenu1.Name = "metroContextMenu1";
			this.metroContextMenu1.Size = new System.Drawing.Size(118, 70);
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewEntryToolStripMenuItem,
            this.addNewPositionOverrideToolStripMenuItem});
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.addToolStripMenuItem.Text = "Add";
			// 
			// addNewEntryToolStripMenuItem
			// 
			this.addNewEntryToolStripMenuItem.Name = "addNewEntryToolStripMenuItem";
			this.addNewEntryToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.addNewEntryToolStripMenuItem.Text = "Add New Entry";
			this.addNewEntryToolStripMenuItem.Click += new System.EventHandler(this.addNewEntryToolStripMenuItem_Click);
			// 
			// addNewPositionOverrideToolStripMenuItem
			// 
			this.addNewPositionOverrideToolStripMenuItem.Name = "addNewPositionOverrideToolStripMenuItem";
			this.addNewPositionOverrideToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
			this.addNewPositionOverrideToolStripMenuItem.Text = "Add New Position Override";
			this.addNewPositionOverrideToolStripMenuItem.Click += new System.EventHandler(this.addNewPositionOverrideToolStripMenuItem_Click);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.removeToolStripMenuItem.Text = "Remove";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.AutoSize = false;
			this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(20, 60);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(309, 24);
			this.menuStrip.TabIndex = 14;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1});
			this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// saveToolStripMenuItem1
			// 
			this.saveToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem1.Image")));
			this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
			this.saveToolStripMenuItem1.Size = new System.Drawing.Size(98, 22);
			this.saveToolStripMenuItem1.Text = "Save";
			this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// flag1Checkbox
			// 
			this.flag1Checkbox.AutoSize = true;
			this.flag1Checkbox.Enabled = false;
			this.flag1Checkbox.Location = new System.Drawing.Point(221, 104);
			this.flag1Checkbox.Name = "flag1Checkbox";
			this.flag1Checkbox.Size = new System.Drawing.Size(96, 15);
			this.flag1Checkbox.TabIndex = 22;
			this.flag1Checkbox.Text = "Mob is tamed";
			this.flag1Checkbox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.flag1Checkbox.UseSelectable = true;
			this.flag1Checkbox.CheckedChanged += new System.EventHandler(this.flag1Checkbox_CheckedChanged);
			// 
			// flag2Checkbox
			// 
			this.flag2Checkbox.AutoSize = true;
			this.flag2Checkbox.Enabled = false;
			this.flag2Checkbox.Location = new System.Drawing.Point(221, 136);
			this.flag2Checkbox.Name = "flag2Checkbox";
			this.flag2Checkbox.Size = new System.Drawing.Size(106, 15);
			this.flag2Checkbox.TabIndex = 23;
			this.flag2Checkbox.Text = "Mob has saddle";
			this.flag2Checkbox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.flag2Checkbox.UseSelectable = true;
			this.flag2Checkbox.CheckedChanged += new System.EventHandler(this.flag2Checkbox_CheckedChanged);
			// 
			// zUpDown
			// 
			this.zUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.zUpDown.DecimalPlaces = 3;
			this.zUpDown.Enabled = false;
			this.zUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.zUpDown.Location = new System.Drawing.Point(254, 220);
			this.zUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.zUpDown.Name = "zUpDown";
			this.zUpDown.Size = new System.Drawing.Size(75, 20);
			this.zUpDown.TabIndex = 29;
			this.zUpDown.ValueChanged += new System.EventHandler(this.zUpDown_ValueChanged);
			// 
			// yUpDown
			// 
			this.yUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.yUpDown.DecimalPlaces = 3;
			this.yUpDown.Enabled = false;
			this.yUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.yUpDown.Location = new System.Drawing.Point(254, 194);
			this.yUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.yUpDown.Name = "yUpDown";
			this.yUpDown.Size = new System.Drawing.Size(75, 20);
			this.yUpDown.TabIndex = 28;
			this.yUpDown.ValueChanged += new System.EventHandler(this.yUpDown_ValueChanged);
			// 
			// zLabel
			// 
			this.zLabel.AutoSize = true;
			this.zLabel.Location = new System.Drawing.Point(221, 220);
			this.zLabel.Name = "zLabel";
			this.zLabel.Size = new System.Drawing.Size(20, 19);
			this.zLabel.TabIndex = 25;
			this.zLabel.Text = "Z:";
			this.zLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// yLabel
			// 
			this.yLabel.AutoSize = true;
			this.yLabel.Location = new System.Drawing.Point(222, 194);
			this.yLabel.Name = "yLabel";
			this.yLabel.Size = new System.Drawing.Size(20, 19);
			this.yLabel.TabIndex = 24;
			this.yLabel.Text = "Y:";
			this.yLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// xUpDown
			// 
			this.xUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.xUpDown.DecimalPlaces = 3;
			this.xUpDown.Enabled = false;
			this.xUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.xUpDown.Location = new System.Drawing.Point(254, 168);
			this.xUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.xUpDown.Name = "xUpDown";
			this.xUpDown.Size = new System.Drawing.Size(75, 20);
			this.xUpDown.TabIndex = 31;
			this.xUpDown.ValueChanged += new System.EventHandler(this.xUpDown_ValueChanged);
			// 
			// xLabel
			// 
			this.xLabel.AutoSize = true;
			this.xLabel.Location = new System.Drawing.Point(222, 168);
			this.xLabel.Name = "xLabel";
			this.xLabel.Size = new System.Drawing.Size(20, 19);
			this.xLabel.TabIndex = 30;
			this.xLabel.Text = "X:";
			this.xLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// BehaviourEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(349, 280);
			this.Controls.Add(this.xUpDown);
			this.Controls.Add(this.xLabel);
			this.Controls.Add(this.zUpDown);
			this.Controls.Add(this.yUpDown);
			this.Controls.Add(this.zLabel);
			this.Controls.Add(this.yLabel);
			this.Controls.Add(this.flag2Checkbox);
			this.Controls.Add(this.flag1Checkbox);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.treeView1);
			this.Name = "BehaviourEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Text = "Behaviour Editor";
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroContextMenu1.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.yUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private MetroFramework.Controls.MetroCheckBox flag1Checkbox;
		private MetroFramework.Controls.MetroCheckBox flag2Checkbox;
		private System.Windows.Forms.NumericUpDown zUpDown;
		private System.Windows.Forms.NumericUpDown yUpDown;
		private MetroFramework.Controls.MetroLabel zLabel;
		private MetroFramework.Controls.MetroLabel yLabel;
		private System.Windows.Forms.NumericUpDown xUpDown;
		private MetroFramework.Controls.MetroLabel xLabel;
		private MetroFramework.Controls.MetroContextMenu metroContextMenu1;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addNewPositionOverrideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addNewEntryToolStripMenuItem;
	}
}