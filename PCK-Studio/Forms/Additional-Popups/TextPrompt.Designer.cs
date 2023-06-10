namespace PckStudio
{
    partial class TextPrompt
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextPrompt));
			this.TextLabel = new System.Windows.Forms.Label();
			this.OKButton = new System.Windows.Forms.Button();
			this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
			this.contextLabel = new MetroFramework.Controls.MetroLabel();
			this.SuspendLayout();
			// 
			// TextLabel
			// 
			resources.ApplyResources(this.TextLabel, "TextLabel");
			this.TextLabel.ForeColor = System.Drawing.Color.White;
			this.TextLabel.Name = "TextLabel";
			// 
			// OKButton
			// 
			resources.ApplyResources(this.OKButton, "OKButton");
			this.OKButton.ForeColor = System.Drawing.Color.White;
			this.OKButton.Name = "OKButton";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKBtn_Click);
			// 
			// InputTextBox
			// 
			resources.ApplyResources(this.InputTextBox, "InputTextBox");
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
			// contextLabel
			// 
			resources.ApplyResources(this.contextLabel, "contextLabel");
			this.contextLabel.FontSize = MetroFramework.MetroLabelSize.Small;
			this.contextLabel.Name = "contextLabel";
			this.contextLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.contextLabel.WrapToLine = true;
			// 
			// RenamePrompt
			// 
			this.AcceptButton = this.OKButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.contextLabel);
			this.Controls.Add(this.InputTextBox);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.TextLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RenamePrompt";
			this.Resizable = false;
			this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.Load += new System.EventHandler(this.RenamePrompt_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button OKButton;
        public System.Windows.Forms.Label TextLabel;
        private MetroFramework.Controls.MetroTextBox InputTextBox;
		public MetroFramework.Controls.MetroLabel contextLabel;
	}
}