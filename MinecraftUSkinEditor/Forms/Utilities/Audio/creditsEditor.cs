using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Audio Editor by MattNL

namespace PckStudio
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
