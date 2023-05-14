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
            this.TextureIcons = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importJavaAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importGifToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportJavaAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulkAnimationSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToInterpolation = new System.Windows.Forms.ToolStripMenuItem();
            this.editorControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBulkSpedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.javaAnimationSupportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InterpolationCheckbox = new MetroFramework.Controls.MetroCheckBox();
            this.AnimationStartStopBtn = new MetroFramework.Controls.MetroButton();
            this.tileLabel = new MetroFramework.Controls.MetroLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.animationPictureBox = new PckStudio.Forms.Editor.AnimationPictureBox();
            this.animationTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationPictureBox)).BeginInit();
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
            this.frameTreeView.ImageIndex = 0;
            this.frameTreeView.ImageList = this.TextureIcons;
            this.frameTreeView.Location = new System.Drawing.Point(20, 88);
            this.frameTreeView.Margin = new System.Windows.Forms.Padding(0);
            this.frameTreeView.Name = "frameTreeView";
            this.frameTreeView.SelectedImageIndex = 0;
            this.frameTreeView.ShowLines = false;
            this.frameTreeView.ShowRootLines = false;
            this.frameTreeView.Size = new System.Drawing.Size(134, 253);
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
            this.addFrameToolStripMenuItem.Name = "addFrameToolStripMenuItem";
            this.addFrameToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.addFrameToolStripMenuItem.Text = "Add Frame";
            this.addFrameToolStripMenuItem.Click += new System.EventHandler(this.addFrameToolStripMenuItem_Click);
            // 
            // removeFrameToolStripMenuItem
            // 
            this.removeFrameToolStripMenuItem.Name = "removeFrameToolStripMenuItem";
            this.removeFrameToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.removeFrameToolStripMenuItem.Text = "Remove Frame";
            this.removeFrameToolStripMenuItem.Click += new System.EventHandler(this.removeFrameToolStripMenuItem_Click);
            // 
            // TextureIcons
            // 
            this.TextureIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.TextureIcons.ImageSize = new System.Drawing.Size(32, 32);
            this.TextureIcons.TransparentColor = System.Drawing.Color.Transparent;
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
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(372, 24);
            this.menuStrip.TabIndex = 14;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1,
            this.toolStripSeparator1,
            this.importToolStripMenuItem,
            this.exportAsToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem1.Image")));
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animationTextureToolStripMenuItem,
            this.importJavaAnimationToolStripMenuItem,
            this.importGifToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // importJavaAnimationToolStripMenuItem
            // 
            this.importJavaAnimationToolStripMenuItem.Name = "importJavaAnimationToolStripMenuItem";
            this.importJavaAnimationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importJavaAnimationToolStripMenuItem.Text = "Java Animation";
            this.importJavaAnimationToolStripMenuItem.Click += new System.EventHandler(this.importJavaAnimationToolStripMenuItem_Click);
            // 
            // importGifToolStripMenuItem
            // 
            this.importGifToolStripMenuItem.Name = "importGifToolStripMenuItem";
            this.importGifToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importGifToolStripMenuItem.Text = "Gif";
            this.importGifToolStripMenuItem.Click += new System.EventHandler(this.importGifToolStripMenuItem_Click);
            // 
            // exportAsToolStripMenuItem
            // 
            this.exportAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportJavaAnimationToolStripMenuItem});
            this.exportAsToolStripMenuItem.Name = "exportAsToolStripMenuItem";
            this.exportAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportAsToolStripMenuItem.Text = "Export as";
            // 
            // exportJavaAnimationToolStripMenuItem
            // 
            this.exportJavaAnimationToolStripMenuItem.Name = "exportJavaAnimationToolStripMenuItem";
            this.exportJavaAnimationToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportJavaAnimationToolStripMenuItem.Text = "Java Animation";
            this.exportJavaAnimationToolStripMenuItem.Click += new System.EventHandler(this.exportJavaAnimationToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bulkAnimationSpeedToolStripMenuItem,
            this.changeTileToolStripMenuItem});
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.editToolStripMenuItem.Text = "Tools";
            // 
            // bulkAnimationSpeedToolStripMenuItem
            // 
            this.bulkAnimationSpeedToolStripMenuItem.Name = "bulkAnimationSpeedToolStripMenuItem";
            this.bulkAnimationSpeedToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.bulkAnimationSpeedToolStripMenuItem.Text = "Set Bulk Animation Speed";
            this.bulkAnimationSpeedToolStripMenuItem.Click += new System.EventHandler(this.bulkAnimationSpeedToolStripMenuItem_Click);
            // 
            // changeTileToolStripMenuItem
            // 
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
            // javaAnimationSupportToolStripMenuItem
            // 
            this.javaAnimationSupportToolStripMenuItem.Name = "javaAnimationSupportToolStripMenuItem";
            this.javaAnimationSupportToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.javaAnimationSupportToolStripMenuItem.Text = "Java Animation Support";
            this.javaAnimationSupportToolStripMenuItem.Click += new System.EventHandler(this.javaAnimationSupportToolStripMenuItem_Click);
            // 
            // InterpolationCheckbox
            // 
            this.InterpolationCheckbox.AutoSize = true;
            this.InterpolationCheckbox.Location = new System.Drawing.Point(161, 63);
            this.InterpolationCheckbox.Name = "InterpolationCheckbox";
            this.InterpolationCheckbox.Size = new System.Drawing.Size(129, 15);
            this.InterpolationCheckbox.TabIndex = 17;
            this.InterpolationCheckbox.Text = "Enable Interpolation";
            this.InterpolationCheckbox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.InterpolationCheckbox.UseSelectable = true;
            this.InterpolationCheckbox.CheckedChanged += new System.EventHandler(this.InterpolationCheckbox_CheckedChanged);
            // 
            // AnimationStartStopBtn
            // 
            this.AnimationStartStopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AnimationStartStopBtn.Location = new System.Drawing.Point(157, 317);
            this.AnimationStartStopBtn.Name = "AnimationStartStopBtn";
            this.AnimationStartStopBtn.Size = new System.Drawing.Size(232, 24);
            this.AnimationStartStopBtn.TabIndex = 18;
            this.AnimationStartStopBtn.Text = "Play Animation";
            this.AnimationStartStopBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.AnimationStartStopBtn.UseSelectable = true;
            this.AnimationStartStopBtn.Click += new System.EventHandler(this.AnimationStartStopBtn_Click);
            // 
            // tileLabel
            // 
            this.tileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tileLabel.AutoSize = true;
            this.tileLabel.Location = new System.Drawing.Point(20, 341);
            this.tileLabel.MinimumSize = new System.Drawing.Size(170, 19);
            this.tileLabel.Name = "tileLabel";
            this.tileLabel.Size = new System.Drawing.Size(57, 19);
            this.tileLabel.TabIndex = 20;
            this.tileLabel.Text = "tileLabel";
            this.tileLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(154, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(238, 24);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // animationPictureBox
            // 
            this.animationPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationPictureBox.Location = new System.Drawing.Point(157, 88);
            this.animationPictureBox.Name = "animationPictureBox";
            this.animationPictureBox.Size = new System.Drawing.Size(235, 223);
            this.animationPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.animationPictureBox.TabIndex = 16;
            this.animationPictureBox.TabStop = false;
            // 
            // animationTextureToolStripMenuItem
            // 
            this.animationTextureToolStripMenuItem.Name = "animationTextureToolStripMenuItem";
            this.animationTextureToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.animationTextureToolStripMenuItem.Text = "Animation Texture";
            this.animationTextureToolStripMenuItem.Click += new System.EventHandler(this.animationTextureToolStripMenuItem_Click);
            // 
            // AnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 362);
            this.Controls.Add(this.InterpolationCheckbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.AnimationStartStopBtn);
            this.Controls.Add(this.tileLabel);
            this.Controls.Add(this.animationPictureBox);
            this.Controls.Add(this.frameTreeView);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(412, 362);
            this.Name = "AnimationEditor";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Animation Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AnimationEditor_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView frameTreeView;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private PckStudio.Forms.Editor.AnimationPictureBox animationPictureBox;
		private MetroFramework.Controls.MetroCheckBox InterpolationCheckbox;
		private MetroFramework.Controls.MetroButton AnimationStartStopBtn;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addFrameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeFrameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem bulkAnimationSpeedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeTileToolStripMenuItem;
		private MetroFramework.Controls.MetroLabel tileLabel;
		private System.Windows.Forms.ToolStripMenuItem howToInterpolation;
		private System.Windows.Forms.ToolStripMenuItem editorControlsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setBulkSpedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem javaAnimationSupportToolStripMenuItem;
		private System.Windows.Forms.ImageList TextureIcons;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importJavaAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importGifToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportJavaAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem animationTextureToolStripMenuItem;
    }
}