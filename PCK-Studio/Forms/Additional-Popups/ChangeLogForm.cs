using PckStudio.Properties;
using PckStudio.ToolboxItems;

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
