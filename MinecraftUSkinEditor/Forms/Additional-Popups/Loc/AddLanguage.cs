using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
