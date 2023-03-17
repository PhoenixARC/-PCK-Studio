namespace PckStudio
{
    partial class RenamePrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenamePrompt));
            this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
            this.TextLabel = new System.Windows.Forms.Label();
            this.RenameButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
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
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            // 
            // RenameButton
            // 
            this.RenameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.RenameButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.RenameButton.BorderRadius = 10;
            this.RenameButton.BorderSize = 1;
            this.RenameButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.RenameButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.RenameButton.FlatAppearance.BorderSize = 0;
            this.RenameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.RenameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            resources.ApplyResources(this.RenameButton, "RenameButton");
            this.RenameButton.ForeColor = System.Drawing.Color.White;
            this.RenameButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.RenameButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.RenameButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.TextColor = System.Drawing.Color.White;
            this.RenameButton.UseVisualStyleBackColor = false;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // RenamePrompt
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.RenameButton);
            this.Controls.Add(this.InputTextBox);
            this.Controls.Add(this.TextLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenamePrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label TextLabel;
        private MetroFramework.Controls.MetroTextBox InputTextBox;
        internal CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton RenameButton;
    }
}