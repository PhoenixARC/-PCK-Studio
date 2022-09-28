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
			this.acceptBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.treeViewBlocks = new System.Windows.Forms.TreeView();
			this.treeViewItems = new System.Windows.Forms.TreeView();
			this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
			this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
			this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
			this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
			this.Blocks = new System.Windows.Forms.TabPage();
			this.Items = new System.Windows.Forms.TabPage();
			this.metroTabControl1.SuspendLayout();
			this.Blocks.SuspendLayout();
			this.Items.SuspendLayout();
			this.SuspendLayout();
			// 
			// acceptBtn
			// 
			this.acceptBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.acceptBtn.ForeColor = System.Drawing.Color.White;
			this.acceptBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.acceptBtn.Location = new System.Drawing.Point(55, 233);
			this.acceptBtn.Name = "acceptBtn";
			this.acceptBtn.Size = new System.Drawing.Size(75, 23);
			this.acceptBtn.TabIndex = 7;
			this.acceptBtn.Text = "Save";
			this.acceptBtn.UseVisualStyleBackColor = true;
			this.acceptBtn.Click += new System.EventHandler(this.AcceptBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CancelBtn.ForeColor = System.Drawing.Color.White;
			this.CancelBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.CancelBtn.Location = new System.Drawing.Point(135, 233);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 13;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// treeViewBlocks
			// 
			this.treeViewBlocks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.treeViewBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewBlocks.ForeColor = System.Drawing.Color.White;
			this.treeViewBlocks.Location = new System.Drawing.Point(0, 0);
			this.treeViewBlocks.Name = "treeViewBlocks";
			this.treeViewBlocks.Size = new System.Drawing.Size(184, 125);
			this.treeViewBlocks.TabIndex = 14;
			this.treeViewBlocks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
			// 
			// treeViewItems
			// 
			this.treeViewItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.treeViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewItems.ForeColor = System.Drawing.Color.White;
			this.treeViewItems.Location = new System.Drawing.Point(0, 0);
			this.treeViewItems.Name = "treeViewItems";
			this.treeViewItems.Size = new System.Drawing.Size(184, 125);
			this.treeViewItems.TabIndex = 14;
			this.treeViewItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViews_AfterSelect);
			// 
			// metroLabel1
			// 
			this.metroLabel1.AutoSize = true;
			this.metroLabel1.Location = new System.Drawing.Point(75, 13);
			this.metroLabel1.Name = "metroLabel1";
			this.metroLabel1.Size = new System.Drawing.Size(114, 19);
			this.metroLabel1.TabIndex = 15;
			this.metroLabel1.Text = "Please select a tile";
			this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroLabel2
			// 
			this.metroLabel2.AutoSize = true;
			this.metroLabel2.Location = new System.Drawing.Point(36, 35);
			this.metroLabel2.Name = "metroLabel2";
			this.metroLabel2.Size = new System.Drawing.Size(46, 19);
			this.metroLabel2.TabIndex = 16;
			this.metroLabel2.Text = "Filter: ";
			this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// metroTextBox1
			// 
			// 
			// 
			// 
			this.metroTextBox1.CustomButton.Image = null;
			this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(113, 1);
			this.metroTextBox1.CustomButton.Name = "";
			this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(21, 21);
			this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
			this.metroTextBox1.CustomButton.TabIndex = 1;
			this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
			this.metroTextBox1.CustomButton.UseSelectable = true;
			this.metroTextBox1.CustomButton.Visible = false;
			this.metroTextBox1.Lines = new string[0];
			this.metroTextBox1.Location = new System.Drawing.Point(75, 35);
			this.metroTextBox1.MaxLength = 32767;
			this.metroTextBox1.Name = "metroTextBox1";
			this.metroTextBox1.PasswordChar = '\0';
			this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.metroTextBox1.SelectedText = "";
			this.metroTextBox1.SelectionLength = 0;
			this.metroTextBox1.SelectionStart = 0;
			this.metroTextBox1.ShortcutsEnabled = true;
			this.metroTextBox1.Size = new System.Drawing.Size(135, 23);
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
			this.metroTabControl1.Controls.Add(this.Items);
			this.metroTabControl1.Location = new System.Drawing.Point(36, 60);
			this.metroTabControl1.Name = "metroTabControl1";
			this.metroTabControl1.SelectedIndex = 0;
			this.metroTabControl1.Size = new System.Drawing.Size(192, 167);
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
			this.Blocks.Size = new System.Drawing.Size(184, 125);
			this.Blocks.TabIndex = 0;
			this.Blocks.Text = "Blocks";
			// 
			// Items
			// 
			this.Items.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.Items.Controls.Add(this.treeViewItems);
			this.Items.Location = new System.Drawing.Point(4, 38);
			this.Items.Name = "Items";
			this.Items.Size = new System.Drawing.Size(184, 125);
			this.Items.TabIndex = 0;
			this.Items.Text = "Items";
			// 
			// ChangeTile
			// 
			this.AcceptButton = this.acceptBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(264, 264);
			this.ControlBox = false;
			this.Controls.Add(this.metroTabControl1);
			this.Controls.Add(this.metroTextBox1);
			this.Controls.Add(this.metroLabel2);
			this.Controls.Add(this.metroLabel1);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.acceptBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangeTile";
			this.Resizable = false;
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
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
		private System.Windows.Forms.Button acceptBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.TreeView treeViewBlocks;
		private System.Windows.Forms.TreeView treeViewItems;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private MetroFramework.Controls.MetroLabel metroLabel2;
		private MetroFramework.Controls.MetroTextBox metroTextBox1;
		private MetroFramework.Controls.MetroTabControl metroTabControl1;
		private System.Windows.Forms.TabPage Blocks;
		private System.Windows.Forms.TabPage Items;
	}
}