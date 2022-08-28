namespace PckStudio.Forms.Editor
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
			this.frameTreeView = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bulkAnimationSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importJavaAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportJavaAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.howToInterpolation = new System.Windows.Forms.ToolStripMenuItem();
			this.editorControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setBulkSpedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.InterpolationCheckbox = new MetroFramework.Controls.MetroCheckBox();
			this.AnimationPlayBtn = new MetroFramework.Controls.MetroButton();
			this.AnimationStopBtn = new MetroFramework.Controls.MetroButton();
			this.tileLabel = new MetroFramework.Controls.MetroLabel();
			this.MipMapCheckbox = new MetroFramework.Controls.MetroCheckBox();
			this.MipMapLabel = new MetroFramework.Controls.MetroLabel();
			this.MipMapNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.pictureBoxWithInterpolationMode1 = new PckStudio.PictureBoxWithInterpolationMode();
			this.javaAnimationSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MipMapNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).BeginInit();
			this.SuspendLayout();
			// 
			// frameTreeView
			// 
			this.frameTreeView.AllowDrop = true;
			this.frameTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.frameTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.frameTreeView.ContextMenuStrip = this.contextMenuStrip1;
			this.frameTreeView.ForeColor = System.Drawing.Color.White;
			this.frameTreeView.Location = new System.Drawing.Point(20, 88);
			this.frameTreeView.Margin = new System.Windows.Forms.Padding(0);
			this.frameTreeView.Name = "frameTreeView";
			this.frameTreeView.Size = new System.Drawing.Size(165, 198);
			this.frameTreeView.TabIndex = 15;
			this.frameTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.frameTreeView_ItemDrag);
			this.frameTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.frameTreeView_AfterSelect);
			this.frameTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_doubleClick);
			this.frameTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.frameTreeView_DragDrop);
			this.frameTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.frameTreeView_DragEnter);
			this.frameTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.frameTreeView_DragOver);
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
			this.menuStrip.Size = new System.Drawing.Size(372, 24);
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
            this.exportJavaAnimationToolStripMenuItem,
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
			this.importJavaAnimationToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
			this.importJavaAnimationToolStripMenuItem.Name = "importJavaAnimationToolStripMenuItem";
			this.importJavaAnimationToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.importJavaAnimationToolStripMenuItem.Text = "Import Java Animation";
			this.importJavaAnimationToolStripMenuItem.Click += new System.EventHandler(this.importJavaAnimationToolStripMenuItem_Click);
			// 
			// exportJavaAnimationToolStripMenuItem
			// 
			this.exportJavaAnimationToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ExportFile;
			this.exportJavaAnimationToolStripMenuItem.Name = "exportJavaAnimationToolStripMenuItem";
			this.exportJavaAnimationToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.exportJavaAnimationToolStripMenuItem.Text = "Export Java Animation";
			this.exportJavaAnimationToolStripMenuItem.Click += new System.EventHandler(this.exportJavaAnimationToolStripMenuItem_Click);
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
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToInterpolation,
            this.editorControlsToolStripMenuItem,
            this.setBulkSpedToolStripMenuItem,
            this.javaAnimationSupportToolStripMenuItem});
			this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// howToInterpolation
			// 
			this.howToInterpolation.Name = "howToInterpolation";
			this.howToInterpolation.Size = new System.Drawing.Size(200, 22);
			this.howToInterpolation.Text = "Interpolation";
			this.howToInterpolation.Click += new System.EventHandler(this.howToInterpolation_Click);
			// 
			// editorControlsToolStripMenuItem
			// 
			this.editorControlsToolStripMenuItem.Name = "editorControlsToolStripMenuItem";
			this.editorControlsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.editorControlsToolStripMenuItem.Text = "Editor Controls";
			this.editorControlsToolStripMenuItem.Click += new System.EventHandler(this.editorControlsToolStripMenuItem_Click);
			// 
			// setBulkSpedToolStripMenuItem
			// 
			this.setBulkSpedToolStripMenuItem.Name = "setBulkSpedToolStripMenuItem";
			this.setBulkSpedToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.setBulkSpedToolStripMenuItem.Text = "Set Bulk Speed";
			this.setBulkSpedToolStripMenuItem.Click += new System.EventHandler(this.setBulkSpeedToolStripMenuItem_Click);
			// 
			// InterpolationCheckbox
			// 
			this.InterpolationCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.InterpolationCheckbox.AutoSize = true;
			this.InterpolationCheckbox.Location = new System.Drawing.Point(188, 317);
			this.InterpolationCheckbox.Name = "InterpolationCheckbox";
			this.InterpolationCheckbox.Size = new System.Drawing.Size(204, 15);
			this.InterpolationCheckbox.TabIndex = 17;
			this.InterpolationCheckbox.Text = "Interpolates (not simulated above)";
			this.InterpolationCheckbox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.InterpolationCheckbox.UseSelectable = true;
			// 
			// AnimationPlayBtn
			// 
			this.AnimationPlayBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AnimationPlayBtn.Location = new System.Drawing.Point(188, 291);
			this.AnimationPlayBtn.Name = "AnimationPlayBtn";
			this.AnimationPlayBtn.Size = new System.Drawing.Size(99, 24);
			this.AnimationPlayBtn.TabIndex = 18;
			this.AnimationPlayBtn.Text = "Play Animation";
			this.AnimationPlayBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.AnimationPlayBtn.UseSelectable = true;
			this.AnimationPlayBtn.Click += new System.EventHandler(this.StartAnimationBtn_Click);
			// 
			// AnimationStopBtn
			// 
			this.AnimationStopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AnimationStopBtn.Enabled = false;
			this.AnimationStopBtn.Location = new System.Drawing.Point(293, 291);
			this.AnimationStopBtn.Name = "AnimationStopBtn";
			this.AnimationStopBtn.Size = new System.Drawing.Size(99, 24);
			this.AnimationStopBtn.TabIndex = 19;
			this.AnimationStopBtn.Text = "Stop Animation";
			this.AnimationStopBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.AnimationStopBtn.UseSelectable = true;
			this.AnimationStopBtn.Click += new System.EventHandler(this.StopAnimationBtn_Click);
			// 
			// tileLabel
			// 
			this.tileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tileLabel.AutoSize = true;
			this.tileLabel.Location = new System.Drawing.Point(20, 290);
			this.tileLabel.MinimumSize = new System.Drawing.Size(170, 19);
			this.tileLabel.Name = "tileLabel";
			this.tileLabel.Size = new System.Drawing.Size(57, 19);
			this.tileLabel.TabIndex = 20;
			this.tileLabel.Text = "tileLabel";
			this.tileLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// MipMapCheckbox
			// 
			this.MipMapCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.MipMapCheckbox.AutoSize = true;
			this.MipMapCheckbox.Location = new System.Drawing.Point(20, 312);
			this.MipMapCheckbox.Name = "MipMapCheckbox";
			this.MipMapCheckbox.Size = new System.Drawing.Size(128, 15);
			this.MipMapCheckbox.TabIndex = 21;
			this.MipMapCheckbox.Text = "Is Mip Map Texture?";
			this.MipMapCheckbox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.MipMapCheckbox.UseSelectable = true;
			this.MipMapCheckbox.CheckedChanged += new System.EventHandler(this.MipMapCheckBox_CheckedChanged);
			// 
			// MipMapLabel
			// 
			this.MipMapLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.MipMapLabel.AutoSize = true;
			this.MipMapLabel.Location = new System.Drawing.Point(21, 330);
			this.MipMapLabel.Name = "MipMapLabel";
			this.MipMapLabel.Size = new System.Drawing.Size(99, 19);
			this.MipMapLabel.TabIndex = 22;
			this.MipMapLabel.Text = "Mip Map Level:";
			this.MipMapLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.MipMapLabel.Visible = false;
			// 
			// MipMapNumericUpDown
			// 
			this.MipMapNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.MipMapNumericUpDown.BackColor = System.Drawing.Color.Black;
			this.MipMapNumericUpDown.ForeColor = System.Drawing.SystemColors.Window;
			this.MipMapNumericUpDown.Location = new System.Drawing.Point(127, 330);
			this.MipMapNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.MipMapNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.MipMapNumericUpDown.Name = "MipMapNumericUpDown";
			this.MipMapNumericUpDown.Size = new System.Drawing.Size(44, 20);
			this.MipMapNumericUpDown.TabIndex = 23;
			this.MipMapNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.MipMapNumericUpDown.Visible = false;
			// 
			// pictureBoxWithInterpolationMode1
			// 
			this.pictureBoxWithInterpolationMode1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxWithInterpolationMode1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			this.pictureBoxWithInterpolationMode1.Location = new System.Drawing.Point(188, 88);
			this.pictureBoxWithInterpolationMode1.Name = "pictureBoxWithInterpolationMode1";
			this.pictureBoxWithInterpolationMode1.Size = new System.Drawing.Size(204, 198);
			this.pictureBoxWithInterpolationMode1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxWithInterpolationMode1.TabIndex = 16;
			this.pictureBoxWithInterpolationMode1.TabStop = false;
			// 
			// javaAnimationSupportToolStripMenuItem
			// 
			this.javaAnimationSupportToolStripMenuItem.Name = "javaAnimationSupportToolStripMenuItem";
			this.javaAnimationSupportToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.javaAnimationSupportToolStripMenuItem.Text = "Java Animation Support";
			this.javaAnimationSupportToolStripMenuItem.Click += new System.EventHandler(this.javaAnimationSupportToolStripMenuItem_Click);
			// 
			// AnimationEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(412, 362);
			this.Controls.Add(this.AnimationStopBtn);
			this.Controls.Add(this.AnimationPlayBtn);
			this.Controls.Add(this.MipMapNumericUpDown);
			this.Controls.Add(this.MipMapLabel);
			this.Controls.Add(this.MipMapCheckbox);
			this.Controls.Add(this.tileLabel);
			this.Controls.Add(this.InterpolationCheckbox);
			this.Controls.Add(this.pictureBoxWithInterpolationMode1);
			this.Controls.Add(this.frameTreeView);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(412, 362);
			this.Name = "AnimationEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Text = "Animation Editor";
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.contextMenuStrip1.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MipMapNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView frameTreeView;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private PictureBoxWithInterpolationMode pictureBoxWithInterpolationMode1;
		private MetroFramework.Controls.MetroCheckBox InterpolationCheckbox;
		private MetroFramework.Controls.MetroButton AnimationPlayBtn;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addFrameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeFrameToolStripMenuItem;
		private MetroFramework.Controls.MetroButton AnimationStopBtn;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bulkAnimationSpeedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importJavaAnimationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeTileToolStripMenuItem;
		private MetroFramework.Controls.MetroLabel tileLabel;
		private MetroFramework.Controls.MetroCheckBox MipMapCheckbox;
		private MetroFramework.Controls.MetroLabel MipMapLabel;
		private System.Windows.Forms.NumericUpDown MipMapNumericUpDown;
		private System.Windows.Forms.ToolStripMenuItem exportJavaAnimationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem howToInterpolation;
		private System.Windows.Forms.ToolStripMenuItem editorControlsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setBulkSpedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem javaAnimationSupportToolStripMenuItem;
	}
}