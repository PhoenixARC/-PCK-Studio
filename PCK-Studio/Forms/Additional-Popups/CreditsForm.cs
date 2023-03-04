using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace PckStudio
{
    public partial class CreditsForm : MetroForm
    {
        int count = 0;
        public CreditsForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (++count == 5)
            {
                MessageBox.Show("🌸Miku🌸 was here!");
                count = 0;
            }
        }
	}
}
