namespace PckStudio.Forms.Additional_Features
{
    partial class CemuInstallPanel
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
            this.BrowseDirectoryBtn = new System.Windows.Forms.Button();
            this.GameDirectoryTextBox = new MetroFramework.Controls.MetroTextBox();
            this.radioButtonJap = new System.Windows.Forms.RadioButton();
            this.radioButtonEur = new System.Windows.Forms.RadioButton();
            this.textBoxHost = new MetroFramework.Controls.MetroTextBox();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            this.myTablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonUs = new System.Windows.Forms.RadioButton();
            this.DLCTreeView = new System.Windows.Forms.TreeView();
            this.DLCContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.openSkinPackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myTablePanel1.SuspendLayout();
            this.DLCContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // BrowseDirectoryBtn
            // 
            this.BrowseDirectoryBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BrowseDirectoryBtn.BackColor = System.Drawing.Color.Sienna;
            this.BrowseDirectoryBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowseDirectoryBtn.FlatAppearance.BorderSize = 0;
            this.BrowseDirectoryBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BrowseDirectoryBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.BrowseDirectoryBtn.ForeColor = System.Drawing.Color.White;
            this.BrowseDirectoryBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BrowseDirectoryBtn.Location = new System.Drawing.Point(287, 36);
            this.BrowseDirectoryBtn.Name = "BrowseDirectoryBtn";
            this.BrowseDirectoryBtn.Size = new System.Drawing.Size(138, 27);
            this.BrowseDirectoryBtn.TabIndex = 12;
            this.BrowseDirectoryBtn.Text = "Browse";
            this.BrowseDirectoryBtn.UseVisualStyleBackColor = false;
            this.BrowseDirectoryBtn.Click += new System.EventHandler(this.BrowseDirectoryBtn_Click);
            // 
            // GameDirectoryTextBox
            // 
            this.GameDirectoryTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.GameDirectoryTextBox, 2);
            // 
            // 
            // 
            this.GameDirectoryTextBox.CustomButton.Image = null;
            this.GameDirectoryTextBox.CustomButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.GameDirectoryTextBox.CustomButton.Location = new System.Drawing.Point(252, 1);
            this.GameDirectoryTextBox.CustomButton.Name = "";
            this.GameDirectoryTextBox.CustomButton.Size = new System.Drawing.Size(25, 25);
            this.GameDirectoryTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.GameDirectoryTextBox.CustomButton.TabIndex = 1;
            this.GameDirectoryTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.GameDirectoryTextBox.CustomButton.UseSelectable = true;
            this.GameDirectoryTextBox.CustomButton.Visible = false;
            this.GameDirectoryTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameDirectoryTextBox.IconRight = true;
            this.GameDirectoryTextBox.Lines = new string[0];
            this.GameDirectoryTextBox.Location = new System.Drawing.Point(3, 36);
            this.GameDirectoryTextBox.MaxLength = 32767;
            this.GameDirectoryTextBox.Name = "GameDirectoryTextBox";
            this.GameDirectoryTextBox.PasswordChar = '\0';
            this.GameDirectoryTextBox.PromptText = "Cemu Game Directory";
            this.GameDirectoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.GameDirectoryTextBox.SelectedText = "";
            this.GameDirectoryTextBox.SelectionLength = 0;
            this.GameDirectoryTextBox.SelectionStart = 0;
            this.GameDirectoryTextBox.ShortcutsEnabled = true;
            this.GameDirectoryTextBox.Size = new System.Drawing.Size(278, 27);
            this.GameDirectoryTextBox.Style = MetroFramework.MetroColorStyle.Blue;
            this.GameDirectoryTextBox.TabIndex = 11;
            this.GameDirectoryTextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.GameDirectoryTextBox.UseSelectable = true;
            this.GameDirectoryTextBox.WaterMark = "Cemu Game Directory";
            this.GameDirectoryTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.GameDirectoryTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // radioButtonJap
            // 
            this.radioButtonJap.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonJap.AutoSize = true;
            this.radioButtonJap.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonJap.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonJap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonJap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonJap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonJap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonJap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonJap.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonJap.ForeColor = System.Drawing.Color.White;
            this.radioButtonJap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonJap.Location = new System.Drawing.Point(287, 69);
            this.radioButtonJap.Name = "radioButtonJap";
            this.radioButtonJap.Size = new System.Drawing.Size(138, 30);
            this.radioButtonJap.TabIndex = 2;
            this.radioButtonJap.Text = "JAP";
            this.radioButtonJap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonJap.UseVisualStyleBackColor = false;
            this.radioButtonJap.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // radioButtonEur
            // 
            this.radioButtonEur.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonEur.AutoSize = true;
            this.radioButtonEur.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonEur.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonEur.Checked = true;
            this.radioButtonEur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonEur.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonEur.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonEur.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonEur.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonEur.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonEur.ForeColor = System.Drawing.Color.White;
            this.radioButtonEur.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonEur.Location = new System.Drawing.Point(3, 69);
            this.radioButtonEur.Name = "radioButtonEur";
            this.radioButtonEur.Size = new System.Drawing.Size(136, 30);
            this.radioButtonEur.TabIndex = 1;
            this.radioButtonEur.TabStop = true;
            this.radioButtonEur.Text = "EUR";
            this.radioButtonEur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonEur.UseVisualStyleBackColor = false;
            this.radioButtonEur.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // textBoxHost
            // 
            this.textBoxHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.textBoxHost, 2);
            // 
            // 
            // 
            this.textBoxHost.CustomButton.Image = null;
            this.textBoxHost.CustomButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textBoxHost.CustomButton.Location = new System.Drawing.Point(252, 1);
            this.textBoxHost.CustomButton.Name = "";
            this.textBoxHost.CustomButton.Size = new System.Drawing.Size(25, 25);
            this.textBoxHost.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.CustomButton.TabIndex = 1;
            this.textBoxHost.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxHost.CustomButton.UseSelectable = true;
            this.textBoxHost.CustomButton.Visible = false;
            this.textBoxHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHost.Enabled = false;
            this.textBoxHost.IconRight = true;
            this.textBoxHost.Lines = new string[0];
            this.textBoxHost.Location = new System.Drawing.Point(3, 3);
            this.textBoxHost.MaxLength = 32767;
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.PasswordChar = '\0';
            this.textBoxHost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxHost.SelectedText = "";
            this.textBoxHost.SelectionLength = 0;
            this.textBoxHost.SelectionStart = 0;
            this.textBoxHost.ShortcutsEnabled = true;
            this.textBoxHost.Size = new System.Drawing.Size(278, 27);
            this.textBoxHost.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.TabIndex = 10;
            this.textBoxHost.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxHost.UseSelectable = true;
            this.textBoxHost.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxHost.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // buttonServerToggle
            // 
            this.buttonServerToggle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonServerToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(178)))), ((int)(((byte)(13)))));
            this.buttonServerToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonServerToggle.FlatAppearance.BorderSize = 0;
            this.buttonServerToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServerToggle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.buttonServerToggle.ForeColor = System.Drawing.Color.White;
            this.buttonServerToggle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonServerToggle.Location = new System.Drawing.Point(287, 3);
            this.buttonServerToggle.Name = "buttonServerToggle";
            this.buttonServerToggle.Size = new System.Drawing.Size(138, 27);
            this.buttonServerToggle.TabIndex = 9;
            this.buttonServerToggle.Text = "Start";
            this.buttonServerToggle.UseVisualStyleBackColor = false;
            // 
            // myTablePanel1
            // 
            this.myTablePanel1.BackColor = System.Drawing.Color.Black;
            this.myTablePanel1.ColumnCount = 3;
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.Controls.Add(this.buttonServerToggle, 2, 0);
            this.myTablePanel1.Controls.Add(this.textBoxHost, 0, 0);
            this.myTablePanel1.Controls.Add(this.radioButtonEur, 0, 2);
            this.myTablePanel1.Controls.Add(this.radioButtonUs, 1, 2);
            this.myTablePanel1.Controls.Add(this.radioButtonJap, 2, 2);
            this.myTablePanel1.Controls.Add(this.GameDirectoryTextBox, 0, 1);
            this.myTablePanel1.Controls.Add(this.BrowseDirectoryBtn, 2, 1);
            this.myTablePanel1.Controls.Add(this.DLCTreeView, 0, 3);
            this.myTablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTablePanel1.Location = new System.Drawing.Point(0, 0);
            this.myTablePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.myTablePanel1.Name = "myTablePanel1";
            this.myTablePanel1.RowCount = 7;
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.myTablePanel1.Size = new System.Drawing.Size(428, 556);
            this.myTablePanel1.TabIndex = 4;
            // 
            // radioButtonUs
            // 
            this.radioButtonUs.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonUs.AutoSize = true;
            this.radioButtonUs.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonUs.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonUs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonUs.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonUs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonUs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonUs.ForeColor = System.Drawing.Color.White;
            this.radioButtonUs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonUs.Location = new System.Drawing.Point(145, 69);
            this.radioButtonUs.Name = "radioButtonUs";
            this.radioButtonUs.Size = new System.Drawing.Size(136, 30);
            this.radioButtonUs.TabIndex = 0;
            this.radioButtonUs.Text = "US";
            this.radioButtonUs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonUs.UseVisualStyleBackColor = false;
            this.radioButtonUs.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // DLCTreeView
            // 
            this.DLCTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myTablePanel1.SetColumnSpan(this.DLCTreeView, 3);
            this.DLCTreeView.ContextMenuStrip = this.DLCContextMenu;
            this.DLCTreeView.Location = new System.Drawing.Point(3, 105);
            this.DLCTreeView.Name = "DLCTreeView";
            this.DLCTreeView.Size = new System.Drawing.Size(422, 448);
            this.DLCTreeView.TabIndex = 13;
            this.DLCTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DLCTreeView_AfterSelect);
            // 
            // DLCContextMenu
            // 
            this.DLCContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSkinPackToolStripMenuItem,
            this.openTexturePackToolStripMenuItem});
            this.DLCContextMenu.Name = "DLCContextMenu";
            this.DLCContextMenu.Size = new System.Drawing.Size(173, 48);
            // 
            // openSkinPackToolStripMenuItem
            // 
            this.openSkinPackToolStripMenuItem.Name = "openSkinPackToolStripMenuItem";
            this.openSkinPackToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openSkinPackToolStripMenuItem.Text = "Open Skin Pack";
            this.openSkinPackToolStripMenuItem.Click += new System.EventHandler(this.openSkinPackToolStripMenuItem_Click);
            // 
            // openTexturePackToolStripMenuItem
            // 
            this.openTexturePackToolStripMenuItem.Name = "openTexturePackToolStripMenuItem";
            this.openTexturePackToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openTexturePackToolStripMenuItem.Text = "Open Texture Pack";
            this.openTexturePackToolStripMenuItem.Click += new System.EventHandler(this.openTexturePackToolStripMenuItem_Click);
            // 
            // CemuInstallPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.myTablePanel1);
            this.Name = "CemuInstallPanel";
            this.Size = new System.Drawing.Size(428, 556);
            this.myTablePanel1.ResumeLayout(false);
            this.myTablePanel1.PerformLayout();
            this.DLCContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BrowseDirectoryBtn;
        private MetroFramework.Controls.MetroTextBox GameDirectoryTextBox;
        private System.Windows.Forms.TableLayoutPanel myTablePanel1;
        private System.Windows.Forms.Button buttonServerToggle;
        private MetroFramework.Controls.MetroTextBox textBoxHost;
        private System.Windows.Forms.RadioButton radioButtonEur;
        private System.Windows.Forms.RadioButton radioButtonUs;
        private System.Windows.Forms.RadioButton radioButtonJap;
        private System.Windows.Forms.TreeView DLCTreeView;
        private MetroFramework.Controls.MetroContextMenu DLCContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openSkinPackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTexturePackToolStripMenuItem;
    }
}
