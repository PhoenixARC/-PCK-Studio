using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

// Audio Editor by MattNL

namespace PckStudio.Forms.Additional_Popups.Audio
{
	public partial class AddCategory : ThemeForm
	{
		private string _category = string.Empty;
		public string Category => _category;
		public AddCategory(string[] avalibleCategories)
		{
			InitializeComponent();
			comboBox1.Items.AddRange(avalibleCategories);
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void cancelButton_Click(object sender, EventArgs e)
		{

		}

        private void addCategory_Load(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            _category = comboBox1.Text;
            DialogResult = DialogResult.OK;
            if (comboBox1.SelectedIndex > -1) Close();
        }
    }
}
