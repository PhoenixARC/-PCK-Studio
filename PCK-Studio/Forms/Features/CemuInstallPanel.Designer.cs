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
            System.Windows.Forms.TableLayoutPanel layoutPanel;
            this.radioButtonEur = new System.Windows.Forms.RadioButton();
            this.radioButtonUs = new System.Windows.Forms.RadioButton();
            this.radioButtonJap = new System.Windows.Forms.RadioButton();
            this.GameDirectoryTextBox = new MetroFramework.Controls.MetroTextBox();
            this.BrowseDirectoryBtn = new System.Windows.Forms.Button();
            this.DLCTreeView = new System.Windows.Forms.TreeView();
            this.DLCContextMenu = new MetroFramework.Controls.MetroContextMenu(this.components);
            this.openSkinPackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTexturePackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCustomPckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            layoutPanel.SuspendLayout();
            this.DLCContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutPanel
            // 
            layoutPanel.BackColor = System.Drawing.Color.Black;
            layoutPanel.ColumnCount = 3;
            layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            layoutPanel.Controls.Add(this.radioButtonEur, 0, 1);
            layoutPanel.Controls.Add(this.radioButtonUs, 1, 1);
            layoutPanel.Controls.Add(this.radioButtonJap, 2, 1);
            layoutPanel.Controls.Add(this.GameDirectoryTextBox, 0, 0);
            layoutPanel.Controls.Add(this.BrowseDirectoryBtn, 2, 0);
            layoutPanel.Controls.Add(this.DLCTreeView, 0, 2);
            layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutPanel.Location = new System.Drawing.Point(0, 0);
            layoutPanel.Margin = new System.Windows.Forms.Padding(0);
            layoutPanel.Name = "layoutPanel";
            layoutPanel.RowCount = 3;
            layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            layoutPanel.Size = new System.Drawing.Size(428, 556);
            layoutPanel.TabIndex = 4;
            // 
            // radioButtonEur
            // 
            this.radioButtonEur.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonEur.AutoSize = true;
            this.radioButtonEur.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonEur.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonEur.Checked = true;
            this.radioButtonEur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonEur.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.radioButtonEur.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonEur.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonEur.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonEur.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonEur.ForeColor = System.Drawing.Color.White;
            this.radioButtonEur.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonEur.Location = new System.Drawing.Point(3, 36);
            this.radioButtonEur.Name = "radioButtonEur";
            this.radioButtonEur.Size = new System.Drawing.Size(136, 30);
            this.radioButtonEur.TabIndex = 1;
            this.radioButtonEur.TabStop = true;
            this.radioButtonEur.Text = "EUR";
            this.radioButtonEur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonEur.UseVisualStyleBackColor = false;
            this.radioButtonEur.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // radioButtonUs
            // 
            this.radioButtonUs.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonUs.AutoSize = true;
            this.radioButtonUs.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonUs.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonUs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonUs.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.radioButtonUs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonUs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonUs.ForeColor = System.Drawing.Color.White;
            this.radioButtonUs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonUs.Location = new System.Drawing.Point(145, 36);
            this.radioButtonUs.Name = "radioButtonUs";
            this.radioButtonUs.Size = new System.Drawing.Size(136, 30);
            this.radioButtonUs.TabIndex = 0;
            this.radioButtonUs.Text = "US";
            this.radioButtonUs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonUs.UseVisualStyleBackColor = false;
            this.radioButtonUs.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // radioButtonJap
            // 
            this.radioButtonJap.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonJap.AutoSize = true;
            this.radioButtonJap.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonJap.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.radioButtonJap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonJap.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.radioButtonJap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonJap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonJap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonJap.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonJap.ForeColor = System.Drawing.Color.White;
            this.radioButtonJap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonJap.Location = new System.Drawing.Point(287, 36);
            this.radioButtonJap.Name = "radioButtonJap";
            this.radioButtonJap.Size = new System.Drawing.Size(138, 30);
            this.radioButtonJap.TabIndex = 2;
            this.radioButtonJap.Text = "JAP";
            this.radioButtonJap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonJap.UseVisualStyleBackColor = false;
            this.radioButtonJap.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // GameDirectoryTextBox
            // 
            this.GameDirectoryTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            layoutPanel.SetColumnSpan(this.GameDirectoryTextBox, 2);
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
            this.GameDirectoryTextBox.Location = new System.Drawing.Point(3, 3);
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
            // BrowseDirectoryBtn
            // 
            this.BrowseDirectoryBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BrowseDirectoryBtn.BackColor = System.Drawing.Color.RoyalBlue;
            this.BrowseDirectoryBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowseDirectoryBtn.FlatAppearance.BorderSize = 0;
            this.BrowseDirectoryBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BrowseDirectoryBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.BrowseDirectoryBtn.ForeColor = System.Drawing.Color.White;
            this.BrowseDirectoryBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BrowseDirectoryBtn.Location = new System.Drawing.Point(287, 3);
            this.BrowseDirectoryBtn.Name = "BrowseDirectoryBtn";
            this.BrowseDirectoryBtn.Size = new System.Drawing.Size(138, 27);
            this.BrowseDirectoryBtn.TabIndex = 12;
            this.BrowseDirectoryBtn.Text = "Browse";
            this.BrowseDirectoryBtn.UseVisualStyleBackColor = false;
            this.BrowseDirectoryBtn.Click += new System.EventHandler(this.BrowseDirectoryBtn_Click);
            // 
            // DLCTreeView
            // 
            this.DLCTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            layoutPanel.SetColumnSpan(this.DLCTreeView, 3);
            this.DLCTreeView.ContextMenuStrip = this.DLCContextMenu;
            this.DLCTreeView.Location = new System.Drawing.Point(3, 72);
            this.DLCTreeView.Name = "DLCTreeView";
            this.DLCTreeView.Size = new System.Drawing.Size(422, 481);
            this.DLCTreeView.TabIndex = 13;
            this.DLCTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DLCTreeView_AfterSelect);
            // 
            // DLCContextMenu
            // 
            this.DLCContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSkinPackToolStripMenuItem,
            this.openTexturePackToolStripMenuItem,
            this.addCustomPckToolStripMenuItem,
            this.removePckToolStripMenuItem});
            this.DLCContextMenu.Name = "DLCContextMenu";
            this.DLCContextMenu.Size = new System.Drawing.Size(173, 92);
            // 
            // openSkinPackToolStripMenuItem
            // 
            this.openSkinPackToolStripMenuItem.Name = "openSkinPackToolStripMenuItem";
            this.openSkinPackToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openSkinPackToolStripMenuItem.Text = "Open Pack";
            this.openSkinPackToolStripMenuItem.Click += new System.EventHandler(this.openSkinPackToolStripMenuItem_Click);
            // 
            // openTexturePackToolStripMenuItem
            // 
            this.openTexturePackToolStripMenuItem.Name = "openTexturePackToolStripMenuItem";
            this.openTexturePackToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openTexturePackToolStripMenuItem.Text = "Open Texture Pack";
            this.openTexturePackToolStripMenuItem.Click += new System.EventHandler(this.openTexturePackToolStripMenuItem_Click);
            // 
            // addCustomPckToolStripMenuItem
            // 
            this.addCustomPckToolStripMenuItem.Name = "addCustomPckToolStripMenuItem";
            this.addCustomPckToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addCustomPckToolStripMenuItem.Text = "Add pck";
            this.addCustomPckToolStripMenuItem.Click += new System.EventHandler(this.addCustomPckToolStripMenuItem_Click);
            // 
            // removePckToolStripMenuItem
            // 
            this.removePckToolStripMenuItem.Name = "removePckToolStripMenuItem";
            this.removePckToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.removePckToolStripMenuItem.Text = "Remove pck";
            this.removePckToolStripMenuItem.Click += new System.EventHandler(this.removePckToolStripMenuItem_Click);
            // 
            // CemuInstallPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(layoutPanel);
            this.Name = "CemuInstallPanel";
            this.Size = new System.Drawing.Size(428, 556);
            layoutPanel.ResumeLayout(false);
            layoutPanel.PerformLayout();
            this.DLCContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BrowseDirectoryBtn;
        private MetroFramework.Controls.MetroTextBox GameDirectoryTextBox;
        private System.Windows.Forms.RadioButton radioButtonEur;
        private System.Windows.Forms.RadioButton radioButtonUs;
        private System.Windows.Forms.RadioButton radioButtonJap;
        private System.Windows.Forms.TreeView DLCTreeView;
        private MetroFramework.Controls.MetroContextMenu DLCContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openSkinPackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTexturePackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCustomPckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePckToolStripMenuItem;
    }
}
