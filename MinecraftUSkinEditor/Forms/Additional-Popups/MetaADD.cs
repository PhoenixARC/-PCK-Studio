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
    public partial class MetaADD : Form
    {
        PCK currentPCK;
        TreeView treeView1;

        public MetaADD(PCK currentPCKIn, TreeView treeView1In)
        {
            InitializeComponent();

            currentPCK = currentPCKIn;
            treeView1 = treeView1In;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                currentPCK.typeCodes.Add(textBox1.Text, treeView1.Nodes.Count);
                currentPCK.types.Add(treeView1.Nodes.Count, textBox1.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("This metatag already exits");
            }
            this.Close();
        }
    }
}
