using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class MultiTextPrompt : MetroFramework.Forms.MetroForm
    {
        public IEnumerable<string> TextOutput => DialogResult == DialogResult.OK ? PromptTextBox.Lines : Array.Empty<string>();
        
        public MultiTextPrompt(IEnumerable<string> textLines)
        {
            InitializeComponent();
            PromptTextBox.Lines = textLines.ToArray();
        }

        public MultiTextPrompt()
            : this(Enumerable.Empty<string>())
        {
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
