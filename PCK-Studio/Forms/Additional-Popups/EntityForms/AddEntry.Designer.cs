namespace PckStudio.Forms.Additional_Popups.EntityForms
{
	partial class AddEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEntry));
            this.treeViewEntity = new System.Windows.Forms.TreeView();
            this.FilterLabel = new MetroFramework.Controls.MetroLabel();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.Blocks = new System.Windows.Forms.TabPage();
            this.CancelBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.AddBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.metroTabControl1.SuspendLayout();
            this.Blocks.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewEntity
            // 
            this.treeViewEntity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeViewEntity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewEntity.ForeColor = System.Drawing.Color.White;
            this.treeViewEntity.Location = new System.Drawing.Point(0, 0);
            this.treeViewEntity.Margin = new System.Windows.Forms.Padding(4);
            this.treeViewEntity.Name = "treeViewEntity";
            this.treeViewEntity.Size = new System.Drawing.Size(427, 184);
            this.treeViewEntity.TabIndex = 14;
            this.treeViewEntity.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
            // 
            // FilterLabel
            // 
            this.FilterLabel.AutoSize = true;
            this.FilterLabel.Location = new System.Drawing.Point(99, 243);
            this.FilterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(46, 19);
            this.FilterLabel.TabIndex = 16;
            this.FilterLabel.Text = "Filter: ";
            this.FilterLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FilterLabel.UseCustomBackColor = true;
            this.FilterLabel.UseCustomForeColor = true;
            // 
            // metroTextBox1
            // 
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(182, 2);
            this.metroTextBox1.CustomButton.Margin = new System.Windows.Forms.Padding(4);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(23, 23);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.Lines = new string[0];
            this.metroTextBox1.Location = new System.Drawing.Point(143, 240);
            this.metroTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.ShortcutsEnabled = true;
            this.metroTextBox1.Size = new System.Drawing.Size(208, 28);
            this.metroTextBox1.TabIndex = 17;
            this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.metroTextBox1.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.Blocks);
            this.metroTabControl1.Location = new System.Drawing.Point(8, 10);
            this.metroTabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(435, 226);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.White;
            this.metroTabControl1.TabIndex = 18;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseSelectable = true;
            // 
            // Blocks
            // 
            this.Blocks.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Blocks.Controls.Add(this.treeViewEntity);
            this.Blocks.Location = new System.Drawing.Point(4, 38);
            this.Blocks.Margin = new System.Windows.Forms.Padding(4);
            this.Blocks.Name = "Blocks";
            this.Blocks.Size = new System.Drawing.Size(427, 184);
            this.Blocks.TabIndex = 0;
            this.Blocks.Text = "Entities";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelBtn.BorderRadius = 10;
            this.CancelBtn.BorderSize = 1;
            this.CancelBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelBtn.FlatAppearance.BorderSize = 0;
            this.CancelBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelBtn.Image = ((System.Drawing.Image)(resources.GetObject("CancelBtn.Image")));
            this.CancelBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelBtn.Location = new System.Drawing.Point(228, 279);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(120, 40);
            this.CancelBtn.TabIndex = 22;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelBtn.TextColor = System.Drawing.Color.White;
            this.CancelBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // AddBtn
            // 
            this.AddBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AddBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.AddBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.AddBtn.BorderRadius = 10;
            this.AddBtn.BorderSize = 1;
            this.AddBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.AddBtn.FlatAppearance.BorderSize = 0;
            this.AddBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.AddBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.AddBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddBtn.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.AddBtn.ForeColor = System.Drawing.Color.White;
            this.AddBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.AddBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.AddBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.AddBtn.Image = global::PckStudio.Properties.Resources.plus___24px;
            this.AddBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AddBtn.Location = new System.Drawing.Point(102, 279);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(120, 40);
            this.AddBtn.TabIndex = 21;
            this.AddBtn.Text = "Add";
            this.AddBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AddBtn.TextColor = System.Drawing.Color.White;
            this.AddBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AddBtn.UseVisualStyleBackColor = false;
            this.AddBtn.Click += new System.EventHandler(this.AddNtb_Click);
            // 
            // AddEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(451, 331);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.FilterLabel);
            this.Controls.Add(this.metroTabControl1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEntry";
            this.Text = "Add Entry";
            this.metroTabControl1.ResumeLayout(false);
            this.Blocks.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void MetroTextBox1_TextChanged(object sender, System.EventArgs e)
		{
			throw new System.NotImplementedException();
		}

		#endregion
		private System.Windows.Forms.TreeView treeViewEntity;
		private MetroFramework.Controls.MetroLabel FilterLabel;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
		private MetroFramework.Controls.MetroTabControl metroTabControl1;
		private System.Windows.Forms.TabPage Blocks;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton CancelBtn;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton AddBtn;
    }
}