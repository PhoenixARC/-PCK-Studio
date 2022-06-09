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
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class MetaADD : MetroForm
    {
        PCKFile currentPCK;

        public MetaADD(PCKFile currentPCKIn)
        {
            InitializeComponent();

            currentPCK = currentPCKIn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentPCK.meta_data.Contains(textBox1.Text))
            {
                MessageBox.Show("This meta tag already exits");
                return;
            }
            currentPCK.meta_data.Add(textBox1.Text);
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }
    }
}
