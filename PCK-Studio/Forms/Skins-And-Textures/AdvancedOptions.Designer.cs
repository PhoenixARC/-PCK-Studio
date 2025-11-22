namespace PckStudio.Popups
{
    partial class AdvancedOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedOptions));
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.propertyTreeview = new System.Windows.Forms.TreeView();
            this.fileTypeComboBox = new MetroFramework.Controls.MetroComboBox();
            this.applyButton = new MetroFramework.Controls.MetroButton();
            this.propertyValueTextBox = new MetroFramework.Controls.MetroTextBox();
            this.propertyKeyTextBox = new MetroFramework.Controls.MetroTextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // propertyTreeview
            // 
            this.propertyTreeview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.propertyTreeview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.propertyTreeview.ForeColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.propertyTreeview, "propertyTreeview");
            this.propertyTreeview.Name = "propertyTreeview";
            this.propertyTreeview.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMeta_AfterSelect);
            // 
            // fileTypeComboBox
            // 
            this.fileTypeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.fileTypeComboBox, "fileTypeComboBox");
            this.fileTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("fileTypeComboBox.Items"),
            resources.GetString("fileTypeComboBox.Items1"),
            resources.GetString("fileTypeComboBox.Items2"),
            resources.GetString("fileTypeComboBox.Items3"),
            resources.GetString("fileTypeComboBox.Items4"),
            resources.GetString("fileTypeComboBox.Items5"),
            resources.GetString("fileTypeComboBox.Items6"),
            resources.GetString("fileTypeComboBox.Items7"),
            resources.GetString("fileTypeComboBox.Items8"),
            resources.GetString("fileTypeComboBox.Items9"),
            resources.GetString("fileTypeComboBox.Items10"),
            resources.GetString("fileTypeComboBox.Items11"),
            resources.GetString("fileTypeComboBox.Items12"),
            resources.GetString("fileTypeComboBox.Items13")});
            this.fileTypeComboBox.Name = "fileTypeComboBox";
            this.fileTypeComboBox.Style = MetroFramework.MetroColorStyle.Silver;
            this.fileTypeComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.fileTypeComboBox.UseSelectable = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = System.Drawing.Color.White;
            label1.Name = "label1";
            // 
            // applyButton
            // 
            this.applyButton.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.Style = MetroFramework.MetroColorStyle.Silver;
            this.applyButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.applyButton.UseSelectable = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = System.Drawing.Color.White;
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = System.Drawing.Color.White;
            label3.Name = "label3";
            // 
            // propertyValueTextBox
            // 
            // 
            // 
            // 
            this.propertyValueTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.propertyValueTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.propertyValueTextBox.CustomButton.Name = "";
            this.propertyValueTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.propertyValueTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.propertyValueTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.propertyValueTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.propertyValueTextBox.CustomButton.UseSelectable = true;
            this.propertyValueTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.propertyValueTextBox.Lines = new string[0];
            resources.ApplyResources(this.propertyValueTextBox, "propertyValueTextBox");
            this.propertyValueTextBox.MaxLength = 32767;
            this.propertyValueTextBox.Name = "propertyValueTextBox";
            this.propertyValueTextBox.PasswordChar = '\0';
            this.propertyValueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.propertyValueTextBox.SelectedText = "";
            this.propertyValueTextBox.SelectionLength = 0;
            this.propertyValueTextBox.SelectionStart = 0;
            this.propertyValueTextBox.ShortcutsEnabled = true;
            this.propertyValueTextBox.Style = MetroFramework.MetroColorStyle.Silver;
            this.propertyValueTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.propertyValueTextBox.UseSelectable = true;
            this.propertyValueTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.propertyValueTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // propertyKeyTextBox
            // 
            // 
            // 
            // 
            this.propertyKeyTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.propertyKeyTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.propertyKeyTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
            this.propertyKeyTextBox.CustomButton.Name = "";
            this.propertyKeyTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
            this.propertyKeyTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.propertyKeyTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
            this.propertyKeyTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.propertyKeyTextBox.CustomButton.UseSelectable = true;
            this.propertyKeyTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
            this.propertyKeyTextBox.Lines = new string[0];
            resources.ApplyResources(this.propertyKeyTextBox, "propertyKeyTextBox");
            this.propertyKeyTextBox.MaxLength = 32767;
            this.propertyKeyTextBox.Name = "propertyKeyTextBox";
            this.propertyKeyTextBox.PasswordChar = '\0';
            this.propertyKeyTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.propertyKeyTextBox.SelectedText = "";
            this.propertyKeyTextBox.SelectionLength = 0;
            this.propertyKeyTextBox.SelectionStart = 0;
            this.propertyKeyTextBox.ShortcutsEnabled = true;
            this.propertyKeyTextBox.Style = MetroFramework.MetroColorStyle.Silver;
            this.propertyKeyTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.propertyKeyTextBox.UseSelectable = true;
            this.propertyKeyTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.propertyKeyTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // AdvancedOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyKeyTextBox);
            this.Controls.Add(label2);
            this.Controls.Add(label3);
            this.Controls.Add(this.propertyValueTextBox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(label1);
            this.Controls.Add(this.fileTypeComboBox);
            this.Controls.Add(this.propertyTreeview);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedOptions";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView propertyTreeview;
        private MetroFramework.Controls.MetroComboBox fileTypeComboBox;
        private MetroFramework.Controls.MetroButton applyButton;
        private MetroFramework.Controls.MetroTextBox propertyValueTextBox;
        private MetroFramework.Controls.MetroTextBox propertyKeyTextBox;
    }
}