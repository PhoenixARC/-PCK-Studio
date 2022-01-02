namespace PckStudio.Forms
{
    partial class installWiiU
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(installWiiU));
            this.metroTabPageMain = new MetroFramework.Controls.MetroTabPage();
            this.myTablePanel1 = new PckStudio.Forms.MyTablePanel();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonSystem = new System.Windows.Forms.RadioButton();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.radioButtonUSB = new System.Windows.Forms.RadioButton();
            this.textBoxHost = new MetroFramework.Controls.MetroTextBox();
            this.radioButtonEur = new System.Windows.Forms.RadioButton();
            this.radioButtonUs = new System.Windows.Forms.RadioButton();
            this.radioButtonJap = new System.Windows.Forms.RadioButton();
            this.listViewPCKS = new System.Windows.Forms.ListView();
            this.metroTabControlMain = new MetroFramework.Controls.MetroTabControl();
            this.contextMenuStripCaffiine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replacePCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metroTabPageMain.SuspendLayout();
            this.myTablePanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.metroTabControlMain.SuspendLayout();
            this.contextMenuStripCaffiine.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroTabPageMain
            // 
            this.metroTabPageMain.Controls.Add(this.myTablePanel1);
            this.metroTabPageMain.HorizontalScrollbarBarColor = true;
            this.metroTabPageMain.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPageMain.HorizontalScrollbarSize = 10;
            resources.ApplyResources(this.metroTabPageMain, "metroTabPageMain");
            this.metroTabPageMain.Name = "metroTabPageMain";
            this.metroTabPageMain.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPageMain.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabPageMain.VerticalScrollbarBarColor = true;
            this.metroTabPageMain.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPageMain.VerticalScrollbarSize = 10;
            // 
            // myTablePanel1
            // 
            this.myTablePanel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.myTablePanel1, "myTablePanel1");
            this.myTablePanel1.Controls.Add(this.buttonServerToggle, 2, 0);
            this.myTablePanel1.Controls.Add(this.panel1, 0, 1);
            this.myTablePanel1.Controls.Add(this.textBoxHost, 0, 0);
            this.myTablePanel1.Controls.Add(this.radioButtonEur, 0, 2);
            this.myTablePanel1.Controls.Add(this.radioButtonUs, 1, 2);
            this.myTablePanel1.Controls.Add(this.radioButtonJap, 2, 2);
            this.myTablePanel1.Controls.Add(this.listViewPCKS, 0, 3);
            this.myTablePanel1.Name = "myTablePanel1";
            // 
            // buttonServerToggle
            // 
            resources.ApplyResources(this.buttonServerToggle, "buttonServerToggle");
            this.buttonServerToggle.BackColor = System.Drawing.Color.DodgerBlue;
            this.buttonServerToggle.FlatAppearance.BorderSize = 0;
            this.buttonServerToggle.ForeColor = System.Drawing.Color.White;
            this.buttonServerToggle.Name = "buttonServerToggle";
            this.buttonServerToggle.UseVisualStyleBackColor = false;
            this.buttonServerToggle.Click += new System.EventHandler(this.buttonServerToggle_Click);
            // 
            // panel1
            // 
            this.myTablePanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.radioButtonSystem);
            this.panel1.Controls.Add(this.buttonSelect);
            this.panel1.Controls.Add(this.radioButtonUSB);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // radioButtonSystem
            // 
            resources.ApplyResources(this.radioButtonSystem, "radioButtonSystem");
            this.radioButtonSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.radioButtonSystem.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonSystem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonSystem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonSystem.ForeColor = System.Drawing.Color.White;
            this.radioButtonSystem.Name = "radioButtonSystem";
            this.radioButtonSystem.TabStop = true;
            this.radioButtonSystem.UseVisualStyleBackColor = false;
            this.radioButtonSystem.CheckedChanged += new System.EventHandler(this.radioButtonSystem_CheckedChanged);
            // 
            // buttonSelect
            // 
            this.buttonSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.buttonSelect.BackgroundImage = global::PckStudio.Properties.Resources.sdDownload;
            resources.ApplyResources(this.buttonSelect, "buttonSelect");
            this.buttonSelect.ForeColor = System.Drawing.Color.White;
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.UseVisualStyleBackColor = false;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // radioButtonUSB
            // 
            resources.ApplyResources(this.radioButtonUSB, "radioButtonUSB");
            this.radioButtonUSB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.radioButtonUSB.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonUSB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUSB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUSB.ForeColor = System.Drawing.Color.White;
            this.radioButtonUSB.Name = "radioButtonUSB";
            this.radioButtonUSB.TabStop = true;
            this.radioButtonUSB.UseVisualStyleBackColor = false;
            this.radioButtonUSB.CheckedChanged += new System.EventHandler(this.radioButtonUSB_CheckedChanged);
            // 
            // textBoxHost
            // 
            resources.ApplyResources(this.textBoxHost, "textBoxHost");
            this.textBoxHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.textBoxHost, 2);
            // 
            // 
            // 
            this.textBoxHost.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.textBoxHost.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.textBoxHost.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.textBoxHost.CustomButton.Name = "";
            this.textBoxHost.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.textBoxHost.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.textBoxHost.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxHost.CustomButton.UseSelectable = true;
            this.textBoxHost.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.textBoxHost.IconRight = true;
            this.textBoxHost.Lines = new string[0];
            this.textBoxHost.MaxLength = 32767;
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.PasswordChar = '\0';
            this.textBoxHost.PromptText = "Wii U IP";
            this.textBoxHost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxHost.SelectedText = "";
            this.textBoxHost.SelectionLength = 0;
            this.textBoxHost.SelectionStart = 0;
            this.textBoxHost.ShortcutsEnabled = true;
            this.textBoxHost.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxHost.UseSelectable = true;
            this.textBoxHost.WaterMark = "Wii U IP";
            this.textBoxHost.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxHost.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // radioButtonEur
            // 
            resources.ApplyResources(this.radioButtonEur, "radioButtonEur");
            this.radioButtonEur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.radioButtonEur.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonEur.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonEur.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonEur.ForeColor = System.Drawing.Color.White;
            this.radioButtonEur.Name = "radioButtonEur";
            this.radioButtonEur.TabStop = true;
            this.radioButtonEur.UseVisualStyleBackColor = false;
            this.radioButtonEur.Click += new System.EventHandler(this.radioButtonEur_Click);
            // 
            // radioButtonUs
            // 
            resources.ApplyResources(this.radioButtonUs, "radioButtonUs");
            this.radioButtonUs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.radioButtonUs.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonUs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUs.ForeColor = System.Drawing.Color.White;
            this.radioButtonUs.Name = "radioButtonUs";
            this.radioButtonUs.TabStop = true;
            this.radioButtonUs.UseVisualStyleBackColor = false;
            this.radioButtonUs.Click += new System.EventHandler(this.radioButtonUs_Click);
            // 
            // radioButtonJap
            // 
            resources.ApplyResources(this.radioButtonJap, "radioButtonJap");
            this.radioButtonJap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.radioButtonJap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonJap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonJap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonJap.ForeColor = System.Drawing.Color.White;
            this.radioButtonJap.Name = "radioButtonJap";
            this.radioButtonJap.TabStop = true;
            this.radioButtonJap.UseVisualStyleBackColor = false;
            this.radioButtonJap.Click += new System.EventHandler(this.radioButtonJap_Click);
            // 
            // listViewPCKS
            // 
            this.listViewPCKS.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.listViewPCKS.BackColor = System.Drawing.Color.Black;
            this.listViewPCKS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myTablePanel1.SetColumnSpan(this.listViewPCKS, 3);
            resources.ApplyResources(this.listViewPCKS, "listViewPCKS");
            this.listViewPCKS.ForeColor = System.Drawing.Color.White;
            this.listViewPCKS.HideSelection = false;
            this.listViewPCKS.Name = "listViewPCKS";
            this.listViewPCKS.UseCompatibleStateImageBehavior = false;
            this.listViewPCKS.View = System.Windows.Forms.View.Details;
            this.listViewPCKS.SelectedIndexChanged += new System.EventHandler(this.listViewPCKS_SelectedIndexChanged);
            this.listViewPCKS.Click += new System.EventHandler(this.listViewPCKS_Click);
            this.listViewPCKS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPCKS_MouseDown);
            // 
            // metroTabControlMain
            // 
            this.metroTabControlMain.Controls.Add(this.metroTabPageMain);
            resources.ApplyResources(this.metroTabControlMain, "metroTabControlMain");
            this.metroTabControlMain.Name = "metroTabControlMain";
            this.metroTabControlMain.SelectedIndex = 0;
            this.metroTabControlMain.Style = MetroFramework.MetroColorStyle.White;
            this.metroTabControlMain.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControlMain.UseSelectable = true;
            // 
            // contextMenuStripCaffiine
            // 
            this.contextMenuStripCaffiine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.replacePCKToolStripMenuItem});
            this.contextMenuStripCaffiine.Name = "contextMenuStripCaffiine";
            resources.ApplyResources(this.contextMenuStripCaffiine, "contextMenuStripCaffiine");
            this.contextMenuStripCaffiine.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripCaffiine_Opening);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            resources.ApplyResources(this.replaceToolStripMenuItem, "replaceToolStripMenuItem");
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // replacePCKToolStripMenuItem
            // 
            this.replacePCKToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replacePCKToolStripMenuItem.Name = "replacePCKToolStripMenuItem";
            resources.ApplyResources(this.replacePCKToolStripMenuItem, "replacePCKToolStripMenuItem");
            this.replacePCKToolStripMenuItem.Click += new System.EventHandler(this.replacePCKToolStripMenuItem_Click);
            // 
            // installWiiU
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.Controls.Add(this.metroTabControlMain);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.Name = "installWiiU";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.SystemShadow;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabPageMain.ResumeLayout(false);
            this.myTablePanel1.ResumeLayout(false);
            this.myTablePanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.metroTabControlMain.ResumeLayout(false);
            this.contextMenuStripCaffiine.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabPage metroTabPageMain;
        private MyTablePanel myTablePanel1;
        private System.Windows.Forms.RadioButton radioButtonJap;
        private System.Windows.Forms.RadioButton radioButtonEur;
        private System.Windows.Forms.RadioButton radioButtonUs;
        private System.Windows.Forms.ListView listViewPCKS;
        private MetroFramework.Controls.MetroTabControl metroTabControlMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCaffiine;
        private System.Windows.Forms.ToolStripMenuItem replacePCKToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonSystem;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.RadioButton radioButtonUSB;
        private System.Windows.Forms.Button buttonServerToggle;
        private MetroFramework.Controls.MetroTextBox textBoxHost;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
    }
}