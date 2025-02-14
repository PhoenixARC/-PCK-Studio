namespace PckStudio.Forms.Features
{
    partial class PckManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PckManager));
            this.supportedPlatformComboBox = new MetroFramework.Controls.MetroComboBox();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Location = new System.Drawing.Point(308, 84);
            metroLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(90, 19);
            metroLabel1.Style = MetroFramework.MetroColorStyle.Black;
            metroLabel1.TabIndex = 1;
            metroLabel1.Text = "Console Type:";
            metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            metroLabel2.Location = new System.Drawing.Point(4, 0);
            metroLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(236, 41);
            metroLabel2.Style = MetroFramework.MetroColorStyle.Black;
            metroLabel2.TabIndex = 13;
            metroLabel2.Text = "Platform type:";
            metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // supportedPlatformComboBox
            // 
            this.supportedPlatformComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.supportedPlatformComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.supportedPlatformComboBox.FormattingEnabled = true;
            this.supportedPlatformComboBox.ItemHeight = 23;
            this.supportedPlatformComboBox.Location = new System.Drawing.Point(248, 4);
            this.supportedPlatformComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.supportedPlatformComboBox.Name = "supportedPlatformComboBox";
            this.supportedPlatformComboBox.PromptText = "Select Platform";
            this.supportedPlatformComboBox.Size = new System.Drawing.Size(237, 29);
            this.supportedPlatformComboBox.Style = MetroFramework.MetroColorStyle.Black;
            this.supportedPlatformComboBox.TabIndex = 0;
            this.supportedPlatformComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.supportedPlatformComboBox.UseSelectable = true;
            this.supportedPlatformComboBox.SelectionChangeCommitted += new System.EventHandler(this.supportedPlatformComboBox_SelectionChangeCommitted);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.supportedPlatformComboBox, 1, 0);
            this.mainPanel.Controls.Add(metroLabel2, 0, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.mainPanel.Size = new System.Drawing.Size(489, 739);
            this.mainPanel.TabIndex = 3;
            // 
            // PckManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(489, 739);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(metroLabel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(486, 733);
            this.Name = "PckManager";
            this.Text = "Pck Manager";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox supportedPlatformComboBox;
        private System.Windows.Forms.TableLayoutPanel mainPanel;
    }
}