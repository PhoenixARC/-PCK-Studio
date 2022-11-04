using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class TextPrompt : MetroFramework.Forms.MetroForm
    {
        public string[] TextOutput => DialogResult == DialogResult.OK ? PromptTextBox.Lines : null;
        public TextPrompt()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
