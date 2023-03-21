namespace PckStudio.Forms.Additional_Features
{
    partial class PS3Panel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.EurDig = new System.Windows.Forms.RadioButton();
            this.USDig = new System.Windows.Forms.RadioButton();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            this.textBoxHost = new MetroFramework.Controls.MetroTextBox();
            this.EurDisc = new System.Windows.Forms.RadioButton();
            this.USDisc = new System.Windows.Forms.RadioButton();
            this.listViewPCKS = new System.Windows.Forms.ListView();
            this.JPDig = new System.Windows.Forms.RadioButton();
            this.ps3Panel = new PckStudio.Forms.MyTablePanel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.listView = new System.Windows.Forms.ListView();
            this.contextMenuStripCaffiine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replacePCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.ps3Panel.SuspendLayout();
            this.contextMenuStripCaffiine.SuspendLayout();
            this.SuspendLayout();
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
            // 
            // textBoxHost
            // 
            this.textBoxHost.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            // 
            // listViewPCKS
            // 
            this.listViewPCKS.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.listViewPCKS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPCKS.Enabled = false;
            this.listViewPCKS.HideSelection = false;
            this.listViewPCKS.Location = new System.Drawing.Point(3, 108);
            this.listViewPCKS.Name = "listViewPCKS";
            this.listViewPCKS.Size = new System.Drawing.Size(421, 426);
            this.listViewPCKS.TabIndex = 3;
            this.listViewPCKS.UseCompatibleStateImageBehavior = false;
            this.listViewPCKS.View = System.Windows.Forms.View.Details;
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
            // 
            // ps3Panel
            // 
            this.ps3Panel.BackColor = System.Drawing.Color.Black;
            this.ps3Panel.ColumnCount = 3;
            this.ps3Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ps3Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.ps3Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.ps3Panel.Controls.Add(this.radioButton1, 1, 1);
            this.ps3Panel.Controls.Add(this.radioButton2, 2, 1);
            this.ps3Panel.Controls.Add(this.button1, 2, 0);
            this.ps3Panel.Controls.Add(this.metroTextBox1, 0, 0);
            this.ps3Panel.Controls.Add(this.radioButton3, 1, 2);
            this.ps3Panel.Controls.Add(this.radioButton4, 2, 2);
            this.ps3Panel.Controls.Add(this.listView, 0, 3);
            this.ps3Panel.Controls.Add(this.radioButton5, 0, 1);
            this.ps3Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ps3Panel.Location = new System.Drawing.Point(0, 0);
            this.ps3Panel.Margin = new System.Windows.Forms.Padding(0);
            this.ps3Panel.Name = "ps3Panel";
            this.ps3Panel.RowCount = 7;
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ps3Panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ps3Panel.Size = new System.Drawing.Size(468, 633);
            this.ps3Panel.TabIndex = 3;
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButton1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(158, 36);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(100, 30);
            this.radioButton1.TabIndex = 11;
            this.radioButton1.Text = "EUR Digital";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = false;
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = System.Drawing.Color.Transparent;
            this.radioButton2.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButton2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            this.radioButton2.Location = new System.Drawing.Point(314, 36);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(91, 30);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.Text = "US Digital";
            this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(178)))), ((int)(((byte)(13)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(314, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 27);
            this.button1.TabIndex = 9;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // metroTextBox1
            // 
            this.metroTextBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.metroTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ps3Panel.SetColumnSpan(this.metroTextBox1, 2);
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(260, 2);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.IconRight = true;
            this.metroTextBox1.Lines = new string[0];
            this.metroTextBox1.Location = new System.Drawing.Point(3, 6);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.PromptText = "PS3 IP";
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.ShortcutsEnabled = true;
            this.metroTextBox1.Size = new System.Drawing.Size(278, 20);
            this.metroTextBox1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.TabIndex = 10;
            this.metroTextBox1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMark = "PS3 IP";
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // radioButton3
            // 
            this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton3.AutoSize = true;
            this.radioButton3.BackColor = System.Drawing.Color.Transparent;
            this.radioButton3.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButton3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton3.ForeColor = System.Drawing.Color.White;
            this.radioButton3.Location = new System.Drawing.Point(158, 72);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(84, 30);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.Text = "EUR Disc";
            this.radioButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton3.UseVisualStyleBackColor = false;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.EurDisc_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton4.AutoSize = true;
            this.radioButton4.BackColor = System.Drawing.Color.Transparent;
            this.radioButton4.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButton4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButton4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButton4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton4.ForeColor = System.Drawing.Color.White;
            this.radioButton4.Location = new System.Drawing.Point(314, 72);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(75, 30);
            this.radioButton4.TabIndex = 2;
            this.radioButton4.Text = "US Disc";
            this.radioButton4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton4.UseVisualStyleBackColor = false;
            // 
            // listView
            // 
            this.listView.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.listView.BackColor = System.Drawing.SystemColors.Info;
            this.ps3Panel.SetColumnSpan(this.listView, 3);
            this.listView.ContextMenuStrip = this.contextMenuStripCaffiine;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Enabled = false;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(3, 108);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(462, 522);
            this.listView.TabIndex = 3;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPCKS_MouseDown);
            // 
            // contextMenuStripCaffiine
            // 
            this.contextMenuStripCaffiine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.replacePCKToolStripMenuItem});
            this.contextMenuStripCaffiine.Name = "contextMenuStripCaffiine";
            this.contextMenuStripCaffiine.Size = new System.Drawing.Size(212, 48);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            // 
            // replacePCKToolStripMenuItem
            // 
            this.replacePCKToolStripMenuItem.Image = global::PckStudio.Properties.Resources.Replace;
            this.replacePCKToolStripMenuItem.Name = "replacePCKToolStripMenuItem";
            this.replacePCKToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.replacePCKToolStripMenuItem.Text = "Replace with external PCK";
            // 
            // radioButton5
            // 
            this.radioButton5.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton5.AutoSize = true;
            this.radioButton5.BackColor = System.Drawing.Color.Transparent;
            this.radioButton5.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButton5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButton5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButton5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton5.ForeColor = System.Drawing.Color.White;
            this.radioButton5.Location = new System.Drawing.Point(3, 36);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(47, 30);
            this.radioButton5.TabIndex = 1;
            this.radioButton5.Text = "JAP";
            this.radioButton5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton5.UseVisualStyleBackColor = false;
            this.radioButton5.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // PS3InstallPanel
            // 
            this.Controls.Add(this.ps3Panel);
            this.Name = "PS3InstallPanel";
            this.Size = new System.Drawing.Size(468, 633);
            this.ps3Panel.ResumeLayout(false);
            this.ps3Panel.PerformLayout();
            this.contextMenuStripCaffiine.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton EurDig;
        private System.Windows.Forms.RadioButton USDig;
        private System.Windows.Forms.Button buttonServerToggle;
        private MetroFramework.Controls.MetroTextBox textBoxHost;
        private System.Windows.Forms.RadioButton EurDisc;
        private System.Windows.Forms.RadioButton USDisc;
        private System.Windows.Forms.ListView listViewPCKS;
        private System.Windows.Forms.RadioButton JPDig;
        private MyTablePanel ps3Panel;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button button1;
        private MetroFramework.Controls.MetroTextBox metroTextBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCaffiine;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replacePCKToolStripMenuItem;
    }
}
