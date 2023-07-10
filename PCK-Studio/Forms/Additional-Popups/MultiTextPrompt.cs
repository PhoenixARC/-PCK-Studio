using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class MultiTextPrompt : MetroFramework.Forms.MetroForm
    {
        public string[] TextOutput => DialogResult == DialogResult.OK ? PromptTextBox.Lines : Array.Empty<string>();
        public MultiTextPrompt(string[] list = null)
        {
            InitializeComponent();
            PromptTextBox.Lines = list;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
