
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
            this.textBoxPass = new MetroFramework.Controls.MetroTextBox();
            this.LockPCKButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SuspendLayout();
            // 
            // textBoxPass
            // 
            this.textBoxPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
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
            this.textBoxPass.Location = new System.Drawing.Point(55, 12);
            this.textBoxPass.MaxLength = 32767;
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.PasswordChar = '●';
            this.textBoxPass.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxPass.SelectedText = "";
            this.textBoxPass.SelectionLength = 0;
            this.textBoxPass.SelectionStart = 0;
            this.textBoxPass.ShortcutsEnabled = true;
            this.textBoxPass.Size = new System.Drawing.Size(245, 20);
            this.textBoxPass.Style = MetroFramework.MetroColorStyle.Silver;
            this.textBoxPass.TabIndex = 2;
            this.textBoxPass.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxPass.UseSelectable = true;
            this.textBoxPass.UseSystemPasswordChar = true;
            this.textBoxPass.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxPass.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // LockPCKButton
            // 
            this.LockPCKButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.LockPCKButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.LockPCKButton.BorderRadius = 10;
            this.LockPCKButton.BorderSize = 1;
            this.LockPCKButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.LockPCKButton.FlatAppearance.BorderSize = 0;
            this.LockPCKButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.LockPCKButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.LockPCKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LockPCKButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LockPCKButton.ForeColor = System.Drawing.Color.White;
            this.LockPCKButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.LockPCKButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.LockPCKButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.LockPCKButton.Image = ((System.Drawing.Image)(resources.GetObject("LockPCKButton.Image")));
            this.LockPCKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LockPCKButton.Location = new System.Drawing.Point(117, 38);
            this.LockPCKButton.Name = "LockPCKButton";
            this.LockPCKButton.Size = new System.Drawing.Size(120, 40);
            this.LockPCKButton.TabIndex = 16;
            this.LockPCKButton.Text = "Lock!";
            this.LockPCKButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LockPCKButton.TextColor = System.Drawing.Color.White;
            this.LockPCKButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.LockPCKButton.UseVisualStyleBackColor = false;
            this.LockPCKButton.Click += new System.EventHandler(this.LockPCKButton_Click);
            // 
            // AddPCKPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(354, 91);
            this.Controls.Add(this.LockPCKButton);
            this.Controls.Add(this.textBoxPass);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddPCKPassword";
            this.Text = "Add Password";
            this.Load += new System.EventHandler(this.AddPCKPassword_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroTextBox textBoxPass;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton LockPCKButton;
    }
}