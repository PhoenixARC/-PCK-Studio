﻿using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
	public partial class ItemSelectionPopUp : MetroFramework.Forms.MetroForm
	{
		public string SelectedItem => DialogResult == DialogResult.OK ? ComboBox.Text : string.Empty;

		public string LabelText
        {
            get => label2.Text;
            set => label2.Text = value;
        }
		public string ButtonText
        {
            get => okBtn.Text;
            set => okBtn.Text = value;
        }

        public ItemSelectionPopUp(params string[] items)
		{
			InitializeComponent();
			ComboBox.Items.AddRange(items);
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			if(ComboBox.SelectedIndex > -1)
				cancelButton_Click(sender, e);
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
