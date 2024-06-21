namespace PckStudio.ToolboxItems
{
    partial class GithubUserPanel
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
            this.userPictureBox = new System.Windows.Forms.PictureBox();
            this.userNameLabel = new MetroFramework.Controls.MetroLabel();
            this.aboutButton = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // userPictureBox
            // 
            this.userPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.userPictureBox.Location = new System.Drawing.Point(8, 34);
            this.userPictureBox.Name = "userPictureBox";
            this.userPictureBox.Size = new System.Drawing.Size(100, 100);
            this.userPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.userPictureBox.TabIndex = 0;
            this.userPictureBox.TabStop = false;
            // 
            // userNameLabel
            // 
            this.userNameLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.userNameLabel.Location = new System.Drawing.Point(8, 6);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(100, 25);
            this.userNameLabel.TabIndex = 0;
            this.userNameLabel.Text = "user name";
            this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.userNameLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(8, 140);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(100, 23);
            this.aboutButton.TabIndex = 0;
            this.aboutButton.Text = "about button";
            this.aboutButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.aboutButton.UseSelectable = true;
            // 
            // GithubUserPanel
            // 
            this.Controls.Add(this.userPictureBox);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.userNameLabel);
            this.MaximumSize = new System.Drawing.Size(115, 170);
            this.MinimumSize = new System.Drawing.Size(115, 170);
            this.Name = "GithubUserPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(115, 170);
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox userPictureBox;
        private MetroFramework.Controls.MetroLabel userNameLabel;
        private MetroFramework.Controls.MetroButton aboutButton;
    }
}
