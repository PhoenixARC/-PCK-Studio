using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Controls;

namespace PckStudio
{
    public partial class NumericPrompt : ImmersiveForm
	{
		public decimal SelectedValue => ValueUpDown.Value;
		
		public int SelectedValueAsInt => (int)SelectedValue;

		public string ToolTipText
		{
			get => toolTipLabel.Text;
			set => toolTipLabel.Text = value;
		}

		public decimal ValueStep
		{
			get => ValueUpDown.Increment;
			set => ValueUpDown.Increment = value;
		}

		public int DecimalPlaces
		{
			get => ValueUpDown.DecimalPlaces;
			set => ValueUpDown.DecimalPlaces = value;
		}

        public decimal Minimum
        {
            get => ValueUpDown.Minimum;
			set => ValueUpDown.Minimum = value;
        }

		public decimal Maximum
		{
            get => ValueUpDown.Maximum;
			set => ValueUpDown.Maximum = value;
        }

		private NumericPrompt()
		{
            InitializeComponent();
        }

		public NumericPrompt(int initialValue)
			: this(initialValue, int.MinValue, int.MaxValue)
		{
		}

		public NumericPrompt(decimal initialValue, decimal minimum, decimal maximum)
			: this()
		{
			Minimum = minimum;
			Maximum = maximum;
			ValueUpDown.Value = initialValue;
		}

		public NumericPrompt(int initialValue, int minimum, int maximum)
			: this((decimal)initialValue, minimum, maximum)
		{
		}

		public NumericPrompt(float initialValue, float minimum, float maximum)
			: this((decimal)initialValue, (decimal)minimum, (decimal)maximum)
		{
		}

        private void OKBtn_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.OK;
        }

		private void RenamePrompt_Load(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(toolTipLabel.Text))
			{
				toolTipLabel.Visible = false;
				Size = new System.Drawing.Size(264, 85);
			}
		}
	}
}
