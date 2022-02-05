using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio
{
	public partial class rename : MetroFramework.Forms.MetroForm
	{
		PCK.MineFile mf;
		public rename(PCK.MineFile mfIn)
		{
			InitializeComponent();
			mf = mfIn;
			textBox1.Text = mf.name;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			mf.name = textBox1.Text;
			this.Close();
		}

		private void addCategory_Load(object sender, EventArgs e)
		{

		}

	}
}
