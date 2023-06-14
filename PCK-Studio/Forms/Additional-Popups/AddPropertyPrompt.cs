using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class AddPropertyPrompt : MetroFramework.Forms.MetroForm
    {
        public KeyValuePair<string, string> Property => new KeyValuePair<string, string>(keyTextBox.Text, valueTextBox.Text);

        public AddPropertyPrompt(KeyValuePair<string, string> property)
            : this(property.Key, property.Value)
        {

        }

        public AddPropertyPrompt(string name, string value)
        {
            InitializeComponent();
            keyTextBox.Text = name;
            valueTextBox.Text = value;
        }

        public AddPropertyPrompt()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}