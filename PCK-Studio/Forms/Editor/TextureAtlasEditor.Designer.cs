namespace PckStudio.Forms.Editor
{
    partial class TextureAtlasEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureAtlasEditor));
            this.internalTileNameLabel = new MetroFramework.Controls.MetroLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyColorMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.originalPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            this.selectTilePictureBox = new PckStudio.ToolboxItems.AnimationPictureBox();
            this.replaceButton = new MetroFramework.Controls.MetroButton();
            this.tileNameLabel = new MetroFramework.Controls.MetroLabel();
            this.setColorButton = new MetroFramework.Controls.MetroButton();
            this.animationButton = new MetroFramework.Controls.MetroButton();
            this.clearColorButton = new MetroFramework.Controls.MetroButton();
            this.extractButton = new MetroFramework.Controls.MetroButton();
            this.colorSlider = new MetroFramework.Controls.MetroTrackBar();
            this.colorSliderLabel = new MetroFramework.Controls.MetroLabel();
            this.variantComboBox = new MetroFramework.Controls.MetroComboBox();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectTilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // internalTileNameLabel
            // 
            this.internalTileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.internalTileNameLabel, 2);
            this.internalTileNameLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.internalTileNameLabel.Location = new System.Drawing.Point(3, 281);
            this.internalTileNameLabel.Name = "internalTileNameLabel";
            this.internalTileNameLabel.Size = new System.Drawing.Size(208, 15);
            this.internalTileNameLabel.TabIndex = 18;
            this.internalTileNameLabel.Text = "InternalTileName";
            this.internalTileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.internalTileNameLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.internalTileNameLabel.WrapToLine = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(20, 60);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(590, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Menu;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyColorMaskToolStripMenuItem,
            this.playAnimationsToolStripMenuItem,
            this.allowGroupsToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // applyColorMaskToolStripMenuItem
            // 
            this.applyColorMaskToolStripMenuItem.Checked = true;
            this.applyColorMaskToolStripMenuItem.CheckOnClick = true;
            this.applyColorMaskToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.applyColorMaskToolStripMenuItem.Name = "applyColorMaskToolStripMenuItem";
            this.applyColorMaskToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.applyColorMaskToolStripMenuItem.Text = "Apply Color Mask";
            this.applyColorMaskToolStripMenuItem.CheckedChanged += new System.EventHandler(this.applyColorMaskToolStripMenuItem_CheckedChanged);
            // 
            // playAnimationsToolStripMenuItem
            // 
            this.playAnimationsToolStripMenuItem.Checked = true;
            this.playAnimationsToolStripMenuItem.CheckOnClick = true;
            this.playAnimationsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.playAnimationsToolStripMenuItem.Name = "playAnimationsToolStripMenuItem";
            this.playAnimationsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.playAnimationsToolStripMenuItem.Text = "Play Animations";
            this.playAnimationsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.playAnimationsToolStripMenuItem_CheckedChanged);
            // 
            // allowGroupsToolStripMenuItem
            // 
            this.allowGroupsToolStripMenuItem.Checked = true;
            this.allowGroupsToolStripMenuItem.CheckOnClick = true;
            this.allowGroupsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allowGroupsToolStripMenuItem.Name = "allowGroupsToolStripMenuItem";
            this.allowGroupsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.allowGroupsToolStripMenuItem.Text = "Allow Groups";
            this.allowGroupsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.allowGroupsToolStripMenuItem_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.43842F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.09603F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.46555F));
            this.tableLayoutPanel1.Controls.Add(this.originalPictureBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.selectTilePictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.replaceButton, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.tileNameLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.setColorButton, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.animationButton, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.clearColorButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.extractButton, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.colorSlider, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.colorSliderLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.variantComboBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.internalTileNameLabel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.82212F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.938088F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.943623F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.943623F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.943623F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.352765F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.350623F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.352765F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.352765F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(590, 565);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // originalPictureBox
            // 
            this.originalPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.originalPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.originalPictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.originalPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.originalPictureBox.Location = new System.Drawing.Point(217, 3);
            this.originalPictureBox.Name = "originalPictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.originalPictureBox, 9);
            this.originalPictureBox.Size = new System.Drawing.Size(370, 535);
            this.originalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.originalPictureBox.TabIndex = 4;
            this.originalPictureBox.TabStop = false;
            this.originalPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.originalPictureBox_MouseClick);
            // 
            // selectTilePictureBox
            // 
            this.selectTilePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectTilePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.selectTilePictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.selectTilePictureBox.BlendColor = System.Drawing.Color.White;
            this.selectTilePictureBox.BlendMode = PckStudio.Core.Extensions.BlendMode.Multiply;
            this.tableLayoutPanel1.SetColumnSpan(this.selectTilePictureBox, 2);
            this.selectTilePictureBox.Image = null;
            this.selectTilePictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.selectTilePictureBox.Location = new System.Drawing.Point(3, 3);
            this.selectTilePictureBox.Name = "selectTilePictureBox";
            this.selectTilePictureBox.Size = new System.Drawing.Size(208, 227);
            this.selectTilePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.selectTilePictureBox.TabIndex = 0;
            this.selectTilePictureBox.TabStop = false;
            this.selectTilePictureBox.UseBlendColor = true;
            // 
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.replaceButton, 2);
            this.replaceButton.Location = new System.Drawing.Point(3, 515);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(208, 23);
            this.replaceButton.TabIndex = 14;
            this.replaceButton.Text = "Replace Tile on Atlas";
            this.replaceButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.replaceButton.UseSelectable = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // tileNameLabel
            // 
            this.tileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.tileNameLabel, 2);
            this.tileNameLabel.Location = new System.Drawing.Point(3, 233);
            this.tileNameLabel.Name = "tileNameLabel";
            this.tileNameLabel.Size = new System.Drawing.Size(208, 48);
            this.tileNameLabel.TabIndex = 19;
            this.tileNameLabel.Text = "TileName";
            this.tileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tileNameLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tileNameLabel.WrapToLine = true;
            // 
            // setColorButton
            // 
            this.setColorButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.setColorButton, 2);
            this.setColorButton.Location = new System.Drawing.Point(3, 434);
            this.setColorButton.Name = "setColorButton";
            this.setColorButton.Size = new System.Drawing.Size(208, 17);
            this.setColorButton.TabIndex = 25;
            this.setColorButton.Text = "Set Custom Colour";
            this.setColorButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.setColorButton.UseSelectable = true;
            this.setColorButton.Click += new System.EventHandler(this.setColorButton_Click);
            // 
            // animationButton
            // 
            this.animationButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.animationButton, 2);
            this.animationButton.Location = new System.Drawing.Point(3, 544);
            this.animationButton.Name = "animationButton";
            this.animationButton.Size = new System.Drawing.Size(208, 18);
            this.animationButton.TabIndex = 16;
            this.animationButton.Text = "Animation";
            this.animationButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.animationButton.UseSelectable = true;
            this.animationButton.Click += new System.EventHandler(this.animationButton_Click);
            // 
            // clearColorButton
            // 
            this.clearColorButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clearColorButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.clearColorButton, 2);
            this.clearColorButton.Location = new System.Drawing.Point(3, 457);
            this.clearColorButton.Name = "clearColorButton";
            this.clearColorButton.Size = new System.Drawing.Size(208, 23);
            this.clearColorButton.TabIndex = 24;
            this.clearColorButton.Text = "Clear Custom Colour";
            this.clearColorButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.clearColorButton.UseSelectable = true;
            this.clearColorButton.Click += new System.EventHandler(this.clearColorButton_Click);
            // 
            // extractButton
            // 
            this.extractButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.extractButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.extractButton, 2);
            this.extractButton.Location = new System.Drawing.Point(3, 486);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(208, 23);
            this.extractButton.TabIndex = 27;
            this.extractButton.Text = "Extract Tile from Atlas";
            this.extractButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.extractButton.UseSelectable = true;
            this.extractButton.Click += new System.EventHandler(this.extractTileToolStripMenuItem_Click);
            // 
            // colorSlider
            // 
            this.colorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.colorSlider.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider.Location = new System.Drawing.Point(64, 380);
            this.colorSlider.Maximum = 255;
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.Size = new System.Drawing.Size(147, 42);
            this.colorSlider.TabIndex = 26;
            this.colorSlider.Text = "metroTrackBar1";
            this.colorSlider.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.colorSlider.Value = 255;
            this.colorSlider.Visible = false;
            this.colorSlider.ValueChanged += new System.EventHandler(this.colorSlider_ValueChanged);
            // 
            // colorSliderLabel
            // 
            this.colorSliderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorSliderLabel.BackColor = System.Drawing.Color.Transparent;
            this.colorSliderLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.colorSliderLabel.Location = new System.Drawing.Point(3, 377);
            this.colorSliderLabel.Name = "colorSliderLabel";
            this.colorSliderLabel.Size = new System.Drawing.Size(55, 48);
            this.colorSliderLabel.TabIndex = 19;
            this.colorSliderLabel.Text = "Color Value:";
            this.colorSliderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.colorSliderLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.colorSliderLabel.Visible = false;
            this.colorSliderLabel.WrapToLine = true;
            // 
            // variantComboBox
            // 
            this.variantComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.variantComboBox, 2);
            this.variantComboBox.Enabled = false;
            this.variantComboBox.FormattingEnabled = true;
            this.variantComboBox.ItemHeight = 23;
            this.variantComboBox.Location = new System.Drawing.Point(3, 338);
            this.variantComboBox.Name = "variantComboBox";
            this.variantComboBox.Size = new System.Drawing.Size(208, 29);
            this.variantComboBox.TabIndex = 17;
            this.variantComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.variantComboBox.UseSelectable = true;
            this.variantComboBox.SelectedIndexChanged += new System.EventHandler(this.variantComboBox_SelectedIndexChanged);
            // 
            // TextureAtlasEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 669);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(630, 669);
            this.Name = "TextureAtlasEditor";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.None;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Texture Atlas Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextureAtlasEditor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectTilePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PckStudio.ToolboxItems.AnimationPictureBox selectTilePictureBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroFramework.Controls.MetroButton replaceButton;
        private PckStudio.ToolboxItems.InterpolationPictureBox originalPictureBox;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private MetroFramework.Controls.MetroButton animationButton;
        private MetroFramework.Controls.MetroComboBox variantComboBox;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyColorMaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playAnimationsToolStripMenuItem;
        private MetroFramework.Controls.MetroLabel tileNameLabel;
        private MetroFramework.Controls.MetroLabel internalTileNameLabel;
        private MetroFramework.Controls.MetroButton clearColorButton;
        private MetroFramework.Controls.MetroButton setColorButton;
        private MetroFramework.Controls.MetroTrackBar colorSlider;
        private MetroFramework.Controls.MetroLabel colorSliderLabel;
        private MetroFramework.Controls.MetroButton extractButton;
        private System.Windows.Forms.ToolStripMenuItem allowGroupsToolStripMenuItem;
    }
}