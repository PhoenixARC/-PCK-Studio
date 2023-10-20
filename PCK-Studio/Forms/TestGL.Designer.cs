namespace PckStudio.Forms
{
    partial class TestGL
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
            this.renderer3D1 = new PckStudio.Rendering.SkinRenderer();
            this.SuspendLayout();
            // 
            // renderer3D1
            // 
            this.renderer3D1.BackColor = System.Drawing.Color.Gray;
            this.renderer3D1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderer3D1.Location = new System.Drawing.Point(0, 0);
            this.renderer3D1.Name = "renderer3D1";
            this.renderer3D1.Size = new System.Drawing.Size(426, 428);
            this.renderer3D1.TabIndex = 8;
            // 
            // TestGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 428);
            this.Controls.Add(this.renderer3D1);
            this.Name = "TestGL";
            this.Text = "TestGL";
            this.Load += new System.EventHandler(this.TestGL_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PckStudio.Rendering.SkinRenderer renderer3D1;
    }
}