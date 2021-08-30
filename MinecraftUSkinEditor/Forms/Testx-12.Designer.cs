
namespace PckStudio.Forms
{
    partial class Testx_12
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
            this.minecraftModelView1 = new PckStudio.Models.MinecraftModelView(this.components);
            this.SuspendLayout();
            // 
            // minecraftModelView1
            // 
            this.minecraftModelView1.BackColor = System.Drawing.Color.Black;
            this.minecraftModelView1.BackGradientColor1 = System.Drawing.SystemColors.ActiveCaptionText;
            this.minecraftModelView1.BackGradientColor2 = System.Drawing.SystemColors.ActiveCaptionText;
            this.minecraftModelView1.BackgroundType = PckStudio.Models.BackgroundTypes.Color;
            this.minecraftModelView1.DegreesX = 0;
            this.minecraftModelView1.DegreesY = 0;
            this.minecraftModelView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minecraftModelView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minecraftModelView1.ForeColor = System.Drawing.Color.Black;
            this.minecraftModelView1.FOV = 70;
            this.minecraftModelView1.Location = new System.Drawing.Point(0, 0);
            this.minecraftModelView1.Name = "minecraftModelView1";
            this.minecraftModelView1.Projection = PckStudio.Models.ProjectionTypes.Perspective;
            this.minecraftModelView1.ShowUsername = false;
            this.minecraftModelView1.Size = new System.Drawing.Size(323, 375);
            this.minecraftModelView1.TabIndex = 0;
            this.minecraftModelView1.Text = "minecraftModelView1";
            this.minecraftModelView1.Username = "";
            // 
            // Testx_12
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 375);
            this.Controls.Add(this.minecraftModelView1);
            this.Name = "Testx_12";
            this.Text = "Skin Preview";
            this.Load += new System.EventHandler(this.Testx_12_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PckStudio.Models.MinecraftModelView minecraftModelView1;
    }
}