namespace PckStudio.Forms.Editor
{
    partial class CustomSkinEditor
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
            System.Windows.Forms.Label label5;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomSkinEditor));
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label labelTextureMappingPreview;
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonDone = new MetroFramework.Controls.MetroButton();
            this.buttonEXPORT = new MetroFramework.Controls.MetroButton();
            this.buttonIMPORT = new MetroFramework.Controls.MetroButton();
            this.importCustomSkinButton = new MetroFramework.Controls.MetroButton();
            this.buttonExportModel = new MetroFramework.Controls.MetroButton();
            this.outlineColorButton = new MetroFramework.Controls.MetroButton();
            this.generateTextureCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.checkGuide = new MetroFramework.Controls.MetroCheckBox();
            this.showArmorCheckbox = new MetroFramework.Controls.MetroCheckBox();
            this.SizeXUpDown = new System.Windows.Forms.NumericUpDown();
            this.SizeYUpDown = new System.Windows.Forms.NumericUpDown();
            this.SizeZUpDown = new System.Windows.Forms.NumericUpDown();
            this.TextureXUpDown = new System.Windows.Forms.NumericUpDown();
            this.TextureYUpDown = new System.Windows.Forms.NumericUpDown();
            this.PosZUpDown = new System.Windows.Forms.NumericUpDown();
            this.PosYUpDown = new System.Windows.Forms.NumericUpDown();
            this.PosXUpDown = new System.Windows.Forms.NumericUpDown();
            this.skinPartListBox = new System.Windows.Forms.ListBox();
            this.clampToViewCheckbox = new MetroFramework.Controls.MetroCheckBox();
            this.captureScreenshotButton = new MetroFramework.Controls.MetroButton();
            this.showToolsCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.skinNameLabel = new MetroFramework.Controls.MetroLabel();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.skinPartsTabPage = new System.Windows.Forms.TabPage();
            this.skinOffsetsTabPage = new System.Windows.Forms.TabPage();
            this.offsetListBox = new System.Windows.Forms.ListBox();
            this.renderer3D1 = new PckStudio.Rendering.SkinRenderer();
            this.uvPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            label5 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            labelTextureMappingPreview = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextureXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextureYUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.skinPartsTabPage.SuspendLayout();
            this.skinOffsetsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ForeColor = System.Drawing.Color.White;
            label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = System.Drawing.Color.White;
            label3.Name = "label3";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.ForeColor = System.Drawing.Color.White;
            label7.Name = "label7";
            // 
            // labelTextureMappingPreview
            // 
            resources.ApplyResources(labelTextureMappingPreview, "labelTextureMappingPreview");
            labelTextureMappingPreview.ForeColor = System.Drawing.Color.White;
            labelTextureMappingPreview.Name = "labelTextureMappingPreview";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.cloneToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.changeColorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // createToolStripMenuItem
            // 
            resources.ApplyResources(this.createToolStripMenuItem, "createToolStripMenuItem");
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // cloneToolStripMenuItem
            // 
            resources.ApplyResources(this.cloneToolStripMenuItem, "cloneToolStripMenuItem");
            this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
            this.cloneToolStripMenuItem.Click += new System.EventHandler(this.cloneToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // changeColorToolStripMenuItem
            // 
            resources.ApplyResources(this.changeColorToolStripMenuItem, "changeColorToolStripMenuItem");
            this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
            // 
            // buttonDone
            // 
            resources.ApplyResources(this.buttonDone, "buttonDone");
            this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonDone.ForeColor = System.Drawing.Color.White;
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonDone.UseSelectable = true;
            this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
            // 
            // buttonEXPORT
            // 
            resources.ApplyResources(this.buttonEXPORT, "buttonEXPORT");
            this.buttonEXPORT.ForeColor = System.Drawing.Color.White;
            this.buttonEXPORT.Name = "buttonEXPORT";
            this.buttonEXPORT.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonEXPORT.UseSelectable = true;
            this.buttonEXPORT.Click += new System.EventHandler(this.buttonEXPORT_Click);
            // 
            // buttonIMPORT
            // 
            resources.ApplyResources(this.buttonIMPORT, "buttonIMPORT");
            this.buttonIMPORT.ForeColor = System.Drawing.Color.White;
            this.buttonIMPORT.Name = "buttonIMPORT";
            this.buttonIMPORT.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonIMPORT.UseSelectable = true;
            this.buttonIMPORT.Click += new System.EventHandler(this.buttonIMPORT_Click);
            // 
            // importCustomSkinButton
            // 
            resources.ApplyResources(this.importCustomSkinButton, "importCustomSkinButton");
            this.importCustomSkinButton.ForeColor = System.Drawing.Color.White;
            this.importCustomSkinButton.Name = "importCustomSkinButton";
            this.importCustomSkinButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.importCustomSkinButton.UseSelectable = true;
            this.importCustomSkinButton.Click += new System.EventHandler(this.importCustomSkinButton_Click);
            // 
            // buttonExportModel
            // 
            resources.ApplyResources(this.buttonExportModel, "buttonExportModel");
            this.buttonExportModel.ForeColor = System.Drawing.Color.White;
            this.buttonExportModel.Name = "buttonExportModel";
            this.buttonExportModel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonExportModel.UseSelectable = true;
            this.buttonExportModel.Click += new System.EventHandler(this.buttonExportModel_Click);
            // 
            // outlineColorButton
            // 
            resources.ApplyResources(this.outlineColorButton, "outlineColorButton");
            this.outlineColorButton.ForeColor = System.Drawing.Color.White;
            this.outlineColorButton.Name = "outlineColorButton";
            this.outlineColorButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.outlineColorButton.UseSelectable = true;
            this.outlineColorButton.Click += new System.EventHandler(this.outlineColorButton_Click);
            // 
            // generateTextureCheckBox
            // 
            resources.ApplyResources(this.generateTextureCheckBox, "generateTextureCheckBox");
            this.generateTextureCheckBox.Name = "generateTextureCheckBox";
            this.generateTextureCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.generateTextureCheckBox.UseSelectable = true;
            // 
            // checkGuide
            // 
            resources.ApplyResources(this.checkGuide, "checkGuide");
            this.checkGuide.Name = "checkGuide";
            this.checkGuide.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.checkGuide.UseSelectable = true;
            this.checkGuide.CheckedChanged += new System.EventHandler(this.checkGuide_CheckedChanged);
            // 
            // showArmorCheckbox
            // 
            resources.ApplyResources(this.showArmorCheckbox, "showArmorCheckbox");
            this.showArmorCheckbox.Name = "showArmorCheckbox";
            this.showArmorCheckbox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.showArmorCheckbox.UseSelectable = true;
            this.showArmorCheckbox.CheckedChanged += new System.EventHandler(this.showArmorCheckbox_CheckedChanged);
            // 
            // SizeXUpDown
            // 
            resources.ApplyResources(this.SizeXUpDown, "SizeXUpDown");
            this.SizeXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.SizeXUpDown.DecimalPlaces = 1;
            this.SizeXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.SizeXUpDown.Name = "SizeXUpDown";
            // 
            // SizeYUpDown
            // 
            resources.ApplyResources(this.SizeYUpDown, "SizeYUpDown");
            this.SizeYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.SizeYUpDown.DecimalPlaces = 1;
            this.SizeYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.SizeYUpDown.Name = "SizeYUpDown";
            // 
            // SizeZUpDown
            // 
            resources.ApplyResources(this.SizeZUpDown, "SizeZUpDown");
            this.SizeZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.SizeZUpDown.DecimalPlaces = 1;
            this.SizeZUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.SizeZUpDown.Name = "SizeZUpDown";
            // 
            // TextureXUpDown
            // 
            resources.ApplyResources(this.TextureXUpDown, "TextureXUpDown");
            this.TextureXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.TextureXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.TextureXUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.TextureXUpDown.Name = "TextureXUpDown";
            // 
            // TextureYUpDown
            // 
            resources.ApplyResources(this.TextureYUpDown, "TextureYUpDown");
            this.TextureYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.TextureYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.TextureYUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.TextureYUpDown.Name = "TextureYUpDown";
            // 
            // PosZUpDown
            // 
            resources.ApplyResources(this.PosZUpDown, "PosZUpDown");
            this.PosZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.PosZUpDown.DecimalPlaces = 1;
            this.PosZUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.PosZUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.PosZUpDown.Name = "PosZUpDown";
            // 
            // PosYUpDown
            // 
            resources.ApplyResources(this.PosYUpDown, "PosYUpDown");
            this.PosYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.PosYUpDown.DecimalPlaces = 1;
            this.PosYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.PosYUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.PosYUpDown.Name = "PosYUpDown";
            // 
            // PosXUpDown
            // 
            resources.ApplyResources(this.PosXUpDown, "PosXUpDown");
            this.PosXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.PosXUpDown.DecimalPlaces = 1;
            this.PosXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
            this.PosXUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.PosXUpDown.Name = "PosXUpDown";
            // 
            // skinPartListBox
            // 
            this.skinPartListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.skinPartListBox.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.skinPartListBox, "skinPartListBox");
            this.skinPartListBox.FormattingEnabled = true;
            this.skinPartListBox.Name = "skinPartListBox";
            this.skinPartListBox.Tag = "";
            this.skinPartListBox.SelectedIndexChanged += new System.EventHandler(this.skinPartListBox_SelectedIndexChanged);
            this.skinPartListBox.DoubleClick += new System.EventHandler(this.skinPartListBox_DoubleClick);
            this.skinPartListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.skinPartListBox_KeyUp);
            // 
            // clampToViewCheckbox
            // 
            resources.ApplyResources(this.clampToViewCheckbox, "clampToViewCheckbox");
            this.clampToViewCheckbox.Name = "clampToViewCheckbox";
            this.clampToViewCheckbox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.clampToViewCheckbox.UseSelectable = true;
            this.clampToViewCheckbox.CheckedChanged += new System.EventHandler(this.clampToViewCheckbox_CheckedChanged);
            // 
            // captureScreenshotButton
            // 
            resources.ApplyResources(this.captureScreenshotButton, "captureScreenshotButton");
            this.captureScreenshotButton.ForeColor = System.Drawing.Color.White;
            this.captureScreenshotButton.Name = "captureScreenshotButton";
            this.captureScreenshotButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.captureScreenshotButton.UseSelectable = true;
            this.captureScreenshotButton.Click += new System.EventHandler(this.captureScreenshotButton_Click);
            // 
            // showToolsCheckBox
            // 
            resources.ApplyResources(this.showToolsCheckBox, "showToolsCheckBox");
            this.showToolsCheckBox.Name = "showToolsCheckBox";
            this.showToolsCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.showToolsCheckBox.UseSelectable = true;
            // 
            // skinNameLabel
            // 
            resources.ApplyResources(this.skinNameLabel, "skinNameLabel");
            this.skinNameLabel.Name = "skinNameLabel";
            this.skinNameLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.skinPartsTabPage);
            this.metroTabControl1.Controls.Add(this.skinOffsetsTabPage);
            resources.ApplyResources(this.metroTabControl1, "metroTabControl1");
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Pink;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseSelectable = true;
            // 
            // skinPartsTabPage
            // 
            this.skinPartsTabPage.Controls.Add(this.skinPartListBox);
            resources.ApplyResources(this.skinPartsTabPage, "skinPartsTabPage");
            this.skinPartsTabPage.Name = "skinPartsTabPage";
            // 
            // skinOffsetsTabPage
            // 
            this.skinOffsetsTabPage.Controls.Add(this.offsetListBox);
            resources.ApplyResources(this.skinOffsetsTabPage, "skinOffsetsTabPage");
            this.skinOffsetsTabPage.Name = "skinOffsetsTabPage";
            // 
            // offsetListBox
            // 
            resources.ApplyResources(this.offsetListBox, "offsetListBox");
            this.offsetListBox.FormattingEnabled = true;
            this.offsetListBox.Name = "offsetListBox";
            // 
            // renderer3D1
            // 
            resources.ApplyResources(this.renderer3D1, "renderer3D1");
            this.renderer3D1.BackColor = System.Drawing.Color.DimGray;
            this.renderer3D1.ClampModel = false;
            this.renderer3D1.MouseSensetivity = 0.01F;
            this.renderer3D1.Name = "renderer3D1";
            this.renderer3D1.OutlineColor = System.Drawing.Color.Empty;
            this.renderer3D1.RefreshRate = 50;
            this.renderer3D1.SelectedIndex = -1;
            this.renderer3D1.ShowArmor = false;
            this.renderer3D1.ShowGuideLines = false;
            this.renderer3D1.Texture = null;
            this.renderer3D1.VSync = true;
            this.renderer3D1.TextureChanging += new System.EventHandler<PckStudio.Rendering.TextureChangingEventArgs>(this.renderer3D1_TextureChanging);
            // 
            // uvPictureBox
            // 
            resources.ApplyResources(this.uvPictureBox, "uvPictureBox");
            this.uvPictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.uvPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.uvPictureBox.Name = "uvPictureBox";
            this.uvPictureBox.TabStop = false;
            // 
            // CustomSkinEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroTabControl1);
            this.Controls.Add(this.skinNameLabel);
            this.Controls.Add(this.showToolsCheckBox);
            this.Controls.Add(this.captureScreenshotButton);
            this.Controls.Add(this.clampToViewCheckbox);
            this.Controls.Add(this.PosZUpDown);
            this.Controls.Add(this.PosYUpDown);
            this.Controls.Add(this.PosXUpDown);
            this.Controls.Add(this.renderer3D1);
            this.Controls.Add(this.TextureYUpDown);
            this.Controls.Add(this.TextureXUpDown);
            this.Controls.Add(this.SizeZUpDown);
            this.Controls.Add(this.SizeYUpDown);
            this.Controls.Add(this.SizeXUpDown);
            this.Controls.Add(this.showArmorCheckbox);
            this.Controls.Add(this.checkGuide);
            this.Controls.Add(this.generateTextureCheckBox);
            this.Controls.Add(this.outlineColorButton);
            this.Controls.Add(this.buttonExportModel);
            this.Controls.Add(this.importCustomSkinButton);
            this.Controls.Add(this.buttonEXPORT);
            this.Controls.Add(labelTextureMappingPreview);
            this.Controls.Add(this.buttonIMPORT);
            this.Controls.Add(label7);
            this.Controls.Add(this.buttonDone);
            this.Controls.Add(label3);
            this.Controls.Add(label5);
            this.Controls.Add(this.uvPictureBox);
            this.Name = "CustomSkinEditor";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomSkinEditor_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextureXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextureYUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).EndInit();
            this.metroTabControl1.ResumeLayout(false);
            this.skinPartsTabPage.ResumeLayout(false);
            this.skinOffsetsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uvPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem;
        private MetroFramework.Controls.MetroButton buttonDone;
        private MetroFramework.Controls.MetroButton outlineColorButton;
        private MetroFramework.Controls.MetroButton buttonExportModel;
        private MetroFramework.Controls.MetroButton importCustomSkinButton;
        private PckStudio.ToolboxItems.InterpolationPictureBox uvPictureBox;
        private MetroFramework.Controls.MetroButton buttonIMPORT;
        private MetroFramework.Controls.MetroButton buttonEXPORT;
        private MetroFramework.Controls.MetroCheckBox generateTextureCheckBox;
        private MetroFramework.Controls.MetroCheckBox checkGuide;
        private MetroFramework.Controls.MetroCheckBox showArmorCheckbox;
        private System.Windows.Forms.NumericUpDown SizeXUpDown;
        private System.Windows.Forms.NumericUpDown SizeYUpDown;
        private System.Windows.Forms.NumericUpDown SizeZUpDown;
        private System.Windows.Forms.NumericUpDown TextureXUpDown;
        private System.Windows.Forms.NumericUpDown TextureYUpDown;
        private System.Windows.Forms.NumericUpDown PosZUpDown;
        private System.Windows.Forms.NumericUpDown PosYUpDown;
        private System.Windows.Forms.NumericUpDown PosXUpDown;
        private Rendering.SkinRenderer renderer3D1;
        private System.Windows.Forms.ListBox skinPartListBox;
        private MetroFramework.Controls.MetroCheckBox clampToViewCheckbox;
        private MetroFramework.Controls.MetroButton captureScreenshotButton;
        private MetroFramework.Controls.MetroCheckBox showToolsCheckBox;
        private MetroFramework.Controls.MetroLabel skinNameLabel;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private System.Windows.Forms.TabPage skinPartsTabPage;
        private System.Windows.Forms.TabPage skinOffsetsTabPage;
        private System.Windows.Forms.ListBox offsetListBox;
    }
}