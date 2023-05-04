using System;
using System.Windows.Forms;
using PckStudio.ToolboxItems;

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
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
