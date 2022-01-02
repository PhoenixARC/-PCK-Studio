using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio.Forms
{
    public partial class goodbye : MetroFramework.Forms.MetroForm
    {
        public goodbye()
        {
            InitializeComponent();
        }

        string BokuNoPico = "https://mega.nz/folder/8d8AkKQA#6sps-PKhtYJvmIJ5ujtshA";

        private void buttonDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UConMxIxpuhxtk6s2oGeulLw");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void goodbye_Load(object sender, EventArgs e)
        {
            System.IO.File.Create(Environment.CurrentDirectory + "\\goodbyemark");
        }
    }
}
