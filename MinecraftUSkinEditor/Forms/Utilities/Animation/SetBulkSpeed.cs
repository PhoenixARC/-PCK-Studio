using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFramework.Forms;
using System.Windows.Forms;

namespace PckStudio.Forms.Utilities.AnimationEditor
{
	public partial class SetBulkSpeed : MetroForm
	{
		TreeView tv;
		public SetBulkSpeed(TreeView treeView)
		{
			tv = treeView;
			InitializeComponent();
			label3.Text = "Frame Time must be greater than 0.";
		}

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

		private void button1_Click(object sender, EventArgs e)
		{
			if(metroTextBox2.Text == "") {}
            else if(Int16.Parse(metroTextBox2.Text) < 0) {}
			else
			{
				int i = 0;
				foreach (TreeNode nodes in tv.Nodes)
				{
					Tuple<string, string> frameData = nodes.Tag as Tuple<string, string>;
					tv.Nodes.RemoveAt(i);
					TreeNode frameNode = new TreeNode();
					Tuple<string, string> finalFrameData = new Tuple<string, string>(frameData.Item1, metroTextBox2.Text);
					frameNode.Tag = finalFrameData;
					frameNode.Text = "Frame: " + frameData.Item1 + ", Frame Time: " + metroTextBox2.Text;
					tv.Nodes.Insert(i, frameNode);
					i++;
				}

				this.Close();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
