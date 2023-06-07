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
		public int SelectedValue => (int)ValueUpDown.Value;

		public int Minimum { set => ValueUpDown.Minimum = value; }
		public int Maximum { set => ValueUpDown.Maximum = value; }

		public NumericPrompt(int initialValue)
			: this(initialValue, 0, 100)
		{

		}

		public NumericPrompt(int initialValue, int minimum, int maximum)
		{
			InitializeComponent();
			ValueUpDown.Value = initialValue;
			Minimum = minimum;
			Maximum = maximum;
		}
        private void OKBtn_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
        }

		private void RenamePrompt_Load(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(ContextLabel.Text))
			{
				ContextLabel.Visible = false;
				Size = new System.Drawing.Size(264, 85);
			}
		}
	}
}
