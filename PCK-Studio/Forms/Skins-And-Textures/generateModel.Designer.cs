﻿namespace PckStudio
{
    partial class generateModel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(generateModel));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buttonDone = new System.Windows.Forms.Button();
			this.labelView = new System.Windows.Forms.Label();
			this.rotateRightBtn = new System.Windows.Forms.Button();
			this.rotateLeftBtn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tabBody = new System.Windows.Forms.TabControl();
			this.tabArmor = new System.Windows.Forms.TabPage();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.myTablePanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.offsetArms = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.offsetBody = new System.Windows.Forms.TextBox();
			this.offsetLegs = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.offsetHead = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboParent = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonEXPORT = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.buttonIMPORT = new System.Windows.Forms.Button();
			this.texturePreview = new System.Windows.Forms.PictureBox();
			this.labelTextureMappingPreview = new System.Windows.Forms.Label();
			this.buttonImportModel = new System.Windows.Forms.Button();
			this.displayBox = new System.Windows.Forms.PictureBox();
			this.buttonExportModel = new System.Windows.Forms.Button();
			this.OpenJSONButton = new System.Windows.Forms.Button();
			this.buttonTemplate = new MetroFramework.Controls.MetroButton();
			this.checkTextureGenerate = new MetroFramework.Controls.MetroCheckBox();
			this.checkGuide = new MetroFramework.Controls.MetroCheckBox();
			this.checkBoxArmor = new MetroFramework.Controls.MetroCheckBox();
			this.SizeXUpDown = new System.Windows.Forms.NumericUpDown();
			this.SizeYUpDown = new System.Windows.Forms.NumericUpDown();
			this.SizeZUpDown = new System.Windows.Forms.NumericUpDown();
			this.TextureXUpDown = new System.Windows.Forms.NumericUpDown();
			this.TextureYUpDown = new System.Windows.Forms.NumericUpDown();
			this.PosZUpDown = new System.Windows.Forms.NumericUpDown();
			this.PosYUpDown = new System.Windows.Forms.NumericUpDown();
			this.PosXUpDown = new System.Windows.Forms.NumericUpDown();
			this.Part = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.X = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Y = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Z = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._Width = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._Height = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Length = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.U = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.V = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listViewBoxes = new System.Windows.Forms.ListView();
			this.contextMenuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabBody.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.myTablePanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.texturePreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.displayBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TextureXUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.TextureYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).BeginInit();
			this.SuspendLayout();
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
			this.changeColorToolStripMenuItem.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
			// 
			// buttonDone
			// 
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonDone, "buttonDone");
			this.buttonDone.ForeColor = System.Drawing.Color.White;
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.UseVisualStyleBackColor = true;
			this.buttonDone.Click += new System.EventHandler(this.buttonDone_Click);
			// 
			// labelView
			// 
			resources.ApplyResources(this.labelView, "labelView");
			this.labelView.ForeColor = System.Drawing.Color.White;
			this.labelView.Name = "labelView";
			// 
			// rotateRightBtn
			// 
			resources.ApplyResources(this.rotateRightBtn, "rotateRightBtn");
			this.rotateRightBtn.ForeColor = System.Drawing.Color.White;
			this.rotateRightBtn.Name = "rotateRightBtn";
			this.rotateRightBtn.UseVisualStyleBackColor = true;
			this.rotateRightBtn.Click += new System.EventHandler(this.rotateRightBtn_Click);
			// 
			// rotateLeftBtn
			// 
			resources.ApplyResources(this.rotateLeftBtn, "rotateLeftBtn");
			this.rotateLeftBtn.ForeColor = System.Drawing.Color.White;
			this.rotateLeftBtn.Name = "rotateLeftBtn";
			this.rotateLeftBtn.UseVisualStyleBackColor = true;
			this.rotateLeftBtn.Click += new System.EventHandler(this.rotateLeftBtn_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tabBody);
			this.groupBox1.ForeColor = System.Drawing.Color.White;
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// tabBody
			// 
			this.tabBody.Controls.Add(this.tabArmor);
			this.tabBody.Controls.Add(this.tabPage1);
			resources.ApplyResources(this.tabBody, "tabBody");
			this.tabBody.Name = "tabBody";
			this.tabBody.SelectedIndex = 0;
			// 
			// tabArmor
			// 
			this.tabArmor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			resources.ApplyResources(this.tabArmor, "tabArmor");
			this.tabArmor.Name = "tabArmor";
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.tabPage1.Controls.Add(this.myTablePanel2);
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			// 
			// myTablePanel2
			// 
			resources.ApplyResources(this.myTablePanel2, "myTablePanel2");
			this.myTablePanel2.Controls.Add(this.offsetArms, 1, 3);
			this.myTablePanel2.Controls.Add(this.label14, 0, 3);
			this.myTablePanel2.Controls.Add(this.offsetBody, 1, 1);
			this.myTablePanel2.Controls.Add(this.offsetLegs, 1, 2);
			this.myTablePanel2.Controls.Add(this.label10, 0, 0);
			this.myTablePanel2.Controls.Add(this.label13, 0, 2);
			this.myTablePanel2.Controls.Add(this.offsetHead, 1, 0);
			this.myTablePanel2.Controls.Add(this.label12, 0, 1);
			this.myTablePanel2.Name = "myTablePanel2";
			// 
			// offsetArms
			// 
			resources.ApplyResources(this.offsetArms, "offsetArms");
			this.offsetArms.Name = "offsetArms";
			this.offsetArms.TextChanged += new System.EventHandler(this.offsetHead_TextChanged);
			// 
			// label14
			// 
			resources.ApplyResources(this.label14, "label14");
			this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.label14.Name = "label14";
			// 
			// offsetBody
			// 
			resources.ApplyResources(this.offsetBody, "offsetBody");
			this.offsetBody.Name = "offsetBody";
			this.offsetBody.TextChanged += new System.EventHandler(this.offsetHead_TextChanged);
			// 
			// offsetLegs
			// 
			resources.ApplyResources(this.offsetLegs, "offsetLegs");
			this.offsetLegs.Name = "offsetLegs";
			this.offsetLegs.TextChanged += new System.EventHandler(this.offsetHead_TextChanged);
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.label10.Name = "label10";
			// 
			// label13
			// 
			resources.ApplyResources(this.label13, "label13");
			this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.label13.Name = "label13";
			// 
			// offsetHead
			// 
			resources.ApplyResources(this.offsetHead, "offsetHead");
			this.offsetHead.Name = "offsetHead";
			this.offsetHead.TextChanged += new System.EventHandler(this.offsetHead_TextChanged);
			// 
			// label12
			// 
			resources.ApplyResources(this.label12, "label12");
			this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.label12.Name = "label12";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = System.Drawing.Color.White;
			this.label6.Name = "label6";
			// 
			// comboParent
			// 
			this.comboParent.FormattingEnabled = true;
			this.comboParent.Items.AddRange(new object[] {
            resources.GetString("comboParent.Items"),
            resources.GetString("comboParent.Items1"),
            resources.GetString("comboParent.Items2"),
            resources.GetString("comboParent.Items3"),
            resources.GetString("comboParent.Items4"),
            resources.GetString("comboParent.Items5")});
			resources.ApplyResources(this.comboParent, "comboParent");
			this.comboParent.Name = "comboParent";
			this.comboParent.SelectedValueChanged += new System.EventHandler(this.comboParent_SelectedIndexChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Name = "label5";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Name = "label3";
			// 
			// buttonEXPORT
			// 
			resources.ApplyResources(this.buttonEXPORT, "buttonEXPORT");
			this.buttonEXPORT.ForeColor = System.Drawing.Color.White;
			this.buttonEXPORT.Name = "buttonEXPORT";
			this.buttonEXPORT.UseVisualStyleBackColor = true;
			this.buttonEXPORT.Click += new System.EventHandler(this.buttonEXPORT_Click);
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.ForeColor = System.Drawing.Color.White;
			this.label7.Name = "label7";
			// 
			// buttonIMPORT
			// 
			resources.ApplyResources(this.buttonIMPORT, "buttonIMPORT");
			this.buttonIMPORT.ForeColor = System.Drawing.Color.White;
			this.buttonIMPORT.Name = "buttonIMPORT";
			this.buttonIMPORT.UseVisualStyleBackColor = true;
			this.buttonIMPORT.Click += new System.EventHandler(this.buttonIMPORT_Click);
			// 
			// texturePreview
			// 
			this.texturePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.texturePreview, "texturePreview");
			this.texturePreview.Name = "texturePreview";
			this.texturePreview.TabStop = false;
			// 
			// labelTextureMappingPreview
			// 
			resources.ApplyResources(this.labelTextureMappingPreview, "labelTextureMappingPreview");
			this.labelTextureMappingPreview.ForeColor = System.Drawing.Color.White;
			this.labelTextureMappingPreview.Name = "labelTextureMappingPreview";
			// 
			// buttonImportModel
			// 
			resources.ApplyResources(this.buttonImportModel, "buttonImportModel");
			this.buttonImportModel.ForeColor = System.Drawing.Color.White;
			this.buttonImportModel.Name = "buttonImportModel";
			this.buttonImportModel.UseVisualStyleBackColor = true;
			this.buttonImportModel.Click += new System.EventHandler(this.buttonImportModel_Click);
			// 
			// displayBox
			// 
			this.displayBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.displayBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.displayBox, "displayBox");
			this.displayBox.Name = "displayBox";
			this.displayBox.TabStop = false;
			// 
			// buttonExportModel
			// 
			resources.ApplyResources(this.buttonExportModel, "buttonExportModel");
			this.buttonExportModel.ForeColor = System.Drawing.Color.White;
			this.buttonExportModel.Name = "buttonExportModel";
			this.buttonExportModel.UseVisualStyleBackColor = true;
			this.buttonExportModel.Click += new System.EventHandler(this.buttonExportModel_Click);
			// 
			// OpenJSONButton
			// 
			resources.ApplyResources(this.OpenJSONButton, "OpenJSONButton");
			this.OpenJSONButton.ForeColor = System.Drawing.Color.White;
			this.OpenJSONButton.Name = "OpenJSONButton";
			this.OpenJSONButton.UseVisualStyleBackColor = true;
			this.OpenJSONButton.Click += new System.EventHandler(this.OpenJSONButton_Click);
			// 
			// buttonTemplate
			// 
			resources.ApplyResources(this.buttonTemplate, "buttonTemplate");
			this.buttonTemplate.Name = "buttonTemplate";
			this.buttonTemplate.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.buttonTemplate.UseSelectable = true;
			this.buttonTemplate.Click += new System.EventHandler(this.buttonTemplate_Click);
			// 
			// checkTextureGenerate
			// 
			resources.ApplyResources(this.checkTextureGenerate, "checkTextureGenerate");
			this.checkTextureGenerate.Checked = true;
			this.checkTextureGenerate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkTextureGenerate.Name = "checkTextureGenerate";
			this.checkTextureGenerate.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.checkTextureGenerate.UseSelectable = true;
			// 
			// checkGuide
			// 
			resources.ApplyResources(this.checkGuide, "checkGuide");
			this.checkGuide.Name = "checkGuide";
			this.checkGuide.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.checkGuide.UseSelectable = true;
			this.checkGuide.CheckedChanged += new System.EventHandler(this.render);
			// 
			// checkBoxArmor
			// 
			resources.ApplyResources(this.checkBoxArmor, "checkBoxArmor");
			this.checkBoxArmor.Name = "checkBoxArmor";
			this.checkBoxArmor.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.checkBoxArmor.UseSelectable = true;
			this.checkBoxArmor.CheckedChanged += new System.EventHandler(this.render);
			// 
			// SizeXUpDown
			// 
			this.SizeXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeXUpDown.DecimalPlaces = 1;
			this.SizeXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.SizeXUpDown, "SizeXUpDown");
			this.SizeXUpDown.Name = "SizeXUpDown";
			this.SizeXUpDown.ValueChanged += new System.EventHandler(this.SizeXUpDown_ValueChanged);
			// 
			// SizeYUpDown
			// 
			this.SizeYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeYUpDown.DecimalPlaces = 1;
			this.SizeYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.SizeYUpDown, "SizeYUpDown");
			this.SizeYUpDown.Name = "SizeYUpDown";
			this.SizeYUpDown.ValueChanged += new System.EventHandler(this.SizeYUpDown_ValueChanged);
			// 
			// SizeZUpDown
			// 
			this.SizeZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeZUpDown.DecimalPlaces = 1;
			this.SizeZUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.SizeZUpDown, "SizeZUpDown");
			this.SizeZUpDown.Name = "SizeZUpDown";
			this.SizeZUpDown.ValueChanged += new System.EventHandler(this.SizeZUpDown_ValueChanged);
			// 
			// TextureXUpDown
			// 
			this.TextureXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.TextureXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.TextureXUpDown, "TextureXUpDown");
			this.TextureXUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
			this.TextureXUpDown.Name = "TextureXUpDown";
			this.TextureXUpDown.ValueChanged += new System.EventHandler(this.TextureXUpDown_ValueChanged);
			// 
			// TextureYUpDown
			// 
			this.TextureYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.TextureYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.TextureYUpDown, "TextureYUpDown");
			this.TextureYUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
			this.TextureYUpDown.Name = "TextureYUpDown";
			this.TextureYUpDown.ValueChanged += new System.EventHandler(this.TextureYUpDown_ValueChanged);
			// 
			// PosZUpDown
			// 
			this.PosZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosZUpDown.DecimalPlaces = 1;
			this.PosZUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.PosZUpDown, "PosZUpDown");
			this.PosZUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.PosZUpDown.Name = "PosZUpDown";
			this.PosZUpDown.ValueChanged += new System.EventHandler(this.PosZUpDown_ValueChanged);
			// 
			// PosYUpDown
			// 
			this.PosYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosYUpDown.DecimalPlaces = 1;
			this.PosYUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.PosYUpDown, "PosYUpDown");
			this.PosYUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.PosYUpDown.Name = "PosYUpDown";
			this.PosYUpDown.ValueChanged += new System.EventHandler(this.PosYUpDown_ValueChanged);
			// 
			// PosXUpDown
			// 
			this.PosXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosXUpDown.DecimalPlaces = 1;
			this.PosXUpDown.ForeColor = System.Drawing.SystemColors.Menu;
			resources.ApplyResources(this.PosXUpDown, "PosXUpDown");
			this.PosXUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.PosXUpDown.Name = "PosXUpDown";
			this.PosXUpDown.ValueChanged += new System.EventHandler(this.PosXUpDown_ValueChanged);
			// 
			// Part
			// 
			resources.ApplyResources(this.Part, "Part");
			// 
			// X
			// 
			resources.ApplyResources(this.X, "X");
			// 
			// Y
			// 
			resources.ApplyResources(this.Y, "Y");
			// 
			// Z
			// 
			resources.ApplyResources(this.Z, "Z");
			// 
			// Width
			// 
			resources.ApplyResources(this._Width, "Width");
			// 
			// Height
			// 
			resources.ApplyResources(this._Height, "Height");
			// 
			// Length
			// 
			resources.ApplyResources(this.Length, "Length");
			// 
			// U
			// 
			resources.ApplyResources(this.U, "U");
			// 
			// V
			// 
			resources.ApplyResources(this.V, "V");
			// 
			// listViewBoxes
			// 
			this.listViewBoxes.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listViewBoxes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Part,
            this.X,
            this.Y,
            this.Z,
            this._Width,
            this._Height,
            this.Length,
            this.U,
            this.V});
			this.listViewBoxes.ContextMenuStrip = this.contextMenuStrip1;
			this.listViewBoxes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.listViewBoxes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewBoxes.HideSelection = false;
			this.listViewBoxes.LabelEdit = true;
			resources.ApplyResources(this.listViewBoxes, "listViewBoxes");
			this.listViewBoxes.MultiSelect = false;
			this.listViewBoxes.Name = "listViewBoxes";
			this.listViewBoxes.UseCompatibleStateImageBehavior = false;
			this.listViewBoxes.View = System.Windows.Forms.View.Details;
			this.listViewBoxes.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listViewBoxes.Click += new System.EventHandler(this.listView1_Click);
			this.listViewBoxes.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			this.listViewBoxes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.delStuffUsingDelKey);
			// 
			// generateModel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.PosZUpDown);
			this.Controls.Add(this.PosYUpDown);
			this.Controls.Add(this.PosXUpDown);
			this.Controls.Add(this.TextureYUpDown);
			this.Controls.Add(this.TextureXUpDown);
			this.Controls.Add(this.SizeZUpDown);
			this.Controls.Add(this.SizeYUpDown);
			this.Controls.Add(this.SizeXUpDown);
			this.Controls.Add(this.checkBoxArmor);
			this.Controls.Add(this.checkGuide);
			this.Controls.Add(this.checkTextureGenerate);
			this.Controls.Add(this.buttonTemplate);
			this.Controls.Add(this.displayBox);
			this.Controls.Add(this.listViewBoxes);
			this.Controls.Add(this.OpenJSONButton);
			this.Controls.Add(this.buttonExportModel);
			this.Controls.Add(this.buttonImportModel);
			this.Controls.Add(this.buttonEXPORT);
			this.Controls.Add(this.labelTextureMappingPreview);
			this.Controls.Add(this.texturePreview);
			this.Controls.Add(this.comboParent);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.buttonIMPORT);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.rotateLeftBtn);
			this.Controls.Add(this.rotateRightBtn);
			this.Controls.Add(this.labelView);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label5);
			this.MaximizeBox = false;
			this.Name = "generateModel";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.generateModel_FormClosing);
			this.Load += new System.EventHandler(this.generateModel_Load);
			this.SizeChanged += new System.EventHandler(this.generateModel_SizeChanged);
			this.contextMenuStrip1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tabBody.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.myTablePanel2.ResumeLayout(false);
			this.myTablePanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.texturePreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.displayBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TextureXUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.TextureYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.Button OpenJSONButton;
        private System.Windows.Forms.Button buttonExportModel;
        private System.Windows.Forms.PictureBox displayBox;
        private System.Windows.Forms.Button buttonImportModel;
        private System.Windows.Forms.Label labelTextureMappingPreview;
        private System.Windows.Forms.PictureBox texturePreview;
        private System.Windows.Forms.Button buttonIMPORT;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonEXPORT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboParent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabBody;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel myTablePanel2;
        private System.Windows.Forms.TextBox offsetArms;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox offsetBody;
        private System.Windows.Forms.TextBox offsetLegs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox offsetHead;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabPage tabArmor;
        private System.Windows.Forms.Button rotateLeftBtn;
        private System.Windows.Forms.Button rotateRightBtn;
        private System.Windows.Forms.Label labelView;
        private MetroFramework.Controls.MetroButton buttonTemplate;
        private MetroFramework.Controls.MetroCheckBox checkTextureGenerate;
        private MetroFramework.Controls.MetroCheckBox checkGuide;
        private MetroFramework.Controls.MetroCheckBox checkBoxArmor;
        private System.Windows.Forms.NumericUpDown SizeXUpDown;
        private System.Windows.Forms.NumericUpDown SizeYUpDown;
        private System.Windows.Forms.NumericUpDown SizeZUpDown;
        private System.Windows.Forms.NumericUpDown TextureXUpDown;
        private System.Windows.Forms.NumericUpDown TextureYUpDown;
        private System.Windows.Forms.NumericUpDown PosZUpDown;
        private System.Windows.Forms.NumericUpDown PosYUpDown;
        private System.Windows.Forms.NumericUpDown PosXUpDown;
        private System.Windows.Forms.ColumnHeader Part;
        private System.Windows.Forms.ColumnHeader X;
        private System.Windows.Forms.ColumnHeader Y;
        private System.Windows.Forms.ColumnHeader Z;
        private System.Windows.Forms.ColumnHeader _Width;
        private System.Windows.Forms.ColumnHeader _Height;
        private System.Windows.Forms.ColumnHeader Length;
        private System.Windows.Forms.ColumnHeader U;
        private System.Windows.Forms.ColumnHeader V;
        private System.Windows.Forms.ListView listViewBoxes;
    }
}