using System;
using MetroFramework.Forms;
using System.Windows.Forms;
using System.Diagnostics;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class FrameEditor : MetroForm
	{
		public int FrameTextureIndex => FrameList.SelectedNode.Index;
		public int FrameTime => (int)FrameTimeUpDown.Value;

		public FrameEditor(ImageList texList)
		{
			InitializeComponent();
			FrameList.ImageList = texList;
			for (int index = 0; index < texList.Images.Count; index++)
			{
				TreeNode frame = new TreeNode($"Frame {index}", index, index);
				FrameList.Nodes.Add(frame);
				Debug.WriteLine(index);
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
            DialogResult = DialogResult.Cancel;
            Close();
		}
	}
}
