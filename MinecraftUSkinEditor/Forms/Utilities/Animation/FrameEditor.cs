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
	public partial class FrameEditor : MetroForm
	{
		bool newF;
		TreeView tv;
		TreeNode node;
		int limit;
		Tuple<string, string> data = new Tuple<string, string>("","");
		public FrameEditor(TreeView treeView, Tuple<string, string> frameData, int frameLimit, bool newFrame, TreeNode nodeToEdit)
		{
            limit = frameLimit;
			node = nodeToEdit;
			data = frameData;
			tv = treeView;
			newF = newFrame;
			InitializeComponent();
			label3.Text = "Frame must be within 0 and " + frameLimit + ".\nFrame Time must be greater than 0.";
			metroTextBox1.Text = data.Item1;
			metroTextBox2.Text = data.Item2;
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
			if(metroTextBox1.Text == "" || metroTextBox2.Text == "") {}
            else if(Int16.Parse(metroTextBox1.Text) > limit || Int16.Parse(metroTextBox1.Text) < 0 || Int16.Parse(metroTextBox2.Text) < 0) {}
			else
			{
				if(newF)
				{
					TreeNode frameNode = new TreeNode();
					Tuple<string, string> finalFrameData = new Tuple<string, string>(metroTextBox1.Text, metroTextBox2.Text);
					frameNode.Tag = finalFrameData;
					frameNode.Text = "Frame: " + metroTextBox1.Text + ", Frame Time: " + metroTextBox2.Text;
					tv.Nodes.Add(frameNode);
				}
				else if(!String.IsNullOrEmpty(data.Item1))
				{
					Tuple<string, string> finalFrameData = new Tuple<string, string>(metroTextBox1.Text, metroTextBox2.Text);
					node.Tag = finalFrameData;
					node.Text = "Frame: " + metroTextBox1.Text + ", Frame Time: " + metroTextBox2.Text;
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
