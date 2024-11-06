﻿using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class SetBulkSpeed : ThemeForm
	{
		public int Ticks => (int)TimeUpDown.Value;
		
		public SetBulkSpeed()
		{
			InitializeComponent();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
