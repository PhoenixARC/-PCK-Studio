﻿using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups.Grf
{
    public partial class AddParameter : ThemeForm
    {
        public string ParameterName => NameTextBox.Text;
        public string ParameterValue => ValueTextBox.Text;
        public AddParameter()
        {
            InitializeComponent();
        }
        public AddParameter(string parameterName, string parameterValue) : this()
        {
            NameTextBox.Text = parameterName;
            ValueTextBox.Text = parameterValue;
        }
        
        public AddParameter(string parameterName, string parameterValue, bool parameterNameBoxEnabled = true) : this(parameterName, parameterValue)
        {
            NameTextBox.Enabled = parameterNameBoxEnabled;
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
            Close();
        }
    }
}
