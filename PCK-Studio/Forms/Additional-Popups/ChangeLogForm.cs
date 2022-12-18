using System;
using PckStudio.Classes.ToolboxItems;
using PckStudio.Properties;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class ChangeLogForm : ThemeForm
    {
        public ChangeLogForm()
        {
            InitializeComponent();
            ChangelogRichTextBox.Text = Resources.CHANGELOG;
        }

        private void ChangeLogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
