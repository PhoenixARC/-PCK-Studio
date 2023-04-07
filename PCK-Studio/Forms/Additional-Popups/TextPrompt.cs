using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class TextPrompt : ThemeForm
    {
        public string[] TextOutput => DialogResult == DialogResult.OK ? PromptTextBox.Lines : Array.Empty<string>();
        public TextPrompt(string[] list = null)
        {
            InitializeComponent();
            PromptTextBox.Lines = list;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
