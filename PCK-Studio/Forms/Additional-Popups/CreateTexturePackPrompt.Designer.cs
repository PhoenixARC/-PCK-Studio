namespace PckStudio
{
    partial class CreateTexturePack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateTexturePack));
            this.OkButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.label1 = new System.Windows.Forms.Label();
            this.metroComboBox1 = new MetroFramework.Controls.MetroComboBox();
            this.TextLabel = new System.Windows.Forms.Label();
            this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
            this.createSkinsPckCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OkButton
            // 
            this.OkButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.OkButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.OkButton.BorderRadius = 10;
            this.OkButton.BorderSize = 1;
            this.OkButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.OkButton.FlatAppearance.BorderSize = 0;
            this.OkButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.OkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.ForeColor = System.Drawing.Color.White;
            this.OkButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.OkButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.OkButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.OkButton.Name = "OkButton";
            this.OkButton.TextColor = System.Drawing.Color.White;
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.LockPCKButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // metroComboBox1
            // 
            this.metroComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.metroComboBox1.ForeColor = System.Drawing.Color.White;
            this.metroComboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.metroComboBox1, "metroComboBox1");
            this.metroComboBox1.Items.AddRange(new object[] {
            resources.GetString("metroComboBox1.Items"),
            resources.GetString("metroComboBox1.Items1"),
            resources.GetString("metroComboBox1.Items2"),
            resources.GetString("metroComboBox1.Items3"),
            resources.GetString("metroComboBox1.Items4"),
            resources.GetString("metroComboBox1.Items5"),
            resources.GetString("metroComboBox1.Items6"),
            resources.GetString("metroComboBox1.Items7"),
            resources.GetString("metroComboBox1.Items8")});
            this.metroComboBox1.Name = "metroComboBox1";
            this.metroComboBox1.Style = MetroFramework.MetroColorStyle.Silver;
            this.metroComboBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroComboBox1.UseCustomBackColor = true;
            this.metroComboBox1.UseCustomForeColor = true;
            this.metroComboBox1.UseSelectable = true;
            // 
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            // 
            // InputTextBox
            // 
            this.InputTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            // 
            // 
            // 
            this.InputTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.InputTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.InputTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.InputTextBox.CustomButton.Name = "";
            this.InputTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.InputTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.InputTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.InputTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.InputTextBox.CustomButton.UseSelectable = true;
            this.InputTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.InputTextBox.ForeColor = System.Drawing.Color.White;
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
            this.InputTextBox.UseCustomBackColor = true;
            this.InputTextBox.UseCustomForeColor = true;
            this.InputTextBox.UseSelectable = true;
            this.InputTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.InputTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // createSkinsPckCheckBox
            // 
            resources.ApplyResources(this.createSkinsPckCheckBox, "createSkinsPckCheckBox");
            this.createSkinsPckCheckBox.Name = "createSkinsPckCheckBox";
            this.createSkinsPckCheckBox.UseVisualStyleBackColor = true;
            // 
            // CreateTexturePack
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.createSkinsPckCheckBox);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.metroComboBox1);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.TextLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTexturePack";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label TextLabel;
        private MetroFramework.Controls.MetroTextBox InputTextBox;
		private MetroFramework.Controls.MetroComboBox metroComboBox1;
		public System.Windows.Forms.Label label1;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton OkButton;
        private System.Windows.Forms.CheckBox createSkinsPckCheckBox;
    }
}