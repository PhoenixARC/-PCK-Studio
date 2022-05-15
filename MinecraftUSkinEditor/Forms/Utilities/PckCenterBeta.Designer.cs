namespace PckStudio.Forms.Utilities
{
    partial class PckCenterBeta
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PckCenterBeta));
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.VitaCheckBox = new MetroFramework.Controls.MetroCheckBox();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.OnlineTreeView = new System.Windows.Forms.TreeView();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.VitaCheckBox2 = new MetroFramework.Controls.MetroCheckBox();
            this.DeleteLocalButton = new System.Windows.Forms.Button();
            this.OpenFolderButton = new System.Windows.Forms.Button();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.LocalTreeView = new System.Windows.Forms.TreeView();
            this.metroComboBox1 = new MetroFramework.Controls.MetroComboBox();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.metroTabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroTabControl1.Controls.Add(this.metroTabPage1);
            this.metroTabControl1.Controls.Add(this.metroTabPage2);
            this.metroTabControl1.Location = new System.Drawing.Point(24, 64);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(767, 620);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Silver;
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.UseSelectable = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.VitaCheckBox);
            this.metroTabPage1.Controls.Add(this.DownloadButton);
            this.metroTabPage1.Controls.Add(this.metroLabel1);
            this.metroTabPage1.Controls.Add(this.tableLayoutPanel1);
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(759, 578);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "Online";
            this.metroTabPage1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // VitaCheckBox
            // 
            this.VitaCheckBox.AutoSize = true;
            this.VitaCheckBox.Location = new System.Drawing.Point(259, 511);
            this.VitaCheckBox.Name = "VitaCheckBox";
            this.VitaCheckBox.Size = new System.Drawing.Size(97, 15);
            this.VitaCheckBox.Style = MetroFramework.MetroColorStyle.Silver;
            this.VitaCheckBox.TabIndex = 5;
            this.VitaCheckBox.Text = "Vita/PS4 PCKs";
            this.VitaCheckBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.VitaCheckBox.UseSelectable = true;
            // 
            // DownloadButton
            // 
            this.DownloadButton.BackColor = System.Drawing.Color.Purple;
            this.DownloadButton.FlatAppearance.BorderSize = 0;
            this.DownloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DownloadButton.ForeColor = System.Drawing.Color.White;
            this.DownloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DownloadButton.Location = new System.Drawing.Point(259, 532);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(169, 43);
            this.DownloadButton.TabIndex = 4;
            this.DownloadButton.Text = "DOWNLOAD TO COLLECTION";
            this.DownloadButton.UseVisualStyleBackColor = false;
            this.DownloadButton.Visible = false;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.Location = new System.Drawing.Point(259, 3);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(146, 75);
            this.metroLabel1.TabIndex = 3;
            this.metroLabel1.Text = "Pack Name: %n\r\nAuthor: %a\r\nDescription: %d";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.metroComboBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.OnlineTreeView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.24221F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.228374F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 73.52941F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 578);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(246, 111);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // OnlineTreeView
            // 
            this.OnlineTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.OnlineTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OnlineTreeView.Location = new System.Drawing.Point(3, 156);
            this.OnlineTreeView.Name = "OnlineTreeView";
            this.OnlineTreeView.Size = new System.Drawing.Size(246, 419);
            this.OnlineTreeView.TabIndex = 0;
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.Controls.Add(this.VitaCheckBox2);
            this.metroTabPage2.Controls.Add(this.DeleteLocalButton);
            this.metroTabPage2.Controls.Add(this.OpenFolderButton);
            this.metroTabPage2.Controls.Add(this.metroLabel2);
            this.metroTabPage2.Controls.Add(this.tableLayoutPanel2);
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(759, 578);
            this.metroTabPage2.Style = MetroFramework.MetroColorStyle.Silver;
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "Local";
            this.metroTabPage2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            this.metroTabPage2.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.VerticalScrollbarSize = 10;
            // 
            // VitaCheckBox2
            // 
            this.VitaCheckBox2.AutoSize = true;
            this.VitaCheckBox2.Location = new System.Drawing.Point(258, 511);
            this.VitaCheckBox2.Name = "VitaCheckBox2";
            this.VitaCheckBox2.Size = new System.Drawing.Size(97, 15);
            this.VitaCheckBox2.Style = MetroFramework.MetroColorStyle.Silver;
            this.VitaCheckBox2.TabIndex = 9;
            this.VitaCheckBox2.Text = "Vita/PS4 PCKs";
            this.VitaCheckBox2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.VitaCheckBox2.UseSelectable = true;
            // 
            // DeleteLocalButton
            // 
            this.DeleteLocalButton.BackColor = System.Drawing.Color.Crimson;
            this.DeleteLocalButton.FlatAppearance.BorderSize = 0;
            this.DeleteLocalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteLocalButton.ForeColor = System.Drawing.Color.White;
            this.DeleteLocalButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeleteLocalButton.Location = new System.Drawing.Point(387, 532);
            this.DeleteLocalButton.Name = "DeleteLocalButton";
            this.DeleteLocalButton.Size = new System.Drawing.Size(87, 43);
            this.DeleteLocalButton.TabIndex = 8;
            this.DeleteLocalButton.Text = "DELETE";
            this.DeleteLocalButton.UseVisualStyleBackColor = false;
            this.DeleteLocalButton.Visible = false;
            // 
            // OpenFolderButton
            // 
            this.OpenFolderButton.BackColor = System.Drawing.Color.SteelBlue;
            this.OpenFolderButton.FlatAppearance.BorderSize = 0;
            this.OpenFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenFolderButton.ForeColor = System.Drawing.Color.White;
            this.OpenFolderButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OpenFolderButton.Location = new System.Drawing.Point(258, 532);
            this.OpenFolderButton.Name = "OpenFolderButton";
            this.OpenFolderButton.Size = new System.Drawing.Size(123, 43);
            this.OpenFolderButton.TabIndex = 7;
            this.OpenFolderButton.Text = "OPEN FOLDER";
            this.OpenFolderButton.UseVisualStyleBackColor = false;
            this.OpenFolderButton.Visible = false;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel2.Location = new System.Drawing.Point(258, 3);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(146, 75);
            this.metroLabel2.TabIndex = 6;
            this.metroLabel2.Text = "Pack Name: %n\r\nAuthor: %a\r\nDescription: %d";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.pictureBox2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.LocalTreeView, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.26829F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.7317F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(252, 578);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(246, 163);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // LocalTreeView
            // 
            this.LocalTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LocalTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocalTreeView.Location = new System.Drawing.Point(3, 172);
            this.LocalTreeView.Name = "LocalTreeView";
            this.LocalTreeView.Size = new System.Drawing.Size(246, 403);
            this.LocalTreeView.TabIndex = 0;
            // 
            // metroComboBox1
            // 
            this.metroComboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroComboBox1.FormattingEnabled = true;
            this.metroComboBox1.ItemHeight = 23;
            this.metroComboBox1.Location = new System.Drawing.Point(3, 120);
            this.metroComboBox1.Name = "metroComboBox1";
            this.metroComboBox1.Size = new System.Drawing.Size(246, 29);
            this.metroComboBox1.Style = MetroFramework.MetroColorStyle.Silver;
            this.metroComboBox1.TabIndex = 6;
            this.metroComboBox1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroComboBox1.UseSelectable = true;
            // 
            // PckCenterBeta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 707);
            this.Controls.Add(this.metroTabControl1);
            this.Name = "PckCenterBeta";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Pck Center";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.metroTabPage2.ResumeLayout(false);
            this.metroTabPage2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView OnlineTreeView;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button DownloadButton;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
        private System.Windows.Forms.Button DeleteLocalButton;
        private System.Windows.Forms.Button OpenFolderButton;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TreeView LocalTreeView;
        private MetroFramework.Controls.MetroCheckBox VitaCheckBox;
        private MetroFramework.Controls.MetroCheckBox VitaCheckBox2;
        private MetroFramework.Controls.MetroComboBox metroComboBox1;
    }
}