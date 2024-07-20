using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;
using OMI.Formats.GameRule;

namespace PckStudio.Forms.Additional_Popups.Grf
{
    public partial class AddParameter : ThemeForm
    {
        public string ParameterName => NameTextBox.Text;
        public string ParameterValue => ValueTextBox.Text;

        
        public AddParameter()
        {
            InitializeComponent();
            NameTextBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            NameTextBox.AutoCompleteCustomSource.AddRange(GameRuleFile.GameRule.ValidParameters);
        }
        
        public AddParameter(string parameterName, string parameterValue, bool isKeyReadonly = true) : this()
        {
            NameTextBox.Text = parameterName;
            ValueTextBox.Text = parameterValue;
            NameTextBox.Enabled = isKeyReadonly;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ParameterName) || string.IsNullOrWhiteSpace(ParameterValue))
            {
                MessageBox.Show(this, "Name or value can't be empty.", "Empty value", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
