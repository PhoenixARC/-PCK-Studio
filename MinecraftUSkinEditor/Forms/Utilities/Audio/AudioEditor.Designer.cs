﻿
namespace PckStudio.Forms.Utilities
{
	partial class AudioEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioEditor));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCategoryStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeCategoryStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.catImages = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.ImageList = this.catImages;
            this.treeView1.LabelEdit = true;
            this.treeView1.Name = "treeView1";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCategoryStripMenuItem,
            this.removeCategoryStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // addCategoryStripMenuItem
            // 
            resources.ApplyResources(this.addCategoryStripMenuItem, "addCategoryStripMenuItem");
            this.addCategoryStripMenuItem.Name = "addCategoryStripMenuItem";
            this.addCategoryStripMenuItem.Click += new System.EventHandler(this.addCategoryStripMenuItem_Click);
            // 
            // removeCategoryStripMenuItem
            // 
            this.removeCategoryStripMenuItem.Image = global::PckStudio.Properties.Resources.Del;
            this.removeCategoryStripMenuItem.Name = "removeCategoryStripMenuItem";
            resources.ApplyResources(this.removeCategoryStripMenuItem, "removeCategoryStripMenuItem");
            this.removeCategoryStripMenuItem.Click += new System.EventHandler(this.removeCategoryStripMenuItem_Click);
            // 
            // catImages
            // 
            this.catImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("catImages.ImageStream")));
            this.catImages.TransparentColor = System.Drawing.Color.Transparent;
            this.catImages.Images.SetKeyName(0, "0_overworld.png");
            this.catImages.Images.SetKeyName(1, "1_nether.png");
            this.catImages.Images.SetKeyName(2, "2_end.png");
            this.catImages.Images.SetKeyName(3, "3_menu.png");
            this.catImages.Images.SetKeyName(4, "4_creative.png");
            this.catImages.Images.SetKeyName(5, "5_mg01.png");
            this.catImages.Images.SetKeyName(6, "6_mg02.png");
            this.catImages.Images.SetKeyName(7, "7_mg03.png");
            // 
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
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
            this.creditsEditorToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // creditsEditorToolStripMenuItem
            // 
            this.creditsEditorToolStripMenuItem.Image = global::PckStudio.Properties.Resources.ExportFile;
            this.creditsEditorToolStripMenuItem.Name = "creditsEditorToolStripMenuItem";
            resources.ApplyResources(this.creditsEditorToolStripMenuItem, "creditsEditorToolStripMenuItem");
            this.creditsEditorToolStripMenuItem.Click += new System.EventHandler(this.creditsEditorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // treeView2
            // 
            this.treeView2.AllowDrop = true;
            resources.ApplyResources(this.treeView2, "treeView2");
            this.treeView2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.treeView2.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView2.ForeColor = System.Drawing.Color.White;
            this.treeView2.Name = "treeView2";
            this.treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            this.treeView2.DragDrop += new System.Windows.Forms.DragEventHandler(this.Binka_DragDrop);
            this.treeView2.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView2_DragEnter);
            this.treeView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView2_KeyDown);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryMenuItem,
            this.removeEntryMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip2, "contextMenuStrip2");
            // 
            // addEntryMenuItem
            // 
            resources.ApplyResources(this.addEntryMenuItem, "addEntryMenuItem");
            this.addEntryMenuItem.Name = "addEntryMenuItem";
            this.addEntryMenuItem.Click += new System.EventHandler(this.addEntryMenuItem_Click);
            // 
            // removeEntryMenuItem
            // 
            this.removeEntryMenuItem.Image = global::PckStudio.Properties.Resources.Del;
            this.removeEntryMenuItem.Name = "removeEntryMenuItem";
            resources.ApplyResources(this.removeEntryMenuItem, "removeEntryMenuItem");
            this.removeEntryMenuItem.Click += new System.EventHandler(this.removeEntryMenuItem_Click);
            // 
            // AudioEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.menuStrip);
            this.Name = "AudioEditor";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.AudioEditor_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
		private System.Windows.Forms.TreeView treeView2;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem removeCategoryStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addCategoryStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem addEntryMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeEntryMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ImageList catImages;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem creditsEditorToolStripMenuItem;
	}
}