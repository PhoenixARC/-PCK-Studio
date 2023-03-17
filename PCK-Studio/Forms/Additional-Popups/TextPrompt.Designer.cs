namespace PckStudio.Forms.Additional_Popups
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
            this.PromptTextBox = new MetroFramework.Controls.MetroTextBox();
            this.CancelButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SaveButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // PromptTextBox
            // 
            // 
            // 
            // 
            this.PromptTextBox.CustomButton.Image = null;
            this.PromptTextBox.CustomButton.Location = new System.Drawing.Point(61, 1);
            this.PromptTextBox.CustomButton.Name = "";
            this.PromptTextBox.CustomButton.Size = new System.Drawing.Size(283, 283);
            this.PromptTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.PromptTextBox.CustomButton.TabIndex = 1;
            this.PromptTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.PromptTextBox.CustomButton.UseSelectable = true;
            this.PromptTextBox.CustomButton.Visible = false;
            this.PromptTextBox.Lines = new string[0];
            this.PromptTextBox.Location = new System.Drawing.Point(7, 9);
            this.PromptTextBox.MaxLength = 32767;
            this.PromptTextBox.Multiline = true;
            this.PromptTextBox.Name = "PromptTextBox";
            this.PromptTextBox.PasswordChar = '\0';
            this.PromptTextBox.PromptText = "Paste Multi-line text here...";
            this.PromptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.PromptTextBox.SelectedText = "";
            this.PromptTextBox.SelectionLength = 0;
            this.PromptTextBox.SelectionStart = 0;
            this.PromptTextBox.ShortcutsEnabled = true;
            this.PromptTextBox.ShowClearButton = true;
            this.PromptTextBox.Size = new System.Drawing.Size(345, 285);
            this.PromptTextBox.Style = MetroFramework.MetroColorStyle.Black;
            this.PromptTextBox.TabIndex = 0;
            this.PromptTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PromptTextBox.UseSelectable = true;
            this.PromptTextBox.WaterMark = "Paste Multi-line text here...";
            this.PromptTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.PromptTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // CancelButton
            // 
            this.CancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelButton.BorderRadius = 10;
            this.CancelButton.BorderSize = 1;
            this.CancelButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelButton.FlatAppearance.BorderSize = 0;
            this.CancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.CancelButton.ForeColor = System.Drawing.Color.White;
            this.CancelButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelButton.Image = ((System.Drawing.Image)(resources.GetObject("CancelButton.Image")));
            this.CancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelButton.Location = new System.Drawing.Point(182, 300);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(120, 40);
            this.CancelButton.TabIndex = 22;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelButton.TextColor = System.Drawing.Color.White;
            this.CancelButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelButton.UseVisualStyleBackColor = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SaveButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SaveButton.BorderRadius = 10;
            this.SaveButton.BorderSize = 1;
            this.SaveButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SaveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.SaveButton.ForeColor = System.Drawing.Color.White;
            this.SaveButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SaveButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SaveButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SaveButton.Location = new System.Drawing.Point(56, 300);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(120, 40);
            this.SaveButton.TabIndex = 21;
            this.SaveButton.Text = "Save";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveButton.TextColor = System.Drawing.Color.White;
            this.SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // TextPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(358, 348);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.PromptTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(0, 0);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 335);
            this.Name = "TextPrompt";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 40);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox PromptTextBox;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton CancelButton;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton SaveButton;
    }
}