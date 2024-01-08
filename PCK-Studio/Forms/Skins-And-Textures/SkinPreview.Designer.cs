
namespace PckStudio.Forms
{
    partial class SkinPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkinPreview));
            this.ModelView = new PckStudio.Rendering.SkinRenderer();
            this.SuspendLayout();
            // 
            // ModelView
            // 
            this.ModelView.BackColor = System.Drawing.Color.DarkGray;
            this.ModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModelView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModelView.ForeColor = System.Drawing.Color.Black;
            this.ModelView.Location = new System.Drawing.Point(0, 0);
            this.ModelView.Name = "ModelView";
            this.ModelView.Size = new System.Drawing.Size(418, 568);
            this.ModelView.TabIndex = 1;
            this.ModelView.Text = "PCK Model View";
            // 
            // SkinPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 568);
            this.Controls.Add(this.ModelView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SkinPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SkinPreview";
            this.ResumeLayout(false);

        }

        #endregion

        private PckStudio.Rendering.SkinRenderer ModelView;
    }
}