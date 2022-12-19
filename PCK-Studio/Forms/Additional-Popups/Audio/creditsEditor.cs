using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

// Audio Editor by MattNL

namespace PckStudio.Forms.Additional_Popups.Audio
{
	public partial class CreditsEditor : ThemeForm
	{
		public string Credits => richTextBox1.Text;
		public CreditsEditor(string cred)
		{
			InitializeComponent();
			richTextBox1.Text = cred;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

        private void creditsEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
