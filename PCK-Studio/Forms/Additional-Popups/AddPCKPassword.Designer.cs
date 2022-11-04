
namespace PckStudio.Forms
{
    partial class AddPCKPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPCKPassword));
            this.buttonUnlocked = new MetroFramework.Controls.MetroButton();
            this.textBoxPass = new MetroFramework.Controls.MetroTextBox();
            this.SuspendLayout();
            // 
            // buttonUnlocked
            // 
            this.buttonUnlocked.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonUnlocked.ForeColor = System.Drawing.Color.White;
            this.buttonUnlocked.Location = new System.Drawing.Point(266, 53);
            this.buttonUnlocked.Name = "buttonUnlocked";
            this.buttonUnlocked.Size = new System.Drawing.Size(75, 23);
            this.buttonUnlocked.Style = MetroFramework.MetroColorStyle.Black;
            this.buttonUnlocked.TabIndex = 3;
            this.buttonUnlocked.Text = "Lock!";
            this.buttonUnlocked.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.buttonUnlocked.UseSelectable = true;
            this.buttonUnlocked.Click += new System.EventHandler(this.buttonUnlocked_Click);
            // 
            // textBoxPass
            // 
            // 
            // 
            // 
            this.textBoxPass.CustomButton.Image = null;
            this.textBoxPass.CustomButton.Location = new System.Drawing.Point(226, 2);
            this.textBoxPass.CustomButton.Name = "";
            this.textBoxPass.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxPass.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxPass.CustomButton.TabIndex = 1;
            this.textBoxPass.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxPass.CustomButton.UseSelectable = true;
            this.textBoxPass.CustomButton.Visible = false;
            this.textBoxPass.Lines = new string[0];
            this.textBoxPass.Location = new System.Drawing.Point(16, 55);
            this.textBoxPass.MaxLength = 32767;
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.PasswordChar = '●';
            this.textBoxPass.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxPass.SelectedText = "";
            this.textBoxPass.SelectionLength = 0;
            this.textBoxPass.SelectionStart = 0;
            this.textBoxPass.ShortcutsEnabled = true;
            this.textBoxPass.Size = new System.Drawing.Size(244, 20);
            this.textBoxPass.Style = MetroFramework.MetroColorStyle.Silver;
            this.textBoxPass.TabIndex = 2;
            this.textBoxPass.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxPass.UseSelectable = true;
            this.textBoxPass.UseSystemPasswordChar = true;
            this.textBoxPass.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxPass.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // AddPCKPassword
            // 
            this.AcceptButton = this.buttonUnlocked;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 116);
            this.Controls.Add(this.buttonUnlocked);
            this.Controls.Add(this.textBoxPass);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddPCKPassword";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Add Password";
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton buttonUnlocked;
        private MetroFramework.Controls.MetroTextBox textBoxPass;
    }
}