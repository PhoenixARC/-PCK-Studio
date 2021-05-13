namespace MinecraftUSkinEditor
{
    partial class addnewskin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(addnewskin));
            this.textTheme = new System.Windows.Forms.TextBox();
            this.contextMenuSkin = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonDone = new System.Windows.Forms.Button();
            this.buttonModelGen = new System.Windows.Forms.Button();
            this.comboBoxSkinType = new System.Windows.Forms.ComboBox();
            this.buttonCape = new System.Windows.Forms.Button();
            this.buttonSkin = new System.Windows.Forms.Button();
            this.displayBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textThemeName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSkinName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textSkinID = new System.Windows.Forms.TextBox();
            this.radioAUTO = new System.Windows.Forms.RadioButton();
            this.radioLOCAL = new System.Windows.Forms.RadioButton();
            this.labelSelectTexture = new System.Windows.Forms.Label();
            this.radioSERVER = new System.Windows.Forms.RadioButton();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.pictureBoxWithInterpolationMode1 = new MinecraftUSkinEditor.PictureBoxWithInterpolationMode();
            this.pictureBoxTexture = new MinecraftUSkinEditor.PictureBoxWithInterpolationMode();
            this.contextMenuSkin.SuspendLayout();
            this.contextMenuCape.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTexture)).BeginInit();
            this.SuspendLayout();
            // 
            // textTheme
            // 
            resources.ApplyResources(this.textTheme, "textTheme");
            this.textTheme.Name = "textTheme";
            this.textTheme.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // contextMenuSkin
            // 
            resources.ApplyResources(this.contextMenuSkin, "contextMenuSkin");
            this.contextMenuSkin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem});
            this.contextMenuSkin.Name = "contextMenuSkin";
            // 
            // replaceToolStripMenuItem
            // 
            resources.ApplyResources(this.replaceToolStripMenuItem, "replaceToolStripMenuItem");
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // contextMenuCape
            // 
            resources.ApplyResources(this.contextMenuCape, "contextMenuCape");
            this.contextMenuCape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem1});
            this.contextMenuCape.Name = "contextMenuCape";
            // 
            // replaceToolStripMenuItem1
            // 
            resources.ApplyResources(this.replaceToolStripMenuItem1, "replaceToolStripMenuItem1");
            this.replaceToolStripMenuItem1.Name = "replaceToolStripMenuItem1";
            this.replaceToolStripMenuItem1.Click += new System.EventHandler(this.replaceToolStripMenuItem1_Click);
            // 
            // buttonDone
            // 
            resources.ApplyResources(this.buttonDone, "buttonDone");
            this.buttonDone.ForeColor = System.Drawing.Color.White;
            this.buttonDone.Name = "buttonDone";
            this.buttonDone.UseVisualStyleBackColor = true;
            this.buttonDone.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // buttonModelGen
            // 
            resources.ApplyResources(this.buttonModelGen, "buttonModelGen");
            this.buttonModelGen.ForeColor = System.Drawing.Color.White;
            this.buttonModelGen.Name = "buttonModelGen";
            this.buttonModelGen.UseVisualStyleBackColor = true;
            this.buttonModelGen.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // comboBoxSkinType
            // 
            resources.ApplyResources(this.comboBoxSkinType, "comboBoxSkinType");
            this.comboBoxSkinType.FormattingEnabled = true;
            this.comboBoxSkinType.Items.AddRange(new object[] {
            resources.GetString("comboBoxSkinType.Items"),
            resources.GetString("comboBoxSkinType.Items1"),
            resources.GetString("comboBoxSkinType.Items2")});
            this.comboBoxSkinType.Name = "comboBoxSkinType";
            // 
            // buttonCape
            // 
            resources.ApplyResources(this.buttonCape, "buttonCape");
            this.buttonCape.Name = "buttonCape";
            this.buttonCape.UseVisualStyleBackColor = true;
            this.buttonCape.Click += new System.EventHandler(this.buttonCape_Click);
            // 
            // buttonSkin
            // 
            resources.ApplyResources(this.buttonSkin, "buttonSkin");
            this.buttonSkin.Name = "buttonSkin";
            this.buttonSkin.UseVisualStyleBackColor = true;
            this.buttonSkin.Click += new System.EventHandler(this.buttonSkin_Click);
            // 
            // displayBox
            // 
            resources.ApplyResources(this.displayBox, "displayBox");
            this.displayBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.displayBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.displayBox.Name = "displayBox";
            this.displayBox.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // textThemeName
            // 
            resources.ApplyResources(this.textThemeName, "textThemeName");
            this.textThemeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textThemeName.Name = "textThemeName";
            this.textThemeName.TextChanged += new System.EventHandler(this.textThemeName_TextChanged);
            this.textThemeName.VisibleChanged += new System.EventHandler(this.textThemeName_VisibleChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // textSkinName
            // 
            resources.ApplyResources(this.textSkinName, "textSkinName");
            this.textSkinName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textSkinName.Name = "textSkinName";
            this.textSkinName.TextChanged += new System.EventHandler(this.textSkinName_TextChanged);
            this.textSkinName.VisibleChanged += new System.EventHandler(this.textSkinName_VisibleChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // textSkinID
            // 
            resources.ApplyResources(this.textSkinID, "textSkinID");
            this.textSkinID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textSkinID.Name = "textSkinID";
            this.textSkinID.TextChanged += new System.EventHandler(this.textSkinID_TextChanged_1);
            // 
            // radioAUTO
            // 
            resources.ApplyResources(this.radioAUTO, "radioAUTO");
            this.radioAUTO.ForeColor = System.Drawing.Color.White;
            this.radioAUTO.Name = "radioAUTO";
            this.radioAUTO.UseVisualStyleBackColor = true;
            this.radioAUTO.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioLOCAL
            // 
            resources.ApplyResources(this.radioLOCAL, "radioLOCAL");
            this.radioLOCAL.Checked = true;
            this.radioLOCAL.ForeColor = System.Drawing.Color.White;
            this.radioLOCAL.Name = "radioLOCAL";
            this.radioLOCAL.TabStop = true;
            this.radioLOCAL.UseVisualStyleBackColor = true;
            this.radioLOCAL.CheckedChanged += new System.EventHandler(this.radioLOCAL_CheckedChanged);
            // 
            // labelSelectTexture
            // 
            resources.ApplyResources(this.labelSelectTexture, "labelSelectTexture");
            this.labelSelectTexture.ForeColor = System.Drawing.Color.White;
            this.labelSelectTexture.Name = "labelSelectTexture";
            this.labelSelectTexture.Click += new System.EventHandler(this.label4_Click);
            // 
            // radioSERVER
            // 
            resources.ApplyResources(this.radioSERVER, "radioSERVER");
            this.radioSERVER.ForeColor = System.Drawing.Color.White;
            this.radioSERVER.Name = "radioSERVER";
            this.radioSERVER.UseVisualStyleBackColor = true;
            this.radioSERVER.CheckedChanged += new System.EventHandler(this.radioSERVER_CheckedChanged);
            // 
            // webBrowser1
            // 
            resources.ApplyResources(this.webBrowser1, "webBrowser1");
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.ScrollBarsEnabled = false;
            this.webBrowser1.Url = new System.Uri("http://nobledez.ga/nobledez.ga/pckStudio/SkinID.php", System.UriKind.Absolute);
            // 
            // pictureBoxWithInterpolationMode1
            // 
            resources.ApplyResources(this.pictureBoxWithInterpolationMode1, "pictureBoxWithInterpolationMode1");
            this.pictureBoxWithInterpolationMode1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.pictureBoxWithInterpolationMode1.Name = "pictureBoxWithInterpolationMode1";
            this.pictureBoxWithInterpolationMode1.TabStop = false;
            // 
            // pictureBoxTexture
            // 
            resources.ApplyResources(this.pictureBoxTexture, "pictureBoxTexture");
            this.pictureBoxTexture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxTexture.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.pictureBoxTexture.Name = "pictureBoxTexture";
            this.pictureBoxTexture.TabStop = false;
            this.pictureBoxTexture.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // addnewskin
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.radioSERVER);
            this.Controls.Add(this.labelSelectTexture);
            this.Controls.Add(this.radioLOCAL);
            this.Controls.Add(this.radioAUTO);
            this.Controls.Add(this.buttonDone);
            this.Controls.Add(this.buttonModelGen);
            this.Controls.Add(this.comboBoxSkinType);
            this.Controls.Add(this.buttonCape);
            this.Controls.Add(this.buttonSkin);
            this.Controls.Add(this.pictureBoxWithInterpolationMode1);
            this.Controls.Add(this.pictureBoxTexture);
            this.Controls.Add(this.displayBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textThemeName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textSkinName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textSkinID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "addnewskin";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.addnewskin_Load);
            this.contextMenuSkin.ResumeLayout(false);
            this.contextMenuCape.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.displayBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithInterpolationMode1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTexture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton radioUpsideDown;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textTheme;
        private System.Windows.Forms.ContextMenuStrip contextMenuSkin;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuCape;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem1;
        private System.Windows.Forms.Button buttonDone;
        private System.Windows.Forms.Button buttonModelGen;
        private System.Windows.Forms.ComboBox comboBoxSkinType;
        private System.Windows.Forms.Button buttonCape;
        private System.Windows.Forms.Button buttonSkin;
        private PictureBoxWithInterpolationMode pictureBoxWithInterpolationMode1;
        private PictureBoxWithInterpolationMode pictureBoxTexture;
        private System.Windows.Forms.PictureBox displayBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textThemeName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSkinName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textSkinID;
        private System.Windows.Forms.RadioButton radioAUTO;
        private System.Windows.Forms.RadioButton radioLOCAL;
        private System.Windows.Forms.Label labelSelectTexture;
        private System.Windows.Forms.RadioButton radioSERVER;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}