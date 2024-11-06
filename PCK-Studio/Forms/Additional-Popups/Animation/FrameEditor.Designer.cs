
namespace PckStudio.Forms.Additional_Popups.Animation
{
	partial class FrameEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.FrameTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.FrameList = new System.Windows.Forms.TreeView();
            this.TextureIcons = new System.Windows.Forms.ImageList(this.components);
            this.CancelButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            this.SaveButton = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            ((System.ComponentModel.ISupportInitialize)(this.FrameTimeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(64, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Frame Time";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(79, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "may/matt was here :3";
            // 
            // FrameTimeUpDown
            // 
            this.FrameTimeUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.FrameTimeUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FrameTimeUpDown.ForeColor = System.Drawing.SystemColors.Window;
            this.FrameTimeUpDown.Location = new System.Drawing.Point(135, 202);
            this.FrameTimeUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.FrameTimeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameTimeUpDown.Name = "FrameTimeUpDown";
            this.FrameTimeUpDown.Size = new System.Drawing.Size(73, 22);
            this.FrameTimeUpDown.TabIndex = 15;
            this.FrameTimeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FrameTimeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FrameList
            // 
            this.FrameList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.FrameList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FrameList.ForeColor = System.Drawing.Color.White;
            this.FrameList.HideSelection = false;
            this.FrameList.ImageIndex = 0;
            this.FrameList.ImageList = this.TextureIcons;
            this.FrameList.Location = new System.Drawing.Point(59, 37);
            this.FrameList.Name = "FrameList";
            this.FrameList.SelectedImageIndex = 0;
            this.FrameList.ShowLines = false;
            this.FrameList.ShowRootLines = false;
            this.FrameList.Size = new System.Drawing.Size(155, 159);
            this.FrameList.TabIndex = 1;
            // 
            // TextureIcons
            // 
            this.TextureIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.TextureIcons.ImageSize = new System.Drawing.Size(32, 32);
            this.TextureIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // CancelButton
            // 
            this.CancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.CancelButton.BorderRadius = 10;
            this.CancelButton.BorderSize = 1;
            this.CancelButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelButton.FlatAppearance.BorderSize = 0;
            this.CancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.CancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.CancelButton.ForeColor = System.Drawing.Color.White;
            this.CancelButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.CancelButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(36)))), ((int)(((byte)(38)))));
            this.CancelButton.Image = ((System.Drawing.Image)(resources.GetObject("CancelButton.Image")));
            this.CancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelButton.Location = new System.Drawing.Point(139, 240);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(120, 40);
            this.CancelButton.TabIndex = 22;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelButton.TextColor = System.Drawing.Color.White;
            this.CancelButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.CancelButton.UseVisualStyleBackColor = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SaveButton.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SaveButton.BorderRadius = 10;
            this.SaveButton.BorderSize = 1;
            this.SaveButton.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SaveButton.FlatAppearance.BorderSize = 0;
            this.SaveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.SaveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.SaveButton.ForeColor = System.Drawing.Color.White;
            this.SaveButton.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SaveButton.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.SaveButton.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SaveButton.Location = new System.Drawing.Point(13, 240);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(120, 40);
            this.SaveButton.TabIndex = 21;
            this.SaveButton.Text = "Save";
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveButton.TextColor = System.Drawing.Color.White;
            this.SaveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // FrameEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(273, 292);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.FrameList);
            this.Controls.Add(this.FrameTimeUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frame Editor";
            ((System.ComponentModel.ISupportInitialize)(this.FrameTimeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown FrameTimeUpDown;
		private System.Windows.Forms.TreeView FrameList;
		public System.Windows.Forms.ImageList TextureIcons;
        private CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton CancelButton;
        internal CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton SaveButton;
    }
}