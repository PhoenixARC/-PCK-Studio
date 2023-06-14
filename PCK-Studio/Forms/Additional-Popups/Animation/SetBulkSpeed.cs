using System;
using MetroFramework.Forms;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class SetBulkSpeed : MetroForm
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
