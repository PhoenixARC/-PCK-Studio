using System;
using MetroFramework.Forms;
using System.Windows.Forms;
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

		}

		private void button2_Click(object sender, EventArgs e)
		{

		}

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Ticks < 0) return;
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
