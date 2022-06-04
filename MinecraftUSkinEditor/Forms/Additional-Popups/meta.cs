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
    public partial class meta : MetroFramework.Forms.MetroForm
    {
        PCKFile currentPCK;

        public meta(PCKFile currentPCKIn)
        {
            InitializeComponent();

            currentPCK = currentPCKIn;

            FormBorderStyle = FormBorderStyle.None;
        }

        private void meta_Load(object sender, EventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            try
            {
                treeView1.Nodes.Clear();
                foreach (string key in currentPCK.meta_data.Keys)
                {
                    treeView1.Nodes.Add(key);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }

        private void treeView1_Click(object sender, EventArgs e)
        {

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PckStudio.MetaADD add = new PckStudio.MetaADD(currentPCK, treeView1);
            add.TopMost = true;
            add.TopLevel = true;
            add.ShowDialog();
            refresh();
            add.Dispose();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("TODO");
                //currentPCK.meta_data.Remove();
                refresh();
            }catch (Exception)
            {

            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
