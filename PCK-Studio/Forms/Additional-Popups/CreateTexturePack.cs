using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class CreateTexturePack : MetroForm
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

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if (metroComboBox1.SelectedIndex < 0)
				return;
			DialogResult = DialogResult.OK;
        }
    }
}
