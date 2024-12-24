namespace PckStudio.Forms.Editor
{
    partial class ModelEditor
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
            this.modelTreeView = new System.Windows.Forms.TreeView();
            this.modelContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showModelBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.modelViewport = new PckStudio.Rendering.ModelRenderer();
            this.namedTexturesTreeView = new System.Windows.Forms.TreeView();
            this.textureImageList = new System.Windows.Forms.ImageList(this.components);
            this.modelContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelTreeView
            // 
            this.modelTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modelTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.modelTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modelTreeView.ContextMenuStrip = this.modelContextMenu;
            this.modelTreeView.ForeColor = System.Drawing.SystemColors.Window;
            this.modelTreeView.HideSelection = false;
            this.modelTreeView.Location = new System.Drawing.Point(3, 3);
            this.modelTreeView.Name = "modelTreeView";
            this.modelTreeView.PathSeparator = ".";
            this.tableLayoutPanel1.SetRowSpan(this.modelTreeView, 2);
            this.modelTreeView.Size = new System.Drawing.Size(287, 440);
            this.modelTreeView.TabIndex = 0;
            this.modelTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.modelTreeView_BeforeSelect);
            // 
            // modelContextMenu
            // 
            this.modelContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.editToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.modelContextMenu.Name = "modelContextMenu";
            this.modelContextMenu.Size = new System.Drawing.Size(118, 70);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Visible = false;
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Visible = false;
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(20, 60);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(660, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem1.Text = "Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showModelBoundsToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showModelBoundsToolStripMenuItem
            // 
            this.showModelBoundsToolStripMenuItem.CheckOnClick = true;
            this.showModelBoundsToolStripMenuItem.Name = "showModelBoundsToolStripMenuItem";
            this.showModelBoundsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.showModelBoundsToolStripMenuItem.Text = "Show Model Bounds";
            this.showModelBoundsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showModelBoundsToolStripMenuItem_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.modelViewport, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.namedTexturesTreeView, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.modelTreeView, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(660, 446);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // modelViewport
            // 
            this.modelViewport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.modelViewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelViewport.Location = new System.Drawing.Point(296, 3);
            this.modelViewport.MouseSensetivity = 0.01F;
            this.modelViewport.Name = "modelViewport";
            this.modelViewport.RefreshRate = 120;
            this.modelViewport.RenderModelBounds = false;
            this.modelViewport.Size = new System.Drawing.Size(361, 261);
            this.modelViewport.TabIndex = 1;
            this.modelViewport.VSync = true;
            // 
            // namedTexturesTreeView
            // 
            this.namedTexturesTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.namedTexturesTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.namedTexturesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.namedTexturesTreeView.ForeColor = System.Drawing.SystemColors.Window;
            this.namedTexturesTreeView.FullRowSelect = true;
            this.namedTexturesTreeView.HideSelection = false;
            this.namedTexturesTreeView.ImageIndex = 0;
            this.namedTexturesTreeView.ImageList = this.textureImageList;
            this.namedTexturesTreeView.Location = new System.Drawing.Point(296, 270);
            this.namedTexturesTreeView.Name = "namedTexturesTreeView";
            this.namedTexturesTreeView.SelectedImageIndex = 0;
            this.namedTexturesTreeView.ShowLines = false;
            this.namedTexturesTreeView.ShowPlusMinus = false;
            this.namedTexturesTreeView.ShowRootLines = false;
            this.namedTexturesTreeView.Size = new System.Drawing.Size(361, 173);
            this.namedTexturesTreeView.TabIndex = 2;
            this.namedTexturesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.namedTexturesTreeView_AfterSelect);
            // 
            // textureImageList
            // 
            this.textureImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.textureImageList.ImageSize = new System.Drawing.Size(36, 36);
            this.textureImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 550);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 550);
            this.Name = "ModelEditor";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Model Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.modelContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView modelTreeView;
        private MetroFramework.Controls.MetroContextMenu modelContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Rendering.ModelRenderer modelViewport;
        private System.Windows.Forms.TreeView namedTexturesTreeView;
        private System.Windows.Forms.ImageList textureImageList;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showModelBoundsToolStripMenuItem;
    }
}