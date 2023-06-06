using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class NumericPrompt : MetroForm
	{
		/// <summary>
		/// Text entered <c>only access when DialogResult == DialogResult.OK</c>
		/// </summary>
		public int NewValue => (int)ValueUpDown.Value;

		public NumericPrompt(int InitialValue)
		{
			InitializeComponent();
			ValueUpDown.Value = InitialValue;
		}
        private void OKBtn_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
        }

		private void RenamePrompt_Load(object sender, EventArgs e)
		{
			if(String.IsNullOrEmpty(ContextLabel.Text))
			{
				ContextLabel.Visible = false;
				Size = new System.Drawing.Size(264, 85);
			}
		}
	}
}
