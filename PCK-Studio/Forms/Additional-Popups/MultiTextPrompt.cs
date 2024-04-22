using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class MultiTextPrompt : ThemeForm
    {
        public string[] TextOutput => DialogResult == DialogResult.OK ? PromptTextBox.Lines : Array.Empty<string>();
        public MultiTextPrompt(string[] list = null)
        {
            InitializeComponent();
            PromptTextBox.Lines = list;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void okBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
