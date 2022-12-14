﻿using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class MipMapPrompt : Form
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public int Levels => (int)numericUpDown1.Value;

		public MipMapPrompt()
		{
			InitializeComponent();
			FormBorderStyle = FormBorderStyle.None;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {

        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.KeyCode == Keys.Enter)
				OKBtn_Click(sender, e);
        }

        private void GenerateOkButton(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
