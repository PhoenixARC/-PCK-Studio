using System;
using System.Windows.Forms;
using PckStudio.ToolboxItems;

namespace PckStudio
{
    public partial class CreateTexturePack : ThemeForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string PackName => InputTextBox.Text;
		public string PackRes => metroComboBox1.Text;

		public CreateTexturePack()
		{
			InitializeComponent();
		}

        private void LockPCKButton_Click(object sender, EventArgs e)
        {
			if (metroComboBox1.SelectedIndex < 0)
				return;
			DialogResult = DialogResult.OK;
        }
    }
}
