using System;
using System.Windows.Forms;
using PckStudio.Classes.ToolboxItems;

namespace PckStudio
{
	public partial class RenamePrompt : ThemeForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string NewText => InputTextBox.Text;

		public RenamePrompt(string InitialText) : this(InitialText, -1)
		{ }

		public RenamePrompt(string InitialText, int maxChar)
		{
			InitializeComponent();
			InputTextBox.Text = InitialText;
			InputTextBox.MaxLength = maxChar < 0 ? short.MaxValue : maxChar;
			FormBorderStyle = FormBorderStyle.None;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.KeyCode == Keys.Enter)
				OKBtn_Click(sender, e);
        }
    }
}
