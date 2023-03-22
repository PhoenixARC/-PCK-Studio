namespace PckStudio.Forms.Utilities
{
    partial class ConsoleInstaller
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
            MetroFramework.Controls.MetroLabel metroLabel1;
            MetroFramework.Controls.MetroLabel metroLabel2;
            this.selectedConsoleComboBox = new MetroFramework.Controls.MetroComboBox();
            this.myTablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.EurDig = new System.Windows.Forms.RadioButton();
            this.USDig = new System.Windows.Forms.RadioButton();
            this.textBoxHost = new MetroFramework.Controls.MetroTextBox();
            this.EurDisc = new System.Windows.Forms.RadioButton();
            this.USDisc = new System.Windows.Forms.RadioButton();
            this.listViewPCKS = new System.Windows.Forms.ListView();
            this.JPDig = new System.Windows.Forms.RadioButton();
            this.buttonServerToggle = new System.Windows.Forms.Button();
            metroLabel1 = new MetroFramework.Controls.MetroLabel();
            metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.myTablePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            metroLabel1.AutoSize = true;
            metroLabel1.Location = new System.Drawing.Point(264, 73);
            metroLabel1.Name = "metroLabel1";
            metroLabel1.Size = new System.Drawing.Size(90, 19);
            metroLabel1.Style = MetroFramework.MetroColorStyle.Black;
            metroLabel1.TabIndex = 1;
            metroLabel1.Text = "Console Type:";
            metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel2
            // 
            metroLabel2.AutoSize = true;
            metroLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            metroLabel2.Location = new System.Drawing.Point(129, 0);
            metroLabel2.Name = "metroLabel2";
            metroLabel2.Size = new System.Drawing.Size(120, 35);
            metroLabel2.Style = MetroFramework.MetroColorStyle.Black;
            metroLabel2.TabIndex = 13;
            metroLabel2.Text = "Console Type:";
            metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // selectedConsoleComboBox
            // 
            this.selectedConsoleComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.selectedConsoleComboBox.FormattingEnabled = true;
            this.selectedConsoleComboBox.ItemHeight = 23;
            this.selectedConsoleComboBox.Items.AddRange(new object[] {
            "Wii U",
            "Play Station 3",
            "PS Vita"});
            this.selectedConsoleComboBox.Location = new System.Drawing.Point(255, 3);
            this.selectedConsoleComboBox.Name = "selectedConsoleComboBox";
            this.selectedConsoleComboBox.PromptText = "Select console";
            this.selectedConsoleComboBox.Size = new System.Drawing.Size(121, 29);
            this.selectedConsoleComboBox.Style = MetroFramework.MetroColorStyle.Black;
            this.selectedConsoleComboBox.TabIndex = 0;
            this.selectedConsoleComboBox.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.selectedConsoleComboBox.UseSelectable = true;
            // 
            // myTablePanel1
            // 
            this.myTablePanel1.BackColor = System.Drawing.Color.Transparent;
            this.myTablePanel1.ColumnCount = 3;
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.myTablePanel1.Controls.Add(this.EurDig, 1, 2);
            this.myTablePanel1.Controls.Add(this.USDig, 2, 2);
            this.myTablePanel1.Controls.Add(this.textBoxHost, 0, 1);
            this.myTablePanel1.Controls.Add(this.EurDisc, 1, 3);
            this.myTablePanel1.Controls.Add(this.USDisc, 2, 3);
            this.myTablePanel1.Controls.Add(this.listViewPCKS, 0, 4);
            this.myTablePanel1.Controls.Add(this.JPDig, 0, 2);
            this.myTablePanel1.Controls.Add(this.buttonServerToggle, 2, 1);
            this.myTablePanel1.Controls.Add(this.selectedConsoleComboBox, 2, 0);
            this.myTablePanel1.Controls.Add(metroLabel2, 1, 0);
            this.myTablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTablePanel1.Location = new System.Drawing.Point(20, 60);
            this.myTablePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.myTablePanel1.Name = "myTablePanel1";
            this.myTablePanel1.RowCount = 5;
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.myTablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.myTablePanel1.Size = new System.Drawing.Size(379, 561);
            this.myTablePanel1.TabIndex = 3;
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
            this.EurDig.Location = new System.Drawing.Point(129, 71);
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
            this.USDig.Location = new System.Drawing.Point(255, 71);
            this.USDig.Name = "USDig";
            this.USDig.Size = new System.Drawing.Size(91, 30);
            this.USDig.TabIndex = 12;
            this.USDig.Text = "US Digital";
            this.USDig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.USDig.UseVisualStyleBackColor = false;
            // 
            // textBoxHost
            // 
            this.textBoxHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.myTablePanel1.SetColumnSpan(this.textBoxHost, 2);
            // 
            // 
            // 
            this.textBoxHost.CustomButton.Image = null;
            this.textBoxHost.CustomButton.Location = new System.Drawing.Point(274, 2);
            this.textBoxHost.CustomButton.Name = "";
            this.textBoxHost.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxHost.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.CustomButton.TabIndex = 1;
            this.textBoxHost.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxHost.CustomButton.UseSelectable = true;
            this.textBoxHost.CustomButton.Visible = false;
            this.textBoxHost.IconRight = true;
            this.textBoxHost.Lines = new string[0];
            this.textBoxHost.Location = new System.Drawing.Point(3, 38);
            this.textBoxHost.MaxLength = 32767;
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.PasswordChar = '\0';
            this.textBoxHost.PromptText = "IP Address";
            this.textBoxHost.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxHost.SelectedText = "";
            this.textBoxHost.SelectionLength = 0;
            this.textBoxHost.SelectionStart = 0;
            this.textBoxHost.ShortcutsEnabled = true;
            this.textBoxHost.Size = new System.Drawing.Size(246, 20);
            this.textBoxHost.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxHost.TabIndex = 10;
            this.textBoxHost.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.textBoxHost.UseSelectable = true;
            this.textBoxHost.WaterMark = "IP Address";
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
            this.EurDisc.Location = new System.Drawing.Point(129, 107);
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
            this.USDisc.Location = new System.Drawing.Point(255, 107);
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
            this.myTablePanel1.SetColumnSpan(this.listViewPCKS, 3);
            this.listViewPCKS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPCKS.Enabled = false;
            this.listViewPCKS.HideSelection = false;
            this.listViewPCKS.Location = new System.Drawing.Point(3, 143);
            this.listViewPCKS.Name = "listViewPCKS";
            this.listViewPCKS.Size = new System.Drawing.Size(373, 415);
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
            this.JPDig.Location = new System.Drawing.Point(3, 71);
            this.JPDig.Name = "JPDig";
            this.JPDig.Size = new System.Drawing.Size(47, 30);
            this.JPDig.TabIndex = 1;
            this.JPDig.Text = "JAP";
            this.JPDig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.JPDig.UseVisualStyleBackColor = false;
            // 
            // buttonServerToggle
            // 
            this.buttonServerToggle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonServerToggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(178)))), ((int)(((byte)(13)))));
            this.buttonServerToggle.FlatAppearance.BorderSize = 0;
            this.buttonServerToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonServerToggle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonServerToggle.ForeColor = System.Drawing.Color.White;
            this.buttonServerToggle.Location = new System.Drawing.Point(255, 38);
            this.buttonServerToggle.Name = "buttonServerToggle";
            this.buttonServerToggle.Size = new System.Drawing.Size(121, 27);
            this.buttonServerToggle.TabIndex = 9;
            this.buttonServerToggle.Text = "Start";
            this.buttonServerToggle.UseVisualStyleBackColor = false;
            // 
            // ConsoleInstaller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 641);
            this.Controls.Add(this.myTablePanel1);
            this.Controls.Add(metroLabel1);
            this.Name = "ConsoleInstaller";
            this.Style = MetroFramework.MetroColorStyle.Silver;
            this.Text = "Console Installer";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.myTablePanel1.ResumeLayout(false);
            this.myTablePanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox selectedConsoleComboBox;
        private System.Windows.Forms.TableLayoutPanel myTablePanel1;
        private System.Windows.Forms.RadioButton EurDig;
        private System.Windows.Forms.RadioButton USDig;
        private System.Windows.Forms.Button buttonServerToggle;
        private MetroFramework.Controls.MetroTextBox textBoxHost;
        private System.Windows.Forms.RadioButton EurDisc;
        private System.Windows.Forms.RadioButton USDisc;
        private System.Windows.Forms.ListView listViewPCKS;
        private System.Windows.Forms.RadioButton JPDig;
    }
}