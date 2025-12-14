
using System.Windows.Forms;

namespace PckStudio.Forms.Editor
{
	partial class COLEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(COLEditor));
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.filterPrompt = new PckStudio.Forms.Additional_Popups.Animation.FilterPrompt();
            this.setColorBtn = new MetroFramework.Controls.MetroButton();
            this.blueUpDown = new System.Windows.Forms.NumericUpDown();
            this.greenUpDown = new System.Windows.Forms.NumericUpDown();
            this.redUpDown = new System.Windows.Forms.NumericUpDown();
            this.alphaUpDown = new System.Windows.Forms.NumericUpDown();
            this.alphaLabel = new MetroFramework.Controls.MetroLabel();
            this.blueLabel = new MetroFramework.Controls.MetroLabel();
            this.greenLabel = new MetroFramework.Controls.MetroLabel();
            this.redLabel = new MetroFramework.Controls.MetroLabel();
            this.colorTextbox = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stripPS4BiomesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU13ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU14ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU19ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU31ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU32ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU43ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU46ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU51ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU53ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU54ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TU69ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._1_9_1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compactViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ColorContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.copyColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreOriginalColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blueUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.ColorContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.filterPrompt);
            this.metroPanel1.Controls.Add(this.setColorBtn);
            this.metroPanel1.Controls.Add(this.blueUpDown);
            this.metroPanel1.Controls.Add(this.greenUpDown);
            this.metroPanel1.Controls.Add(this.redUpDown);
            this.metroPanel1.Controls.Add(this.alphaUpDown);
            this.metroPanel1.Controls.Add(this.alphaLabel);
            this.metroPanel1.Controls.Add(this.blueLabel);
            this.metroPanel1.Controls.Add(this.greenLabel);
            this.metroPanel1.Controls.Add(this.redLabel);
            this.metroPanel1.Controls.Add(this.colorTextbox);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.metroPanel1, "metroPanel1");
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Style = MetroFramework.MetroColorStyle.Silver;
            this.metroPanel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // filterPrompt
            // 
            this.filterPrompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.filterPrompt.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.filterPrompt, "filterPrompt");
            this.filterPrompt.Name = "filterPrompt";
            this.filterPrompt.OnSelectedItemChanged += new System.EventHandler(this.filterPrompt_OnSelectedItemChanged);
            // 
            // setColorBtn
            // 
            resources.ApplyResources(this.setColorBtn, "setColorBtn");
            this.setColorBtn.Name = "setColorBtn";
            this.setColorBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.setColorBtn.UseSelectable = true;
            this.setColorBtn.Click += new System.EventHandler(this.setColorBtn_Click);
            // 
            // blueUpDown
            // 
            this.blueUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.blueUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.blueUpDown, "blueUpDown");
            this.blueUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueUpDown.Name = "blueUpDown";
            // 
            // greenUpDown
            // 
            this.greenUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.greenUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.greenUpDown, "greenUpDown");
            this.greenUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenUpDown.Name = "greenUpDown";
            // 
            // redUpDown
            // 
            this.redUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.redUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.redUpDown, "redUpDown");
            this.redUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redUpDown.Name = "redUpDown";
            // 
            // alphaUpDown
            // 
            this.alphaUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.alphaUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.alphaUpDown, "alphaUpDown");
            this.alphaUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.alphaUpDown.Name = "alphaUpDown";
            this.alphaUpDown.ValueChanged += new System.EventHandler(this.alpha_ValueChanged);
            // 
            // alphaLabel
            // 
            resources.ApplyResources(this.alphaLabel, "alphaLabel");
            this.alphaLabel.Name = "alphaLabel";
            this.alphaLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // blueLabel
            // 
            resources.ApplyResources(this.blueLabel, "blueLabel");
            this.blueLabel.Name = "blueLabel";
            this.blueLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // greenLabel
            // 
            resources.ApplyResources(this.greenLabel, "greenLabel");
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // redLabel
            // 
            resources.ApplyResources(this.redLabel, "redLabel");
            this.redLabel.Name = "redLabel";
            this.redLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // colorTextbox
            // 
            this.colorTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            // 
            // 
            // 
            this.colorTextbox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.colorTextbox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.colorTextbox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.colorTextbox.CustomButton.Name = "";
            this.colorTextbox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.colorTextbox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.colorTextbox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.colorTextbox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.colorTextbox.CustomButton.UseSelectable = true;
            this.colorTextbox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.colorTextbox.Lines = new string[0];
            resources.ApplyResources(this.colorTextbox, "colorTextbox");
            this.colorTextbox.MaxLength = 6;
            this.colorTextbox.Name = "colorTextbox";
            this.colorTextbox.PasswordChar = '\0';
            this.colorTextbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.colorTextbox.SelectedText = "";
            this.colorTextbox.SelectionLength = 0;
            this.colorTextbox.SelectionStart = 0;
            this.colorTextbox.ShortcutsEnabled = true;
            this.colorTextbox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.colorTextbox.UseSelectable = true;
            this.colorTextbox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.colorTextbox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.colorTextbox.TextChanged += new System.EventHandler(this.colorBox_TextChanged);
            this.colorTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.colorTextbox_KeyPress);
            // 
            // metroLabel1
            // 
            resources.ApplyResources(this.metroLabel1, "metroLabel1");
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // saveToolStripMenuItem1
            // 
            resources.ApplyResources(this.saveToolStripMenuItem1, "saveToolStripMenuItem1");
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripPS4BiomesToolStripMenuItem,
            this.targetUpdateToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // stripPS4BiomesToolStripMenuItem
            // 
            this.stripPS4BiomesToolStripMenuItem.Name = "stripPS4BiomesToolStripMenuItem";
            resources.ApplyResources(this.stripPS4BiomesToolStripMenuItem, "stripPS4BiomesToolStripMenuItem");
            this.stripPS4BiomesToolStripMenuItem.Click += new System.EventHandler(this.stripPS4BiomesToolStripMenuItem_Click);
            // 
            // targetUpdateToolStripMenuItem
            // 
            this.targetUpdateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TU12ToolStripMenuItem,
            this.TU13ToolStripMenuItem,
            this.TU14ToolStripMenuItem,
            this.TU19ToolStripMenuItem,
            this.TU31ToolStripMenuItem,
            this.TU32ToolStripMenuItem,
            this.TU43ToolStripMenuItem,
            this.TU46ToolStripMenuItem,
            this.TU51ToolStripMenuItem,
            this.TU53ToolStripMenuItem,
            this.TU54ToolStripMenuItem,
            this.TU69ToolStripMenuItem,
            this._1_9_1ToolStripMenuItem});
            this.targetUpdateToolStripMenuItem.Name = "targetUpdateToolStripMenuItem";
            resources.ApplyResources(this.targetUpdateToolStripMenuItem, "targetUpdateToolStripMenuItem");
            // 
            // TU12ToolStripMenuItem
            // 
            this.TU12ToolStripMenuItem.Name = "TU12ToolStripMenuItem";
            resources.ApplyResources(this.TU12ToolStripMenuItem, "TU12ToolStripMenuItem");
            // 
            // TU13ToolStripMenuItem
            // 
            this.TU13ToolStripMenuItem.Name = "TU13ToolStripMenuItem";
            resources.ApplyResources(this.TU13ToolStripMenuItem, "TU13ToolStripMenuItem");
            // 
            // TU14ToolStripMenuItem
            // 
            this.TU14ToolStripMenuItem.Name = "TU14ToolStripMenuItem";
            resources.ApplyResources(this.TU14ToolStripMenuItem, "TU14ToolStripMenuItem");
            // 
            // TU19ToolStripMenuItem
            // 
            this.TU19ToolStripMenuItem.Name = "TU19ToolStripMenuItem";
            resources.ApplyResources(this.TU19ToolStripMenuItem, "TU19ToolStripMenuItem");
            // 
            // TU31ToolStripMenuItem
            // 
            this.TU31ToolStripMenuItem.Name = "TU31ToolStripMenuItem";
            resources.ApplyResources(this.TU31ToolStripMenuItem, "TU31ToolStripMenuItem");
            // 
            // TU32ToolStripMenuItem
            // 
            this.TU32ToolStripMenuItem.Name = "TU32ToolStripMenuItem";
            resources.ApplyResources(this.TU32ToolStripMenuItem, "TU32ToolStripMenuItem");
            // 
            // TU43ToolStripMenuItem
            // 
            this.TU43ToolStripMenuItem.Name = "TU43ToolStripMenuItem";
            resources.ApplyResources(this.TU43ToolStripMenuItem, "TU43ToolStripMenuItem");
            // 
            // TU46ToolStripMenuItem
            // 
            this.TU46ToolStripMenuItem.Name = "TU46ToolStripMenuItem";
            resources.ApplyResources(this.TU46ToolStripMenuItem, "TU46ToolStripMenuItem");
            // 
            // TU51ToolStripMenuItem
            // 
            this.TU51ToolStripMenuItem.Name = "TU51ToolStripMenuItem";
            resources.ApplyResources(this.TU51ToolStripMenuItem, "TU51ToolStripMenuItem");
            // 
            // TU53ToolStripMenuItem
            // 
            this.TU53ToolStripMenuItem.Name = "TU53ToolStripMenuItem";
            resources.ApplyResources(this.TU53ToolStripMenuItem, "TU53ToolStripMenuItem");
            // 
            // TU54ToolStripMenuItem
            // 
            this.TU54ToolStripMenuItem.Name = "TU54ToolStripMenuItem";
            resources.ApplyResources(this.TU54ToolStripMenuItem, "TU54ToolStripMenuItem");
            // 
            // TU69ToolStripMenuItem
            // 
            this.TU69ToolStripMenuItem.Name = "TU69ToolStripMenuItem";
            resources.ApplyResources(this.TU69ToolStripMenuItem, "TU69ToolStripMenuItem");
            // 
            // _1_9_1ToolStripMenuItem
            // 
            this._1_9_1ToolStripMenuItem.Name = "_1_9_1ToolStripMenuItem";
            resources.ApplyResources(this._1_9_1ToolStripMenuItem, "_1_9_1ToolStripMenuItem");
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compactViewToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // compactViewToolStripMenuItem
            // 
            this.compactViewToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.compactViewToolStripMenuItem.CheckOnClick = true;
            this.compactViewToolStripMenuItem.Name = "compactViewToolStripMenuItem";
            resources.ApplyResources(this.compactViewToolStripMenuItem, "compactViewToolStripMenuItem");
            this.compactViewToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.compactViewToolStripMenuItem_CheckStateChanged);
            // 
            // ColorContextMenu
            // 
            this.ColorContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyColorToolStripMenuItem,
            this.pasteColorToolStripMenuItem,
            this.restoreOriginalColorToolStripMenuItem,
            this.addColorToolStripMenuItem,
            this.removeColorToolStripMenuItem});
            this.ColorContextMenu.Name = "ColorContextMenu";
            resources.ApplyResources(this.ColorContextMenu, "ColorContextMenu");
            // 
            // copyColorToolStripMenuItem
            // 
            this.copyColorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_copy;
            this.copyColorToolStripMenuItem.Name = "copyColorToolStripMenuItem";
            resources.ApplyResources(this.copyColorToolStripMenuItem, "copyColorToolStripMenuItem");
            this.copyColorToolStripMenuItem.Click += new System.EventHandler(this.copyColorToolStripMenuItem_Click);
            // 
            // pasteColorToolStripMenuItem
            // 
            this.pasteColorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_paste;
            this.pasteColorToolStripMenuItem.Name = "pasteColorToolStripMenuItem";
            resources.ApplyResources(this.pasteColorToolStripMenuItem, "pasteColorToolStripMenuItem");
            this.pasteColorToolStripMenuItem.Click += new System.EventHandler(this.pasteColorToolStripMenuItem_Click);
            // 
            // restoreOriginalColorToolStripMenuItem
            // 
            this.restoreOriginalColorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_restore;
            this.restoreOriginalColorToolStripMenuItem.Name = "restoreOriginalColorToolStripMenuItem";
            resources.ApplyResources(this.restoreOriginalColorToolStripMenuItem, "restoreOriginalColorToolStripMenuItem");
            this.restoreOriginalColorToolStripMenuItem.Click += new System.EventHandler(this.restoreOriginalColorToolStripMenuItem_Click);
            // 
            // addColorToolStripMenuItem
            // 
            this.addColorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_new;
            this.addColorToolStripMenuItem.Name = "addColorToolStripMenuItem";
            resources.ApplyResources(this.addColorToolStripMenuItem, "addColorToolStripMenuItem");
            this.addColorToolStripMenuItem.Click += new System.EventHandler(this.addColorToolStripMenuItem_Click);
            // 
            // removeColorToolStripMenuItem
            // 
            this.removeColorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_delete;
            this.removeColorToolStripMenuItem.Name = "removeColorToolStripMenuItem";
            resources.ApplyResources(this.removeColorToolStripMenuItem, "removeColorToolStripMenuItem");
            this.removeColorToolStripMenuItem.Click += new System.EventHandler(this.removeColorToolStripMenuItem_Click);
            // 
            // COLEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.metroPanel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "COLEditor";
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blueUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ColorContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private MetroFramework.Controls.MetroPanel metroPanel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private MetroFramework.Controls.MetroLabel blueLabel;
		private MetroFramework.Controls.MetroLabel greenLabel;
		private MetroFramework.Controls.MetroLabel redLabel;
		private MetroFramework.Controls.MetroLabel alphaLabel;
		private System.Windows.Forms.NumericUpDown alphaUpDown;
		private MetroFramework.Controls.MetroButton setColorBtn;
		private NumericUpDown blueUpDown;
		private NumericUpDown greenUpDown;
		private NumericUpDown redUpDown;
		private MetroFramework.Controls.MetroTextBox colorTextbox;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private MetroFramework.Controls.MetroContextMenu ColorContextMenu;
		private ToolStripMenuItem restoreOriginalColorToolStripMenuItem;
        private ToolStripMenuItem copyColorToolStripMenuItem;
        private ToolStripMenuItem pasteColorToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem targetUpdateToolStripMenuItem;
        private ToolStripMenuItem TU12ToolStripMenuItem;
        private ToolStripMenuItem TU13ToolStripMenuItem;
        private ToolStripMenuItem TU14ToolStripMenuItem;
        private ToolStripMenuItem TU19ToolStripMenuItem;
        private ToolStripMenuItem TU31ToolStripMenuItem;
        private ToolStripMenuItem TU32ToolStripMenuItem;
        private ToolStripMenuItem TU43ToolStripMenuItem;
        private ToolStripMenuItem TU46ToolStripMenuItem;
        private ToolStripMenuItem TU51ToolStripMenuItem;
        private ToolStripMenuItem TU53ToolStripMenuItem;
        private ToolStripMenuItem TU54ToolStripMenuItem;
        private ToolStripMenuItem TU69ToolStripMenuItem;
        private ToolStripMenuItem _1_9_1ToolStripMenuItem;
        private ToolStripMenuItem stripPS4BiomesToolStripMenuItem;
        private ToolStripMenuItem addColorToolStripMenuItem;
        private ToolStripMenuItem removeColorToolStripMenuItem;
        private Additional_Popups.Animation.FilterPrompt filterPrompt;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem compactViewToolStripMenuItem;
    }
}