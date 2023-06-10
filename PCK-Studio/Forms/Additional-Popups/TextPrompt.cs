using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class TextPrompt : MetroForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public string NewText => InputTextBox.Text;

		public TextPrompt(string InitialText) : this(InitialText, -1)
		{ }

		public TextPrompt(string InitialText, int maxChar)
		{
			InitializeComponent();
			InputTextBox.Text = InitialText;
			InputTextBox.MaxLength = maxChar < 0 ? short.MaxValue : maxChar;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if (String.IsNullOrEmpty(InputTextBox.Text))
			{
				MessageBox.Show(this, "Please insert a value in the text box.", "Empty string");
			}
			else DialogResult = DialogResult.OK;
        }

		private void RenamePrompt_Load(object sender, EventArgs e)
		{
			if(String.IsNullOrEmpty(contextLabel.Text))
			{
				contextLabel.Visible = false;
				Size = new System.Drawing.Size(264, 85);
			}
		}
	}
}
