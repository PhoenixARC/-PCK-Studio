namespace PckStudio.Forms.Additional_Popups
{
    partial class ChangeLogForm
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
            this.ChangelogRichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ChangelogRichTextBox
            // 
            this.ChangelogRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ChangelogRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChangelogRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChangelogRichTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ChangelogRichTextBox.ForeColor = System.Drawing.Color.White;
            this.ChangelogRichTextBox.Location = new System.Drawing.Point(10, 10);
            this.ChangelogRichTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 30, 30);
            this.ChangelogRichTextBox.Name = "ChangelogRichTextBox";
            this.ChangelogRichTextBox.ReadOnly = true;
            this.ChangelogRichTextBox.Size = new System.Drawing.Size(424, 441);
            this.ChangelogRichTextBox.TabIndex = 23;
            this.ChangelogRichTextBox.Text = "";
            // 
            // ChangeLogForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(444, 461);
            this.Controls.Add(this.ChangelogRichTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "ChangeLogForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "What\'s new in PCK Studio?";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ChangelogRichTextBox;
    }
}
