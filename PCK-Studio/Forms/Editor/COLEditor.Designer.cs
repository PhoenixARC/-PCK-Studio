
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.verticalColorSlider1 = new ColorPicker.VerticalColorSlider();
            this.cBox = new ColorPicker.ColorBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.txRed = new ColorPicker.NumericTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.txGreen = new ColorPicker.NumericTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.txBlue = new ColorPicker.NumericTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txHex = new System.Windows.Forms.TextBox();
            this.txTransp = new ColorPicker.NumericTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.setColorBtn = new MetroFramework.Controls.MetroButton();
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.colorsTab = new System.Windows.Forms.TabPage();
            this.colorTreeView = new System.Windows.Forms.TreeView();
            this.ColorContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.restoreOriginalColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waterTab = new System.Windows.Forms.TabPage();
            this.waterTreeView = new System.Windows.Forms.TreeView();
            this.underwaterTab = new System.Windows.Forms.TabPage();
            this.underwaterTreeView = new System.Windows.Forms.TreeView();
            this.fogTab = new System.Windows.Forms.TabPage();
            this.fogTreeView = new System.Windows.Forms.TreeView();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.targetUpdateToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.metroPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.colorsTab.SuspendLayout();
            this.ColorContextMenu.SuspendLayout();
            this.waterTab.SuspendLayout();
            this.underwaterTab.SuspendLayout();
            this.fogTab.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.panel2);
            this.metroPanel1.Controls.Add(this.tabControl);
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
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.setColorBtn);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.verticalColorSlider1);
            this.panel1.Controls.Add(this.cBox);
            this.panel1.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // verticalColorSlider1
            // 
            this.verticalColorSlider1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.verticalColorSlider1.DrawStyle = ColorPicker.DrawStyles.Hue;
            resources.ApplyResources(this.verticalColorSlider1, "verticalColorSlider1");
            this.verticalColorSlider1.Name = "verticalColorSlider1";
            this.verticalColorSlider1.RGB = 16711680;
            // 
            // cBox
            // 
            this.cBox.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cBox.DrawStyle = ColorPicker.DrawStyles.Hue;
            resources.ApplyResources(this.cBox, "cBox");
            this.cBox.Name = "cBox";
            this.cBox.RGB = 16711680;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.panel3.Controls.Add(this.metroLabel6);
            this.panel3.Controls.Add(this.metroLabel5);
            this.panel3.Controls.Add(this.txRed);
            this.panel3.Controls.Add(this.metroLabel4);
            this.panel3.Controls.Add(this.txGreen);
            this.panel3.Controls.Add(this.metroLabel3);
            this.panel3.Controls.Add(this.txBlue);
            this.panel3.Controls.Add(this.metroLabel1);
            this.panel3.Controls.Add(this.txHex);
            this.panel3.Controls.Add(this.txTransp);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // metroLabel6
            // 
            resources.ApplyResources(this.metroLabel6, "metroLabel6");
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel6.UseCustomBackColor = true;
            this.metroLabel6.UseCustomForeColor = true;
            // 
            // metroLabel5
            // 
            resources.ApplyResources(this.metroLabel5, "metroLabel5");
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel5.UseCustomBackColor = true;
            this.metroLabel5.UseCustomForeColor = true;
            // 
            // txRed
            // 
            this.txRed.AllowDecimal = false;
            this.txRed.AllowNull = false;
            this.txRed.AllowSign = false;
            this.txRed.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txRed.DoubleValue = 0D;
            this.txRed.Format = "";
            this.txRed.Int32Value = 0;
            resources.ApplyResources(this.txRed, "txRed");
            this.txRed.Name = "txRed";
            this.txRed.UInt32Value = ((uint)(0u));
            // 
            // metroLabel4
            // 
            resources.ApplyResources(this.metroLabel4, "metroLabel4");
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel4.UseCustomBackColor = true;
            this.metroLabel4.UseCustomForeColor = true;
            // 
            // txGreen
            // 
            this.txGreen.AllowDecimal = false;
            this.txGreen.AllowNull = false;
            this.txGreen.AllowSign = false;
            this.txGreen.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txGreen.DoubleValue = 0D;
            this.txGreen.Format = "";
            this.txGreen.Int32Value = 0;
            resources.ApplyResources(this.txGreen, "txGreen");
            this.txGreen.Name = "txGreen";
            this.txGreen.UInt32Value = ((uint)(0u));
            // 
            // metroLabel3
            // 
            resources.ApplyResources(this.metroLabel3, "metroLabel3");
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel3.UseCustomBackColor = true;
            this.metroLabel3.UseCustomForeColor = true;
            // 
            // txBlue
            // 
            this.txBlue.AllowDecimal = false;
            this.txBlue.AllowNull = false;
            this.txBlue.AllowSign = false;
            this.txBlue.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txBlue.DoubleValue = 0D;
            this.txBlue.Format = "";
            this.txBlue.Int32Value = 0;
            resources.ApplyResources(this.txBlue, "txBlue");
            this.txBlue.Name = "txBlue";
            this.txBlue.UInt32Value = ((uint)(0u));
            // 
            // metroLabel1
            // 
            resources.ApplyResources(this.metroLabel1, "metroLabel1");
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // txHex
            // 
            resources.ApplyResources(this.txHex, "txHex");
            this.txHex.Name = "txHex";
            // 
            // txTransp
            // 
            this.txTransp.AllowDecimal = false;
            this.txTransp.AllowNull = false;
            this.txTransp.AllowSign = false;
            this.txTransp.DecimalValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txTransp.DoubleValue = 0D;
            this.txTransp.Format = "";
            this.txTransp.Int32Value = 0;
            resources.ApplyResources(this.txTransp, "txTransp");
            this.txTransp.Name = "txTransp";
            this.txTransp.UInt32Value = ((uint)(0u));
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // setColorBtn
            // 
            resources.ApplyResources(this.setColorBtn, "setColorBtn");
            this.setColorBtn.Name = "setColorBtn";
            this.setColorBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.setColorBtn.UseSelectable = true;
            this.setColorBtn.Click += new System.EventHandler(this.setColorBtn_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.colorsTab);
            this.tabControl.Controls.Add(this.waterTab);
            this.tabControl.Controls.Add(this.underwaterTab);
            this.tabControl.Controls.Add(this.fogTab);
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Style = MetroFramework.MetroColorStyle.White;
            this.tabControl.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tabControl.UseSelectable = true;
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
            this.colorTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.colorTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorTreeView.ContextMenuStrip = this.ColorContextMenu;
            resources.ApplyResources(this.colorTreeView, "colorTreeView");
            this.colorTreeView.ForeColor = System.Drawing.Color.White;
            this.colorTreeView.Name = "colorTreeView";
            this.colorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.colorTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // ColorContextMenu
            // 
            this.ColorContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreOriginalColorToolStripMenuItem,
            this.copyColorToolStripMenuItem,
            this.pasteColorToolStripMenuItem});
            this.ColorContextMenu.Name = "ColorContextMenu";
            resources.ApplyResources(this.ColorContextMenu, "ColorContextMenu");
            // 
            // restoreOriginalColorToolStripMenuItem
            // 
            this.restoreOriginalColorToolStripMenuItem.Name = "restoreOriginalColorToolStripMenuItem";
            resources.ApplyResources(this.restoreOriginalColorToolStripMenuItem, "restoreOriginalColorToolStripMenuItem");
            this.restoreOriginalColorToolStripMenuItem.Click += new System.EventHandler(this.restoreOriginalColorToolStripMenuItem_Click);
            // 
            // copyColorToolStripMenuItem
            // 
            this.copyColorToolStripMenuItem.Name = "copyColorToolStripMenuItem";
            resources.ApplyResources(this.copyColorToolStripMenuItem, "copyColorToolStripMenuItem");
            this.copyColorToolStripMenuItem.Click += new System.EventHandler(this.copyColorToolStripMenuItem_Click);
            // 
            // pasteColorToolStripMenuItem
            // 
            this.pasteColorToolStripMenuItem.Name = "pasteColorToolStripMenuItem";
            resources.ApplyResources(this.pasteColorToolStripMenuItem, "pasteColorToolStripMenuItem");
            this.pasteColorToolStripMenuItem.Click += new System.EventHandler(this.pasteColorToolStripMenuItem_Click);
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
            this.waterTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.waterTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waterTreeView.ContextMenuStrip = this.ColorContextMenu;
            resources.ApplyResources(this.waterTreeView, "waterTreeView");
            this.waterTreeView.ForeColor = System.Drawing.Color.White;
            this.waterTreeView.Name = "waterTreeView";
            this.waterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            this.waterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView2_KeyDown);
            // 
            // underwaterTab
            // 
            this.underwaterTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.underwaterTab.Controls.Add(this.underwaterTreeView);
            resources.ApplyResources(this.underwaterTab, "underwaterTab");
            this.underwaterTab.Name = "underwaterTab";
            // 
            // underwaterTreeView
            // 
            this.underwaterTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.underwaterTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.underwaterTreeView.ContextMenuStrip = this.ColorContextMenu;
            this.underwaterTreeView.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.underwaterTreeView, "underwaterTreeView");
            this.underwaterTreeView.Name = "underwaterTreeView";
            this.underwaterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView3_AfterSelect);
            this.underwaterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView3_KeyDown);
            // 
            // fogTab
            // 
            this.fogTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.fogTab.Controls.Add(this.fogTreeView);
            resources.ApplyResources(this.fogTab, "fogTab");
            this.fogTab.Name = "fogTab";
            // 
            // fogTreeView
            // 
            this.fogTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.fogTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fogTreeView.ContextMenuStrip = this.ColorContextMenu;
            this.fogTreeView.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.fogTreeView, "fogTreeView");
            this.fogTreeView.Name = "fogTreeView";
            this.fogTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView4_AfterSelect);
            this.fogTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView4_KeyDown);
            // 
            // metroTextBox1
            // 
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.metroTextBox1.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
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
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.targetUpdateToolToolStripMenuItem});
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
            // targetUpdateToolToolStripMenuItem
            // 
            this.targetUpdateToolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.targetUpdateToolToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.targetUpdateToolToolStripMenuItem.Name = "targetUpdateToolToolStripMenuItem";
            resources.ApplyResources(this.targetUpdateToolToolStripMenuItem, "targetUpdateToolToolStripMenuItem");
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
            // COLEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.menuStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "COLEditor";
            this.metroPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.colorsTab.ResumeLayout(false);
            this.ColorContextMenu.ResumeLayout(false);
            this.waterTab.ResumeLayout(false);
            this.underwaterTab.ResumeLayout(false);
            this.fogTab.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
		private TabPage waterTab;
		private TabPage colorsTab;
		private MetroFramework.Controls.MetroTabControl tabControl;
		private MetroFramework.Controls.MetroButton setColorBtn;
		private TabPage underwaterTab;
		private TabPage fogTab;
		private MetroFramework.Controls.MetroContextMenu ColorContextMenu;
		private ToolStripMenuItem restoreOriginalColorToolStripMenuItem;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
		private MetroFramework.Controls.MetroLabel metroLabel2;
		private ToolStripMenuItem targetUpdateToolToolStripMenuItem;
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
		private ToolStripMenuItem copyColorToolStripMenuItem;
		private ToolStripMenuItem pasteColorToolStripMenuItem;
        private Panel panel2;
        private Panel panel1;
        private Panel panel3;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private ColorPicker.NumericTextBox txRed;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private ColorPicker.NumericTextBox txGreen;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private ColorPicker.NumericTextBox txBlue;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private TextBox txHex;
        private ColorPicker.NumericTextBox txTransp;
        private ColorPicker.ColorBox cBox;
        private ColorPicker.VerticalColorSlider verticalColorSlider1;
    }
}