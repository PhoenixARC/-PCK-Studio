namespace PckStudio
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
            this.prevButton = new MetroFramework.Controls.MetroButton();
            this.nextButton = new MetroFramework.Controls.MetroButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.originalPictureBox = new PckStudio.PictureBoxWithInterpolationMode();
            this.selectTilePictureBox = new PckStudio.PictureBoxWithInterpolationMode();
            this.infoTextBox = new System.Windows.Forms.RichTextBox();
            this.replaceButton = new MetroFramework.Controls.MetroButton();
            this.animationButton = new MetroFramework.Controls.MetroButton();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectTilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // prevButton
            // 
            this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prevButton.Enabled = false;
            this.prevButton.Location = new System.Drawing.Point(3, 210);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(145, 19);
            this.prevButton.TabIndex = 1;
            this.prevButton.Text = "Prev.";
            this.prevButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.prevButton.UseSelectable = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(154, 210);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(145, 19);
            this.nextButton.TabIndex = 2;
            this.nextButton.Text = "Next";
            this.nextButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.nextButton.UseSelectable = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(20, 60);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(759, 24);
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
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // extractTileToolStripMenuItem
            // 
            this.extractTileToolStripMenuItem.Name = "extractTileToolStripMenuItem";
            this.extractTileToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.extractTileToolStripMenuItem.Text = "Extract Tile";
            this.extractTileToolStripMenuItem.Click += new System.EventHandler(this.extractTileToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.prevButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.nextButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.originalPictureBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.selectTilePictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.infoTextBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.replaceButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.animationButton, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.09524F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.61905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.761905F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(759, 545);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // originalPictureBox
            // 
            this.originalPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.originalPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originalPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.originalPictureBox.Location = new System.Drawing.Point(305, 3);
            this.originalPictureBox.Name = "originalPictureBox";
            this.tableLayoutPanel1.SetRowSpan(this.originalPictureBox, 5);
            this.originalPictureBox.Size = new System.Drawing.Size(451, 539);
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
            this.tableLayoutPanel1.SetColumnSpan(this.selectTilePictureBox, 2);
            this.selectTilePictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.selectTilePictureBox.Location = new System.Drawing.Point(3, 3);
            this.selectTilePictureBox.Name = "selectTilePictureBox";
            this.selectTilePictureBox.Size = new System.Drawing.Size(296, 201);
            this.selectTilePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.selectTilePictureBox.TabIndex = 0;
            this.selectTilePictureBox.TabStop = false;
            // 
            // infoTextBox
            // 
            this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.infoTextBox, 2);
            this.infoTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.infoTextBox.Location = new System.Drawing.Point(3, 235);
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.Size = new System.Drawing.Size(296, 253);
            this.infoTextBox.TabIndex = 15;
            this.infoTextBox.TabStop = false;
            this.infoTextBox.Text = "";
            // 
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.replaceButton, 2);
            this.replaceButton.Location = new System.Drawing.Point(3, 519);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(296, 23);
            this.replaceButton.TabIndex = 14;
            this.replaceButton.Text = "Replace";
            this.replaceButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.replaceButton.UseSelectable = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // animationButton
            // 
            this.animationButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.animationButton, 2);
            this.animationButton.Location = new System.Drawing.Point(3, 494);
            this.animationButton.Name = "animationButton";
            this.animationButton.Size = new System.Drawing.Size(296, 19);
            this.animationButton.TabIndex = 16;
            this.animationButton.Text = "Animation";
            this.animationButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.animationButton.UseSelectable = true;
            this.animationButton.Click += new System.EventHandler(this.animationButton_Click);
            // 
            // TextureAtlasEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 649);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TextureAtlasEditor";
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.None;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Texture Atlas Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
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

        private PictureBoxWithInterpolationMode selectTilePictureBox;
        private MetroFramework.Controls.MetroButton prevButton;
        private MetroFramework.Controls.MetroButton nextButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroFramework.Controls.MetroButton replaceButton;
        private PictureBoxWithInterpolationMode originalPictureBox;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.RichTextBox infoTextBox;
        private MetroFramework.Controls.MetroButton animationButton;
        private System.Windows.Forms.ToolStripMenuItem extractTileToolStripMenuItem;
    }
}