using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio
{
	public partial class renameLoc : MetroFramework.Forms.MetroForm
	{
		public string NewText = string.Empty;

		public renameLoc(string initialText)
		{
			InitializeComponent();
			textBox1.Text = initialText;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			NewText = textBox1.Text;
			DialogResult = DialogResult.OK;
			Close();
		}
    }
}
