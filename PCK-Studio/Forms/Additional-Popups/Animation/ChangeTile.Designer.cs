namespace PckStudio.Forms.Additional_Popups.Animation
{
	partial class ChangeTile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeTile));
            this.treeViewBlocks = new System.Windows.Forms.TreeView();
            this.treeViewItems = new System.Windows.Forms.TreeView();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.Blocks = new System.Windows.Forms.TabPage();
            this.Items = new System.Windows.Forms.TabPage();
            this.acceptBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.CancelBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.metroTabControl1.SuspendLayout();
            this.Blocks.SuspendLayout();
            this.Items.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewBlocks
            // 
            this.treeViewBlocks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeViewBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBlocks.ForeColor = System.Drawing.Color.White;
            this.treeViewBlocks.Location = new System.Drawing.Point(0, 0);
            this.treeViewBlocks.Name = "treeViewBlocks";
            this.treeViewBlocks.Size = new System.Drawing.Size(318, 142);
            this.treeViewBlocks.TabIndex = 14;
            this.treeViewBlocks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
            // 
            // treeViewItems
            // 
            this.treeViewItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeViewItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewItems.ForeColor = System.Drawing.Color.White;
            this.treeViewItems.Location = new System.Drawing.Point(0, 0);
            this.treeViewItems.Name = "treeViewItems";
            this.treeViewItems.Size = new System.Drawing.Size(318, 142);
            this.treeViewItems.TabIndex = 14;
            this.treeViewItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(133, 19);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(46, 19);
            this.metroLabel2.TabIndex = 16;
            this.metroLabel2.Text = "Filter: ";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroTextBox1
            // 
            this.metroTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(134, 1);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.Lines = new string[0];
            this.metroTextBox1.Location = new System.Drawing.Point(173, 18);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.ShortcutsEnabled = true;
            this.metroTextBox1.Size = new System.Drawing.Size(156, 23);
            this.metroTextBox1.TabIndex = 17;
            this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTextBox1.UseCustomBackColor = true;
            this.metroTextBox1.UseCustomForeColor = true;
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.metroTextBox1.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.Blocks);
            this.metroTabControl1.Controls.Add(this.Items);
            this.metroTabControl1.Location = new System.Drawing.Point(6, 8);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(326, 184);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.White;
            this.metroTabControl1.TabIndex = 18;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseSelectable = true;
            // 
            // Blocks
            // 
            this.Blocks.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Blocks.Controls.Add(this.treeViewBlocks);
            this.Blocks.Location = new System.Drawing.Point(4, 38);
            this.Blocks.Name = "Blocks";
            this.Blocks.Size = new System.Drawing.Size(318, 142);
            this.Blocks.TabIndex = 0;
            this.Blocks.Text = "Blocks";
            // 
            // Items
            // 
            this.Items.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Items.Controls.Add(this.treeViewItems);
            this.Items.Location = new System.Drawing.Point(4, 38);
            this.Items.Name = "Items";
            this.Items.Size = new System.Drawing.Size(318, 142);
            this.Items.TabIndex = 0;
            this.Items.Text = "Items";
            // 
            // SaveButton
            // 
            this.acceptBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.acceptBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.acceptBtn.BorderRadius = 10;
            this.acceptBtn.BorderSize = 1;
            this.acceptBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.acceptBtn.FlatAppearance.BorderSize = 0;
            this.acceptBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.acceptBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.acceptBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.acceptBtn.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.acceptBtn.ForeColor = System.Drawing.Color.White;
            this.acceptBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.acceptBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.acceptBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.acceptBtn.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.acceptBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.acceptBtn.Location = new System.Drawing.Point(46, 198);
            this.acceptBtn.Name = "SaveButton";
            this.acceptBtn.Size = new System.Drawing.Size(120, 40);
            this.acceptBtn.TabIndex = 19;
            this.acceptBtn.Text = "Save";
            this.acceptBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.acceptBtn.TextColor = System.Drawing.Color.White;
            this.acceptBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.acceptBtn.UseVisualStyleBackColor = false;
            this.acceptBtn.Click += new System.EventHandler(this.AcceptBtn_Click);
            // 
            // CancelButton
            // 
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
            this.CancelBtn.Image = ((System.Drawing.Image)(resources.GetObject("CancelButton.Image")));
            this.CancelBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelBtn.Location = new System.Drawing.Point(172, 198);
            this.CancelBtn.Name = "CancelButton";
            this.CancelBtn.Size = new System.Drawing.Size(120, 40);
            this.CancelBtn.TabIndex = 20;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelBtn.TextColor = System.Drawing.Color.White;
            this.CancelBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // ChangeTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(338, 246);
            this.ControlBox = false;
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroTabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeTile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.metroTabControl1.ResumeLayout(false);
            this.Blocks.ResumeLayout(false);
            this.Items.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void MetroTextBox1_TextChanged(object sender, System.EventArgs e)
		{
			throw new System.NotImplementedException();
		}

		#endregion
		private System.Windows.Forms.TreeView treeViewBlocks;
		private System.Windows.Forms.TreeView treeViewItems;
		private MetroFramework.Controls.MetroLabel metroLabel2;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
		private MetroFramework.Controls.MetroTabControl metroTabControl1;
		private System.Windows.Forms.TabPage Blocks;
		private System.Windows.Forms.TabPage Items;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton acceptBtn;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton CancelBtn;
    }
}