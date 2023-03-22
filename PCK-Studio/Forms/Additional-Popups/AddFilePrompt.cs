using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class AddFilePrompt : MetroForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string filepath => InputTextBox.Text;
		public int filetype => FileTypeComboBox.SelectedIndex;

		public AddFilePrompt(string InitialText) : this(InitialText, -1)
		{ }

		public AddFilePrompt(string InitialText, int maxChar)
		{
			InitializeComponent();
			InputTextBox.Text = InitialText;
			InputTextBox.MaxLength = maxChar < 0 ? short.MaxValue : maxChar;
			FormBorderStyle = FormBorderStyle.None;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if(FileTypeComboBox.SelectedIndex > -1) DialogResult = DialogResult.OK;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.KeyCode == Keys.Enter)
				OKBtn_Click(sender, e);
        }
    }
}
