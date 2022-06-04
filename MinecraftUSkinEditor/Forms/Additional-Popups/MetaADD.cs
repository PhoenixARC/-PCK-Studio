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
    public partial class MetaADD : Form
    {
        PCKFile currentPCK;
        TreeView treeView1;

        public MetaADD(PCKFile currentPCKIn, TreeView treeView1In)
        {
            InitializeComponent();

            currentPCK = currentPCKIn;
            treeView1 = treeView1In;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentPCK.meta_data.ContainsKey(textBox1.Text))
            {
                MessageBox.Show("This metatag already exits");
                return;
            }
            if (!currentPCK.meta_data.ContainsValue(currentPCK.meta_data.Count))
                currentPCK.meta_data.Add(textBox1.Text, currentPCK.meta_data.Count);
            Close();
        }
    }
}
