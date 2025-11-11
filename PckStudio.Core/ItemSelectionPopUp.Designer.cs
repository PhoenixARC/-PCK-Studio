namespace PckStudio.Core.Additional_Popups
{
    partial class ItemSelectionPopUp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemSelectionPopUp));
            this.label2 = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // okBtn
            // 
            resources.ApplyResources(this.okBtn, "okBtn");
            this.okBtn.ForeColor = System.Drawing.Color.White;
            this.okBtn.Name = "okBtn";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ComboBox
            // 
            this.ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.ComboBox, "ComboBox");
            this.ComboBox.Name = "ComboBox";
            // 
            // ItemSelectionPopUp
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.ComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ItemSelectionPopUp";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox ComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okBtn;
    }
}