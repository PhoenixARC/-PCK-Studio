namespace PckStudio.Forms
{
    partial class Pref
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pref));
            MetroFramework.Controls.MetroLabel metroLabel1;
            MetroFramework.Controls.MetroLabel metroLabel2;
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.webServerTextBox = new MetroFramework.Controls.MetroTextBox();
            this.pckWebServerTextBox = new MetroFramework.Controls.MetroTextBox();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.ForeColor = System.Drawing.Color.White;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = false;
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.Purple;
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.ForeColor = System.Drawing.Color.White;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // webServerTextBox
            // 
            // 
            // 
            // 
            this.webServerTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.webServerTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.webServerTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.webServerTextBox.CustomButton.Name = "";
            this.webServerTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.webServerTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.webServerTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.webServerTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.webServerTextBox.CustomButton.UseSelectable = true;
            this.webServerTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.webServerTextBox.Lines = new string[0];
            resources.ApplyResources(this.webServerTextBox, "webServerTextBox");
            this.webServerTextBox.MaxLength = 32767;
            this.webServerTextBox.Name = "webServerTextBox";
            this.webServerTextBox.PasswordChar = '\0';
            this.webServerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.webServerTextBox.SelectedText = "";
            this.webServerTextBox.SelectionLength = 0;
            this.webServerTextBox.SelectionStart = 0;
            this.webServerTextBox.ShortcutsEnabled = true;
            this.webServerTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.webServerTextBox.UseSelectable = true;
            this.webServerTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.webServerTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            resources.ApplyResources(metroLabel1, "metroLabel1");
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            resources.ApplyResources(metroLabel2, "metroLabel2");
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // pckWebServerTextBox
            // 
            // 
            // 
            // 
            this.pckWebServerTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.pckWebServerTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode1")));
            this.pckWebServerTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
            this.pckWebServerTextBox.CustomButton.Name = "";
            this.pckWebServerTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
            this.pckWebServerTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.pckWebServerTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
            this.pckWebServerTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.pckWebServerTextBox.CustomButton.UseSelectable = true;
            this.pckWebServerTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
            this.pckWebServerTextBox.Lines = new string[0];
            resources.ApplyResources(this.pckWebServerTextBox, "pckWebServerTextBox");
            this.pckWebServerTextBox.MaxLength = 32767;
            this.pckWebServerTextBox.Name = "pckWebServerTextBox";
            this.pckWebServerTextBox.PasswordChar = '\0';
            this.pckWebServerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.pckWebServerTextBox.SelectedText = "";
            this.pckWebServerTextBox.SelectionLength = 0;
            this.pckWebServerTextBox.SelectionStart = 0;
            this.pckWebServerTextBox.ShortcutsEnabled = true;
            this.pckWebServerTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.pckWebServerTextBox.UseSelectable = true;
            this.pckWebServerTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.pckWebServerTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // Pref
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(metroLabel2);
            this.Controls.Add(this.pckWebServerTextBox);
            this.Controls.Add(metroLabel1);
            this.Controls.Add(this.webServerTextBox);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonSave);
            this.Name = "Pref";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonSave;
        private MetroFramework.Controls.MetroTextBox webServerTextBox;
        private MetroFramework.Controls.MetroTextBox pckWebServerTextBox;
    }
}