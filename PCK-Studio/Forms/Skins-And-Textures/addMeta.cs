using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class addMeta : MetroFramework.Forms.MetroForm
    {

        public string PropertyName => textBox1.Text;
        public string PropertyValue => textBox2.Text;

        public addMeta(string name, string value)
        {
            InitializeComponent();
            textBox1.Text = name;
            textBox2.Text = value;
        }

        public addMeta()
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