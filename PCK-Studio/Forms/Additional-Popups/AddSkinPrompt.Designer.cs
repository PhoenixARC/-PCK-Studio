namespace PckStudio.Forms.Additional_Popups
{
    partial class AddSkinPrompt
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
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddSkinPrompt));
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.contextMenuSkin = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonDone = new MetroFramework.Controls.MetroButton();
            this.buttonModelGen = new MetroFramework.Controls.MetroButton();
            this.buttonCape = new MetroFramework.Controls.MetroButton();
            this.buttonSkin = new MetroFramework.Controls.MetroButton();
            this.displayBox = new System.Windows.Forms.PictureBox();
            this.radioButtonAuto = new MetroFramework.Controls.MetroRadioButton();
            this.radioButtonManual = new MetroFramework.Controls.MetroRadioButton();
            this.textSkinID = new MetroFramework.Controls.MetroTextBox();
            this.textSkinName = new MetroFramework.Controls.MetroTextBox();
            this.textThemeName = new MetroFramework.Controls.MetroTextBox();
            this.labelSelectTexture = new MetroFramework.Controls.MetroLabel();
            this.capeLabel = new MetroFramework.Controls.MetroLabel();
            this.buttonAnimGen = new MetroFramework.Controls.MetroButton();
            this.capePictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            this.skinPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.contextMenuSkin.SuspendLayout();
            this.contextMenuCape.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.capePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = System.Drawing.Color.White;
            label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = System.Drawing.Color.White;
            label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = System.Drawing.Color.White;
            label1.Name = "label1";
            // 
            // contextMenuSkin
            // 
            this.contextMenuSkin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem});
            this.contextMenuSkin.Name = "contextMenuSkin";
            resources.ApplyResources(this.contextMenuSkin, "contextMenuSkin");
            // 
            // replaceToolStripMenuItem
            // 
            resources.ApplyResources(this.replaceToolStripMenuItem, "replaceToolStripMenuItem");
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // contextMenuCape
            // 
            this.contextMenuCape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem1});
            this.contextMenuCape.Name = "contextMenuCape";
            resources.ApplyResources(this.contextMenuCape, "contextMenuCape");
            // 
            // replaceToolStripMenuItem1
            // 
            resources.ApplyResources(this.replaceToolStripMenuItem1, "replaceToolStripMenuItem1");
            this.replaceToolStripMenuItem1.Name = "replaceToolStripMenuItem1";
            // 
            // buttonDone
            // 
            resources.ApplyResources(this.buttonDone, "buttonDone");
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonDone.UseSelectable = true;
            this.buttonDone.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // buttonModelGen
            // 
            resources.ApplyResources(this.buttonModelGen, "buttonModelGen");
            this.buttonModelGen.Name = "buttonModelGen";
            this.buttonModelGen.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonModelGen.UseSelectable = true;
            this.buttonModelGen.Click += new System.EventHandler(this.CreateCustomModel_Click);
            // 
            // buttonCape
            // 
            this.buttonCape.BackgroundImage = global::PckStudio.Properties.Resources.HamburgerMenuIcon;
            resources.ApplyResources(this.buttonCape, "buttonCape");
            this.buttonCape.Name = "buttonCape";
            this.buttonCape.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonCape.UseSelectable = true;
            this.buttonCape.Click += new System.EventHandler(this.buttonCape_Click);
            // 
            // buttonSkin
            // 
            this.buttonSkin.BackgroundImage = global::PckStudio.Properties.Resources.HamburgerMenuIcon;
            resources.ApplyResources(this.buttonSkin, "buttonSkin");
            this.buttonSkin.Name = "buttonSkin";
            this.buttonSkin.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonSkin.UseSelectable = true;
            this.buttonSkin.Click += new System.EventHandler(this.buttonSkin_Click);
            // 
            // displayBox
            // 
            this.displayBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
            resources.ApplyResources(this.displayBox, "displayBox");
            this.displayBox.Name = "displayBox";
            this.displayBox.TabStop = false;
            // 
            // radioButtonAuto
            // 
            resources.ApplyResources(this.radioButtonAuto, "radioButtonAuto");
            this.radioButtonAuto.Name = "radioButtonAuto";
            this.radioButtonAuto.Style = MetroFramework.MetroColorStyle.White;
            this.radioButtonAuto.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.radioButtonAuto.UseSelectable = true;
            this.radioButtonAuto.CheckedChanged += new System.EventHandler(this.radioButtonAuto_CheckedChanged);
            // 
            // radioButtonManual
            // 
            resources.ApplyResources(this.radioButtonManual, "radioButtonManual");
            this.radioButtonManual.Checked = true;
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Style = MetroFramework.MetroColorStyle.White;
            this.radioButtonManual.TabStop = true;
            this.radioButtonManual.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.radioButtonManual.UseSelectable = true;
            this.radioButtonManual.CheckedChanged += new System.EventHandler(this.radioButtonManual_CheckedChanged);
            // 
            // textSkinID
            // 
            // 
            // 
            // 
            this.textSkinID.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.textSkinID.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.textSkinID.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.textSkinID.CustomButton.Name = "";
            this.textSkinID.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.textSkinID.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textSkinID.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.textSkinID.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textSkinID.CustomButton.UseSelectable = true;
            this.textSkinID.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.textSkinID.ForeColor = System.Drawing.Color.White;
            this.textSkinID.Lines = new string[0];
            resources.ApplyResources(this.textSkinID, "textSkinID");
            this.textSkinID.MaxLength = 8;
            this.textSkinID.Name = "textSkinID";
            this.textSkinID.PasswordChar = '\0';
            this.textSkinID.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textSkinID.SelectedText = "";
            this.textSkinID.SelectionLength = 0;
            this.textSkinID.SelectionStart = 0;
            this.textSkinID.ShortcutsEnabled = true;
            this.textSkinID.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textSkinID.UseSelectable = true;
            this.textSkinID.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textSkinID.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.textSkinID.TextChanged += new System.EventHandler(this.textSkinID_TextChanged);
            // 
            // textSkinName
            // 
            // 
            // 
            // 
            this.textSkinName.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.textSkinName.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode1")));
            this.textSkinName.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
            this.textSkinName.CustomButton.Name = "";
            this.textSkinName.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
            this.textSkinName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textSkinName.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
            this.textSkinName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textSkinName.CustomButton.UseSelectable = true;
            this.textSkinName.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
            this.textSkinName.ForeColor = System.Drawing.Color.White;
            this.textSkinName.Lines = new string[0];
            resources.ApplyResources(this.textSkinName, "textSkinName");
            this.textSkinName.MaxLength = 32767;
            this.textSkinName.Name = "textSkinName";
            this.textSkinName.PasswordChar = '\0';
            this.textSkinName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textSkinName.SelectedText = "";
            this.textSkinName.SelectionLength = 0;
            this.textSkinName.SelectionStart = 0;
            this.textSkinName.ShortcutsEnabled = true;
            this.textSkinName.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textSkinName.UseSelectable = true;
            this.textSkinName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textSkinName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // textThemeName
            // 
            // 
            // 
            // 
            this.textThemeName.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image2")));
            this.textThemeName.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode2")));
            this.textThemeName.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location2")));
            this.textThemeName.CustomButton.Name = "";
            this.textThemeName.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size2")));
            this.textThemeName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textThemeName.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex2")));
            this.textThemeName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textThemeName.CustomButton.UseSelectable = true;
            this.textThemeName.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible2")));
            this.textThemeName.ForeColor = System.Drawing.Color.White;
            this.textThemeName.Lines = new string[0];
            resources.ApplyResources(this.textThemeName, "textThemeName");
            this.textThemeName.MaxLength = 32767;
            this.textThemeName.Name = "textThemeName";
            this.textThemeName.PasswordChar = '\0';
            this.textThemeName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textThemeName.SelectedText = "";
            this.textThemeName.SelectionLength = 0;
            this.textThemeName.SelectionStart = 0;
            this.textThemeName.ShortcutsEnabled = true;
            this.textThemeName.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textThemeName.UseSelectable = true;
            this.textThemeName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textThemeName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // labelSelectTexture
            // 
            resources.ApplyResources(this.labelSelectTexture, "labelSelectTexture");
            this.labelSelectTexture.Name = "labelSelectTexture";
            this.labelSelectTexture.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // capeLabel
            // 
            resources.ApplyResources(this.capeLabel, "capeLabel");
            this.capeLabel.Name = "capeLabel";
            this.capeLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // buttonAnimGen
            // 
            resources.ApplyResources(this.buttonAnimGen, "buttonAnimGen");
            this.buttonAnimGen.Name = "buttonAnimGen";
            this.buttonAnimGen.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonAnimGen.UseSelectable = true;
            this.buttonAnimGen.Click += new System.EventHandler(this.buttonAnimGen_Click);
            // 
            // capePictureBox
            // 
            resources.ApplyResources(this.capePictureBox, "capePictureBox");
            this.capePictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.capePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.capePictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.capePictureBox.Name = "capePictureBox";
            this.capePictureBox.TabStop = false;
            this.capePictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.capePictureBox_MouseClick);
            // 
            // skinPictureBox
            // 
            resources.ApplyResources(this.skinPictureBox, "skinPictureBox");
            this.skinPictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.skinPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.skinPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.skinPictureBox.Name = "skinPictureBox";
            this.skinPictureBox.TabStop = false;
            this.skinPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.skinPictureBox_MouseClick);
            // 
            // AddNewSkin
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonCape);
            this.Controls.Add(this.buttonAnimGen);
            this.Controls.Add(this.capeLabel);
            this.Controls.Add(this.textThemeName);
            this.Controls.Add(this.textSkinName);
            this.Controls.Add(this.textSkinID);
            this.Controls.Add(this.labelSelectTexture);
            this.Controls.Add(this.radioButtonManual);
            this.Controls.Add(this.radioButtonAuto);
            this.Controls.Add(this.buttonDone);
            this.Controls.Add(this.buttonModelGen);
            this.Controls.Add(this.buttonSkin);
            this.Controls.Add(this.capePictureBox);
            this.Controls.Add(this.skinPictureBox);
            this.Controls.Add(this.displayBox);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.MaximizeBox = false;
            this.Name = "AddNewSkin";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.AddNewSkin_Load);
            this.contextMenuSkin.ResumeLayout(false);
            this.contextMenuCape.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.displayBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.capePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuSkin;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuCape;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem1;
        private MetroFramework.Controls.MetroButton buttonDone;
        private MetroFramework.Controls.MetroButton buttonModelGen;
        private MetroFramework.Controls.MetroButton buttonCape;
        private MetroFramework.Controls.MetroButton buttonSkin;
        private System.Windows.Forms.PictureBox displayBox;
        private MetroFramework.Controls.MetroRadioButton radioButtonAuto;
        private MetroFramework.Controls.MetroRadioButton radioButtonManual;
		private MetroFramework.Controls.MetroTextBox textSkinID;
		private MetroFramework.Controls.MetroTextBox textSkinName;
		private MetroFramework.Controls.MetroTextBox textThemeName;
		private PckStudio.ToolboxItems.InterpolationPictureBox skinPictureBox;
        private PckStudio.ToolboxItems.InterpolationPictureBox capePictureBox;
		private MetroFramework.Controls.MetroButton buttonAnimGen;
        private MetroFramework.Controls.MetroLabel labelSelectTexture;
        private MetroFramework.Controls.MetroLabel capeLabel;
    }
}