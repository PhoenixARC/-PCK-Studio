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
            this.renderer3D1 = new PckStudio.ToolboxItems.Renderer3D();
            this.SuspendLayout();
            // 
            // renderer3D1
            // 
            this.renderer3D1.BackColor = System.Drawing.Color.Gray;
            this.renderer3D1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderer3D1.InDesignMode = true;
            this.renderer3D1.Location = new System.Drawing.Point(0, 0);
            this.renderer3D1.LookX = 0D;
            this.renderer3D1.LookY = 0D;
            this.renderer3D1.Model = PckStudio.ToolboxItems.Renderer3D.Models.Steve;
            this.renderer3D1.Name = "renderer3D1";
            this.renderer3D1.Paintable = true;
            this.renderer3D1.RotationX = 0;
            this.renderer3D1.RotationY = 0;
            this.renderer3D1.Show2ndBody = true;
            this.renderer3D1.Show2ndHead = true;
            this.renderer3D1.Show2ndLeftArm = true;
            this.renderer3D1.Show2ndLeftLeg = true;
            this.renderer3D1.Show2ndRightArm = true;
            this.renderer3D1.Show2ndRightLeg = true;
            this.renderer3D1.ShowBody = true;
            this.renderer3D1.ShowHead = true;
            this.renderer3D1.ShowLeftArm = true;
            this.renderer3D1.ShowLeftLeg = true;
            this.renderer3D1.ShowRightArm = true;
            this.renderer3D1.ShowRightLeg = true;
            this.renderer3D1.Size = new System.Drawing.Size(426, 428);
            this.renderer3D1.Skin = global::PckStudio.Properties.Resources.steve;
            this.renderer3D1.TabIndex = 8;
            this.renderer3D1.Zoom = 1D;
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

        private ToolboxItems.Renderer3D renderer3D1;
    }
}