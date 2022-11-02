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

using PckStudio.Forms.Editor;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class FrameEditor : MetroForm
	{
		public int FrameTextureIndex => FrameList.SelectedNode.Index;
		public int FrameTime => (int)FrameTimeUpDown.Value;

		public FrameEditor(ImageList texList)
		{
			InitializeComponent();
			label3.Text = "Select a frame and frame time:";
			FrameList.ImageList = texList;

			int index = 0;
			foreach (Image frameTex in texList.Images)
			{
				TreeNode frame = new TreeNode($"Frame {index}", index, index);
				FrameList.Nodes.Add(frame);
				Console.WriteLine(index);
				index++;
			}
		}

		public FrameEditor(int frameTime, int frameTextureIndex, ImageList texList) : this(texList)
		{
			FrameList.SelectedNode = FrameList.Nodes[frameTextureIndex];
			FrameList.SelectedNode.EnsureVisible();
			FrameTimeUpDown.Value = frameTime;
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
