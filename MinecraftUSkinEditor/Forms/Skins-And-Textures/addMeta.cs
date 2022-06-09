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
        PCKFile.FileData file;

        public addMeta(PCKFile.FileData fileIn)
        {
            InitializeComponent();
            file = fileIn;
            FormBorderStyle = FormBorderStyle.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            file.properties.Add(new ValueTuple<string, string>(textBox1.Text, textBox2.Text ));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
