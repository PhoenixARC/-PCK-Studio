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
		public int FrameTextureIndex => (int)FrameIndexUpDown.Value;
		public int FrameTime => (int)FrameTimeUpDown.Value;

		public FrameEditor(int indexLimit)
		{
			InitializeComponent();
			label3.Text = "Frame must be within 0 and " + indexLimit + ".";
            FrameIndexUpDown.Maximum = indexLimit;
			FrameTimeUpDown.Minimum = 1;
		}
		public FrameEditor(int frameTime, int frameTextureIndex, int indexLimit) : this(indexLimit)
		{
            FrameIndexUpDown.Value = frameTextureIndex;
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
