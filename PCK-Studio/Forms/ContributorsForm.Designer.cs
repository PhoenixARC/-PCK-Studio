namespace PckStudio.Forms
{
    partial class ContributorsForm
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
            MetroFramework.Controls.MetroLabel metroLabel1;
            MetroFramework.Controls.MetroLabel metroLabel2;
            MetroFramework.Controls.MetroLabel metroLabel5;
            MetroFramework.Controls.MetroLabel metroLabel6;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContributorsForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buildLabel = new MetroFramework.Controls.MetroLabel();
            this.contributorsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            metroLabel5 = new MetroFramework.Controls.MetroLabel();
            metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Enabled = false;
            metroLabel1.Location = new System.Drawing.Point(27, 548);
            metroLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(168, 19);
            metroLabel1.TabIndex = 1;
            metroLabel1.Text = "Maintained by PhoenixARC";
            metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Enabled = false;
            metroLabel2.Location = new System.Drawing.Point(27, 526);
            metroLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(368, 19);
            metroLabel2.TabIndex = 2;
            metroLabel2.Text = "Originally created and developed by jam1garner && Nobledez";
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel5
            // 
            metroLabel5.AutoSize = true;
            metroLabel5.Enabled = false;
            metroLabel5.Location = new System.Drawing.Point(463, 526);
            metroLabel5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            metroLabel6.Location = new System.Drawing.Point(463, 548);
            metroLabel6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
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
            this.buildLabel.Location = new System.Drawing.Point(578, 7);
            this.buildLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.buildLabel.Name = "buildLabel";
            this.buildLabel.Size = new System.Drawing.Size(248, 62);
            this.buildLabel.TabIndex = 7;
            this.buildLabel.Text = "Build Information";
            this.buildLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.buildLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buildLabel.WrapToLine = true;
            // 
            // contributorsLayoutPanel
            // 
            this.contributorsLayoutPanel.AutoScroll = true;
            this.contributorsLayoutPanel.Location = new System.Drawing.Point(18, 72);
            this.contributorsLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
            this.contributorsLayoutPanel.Name = "contributorsLayoutPanel";
            this.contributorsLayoutPanel.Size = new System.Drawing.Size(798, 450);
            this.contributorsLayoutPanel.TabIndex = 8;
            // 
            // ContributorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(833, 532);
            this.Controls.Add(this.contributorsLayoutPanel);
            this.Controls.Add(this.buildLabel);
            this.Controls.Add(metroLabel6);
            this.Controls.Add(metroLabel1);
            this.Controls.Add(metroLabel5);
            this.Controls.Add(metroLabel2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(849, 571);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(849, 571);
            this.Name = "ContributorsForm";
            this.Text = "Contributors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private MetroFramework.Controls.MetroLabel buildLabel;
        private System.Windows.Forms.FlowLayoutPanel contributorsLayoutPanel;
    }
}