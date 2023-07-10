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

		public string OKButtonText
		{
			set => OKButton.Text = value;
		}

		public string LabelText
		{
			set => TextLabel.Text = value;
		}

		public TextPrompt() : this(string.Empty, -1)
		{ }

		public TextPrompt(string initialText) : this(initialText, -1)
		{ }

		public TextPrompt(string initialText, int maxTextLength)
		{
			InitializeComponent();
			InputTextBox.Text = initialText;
			InputTextBox.MaxLength = maxTextLength < 0 ? short.MaxValue : maxTextLength;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			if (string.IsNullOrEmpty(InputTextBox.Text))
			{
				MessageBox.Show(this, "Please insert a value in the text box.", "Empty string");
			}
			else DialogResult = DialogResult.OK;
        }

		private void RenamePrompt_Load(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(contextLabel.Text))
			{
				contextLabel.Visible = false;
				Size = new System.Drawing.Size(264, 85);
			}
		}
	}
}
