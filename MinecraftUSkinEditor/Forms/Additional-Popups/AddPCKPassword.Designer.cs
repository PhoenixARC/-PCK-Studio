
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
            this.buttonUnlocked = new System.Windows.Forms.Button();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonUnlocked
            // 
            this.buttonUnlocked.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonUnlocked.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUnlocked.ForeColor = System.Drawing.Color.White;
            this.buttonUnlocked.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonUnlocked.Location = new System.Drawing.Point(266, 53);
            this.buttonUnlocked.Name = "buttonUnlocked";
            this.buttonUnlocked.Size = new System.Drawing.Size(75, 23);
            this.buttonUnlocked.TabIndex = 3;
            this.buttonUnlocked.Text = "Lock!";
            this.buttonUnlocked.UseVisualStyleBackColor = true;
            this.buttonUnlocked.Click += new System.EventHandler(this.buttonUnlocked_Click);
            // 
            // textBoxPass
            // 
            this.textBoxPass.Location = new System.Drawing.Point(16, 55);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.Size = new System.Drawing.Size(244, 20);
            this.textBoxPass.TabIndex = 2;
            this.textBoxPass.Text = "Password";
            // 
            // AddPCKPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 129);
            this.Controls.Add(this.buttonUnlocked);
            this.Controls.Add(this.textBoxPass);
            this.Name = "AddPCKPassword";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Add Password";
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUnlocked;
        private System.Windows.Forms.TextBox textBoxPass;
    }
}