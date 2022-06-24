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
		public string Credits { get; private set; }
		public creditsEditor(string cred)
		{
			InitializeComponent();
			Credits = cred;
			richTextBox1.Text = cred;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Credits = richTextBox1.Text;
			Close();
		}

		private void metroLabel1_Click(object sender, EventArgs e)
		{

		}
	}
}
