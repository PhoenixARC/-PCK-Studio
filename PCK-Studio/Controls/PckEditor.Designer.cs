﻿using System.Windows.Forms;

namespace PckStudio.Controls
{
    partial class PckEditor
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.PictureBox logoPictureBox;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PckEditor));
            this.pckFileLabel = new MetroFramework.Controls.MetroLabel();
            this.labelImageSize = new MetroFramework.Controls.MetroLabel();
            this.fileEntryCountLabel = new MetroFramework.Controls.MetroLabel();
            this.PropertiesTabControl = new MetroFramework.Controls.MetroTabControl();
            this.MetaTab = new MetroFramework.Controls.MetroTabPage();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.entryTypeTextBox = new MetroFramework.Controls.MetroTextBox();
            this.entryDataTextBox = new MetroFramework.Controls.MetroTextBox();
            this.buttonEdit = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.treeMeta = new System.Windows.Forms.TreeView();
            this.contextMenuMetaTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addBOXEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addANIMEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addMultipleEntriesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAllEntriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label11 = new MetroFramework.Controls.MetroLabel();
            this.treeViewMain = new System.Windows.Forms.TreeView();
            this.contextMenuPCKEntries = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAnimatedTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audiopckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colourscolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateSkinsPCKToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.behavioursbinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entityMaterialsbinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSkinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importExtractedSkinsFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.as3DSTextureFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFileTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.capeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languagesFileLOCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameRulesFileGRFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioPCKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coloursCOLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameRulesHeaderGRHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinsPCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelsFileBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.behavioursFileBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entityMaterialsFileBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.miscFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateMipMapTextureToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFileInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.correctSkinDecimalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSubPCKEndiannessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigEndianXbox360PS3WiiUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.littleEndianPS4PSVitaSwitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setModelContainerFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.version1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.version2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.version3114ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cloneFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.addMultipleEntriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LittleEndianCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.previewPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            logoPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(logoPictureBox)).BeginInit();
            this.PropertiesTabControl.SuspendLayout();
            this.MetaTab.SuspendLayout();
            this.contextMenuMetaTree.SuspendLayout();
            this.contextMenuPCKEntries.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // logoPictureBox
            // 
            resources.ApplyResources(logoPictureBox, "logoPictureBox");
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.TabStop = false;
            // 
            // pckFileLabel
            // 
            this.pckFileLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.pckFileLabel, "pckFileLabel");
            this.pckFileLabel.Name = "pckFileLabel";
            this.pckFileLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // labelImageSize
            // 
            resources.ApplyResources(this.labelImageSize, "labelImageSize");
            this.labelImageSize.Name = "labelImageSize";
            this.labelImageSize.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // fileEntryCountLabel
            // 
            resources.ApplyResources(this.fileEntryCountLabel, "fileEntryCountLabel");
            this.fileEntryCountLabel.Name = "fileEntryCountLabel";
            this.fileEntryCountLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // PropertiesTabControl
            // 
            this.PropertiesTabControl.Controls.Add(this.MetaTab);
            resources.ApplyResources(this.PropertiesTabControl, "PropertiesTabControl");
            this.PropertiesTabControl.Name = "PropertiesTabControl";
            this.PropertiesTabControl.SelectedIndex = 0;
            this.PropertiesTabControl.Style = MetroFramework.MetroColorStyle.Silver;
            this.PropertiesTabControl.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PropertiesTabControl.UseSelectable = true;
            // 
            // MetaTab
            // 
            this.MetaTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MetaTab.Controls.Add(this.metroLabel2);
            this.MetaTab.Controls.Add(this.entryTypeTextBox);
            this.MetaTab.Controls.Add(this.entryDataTextBox);
            this.MetaTab.Controls.Add(this.buttonEdit);
            this.MetaTab.Controls.Add(this.metroLabel1);
            this.MetaTab.Controls.Add(this.treeMeta);
            this.MetaTab.HorizontalScrollbarBarColor = true;
            this.MetaTab.HorizontalScrollbarHighlightOnWheel = false;
            this.MetaTab.HorizontalScrollbarSize = 10;
            resources.ApplyResources(this.MetaTab, "MetaTab");
            this.MetaTab.Name = "MetaTab";
            this.MetaTab.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.MetaTab.VerticalScrollbarBarColor = true;
            this.MetaTab.VerticalScrollbarHighlightOnWheel = false;
            this.MetaTab.VerticalScrollbarSize = 10;
            // 
            // metroLabel2
            // 
            resources.ApplyResources(this.metroLabel2, "metroLabel2");
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // entryTypeTextBox
            // 
            resources.ApplyResources(this.entryTypeTextBox, "entryTypeTextBox");
            // 
            // 
            // 
            this.entryTypeTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.entryTypeTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.entryTypeTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.entryTypeTextBox.CustomButton.Name = "";
            this.entryTypeTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.entryTypeTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.entryTypeTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.entryTypeTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.entryTypeTextBox.CustomButton.UseSelectable = true;
            this.entryTypeTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.entryTypeTextBox.Lines = new string[0];
            this.entryTypeTextBox.MaxLength = 32767;
            this.entryTypeTextBox.Name = "entryTypeTextBox";
            this.entryTypeTextBox.PasswordChar = '\0';
            this.entryTypeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.entryTypeTextBox.SelectedText = "";
            this.entryTypeTextBox.SelectionLength = 0;
            this.entryTypeTextBox.SelectionStart = 0;
            this.entryTypeTextBox.ShortcutsEnabled = true;
            this.entryTypeTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.entryTypeTextBox.UseSelectable = true;
            this.entryTypeTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.entryTypeTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // entryDataTextBox
            // 
            resources.ApplyResources(this.entryDataTextBox, "entryDataTextBox");
            // 
            // 
            // 
            this.entryDataTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.entryDataTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode1")));
            this.entryDataTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
            this.entryDataTextBox.CustomButton.Name = "";
            this.entryDataTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
            this.entryDataTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.entryDataTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
            this.entryDataTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.entryDataTextBox.CustomButton.UseSelectable = true;
            this.entryDataTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
            this.entryDataTextBox.Lines = new string[0];
            this.entryDataTextBox.MaxLength = 32767;
            this.entryDataTextBox.Name = "entryDataTextBox";
            this.entryDataTextBox.PasswordChar = '\0';
            this.entryDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.entryDataTextBox.SelectedText = "";
            this.entryDataTextBox.SelectionLength = 0;
            this.entryDataTextBox.SelectionStart = 0;
            this.entryDataTextBox.ShortcutsEnabled = true;
            this.entryDataTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.entryDataTextBox.UseSelectable = true;
            this.entryDataTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.entryDataTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonEdit.UseSelectable = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // metroLabel1
            // 
            resources.ApplyResources(this.metroLabel1, "metroLabel1");
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // treeMeta
            // 
            this.treeMeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
            this.treeMeta.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeMeta.ContextMenuStrip = this.contextMenuMetaTree;
            resources.ApplyResources(this.treeMeta, "treeMeta");
            this.treeMeta.ForeColor = System.Drawing.SystemColors.Window;
            this.treeMeta.Name = "treeMeta";
            this.treeMeta.PathSeparator = "/";
            this.treeMeta.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMeta_AfterSelect);
            this.treeMeta.DoubleClick += new System.EventHandler(this.treeMeta_DoubleClick);
            this.treeMeta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeMeta_KeyDown);
            // 
            // contextMenuMetaTree
            // 
            this.contextMenuMetaTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem,
            this.addMultipleEntriesToolStripMenuItem1,
            this.deleteEntryToolStripMenuItem,
            this.editAllEntriesToolStripMenuItem});
            this.contextMenuMetaTree.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuMetaTree, "contextMenuMetaTree");
            // 
            // addEntryToolStripMenuItem
            // 
            this.addEntryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem1,
            this.addBOXEntryToolStripMenuItem1,
            this.addANIMEntryToolStripMenuItem1});
            resources.ApplyResources(this.addEntryToolStripMenuItem, "addEntryToolStripMenuItem");
            this.addEntryToolStripMenuItem.Name = "addEntryToolStripMenuItem";
            // 
            // addEntryToolStripMenuItem1
            // 
            this.addEntryToolStripMenuItem1.Name = "addEntryToolStripMenuItem1";
            resources.ApplyResources(this.addEntryToolStripMenuItem1, "addEntryToolStripMenuItem1");
            this.addEntryToolStripMenuItem1.Click += new System.EventHandler(this.addEntryToolStripMenuItem_Click);
            // 
            // addBOXEntryToolStripMenuItem1
            // 
            this.addBOXEntryToolStripMenuItem1.Name = "addBOXEntryToolStripMenuItem1";
            resources.ApplyResources(this.addBOXEntryToolStripMenuItem1, "addBOXEntryToolStripMenuItem1");
            this.addBOXEntryToolStripMenuItem1.Click += new System.EventHandler(this.addBOXEntryToolStripMenuItem1_Click);
            // 
            // addANIMEntryToolStripMenuItem1
            // 
            this.addANIMEntryToolStripMenuItem1.Name = "addANIMEntryToolStripMenuItem1";
            resources.ApplyResources(this.addANIMEntryToolStripMenuItem1, "addANIMEntryToolStripMenuItem1");
            this.addANIMEntryToolStripMenuItem1.Click += new System.EventHandler(this.addANIMEntryToolStripMenuItem1_Click);
            // 
            // addMultipleEntriesToolStripMenuItem1
            // 
            this.addMultipleEntriesToolStripMenuItem1.Name = "addMultipleEntriesToolStripMenuItem1";
            resources.ApplyResources(this.addMultipleEntriesToolStripMenuItem1, "addMultipleEntriesToolStripMenuItem1");
            this.addMultipleEntriesToolStripMenuItem1.Click += new System.EventHandler(this.addMultipleEntriesToolStripMenuItem1_Click);
            // 
            // deleteEntryToolStripMenuItem
            // 
            this.deleteEntryToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_delete;
            this.deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
            resources.ApplyResources(this.deleteEntryToolStripMenuItem, "deleteEntryToolStripMenuItem");
            this.deleteEntryToolStripMenuItem.Click += new System.EventHandler(this.deleteEntryToolStripMenuItem_Click);
            // 
            // editAllEntriesToolStripMenuItem
            // 
            this.editAllEntriesToolStripMenuItem.Name = "editAllEntriesToolStripMenuItem";
            resources.ApplyResources(this.editAllEntriesToolStripMenuItem, "editAllEntriesToolStripMenuItem");
            this.editAllEntriesToolStripMenuItem.Click += new System.EventHandler(this.editAllEntriesToolStripMenuItem_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // treeViewMain
            // 
            this.treeViewMain.AllowDrop = true;
            this.treeViewMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.treeViewMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewMain.ContextMenuStrip = this.contextMenuPCKEntries;
            resources.ApplyResources(this.treeViewMain, "treeViewMain");
            this.treeViewMain.ForeColor = System.Drawing.Color.White;
            this.treeViewMain.ImageList = this.imageList;
            this.treeViewMain.LabelEdit = true;
            this.treeViewMain.Name = "treeViewMain";
            this.treeViewMain.PathSeparator = "/";
            this.treeViewMain.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewMain_BeforeLabelEdit);
            this.treeViewMain.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewMain_ItemDrag);
            this.treeViewMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMain_AfterSelect);
            this.treeViewMain.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewMain_NodeMouseClick);
            this.treeViewMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewMain_DragDrop);
            this.treeViewMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewMain_DragEnter);
            this.treeViewMain.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewMain_DragOver);
            this.treeViewMain.DoubleClick += new System.EventHandler(this.treeViewMain_DoubleClick);
            this.treeViewMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewMain_KeyDown);
            // 
            // contextMenuPCKEntries
            // 
            this.contextMenuPCKEntries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.importSkinsToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.setFileTypeToolStripMenuItem,
            this.toolStripSeparator5,
            this.miscFunctionsToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.toolStripSeparator6,
            this.cloneFileToolStripMenuItem,
            this.renameFileToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.deleteFileToolStripMenuItem});
            this.contextMenuPCKEntries.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuPCKEntries, "contextMenuPCKEntries");
            this.contextMenuPCKEntries.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuPCKEntries_Opening);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.folderToolStripMenuItem,
            this.skinToolStripMenuItem,
            this.createAnimatedTextureToolStripMenuItem,
            this.audiopckToolStripMenuItem,
            this.colourscolToolStripMenuItem,
            this.CreateSkinsPCKToolStripMenuItem1,
            this.behavioursbinToolStripMenuItem,
            this.entityMaterialsbinToolStripMenuItem});
            resources.ApplyResources(this.createToolStripMenuItem, "createToolStripMenuItem");
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            // 
            // folderToolStripMenuItem
            // 
            resources.ApplyResources(this.folderToolStripMenuItem, "folderToolStripMenuItem");
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Click += new System.EventHandler(this.folderToolStripMenuItem_Click);
            // 
            // skinToolStripMenuItem
            // 
            resources.ApplyResources(this.skinToolStripMenuItem, "skinToolStripMenuItem");
            this.skinToolStripMenuItem.Name = "skinToolStripMenuItem";
            this.skinToolStripMenuItem.Click += new System.EventHandler(this.createSkinToolStripMenuItem_Click);
            // 
            // createAnimatedTextureToolStripMenuItem
            // 
            resources.ApplyResources(this.createAnimatedTextureToolStripMenuItem, "createAnimatedTextureToolStripMenuItem");
            this.createAnimatedTextureToolStripMenuItem.Name = "createAnimatedTextureToolStripMenuItem";
            this.createAnimatedTextureToolStripMenuItem.Click += new System.EventHandler(this.createAnimatedTextureToolStripMenuItem_Click);
            // 
            // audiopckToolStripMenuItem
            // 
            this.audiopckToolStripMenuItem.Image = global::PckStudio.Properties.Resources.BINKA_ICON;
            this.audiopckToolStripMenuItem.Name = "audiopckToolStripMenuItem";
            resources.ApplyResources(this.audiopckToolStripMenuItem, "audiopckToolStripMenuItem");
            this.audiopckToolStripMenuItem.Click += new System.EventHandler(this.audiopckToolStripMenuItem_Click);
            // 
            // colourscolToolStripMenuItem
            // 
            this.colourscolToolStripMenuItem.Image = global::PckStudio.Properties.Resources.COL_ICON;
            this.colourscolToolStripMenuItem.Name = "colourscolToolStripMenuItem";
            resources.ApplyResources(this.colourscolToolStripMenuItem, "colourscolToolStripMenuItem");
            this.colourscolToolStripMenuItem.Click += new System.EventHandler(this.colourscolToolStripMenuItem_Click);
            // 
            // CreateSkinsPCKToolStripMenuItem1
            // 
            this.CreateSkinsPCKToolStripMenuItem1.Image = global::PckStudio.Properties.Resources.SKINS_ICON;
            this.CreateSkinsPCKToolStripMenuItem1.Name = "CreateSkinsPCKToolStripMenuItem1";
            resources.ApplyResources(this.CreateSkinsPCKToolStripMenuItem1, "CreateSkinsPCKToolStripMenuItem1");
            this.CreateSkinsPCKToolStripMenuItem1.Click += new System.EventHandler(this.CreateSkinsPCKToolStripMenuItem1_Click);
            // 
            // behavioursbinToolStripMenuItem
            // 
            this.behavioursbinToolStripMenuItem.Image = global::PckStudio.Properties.Resources.BEHAVIOURS_ICON;
            this.behavioursbinToolStripMenuItem.Name = "behavioursbinToolStripMenuItem";
            resources.ApplyResources(this.behavioursbinToolStripMenuItem, "behavioursbinToolStripMenuItem");
            this.behavioursbinToolStripMenuItem.Click += new System.EventHandler(this.behavioursbinToolStripMenuItem_Click);
            // 
            // entityMaterialsbinToolStripMenuItem
            // 
            this.entityMaterialsbinToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ENTITY_MATERIALS_ICON;
            this.entityMaterialsbinToolStripMenuItem.Name = "entityMaterialsbinToolStripMenuItem";
            resources.ApplyResources(this.entityMaterialsbinToolStripMenuItem, "entityMaterialsbinToolStripMenuItem");
            this.entityMaterialsbinToolStripMenuItem.Click += new System.EventHandler(this.entityMaterialsbinToolStripMenuItem_Click);
            // 
            // importSkinsToolStripMenuItem
            // 
            this.importSkinsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSkinToolStripMenuItem,
            this.importExtractedSkinsFolderToolStripMenuItem,
            this.addTextureToolStripMenuItem,
            this.addFileToolStripMenuItem});
            resources.ApplyResources(this.importSkinsToolStripMenuItem, "importSkinsToolStripMenuItem");
            this.importSkinsToolStripMenuItem.Name = "importSkinsToolStripMenuItem";
            // 
            // importSkinToolStripMenuItem
            // 
            resources.ApplyResources(this.importSkinToolStripMenuItem, "importSkinToolStripMenuItem");
            this.importSkinToolStripMenuItem.Name = "importSkinToolStripMenuItem";
            this.importSkinToolStripMenuItem.Click += new System.EventHandler(this.importSkinToolStripMenuItem_Click);
            // 
            // importExtractedSkinsFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.importExtractedSkinsFolderToolStripMenuItem, "importExtractedSkinsFolderToolStripMenuItem");
            this.importExtractedSkinsFolderToolStripMenuItem.Name = "importExtractedSkinsFolderToolStripMenuItem";
            this.importExtractedSkinsFolderToolStripMenuItem.Click += new System.EventHandler(this.importExtractedSkinsFolder);
            // 
            // addTextureToolStripMenuItem
            // 
            this.addTextureToolStripMenuItem.Image = global::PckStudio.Properties.Resources.AddTexture;
            this.addTextureToolStripMenuItem.Name = "addTextureToolStripMenuItem";
            resources.ApplyResources(this.addTextureToolStripMenuItem, "addTextureToolStripMenuItem");
            this.addTextureToolStripMenuItem.Click += new System.EventHandler(this.addTextureToolStripMenuItem_Click);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Image = global::PckStudio.Properties.Resources.blank;
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            resources.ApplyResources(this.addFileToolStripMenuItem, "addFileToolStripMenuItem");
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.as3DSTextureFileToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
            // 
            // as3DSTextureFileToolStripMenuItem
            // 
            this.as3DSTextureFileToolStripMenuItem.Name = "as3DSTextureFileToolStripMenuItem";
            resources.ApplyResources(this.as3DSTextureFileToolStripMenuItem, "as3DSTextureFileToolStripMenuItem");
            this.as3DSTextureFileToolStripMenuItem.Click += new System.EventHandler(this.as3DSTextureFileToolStripMenuItem_Click);
            // 
            // setFileTypeToolStripMenuItem
            // 
            this.setFileTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skinToolStripMenuItem1,
            this.capeToolStripMenuItem,
            this.textureToolStripMenuItem,
            this.languagesFileLOCToolStripMenuItem,
            this.gameRulesFileGRFToolStripMenuItem,
            this.audioPCKFileToolStripMenuItem,
            this.coloursCOLFileToolStripMenuItem,
            this.gameRulesHeaderGRHToolStripMenuItem,
            this.skinsPCKToolStripMenuItem,
            this.modelsFileBINToolStripMenuItem,
            this.behavioursFileBINToolStripMenuItem,
            this.entityMaterialsFileBINToolStripMenuItem});
            this.setFileTypeToolStripMenuItem.Name = "setFileTypeToolStripMenuItem";
            resources.ApplyResources(this.setFileTypeToolStripMenuItem, "setFileTypeToolStripMenuItem");
            // 
            // skinToolStripMenuItem1
            // 
            this.skinToolStripMenuItem1.Image = global::PckStudio.Properties.Resources.SKIN_ICON;
            this.skinToolStripMenuItem1.Name = "skinToolStripMenuItem1";
            resources.ApplyResources(this.skinToolStripMenuItem1, "skinToolStripMenuItem1");
            // 
            // capeToolStripMenuItem
            // 
            this.capeToolStripMenuItem.Image = global::PckStudio.Properties.Resources.CAPE_ICON;
            this.capeToolStripMenuItem.Name = "capeToolStripMenuItem";
            resources.ApplyResources(this.capeToolStripMenuItem, "capeToolStripMenuItem");
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.Image = global::PckStudio.Properties.Resources.TEXTURE_ICON;
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            resources.ApplyResources(this.textureToolStripMenuItem, "textureToolStripMenuItem");
            // 
            // languagesFileLOCToolStripMenuItem
            // 
            this.languagesFileLOCToolStripMenuItem.Image = global::PckStudio.Properties.Resources.LOC_ICON;
            this.languagesFileLOCToolStripMenuItem.Name = "languagesFileLOCToolStripMenuItem";
            resources.ApplyResources(this.languagesFileLOCToolStripMenuItem, "languagesFileLOCToolStripMenuItem");
            // 
            // gameRulesFileGRFToolStripMenuItem
            // 
            this.gameRulesFileGRFToolStripMenuItem.Image = global::PckStudio.Properties.Resources.GRF_ICON;
            this.gameRulesFileGRFToolStripMenuItem.Name = "gameRulesFileGRFToolStripMenuItem";
            resources.ApplyResources(this.gameRulesFileGRFToolStripMenuItem, "gameRulesFileGRFToolStripMenuItem");
            // 
            // audioPCKFileToolStripMenuItem
            // 
            this.audioPCKFileToolStripMenuItem.Image = global::PckStudio.Properties.Resources.BINKA_ICON;
            this.audioPCKFileToolStripMenuItem.Name = "audioPCKFileToolStripMenuItem";
            resources.ApplyResources(this.audioPCKFileToolStripMenuItem, "audioPCKFileToolStripMenuItem");
            // 
            // coloursCOLFileToolStripMenuItem
            // 
            this.coloursCOLFileToolStripMenuItem.Image = global::PckStudio.Properties.Resources.COL_ICON;
            this.coloursCOLFileToolStripMenuItem.Name = "coloursCOLFileToolStripMenuItem";
            resources.ApplyResources(this.coloursCOLFileToolStripMenuItem, "coloursCOLFileToolStripMenuItem");
            // 
            // gameRulesHeaderGRHToolStripMenuItem
            // 
            this.gameRulesHeaderGRHToolStripMenuItem.Image = global::PckStudio.Properties.Resources.GRH_ICON;
            this.gameRulesHeaderGRHToolStripMenuItem.Name = "gameRulesHeaderGRHToolStripMenuItem";
            resources.ApplyResources(this.gameRulesHeaderGRHToolStripMenuItem, "gameRulesHeaderGRHToolStripMenuItem");
            // 
            // skinsPCKToolStripMenuItem
            // 
            this.skinsPCKToolStripMenuItem.Image = global::PckStudio.Properties.Resources.SKINS_ICON;
            this.skinsPCKToolStripMenuItem.Name = "skinsPCKToolStripMenuItem";
            resources.ApplyResources(this.skinsPCKToolStripMenuItem, "skinsPCKToolStripMenuItem");
            // 
            // modelsFileBINToolStripMenuItem
            // 
            this.modelsFileBINToolStripMenuItem.Image = global::PckStudio.Properties.Resources.MODELS_ICON;
            this.modelsFileBINToolStripMenuItem.Name = "modelsFileBINToolStripMenuItem";
            resources.ApplyResources(this.modelsFileBINToolStripMenuItem, "modelsFileBINToolStripMenuItem");
            // 
            // behavioursFileBINToolStripMenuItem
            // 
            this.behavioursFileBINToolStripMenuItem.Image = global::PckStudio.Properties.Resources.BEHAVIOURS_ICON;
            this.behavioursFileBINToolStripMenuItem.Name = "behavioursFileBINToolStripMenuItem";
            resources.ApplyResources(this.behavioursFileBINToolStripMenuItem, "behavioursFileBINToolStripMenuItem");
            // 
            // entityMaterialsFileBINToolStripMenuItem
            // 
            this.entityMaterialsFileBINToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ENTITY_MATERIALS_ICON;
            this.entityMaterialsFileBINToolStripMenuItem.Name = "entityMaterialsFileBINToolStripMenuItem";
            resources.ApplyResources(this.entityMaterialsFileBINToolStripMenuItem, "entityMaterialsFileBINToolStripMenuItem");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // miscFunctionsToolStripMenuItem
            // 
            this.miscFunctionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateMipMapTextureToolStripMenuItem1,
            this.viewFileInfoToolStripMenuItem,
            this.correctSkinDecimalsToolStripMenuItem,
            this.setSubPCKEndiannessToolStripMenuItem,
            this.setModelContainerFormatToolStripMenuItem});
            this.miscFunctionsToolStripMenuItem.Name = "miscFunctionsToolStripMenuItem";
            resources.ApplyResources(this.miscFunctionsToolStripMenuItem, "miscFunctionsToolStripMenuItem");
            // 
            // generateMipMapTextureToolStripMenuItem1
            // 
            this.generateMipMapTextureToolStripMenuItem1.Name = "generateMipMapTextureToolStripMenuItem1";
            resources.ApplyResources(this.generateMipMapTextureToolStripMenuItem1, "generateMipMapTextureToolStripMenuItem1");
            this.generateMipMapTextureToolStripMenuItem1.Click += new System.EventHandler(this.generateMipMapTextureToolStripMenuItem_Click);
            // 
            // viewFileInfoToolStripMenuItem
            // 
            this.viewFileInfoToolStripMenuItem.Name = "viewFileInfoToolStripMenuItem";
            resources.ApplyResources(this.viewFileInfoToolStripMenuItem, "viewFileInfoToolStripMenuItem");
            this.viewFileInfoToolStripMenuItem.Click += new System.EventHandler(this.viewFileInfoToolStripMenuItem_Click);
            // 
            // correctSkinDecimalsToolStripMenuItem
            // 
            this.correctSkinDecimalsToolStripMenuItem.Name = "correctSkinDecimalsToolStripMenuItem";
            resources.ApplyResources(this.correctSkinDecimalsToolStripMenuItem, "correctSkinDecimalsToolStripMenuItem");
            this.correctSkinDecimalsToolStripMenuItem.Click += new System.EventHandler(this.correctSkinDecimalsToolStripMenuItem_Click);
            // 
            // setSubPCKEndiannessToolStripMenuItem
            // 
            this.setSubPCKEndiannessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bigEndianXbox360PS3WiiUToolStripMenuItem,
            this.littleEndianPS4PSVitaSwitchToolStripMenuItem});
            this.setSubPCKEndiannessToolStripMenuItem.Name = "setSubPCKEndiannessToolStripMenuItem";
            resources.ApplyResources(this.setSubPCKEndiannessToolStripMenuItem, "setSubPCKEndiannessToolStripMenuItem");
            // 
            // bigEndianXbox360PS3WiiUToolStripMenuItem
            // 
            this.bigEndianXbox360PS3WiiUToolStripMenuItem.Name = "bigEndianXbox360PS3WiiUToolStripMenuItem";
            resources.ApplyResources(this.bigEndianXbox360PS3WiiUToolStripMenuItem, "bigEndianXbox360PS3WiiUToolStripMenuItem");
            this.bigEndianXbox360PS3WiiUToolStripMenuItem.Click += new System.EventHandler(this.bigEndianToolStripMenuItem_Click);
            // 
            // littleEndianPS4PSVitaSwitchToolStripMenuItem
            // 
            this.littleEndianPS4PSVitaSwitchToolStripMenuItem.Name = "littleEndianPS4PSVitaSwitchToolStripMenuItem";
            resources.ApplyResources(this.littleEndianPS4PSVitaSwitchToolStripMenuItem, "littleEndianPS4PSVitaSwitchToolStripMenuItem");
            this.littleEndianPS4PSVitaSwitchToolStripMenuItem.Click += new System.EventHandler(this.littleEndianToolStripMenuItem_Click);
            // 
            // setModelContainerFormatToolStripMenuItem
            // 
            this.setModelContainerFormatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.version1ToolStripMenuItem,
            this.version2ToolStripMenuItem,
            this.version3114ToolStripMenuItem});
            this.setModelContainerFormatToolStripMenuItem.Name = "setModelContainerFormatToolStripMenuItem";
            resources.ApplyResources(this.setModelContainerFormatToolStripMenuItem, "setModelContainerFormatToolStripMenuItem");
            // 
            // version1ToolStripMenuItem
            // 
            this.version1ToolStripMenuItem.Name = "version1ToolStripMenuItem";
            resources.ApplyResources(this.version1ToolStripMenuItem, "version1ToolStripMenuItem");
            this.version1ToolStripMenuItem.Click += new System.EventHandler(this.setModelVersion1ToolStripMenuItem_Click);
            // 
            // version2ToolStripMenuItem
            // 
            this.version2ToolStripMenuItem.Name = "version2ToolStripMenuItem";
            resources.ApplyResources(this.version2ToolStripMenuItem, "version2ToolStripMenuItem");
            this.version2ToolStripMenuItem.Click += new System.EventHandler(this.setModelVersion2ToolStripMenuItem_Click);
            // 
            // version3114ToolStripMenuItem
            // 
            this.version3114ToolStripMenuItem.Name = "version3114ToolStripMenuItem";
            resources.ApplyResources(this.version3114ToolStripMenuItem, "version3114ToolStripMenuItem");
            this.version3114ToolStripMenuItem.Click += new System.EventHandler(this.setModelVersion3ToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            resources.ApplyResources(this.extractToolStripMenuItem, "extractToolStripMenuItem");
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // cloneFileToolStripMenuItem
            // 
            this.cloneFileToolStripMenuItem.Name = "cloneFileToolStripMenuItem";
            resources.ApplyResources(this.cloneFileToolStripMenuItem, "cloneFileToolStripMenuItem");
            this.cloneFileToolStripMenuItem.Click += new System.EventHandler(this.cloneFileToolStripMenuItem_Click);
            // 
            // renameFileToolStripMenuItem
            // 
            resources.ApplyResources(this.renameFileToolStripMenuItem, "renameFileToolStripMenuItem");
            this.renameFileToolStripMenuItem.Name = "renameFileToolStripMenuItem";
            this.renameFileToolStripMenuItem.Click += new System.EventHandler(this.renameFileToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            resources.ApplyResources(this.replaceToolStripMenuItem, "replaceToolStripMenuItem");
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // deleteFileToolStripMenuItem
            // 
            this.deleteFileToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_delete;
            this.deleteFileToolStripMenuItem.Name = "deleteFileToolStripMenuItem";
            resources.ApplyResources(this.deleteFileToolStripMenuItem, "deleteFileToolStripMenuItem");
            this.deleteFileToolStripMenuItem.Click += new System.EventHandler(this.deleteFileToolStripMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // addMultipleEntriesToolStripMenuItem
            // 
            resources.ApplyResources(this.addMultipleEntriesToolStripMenuItem, "addMultipleEntriesToolStripMenuItem");
            this.addMultipleEntriesToolStripMenuItem.Name = "addMultipleEntriesToolStripMenuItem";
            // 
            // LittleEndianCheckBox
            // 
            resources.ApplyResources(this.LittleEndianCheckBox, "LittleEndianCheckBox");
            this.LittleEndianCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.LittleEndianCheckBox.Name = "LittleEndianCheckBox";
            this.LittleEndianCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.LittleEndianCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.LittleEndianCheckBox.UseSelectable = true;
            // 
            // previewPictureBox
            // 
            resources.ApplyResources(this.previewPictureBox, "previewPictureBox");
            this.previewPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.previewPictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.previewPictureBox.Image = global::PckStudio.Properties.Resources.NoImageFound;
            this.previewPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.TabStop = false;
            // 
            // PckEditor
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.LittleEndianCheckBox);
            this.Controls.Add(this.pckFileLabel);
            this.Controls.Add(this.labelImageSize);
            this.Controls.Add(this.fileEntryCountLabel);
            this.Controls.Add(this.PropertiesTabControl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.treeViewMain);
            this.Controls.Add(logoPictureBox);
            this.Controls.Add(this.previewPictureBox);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "PckEditor";
            this.Load += new System.EventHandler(this.PckEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(logoPictureBox)).EndInit();
            this.PropertiesTabControl.ResumeLayout(false);
            this.MetaTab.ResumeLayout(false);
            this.MetaTab.PerformLayout();
            this.contextMenuMetaTree.ResumeLayout(false);
            this.contextMenuPCKEntries.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel pckFileLabel;
        private MetroFramework.Controls.MetroLabel labelImageSize;
        private MetroFramework.Controls.MetroLabel fileEntryCountLabel;
        private MetroFramework.Controls.MetroTabControl PropertiesTabControl;
        private MetroFramework.Controls.MetroTabPage MetaTab;
        private System.Windows.Forms.TreeView treeMeta;
        private MetroFramework.Controls.MetroLabel label11;
        private System.Windows.Forms.TreeView treeViewMain;
        private PckStudio.ToolboxItems.InterpolationPictureBox previewPictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuMetaTree;
        private System.Windows.Forms.ToolStripMenuItem addEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addBOXEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addANIMEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addMultipleEntriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAllEntriesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuPCKEntries;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createAnimatedTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audiopckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colourscolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateSkinsPCKToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem behavioursbinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entityMaterialsbinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSkinsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSkinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importExtractedSkinsFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem as3DSTextureFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setFileTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skinToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem capeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languagesFileLOCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameRulesFileGRFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem audioPCKFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coloursCOLFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameRulesHeaderGRHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skinsPCKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelsFileBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem behavioursFileBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entityMaterialsFileBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miscFunctionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateMipMapTextureToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem viewFileInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem correctSkinDecimalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloneFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFileToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripMenuItem addMultipleEntriesToolStripMenuItem1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTextBox entryTypeTextBox;
        private MetroFramework.Controls.MetroTextBox entryDataTextBox;
        private MetroFramework.Controls.MetroButton buttonEdit;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroCheckBox LittleEndianCheckBox;
        private System.Windows.Forms.ToolStripMenuItem setSubPCKEndiannessToolStripMenuItem;
        private ToolStripMenuItem bigEndianXbox360PS3WiiUToolStripMenuItem;
        private ToolStripMenuItem littleEndianPS4PSVitaSwitchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setModelContainerFormatToolStripMenuItem;
        private ToolStripMenuItem version1ToolStripMenuItem;
        private ToolStripMenuItem version2ToolStripMenuItem;
        private ToolStripMenuItem version3114ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
    }
}
