namespace PckStudio.Controls
{
    partial class DefaultPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefaultPanel));
            this.PropertiesTabControl = new MetroFramework.Controls.MetroTabControl();
            this.MetaTab = new MetroFramework.Controls.MetroTabPage();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.buttonEdit = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.treeMeta = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.displayNameLabel = new System.Windows.Forms.Label();
            this.previewPictureBox = new PckStudio.ToolboxItems.InterpolationPictureBox();
            this.skinRenderer = new PckStudio.Rendering.SkinRenderer();
            this.themeNameLabel = new System.Windows.Forms.Label();
            this.contextMenuMetaTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addBOXEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addANIMEntryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addMultipleEntriesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAllEntriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PropertiesTabControl.SuspendLayout();
            this.MetaTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).BeginInit();
            this.contextMenuMetaTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // PropertiesTabControl
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.PropertiesTabControl, 2);
            this.PropertiesTabControl.Controls.Add(this.MetaTab);
            this.PropertiesTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertiesTabControl.Location = new System.Drawing.Point(3, 369);
            this.PropertiesTabControl.Name = "PropertiesTabControl";
            this.PropertiesTabControl.SelectedIndex = 0;
            this.PropertiesTabControl.Size = new System.Drawing.Size(907, 322);
            this.PropertiesTabControl.Style = MetroFramework.MetroColorStyle.Silver;
            this.PropertiesTabControl.TabIndex = 13;
            this.PropertiesTabControl.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PropertiesTabControl.UseSelectable = true;
            // 
            // MetaTab
            // 
            this.MetaTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MetaTab.Controls.Add(this.metroLabel2);
            this.MetaTab.Controls.Add(this.buttonEdit);
            this.MetaTab.Controls.Add(this.metroLabel1);
            this.MetaTab.Controls.Add(this.treeMeta);
            this.MetaTab.HorizontalScrollbarBarColor = true;
            this.MetaTab.HorizontalScrollbarHighlightOnWheel = false;
            this.MetaTab.HorizontalScrollbarSize = 10;
            this.MetaTab.Location = new System.Drawing.Point(4, 38);
            this.MetaTab.Name = "MetaTab";
            this.MetaTab.Padding = new System.Windows.Forms.Padding(5);
            this.MetaTab.Size = new System.Drawing.Size(899, 280);
            this.MetaTab.TabIndex = 0;
            this.MetaTab.Text = "Properties";
            this.MetaTab.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.MetaTab.VerticalScrollbarBarColor = true;
            this.MetaTab.VerticalScrollbarHighlightOnWheel = false;
            this.MetaTab.VerticalScrollbarSize = 10;
            // 
            // metroLabel2
            // 
            this.metroLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(379, 146);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(0, 0);
            this.metroLabel2.TabIndex = 15;
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(3, 248);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(201, 31);
            this.buttonEdit.TabIndex = 20;
            this.buttonEdit.Text = "buttonEdit";
            this.buttonEdit.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonEdit.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(441, 39);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(0, 0);
            this.metroLabel1.TabIndex = 13;
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // treeMeta
            // 
            this.treeMeta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeMeta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.treeMeta.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeMeta.ForeColor = System.Drawing.SystemColors.Window;
            this.treeMeta.Location = new System.Drawing.Point(3, 4);
            this.treeMeta.Name = "treeMeta";
            this.treeMeta.PathSeparator = "/";
            this.treeMeta.Size = new System.Drawing.Size(888, 238);
            this.treeMeta.TabIndex = 0;
            this.treeMeta.DoubleClick += new System.EventHandler(this.treeMeta_DoubleClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.displayNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.PropertiesTabControl, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.previewPictureBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.skinRenderer, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.themeNameLabel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(913, 694);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // displayNameLabel
            // 
            this.displayNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayNameLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.displayNameLabel, 2);
            this.displayNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayNameLabel.ForeColor = System.Drawing.Color.White;
            this.displayNameLabel.Location = new System.Drawing.Point(3, 0);
            this.displayNameLabel.Name = "displayNameLabel";
            this.displayNameLabel.Size = new System.Drawing.Size(907, 20);
            this.displayNameLabel.TabIndex = 15;
            this.displayNameLabel.Text = "name";
            this.displayNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // previewPictureBox
            // 
            this.previewPictureBox.BackgroundInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.previewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPictureBox.Image = global::PckStudio.Properties.Resources.NoImageFound;
            this.previewPictureBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.previewPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.previewPictureBox.Location = new System.Drawing.Point(3, 43);
            this.previewPictureBox.Name = "previewPictureBox";
            this.previewPictureBox.Size = new System.Drawing.Size(450, 320);
            this.previewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.previewPictureBox.TabIndex = 12;
            this.previewPictureBox.TabStop = false;
            // 
            // skinRenderer
            // 
            this.skinRenderer.AllowCameraMovement = false;
            this.skinRenderer.Animate = true;
            this.skinRenderer.ArmorTexture = null;
            this.skinRenderer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.skinRenderer.CapeTexture = null;
            this.skinRenderer.CenterOnSelect = false;
            this.skinRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skinRenderer.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.skinRenderer.GuideLineColor = System.Drawing.Color.Empty;
            this.skinRenderer.HighlightlingColor = System.Drawing.Color.Aqua;
            this.skinRenderer.Location = new System.Drawing.Point(459, 43);
            this.skinRenderer.MouseSensetivity = 0.01F;
            this.skinRenderer.Name = "skinRenderer";
            this.skinRenderer.RefreshRate = 60;
            this.skinRenderer.RenderGroundPlane = false;
            this.skinRenderer.RenderSkyBox = false;
            this.skinRenderer.SelectedIndex = -1;
            this.skinRenderer.SelectedIndices = new int[0];
            this.skinRenderer.ShowArmor = false;
            this.skinRenderer.ShowBoundingBox = false;
            this.skinRenderer.ShowGuideLines = false;
            this.skinRenderer.Size = new System.Drawing.Size(451, 320);
            this.skinRenderer.TabIndex = 14;
            this.skinRenderer.Texture = null;
            this.skinRenderer.Visible = false;
            this.skinRenderer.VSync = true;
            // 
            // themeNameLabel
            // 
            this.themeNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.themeNameLabel.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.themeNameLabel, 2);
            this.themeNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.themeNameLabel.ForeColor = System.Drawing.Color.White;
            this.themeNameLabel.Location = new System.Drawing.Point(3, 20);
            this.themeNameLabel.Name = "themeNameLabel";
            this.themeNameLabel.Size = new System.Drawing.Size(907, 20);
            this.themeNameLabel.TabIndex = 16;
            this.themeNameLabel.Text = "theme";
            this.themeNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenuMetaTree
            // 
            this.contextMenuMetaTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem,
            this.addMultipleEntriesToolStripMenuItem1,
            this.deleteEntryToolStripMenuItem,
            this.editAllEntriesToolStripMenuItem});
            this.contextMenuMetaTree.Name = "contextMenuStrip1";
            this.contextMenuMetaTree.Size = new System.Drawing.Size(182, 92);
            // 
            // addEntryToolStripMenuItem
            // 
            this.addEntryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEntryToolStripMenuItem1,
            this.addBOXEntryToolStripMenuItem1,
            this.addANIMEntryToolStripMenuItem1});
            this.addEntryToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addEntryToolStripMenuItem.Image")));
            this.addEntryToolStripMenuItem.Name = "addEntryToolStripMenuItem";
            this.addEntryToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addEntryToolStripMenuItem.Text = "Add Entry";
            // 
            // addEntryToolStripMenuItem1
            // 
            this.addEntryToolStripMenuItem1.Name = "addEntryToolStripMenuItem1";
            this.addEntryToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.addEntryToolStripMenuItem1.Text = "Add Entry";
            this.addEntryToolStripMenuItem1.Click += new System.EventHandler(this.addEntryToolStripMenuItem_Click);
            // 
            // addBOXEntryToolStripMenuItem1
            // 
            this.addBOXEntryToolStripMenuItem1.Name = "addBOXEntryToolStripMenuItem1";
            this.addBOXEntryToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.addBOXEntryToolStripMenuItem1.Text = "Add BOX Entry";
            this.addBOXEntryToolStripMenuItem1.Click += new System.EventHandler(this.addBOXEntryToolStripMenuItem1_Click);
            // 
            // addANIMEntryToolStripMenuItem1
            // 
            this.addANIMEntryToolStripMenuItem1.Name = "addANIMEntryToolStripMenuItem1";
            this.addANIMEntryToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.addANIMEntryToolStripMenuItem1.Text = "Add ANIM Entry";
            this.addANIMEntryToolStripMenuItem1.Click += new System.EventHandler(this.addANIMEntryToolStripMenuItem1_Click);
            // 
            // addMultipleEntriesToolStripMenuItem1
            // 
            this.addMultipleEntriesToolStripMenuItem1.Name = "addMultipleEntriesToolStripMenuItem1";
            this.addMultipleEntriesToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.addMultipleEntriesToolStripMenuItem1.Text = "Add Multiple Entries";
            this.addMultipleEntriesToolStripMenuItem1.Click += new System.EventHandler(this.addMultipleEntriesToolStripMenuItem1_Click);
            // 
            // deleteEntryToolStripMenuItem
            // 
            this.deleteEntryToolStripMenuItem.Image = global::PckStudio.Properties.Resources.file_delete;
            this.deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
            this.deleteEntryToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.deleteEntryToolStripMenuItem.Text = "Delete Entry";
            // 
            // editAllEntriesToolStripMenuItem
            // 
            this.editAllEntriesToolStripMenuItem.Name = "editAllEntriesToolStripMenuItem";
            this.editAllEntriesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editAllEntriesToolStripMenuItem.Click += new System.EventHandler(this.editAllEntriesToolStripMenuItem_Click);
            // 
            // DefaultPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DefaultPanel";
            this.Size = new System.Drawing.Size(913, 694);
            this.PropertiesTabControl.ResumeLayout(false);
            this.MetaTab.ResumeLayout(false);
            this.MetaTab.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPictureBox)).EndInit();
            this.contextMenuMetaTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PckStudio.ToolboxItems.InterpolationPictureBox previewPictureBox;
        private MetroFramework.Controls.MetroTabControl PropertiesTabControl;
        private MetroFramework.Controls.MetroTabPage MetaTab;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroButton buttonEdit;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.TreeView treeMeta;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuMetaTree;
        private System.Windows.Forms.ToolStripMenuItem addEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addBOXEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addANIMEntryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addMultipleEntriesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAllEntriesToolStripMenuItem;
        private Rendering.SkinRenderer skinRenderer;
        private System.Windows.Forms.Label displayNameLabel;
        private System.Windows.Forms.Label themeNameLabel;
    }
}
