namespace PckStudio.Forms
{
    partial class CreditsForm
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
            System.Windows.Forms.PictureBox pictureBox1;
            MetroFramework.Controls.MetroLabel metroLabel1;
            MetroFramework.Controls.MetroLabel metroLabel2;
            MetroFramework.Controls.MetroLabel metroLabel3;
            MetroFramework.Controls.MetroLabel metroLabel4;
            MetroFramework.Controls.MetroLabel metroLabel5;
            MetroFramework.Controls.MetroLabel metroLabel6;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreditsForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buildLabel = new MetroFramework.Controls.MetroLabel();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            metroLabel3 = new MetroFramework.Controls.MetroLabel();
            metroLabel4 = new MetroFramework.Controls.MetroLabel();
            metroLabel5 = new MetroFramework.Controls.MetroLabel();
            metroLabel6 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.Enabled = false;
            pictureBox1.Image = global::PckStudio.Properties.Resources.Splash;
            pictureBox1.Location = new System.Drawing.Point(4, 5);
            pictureBox1.Margin = new System.Windows.Forms.Padding(0, 0, 11, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(550, 293);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Enabled = false;
            metroLabel1.Location = new System.Drawing.Point(4, 301);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(250, 19);
            metroLabel1.TabIndex = 1;
            metroLabel1.Text = "Restored and maintained by PhoenixARC";
            metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Enabled = false;
            metroLabel2.Location = new System.Drawing.Point(314, 301);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(269, 19);
            metroLabel2.TabIndex = 2;
            metroLabel2.Text = "Utilizing the Nobledez Website by Newagent";
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel3
            // 
            metroLabel3.AutoSize = true;
            metroLabel3.Enabled = false;
            metroLabel3.Location = new System.Drawing.Point(314, 339);
            metroLabel3.Name = "metroLabel3";
            metroLabel3.Size = new System.Drawing.Size(212, 19);
            metroLabel3.TabIndex = 3;
            metroLabel3.Text = "3D skin renderer by Łukasz Rejman";
            metroLabel3.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel4
            // 
            metroLabel4.AutoSize = true;
            metroLabel4.Enabled = false;
            metroLabel4.Location = new System.Drawing.Point(314, 320);
            metroLabel4.Name = "metroLabel4";
            metroLabel4.Size = new System.Drawing.Size(199, 19);
            metroLabel4.TabIndex = 4;
            metroLabel4.Text = "3D renderer found by Newagent";
            metroLabel4.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel5
            // 
            metroLabel5.AutoSize = true;
            metroLabel5.Enabled = false;
            metroLabel5.Location = new System.Drawing.Point(4, 320);
            metroLabel5.Name = "metroLabel5";
            metroLabel5.Size = new System.Drawing.Size(300, 19);
            metroLabel5.TabIndex = 5;
            metroLabel5.Text = "Additional development by MattNL and Miku-666";
            metroLabel5.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel6
            // 
            metroLabel6.AutoSize = true;
            metroLabel6.Enabled = false;
            metroLabel6.Location = new System.Drawing.Point(4, 339);
            metroLabel6.Name = "metroLabel6";
            metroLabel6.Size = new System.Drawing.Size(203, 19);
            metroLabel6.TabIndex = 6;
            metroLabel6.Text = "Code base overhaul by Miku-666";
            metroLabel6.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // buildLabel
            // 
            this.buildLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(34)))), ((int)(((byte)(56)))));
            this.buildLabel.Enabled = false;
            this.buildLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.buildLabel.Location = new System.Drawing.Point(314, 30);
            this.buildLabel.Name = "buildLabel";
            this.buildLabel.Size = new System.Drawing.Size(212, 171);
            this.buildLabel.TabIndex = 7;
            this.buildLabel.Text = "Build Information";
            this.buildLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.buildLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CreditsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 364);
            this.Controls.Add(this.buildLabel);
            this.Controls.Add(metroLabel6);
            this.Controls.Add(metroLabel1);
            this.Controls.Add(metroLabel5);
            this.Controls.Add(metroLabel4);
            this.Controls.Add(metroLabel3);
            this.Controls.Add(metroLabel2);
            this.Controls.Add(pictureBox1);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreditsForm";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Text = "programInfo";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private MetroFramework.Controls.MetroLabel buildLabel;
    }
}