using System;
using System.Windows.Forms;
using PckStudio.Classes.ToolboxItems;

namespace PckStudio
{
    public partial class CreateTexturePack : ThemeForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string packName => InputTextBox.Text;
		public string packRes => metroComboBox1.Text;

		public CreateTexturePack(string InitialText)
		{
			InitializeComponent();
			InputTextBox.Text = InitialText;
			FormBorderStyle = FormBorderStyle.None;
		}

        private void LockPCKButton_Click(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex < 0) return;
            DialogResult = DialogResult.OK;
        }
    }
}
