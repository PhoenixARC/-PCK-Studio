namespace PckStudio.Forms
{
    partial class installPS3
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
            this.metroTabPageMain = new MetroFramework.Controls.MetroTabPage();
            this.myTablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EurDig = new System.Windows.Forms.RadioButton();
            this.USDig = new System.Windows.Forms.RadioButton();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            this.textBoxHost = new MetroFramework.Controls.MetroTextBox();
            this.EurDisc = new System.Windows.Forms.RadioButton();
            this.USDisc = new System.Windows.Forms.RadioButton();
            this.listViewPCKS = new System.Windows.Forms.ListView();
            this.JPDig = new System.Windows.Forms.RadioButton();
            this.metroTabControlMain = new MetroFramework.Controls.MetroTabControl();
            this.contextMenuStripCaffiine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replacePCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metroTabPageMain.SuspendLayout();
            this.myTablePanel1.SuspendLayout();
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
            this.metroTabPageMain.Location = new System.Drawing.Point(4, 38);
            this.metroTabPageMain.Name = "metroTabPageMain";
            this.metroTabPageMain.Size = new System.Drawing.Size(427, 537);
            this.metroTabPageMain.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTabPageMain.TabIndex = 0;
            this.metroTabPageMain.Text = "Installer";
            this.metroTabPageMain.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabPageMain.VerticalScrollbarBarColor = true;
            this.metroTabPageMain.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPageMain.VerticalScrollbarSize = 10;
            // 
            // myTablePanel1
            // 
            this.myTablePanel1.BackColor = System.Drawing.Color.Transparent;
            this.myTablePanel1.ColumnCount = 3;
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.Controls.Add(this.EurDig, 1, 1);
            this.myTablePanel1.Controls.Add(this.USDig, 2, 1);
            this.myTablePanel1.Controls.Add(this.buttonServerToggle, 2, 0);
            this.myTablePanel1.Controls.Add(this.textBoxHost, 0, 0);
            this.myTablePanel1.Controls.Add(this.EurDisc, 1, 2);
            this.myTablePanel1.Controls.Add(this.USDisc, 2, 2);
            this.myTablePanel1.Controls.Add(this.listViewPCKS, 0, 3);
            this.myTablePanel1.Controls.Add(this.JPDig, 0, 1);
            this.myTablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTablePanel1.Location = new System.Drawing.Point(0, 0);
            this.myTablePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.myTablePanel1.Name = "myTablePanel1";
            this.myTablePanel1.RowCount = 7;
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.Size = new System.Drawing.Size(427, 537);
            this.myTablePanel1.TabIndex = 2;
            // 
            // EurDig
            // 
            this.EurDig.Appearance = System.Windows.Forms.Appearance.Button;
            this.EurDig.AutoSize = true;
            this.EurDig.BackColor = System.Drawing.Color.Transparent;
            this.EurDig.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.EurDig.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.EurDig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.EurDig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.EurDig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EurDig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EurDig.ForeColor = System.Drawing.Color.White;
            this.EurDig.Location = new System.Drawing.Point(145, 36);
            this.EurDig.Name = "EurDig";
            this.EurDig.Size = new System.Drawing.Size(100, 30);
            this.EurDig.TabIndex = 11;
            this.EurDig.Text = "EUR Digital";
            this.EurDig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EurDig.UseVisualStyleBackColor = false;
            this.EurDig.CheckedChanged += new System.EventHandler(this.EurDig_CheckedChanged);
            // 
            // USDig
            // 
            this.USDig.Appearance = System.Windows.Forms.Appearance.Button;
            this.USDig.AutoSize = true;
            this.USDig.BackColor = System.Drawing.Color.Transparent;
            this.USDig.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.USDig.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.USDig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.USDig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.USDig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.USDig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.USDig.ForeColor = System.Drawing.Color.White;
            this.USDig.Location = new System.Drawing.Point(287, 36);
            this.USDig.Name = "USDig";
            this.USDig.Size = new System.Drawing.Size(91, 30);
            this.USDig.TabIndex = 12;
            this.USDig.Text = "US Digital";
            this.USDig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.USDig.UseVisualStyleBackColor = false;
            this.USDig.CheckedChanged += new System.EventHandler(this.USDig_CheckedChanged);
            // 
            // buttonServerToggle
            // 
            this.buttonServerToggle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonServerToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(178)))), ((int)(((byte)(13)))));
            this.buttonServerToggle.FlatAppearance.BorderSize = 0;
            this.buttonServerToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServerToggle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonServerToggle.ForeColor = System.Drawing.Color.White;
            this.buttonServerToggle.Location = new System.Drawing.Point(287, 3);
            this.buttonServerToggle.Name = "buttonServerToggle";
            this.buttonServerToggle.Size = new System.Drawing.Size(137, 27);
            this.buttonServerToggle.TabIndex = 9;
            this.buttonServerToggle.Text = "Start";
            this.buttonServerToggle.UseVisualStyleBackColor = false;
            this.buttonServerToggle.Click += new System.EventHandler(this.buttonServerToggle_Click);
            // 
            // textBoxHost
            // 
            this.textBoxHost.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.textBoxHost, 2);
            // 
            // 
            // 
            this.textBoxHost.CustomButton.Image = null;
            this.textBoxHost.CustomButton.Location = new System.Drawing.Point(260, 2);
            this.textBoxHost.CustomButton.Name = "";
            this.textBoxHost.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxHost.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.CustomButton.TabIndex = 1;
            this.textBoxHost.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxHost.CustomButton.UseSelectable = true;
            this.textBoxHost.CustomButton.Visible = false;
            this.textBoxHost.IconRight = true;
            this.textBoxHost.Lines = new string[0];
            this.textBoxHost.Location = new System.Drawing.Point(3, 6);
            this.textBoxHost.MaxLength = 32767;
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.PasswordChar = '\0';
            this.textBoxHost.PromptText = "PS3 IP";
            this.textBoxHost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxHost.SelectedText = "";
            this.textBoxHost.SelectionLength = 0;
            this.textBoxHost.SelectionStart = 0;
            this.textBoxHost.ShortcutsEnabled = true;
            this.textBoxHost.Size = new System.Drawing.Size(278, 20);
            this.textBoxHost.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.TabIndex = 10;
            this.textBoxHost.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxHost.UseSelectable = true;
            this.textBoxHost.WaterMark = "PS3 IP";
            this.textBoxHost.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxHost.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // EurDisc
            // 
            this.EurDisc.Appearance = System.Windows.Forms.Appearance.Button;
            this.EurDisc.AutoSize = true;
            this.EurDisc.BackColor = System.Drawing.Color.Transparent;
            this.EurDisc.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.EurDisc.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.EurDisc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.EurDisc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.EurDisc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EurDisc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EurDisc.ForeColor = System.Drawing.Color.White;
            this.EurDisc.Location = new System.Drawing.Point(145, 72);
            this.EurDisc.Name = "EurDisc";
            this.EurDisc.Size = new System.Drawing.Size(84, 30);
            this.EurDisc.TabIndex = 0;
            this.EurDisc.Text = "EUR Disc";
            this.EurDisc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EurDisc.UseVisualStyleBackColor = false;
            this.EurDisc.CheckedChanged += new System.EventHandler(this.EurDisc_CheckedChanged);
            // 
            // USDisc
            // 
            this.USDisc.Appearance = System.Windows.Forms.Appearance.Button;
            this.USDisc.AutoSize = true;
            this.USDisc.BackColor = System.Drawing.Color.Transparent;
            this.USDisc.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.USDisc.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.USDisc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.USDisc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.USDisc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.USDisc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.USDisc.ForeColor = System.Drawing.Color.White;
            this.USDisc.Location = new System.Drawing.Point(287, 72);
            this.USDisc.Name = "USDisc";
            this.USDisc.Size = new System.Drawing.Size(75, 30);
            this.USDisc.TabIndex = 2;
            this.USDisc.Text = "US Disc";
            this.USDisc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.USDisc.UseVisualStyleBackColor = false;
            this.USDisc.CheckedChanged += new System.EventHandler(this.USDisc_CheckedChanged);
            // 
            // listViewPCKS
            // 
            this.listViewPCKS.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.myTablePanel1.SetColumnSpan(this.listViewPCKS, 3);
            this.listViewPCKS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPCKS.Enabled = false;
            this.listViewPCKS.HideSelection = false;
            this.listViewPCKS.Location = new System.Drawing.Point(3, 108);
            this.listViewPCKS.Name = "listViewPCKS";
            this.listViewPCKS.Size = new System.Drawing.Size(421, 426);
            this.listViewPCKS.TabIndex = 3;
            this.listViewPCKS.UseCompatibleStateImageBehavior = false;
            this.listViewPCKS.View = System.Windows.Forms.View.Details;
            this.listViewPCKS.SelectedIndexChanged += new System.EventHandler(this.listViewPCKS_SelectedIndexChanged);
            this.listViewPCKS.Click += new System.EventHandler(this.listViewPCKS_Click);
            this.listViewPCKS.DoubleClick += new System.EventHandler(this.listViewPCKS_DoubleClick);
            this.listViewPCKS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPCKS_MouseDown);
            // 
            // JPDig
            // 
            this.JPDig.Appearance = System.Windows.Forms.Appearance.Button;
            this.JPDig.AutoSize = true;
            this.JPDig.BackColor = System.Drawing.Color.Transparent;
            this.JPDig.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.JPDig.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.JPDig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.JPDig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.JPDig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.JPDig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JPDig.ForeColor = System.Drawing.Color.White;
            this.JPDig.Location = new System.Drawing.Point(3, 36);
            this.JPDig.Name = "JPDig";
            this.JPDig.Size = new System.Drawing.Size(47, 30);
            this.JPDig.TabIndex = 1;
            this.JPDig.Text = "JAP";
            this.JPDig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.JPDig.UseVisualStyleBackColor = false;
            this.JPDig.CheckedChanged += new System.EventHandler(this.JPDig_CheckedChanged);
            // 
            // metroTabControlMain
            // 
            this.metroTabControlMain.Controls.Add(this.metroTabPageMain);
            this.metroTabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControlMain.Location = new System.Drawing.Point(20, 60);
            this.metroTabControlMain.Name = "metroTabControlMain";
            this.metroTabControlMain.SelectedIndex = 0;
            this.metroTabControlMain.Size = new System.Drawing.Size(435, 579);
            this.metroTabControlMain.Style = MetroFramework.MetroColorStyle.White;
            this.metroTabControlMain.TabIndex = 0;
            this.metroTabControlMain.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControlMain.UseSelectable = true;
            // 
            // contextMenuStripCaffiine
            // 
            this.contextMenuStripCaffiine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.replacePCKToolStripMenuItem});
            this.contextMenuStripCaffiine.Name = "contextMenuStripCaffiine";
            this.contextMenuStripCaffiine.Size = new System.Drawing.Size(212, 48);
            this.contextMenuStripCaffiine.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripCaffiine_Opening);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // replacePCKToolStripMenuItem
            // 
            this.replacePCKToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replacePCKToolStripMenuItem.Name = "replacePCKToolStripMenuItem";
            this.replacePCKToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.replacePCKToolStripMenuItem.Text = "Replace with external PCK";
            this.replacePCKToolStripMenuItem.Click += new System.EventHandler(this.replacePCKToolStripMenuItem_Click);
            // 
            // installPS3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(475, 659);
            this.Controls.Add(this.metroTabControlMain);
            this.MaximizeBox = false;
            this.Name = "installPS3";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.SystemShadow;
            this.Style = MetroFramework.MetroColorStyle.White;
            this.Text = "Install to Playstation®3";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.installPS3_Load);
            this.metroTabPageMain.ResumeLayout(false);
            this.myTablePanel1.ResumeLayout(false);
            this.myTablePanel1.PerformLayout();
            this.metroTabControlMain.ResumeLayout(false);
            this.contextMenuStripCaffiine.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabPage metroTabPageMain;
        private System.Windows.Forms.TableLayoutPanel myTablePanel1;
        private System.Windows.Forms.RadioButton USDisc;
        private System.Windows.Forms.RadioButton JPDig;
        private System.Windows.Forms.RadioButton EurDisc;
        private System.Windows.Forms.ListView listViewPCKS;
        private MetroFramework.Controls.MetroTabControl metroTabControlMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCaffiine;
        private System.Windows.Forms.ToolStripMenuItem replacePCKToolStripMenuItem;
        private System.Windows.Forms.Button buttonServerToggle;
        private MetroFramework.Controls.MetroTextBox textBoxHost;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.RadioButton USDig;
        private System.Windows.Forms.RadioButton EurDig;
    }
}