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
		TreeNode node;

		public renameLoc(TreeNode nodeIn)
		{
			InitializeComponent();
			node = nodeIn;
			textBox1.Text = nodeIn.Text;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			node.Name = textBox1.Text;
			node.Text = textBox1.Text;
			this.Close();
		}

		private void addCategory_Load(object sender, EventArgs e)
		{

		}

	}
}
