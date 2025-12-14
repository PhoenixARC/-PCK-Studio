namespace PckStudio.Forms.Additional_Popups.Animation
{
	partial class FilterPrompt
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
            MetroFramework.Controls.MetroLabel metroLabel2;
            this.filterTextBox = new MetroFramework.Controls.MetroTextBox();
            this.tabController = new MetroFramework.Controls.MetroTabControl();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Location = new System.Drawing.Point(2, 6);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(46, 19);
            metroLabel2.TabIndex = 16;
            metroLabel2.Text = "Filter: ";
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // filterTextBox
            // 
            // 
            // 
            // 
            this.filterTextBox.CustomButton.Image = null;
            this.filterTextBox.CustomButton.Location = new System.Drawing.Point(134, 1);
            this.filterTextBox.CustomButton.Name = "";
            this.filterTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.filterTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.filterTextBox.CustomButton.TabIndex = 1;
            this.filterTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.filterTextBox.CustomButton.UseSelectable = true;
            this.filterTextBox.CustomButton.Visible = false;
            this.filterTextBox.Lines = new string[0];
            this.filterTextBox.Location = new System.Drawing.Point(42, 5);
            this.filterTextBox.MaxLength = 32767;
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.PasswordChar = '\0';
            this.filterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.filterTextBox.SelectedText = "";
            this.filterTextBox.SelectionLength = 0;
            this.filterTextBox.SelectionStart = 0;
            this.filterTextBox.ShortcutsEnabled = true;
            this.filterTextBox.Size = new System.Drawing.Size(156, 23);
            this.filterTextBox.TabIndex = 0;
            this.filterTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.filterTextBox.UseSelectable = true;
            this.filterTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.filterTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // tabController
            // 
            this.tabController.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabController.Location = new System.Drawing.Point(0, 34);
            this.tabController.Name = "tabController";
            this.tabController.Size = new System.Drawing.Size(360, 408);
            this.tabController.Style = MetroFramework.MetroColorStyle.Silver;
            this.tabController.TabIndex = 18;
            this.tabController.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tabController.UseSelectable = true;
            // 
            // FilterPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(metroLabel2);
            this.Controls.Add(this.tabController);
            this.Name = "FilterPrompt";
            this.Size = new System.Drawing.Size(360, 442);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

#endregion

		private MetroFramework.Controls.MetroTextBox filterTextBox;
        private MetroFramework.Controls.MetroTabControl tabController;
    }
}