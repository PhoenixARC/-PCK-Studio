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
            this.autoSaveCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.SettingToolTip = new MetroFramework.Components.MetroToolTip();
            this.endianCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.autoUpdateCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.autoLoadPckCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.showPresenceCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.grf_paramKeyComboBoxCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.usePrereleaseCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.SuspendLayout();
            // 
            // autoSaveCheckBox
            // 
            this.autoSaveCheckBox.AutoSize = true;
            this.autoSaveCheckBox.Location = new System.Drawing.Point(23, 63);
            this.autoSaveCheckBox.Name = "autoSaveCheckBox";
            this.autoSaveCheckBox.Size = new System.Drawing.Size(76, 15);
            this.autoSaveCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.autoSaveCheckBox.TabIndex = 0;
            this.autoSaveCheckBox.Text = "Auto Save";
            this.autoSaveCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.autoSaveCheckBox, "Whether to automatically save changes inside of file editor such as the loc edito" +
        "r");
            this.autoSaveCheckBox.UseSelectable = true;
            this.autoSaveCheckBox.CheckedChanged += new System.EventHandler(this.autoSaveCheckBox_CheckedChanged);
            // 
            // SettingToolTip
            // 
            this.SettingToolTip.Style = MetroFramework.MetroColorStyle.White;
            this.SettingToolTip.StyleManager = null;
            this.SettingToolTip.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // endianCheckBox
            // 
            this.endianCheckBox.AutoSize = true;
            this.endianCheckBox.Location = new System.Drawing.Point(23, 84);
            this.endianCheckBox.Name = "endianCheckBox";
            this.endianCheckBox.Size = new System.Drawing.Size(75, 15);
            this.endianCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.endianCheckBox.TabIndex = 1;
            this.endianCheckBox.Text = "Open Vita";
            this.endianCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.endianCheckBox, "Whether to automatically set the \'Open as Switch/Vita pck\' checkbox");
            this.endianCheckBox.UseSelectable = true;
            this.endianCheckBox.CheckedChanged += new System.EventHandler(this.endianCheckBox_CheckedChanged);
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(23, 105);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(90, 15);
            this.autoUpdateCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.autoUpdateCheckBox.TabIndex = 2;
            this.autoUpdateCheckBox.Text = "Auto Update";
            this.autoUpdateCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.autoUpdateCheckBox, "Whether to automatically check for updates");
            this.autoUpdateCheckBox.UseSelectable = true;
            this.autoUpdateCheckBox.CheckedChanged += new System.EventHandler(this.autoUpdateCheckBox_CheckedChanged);
            // 
            // autoLoadPckCheckBox
            // 
            this.autoLoadPckCheckBox.AutoSize = true;
            this.autoLoadPckCheckBox.Location = new System.Drawing.Point(23, 126);
            this.autoLoadPckCheckBox.Name = "autoLoadPckCheckBox";
            this.autoLoadPckCheckBox.Size = new System.Drawing.Size(331, 15);
            this.autoLoadPckCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.autoLoadPckCheckBox.TabIndex = 3;
            this.autoLoadPckCheckBox.Text = "Auto load additional pck files (also known as SubPCK files)";
            this.autoLoadPckCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.autoLoadPckCheckBox, "Whether to automatically load files inside that end in .pck");
            this.autoLoadPckCheckBox.UseSelectable = true;
            this.autoLoadPckCheckBox.CheckedChanged += new System.EventHandler(this.autoLoadPckCheckBox_CheckedChanged);
            // 
            // showPresenceCheckBox
            // 
            this.showPresenceCheckBox.AutoSize = true;
            this.showPresenceCheckBox.Location = new System.Drawing.Point(23, 147);
            this.showPresenceCheckBox.Name = "showPresenceCheckBox";
            this.showPresenceCheckBox.Size = new System.Drawing.Size(171, 15);
            this.showPresenceCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.showPresenceCheckBox.TabIndex = 4;
            this.showPresenceCheckBox.Text = "Show Discord Rich Presence";
            this.showPresenceCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.showPresenceCheckBox, "Whether to show a rich presence on discord");
            this.showPresenceCheckBox.UseSelectable = true;
            this.showPresenceCheckBox.CheckedChanged += new System.EventHandler(this.showPresenceCheckBox_CheckedChanged);
            // 
            // grf_paramKeyComboBoxCheckBox
            // 
            this.grf_paramKeyComboBoxCheckBox.AutoSize = true;
            this.grf_paramKeyComboBoxCheckBox.Location = new System.Drawing.Point(23, 169);
            this.grf_paramKeyComboBoxCheckBox.Name = "grf_paramKeyComboBoxCheckBox";
            this.grf_paramKeyComboBoxCheckBox.Size = new System.Drawing.Size(100, 15);
            this.grf_paramKeyComboBoxCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.grf_paramKeyComboBoxCheckBox.TabIndex = 5;
            this.grf_paramKeyComboBoxCheckBox.Text = "Select GRF Key";
            this.grf_paramKeyComboBoxCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.grf_paramKeyComboBoxCheckBox, "Use a combobox instead of typing the parameter key name");
            this.grf_paramKeyComboBoxCheckBox.UseSelectable = true;
            this.grf_paramKeyComboBoxCheckBox.CheckedChanged += new System.EventHandler(this.grf_paramKeyComboBoxCheckBox_CheckedChanged);
            // 
            // usePrereleaseCheckBox
            // 
            this.usePrereleaseCheckBox.AutoSize = true;
            this.usePrereleaseCheckBox.Location = new System.Drawing.Point(119, 105);
            this.usePrereleaseCheckBox.Name = "usePrereleaseCheckBox";
            this.usePrereleaseCheckBox.Size = new System.Drawing.Size(98, 15);
            this.usePrereleaseCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.usePrereleaseCheckBox.TabIndex = 6;
            this.usePrereleaseCheckBox.Text = "Use Beta Build";
            this.usePrereleaseCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SettingToolTip.SetToolTip(this.usePrereleaseCheckBox, "Whether to automatically check for updates");
            this.usePrereleaseCheckBox.UseSelectable = true;
            this.usePrereleaseCheckBox.Visible = false;
            // 
            // AppSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 270);
            this.Controls.Add(this.usePrereleaseCheckBox);
            this.Controls.Add(this.grf_paramKeyComboBoxCheckBox);
            this.Controls.Add(this.showPresenceCheckBox);
            this.Controls.Add(this.autoLoadPckCheckBox);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.Controls.Add(this.endianCheckBox);
            this.Controls.Add(this.autoSaveCheckBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettingsForm";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Text = "Application Settings";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppBehaviorSettingsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroCheckBox autoSaveCheckBox;
        private MetroFramework.Components.MetroToolTip SettingToolTip;
        private MetroFramework.Controls.MetroCheckBox endianCheckBox;
        private MetroFramework.Controls.MetroCheckBox autoUpdateCheckBox;
        private MetroFramework.Controls.MetroCheckBox autoLoadPckCheckBox;
        private MetroFramework.Controls.MetroCheckBox showPresenceCheckBox;
        private MetroFramework.Controls.MetroCheckBox grf_paramKeyComboBoxCheckBox;
        private MetroFramework.Controls.MetroCheckBox usePrereleaseCheckBox;
    }
}