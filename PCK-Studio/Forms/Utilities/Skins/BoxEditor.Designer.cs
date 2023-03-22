namespace PckStudio.Forms.Utilities.Skins
{
	partial class BoxEditor
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
			this.closeButton = new MetroFramework.Controls.MetroButton();
			this.toolTip = new MetroFramework.Components.MetroToolTip();
			this.parentLabel = new MetroFramework.Controls.MetroLabel();
			this.parentComboBox = new MetroFramework.Controls.MetroComboBox();
			this.positionLabel = new MetroFramework.Controls.MetroLabel();
			this.PosXUpDown = new System.Windows.Forms.NumericUpDown();
			this.PosYUpDown = new System.Windows.Forms.NumericUpDown();
			this.PosZUpDown = new System.Windows.Forms.NumericUpDown();
			this.SizeZUpDown = new System.Windows.Forms.NumericUpDown();
			this.SizeYUpDown = new System.Windows.Forms.NumericUpDown();
			this.SizeXUpDown = new System.Windows.Forms.NumericUpDown();
			this.sizeLabel = new MetroFramework.Controls.MetroLabel();
			this.uvYUpDown = new System.Windows.Forms.NumericUpDown();
			this.uvXUpDown = new System.Windows.Forms.NumericUpDown();
			this.uvLabel = new MetroFramework.Controls.MetroLabel();
			this.armorCheckBox = new MetroFramework.Controls.MetroCheckBox();
			this.mirrorCheckBox = new MetroFramework.Controls.MetroCheckBox();
			this.inflationUpDown = new System.Windows.Forms.NumericUpDown();
			this.inflationLabel = new MetroFramework.Controls.MetroLabel();
			((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.uvYUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.uvXUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.inflationUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(252, 187);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(126, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Save";
			this.closeButton.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.closeButton.UseSelectable = true;
			this.closeButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// toolTip
			// 
			this.toolTip.StripAmpersands = true;
			this.toolTip.Style = MetroFramework.MetroColorStyle.Blue;
			this.toolTip.StyleManager = null;
			this.toolTip.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// parentLabel
			// 
			this.parentLabel.AutoSize = true;
			this.parentLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
			this.parentLabel.Location = new System.Drawing.Point(357, 72);
			this.parentLabel.Name = "parentLabel";
			this.parentLabel.Size = new System.Drawing.Size(64, 25);
			this.parentLabel.TabIndex = 2;
			this.parentLabel.Text = "Parent:";
			this.parentLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// parentComboBox
			// 
			this.parentComboBox.FormattingEnabled = true;
			this.parentComboBox.ItemHeight = 23;
			this.parentComboBox.Items.AddRange(new object[] {
            "HEAD",
            "BODY",
            "ARM0",
            "ARM1",
            "LEG0",
            "LEG1",
            "HEADWEAR",
            "JACKET",
            "SLEEVE0",
            "SLEEVE1",
            "PANTS0",
            "PANTS1",
            "WAIST",
            "LEGGING0",
            "LEGGING1",
            "SOCK0",
            "SOCK1",
            "BOOT0",
            "BOOT1",
            "ARMARMOR1",
            "ARMARMOR0",
            "BODYARMOR",
            "BELT",
            "TOOL0",
            "TOOL1",
            "HELMET",
            "SHOULDER0",
            "SHOULDER1",
            "CHEST"});
			this.parentComboBox.Location = new System.Drawing.Point(417, 72);
			this.parentComboBox.Name = "parentComboBox";
			this.parentComboBox.Size = new System.Drawing.Size(163, 29);
			this.parentComboBox.TabIndex = 3;
			this.parentComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.parentComboBox.UseSelectable = true;
			// 
			// positionLabel
			// 
			this.positionLabel.AutoSize = true;
			this.positionLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
			this.positionLabel.Location = new System.Drawing.Point(33, 72);
			this.positionLabel.Name = "positionLabel";
			this.positionLabel.Size = new System.Drawing.Size(75, 25);
			this.positionLabel.TabIndex = 4;
			this.positionLabel.Text = "Position:";
			this.positionLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// PosXUpDown
			// 
			this.PosXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosXUpDown.DecimalPlaces = 3;
			this.PosXUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.PosXUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.PosXUpDown.Location = new System.Drawing.Point(120, 76);
			this.PosXUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.PosXUpDown.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
			this.PosXUpDown.Name = "PosXUpDown";
			this.PosXUpDown.Size = new System.Drawing.Size(73, 20);
			this.PosXUpDown.TabIndex = 19;
			// 
			// PosYUpDown
			// 
			this.PosYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosYUpDown.DecimalPlaces = 3;
			this.PosYUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.PosYUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.PosYUpDown.Location = new System.Drawing.Point(199, 76);
			this.PosYUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.PosYUpDown.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
			this.PosYUpDown.Name = "PosYUpDown";
			this.PosYUpDown.Size = new System.Drawing.Size(73, 20);
			this.PosYUpDown.TabIndex = 20;
			// 
			// PosZUpDown
			// 
			this.PosZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.PosZUpDown.DecimalPlaces = 3;
			this.PosZUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.PosZUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.PosZUpDown.Location = new System.Drawing.Point(278, 76);
			this.PosZUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.PosZUpDown.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
			this.PosZUpDown.Name = "PosZUpDown";
			this.PosZUpDown.Size = new System.Drawing.Size(73, 20);
			this.PosZUpDown.TabIndex = 21;
			// 
			// SizeZUpDown
			// 
			this.SizeZUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeZUpDown.DecimalPlaces = 3;
			this.SizeZUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.SizeZUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.SizeZUpDown.Location = new System.Drawing.Point(278, 102);
			this.SizeZUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.SizeZUpDown.Name = "SizeZUpDown";
			this.SizeZUpDown.Size = new System.Drawing.Size(73, 20);
			this.SizeZUpDown.TabIndex = 25;
			// 
			// SizeYUpDown
			// 
			this.SizeYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeYUpDown.DecimalPlaces = 3;
			this.SizeYUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.SizeYUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.SizeYUpDown.Location = new System.Drawing.Point(199, 102);
			this.SizeYUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.SizeYUpDown.Name = "SizeYUpDown";
			this.SizeYUpDown.Size = new System.Drawing.Size(73, 20);
			this.SizeYUpDown.TabIndex = 24;
			// 
			// SizeXUpDown
			// 
			this.SizeXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.SizeXUpDown.DecimalPlaces = 3;
			this.SizeXUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.SizeXUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.SizeXUpDown.Location = new System.Drawing.Point(120, 102);
			this.SizeXUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.SizeXUpDown.Name = "SizeXUpDown";
			this.SizeXUpDown.Size = new System.Drawing.Size(73, 20);
			this.SizeXUpDown.TabIndex = 23;
			// 
			// sizeLabel
			// 
			this.sizeLabel.AutoSize = true;
			this.sizeLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
			this.sizeLabel.Location = new System.Drawing.Point(33, 97);
			this.sizeLabel.Name = "sizeLabel";
			this.sizeLabel.Size = new System.Drawing.Size(46, 25);
			this.sizeLabel.TabIndex = 22;
			this.sizeLabel.Text = "Size:";
			this.sizeLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// uvYUpDown
			// 
			this.uvYUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.uvYUpDown.DecimalPlaces = 3;
			this.uvYUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.uvYUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.uvYUpDown.Location = new System.Drawing.Point(199, 128);
			this.uvYUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.uvYUpDown.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
			this.uvYUpDown.Name = "uvYUpDown";
			this.uvYUpDown.Size = new System.Drawing.Size(73, 20);
			this.uvYUpDown.TabIndex = 28;
			// 
			// uvXUpDown
			// 
			this.uvXUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.uvXUpDown.DecimalPlaces = 3;
			this.uvXUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.uvXUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.uvXUpDown.Location = new System.Drawing.Point(120, 128);
			this.uvXUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.uvXUpDown.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
			this.uvXUpDown.Name = "uvXUpDown";
			this.uvXUpDown.Size = new System.Drawing.Size(73, 20);
			this.uvXUpDown.TabIndex = 27;
			// 
			// uvLabel
			// 
			this.uvLabel.AutoSize = true;
			this.uvLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
			this.uvLabel.Location = new System.Drawing.Point(33, 123);
			this.uvLabel.Name = "uvLabel";
			this.uvLabel.Size = new System.Drawing.Size(39, 25);
			this.uvLabel.TabIndex = 26;
			this.uvLabel.Text = "UV:";
			this.uvLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// armorCheckBox
			// 
			this.armorCheckBox.AutoSize = true;
			this.armorCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Tall;
			this.armorCheckBox.FontWeight = MetroFramework.MetroCheckBoxWeight.Light;
			this.armorCheckBox.Location = new System.Drawing.Point(363, 101);
			this.armorCheckBox.Name = "armorCheckBox";
			this.armorCheckBox.Size = new System.Drawing.Size(225, 25);
			this.armorCheckBox.TabIndex = 29;
			this.armorCheckBox.Text = "Hide when wearing armor";
			this.armorCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.armorCheckBox.UseSelectable = true;
			// 
			// mirrorCheckBox
			// 
			this.mirrorCheckBox.AutoSize = true;
			this.mirrorCheckBox.FontSize = MetroFramework.MetroCheckBoxSize.Tall;
			this.mirrorCheckBox.FontWeight = MetroFramework.MetroCheckBoxWeight.Light;
			this.mirrorCheckBox.Location = new System.Drawing.Point(363, 130);
			this.mirrorCheckBox.Name = "mirrorCheckBox";
			this.mirrorCheckBox.Size = new System.Drawing.Size(133, 25);
			this.mirrorCheckBox.TabIndex = 30;
			this.mirrorCheckBox.Text = "Mirror Texture";
			this.mirrorCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
			this.mirrorCheckBox.UseSelectable = true;
			// 
			// inflationUpDown
			// 
			this.inflationUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
			this.inflationUpDown.DecimalPlaces = 3;
			this.inflationUpDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
			this.inflationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.inflationUpDown.Location = new System.Drawing.Point(120, 154);
			this.inflationUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this.inflationUpDown.Name = "inflationUpDown";
			this.inflationUpDown.Size = new System.Drawing.Size(73, 20);
			this.inflationUpDown.TabIndex = 32;
			// 
			// inflationLabel
			// 
			this.inflationLabel.AutoSize = true;
			this.inflationLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
			this.inflationLabel.Location = new System.Drawing.Point(33, 149);
			this.inflationLabel.Name = "inflationLabel";
			this.inflationLabel.Size = new System.Drawing.Size(55, 25);
			this.inflationLabel.TabIndex = 31;
			this.inflationLabel.Text = "Scale:";
			this.inflationLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
			// 
			// BoxEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(630, 220);
			this.Controls.Add(this.inflationUpDown);
			this.Controls.Add(this.uvYUpDown);
			this.Controls.Add(this.uvXUpDown);
			this.Controls.Add(this.SizeZUpDown);
			this.Controls.Add(this.SizeYUpDown);
			this.Controls.Add(this.SizeXUpDown);
			this.Controls.Add(this.PosZUpDown);
			this.Controls.Add(this.PosYUpDown);
			this.Controls.Add(this.PosXUpDown);
			this.Controls.Add(this.inflationLabel);
			this.Controls.Add(this.parentComboBox);
			this.Controls.Add(this.mirrorCheckBox);
			this.Controls.Add(this.armorCheckBox);
			this.Controls.Add(this.uvLabel);
			this.Controls.Add(this.sizeLabel);
			this.Controls.Add(this.positionLabel);
			this.Controls.Add(this.parentLabel);
			this.Controls.Add(this.closeButton);
			this.MaximumSize = new System.Drawing.Size(630, 554);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(630, 220);
			this.Name = "BoxEditor";
			this.Style = MetroFramework.MetroColorStyle.Silver;
			this.Text = "BOX Editor";
			this.Theme = MetroFramework.MetroThemeStyle.Dark;
			((System.ComponentModel.ISupportInitialize)(this.PosXUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PosYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PosZUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeZUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SizeXUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.uvYUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.uvXUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.inflationUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private MetroFramework.Controls.MetroButton closeButton;
		private MetroFramework.Components.MetroToolTip toolTip;
		private MetroFramework.Controls.MetroLabel parentLabel;
		private MetroFramework.Controls.MetroComboBox parentComboBox;
		private MetroFramework.Controls.MetroLabel positionLabel;
		private System.Windows.Forms.NumericUpDown PosXUpDown;
		private System.Windows.Forms.NumericUpDown PosYUpDown;
		private System.Windows.Forms.NumericUpDown PosZUpDown;
		private System.Windows.Forms.NumericUpDown SizeZUpDown;
		private System.Windows.Forms.NumericUpDown SizeYUpDown;
		private System.Windows.Forms.NumericUpDown SizeXUpDown;
		private MetroFramework.Controls.MetroLabel sizeLabel;
		private System.Windows.Forms.NumericUpDown uvYUpDown;
		private System.Windows.Forms.NumericUpDown uvXUpDown;
		private MetroFramework.Controls.MetroLabel uvLabel;
		private MetroFramework.Controls.MetroCheckBox armorCheckBox;
		private MetroFramework.Controls.MetroCheckBox mirrorCheckBox;
		private System.Windows.Forms.NumericUpDown inflationUpDown;
		private MetroFramework.Controls.MetroLabel inflationLabel;
	}
}