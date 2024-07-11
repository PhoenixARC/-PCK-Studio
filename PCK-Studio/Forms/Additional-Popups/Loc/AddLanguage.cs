using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups.Loc
{
    public partial class AddLanguage : MetroFramework.Forms.MetroForm
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
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
