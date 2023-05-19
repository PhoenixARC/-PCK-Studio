using System;
using System.Windows.Forms;

namespace PckStudio.Forms.Additional_Popups
{
    public partial class AddPropertyPrompt : MetroFramework.Forms.MetroForm
    {
        public string PropertyName => textBox1.Text;
        public string PropertyValue => textBox2.Text;

        public AddPropertyPrompt(string name, string value)
        {
            InitializeComponent();
            textBox1.Text = name;
            textBox2.Text = value;
        }

        public AddPropertyPrompt()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}