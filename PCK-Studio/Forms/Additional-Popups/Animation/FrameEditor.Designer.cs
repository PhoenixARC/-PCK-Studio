
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
            this.SaveBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.FrameIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.FrameTimeUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.FrameIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrameTimeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveBtn
            // 
            this.SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveBtn.ForeColor = System.Drawing.Color.White;
            this.SaveBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SaveBtn.Location = new System.Drawing.Point(55, 111);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 7;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(9, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Frame Index";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(9, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Frame Time";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(47, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "may/matt was here :3";
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelBtn.ForeColor = System.Drawing.Color.White;
            this.CancelBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelBtn.Location = new System.Drawing.Point(135, 111);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 13;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // FrameIndexUpDown
            // 
            this.FrameIndexUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.FrameIndexUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FrameIndexUpDown.ForeColor = System.Drawing.SystemColors.Window;
            this.FrameIndexUpDown.Location = new System.Drawing.Point(77, 54);
            this.FrameIndexUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameIndexUpDown.Name = "FrameIndexUpDown";
            this.FrameIndexUpDown.Size = new System.Drawing.Size(179, 20);
            this.FrameIndexUpDown.TabIndex = 14;
            this.FrameIndexUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FrameTimeUpDown
            // 
            this.FrameTimeUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.FrameTimeUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FrameTimeUpDown.ForeColor = System.Drawing.SystemColors.Window;
            this.FrameTimeUpDown.Location = new System.Drawing.Point(77, 81);
            this.FrameTimeUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.FrameTimeUpDown.Name = "FrameTimeUpDown";
            this.FrameTimeUpDown.Size = new System.Drawing.Size(179, 20);
            this.FrameTimeUpDown.TabIndex = 15;
            this.FrameTimeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FrameEditor
            // 
            this.AcceptButton = this.SaveBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(264, 140);
            this.ControlBox = false;
            this.Controls.Add(this.FrameTimeUpDown);
            this.Controls.Add(this.FrameIndexUpDown);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(266, 142);
            this.Name = "FrameEditor";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            ((System.ComponentModel.ISupportInitialize)(this.FrameIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrameTimeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.NumericUpDown FrameIndexUpDown;
        private System.Windows.Forms.NumericUpDown FrameTimeUpDown;
    }
}