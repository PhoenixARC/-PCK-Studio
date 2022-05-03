namespace PckStudio
{
	partial class AnimationEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationEditor));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bulkAnimationSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importJavaAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.metroCheckBox1 = new MetroFramework.Controls.MetroCheckBox();
			this.metroButton1 = new MetroFramework.Controls.MetroButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.metroButton2 = new MetroFramework.Controls.MetroButton();
			this.tileLabel = new MetroFramework.Controls.MetroLabel();
			this.metroCheckBox2 = new MetroFramework.Controls.MetroCheckBox();
			this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.pictureBoxWithInterpolationMode1 = new PckStudio.PictureBoxWithInterpolationMode();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).BeginInit();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
			this.treeView1.ForeColor = System.Drawing.Color.White;
			this.treeView1.Location = new System.Drawing.Point(20, 84);
			this.treeView1.Margin = new System.Windows.Forms.Padding(0);
			this.treeView1.MaximumSize = new System.Drawing.Size(205, 350);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(165, 196);
			this.treeView1.TabIndex = 15;
			this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_doubleClick);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFrameToolStripMenuItem,
            this.removeFrameToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(154, 48);
			// 
			// addFrameToolStripMenuItem
			// 
			this.addFrameToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ExportFile;
			this.addFrameToolStripMenuItem.Name = "addFrameToolStripMenuItem";
			this.addFrameToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.addFrameToolStripMenuItem.Text = "Add Frame";
			this.addFrameToolStripMenuItem.Click += new System.EventHandler(this.addFrameToolStripMenuItem_Click);
			// 
			// removeFrameToolStripMenuItem
			// 
			this.removeFrameToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Del;
			this.removeFrameToolStripMenuItem.Name = "removeFrameToolStripMenuItem";
			this.removeFrameToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.removeFrameToolStripMenuItem.Text = "Remove Frame";
			this.removeFrameToolStripMenuItem.Click += new System.EventHandler(this.removeFrameToolStripMenuItem_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.AutoSize = false;
			this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(20, 60);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(360, 24);
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
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulkAnimationSpeedToolStripMenuItem,
            this.importJavaAnimationToolStripMenuItem,
            this.changeTileToolStripMenuItem});
			this.editToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.editToolStripMenuItem.Text = "Tools";
			// 
			// bulkAnimationSpeedToolStripMenuItem
			// 
			this.bulkAnimationSpeedToolStripMenuItem.Image = global::PckStudio.Properties.Resources.clock;
			this.bulkAnimationSpeedToolStripMenuItem.Name = "bulkAnimationSpeedToolStripMenuItem";
			this.bulkAnimationSpeedToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.bulkAnimationSpeedToolStripMenuItem.Text = "Set Bulk Animation Speed";
			this.bulkAnimationSpeedToolStripMenuItem.Click += new System.EventHandler(this.bulkAnimationSpeedToolStripMenuItem_Click);
			// 
			// importJavaAnimationToolStripMenuItem
			// 
			this.importJavaAnimationToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ExportFile;
			this.importJavaAnimationToolStripMenuItem.Name = "importJavaAnimationToolStripMenuItem";
			this.importJavaAnimationToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.importJavaAnimationToolStripMenuItem.Text = "Import Java Animation";
			this.importJavaAnimationToolStripMenuItem.Click += new System.EventHandler(this.importJavaAnimationToolStripMenuItem_Click);
			// 
			// changeTileToolStripMenuItem
			// 
			this.changeTileToolStripMenuItem.Image = global::PckStudio.Properties.Resources.changeTile;
			this.changeTileToolStripMenuItem.Name = "changeTileToolStripMenuItem";
			this.changeTileToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.changeTileToolStripMenuItem.Text = "Change Tile";
			this.changeTileToolStripMenuItem.Click += new System.EventHandler(this.changeTileToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
			// 
			// metroCheckBox1
			// 
			this.metroCheckBox1.AutoSize = true;
			this.metroCheckBox1.Location = new System.Drawing.Point(182, 312);
			this.metroCheckBox1.Name = "metroCheckBox1";
			this.metroCheckBox1.Size = new System.Drawing.Size(204, 15);
			this.metroCheckBox1.TabIndex = 17;
			this.metroCheckBox1.Text = "Interpolates (not simulated above)";
			this.metroCheckBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroCheckBox1.UseSelectable = true;
			// 
			// metroButton1
			// 
			this.metroButton1.Location = new System.Drawing.Point(182, 286);
			this.metroButton1.Name = "metroButton1";
			this.metroButton1.Size = new System.Drawing.Size(99, 24);
			this.metroButton1.TabIndex = 18;
			this.metroButton1.Text = "Play Animation";
			this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroButton1.UseSelectable = true;
			this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 1;
			this.timer1.Tick += new System.EventHandler(this.animate);
			// 
			// metroButton2
			// 
			this.metroButton2.Enabled = false;
			this.metroButton2.Location = new System.Drawing.Point(287, 286);
			this.metroButton2.Name = "metroButton2";
			this.metroButton2.Size = new System.Drawing.Size(99, 24);
			this.metroButton2.TabIndex = 19;
			this.metroButton2.Text = "Stop Animation";
			this.metroButton2.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroButton2.UseSelectable = true;
			this.metroButton2.Click += new System.EventHandler(this.metroButton2_Click);
			// 
			// tileLabel
			// 
			this.tileLabel.AutoSize = true;
			this.tileLabel.Location = new System.Drawing.Point(6, 286);
			this.tileLabel.MinimumSize = new System.Drawing.Size(170, 19);
			this.tileLabel.Name = "tileLabel";
			this.tileLabel.Size = new System.Drawing.Size(57, 19);
			this.tileLabel.TabIndex = 20;
			this.tileLabel.Text = "tileLabel";
			this.tileLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroCheckBox2
			// 
			this.metroCheckBox2.AutoSize = true;
			this.metroCheckBox2.Location = new System.Drawing.Point(6, 308);
			this.metroCheckBox2.Name = "metroCheckBox2";
			this.metroCheckBox2.Size = new System.Drawing.Size(128, 15);
			this.metroCheckBox2.TabIndex = 21;
			this.metroCheckBox2.Text = "Is Mip Map Texture?";
			this.metroCheckBox2.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroCheckBox2.UseSelectable = true;
			this.metroCheckBox2.CheckedChanged += new System.EventHandler(this.metroCheckBox2_CheckedChanged);
			// 
			// metroLabel1
			// 
			this.metroLabel1.AutoSize = true;
			this.metroLabel1.Location = new System.Drawing.Point(7, 326);
			this.metroLabel1.Name = "metroLabel1";
			this.metroLabel1.Size = new System.Drawing.Size(99, 19);
			this.metroLabel1.TabIndex = 22;
			this.metroLabel1.Text = "Mip Map Level:";
			this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroLabel1.Visible = false;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.BackColor = System.Drawing.Color.Black;
			this.numericUpDown1.ForeColor = System.Drawing.SystemColors.Window;
			this.numericUpDown1.Location = new System.Drawing.Point(113, 326);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.numericUpDown1.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(44, 20);
			this.numericUpDown1.TabIndex = 23;
			this.numericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.numericUpDown1.Visible = false;
			// 
			// pictureBoxWithInterpolationMode1
			// 
			this.pictureBoxWithInterpolationMode1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.pictureBoxWithInterpolationMode1.Location = new System.Drawing.Point(188, 88);
			this.pictureBoxWithInterpolationMode1.Name = "pictureBoxWithInterpolationMode1";
			this.pictureBoxWithInterpolationMode1.Size = new System.Drawing.Size(192, 192);
			this.pictureBoxWithInterpolationMode1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxWithInterpolationMode1.TabIndex = 16;
			this.pictureBoxWithInterpolationMode1.TabStop = false;
			// 
			// AnimationEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 356);
			this.Controls.Add(this.metroButton2);
			this.Controls.Add(this.metroButton1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.metroLabel1);
			this.Controls.Add(this.metroCheckBox2);
			this.Controls.Add(this.tileLabel);
			this.Controls.Add(this.metroCheckBox1);
			this.Controls.Add(this.pictureBoxWithInterpolationMode1);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.menuStrip);
			this.Name = "AnimationEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Text = "Animation Editor";
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private PictureBoxWithInterpolationMode pictureBoxWithInterpolationMode1;
		private MetroFramework.Controls.MetroCheckBox metroCheckBox1;
		private MetroFramework.Controls.MetroButton metroButton1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addFrameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeFrameToolStripMenuItem;
		private System.Windows.Forms.Timer timer1;
		private MetroFramework.Controls.MetroButton metroButton2;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bulkAnimationSpeedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importJavaAnimationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeTileToolStripMenuItem;
		private MetroFramework.Controls.MetroLabel tileLabel;
		private MetroFramework.Controls.MetroCheckBox metroCheckBox2;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}