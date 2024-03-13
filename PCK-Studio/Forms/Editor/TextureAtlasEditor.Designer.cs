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
            this.variantLabel = new MetroFramework.Controls.MetroLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyColorMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.originalPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            this.selectTilePictureBox = new PckStudio.ToolboxItems.AnimationPictureBox();
            this.replaceButton = new MetroFramework.Controls.MetroButton();
            this.tileNameLabel = new MetroFramework.Controls.MetroLabel();
            this.variantComboBox = new MetroFramework.Controls.MetroComboBox();
            this.setColorButton = new MetroFramework.Controls.MetroButton();
            this.animationButton = new MetroFramework.Controls.MetroButton();
            this.clearColorButton = new MetroFramework.Controls.MetroButton();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectTilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // variantLabel
            // 
            this.variantLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variantLabel.AutoSize = true;
            this.variantLabel.Location = new System.Drawing.Point(3, 232);
            this.variantLabel.Name = "variantLabel";
            this.variantLabel.Size = new System.Drawing.Size(55, 25);
            this.variantLabel.TabIndex = 18;
            this.variantLabel.Text = "Variant:";
            this.variantLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.variantLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
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
            this.saveToolStripMenuItem,
            this.extractTileToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Menu;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // extractTileToolStripMenuItem
            // 
            this.extractTileToolStripMenuItem.Name = "extractTileToolStripMenuItem";
            this.extractTileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractTileToolStripMenuItem.Text = "Extract Tile";
            this.extractTileToolStripMenuItem.Click += new System.EventHandler(this.extractTileToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyColorMaskToolStripMenuItem,
            this.playAnimationsToolStripMenuItem});
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
            this.tableLayoutPanel1.Controls.Add(this.replaceButton, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tileNameLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.variantComboBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.variantLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.setColorButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.animationButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.clearColorButton, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.09524F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.09524F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(590, 565);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // originalPictureBox
            // 
            this.originalPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.originalPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originalPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.originalPictureBox.Location = new System.Drawing.Point(217, 3);
            this.originalPictureBox.Name = "originalPictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.originalPictureBox, 7);
            this.originalPictureBox.Size = new System.Drawing.Size(370, 533);
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
            this.selectTilePictureBox.BlendColor = System.Drawing.Color.White;
            this.selectTilePictureBox.BlendMode = PckStudio.Extensions.BlendMode.Multiply;
            this.tableLayoutPanel1.SetColumnSpan(this.selectTilePictureBox, 2);
            this.selectTilePictureBox.Image = null;
            this.selectTilePictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.selectTilePictureBox.Location = new System.Drawing.Point(3, 3);
            this.selectTilePictureBox.Name = "selectTilePictureBox";
            this.selectTilePictureBox.Size = new System.Drawing.Size(208, 201);
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
            this.replaceButton.Location = new System.Drawing.Point(3, 517);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(208, 19);
            this.replaceButton.TabIndex = 14;
            this.replaceButton.Text = "Replace";
            this.replaceButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.replaceButton.UseSelectable = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // tileNameLabel
            // 
            this.tileNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tileNameLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.tileNameLabel, 2);
            this.tileNameLabel.Location = new System.Drawing.Point(3, 213);
            this.tileNameLabel.Name = "tileNameLabel";
            this.tileNameLabel.Size = new System.Drawing.Size(208, 19);
            this.tileNameLabel.TabIndex = 19;
            this.tileNameLabel.Text = "TileName";
            this.tileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tileNameLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // variantComboBox
            // 
            this.variantComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.variantComboBox.Enabled = false;
            this.variantComboBox.FormattingEnabled = true;
            this.variantComboBox.ItemHeight = 23;
            this.variantComboBox.Location = new System.Drawing.Point(64, 235);
            this.variantComboBox.Name = "variantComboBox";
            this.variantComboBox.Size = new System.Drawing.Size(147, 29);
            this.variantComboBox.TabIndex = 17;
            this.variantComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.variantComboBox.UseSelectable = true;
            this.variantComboBox.SelectedIndexChanged += new System.EventHandler(this.variantComboBox_SelectedIndexChanged);
            // 
            // setColorButton
            // 
            this.setColorButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.setColorButton, 2);
            this.setColorButton.Location = new System.Drawing.Point(3, 467);
            this.setColorButton.Name = "setColorButton";
            this.setColorButton.Size = new System.Drawing.Size(208, 19);
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
            this.animationButton.Location = new System.Drawing.Point(3, 542);
            this.animationButton.Name = "animationButton";
            this.animationButton.Size = new System.Drawing.Size(208, 20);
            this.animationButton.TabIndex = 16;
            this.animationButton.Text = "Animation";
            this.animationButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.animationButton.UseSelectable = true;
            this.animationButton.Click += new System.EventHandler(this.animationButton_Click);
            // 
            // clearColorButton
            // 
            this.clearColorButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.clearColorButton, 2);
            this.clearColorButton.Location = new System.Drawing.Point(3, 492);
            this.clearColorButton.Name = "clearColorButton";
            this.clearColorButton.Size = new System.Drawing.Size(208, 19);
            this.clearColorButton.TabIndex = 24;
            this.clearColorButton.Text = "Clear Custom Colour";
            this.clearColorButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.clearColorButton.UseSelectable = true;
            this.clearColorButton.Click += new System.EventHandler(this.clearColorButton_Click);
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
        private System.Windows.Forms.ToolStripMenuItem extractTileToolStripMenuItem;
        private MetroFramework.Controls.MetroComboBox variantComboBox;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyColorMaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playAnimationsToolStripMenuItem;
        private MetroFramework.Controls.MetroLabel tileNameLabel;
        private MetroFramework.Controls.MetroLabel variantLabel;
        private MetroFramework.Controls.MetroButton clearColorButton;
        private MetroFramework.Controls.MetroButton setColorButton;
    }
}