using System;
using System.IO;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio.Forms
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
            if (File.Exists(Environment.CurrentDirectory + "\\settings.ini"))
            {

                string[] settings = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                webServerTextBox.Text = settings[0];
                pckWebServerTextBox.Text = settings[1];
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            File.WriteAllText(Environment.CurrentDirectory + "\\settings.ini", webServerTextBox.Text + "\n" + pckWebServerTextBox.Text);
        }
    }
}
