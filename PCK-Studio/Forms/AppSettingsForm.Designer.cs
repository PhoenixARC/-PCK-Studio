namespace PckStudio.Forms
{
    partial class AppSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppSettingsForm));
            this.SettingToolTip = new MetroFramework.Components.MetroToolTip();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // SettingToolTip
            // 
            this.SettingToolTip.Style = MetroFramework.MetroColorStyle.White;
            this.SettingToolTip.StyleManager = null;
            this.SettingToolTip.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(20, 60);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(3);
            this.flowLayoutPanel.Size = new System.Drawing.Size(487, 190);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // AppSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 270);
            this.Controls.Add(this.flowLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettingsForm";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppBehaviorSettingsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Components.MetroToolTip SettingToolTip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    }
}