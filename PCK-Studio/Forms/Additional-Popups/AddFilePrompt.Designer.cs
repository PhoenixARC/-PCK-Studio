namespace PckStudio.Popups
{
    partial class AddFilePrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddFilePrompt));
            this.TextLabel = new System.Windows.Forms.Label();
            this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FileTypeComboBox = new MetroFramework.Controls.MetroComboBox();
            this.AddBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            // 
            // InputTextBox
            // 
            // 
            // 
            // 
            this.InputTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.InputTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.InputTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.InputTextBox.CustomButton.Margin = ((System.Windows.Forms.Padding)(resources.GetObject("resource.Margin")));
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // FileTypeComboBox
            // 
            this.FileTypeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.FileTypeComboBox, "FileTypeComboBox");
            this.FileTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("FileTypeComboBox.Items"),
            resources.GetString("FileTypeComboBox.Items1"),
            resources.GetString("FileTypeComboBox.Items2"),
            resources.GetString("FileTypeComboBox.Items3"),
            resources.GetString("FileTypeComboBox.Items4"),
            resources.GetString("FileTypeComboBox.Items5"),
            resources.GetString("FileTypeComboBox.Items6"),
            resources.GetString("FileTypeComboBox.Items7"),
            resources.GetString("FileTypeComboBox.Items8"),
            resources.GetString("FileTypeComboBox.Items9"),
            resources.GetString("FileTypeComboBox.Items10"),
            resources.GetString("FileTypeComboBox.Items11"),
            resources.GetString("FileTypeComboBox.Items12"),
            resources.GetString("FileTypeComboBox.Items13"),
            resources.GetString("FileTypeComboBox.Items14")});
            this.FileTypeComboBox.Name = "FileTypeComboBox";
            this.FileTypeComboBox.Style = MetroFramework.MetroColorStyle.Blue;
            this.FileTypeComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FileTypeComboBox.UseSelectable = true;
            // 
            // AddBtn
            // 
            resources.ApplyResources(this.AddBtn, "AddBtn");
            this.AddBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.AddBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.AddBtn.BorderRadius = 10;
            this.AddBtn.BorderSize = 1;
            this.AddBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.AddBtn.FlatAppearance.BorderSize = 0;
            this.AddBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.AddBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.AddBtn.ForeColor = System.Drawing.Color.White;
            this.AddBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.AddBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.AddBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.AddBtn.Image = global::PckStudio.Properties.Resources.plus___24px;
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.TextColor = System.Drawing.Color.White;
            this.AddBtn.UseVisualStyleBackColor = false;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // AddFilePrompt
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.FileTypeComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.TextLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddFilePrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label TextLabel;
        private MetroFramework.Controls.MetroTextBox InputTextBox;
		public System.Windows.Forms.Label label1;
		private MetroFramework.Controls.MetroComboBox FileTypeComboBox;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton AddBtn;
    }
}