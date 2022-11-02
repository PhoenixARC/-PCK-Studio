
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
			this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
			this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
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
			this.waterTab = new System.Windows.Forms.TabPage();
			this.waterTreeView = new System.Windows.Forms.TreeView();
			this.ColorContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
			this.restoreOriginalColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.underwaterTreeView = new System.Windows.Forms.TreeView();
			this.fogTreeView = new System.Windows.Forms.TreeView();
			this.colorsTab = new System.Windows.Forms.TabPage();
			this.colorTreeView = new System.Windows.Forms.TreeView();
			this.tabControl = new MetroFramework.Controls.MetroTabControl();
			this.underwaterTab = new System.Windows.Forms.TabPage();
			this.fogTab = new System.Windows.Forms.TabPage();
			this.metroPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.blueUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.greenUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.redUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.waterTab.SuspendLayout();
			this.ColorContextMenu.SuspendLayout();
			this.colorsTab.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.underwaterTab.SuspendLayout();
			this.fogTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// metroPanel1
			// 
			this.metroPanel1.Controls.Add(this.metroTextBox1);
			this.metroPanel1.Controls.Add(this.metroLabel2);
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
			// metroTextBox1
			// 
			// 
			// 
			// 
			this.metroTextBox1.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
			this.metroTextBox1.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
			this.metroTextBox1.CustomButton.Name = "";
			this.metroTextBox1.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
			this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
			this.metroTextBox1.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
			this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
			this.metroTextBox1.CustomButton.UseSelectable = true;
			this.metroTextBox1.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
			this.metroTextBox1.Lines = new string[0];
			resources.ApplyResources(this.metroTextBox1, "metroTextBox1");
			this.metroTextBox1.MaxLength = 32767;
			this.metroTextBox1.Name = "metroTextBox1";
			this.metroTextBox1.PasswordChar = '\0';
			this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.metroTextBox1.SelectedText = "";
			this.metroTextBox1.SelectionLength = 0;
			this.metroTextBox1.SelectionStart = 0;
			this.metroTextBox1.ShortcutsEnabled = true;
			this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroTextBox1.UseSelectable = true;
			this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
			this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
			this.metroTextBox1.TextChanged += new System.EventHandler(this.metroTextBox1_TextChanged);
			// 
			// metroLabel2
			// 
			resources.ApplyResources(this.metroLabel2, "metroLabel2");
			this.metroLabel2.Name = "metroLabel2";
			this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
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
			this.blueUpDown.BackColor = System.Drawing.SystemColors.Desktop;
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
			this.greenUpDown.BackColor = System.Drawing.SystemColors.Desktop;
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
			this.redUpDown.BackColor = System.Drawing.SystemColors.Desktop;
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
			this.alphaUpDown.BackColor = System.Drawing.SystemColors.Desktop;
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
			// 
			// 
			// 
			this.colorTextbox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
			this.colorTextbox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
			this.colorTextbox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
			this.colorTextbox.CustomButton.Name = "";
			this.colorTextbox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
			this.colorTextbox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
			this.colorTextbox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
			this.colorTextbox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
			this.colorTextbox.CustomButton.UseSelectable = true;
			this.colorTextbox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
			this.colorTextbox.Lines = new string[0];
			resources.ApplyResources(this.colorTextbox, "colorTextbox");
			this.colorTextbox.MaxLength = 32767;
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
            this.fileToolStripMenuItem});
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
			// waterTab
			// 
			this.waterTab.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.waterTab.Controls.Add(this.waterTreeView);
			resources.ApplyResources(this.waterTab, "waterTab");
			this.waterTab.Name = "waterTab";
			// 
			// waterTreeView
			// 
			this.waterTreeView.ContextMenuStrip = this.ColorContextMenu;
			resources.ApplyResources(this.waterTreeView, "waterTreeView");
			this.waterTreeView.Name = "waterTreeView";
			this.waterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
			this.waterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView2_KeyDown);
			// 
			// ColorContextMenu
			// 
			this.ColorContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreOriginalColorToolStripMenuItem});
			this.ColorContextMenu.Name = "ColorContextMenu";
			resources.ApplyResources(this.ColorContextMenu, "ColorContextMenu");
			// 
			// restoreOriginalColorToolStripMenuItem
			// 
			this.restoreOriginalColorToolStripMenuItem.Name = "restoreOriginalColorToolStripMenuItem";
			resources.ApplyResources(this.restoreOriginalColorToolStripMenuItem, "restoreOriginalColorToolStripMenuItem");
			this.restoreOriginalColorToolStripMenuItem.Click += new System.EventHandler(this.restoreOriginalColorToolStripMenuItem_Click);
			// 
			// underwaterTreeView
			// 
			this.underwaterTreeView.ContextMenuStrip = this.ColorContextMenu;
			resources.ApplyResources(this.underwaterTreeView, "underwaterTreeView");
			this.underwaterTreeView.Name = "underwaterTreeView";
			this.underwaterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView3_AfterSelect);
			this.underwaterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView3_KeyDown);
			// 
			// fogTreeView
			// 
			this.fogTreeView.ContextMenuStrip = this.ColorContextMenu;
			resources.ApplyResources(this.fogTreeView, "fogTreeView");
			this.fogTreeView.Name = "fogTreeView";
			this.fogTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView4_AfterSelect);
			this.fogTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView4_KeyDown);
			// 
			// colorsTab
			// 
			this.colorsTab.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.colorsTab.Controls.Add(this.colorTreeView);
			resources.ApplyResources(this.colorsTab, "colorsTab");
			this.colorsTab.Name = "colorsTab";
			// 
			// colorTreeView
			// 
			this.colorTreeView.ContextMenuStrip = this.ColorContextMenu;
			resources.ApplyResources(this.colorTreeView, "colorTreeView");
			this.colorTreeView.Name = "colorTreeView";
			this.colorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.colorTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			// 
			// tabControl
			// 
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.fogTab);
			this.tabControl.Controls.Add(this.colorsTab);
			this.tabControl.Controls.Add(this.waterTab);
			this.tabControl.Controls.Add(this.underwaterTab);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Style = MetroFramework.MetroColorStyle.White;
			this.tabControl.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.tabControl.UseSelectable = true;
			// 
			// underwaterTab
			// 
			this.underwaterTab.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.underwaterTab.Controls.Add(this.underwaterTreeView);
			resources.ApplyResources(this.underwaterTab, "underwaterTab");
			this.underwaterTab.Name = "underwaterTab";
			// 
			// fogTab
			// 
			this.fogTab.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.fogTab.Controls.Add(this.fogTreeView);
			resources.ApplyResources(this.fogTab, "fogTab");
			this.fogTab.Name = "fogTab";
			// 
			// COLEditor
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.metroPanel1);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Name = "COLEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroPanel1.ResumeLayout(false);
			this.metroPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.blueUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.greenUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.redUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.waterTab.ResumeLayout(false);
			this.ColorContextMenu.ResumeLayout(false);
			this.colorsTab.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.underwaterTab.ResumeLayout(false);
			this.fogTab.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private TreeView colorTreeView;
        private TreeView waterTreeView;
		private TreeView fogTreeView;
		private TreeView underwaterTreeView;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private MetroFramework.Controls.MetroLabel blueLabel;
        private MetroFramework.Controls.MetroLabel greenLabel;
        private MetroFramework.Controls.MetroLabel redLabel;
        private MetroFramework.Controls.MetroLabel alphaLabel;
        private System.Windows.Forms.NumericUpDown alphaUpDown;
        private TabPage waterTab;
        private TabPage colorsTab;
        private MetroFramework.Controls.MetroTabControl tabControl;
        private MetroFramework.Controls.MetroButton setColorBtn;
		private TabPage underwaterTab;
		private TabPage fogTab;
		private NumericUpDown blueUpDown;
		private NumericUpDown greenUpDown;
		private NumericUpDown redUpDown;
		private MetroFramework.Controls.MetroTextBox colorTextbox;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private MetroFramework.Controls.MetroContextMenu ColorContextMenu;
		private ToolStripMenuItem restoreOriginalColorToolStripMenuItem;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
		private MetroFramework.Controls.MetroLabel metroLabel2;
	}
}