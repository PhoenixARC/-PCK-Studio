using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;
using OMI.Formats.GameRule;
using PckStudio.Properties;

namespace PckStudio.Forms.Additional_Popups.Grf
{
    public partial class AddParameter : ThemeForm
    {
        public string ParameterName => NameTextBox.Text;
        public string ParameterValue => ValueTextBox.Text;

        private bool _useComboBox
        {
            get
            {
                return availableComboBox.Visible && !NameTextBox.Visible;
            }
            set
            {
                NameTextBox.Visible = !value;
                availableComboBox.Visible = value;
            }
        }
        
        public AddParameter()
        {
            InitializeComponent();
            availableComboBox.Items.Clear();
            availableComboBox.Items.AddRange(GameRuleFile.GameRule.ValidParameters);
            _useComboBox = Settings.Default.UseComboBoxForGRFParameter;
        }
        
        public AddParameter(string parameterName, string parameterValue, bool isKeyReadonly = true) : this()
        {
            NameTextBox.Text = parameterName;
            ValueTextBox.Text = parameterValue;
            NameTextBox.Enabled = isKeyReadonly;
            availableComboBox.Enabled = isKeyReadonly;
            availableComboBox.SelectedItem = parameterName;
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
                MessageBox.Show("Name and Value need valid values");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void availableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NameTextBox.Text = availableComboBox.SelectedItem.ToString();
        }
    }
}
