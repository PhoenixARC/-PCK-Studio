namespace PckStudio
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
			this.treeMeta = new System.Windows.Forms.TreeView();
			this.comboBox1 = new MetroFramework.Controls.MetroComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.applyButton = new MetroFramework.Controls.MetroButton();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.entryDataTextBox = new MetroFramework.Controls.MetroTextBox();
			this.entryTypeTextBox = new MetroFramework.Controls.MetroTextBox();
			this.SuspendLayout();
			// 
			// treeMeta
			// 
			resources.ApplyResources(this.treeMeta, "treeMeta");
			this.treeMeta.Name = "treeMeta";
			this.treeMeta.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMeta_AfterSelect);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4"),
            resources.GetString("comboBox1.Items5"),
            resources.GetString("comboBox1.Items6"),
            resources.GetString("comboBox1.Items7"),
            resources.GetString("comboBox1.Items8"),
            resources.GetString("comboBox1.Items9"),
            resources.GetString("comboBox1.Items10"),
            resources.GetString("comboBox1.Items11"),
            resources.GetString("comboBox1.Items12"),
            resources.GetString("comboBox1.Items13")});
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Style = MetroFramework.MetroColorStyle.Silver;
			this.comboBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.comboBox1.UseSelectable = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Name = "label1";
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
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Name = "label3";
			// 
			// entryDataTextBox
			// 
			// 
			// 
			// 
			this.entryDataTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
			this.entryDataTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
			this.entryDataTextBox.CustomButton.Name = "";
			this.entryDataTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
			this.entryDataTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
			this.entryDataTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
			this.entryDataTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
			this.entryDataTextBox.CustomButton.UseSelectable = true;
			this.entryDataTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
			this.entryDataTextBox.Lines = new string[0];
			resources.ApplyResources(this.entryDataTextBox, "entryDataTextBox");
			this.entryDataTextBox.MaxLength = 32767;
			this.entryDataTextBox.Name = "entryDataTextBox";
			this.entryDataTextBox.PasswordChar = '\0';
			this.entryDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.entryDataTextBox.SelectedText = "";
			this.entryDataTextBox.SelectionLength = 0;
			this.entryDataTextBox.SelectionStart = 0;
			this.entryDataTextBox.ShortcutsEnabled = true;
			this.entryDataTextBox.Style = MetroFramework.MetroColorStyle.Silver;
			this.entryDataTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.entryDataTextBox.UseSelectable = true;
			this.entryDataTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
			this.entryDataTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
			// 
			// entryTypeTextBox
			// 
			// 
			// 
			// 
			this.entryTypeTextBox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
			this.entryTypeTextBox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
			this.entryTypeTextBox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location1")));
			this.entryTypeTextBox.CustomButton.Name = "";
			this.entryTypeTextBox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size1")));
			this.entryTypeTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
			this.entryTypeTextBox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex1")));
			this.entryTypeTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
			this.entryTypeTextBox.CustomButton.UseSelectable = true;
			this.entryTypeTextBox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible1")));
			this.entryTypeTextBox.Lines = new string[0];
			resources.ApplyResources(this.entryTypeTextBox, "entryTypeTextBox");
			this.entryTypeTextBox.MaxLength = 32767;
			this.entryTypeTextBox.Name = "entryTypeTextBox";
			this.entryTypeTextBox.PasswordChar = '\0';
			this.entryTypeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.entryTypeTextBox.SelectedText = "";
			this.entryTypeTextBox.SelectionLength = 0;
			this.entryTypeTextBox.SelectionStart = 0;
			this.entryTypeTextBox.ShortcutsEnabled = true;
			this.entryTypeTextBox.Style = MetroFramework.MetroColorStyle.Silver;
			this.entryTypeTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.entryTypeTextBox.UseSelectable = true;
			this.entryTypeTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
			this.entryTypeTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
			// 
			// AdvancedOptions
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.entryTypeTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.entryDataTextBox);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.treeMeta);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AdvancedOptions";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeMeta;
        private MetroFramework.Controls.MetroComboBox comboBox1;
        private MetroFramework.Controls.MetroButton applyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private MetroFramework.Controls.MetroTextBox entryDataTextBox;
        private MetroFramework.Controls.MetroTextBox entryTypeTextBox;
    }
}