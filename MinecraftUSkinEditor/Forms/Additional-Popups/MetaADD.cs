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
            if (currentPCK.meta_data.ContainsValue(textBox1.Text))
            {
                MessageBox.Show("This metatag already exits");
                return;
            }
            currentPCK.meta_data.Add(currentPCK.meta_data.Count, textBox1.Text);
            Close();
        }
    }
}
