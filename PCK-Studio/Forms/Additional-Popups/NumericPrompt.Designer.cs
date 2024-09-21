namespace PckStudio
{
    partial class NumericPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumericPrompt));
            this.TextLabel = new System.Windows.Forms.Label();
            this.ContextLabel = new MetroFramework.Controls.MetroLabel();
            this.ValueUpDown = new System.Windows.Forms.NumericUpDown();
            this.setBtn = new CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton();
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            // 
            // ContextLabel
            // 
            resources.ApplyResources(this.ContextLabel, "ContextLabel");
            this.ContextLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.ContextLabel.Name = "ContextLabel";
            this.ContextLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ContextLabel.WrapToLine = true;
            // 
            // ValueUpDown
            // 
            resources.ApplyResources(this.ValueUpDown, "ValueUpDown");
            this.ValueUpDown.Name = "ValueUpDown";
            // 
            // setBtn
            // 
            this.setBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.setBtn.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.setBtn.BorderRadius = 10;
            this.setBtn.BorderSize = 1;
            this.setBtn.ClickedColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.setBtn.FlatAppearance.BorderSize = 0;
            this.setBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.setBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            resources.ApplyResources(this.setBtn, "setBtn");
            this.setBtn.ForeColor = System.Drawing.Color.White;
            this.setBtn.GradientColorPrimary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.setBtn.GradientColorSecondary = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.setBtn.HoverOverColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(250)))), ((int)(((byte)(165)))));
            this.setBtn.Name = "setBtn";
            this.setBtn.TextColor = System.Drawing.Color.White;
            this.setBtn.UseVisualStyleBackColor = false;
            // 
            // NumericPrompt
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.setBtn);
            this.Controls.Add(this.ValueUpDown);
            this.Controls.Add(this.ContextLabel);
            this.Controls.Add(this.TextLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NumericPrompt";
            this.Load += new System.EventHandler(this.RenamePrompt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label TextLabel;
		public MetroFramework.Controls.MetroLabel ContextLabel;
        private System.Windows.Forms.NumericUpDown ValueUpDown;
        internal CBH.Ultimate.Controls.CrEaTiiOn_Ultimate_GradientButton setBtn;
    }
}