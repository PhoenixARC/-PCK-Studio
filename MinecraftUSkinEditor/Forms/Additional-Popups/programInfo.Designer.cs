namespace PckStudio
{
    partial class programInfo
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
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
			this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
			this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
			this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
			this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::PckStudio.Properties.Resources.Splash;
			this.pictureBox1.Location = new System.Drawing.Point(14, 33);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(575, 293);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// metroLabel1
			// 
			this.metroLabel1.AutoSize = true;
			this.metroLabel1.Location = new System.Drawing.Point(6, 333);
			this.metroLabel1.Name = "metroLabel1";
			this.metroLabel1.Size = new System.Drawing.Size(250, 19);
			this.metroLabel1.TabIndex = 1;
			this.metroLabel1.Text = "Restored and maintained by PhoenixARC";
			this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.metroLabel1.Click += new System.EventHandler(this.metroLabel1_Click);
			// 
			// metroLabel2
			// 
			this.metroLabel2.AutoSize = true;
			this.metroLabel2.Location = new System.Drawing.Point(331, 333);
			this.metroLabel2.Name = "metroLabel2";
			this.metroLabel2.Size = new System.Drawing.Size(269, 19);
			this.metroLabel2.TabIndex = 2;
			this.metroLabel2.Text = "Utilizing the Nobledez Website by Newagent";
			this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroLabel3
			// 
			this.metroLabel3.AutoSize = true;
			this.metroLabel3.Location = new System.Drawing.Point(4, 367);
			this.metroLabel3.Name = "metroLabel3";
			this.metroLabel3.Size = new System.Drawing.Size(212, 19);
			this.metroLabel3.TabIndex = 3;
			this.metroLabel3.Text = "3D skin renderer by Łukasz Rejman";
			this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroLabel4
			// 
			this.metroLabel4.AutoSize = true;
			this.metroLabel4.Location = new System.Drawing.Point(331, 367);
			this.metroLabel4.Name = "metroLabel4";
			this.metroLabel4.Size = new System.Drawing.Size(199, 19);
			this.metroLabel4.TabIndex = 4;
			this.metroLabel4.Text = "3D renderer found by Newagent";
			this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroLabel5
			// 
			this.metroLabel5.AutoSize = true;
			this.metroLabel5.Location = new System.Drawing.Point(4, 350);
			this.metroLabel5.Name = "metroLabel5";
			this.metroLabel5.Size = new System.Drawing.Size(236, 19);
			this.metroLabel5.TabIndex = 5;
			this.metroLabel5.Text = "Additional code and utilities by MattNL";
			this.metroLabel5.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// programInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
			this.ClientSize = new System.Drawing.Size(602, 392);
			this.Controls.Add(this.metroLabel1);
			this.Controls.Add(this.metroLabel5);
			this.Controls.Add(this.metroLabel4);
			this.Controls.Add(this.metroLabel3);
			this.Controls.Add(this.metroLabel2);
			this.Controls.Add(this.pictureBox1);
			this.DisplayHeader = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "programInfo";
			this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
			this.Resizable = false;
			this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Text = "programInfo";
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.Load += new System.EventHandler(this.programInfo_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel4;
		private MetroFramework.Controls.MetroLabel metroLabel5;
	}
}