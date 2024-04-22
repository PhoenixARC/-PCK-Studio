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
            this.InputTextBox = new MetroFramework.Controls.MetroTextBox();
            this.OKButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
            // 
            // 
            // 
            this.InputTextBox.CustomButton.Image = null;
            this.InputTextBox.CustomButton.Location = new System.Drawing.Point(61, 1);
            this.InputTextBox.CustomButton.Name = "";
            this.InputTextBox.CustomButton.Size = new System.Drawing.Size(283, 283);
            this.InputTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.InputTextBox.CustomButton.TabIndex = 1;
            this.InputTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.InputTextBox.CustomButton.UseSelectable = true;
            this.InputTextBox.CustomButton.Visible = false;
            this.InputTextBox.Lines = new string[0];
            this.InputTextBox.Location = new System.Drawing.Point(7, 9);
            this.InputTextBox.MaxLength = 32767;
            this.InputTextBox.Multiline = true;
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.PasswordChar = '\0';
            this.InputTextBox.PromptText = "Paste Multi-line text here...";
            this.InputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.InputTextBox.SelectedText = "";
            this.InputTextBox.SelectionLength = 0;
            this.InputTextBox.SelectionStart = 0;
            this.InputTextBox.ShortcutsEnabled = true;
            this.InputTextBox.ShowClearButton = true;
            this.InputTextBox.Size = new System.Drawing.Size(345, 285);
            this.InputTextBox.Style = MetroFramework.MetroColorStyle.Black;
            this.InputTextBox.TabIndex = 0;
            this.InputTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.InputTextBox.UseSelectable = true;
            this.InputTextBox.WaterMark = "Paste Multi-line text here...";
            this.InputTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.InputTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // OKButton
            // 
            this.OKButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.OKButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.OKButton.BorderRadius = 10;
            this.OKButton.BorderSize = 1;
            this.OKButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.OKButton.FlatAppearance.BorderSize = 0;
            this.OKButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.OKButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OKButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.OKButton.ForeColor = System.Drawing.Color.White;
            this.OKButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.OKButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.OKButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.OKButton.Image = ((System.Drawing.Image)(resources.GetObject("OKButton.Image")));
            this.OKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OKButton.Location = new System.Drawing.Point(119, 300);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(120, 40);
            this.OKButton.TabIndex = 21;
            this.OKButton.Text = "Save";
            this.OKButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.OKButton.TextColor = System.Drawing.Color.White;
            this.OKButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OKButton.UseVisualStyleBackColor = false;
            this.OKButton.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // TextPrompt
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(358, 348);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.InputTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.MinimizeBox = false;
            this.Name = "TextPrompt";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 40);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Text Prompt";
            this.Load += new System.EventHandler(this.RenamePrompt_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox InputTextBox;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton OKButton;
        private System.Windows.Forms.Label TextLabel;
        private MetroFramework.Controls.MetroLabel contextLabel;
    }
}