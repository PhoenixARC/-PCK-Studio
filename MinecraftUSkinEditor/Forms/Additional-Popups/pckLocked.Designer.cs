namespace PckStudio.Forms
{
    partial class pckLocked
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pckLocked));
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.buttonUnlocked = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPass
            // 
            this.textBoxPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.textBoxPass.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPass.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.textBoxPass, "textBoxPass");
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.Click += new System.EventHandler(this.textBoxPass_Click);
            this.textBoxPass.TextChanged += new System.EventHandler(this.textBoxPass_TextChanged);
            this.textBoxPass.Enter += new System.EventHandler(this.textBoxPass_Enter);
            // 
            // buttonUnlocked
            // 
            this.buttonUnlocked.BackColor = System.Drawing.Color.DodgerBlue;
            this.buttonUnlocked.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonUnlocked, "buttonUnlocked");
            this.buttonUnlocked.ForeColor = System.Drawing.Color.White;
            this.buttonUnlocked.Name = "buttonUnlocked";
            this.buttonUnlocked.UseVisualStyleBackColor = false;
            this.buttonUnlocked.Click += new System.EventHandler(this.buttonUnlocked_Click);
            // 
            // pckLocked
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.Controls.Add(this.buttonUnlocked);
            this.Controls.Add(this.textBoxPass);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "pckLocked";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.Button buttonUnlocked;
    }
}