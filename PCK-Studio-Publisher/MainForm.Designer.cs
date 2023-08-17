namespace PCK_Studio_Publisher
{
    partial class MainForm
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
            System.Windows.Forms.GroupBox groupBox2;
            this.repositoryGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ownerTextBox = new System.Windows.Forms.TextBox();
            this.repoTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.releaseInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bodyTextBox = new System.Windows.Forms.RichTextBox();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.prereleaseRadioButton = new System.Windows.Forms.RadioButton();
            this.publishReleaseButton = new System.Windows.Forms.Button();
            this.latestReleaseRadioButton = new System.Windows.Forms.RadioButton();
            this.assetGroupBox = new System.Windows.Forms.GroupBox();
            this.brosweAssetButton = new System.Windows.Forms.Button();
            this.assetOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tagTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.assetLabel = new System.Windows.Forms.Label();
            this.currentUserLoginLabel = new System.Windows.Forms.Label();
            this.currentUserNameLabel = new System.Windows.Forms.Label();
            this.accessTokenTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            this.repositoryGroupBox.SuspendLayout();
            this.releaseInfoGroupBox.SuspendLayout();
            this.assetGroupBox.SuspendLayout();
            groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // repositoryGroupBox
            // 
            this.repositoryGroupBox.AutoSize = true;
            this.repositoryGroupBox.Controls.Add(this.label2);
            this.repositoryGroupBox.Controls.Add(this.ownerTextBox);
            this.repositoryGroupBox.Controls.Add(this.repoTextBox);
            this.repositoryGroupBox.Controls.Add(this.label1);
            this.repositoryGroupBox.Enabled = false;
            this.repositoryGroupBox.Location = new System.Drawing.Point(11, 159);
            this.repositoryGroupBox.Name = "repositoryGroupBox";
            this.repositoryGroupBox.Size = new System.Drawing.Size(178, 87);
            this.repositoryGroupBox.TabIndex = 9;
            this.repositoryGroupBox.TabStop = false;
            this.repositoryGroupBox.Text = "Repository";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name";
            // 
            // ownerTextBox
            // 
            this.ownerTextBox.Location = new System.Drawing.Point(51, 22);
            this.ownerTextBox.Name = "ownerTextBox";
            this.ownerTextBox.Size = new System.Drawing.Size(121, 20);
            this.ownerTextBox.TabIndex = 2;
            // 
            // repoTextBox
            // 
            this.repoTextBox.Location = new System.Drawing.Point(51, 48);
            this.repoTextBox.Name = "repoTextBox";
            this.repoTextBox.Size = new System.Drawing.Size(121, 20);
            this.repoTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Owner";
            // 
            // releaseInfoGroupBox
            // 
            this.releaseInfoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.releaseInfoGroupBox.Controls.Add(this.label5);
            this.releaseInfoGroupBox.Controls.Add(this.tagTextBox);
            this.releaseInfoGroupBox.Controls.Add(this.label4);
            this.releaseInfoGroupBox.Controls.Add(this.label3);
            this.releaseInfoGroupBox.Controls.Add(this.bodyTextBox);
            this.releaseInfoGroupBox.Controls.Add(this.titleTextBox);
            this.releaseInfoGroupBox.Controls.Add(this.prereleaseRadioButton);
            this.releaseInfoGroupBox.Controls.Add(this.publishReleaseButton);
            this.releaseInfoGroupBox.Controls.Add(this.latestReleaseRadioButton);
            this.releaseInfoGroupBox.Enabled = false;
            this.releaseInfoGroupBox.Location = new System.Drawing.Point(196, 12);
            this.releaseInfoGroupBox.Name = "releaseInfoGroupBox";
            this.releaseInfoGroupBox.Size = new System.Drawing.Size(592, 426);
            this.releaseInfoGroupBox.TabIndex = 12;
            this.releaseInfoGroupBox.TabStop = false;
            this.releaseInfoGroupBox.Text = "Release infomartion";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Description";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 24);
            this.label3.TabIndex = 10;
            this.label3.Text = "Title";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bodyTextBox
            // 
            this.bodyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bodyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bodyTextBox.Location = new System.Drawing.Point(15, 94);
            this.bodyTextBox.Name = "bodyTextBox";
            this.bodyTextBox.Size = new System.Drawing.Size(550, 271);
            this.bodyTextBox.TabIndex = 6;
            this.bodyTextBox.Text = "";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(15, 43);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(435, 20);
            this.titleTextBox.TabIndex = 8;
            // 
            // prereleaseRadioButton
            // 
            this.prereleaseRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prereleaseRadioButton.AutoSize = true;
            this.prereleaseRadioButton.Location = new System.Drawing.Point(15, 371);
            this.prereleaseRadioButton.Name = "prereleaseRadioButton";
            this.prereleaseRadioButton.Size = new System.Drawing.Size(119, 17);
            this.prereleaseRadioButton.TabIndex = 0;
            this.prereleaseRadioButton.Text = "Set as a pre-release";
            this.prereleaseRadioButton.UseVisualStyleBackColor = true;
            // 
            // publishReleaseButton
            // 
            this.publishReleaseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.publishReleaseButton.Location = new System.Drawing.Point(457, 386);
            this.publishReleaseButton.Name = "publishReleaseButton";
            this.publishReleaseButton.Size = new System.Drawing.Size(108, 23);
            this.publishReleaseButton.TabIndex = 7;
            this.publishReleaseButton.Text = "Publish Release";
            this.publishReleaseButton.UseVisualStyleBackColor = true;
            this.publishReleaseButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.publishReleaseButton_MouseClick);
            // 
            // latestReleaseRadioButton
            // 
            this.latestReleaseRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.latestReleaseRadioButton.AutoSize = true;
            this.latestReleaseRadioButton.Checked = true;
            this.latestReleaseRadioButton.Location = new System.Drawing.Point(15, 394);
            this.latestReleaseRadioButton.Name = "latestReleaseRadioButton";
            this.latestReleaseRadioButton.Size = new System.Drawing.Size(138, 17);
            this.latestReleaseRadioButton.TabIndex = 1;
            this.latestReleaseRadioButton.TabStop = true;
            this.latestReleaseRadioButton.Text = "Set as the latest release";
            this.latestReleaseRadioButton.UseVisualStyleBackColor = true;
            // 
            // assetGroupBox
            // 
            this.assetGroupBox.Controls.Add(this.assetLabel);
            this.assetGroupBox.Controls.Add(this.brosweAssetButton);
            this.assetGroupBox.Enabled = false;
            this.assetGroupBox.Location = new System.Drawing.Point(13, 252);
            this.assetGroupBox.Name = "assetGroupBox";
            this.assetGroupBox.Size = new System.Drawing.Size(177, 145);
            this.assetGroupBox.TabIndex = 13;
            this.assetGroupBox.TabStop = false;
            this.assetGroupBox.Text = "Assets";
            // 
            // brosweAssetButton
            // 
            this.brosweAssetButton.Location = new System.Drawing.Point(6, 116);
            this.brosweAssetButton.Name = "brosweAssetButton";
            this.brosweAssetButton.Size = new System.Drawing.Size(75, 23);
            this.brosweAssetButton.TabIndex = 0;
            this.brosweAssetButton.Text = "Browse";
            this.brosweAssetButton.UseVisualStyleBackColor = true;
            this.brosweAssetButton.Click += new System.EventHandler(this.brosweAssetButton_Click);
            // 
            // tagTextBox
            // 
            this.tagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagTextBox.Location = new System.Drawing.Point(456, 43);
            this.tagTextBox.Name = "tagTextBox";
            this.tagTextBox.Size = new System.Drawing.Size(109, 20);
            this.tagTextBox.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(453, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Tag";
            // 
            // assetLabel
            // 
            this.assetLabel.AutoSize = true;
            this.assetLabel.Location = new System.Drawing.Point(6, 97);
            this.assetLabel.Name = "assetLabel";
            this.assetLabel.Size = new System.Drawing.Size(32, 13);
            this.assetLabel.TabIndex = 1;
            this.assetLabel.Text = "asset";
            // 
            // groupBox2
            // 
            groupBox2.AutoSize = true;
            groupBox2.Controls.Add(this.currentUserNameLabel);
            groupBox2.Controls.Add(this.currentUserLoginLabel);
            groupBox2.Enabled = false;
            groupBox2.Location = new System.Drawing.Point(12, 79);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(177, 74);
            groupBox2.TabIndex = 14;
            groupBox2.TabStop = false;
            groupBox2.Text = "Current login";
            // 
            // currentUserLoginLabel
            // 
            this.currentUserLoginLabel.AutoSize = true;
            this.currentUserLoginLabel.Location = new System.Drawing.Point(9, 20);
            this.currentUserLoginLabel.Name = "currentUserLoginLabel";
            this.currentUserLoginLabel.Size = new System.Drawing.Size(88, 13);
            this.currentUserLoginLabel.TabIndex = 0;
            this.currentUserLoginLabel.Text = "currentUserLogin";
            // 
            // currentUserNameLabel
            // 
            this.currentUserNameLabel.AutoSize = true;
            this.currentUserNameLabel.Location = new System.Drawing.Point(7, 45);
            this.currentUserNameLabel.Name = "currentUserNameLabel";
            this.currentUserNameLabel.Size = new System.Drawing.Size(90, 13);
            this.currentUserNameLabel.TabIndex = 1;
            this.currentUserNameLabel.Text = "currentUserName";
            this.currentUserNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // accessTokenTextBox
            // 
            this.accessTokenTextBox.Location = new System.Drawing.Point(6, 22);
            this.accessTokenTextBox.Name = "accessTokenTextBox";
            this.accessTokenTextBox.Size = new System.Drawing.Size(166, 20);
            this.accessTokenTextBox.TabIndex = 15;
            this.accessTokenTextBox.UseSystemPasswordChar = true;
            this.accessTokenTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.accessTokenTextBox_PreviewKeyDown);
            // 
            // groupBox4
            // 
            this.groupBox4.AutoSize = true;
            this.groupBox4.Controls.Add(this.accessTokenTextBox);
            this.groupBox4.Location = new System.Drawing.Point(13, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(178, 61);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Access Token";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(groupBox2);
            this.Controls.Add(this.assetGroupBox);
            this.Controls.Add(this.repositoryGroupBox);
            this.Controls.Add(this.releaseInfoGroupBox);
            this.Controls.Add(this.groupBox4);
            this.Name = "MainForm";
            this.repositoryGroupBox.ResumeLayout(false);
            this.repositoryGroupBox.PerformLayout();
            this.releaseInfoGroupBox.ResumeLayout(false);
            this.releaseInfoGroupBox.PerformLayout();
            this.assetGroupBox.ResumeLayout(false);
            this.assetGroupBox.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton prereleaseRadioButton;
        private System.Windows.Forms.RadioButton latestReleaseRadioButton;
        private System.Windows.Forms.TextBox ownerTextBox;
        private System.Windows.Forms.TextBox repoTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox bodyTextBox;
        private System.Windows.Forms.Button publishReleaseButton;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog assetOpenFileDialog;
        private System.Windows.Forms.Button brosweAssetButton;
        private System.Windows.Forms.TextBox tagTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox releaseInfoGroupBox;
        private System.Windows.Forms.Label assetLabel;
        private System.Windows.Forms.Label currentUserLoginLabel;
        private System.Windows.Forms.Label currentUserNameLabel;
        private System.Windows.Forms.TextBox accessTokenTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox repositoryGroupBox;
        private System.Windows.Forms.GroupBox assetGroupBox;
    }
}

