
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
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
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
            this.waterTab = new System.Windows.Forms.TabPage();
            this.waterTreeView = new System.Windows.Forms.TreeView();
            this.ColorContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.restoreOriginalColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.underwaterTreeView = new System.Windows.Forms.TreeView();
            this.fogTreeView = new System.Windows.Forms.TreeView();
            this.colorsTab = new System.Windows.Forms.TabPage();
            this.colorTreeView = new System.Windows.Forms.TreeView();
            this.tabControl = new MetroFramework.Controls.MetroTabControl();
            this.underwaterTab = new System.Windows.Forms.TabPage();
            this.fogTab = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.crEaTiiOn_ModernSlider1 = new CBH.Controls.CrEaTiiOn_ModernSlider();
            this.crEaTiiOn_ModernSlider2 = new CBH.Controls.CrEaTiiOn_ModernSlider();
            this.crEaTiiOn_ModernSlider3 = new CBH.Controls.CrEaTiiOn_ModernSlider();
            this.crEaTiiOn_ModernSlider4 = new CBH.Controls.CrEaTiiOn_ModernSlider();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.colorBox2D1 = new MechanikaDesign.WinForms.UI.ColorPicker.ColorBox2D();
            this.colorSliderVertical1 = new MechanikaDesign.WinForms.UI.ColorPicker.ColorSliderVertical();
            this.SetColorButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl.SuspendLayout();
            this.colorsTab.SuspendLayout();
            this.ColorContextMenu.SuspendLayout();
            this.waterTab.SuspendLayout();
            this.underwaterTab.SuspendLayout();
            this.fogTab.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            this.SuspendLayout();
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
            this.metroTextBox1.Lines = new string[] {
        "Filter by searching..."};
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
            // alphaLabel
            // 
            resources.ApplyResources(this.alphaLabel, "alphaLabel");
            this.alphaLabel.Name = "alphaLabel";
            this.alphaLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.alphaLabel.UseCustomBackColor = true;
            this.alphaLabel.UseCustomForeColor = true;
            // 
            // blueLabel
            // 
            resources.ApplyResources(this.blueLabel, "blueLabel");
            this.blueLabel.Name = "blueLabel";
            this.blueLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.blueLabel.UseCustomBackColor = true;
            this.blueLabel.UseCustomForeColor = true;
            // 
            // greenLabel
            // 
            resources.ApplyResources(this.greenLabel, "greenLabel");
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.greenLabel.UseCustomBackColor = true;
            this.greenLabel.UseCustomForeColor = true;
            // 
            // redLabel
            // 
            resources.ApplyResources(this.redLabel, "redLabel");
            this.redLabel.Name = "redLabel";
            this.redLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.redLabel.UseCustomBackColor = true;
            this.redLabel.UseCustomForeColor = true;
            // 
            // colorTextbox
            // 
            // 
            // 
            // 
            this.colorTextbox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.colorTextbox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode1")));
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
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
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
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
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
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
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
            resources.ApplyResources(this.targetUpdateToolToolStripMenuItem, "targetUpdateToolToolStripMenuItem");
            this.targetUpdateToolToolStripMenuItem.Name = "targetUpdateToolToolStripMenuItem";
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
            // waterTab
            // 
            this.waterTab.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.waterTab.Controls.Add(this.waterTreeView);
            resources.ApplyResources(this.waterTab, "waterTab");
            this.waterTab.Name = "waterTab";
            // 
            // waterTreeView
            // 
            this.waterTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.waterTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waterTreeView.ContextMenuStrip = this.ColorContextMenu;
            resources.ApplyResources(this.waterTreeView, "waterTreeView");
            this.waterTreeView.ForeColor = System.Drawing.Color.White;
            this.waterTreeView.Name = "waterTreeView";
            this.waterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            this.waterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView2_KeyDown);
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
            // underwaterTreeView
            // 
            this.underwaterTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.underwaterTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.underwaterTreeView.ContextMenuStrip = this.ColorContextMenu;
            this.underwaterTreeView.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.underwaterTreeView, "underwaterTreeView");
            this.underwaterTreeView.Name = "underwaterTreeView";
            this.underwaterTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView3_AfterSelect);
            this.underwaterTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView3_KeyDown);
            // 
            // fogTreeView
            // 
            this.fogTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.fogTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fogTreeView.ContextMenuStrip = this.ColorContextMenu;
            this.fogTreeView.ForeColor = System.Drawing.Color.White;
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
            this.colorTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.colorTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorTreeView.ContextMenuStrip = this.ColorContextMenu;
            resources.ApplyResources(this.colorTreeView, "colorTreeView");
            this.colorTreeView.ForeColor = System.Drawing.Color.White;
            this.colorTreeView.Name = "colorTreeView";
            this.colorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.colorTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.colorsTab);
            this.tabControl.Controls.Add(this.waterTab);
            this.tabControl.Controls.Add(this.underwaterTab);
            this.tabControl.Controls.Add(this.fogTab);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.SetColorButton);
            this.panel1.Controls.Add(this.colorSliderVertical1);
            this.panel1.Controls.Add(this.colorBox2D1);
            this.panel1.Controls.Add(this.numericUpDown4);
            this.panel1.Controls.Add(this.numericUpDown3);
            this.panel1.Controls.Add(this.numericUpDown2);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.crEaTiiOn_ModernSlider3);
            this.panel1.Controls.Add(this.crEaTiiOn_ModernSlider4);
            this.panel1.Controls.Add(this.crEaTiiOn_ModernSlider2);
            this.panel1.Controls.Add(this.crEaTiiOn_ModernSlider1);
            this.panel1.Controls.Add(this.blueLabel);
            this.panel1.Controls.Add(this.alphaLabel);
            this.panel1.Controls.Add(this.greenLabel);
            this.panel1.Controls.Add(this.redLabel);
            this.panel1.Controls.Add(this.colorTextbox);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.metroLabel1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // crEaTiiOn_ModernSlider1
            // 
            this.crEaTiiOn_ModernSlider1.BarThickness = 4;
            this.crEaTiiOn_ModernSlider1.BigStepIncrement = 10;
            this.crEaTiiOn_ModernSlider1.Colors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("crEaTiiOn_ModernSlider1.Colors")));
            this.crEaTiiOn_ModernSlider1.CompositingQualityType = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.crEaTiiOn_ModernSlider1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crEaTiiOn_ModernSlider1.FilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(119)))), ((int)(((byte)(215)))));
            this.crEaTiiOn_ModernSlider1.InterpolationType = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.crEaTiiOn_ModernSlider1.KnobColor = System.Drawing.Color.Gray;
            this.crEaTiiOn_ModernSlider1.KnobImage = null;
            resources.ApplyResources(this.crEaTiiOn_ModernSlider1, "crEaTiiOn_ModernSlider1");
            this.crEaTiiOn_ModernSlider1.Max = 100;
            this.crEaTiiOn_ModernSlider1.Name = "crEaTiiOn_ModernSlider1";
            this.crEaTiiOn_ModernSlider1.Percentage = 50;
            this.crEaTiiOn_ModernSlider1.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.crEaTiiOn_ModernSlider1.Positions = ((System.Collections.Generic.List<float>)(resources.GetObject("crEaTiiOn_ModernSlider1.Positions")));
            this.crEaTiiOn_ModernSlider1.QuickHopping = false;
            this.crEaTiiOn_ModernSlider1.SliderStyle = CBH.Controls.CrEaTiiOn_ModernSlider.Style.Windows10;
            this.crEaTiiOn_ModernSlider1.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.crEaTiiOn_ModernSlider1.UnfilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(169)))), ((int)(((byte)(219)))));
            // 
            // crEaTiiOn_ModernSlider2
            // 
            this.crEaTiiOn_ModernSlider2.BarThickness = 4;
            this.crEaTiiOn_ModernSlider2.BigStepIncrement = 10;
            this.crEaTiiOn_ModernSlider2.Colors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("crEaTiiOn_ModernSlider2.Colors")));
            this.crEaTiiOn_ModernSlider2.CompositingQualityType = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.crEaTiiOn_ModernSlider2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crEaTiiOn_ModernSlider2.FilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(119)))), ((int)(((byte)(215)))));
            this.crEaTiiOn_ModernSlider2.InterpolationType = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.crEaTiiOn_ModernSlider2.KnobColor = System.Drawing.Color.Gray;
            this.crEaTiiOn_ModernSlider2.KnobImage = null;
            resources.ApplyResources(this.crEaTiiOn_ModernSlider2, "crEaTiiOn_ModernSlider2");
            this.crEaTiiOn_ModernSlider2.Max = 100;
            this.crEaTiiOn_ModernSlider2.Name = "crEaTiiOn_ModernSlider2";
            this.crEaTiiOn_ModernSlider2.Percentage = 50;
            this.crEaTiiOn_ModernSlider2.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.crEaTiiOn_ModernSlider2.Positions = ((System.Collections.Generic.List<float>)(resources.GetObject("crEaTiiOn_ModernSlider2.Positions")));
            this.crEaTiiOn_ModernSlider2.QuickHopping = false;
            this.crEaTiiOn_ModernSlider2.SliderStyle = CBH.Controls.CrEaTiiOn_ModernSlider.Style.Windows10;
            this.crEaTiiOn_ModernSlider2.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.crEaTiiOn_ModernSlider2.UnfilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(169)))), ((int)(((byte)(219)))));
            // 
            // crEaTiiOn_ModernSlider3
            // 
            this.crEaTiiOn_ModernSlider3.BarThickness = 4;
            this.crEaTiiOn_ModernSlider3.BigStepIncrement = 10;
            this.crEaTiiOn_ModernSlider3.Colors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("crEaTiiOn_ModernSlider3.Colors")));
            this.crEaTiiOn_ModernSlider3.CompositingQualityType = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.crEaTiiOn_ModernSlider3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crEaTiiOn_ModernSlider3.FilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(119)))), ((int)(((byte)(215)))));
            this.crEaTiiOn_ModernSlider3.InterpolationType = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.crEaTiiOn_ModernSlider3.KnobColor = System.Drawing.Color.Gray;
            this.crEaTiiOn_ModernSlider3.KnobImage = null;
            resources.ApplyResources(this.crEaTiiOn_ModernSlider3, "crEaTiiOn_ModernSlider3");
            this.crEaTiiOn_ModernSlider3.Max = 100;
            this.crEaTiiOn_ModernSlider3.Name = "crEaTiiOn_ModernSlider3";
            this.crEaTiiOn_ModernSlider3.Percentage = 50;
            this.crEaTiiOn_ModernSlider3.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.crEaTiiOn_ModernSlider3.Positions = ((System.Collections.Generic.List<float>)(resources.GetObject("crEaTiiOn_ModernSlider3.Positions")));
            this.crEaTiiOn_ModernSlider3.QuickHopping = false;
            this.crEaTiiOn_ModernSlider3.SliderStyle = CBH.Controls.CrEaTiiOn_ModernSlider.Style.Windows10;
            this.crEaTiiOn_ModernSlider3.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.crEaTiiOn_ModernSlider3.UnfilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(169)))), ((int)(((byte)(219)))));
            // 
            // crEaTiiOn_ModernSlider4
            // 
            this.crEaTiiOn_ModernSlider4.BarThickness = 4;
            this.crEaTiiOn_ModernSlider4.BigStepIncrement = 10;
            this.crEaTiiOn_ModernSlider4.Colors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("crEaTiiOn_ModernSlider4.Colors")));
            this.crEaTiiOn_ModernSlider4.CompositingQualityType = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            this.crEaTiiOn_ModernSlider4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.crEaTiiOn_ModernSlider4.FilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(119)))), ((int)(((byte)(215)))));
            this.crEaTiiOn_ModernSlider4.InterpolationType = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.crEaTiiOn_ModernSlider4.KnobColor = System.Drawing.Color.Gray;
            this.crEaTiiOn_ModernSlider4.KnobImage = null;
            resources.ApplyResources(this.crEaTiiOn_ModernSlider4, "crEaTiiOn_ModernSlider4");
            this.crEaTiiOn_ModernSlider4.Max = 100;
            this.crEaTiiOn_ModernSlider4.Name = "crEaTiiOn_ModernSlider4";
            this.crEaTiiOn_ModernSlider4.Percentage = 50;
            this.crEaTiiOn_ModernSlider4.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.crEaTiiOn_ModernSlider4.Positions = ((System.Collections.Generic.List<float>)(resources.GetObject("crEaTiiOn_ModernSlider4.Positions")));
            this.crEaTiiOn_ModernSlider4.QuickHopping = false;
            this.crEaTiiOn_ModernSlider4.SliderStyle = CBH.Controls.CrEaTiiOn_ModernSlider.Style.Windows10;
            this.crEaTiiOn_ModernSlider4.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.crEaTiiOn_ModernSlider4.UnfilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(169)))), ((int)(((byte)(219)))));
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Name = "numericUpDown1";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.numericUpDown2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown2.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.numericUpDown2, "numericUpDown2");
            this.numericUpDown2.Name = "numericUpDown2";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.numericUpDown3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown3.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.numericUpDown3, "numericUpDown3");
            this.numericUpDown3.Name = "numericUpDown3";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.numericUpDown4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown4.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.numericUpDown4, "numericUpDown4");
            this.numericUpDown4.Name = "numericUpDown4";
            // 
            // colorBox2D1
            // 
            this.colorBox2D1.ColorMode = MechanikaDesign.WinForms.UI.ColorPicker.ColorModes.Hue;
            this.colorBox2D1.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.colorBox2D1, "colorBox2D1");
            this.colorBox2D1.Name = "colorBox2D1";
            // 
            // colorSliderVertical1
            // 
            this.colorSliderVertical1.BackColor = System.Drawing.Color.Transparent;
            this.colorSliderVertical1.ColorMode = MechanikaDesign.WinForms.UI.ColorPicker.ColorModes.Hue;
            this.colorSliderVertical1.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.colorSliderVertical1, "colorSliderVertical1");
            this.colorSliderVertical1.Name = "colorSliderVertical1";
            this.colorSliderVertical1.NubColor = System.Drawing.Color.Empty;
            this.colorSliderVertical1.Position = 0;
            // 
            // SetColorButton
            // 
            this.SetColorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SetColorButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SetColorButton.BorderRadius = 10;
            this.SetColorButton.BorderSize = 1;
            this.SetColorButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SetColorButton.FlatAppearance.BorderSize = 0;
            this.SetColorButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SetColorButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            resources.ApplyResources(this.SetColorButton, "SetColorButton");
            this.SetColorButton.ForeColor = System.Drawing.Color.White;
            this.SetColorButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SetColorButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SetColorButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.SetColorButton.Name = "SetColorButton";
            this.SetColorButton.TextColor = System.Drawing.Color.White;
            this.SetColorButton.UseVisualStyleBackColor = false;
            this.SetColorButton.Click += new System.EventHandler(this.SetColorButton_Click);
            // 
            // COLEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.menuStrip);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "COLEditor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.colorsTab.ResumeLayout(false);
            this.ColorContextMenu.ResumeLayout(false);
            this.waterTab.ResumeLayout(false);
            this.underwaterTab.ResumeLayout(false);
            this.fogTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
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
		private TabPage waterTab;
		private TabPage colorsTab;
		private MetroFramework.Controls.MetroTabControl tabControl;
		private TabPage underwaterTab;
		private TabPage fogTab;
		private MetroFramework.Controls.MetroTextBox colorTextbox;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private MetroFramework.Controls.MetroContextMenu ColorContextMenu;
		private ToolStripMenuItem restoreOriginalColorToolStripMenuItem;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
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
        private Panel panel1;
        private MechanikaDesign.WinForms.UI.ColorPicker.ColorSliderVertical colorSliderVertical1;
        private MechanikaDesign.WinForms.UI.ColorPicker.ColorBox2D colorBox2D1;
        private NumericUpDown numericUpDown4;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown1;
        private CBH.Controls.CrEaTiiOn_ModernSlider crEaTiiOn_ModernSlider3;
        private CBH.Controls.CrEaTiiOn_ModernSlider crEaTiiOn_ModernSlider4;
        private CBH.Controls.CrEaTiiOn_ModernSlider crEaTiiOn_ModernSlider2;
        private CBH.Controls.CrEaTiiOn_ModernSlider crEaTiiOn_ModernSlider1;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton SetColorButton;
    }
}