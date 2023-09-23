﻿using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class CreateTexturePackPrompt : MetroForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public bool CreateSkinsPck => createSkinsPckCheckBox.Checked;
		public string PackName => InputTextBox.Text;
		public string PackRes => resolutionComboBox.Text;

		public CreateTexturePackPrompt()
		{
			InitializeComponent();
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if (resolutionComboBox.SelectedIndex < 0)
				return;
			DialogResult = DialogResult.OK;
        }
    }
}
