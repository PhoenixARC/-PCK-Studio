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
	public partial class rename : MetroFramework.Forms.MetroForm
	{
		String oldName;
		String newName;
		TreeNode node;

		public rename(TreeNode nodeIn)
		{
			Console.WriteLine("Full Node Path - " + nodeIn.FullPath.Replace("\\", "/"));
			string[] parents = nodeIn.FullPath.Split(nodeIn.TreeView.PathSeparator.ToCharArray());
			foreach (string parent in parents)
			{
				Console.WriteLine(" - " + parent);
			}
			InitializeComponent();
			node = nodeIn;
			oldName = nodeIn.Text;
			textBox1.Text = nodeIn.Text;
			FormBorderStyle = FormBorderStyle.None;
		}

		private void fixDirectoryNameForFiles(TreeNode dirN)
		{
			foreach (TreeNode n in dirN.Nodes)
			{
				if (n.Tag == null)
				{
					fixDirectoryNameForFiles(n);
					continue;
				}
				PCK.MineFile mf = (PCK.MineFile)n.Tag;
				string fullNew = mf.name.Replace(oldName + "/", newName + "/");
				Console.WriteLine("Full old - " + mf.name + " - Old: " + oldName + " - New: " + newName + " - " + fullNew);
				mf.name = fullNew;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			newName = textBox1.Text;
			node.Name = textBox1.Text;
			if (node.Tag == null) fixDirectoryNameForFiles(node);
			else
			{
				PCK.MineFile mf = (PCK.MineFile)node.Tag;
				string path = Path.GetDirectoryName(node.FullPath.Replace("\\", "/"));
				string fullNew = path + "/" + newName;
				mf.name = fullNew;
				Console.WriteLine("Full old - " + mf.name + " - Old: " + oldName + " - New: " + fullNew);
			}
			this.Close();
		}

		private void addCategory_Load(object sender, EventArgs e)
		{

		}

	}
}
