namespace PckStudio
{
    partial class CreateTexturePackPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTexturePackPrompt));
            this.TextLabel = new MetroFramework.Controls.MetroLabel();
            this.OKButton = new MetroFramework.Controls.MetroButton();
            this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
            this.resolutionComboBox = new MetroFramework.Controls.MetroComboBox();
            this.label1 = new MetroFramework.Controls.MetroLabel();
            this.createSkinsPckCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            this.TextLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.Name = "OKButton";
            this.OKButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.OKButton.UseSelectable = true;
            this.OKButton.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // InputTextBox
            // 
            // 
            // 
            // 
            this.InputTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.InputTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.InputTextBox.CustomButton.Name = "";
            this.InputTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.InputTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.InputTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.InputTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.InputTextBox.CustomButton.UseSelectable = true;
            this.InputTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.InputTextBox.Lines = new string[0];
            resources.ApplyResources(this.InputTextBox, "InputTextBox");
            this.InputTextBox.MaxLength = 255;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.PasswordChar = '\0';
            this.InputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.InputTextBox.SelectedText = "";
            this.InputTextBox.SelectionLength = 0;
            this.InputTextBox.SelectionStart = 0;
            this.InputTextBox.ShortcutsEnabled = true;
            this.InputTextBox.Style = MetroFramework.MetroColorStyle.White;
            this.InputTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.InputTextBox.UseSelectable = true;
            this.InputTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.InputTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // resolutionComboBox
            // 
            this.resolutionComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.resolutionComboBox, "resolutionComboBox");
            this.resolutionComboBox.Name = "resolutionComboBox";
            this.resolutionComboBox.Style = MetroFramework.MetroColorStyle.Silver;
            this.resolutionComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.resolutionComboBox.UseSelectable = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            this.label1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // createSkinsPckCheckBox
            // 
            resources.ApplyResources(this.createSkinsPckCheckBox, "createSkinsPckCheckBox");
            this.createSkinsPckCheckBox.Name = "createSkinsPckCheckBox";
            this.createSkinsPckCheckBox.Style = MetroFramework.MetroColorStyle.White;
            this.createSkinsPckCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.createSkinsPckCheckBox.UseSelectable = true;
            // 
            // CreateTexturePackPrompt
            // 
            this.AcceptButton = this.OKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.createSkinsPckCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.resolutionComboBox);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.TextLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTexturePackPrompt";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public MetroFramework.Controls.MetroButton OKButton;
        public MetroFramework.Controls.MetroLabel TextLabel;
        private MetroFramework.Controls.MetroTextBox InputTextBox;
		private MetroFramework.Controls.MetroComboBox resolutionComboBox;
		public MetroFramework.Controls.MetroLabel label1;
        private MetroFramework.Controls.MetroCheckBox createSkinsPckCheckBox;
    }
}