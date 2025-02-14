
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
            this.ModelView = new PckStudio.Models.MinecraftModelView(this.components);
            this.SuspendLayout();
            // 
            // ModelView
            // 
            this.ModelView.BackColor = System.Drawing.Color.DarkGray;
            this.ModelView.BackGradientColor1 = System.Drawing.SystemColors.ActiveCaptionText;
            this.ModelView.BackGradientColor2 = System.Drawing.SystemColors.ActiveCaptionText;
            this.ModelView.BackgroundType = PckStudio.Models.BackgroundTypes.Color;
            this.ModelView.DegreesX = 0;
            this.ModelView.DegreesY = 0;
            this.ModelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModelView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModelView.ForeColor = System.Drawing.Color.Black;
            this.ModelView.FOV = 90;
            this.ModelView.Location = new System.Drawing.Point(0, 0);
            this.ModelView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ModelView.Name = "ModelView";
            this.ModelView.Projection = PckStudio.Models.ProjectionTypes.Perspective;
            this.ModelView.Size = new System.Drawing.Size(487, 656);
            this.ModelView.TabIndex = 1;
            this.ModelView.Text = "PCK Model View";
            // 
            // SkinPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(487, 656);
            this.Controls.Add(this.ModelView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SkinPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SkinPreview";
            this.Load += new System.EventHandler(this.SkinPreview_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PckStudio.Models.MinecraftModelView ModelView;
    }
}