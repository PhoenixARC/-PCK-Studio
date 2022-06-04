using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class AdvancedOptions : MetroFramework.Forms.MetroForm
    {
        PCKFile.FileData mf;
        PCKFile currentPCK;

        public AdvancedOptions(PCKFile currentPCKIn)
        {
            InitializeComponent();
            currentPCK = currentPCKIn;

            FormBorderStyle = FormBorderStyle.None;
        }

        private void AdvancedOptions_Load(object sender, EventArgs e)
        {

        }

        private void addPresetToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {

        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PckStudio.addMetaAdvanced add = new PckStudio.addMetaAdvanced(treeMeta);
            add.ShowDialog();
            add.Dispose();
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeMeta.SelectedNode.Remove();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "All")
                {
                    foreach (TreeNode node in treeMeta.Nodes)
                    {
                        foreach (PCKFile.FileData mf in currentPCK.file_entries)
                        {
                            mf.properties.Add(new Tuple<string, string>(node.Text, node.Tag.ToString()));
                        }
                    }
                    MessageBox.Show("Data Added to All Entries");
                }
                else if (comboBox1.Text == "64x64")
                {
                    int count = treeMeta.Nodes.Count;
                    int i = 0;
                    do
                    {
                        foreach (PCKFile.FileData mf in currentPCK.file_entries)
                        {
                            MemoryStream png = new MemoryStream(mf.data);
                            if (Path.GetExtension(mf.name) == ".png")
                            {
                                if (Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width)
                                {
                                    mf.properties.Add(new Tuple<string, string>(treeMeta.Nodes[i].Text, treeMeta.Nodes[i].Tag.ToString()));
                                }
                            }
                        }
                        i += 1;
                        count -= 1;
                    } while (count != 0);
                    MessageBox.Show("Data Added to 64x64 Image Entries");
                }
                else if (comboBox1.Text == "64x32")
                {
                    int count = treeMeta.Nodes.Count;
                    int i = 0;

                    do
                    {
                        foreach (PCKFile.FileData mf in currentPCK.file_entries)
                        {
                            MemoryStream png = new MemoryStream(mf.data);
                            if (Path.GetExtension(mf.name) == ".png")
                            {
                                if (Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width / 2)
                                {
                                    mf.properties.Add(new Tuple<string, string>(treeMeta.Nodes[i].Text, treeMeta.Nodes[i].Tag.ToString() ));
                                }
                            }
                        }
                        i += 1;
                        count -= 1;
                    } while (count != 0);
                    MessageBox.Show("Data Added to  64x32 Image Entries");
                }
                else if (comboBox1.Text == "PNG Files")
                {
                    int count = treeMeta.Nodes.Count;
                    int i = 0;

                    do
                    {
                        foreach (PCKFile.FileData mf in currentPCK.file_entries)
                        {
                            if (Path.GetExtension(mf.name) == ".png")
                            {
                                mf.properties.Add(new Tuple<string, string>(treeMeta.Nodes[i].Text, treeMeta.Nodes[i].Tag.ToString() ));
                            }
                        }
                        i += 1;
                        count -= 1;
                    } while (count != 0);
                    MessageBox.Show("Data Added to All PNG Image Entries");
                }
                else
                {
                    MessageBox.Show("Please Select an Application Argument");
                }
            }catch (Exception)
            {
                MessageBox.Show("A Probelm Occured..");
            }
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = treeMeta.SelectedNode.Text;
            textBox1.Text = treeMeta.SelectedNode.Tag.ToString();
        }
    }
}
