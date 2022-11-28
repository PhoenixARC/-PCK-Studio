using System;
using System.Windows.Forms;

// Audio Editor by MattNL

namespace PckStudio.Forms.Additional_Popups.Audio
{
	public partial class creditsEditor : MetroFramework.Forms.MetroForm
	{
		public string Credits => richTextBox1.Text;
		public creditsEditor(string cred)
		{
			InitializeComponent();
			richTextBox1.Text = cred;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
