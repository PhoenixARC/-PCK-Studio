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
    public partial class EntryEditor : Form
    {
        public EntryEditor(Dictionary<int,string> types, PCKFile.FileData file)
        {
            InitializeComponent();
            this.types = types;
            this.file = file;
        }

        Dictionary<int, string> types;
        PCKFile.FileData file;

        private void renameProperly()
        {

        }

        private void EntryEditor_Load(object sender, EventArgs e)
        {
            foreach(int type in types.Keys)
                comboBox1.Items.Add(types[type]);

            foreach (var entry in file.properties)
            {
                TreeNode meta = new TreeNode(entry.Key);

                meta.Tag = entry;
                treeView1.Nodes.Add(meta);
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object[] strings = (object[])e.Node.Tag;
            comboBox1.Text = (string)strings[0];
            textBox1.Text = (string)strings[1];
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode != null)
            {
                object[] strings = (object[])treeView1.SelectedNode.Tag;
                strings[0] = comboBox1.Text;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                object[] strings = (object[])treeView1.SelectedNode.Tag;
                strings[1] = textBox1.Text;
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file.properties.Add("Replace me", "Or it won't save");
            TreeNode t = new TreeNode("temp name");
            treeView1.Nodes.Add(t);
            renameProperly();
            treeView1.SelectedNode = t;
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var temp = (string)treeView1.SelectedNode.Tag;
                file.properties.Remove(temp);
                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }
        }
    }
}
