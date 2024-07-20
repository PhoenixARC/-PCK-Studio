using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups.Loc
{
    public partial class AddLanguage : ThemeForm
    {
        public string SelectedLanguage => LanguageComboBox.Text;
        public AddLanguage(string[] avalibleLanguages)
        {
            InitializeComponent();
            LanguageComboBox.Items.AddRange(avalibleLanguages);
            LanguageComboBox.SelectedIndex = 0;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {

        }

        private void AddLanguage_Load(object sender, EventArgs e)
        {

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
