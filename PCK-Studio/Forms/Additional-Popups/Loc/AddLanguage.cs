using System;
using System.Windows.Forms;
using PckStudio.Controls;

namespace PckStudio.Forms.Additional_Popups.Loc
{
    public partial class AddLanguage : ImmersiveForm
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
