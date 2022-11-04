namespace PckStudio.Forms.Additional_Popups.Loc
{
    partial class AddLanguage
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
            this.LanguageComboBox = new MetroFramework.Controls.MetroComboBox();
            this.AddBtn = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // LanguageComboBox
            // 
            this.LanguageComboBox.FormattingEnabled = true;
            this.LanguageComboBox.ItemHeight = 23;
            this.LanguageComboBox.Location = new System.Drawing.Point(23, 63);
            this.LanguageComboBox.Name = "LanguageComboBox";
            this.LanguageComboBox.Size = new System.Drawing.Size(243, 29);
            this.LanguageComboBox.Style = MetroFramework.MetroColorStyle.Black;
            this.LanguageComboBox.TabIndex = 0;
            this.LanguageComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.LanguageComboBox.UseSelectable = true;
            // 
            // AddBtn
            // 
            this.AddBtn.Location = new System.Drawing.Point(91, 98);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(101, 23);
            this.AddBtn.TabIndex = 1;
            this.AddBtn.Text = "Add";
            this.AddBtn.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.AddBtn.UseSelectable = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // AddLanguage
            // 
            this.AcceptButton = this.AddBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 140);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.LanguageComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(289, 140);
            this.Name = "AddLanguage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Add Language";
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox LanguageComboBox;
        private MetroFramework.Controls.MetroButton AddBtn;
    }
}