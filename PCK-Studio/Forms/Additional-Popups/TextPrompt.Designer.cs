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
            this.PromptTextBox = new MetroFramework.Controls.MetroTextBox();
            this.okBtn = new MetroFramework.Controls.MetroButton();
            this.cancelBtn = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // PromptTextBox
            // 
            // 
            // 
            // 
            this.PromptTextBox.CustomButton.Image = null;
            this.PromptTextBox.CustomButton.Location = new System.Drawing.Point(34, 1);
            this.PromptTextBox.CustomButton.Name = "";
            this.PromptTextBox.CustomButton.Size = new System.Drawing.Size(283, 283);
            this.PromptTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.PromptTextBox.CustomButton.TabIndex = 1;
            this.PromptTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.PromptTextBox.CustomButton.UseSelectable = true;
            this.PromptTextBox.CustomButton.Visible = false;
            this.PromptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PromptTextBox.Lines = new string[0];
            this.PromptTextBox.Location = new System.Drawing.Point(20, 60);
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
            this.PromptTextBox.Size = new System.Drawing.Size(318, 285);
            this.PromptTextBox.Style = MetroFramework.MetroColorStyle.Black;
            this.PromptTextBox.TabIndex = 0;
            this.PromptTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PromptTextBox.UseSelectable = true;
            this.PromptTextBox.WaterMark = "Paste Multi-line text here...";
            this.PromptTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.PromptTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.Location = new System.Drawing.Point(264, 351);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(74, 23);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "OK";
            this.okBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.okBtn.UseSelectable = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(183, 351);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cancelBtn.UseSelectable = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // TextPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(358, 385);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
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
        private MetroFramework.Controls.MetroButton okBtn;
        private MetroFramework.Controls.MetroButton cancelBtn;
    }
}