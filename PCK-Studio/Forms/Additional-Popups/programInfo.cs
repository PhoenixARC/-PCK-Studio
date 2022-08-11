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
    public partial class programInfo : MetroForm
    {
        int count = 0;
        public programInfo()
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
