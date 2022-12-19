using PckStudio.ToolboxItems;
using System;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class AddMeta : ThemeForm
    {
        public string PropertyName => textBox1.Text;
        public string PropertyValue => textBox2.Text;

        public AddMeta(string name, string value)
        {
            InitializeComponent();
            textBox1.Text = name;
            textBox2.Text = value;
        }

        public AddMeta()
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