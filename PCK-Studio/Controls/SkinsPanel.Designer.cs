namespace PckStudio.Controls
{
    partial class SkinsPanel
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.skinRenderer1 = new PckStudio.Rendering.SkinRenderer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.skinRenderer1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.ForeColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(660, 517);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // skinRenderer1
            // 
            this.skinRenderer1.AllowCameraMovement = false;
            this.skinRenderer1.Animate = true;
            this.skinRenderer1.ArmorTexture = null;
            this.skinRenderer1.BackColor = System.Drawing.Color.Black;
            this.skinRenderer1.CapeTexture = null;
            this.skinRenderer1.CenterOnSelect = false;
            this.skinRenderer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skinRenderer1.GuideLineColor = System.Drawing.Color.Empty;
            this.skinRenderer1.HighlightlingColor = System.Drawing.Color.Aqua;
            this.skinRenderer1.Location = new System.Drawing.Point(333, 3);
            this.skinRenderer1.MouseSensetivity = 0.01F;
            this.skinRenderer1.Name = "skinRenderer1";
            this.skinRenderer1.RefreshRate = 60;
            this.skinRenderer1.RenderGroundPlane = true;
            this.skinRenderer1.RenderSkyBox = true;
            this.skinRenderer1.SelectedIndex = -1;
            this.skinRenderer1.SelectedIndices = new int[0];
            this.skinRenderer1.ShowArmor = false;
            this.skinRenderer1.ShowBoundingBox = false;
            this.skinRenderer1.ShowGuideLines = false;
            this.skinRenderer1.Size = new System.Drawing.Size(324, 252);
            this.skinRenderer1.TabIndex = 0;
            this.skinRenderer1.Texture = null;
            this.skinRenderer1.VSync = true;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.SystemColors.Window;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.PathSeparator = ".";
            this.tableLayoutPanel1.SetRowSpan(this.treeView1, 2);
            this.treeView1.ShowLines = false;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(324, 511);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // SkinsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SkinsPanel";
            this.Size = new System.Drawing.Size(660, 517);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Rendering.SkinRenderer skinRenderer1;
        private System.Windows.Forms.TreeView treeView1;
    }
}
