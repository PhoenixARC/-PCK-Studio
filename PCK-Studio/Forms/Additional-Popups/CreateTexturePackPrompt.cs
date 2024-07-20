using System;
using System.Windows.Forms;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class CreateTexturePackPrompt : ThemeForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public bool CreateSkinsPck => createSkinsPckCheckBox.Checked;
		public string PackName => InputTextBox.Text;
		public string PackRes => metroComboBox1.Text;

		public CreateTexturePackPrompt()
		{
			InitializeComponent();
		}

        private void LockPCKButton_Click(object sender, EventArgs e)
        {
			if (metroComboBox1.SelectedIndex < 0)
				return;
			DialogResult = DialogResult.OK;
        }

        private void createSkinsPckCheckBox_CheckedChanged(object sender)
        {

        }
    }
}
