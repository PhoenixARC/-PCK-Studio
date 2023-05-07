﻿namespace PckStudio.Features
{
    partial class WiiUPanel
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
            this.myTablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            this.IPv4TextBox = new MetroFramework.Controls.MetroTextBox();
            this.listViewPCKS = new System.Windows.Forms.ListView();
            this.contextMenuStripCaffiine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replacePCKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioButtonUSB = new System.Windows.Forms.RadioButton();
            this.TextBoxPackImage = new MetroFramework.Controls.MetroTextBox();
            this.radioButtonSystem = new System.Windows.Forms.RadioButton();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.PackImageSelection = new System.Windows.Forms.Button();
            this.regionLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonEur = new System.Windows.Forms.RadioButton();
            this.radioButtonUs = new System.Windows.Forms.RadioButton();
            this.radioButtonJap = new System.Windows.Forms.RadioButton();
            this.myTablePanel1.SuspendLayout();
            this.contextMenuStripCaffiine.SuspendLayout();
            this.regionLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // myTablePanel1
            // 
            this.myTablePanel1.BackColor = System.Drawing.Color.Black;
            this.myTablePanel1.ColumnCount = 3;
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.Controls.Add(this.buttonServerToggle, 2, 0);
            this.myTablePanel1.Controls.Add(this.IPv4TextBox, 0, 0);
            this.myTablePanel1.Controls.Add(this.listViewPCKS, 0, 4);
            this.myTablePanel1.Controls.Add(this.radioButtonUSB);
            this.myTablePanel1.Controls.Add(this.TextBoxPackImage, 0, 1);
            this.myTablePanel1.Controls.Add(this.radioButtonSystem, 1, 2);
            this.myTablePanel1.Controls.Add(this.buttonSelect, 0, 2);
            this.myTablePanel1.Controls.Add(this.PackImageSelection, 2, 1);
            this.myTablePanel1.Controls.Add(this.regionLayoutPanel, 0, 3);
            this.myTablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTablePanel1.Location = new System.Drawing.Point(0, 0);
            this.myTablePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.myTablePanel1.Name = "myTablePanel1";
            this.myTablePanel1.RowCount = 8;
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.Size = new System.Drawing.Size(430, 550);
            this.myTablePanel1.TabIndex = 3;
            // 
            // buttonServerToggle
            // 
            this.buttonServerToggle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonServerToggle.BackColor = System.Drawing.Color.SpringGreen;
            this.buttonServerToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonServerToggle.Enabled = false;
            this.buttonServerToggle.FlatAppearance.BorderSize = 0;
            this.buttonServerToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServerToggle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.buttonServerToggle.ForeColor = System.Drawing.Color.White;
            this.buttonServerToggle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonServerToggle.Location = new System.Drawing.Point(289, 3);
            this.buttonServerToggle.Name = "buttonServerToggle";
            this.buttonServerToggle.Size = new System.Drawing.Size(138, 27);
            this.buttonServerToggle.TabIndex = 9;
            this.buttonServerToggle.Text = "Start";
            this.buttonServerToggle.UseVisualStyleBackColor = false;
            this.buttonServerToggle.Click += new System.EventHandler(this.buttonServerToggle_Click);
            // 
            // IPv4TextBox
            // 
            this.IPv4TextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.IPv4TextBox, 2);
            // 
            // 
            // 
            this.IPv4TextBox.CustomButton.Image = null;
            this.IPv4TextBox.CustomButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IPv4TextBox.CustomButton.Location = new System.Drawing.Point(312, 1);
            this.IPv4TextBox.CustomButton.Name = "";
            this.IPv4TextBox.CustomButton.Size = new System.Drawing.Size(25, 25);
            this.IPv4TextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.IPv4TextBox.CustomButton.TabIndex = 1;
            this.IPv4TextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.IPv4TextBox.CustomButton.UseSelectable = true;
            this.IPv4TextBox.CustomButton.Visible = false;
            this.IPv4TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IPv4TextBox.IconRight = true;
            this.IPv4TextBox.Lines = new string[0];
            this.IPv4TextBox.Location = new System.Drawing.Point(3, 3);
            this.IPv4TextBox.MaxLength = 32767;
            this.IPv4TextBox.Name = "IPv4TextBox";
            this.IPv4TextBox.PasswordChar = '\0';
            this.IPv4TextBox.PromptText = "Wii U IP";
            this.IPv4TextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.IPv4TextBox.SelectedText = "";
            this.IPv4TextBox.SelectionLength = 0;
            this.IPv4TextBox.SelectionStart = 0;
            this.IPv4TextBox.ShortcutsEnabled = true;
            this.IPv4TextBox.Size = new System.Drawing.Size(280, 27);
            this.IPv4TextBox.Style = MetroFramework.MetroColorStyle.Blue;
            this.IPv4TextBox.TabIndex = 10;
            this.IPv4TextBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.IPv4TextBox.UseSelectable = true;
            this.IPv4TextBox.WaterMark = "Wii U IP";
            this.IPv4TextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.IPv4TextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // listViewPCKS
            // 
            this.listViewPCKS.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.myTablePanel1.SetColumnSpan(this.listViewPCKS, 3);
            this.listViewPCKS.ContextMenuStrip = this.contextMenuStripCaffiine;
            this.listViewPCKS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPCKS.Enabled = false;
            this.listViewPCKS.HideSelection = false;
            this.listViewPCKS.Location = new System.Drawing.Point(3, 151);
            this.listViewPCKS.Name = "listViewPCKS";
            this.listViewPCKS.Size = new System.Drawing.Size(424, 396);
            this.listViewPCKS.TabIndex = 3;
            this.listViewPCKS.UseCompatibleStateImageBehavior = false;
            this.listViewPCKS.View = System.Windows.Forms.View.Details;
            this.listViewPCKS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewPCKS_MouseDown);
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
            // radioButtonUSB
            // 
            this.radioButtonUSB.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonUSB.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonUSB.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonUSB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonUSB.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonUSB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUSB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUSB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonUSB.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonUSB.ForeColor = System.Drawing.Color.White;
            this.radioButtonUSB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonUSB.Location = new System.Drawing.Point(289, 69);
            this.radioButtonUSB.Name = "radioButtonUSB";
            this.radioButtonUSB.Size = new System.Drawing.Size(138, 30);
            this.radioButtonUSB.TabIndex = 6;
            this.radioButtonUSB.Text = "USB";
            this.radioButtonUSB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonUSB.UseVisualStyleBackColor = false;
            this.radioButtonUSB.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // TextBoxPackImage
            // 
            this.TextBoxPackImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.TextBoxPackImage, 2);
            // 
            // 
            // 
            this.TextBoxPackImage.CustomButton.Image = null;
            this.TextBoxPackImage.CustomButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TextBoxPackImage.CustomButton.Location = new System.Drawing.Point(312, 1);
            this.TextBoxPackImage.CustomButton.Name = "";
            this.TextBoxPackImage.CustomButton.Size = new System.Drawing.Size(25, 25);
            this.TextBoxPackImage.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.TextBoxPackImage.CustomButton.TabIndex = 1;
            this.TextBoxPackImage.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.TextBoxPackImage.CustomButton.UseSelectable = true;
            this.TextBoxPackImage.CustomButton.Visible = false;
            this.TextBoxPackImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxPackImage.IconRight = true;
            this.TextBoxPackImage.Lines = new string[0];
            this.TextBoxPackImage.Location = new System.Drawing.Point(3, 36);
            this.TextBoxPackImage.MaxLength = 32767;
            this.TextBoxPackImage.Name = "TextBoxPackImage";
            this.TextBoxPackImage.PasswordChar = '\0';
            this.TextBoxPackImage.PromptText = "Pack Image";
            this.TextBoxPackImage.ReadOnly = true;
            this.TextBoxPackImage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.TextBoxPackImage.SelectedText = "";
            this.TextBoxPackImage.SelectionLength = 0;
            this.TextBoxPackImage.SelectionStart = 0;
            this.TextBoxPackImage.ShortcutsEnabled = true;
            this.TextBoxPackImage.Size = new System.Drawing.Size(280, 27);
            this.TextBoxPackImage.Style = MetroFramework.MetroColorStyle.Blue;
            this.TextBoxPackImage.TabIndex = 11;
            this.TextBoxPackImage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TextBoxPackImage.UseSelectable = true;
            this.TextBoxPackImage.WaterMark = "Pack Image";
            this.TextBoxPackImage.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.TextBoxPackImage.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // radioButtonSystem
            // 
            this.radioButtonSystem.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonSystem.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonSystem.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonSystem.Checked = true;
            this.radioButtonSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonSystem.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.radioButtonSystem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonSystem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonSystem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonSystem.ForeColor = System.Drawing.Color.White;
            this.radioButtonSystem.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonSystem.Location = new System.Drawing.Point(146, 69);
            this.radioButtonSystem.Name = "radioButtonSystem";
            this.radioButtonSystem.Size = new System.Drawing.Size(137, 30);
            this.radioButtonSystem.TabIndex = 5;
            this.radioButtonSystem.TabStop = true;
            this.radioButtonSystem.Text = "System";
            this.radioButtonSystem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonSystem.UseVisualStyleBackColor = false;
            this.radioButtonSystem.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // buttonSelect
            // 
            this.buttonSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSelect.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.buttonSelect.ForeColor = System.Drawing.Color.White;
            this.buttonSelect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonSelect.Location = new System.Drawing.Point(3, 69);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(137, 30);
            this.buttonSelect.TabIndex = 1;
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // PackImageSelection
            // 
            this.PackImageSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PackImageSelection.BackColor = System.Drawing.Color.DarkCyan;
            this.PackImageSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PackImageSelection.Enabled = false;
            this.PackImageSelection.FlatAppearance.BorderSize = 0;
            this.PackImageSelection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PackImageSelection.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.PackImageSelection.ForeColor = System.Drawing.Color.White;
            this.PackImageSelection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.PackImageSelection.Location = new System.Drawing.Point(289, 36);
            this.PackImageSelection.Name = "PackImageSelection";
            this.PackImageSelection.Size = new System.Drawing.Size(138, 27);
            this.PackImageSelection.TabIndex = 12;
            this.PackImageSelection.Text = "Browse";
            this.PackImageSelection.UseVisualStyleBackColor = false;
            this.PackImageSelection.Click += new System.EventHandler(this.PackImageSelection_Click);
            // 
            // regionLayoutPanel
            // 
            this.regionLayoutPanel.ColumnCount = 3;
            this.myTablePanel1.SetColumnSpan(this.regionLayoutPanel, 3);
            this.regionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.regionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.regionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.regionLayoutPanel.Controls.Add(this.radioButtonEur, 0, 0);
            this.regionLayoutPanel.Controls.Add(this.radioButtonUs, 1, 0);
            this.regionLayoutPanel.Controls.Add(this.radioButtonJap, 2, 0);
            this.regionLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regionLayoutPanel.Location = new System.Drawing.Point(3, 105);
            this.regionLayoutPanel.Name = "regionLayoutPanel";
            this.regionLayoutPanel.RowCount = 1;
            this.regionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.regionLayoutPanel.Size = new System.Drawing.Size(424, 40);
            this.regionLayoutPanel.TabIndex = 13;
            // 
            // radioButtonEur
            // 
            this.radioButtonEur.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonEur.AutoSize = true;
            this.radioButtonEur.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonEur.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonEur.Checked = true;
            this.radioButtonEur.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonEur.FlatAppearance.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.radioButtonEur.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonEur.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonEur.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonEur.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonEur.ForeColor = System.Drawing.Color.White;
            this.radioButtonEur.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonEur.Location = new System.Drawing.Point(3, 3);
            this.radioButtonEur.Name = "radioButtonEur";
            this.radioButtonEur.Size = new System.Drawing.Size(135, 34);
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
            this.radioButtonUs.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonUs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonUs.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonUs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonUs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonUs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonUs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonUs.ForeColor = System.Drawing.Color.White;
            this.radioButtonUs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonUs.Location = new System.Drawing.Point(144, 3);
            this.radioButtonUs.Name = "radioButtonUs";
            this.radioButtonUs.Size = new System.Drawing.Size(135, 34);
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
            this.radioButtonJap.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonJap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonJap.FlatAppearance.CheckedBackColor = System.Drawing.Color.Teal;
            this.radioButtonJap.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.radioButtonJap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.radioButtonJap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonJap.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.radioButtonJap.ForeColor = System.Drawing.Color.White;
            this.radioButtonJap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButtonJap.Location = new System.Drawing.Point(285, 3);
            this.radioButtonJap.Name = "radioButtonJap";
            this.radioButtonJap.Size = new System.Drawing.Size(136, 34);
            this.radioButtonJap.TabIndex = 2;
            this.radioButtonJap.Text = "JAP";
            this.radioButtonJap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonJap.UseVisualStyleBackColor = false;
            this.radioButtonJap.Click += new System.EventHandler(this.radioButton_Click);
            // 
            // WiiUPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.myTablePanel1);
            this.Name = "WiiUPanel";
            this.Size = new System.Drawing.Size(430, 550);
            this.myTablePanel1.ResumeLayout(false);
            this.contextMenuStripCaffiine.ResumeLayout(false);
            this.regionLayoutPanel.ResumeLayout(false);
            this.regionLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel myTablePanel1;
        private System.Windows.Forms.Button buttonServerToggle;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.RadioButton radioButtonSystem;
        private System.Windows.Forms.RadioButton radioButtonUSB;
        private System.Windows.Forms.RadioButton radioButtonEur;
        private System.Windows.Forms.RadioButton radioButtonUs;
        private System.Windows.Forms.RadioButton radioButtonJap;
        private MetroFramework.Controls.MetroTextBox IPv4TextBox;
        private System.Windows.Forms.ListView listViewPCKS;
        private MetroFramework.Controls.MetroTextBox TextBoxPackImage;
        private System.Windows.Forms.Button PackImageSelection;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCaffiine;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replacePCKToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel regionLayoutPanel;
    }
}
