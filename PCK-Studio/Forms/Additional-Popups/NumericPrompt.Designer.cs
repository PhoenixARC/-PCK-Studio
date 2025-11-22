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
            this.OKButton = new System.Windows.Forms.Button();
            this.ValueUpDown = new System.Windows.Forms.NumericUpDown();
            this.toolTipLabel = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TextLabel
            // 
            resources.ApplyResources(this.TextLabel, "TextLabel");
            this.TextLabel.ForeColor = System.Drawing.Color.White;
            this.TextLabel.Name = "TextLabel";
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.ForeColor = System.Drawing.Color.White;
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // ValueUpDown
            // 
            resources.ApplyResources(this.ValueUpDown, "ValueUpDown");
            this.ValueUpDown.BackColor = System.Drawing.SystemColors.WindowText;
            this.ValueUpDown.ForeColor = System.Drawing.SystemColors.Window;
            this.ValueUpDown.Name = "ValueUpDown";
            // 
            // toolTipLabel
            // 
            resources.ApplyResources(this.toolTipLabel, "toolTipLabel");
            this.toolTipLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.toolTipLabel.Name = "toolTipLabel";
            this.toolTipLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toolTipLabel.WrapToLine = true;
            // 
            // NumericPrompt
            // 
            this.AcceptButton = this.OKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ValueUpDown);
            this.Controls.Add(this.toolTipLabel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.TextLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NumericPrompt";
            this.Load += new System.EventHandler(this.RenamePrompt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ValueUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button OKButton;
        public System.Windows.Forms.Label TextLabel;
        private System.Windows.Forms.NumericUpDown ValueUpDown;
        private MetroFramework.Controls.MetroLabel toolTipLabel;
    }
}