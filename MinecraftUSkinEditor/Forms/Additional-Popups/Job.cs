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
    public partial class Job : MetroForm
    {
        public Job()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Job_Load(object sender, EventArgs e)
        {
            File.Create(Environment.CurrentDirectory + "\\discordmark");
        }

        private void buttonDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/Byh4hcq25w");
            this.Close();
        }
    }
}
