namespace PckStudio.Forms.Additional_Popups
{
    partial class MultiTextPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiTextPrompt));
            this.PromptTextBox = new MetroFramework.Controls.MetroTextBox();
            this.cancelBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.okBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // PromptTextBox
            // 
            // 
            // 
            // 
            this.PromptTextBox.CustomButton.Image = null;
            this.PromptTextBox.CustomButton.Location = new System.Drawing.Point(71, 1);
            this.PromptTextBox.CustomButton.Margin = new System.Windows.Forms.Padding(4);
            this.PromptTextBox.CustomButton.Name = "";
            this.PromptTextBox.CustomButton.Size = new System.Drawing.Size(363, 363);
            this.PromptTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.PromptTextBox.CustomButton.TabIndex = 1;
            this.PromptTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.PromptTextBox.CustomButton.UseSelectable = true;
            this.PromptTextBox.CustomButton.Visible = false;
            this.PromptTextBox.Lines = new string[0];
            this.PromptTextBox.Location = new System.Drawing.Point(21, 18);
            this.PromptTextBox.Margin = new System.Windows.Forms.Padding(4);
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
            this.PromptTextBox.Size = new System.Drawing.Size(435, 365);
            this.PromptTextBox.Style = MetroFramework.MetroColorStyle.Black;
            this.PromptTextBox.TabIndex = 0;
            this.PromptTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PromptTextBox.UseSelectable = true;
            this.PromptTextBox.WaterMark = "Paste Multi-line text here...";
            this.PromptTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.PromptTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // cancelBtn
            // 
            this.cancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.cancelBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.cancelBtn.BorderRadius = 10;
            this.cancelBtn.BorderSize = 1;
            this.cancelBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.cancelBtn.FlatAppearance.BorderSize = 0;
            this.cancelBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.cancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cancelBtn.ForeColor = System.Drawing.Color.White;
            this.cancelBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.cancelBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.cancelBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.cancelBtn.Image = ((System.Drawing.Image)(resources.GetObject("cancelBtn.Image")));
            this.cancelBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelBtn.Location = new System.Drawing.Point(241, 399);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(120, 40);
            this.cancelBtn.TabIndex = 24;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cancelBtn.TextColor = System.Drawing.Color.White;
            this.cancelBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click_1);
            // 
            // okBtn
            // 
            this.okBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.okBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.okBtn.BorderRadius = 10;
            this.okBtn.BorderSize = 1;
            this.okBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.okBtn.FlatAppearance.BorderSize = 0;
            this.okBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.okBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okBtn.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.okBtn.ForeColor = System.Drawing.Color.White;
            this.okBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.okBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.okBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.okBtn.Image = ((System.Drawing.Image)(resources.GetObject("okBtn.Image")));
            this.okBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.okBtn.Location = new System.Drawing.Point(115, 399);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(120, 40);
            this.okBtn.TabIndex = 23;
            this.okBtn.Text = "Ok";
            this.okBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.okBtn.TextColor = System.Drawing.Color.White;
            this.okBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.okBtn.UseVisualStyleBackColor = false;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click_1);
            // 
            // MultiTextPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(477, 446);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.PromptTextBox);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 412);
            this.Name = "MultiTextPrompt";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 49);
            this.Text = "Multi-Text Prompt";
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox PromptTextBox;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton cancelBtn;
        internal CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton okBtn;
    }
}