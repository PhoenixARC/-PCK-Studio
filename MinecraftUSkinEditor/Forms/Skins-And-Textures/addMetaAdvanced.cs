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
    public partial class addMetaAdvanced : MetroFramework.Forms.MetroForm
    {
        TreeView treeMeta;

        public addMetaAdvanced(TreeView treeMetaIn)
        {
            InitializeComponent();

            treeMeta = treeMetaIn;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode add = new TreeNode();
            add.Text = textBox1.Text;
            add.Tag = textBox2.Text;

            treeMeta.Nodes.Add(add);
            Close();
        }

        private void addMetaAdvanced_Load(object sender, EventArgs e)
        {

        }
    }
}
