using PckStudio.ToolboxItems;
using System;
using System.Collections.Generic;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class AddPropertyPrompt : ThemeForm
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

        }
    }
}