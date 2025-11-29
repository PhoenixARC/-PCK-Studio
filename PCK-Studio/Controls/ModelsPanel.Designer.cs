namespace PckStudio.Controls
{
    partial class ModelsPanel
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
            this.modelTreeView = new System.Windows.Forms.TreeView();
            this.modelRenderer = new PckStudio.Rendering.ModelRenderer();
            this.textureTreeView = new System.Windows.Forms.TreeView();
            this.showBoundsCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.Controls.Add(this.modelTreeView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.modelRenderer, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textureTreeView, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.showBoundsCheckBox, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(732, 612);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // modelTreeView
            // 
            this.modelTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.modelTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modelTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelTreeView.ForeColor = System.Drawing.SystemColors.Window;
            this.modelTreeView.Location = new System.Drawing.Point(3, 3);
            this.modelTreeView.Name = "modelTreeView";
            this.tableLayoutPanel1.SetRowSpan(this.modelTreeView, 3);
            this.modelTreeView.Size = new System.Drawing.Size(250, 606);
            this.modelTreeView.TabIndex = 0;
            this.modelTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // modelRenderer
            // 
            this.modelRenderer.AllowCameraMovement = false;
            this.modelRenderer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.modelRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelRenderer.ForeColor = System.Drawing.Color.White;
            this.modelRenderer.Location = new System.Drawing.Point(259, 3);
            this.modelRenderer.MouseSensetivity = 0.01F;
            this.modelRenderer.Name = "modelRenderer";
            this.modelRenderer.RefreshRate = 60;
            this.modelRenderer.RenderModelBounds = false;
            this.modelRenderer.Size = new System.Drawing.Size(470, 300);
            this.modelRenderer.TabIndex = 1;
            this.modelRenderer.Visible = false;
            this.modelRenderer.VSync = true;
            // 
            // textureTreeView
            // 
            this.textureTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.textureTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textureTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureTreeView.ForeColor = System.Drawing.SystemColors.Window;
            this.textureTreeView.Location = new System.Drawing.Point(259, 339);
            this.textureTreeView.Name = "textureTreeView";
            this.textureTreeView.ShowLines = false;
            this.textureTreeView.Size = new System.Drawing.Size(470, 270);
            this.textureTreeView.TabIndex = 2;
            this.textureTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.textureTreeView_AfterSelect);
            // 
            // showBoundsCheckBox
            // 
            this.showBoundsCheckBox.AutoSize = true;
            this.showBoundsCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.showBoundsCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showBoundsCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showBoundsCheckBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.showBoundsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.showBoundsCheckBox.ForeColor = System.Drawing.Color.White;
            this.showBoundsCheckBox.Location = new System.Drawing.Point(610, 309);
            this.showBoundsCheckBox.Name = "showBoundsCheckBox";
            this.showBoundsCheckBox.Size = new System.Drawing.Size(119, 24);
            this.showBoundsCheckBox.TabIndex = 3;
            this.showBoundsCheckBox.Text = "Show Bounding box";
            this.showBoundsCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showBoundsCheckBox.UseVisualStyleBackColor = false;
            this.showBoundsCheckBox.CheckedChanged += new System.EventHandler(this.showBoundsCheckBox1_CheckedChanged);
            // 
            // ModelsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ModelsPanel";
            this.Size = new System.Drawing.Size(732, 612);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView modelTreeView;
        private Rendering.ModelRenderer modelRenderer;
        private System.Windows.Forms.TreeView textureTreeView;
        private System.Windows.Forms.CheckBox showBoundsCheckBox;
    }
}
