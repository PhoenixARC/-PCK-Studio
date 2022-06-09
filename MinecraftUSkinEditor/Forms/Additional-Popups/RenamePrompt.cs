using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class RenamePrompt : MetroForm
	{
		public string NewText => InputTextBox.Text;

		public RenamePrompt(TreeNode nodeIn)
		{
			InitializeComponent();
			InputTextBox.Text = nodeIn.Text;
			FormBorderStyle = FormBorderStyle.None;
		}
		public RenamePrompt(string InitialText)
		{
			InitializeComponent();
			InputTextBox.Text = InitialText;
			FormBorderStyle = FormBorderStyle.None;
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
        }
    }
}
