namespace minekampf.Forms
{
    partial class creatorSpotlight
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(creatorSpotlight));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.buttonOpenInBrowser = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            resources.ApplyResources(this.webBrowser1, "webBrowser1");
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScrollBarsEnabled = false;
            // 
            // buttonOpenInBrowser
            // 
            resources.ApplyResources(this.buttonOpenInBrowser, "buttonOpenInBrowser");
            this.buttonOpenInBrowser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.buttonOpenInBrowser.ForeColor = System.Drawing.Color.White;
            this.buttonOpenInBrowser.Name = "buttonOpenInBrowser";
            this.buttonOpenInBrowser.UseVisualStyleBackColor = false;
            this.buttonOpenInBrowser.Click += new System.EventHandler(this.buttonOpenInBrowser_Click);
            // 
            // creatorSpotlight
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonOpenInBrowser);
            this.Controls.Add(this.webBrowser1);
            this.MaximizeBox = false;
            this.Name = "creatorSpotlight";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.creatorSpotlight_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button buttonOpenInBrowser;
    }
}