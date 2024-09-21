using PckStudio.ToolboxItems;
using System;

namespace PckStudio
{
    public partial class NumericPrompt : ThemeForm
    {
        public int SelectedValue => (int)ValueUpDown.Value;

        public int Minimum { set => ValueUpDown.Minimum = value; }
        public int Maximum { set => ValueUpDown.Maximum = value; }

        public NumericPrompt(int initialValue)
            : this(initialValue, int.MinValue, int.MaxValue)
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

        }

        private void RenamePrompt_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ContextLabel.Text))
            {
                ContextLabel.Visible = false;
                Size = new System.Drawing.Size(264, 85);
            }
        }
    }
}
