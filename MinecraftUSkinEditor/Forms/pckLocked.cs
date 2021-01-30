using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinecraftUSkinEditor;

namespace minekampf.Forms
{
    public partial class pckLocked : MetroFramework.Forms.MetroForm
    {
        string pass;

        public pckLocked(string pass, bool correct)
        {
            this.pass = pass;

            InitializeComponent();
        }

        private void textBoxPass_Click(object sender, EventArgs e)
        {
        }

        private void buttonUnlocked_Click(object sender, EventArgs e)
        {
            if (textBoxPass.Text == pass)
            {
                FormMain.correct = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect password!");
                textBoxPass.Text = "";
            }
        }

        private void textBoxPass_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBoxPass_Enter(object sender, EventArgs e)
        {
        }

        private void BypassButton_Click(object sender, EventArgs e)
        {
            FormMain.correct = true;
        }
    }
}
