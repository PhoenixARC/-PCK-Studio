using System;
using PckStudio.ToolboxItems;
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
    }
}
