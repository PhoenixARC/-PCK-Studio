namespace PckStudio.Forms.Utilities
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
            this.autoSaveCheckBox = new CBH.Controls.CrEaTiiOn_CustomCheckBox();
            this.endianCheckBox = new CBH.Controls.CrEaTiiOn_CustomCheckBox();
            this.autoUpdateCheckBox = new CBH.Controls.CrEaTiiOn_CustomCheckBox();
            this.autoLoadPckCheckBox = new CBH.Controls.CrEaTiiOn_CustomCheckBox();
            this.showPresenceCheckBox = new CBH.Controls.CrEaTiiOn_CustomCheckBox();
            this.SuspendLayout();
            // 
            // SettingToolTip
            // 
            this.SettingToolTip.Style = MetroFramework.MetroColorStyle.White;
            this.SettingToolTip.StyleManager = null;
            this.SettingToolTip.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // autoSaveCheckBox
            // 
            this.autoSaveCheckBox.BadgeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.autoSaveCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.autoSaveCheckBox.CheckboxCheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(166)))));
            this.autoSaveCheckBox.CheckboxColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.autoSaveCheckBox.CheckboxHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(156)))));
            this.autoSaveCheckBox.CheckboxStyle = CBH.Controls.CrEaTiiOn_CustomCheckBox.Style.Material;
            this.autoSaveCheckBox.Checked = false;
            this.autoSaveCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.autoSaveCheckBox.ForeColor = System.Drawing.Color.White;
            this.autoSaveCheckBox.Location = new System.Drawing.Point(12, 16);
            this.autoSaveCheckBox.Name = "autoSaveCheckBox";
            this.autoSaveCheckBox.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.autoSaveCheckBox.Size = new System.Drawing.Size(76, 15);
            this.autoSaveCheckBox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.autoSaveCheckBox.TabIndex = 0;
            this.autoSaveCheckBox.Text = "Auto Save";
            this.autoSaveCheckBox.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.autoSaveCheckBox.TickThickness = 2;
            this.SettingToolTip.SetToolTip(this.autoSaveCheckBox, "Whether to automatically save changes inside of file editor such as the loc edito" +
        "r");
            this.autoSaveCheckBox.CheckedStateChanged += new System.EventHandler(this.autoSaveCheckBox_CheckedChanged);
            // 
            // endianCheckBox
            // 
            this.endianCheckBox.BadgeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.endianCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.endianCheckBox.CheckboxCheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(166)))));
            this.endianCheckBox.CheckboxColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.endianCheckBox.CheckboxHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(156)))));
            this.endianCheckBox.CheckboxStyle = CBH.Controls.CrEaTiiOn_CustomCheckBox.Style.Material;
            this.endianCheckBox.Checked = false;
            this.endianCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.endianCheckBox.ForeColor = System.Drawing.Color.White;
            this.endianCheckBox.Location = new System.Drawing.Point(12, 37);
            this.endianCheckBox.Name = "endianCheckBox";
            this.endianCheckBox.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.endianCheckBox.Size = new System.Drawing.Size(75, 15);
            this.endianCheckBox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.endianCheckBox.TabIndex = 1;
            this.endianCheckBox.Text = "Open Vita";
            this.endianCheckBox.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.endianCheckBox.TickThickness = 2;
            this.SettingToolTip.SetToolTip(this.endianCheckBox, "Whether to automatically set the \'Open as Switch/Vita pck\' checkbox");
            this.endianCheckBox.CheckedStateChanged += new System.EventHandler(this.endianCheckBox_CheckedChanged);
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.BadgeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.autoUpdateCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.autoUpdateCheckBox.CheckboxCheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(166)))));
            this.autoUpdateCheckBox.CheckboxColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.autoUpdateCheckBox.CheckboxHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(156)))));
            this.autoUpdateCheckBox.CheckboxStyle = CBH.Controls.CrEaTiiOn_CustomCheckBox.Style.Material;
            this.autoUpdateCheckBox.Checked = false;
            this.autoUpdateCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.autoUpdateCheckBox.Enabled = false;
            this.autoUpdateCheckBox.ForeColor = System.Drawing.Color.White;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(12, 58);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(90, 15);
            this.autoUpdateCheckBox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.autoUpdateCheckBox.TabIndex = 2;
            this.autoUpdateCheckBox.Text = "Auto Update";
            this.autoUpdateCheckBox.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.autoUpdateCheckBox.TickThickness = 2;
            this.SettingToolTip.SetToolTip(this.autoUpdateCheckBox, "Whether to automatically check for updates");
            // 
            // autoLoadPckCheckBox
            // 
            this.autoLoadPckCheckBox.BadgeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.autoLoadPckCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.autoLoadPckCheckBox.CheckboxCheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(166)))));
            this.autoLoadPckCheckBox.CheckboxColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.autoLoadPckCheckBox.CheckboxHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(156)))));
            this.autoLoadPckCheckBox.CheckboxStyle = CBH.Controls.CrEaTiiOn_CustomCheckBox.Style.Material;
            this.autoLoadPckCheckBox.Checked = false;
            this.autoLoadPckCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.autoLoadPckCheckBox.ForeColor = System.Drawing.Color.White;
            this.autoLoadPckCheckBox.Location = new System.Drawing.Point(12, 79);
            this.autoLoadPckCheckBox.Name = "autoLoadPckCheckBox";
            this.autoLoadPckCheckBox.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.autoLoadPckCheckBox.Size = new System.Drawing.Size(331, 15);
            this.autoLoadPckCheckBox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.autoLoadPckCheckBox.TabIndex = 3;
            this.autoLoadPckCheckBox.Text = "Auto load additional pck files (also known as SubPCK files)";
            this.autoLoadPckCheckBox.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.autoLoadPckCheckBox.TickThickness = 2;
            this.SettingToolTip.SetToolTip(this.autoLoadPckCheckBox, "Whether to automatically load files inside that end in .pck");
            this.autoLoadPckCheckBox.CheckedStateChanged += new System.EventHandler(this.autoLoadPckCheckBox_CheckedChanged);
            // 
            // showPresenceCheckBox
            // 
            this.showPresenceCheckBox.BadgeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.showPresenceCheckBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.showPresenceCheckBox.CheckboxCheckColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(166)))));
            this.showPresenceCheckBox.CheckboxColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.showPresenceCheckBox.CheckboxHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(34)))), ((int)(((byte)(156)))));
            this.showPresenceCheckBox.CheckboxStyle = CBH.Controls.CrEaTiiOn_CustomCheckBox.Style.Material;
            this.showPresenceCheckBox.Checked = false;
            this.showPresenceCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.showPresenceCheckBox.ForeColor = System.Drawing.Color.White;
            this.showPresenceCheckBox.Location = new System.Drawing.Point(12, 100);
            this.showPresenceCheckBox.Name = "showPresenceCheckBox";
            this.showPresenceCheckBox.PixelOffsetType = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.showPresenceCheckBox.Size = new System.Drawing.Size(171, 15);
            this.showPresenceCheckBox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.showPresenceCheckBox.TabIndex = 4;
            this.showPresenceCheckBox.Text = "Show Discord Rich Presence";
            this.showPresenceCheckBox.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.showPresenceCheckBox.TickThickness = 2;
            this.SettingToolTip.SetToolTip(this.showPresenceCheckBox, "Whether to show a rich presence on discord");
            this.showPresenceCheckBox.CheckedStateChanged += new System.EventHandler(this.showPresenceCheckBox_CheckedChanged);
            // 
            // AppSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(527, 270);
            this.Controls.Add(this.showPresenceCheckBox);
            this.Controls.Add(this.autoLoadPckCheckBox);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.Controls.Add(this.endianCheckBox);
            this.Controls.Add(this.autoSaveCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppBehaviorSettingsForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Components.MetroToolTip SettingToolTip;
        private CBH.Controls.CrEaTiiOn_CustomCheckBox autoSaveCheckBox;
        private CBH.Controls.CrEaTiiOn_CustomCheckBox endianCheckBox;
        private CBH.Controls.CrEaTiiOn_CustomCheckBox autoUpdateCheckBox;
        private CBH.Controls.CrEaTiiOn_CustomCheckBox autoLoadPckCheckBox;
        private CBH.Controls.CrEaTiiOn_CustomCheckBox showPresenceCheckBox;
    }
}