namespace PckStudio.Forms
{
    partial class pckCenterOpen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pckCenterOpen));
            this.buttonDirect = new System.Windows.Forms.Button();
            this.labelName = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonInstallPs3 = new System.Windows.Forms.Button();
            this.buttonInstallXbox = new System.Windows.Forms.Button();
            this.buttonInstallWiiU = new System.Windows.Forms.Button();
            this.pictureBoxDisplay = new System.Windows.Forms.PictureBox();
            this.buttonBedrock = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDirect
            // 
            this.buttonDirect.BackColor = System.Drawing.Color.Purple;
            this.buttonDirect.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonDirect, "buttonDirect");
            this.buttonDirect.ForeColor = System.Drawing.Color.White;
            this.buttonDirect.Name = "buttonDirect";
            this.buttonDirect.UseVisualStyleBackColor = false;
            this.buttonDirect.Click += new System.EventHandler(this.buttonDirect_Click);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.ForeColor = System.Drawing.Color.White;
            this.labelName.Name = "labelName";
            // 
            // labelDesc
            // 
            resources.ApplyResources(this.labelDesc, "labelDesc");
            this.labelDesc.ForeColor = System.Drawing.Color.White;
            this.labelDesc.Name = "labelDesc";
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackColor = System.Drawing.Color.Red;
            this.buttonDelete.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.ForeColor = System.Drawing.Color.White;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.BackColor = System.Drawing.Color.SlateGray;
            this.buttonExport.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonExport, "buttonExport");
            this.buttonExport.ForeColor = System.Drawing.Color.White;
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.UseVisualStyleBackColor = false;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonInstallPs3
            // 
            this.buttonInstallPs3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.buttonInstallPs3.BackgroundImage = global::PckStudio.Properties.Resources.PS3;
            resources.ApplyResources(this.buttonInstallPs3, "buttonInstallPs3");
            this.buttonInstallPs3.FlatAppearance.BorderSize = 0;
            this.buttonInstallPs3.ForeColor = System.Drawing.Color.White;
            this.buttonInstallPs3.Name = "buttonInstallPs3";
            this.buttonInstallPs3.UseVisualStyleBackColor = false;
            this.buttonInstallPs3.Click += new System.EventHandler(this.buttonInstallPs3_Click);
            // 
            // buttonInstallXbox
            // 
            this.buttonInstallXbox.BackColor = System.Drawing.Color.Lime;
            this.buttonInstallXbox.BackgroundImage = global::PckStudio.Properties.Resources.Xbox;
            resources.ApplyResources(this.buttonInstallXbox, "buttonInstallXbox");
            this.buttonInstallXbox.FlatAppearance.BorderSize = 0;
            this.buttonInstallXbox.ForeColor = System.Drawing.Color.White;
            this.buttonInstallXbox.Name = "buttonInstallXbox";
            this.buttonInstallXbox.UseVisualStyleBackColor = false;
            this.buttonInstallXbox.Click += new System.EventHandler(this.buttonInstallXbox_Click);
            // 
            // buttonInstallWiiU
            // 
            this.buttonInstallWiiU.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.buttonInstallWiiU.BackgroundImage = global::PckStudio.Properties.Resources.WiiU;
            resources.ApplyResources(this.buttonInstallWiiU, "buttonInstallWiiU");
            this.buttonInstallWiiU.FlatAppearance.BorderSize = 0;
            this.buttonInstallWiiU.ForeColor = System.Drawing.Color.White;
            this.buttonInstallWiiU.Name = "buttonInstallWiiU";
            this.buttonInstallWiiU.UseVisualStyleBackColor = false;
            this.buttonInstallWiiU.Click += new System.EventHandler(this.buttonInstallWiiU_Click);
            // 
            // pictureBoxDisplay
            // 
            resources.ApplyResources(this.pictureBoxDisplay, "pictureBoxDisplay");
            this.pictureBoxDisplay.Name = "pictureBoxDisplay";
            this.pictureBoxDisplay.TabStop = false;
            // 
            // buttonBedrock
            // 
            this.buttonBedrock.BackColor = System.Drawing.Color.Green;
            this.buttonBedrock.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.buttonBedrock, "buttonBedrock");
            this.buttonBedrock.ForeColor = System.Drawing.Color.White;
            this.buttonBedrock.Name = "buttonBedrock";
            this.buttonBedrock.UseVisualStyleBackColor = false;
            this.buttonBedrock.Click += new System.EventHandler(this.convertToBedrockToolStripMenuItem_Click);
            // 
            // pckCenterOpen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.Controls.Add(this.buttonDirect);
            this.Controls.Add(this.buttonBedrock);
            this.Controls.Add(this.buttonInstallPs3);
            this.Controls.Add(this.buttonInstallXbox);
            this.Controls.Add(this.buttonInstallWiiU);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.pictureBoxDisplay);
            this.MaximizeBox = false;
            this.Name = "pckCenterOpen";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.White;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.pckCenterOpen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxDisplay;
        private System.Windows.Forms.Button buttonDirect;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonInstallWiiU;
        private System.Windows.Forms.Button buttonInstallXbox;
        private System.Windows.Forms.Button buttonInstallPs3;
        private System.Windows.Forms.Button buttonBedrock;
    }
}