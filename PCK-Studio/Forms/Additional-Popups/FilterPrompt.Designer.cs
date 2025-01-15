﻿namespace PckStudio.Forms.Additional_Popups.Animation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterPrompt));
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.filterTextBox = new MetroFramework.Controls.MetroTextBox();
            this.tabController = new MetroFramework.Controls.MetroTabControl();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Location = new System.Drawing.Point(155, 22);
            metroLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(46, 19);
            metroLabel2.TabIndex = 16;
            metroLabel2.Text = "Filter: ";
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // acceptButton
            // 
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.acceptButton.ForeColor = System.Drawing.Color.White;
            this.acceptButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.acceptButton.Location = new System.Drawing.Point(108, 226);
            this.acceptButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(88, 26);
            this.acceptButton.TabIndex = 7;
            this.acceptButton.Text = "Save";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.AcceptBtn_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelButton.Location = new System.Drawing.Point(200, 226);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 26);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // filterTextBox
            // 
            // 
            // 
            // 
            this.filterTextBox.CustomButton.Image = null;
            this.filterTextBox.CustomButton.Location = new System.Drawing.Point(101, 1);
            this.filterTextBox.CustomButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.filterTextBox.CustomButton.Name = "";
            this.filterTextBox.CustomButton.Size = new System.Drawing.Size(13, 13);
            this.filterTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.filterTextBox.CustomButton.TabIndex = 1;
            this.filterTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.filterTextBox.CustomButton.UseSelectable = true;
            this.filterTextBox.CustomButton.Visible = false;
            this.filterTextBox.Lines = new string[0];
            this.filterTextBox.Location = new System.Drawing.Point(202, 20);
            this.filterTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.filterTextBox.MaxLength = 32767;
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.PasswordChar = '\0';
            this.filterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.filterTextBox.SelectedText = "";
            this.filterTextBox.SelectionLength = 0;
            this.filterTextBox.SelectionStart = 0;
            this.filterTextBox.ShortcutsEnabled = true;
            this.filterTextBox.Size = new System.Drawing.Size(182, 26);
            this.filterTextBox.TabIndex = 17;
            this.filterTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.filterTextBox.UseSelectable = true;
            this.filterTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.filterTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // tabController
            // 
            this.tabController.Location = new System.Drawing.Point(7, 10);
            this.tabController.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabController.Name = "tabController";
            this.tabController.Size = new System.Drawing.Size(381, 212);
            this.tabController.Style = MetroFramework.MetroColorStyle.White;
            this.tabController.TabIndex = 18;
            this.tabController.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.tabController.UseSelectable = true;
            // 
            // FilterPrompt
            // 
            this.AcceptButton = this.acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(395, 263);
            this.Controls.Add(this.filterTextBox);
            this.Controls.Add(metroLabel2);
            this.Controls.Add(this.tabController);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FilterPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void MetroTextBox1_TextChanged(object sender, System.EventArgs e)
		{
			throw new System.NotImplementedException();
		}

		#endregion
		private System.Windows.Forms.Button acceptButton;
		private System.Windows.Forms.Button cancelButton;
		private MetroFramework.Controls.MetroTextBox filterTextBox;
        private MetroFramework.Controls.MetroTabControl tabController;
    }
}