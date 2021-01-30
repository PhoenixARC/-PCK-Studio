using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftUSkinEditor
{
    public partial class addMeta : MetroFramework.Forms.MetroForm
    {
        PCK currentPCK;
        PCK.MineFile file;

        public addMeta(PCK.MineFile fileIn, PCK currentPCKIn)
        {
            InitializeComponent();
            file = fileIn;
            currentPCK = currentPCKIn;
            FormBorderStyle = FormBorderStyle.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object[] obj = { textBox1.Text, textBox2.Text };
            file.entries.Add(obj);
            this.Close();
        }

        private void addMeta_Load(object sender, EventArgs e)
        {

        }
    }
}
