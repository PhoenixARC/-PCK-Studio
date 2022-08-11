using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio.Forms
{
    public partial class Pref : MetroForm
    {
        public Pref()
        {
            InitializeComponent();
        }

        private void Pref_Load(object sender, EventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\settings.ini"))
            {
                string host = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];
                metroTextBox1.Text = host;
                string host1 = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[1];
                metroTextBox2.Text = host1;
            }
        }

        private void buttonDonate_Click(object sender, EventArgs e)
        {
            File.WriteAllText(Environment.CurrentDirectory + "\\settings.ini", metroTextBox1.Text + "\n" + metroTextBox2.Text);
        }
    }
}
