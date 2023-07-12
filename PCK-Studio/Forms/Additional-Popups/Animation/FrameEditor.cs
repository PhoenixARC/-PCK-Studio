﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PckStudio.ToolboxItems;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class FrameEditor : ThemeForm
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
			}
		}

		public FrameEditor(int frameTime, int frameTextureIndex, ImageList texList) : this(texList)
		{
			FrameList.SelectedNode = FrameList.Nodes[frameTextureIndex];
			FrameList.SelectedNode.EnsureVisible();
			FrameTimeUpDown.Value = frameTime;
		}

        private void FrameEditor_Load(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

		private void CancelButton_Click(object sender, EventArgs e)
		{
            DialogResult = DialogResult.Cancel;
            Close();
		}
	}
}
