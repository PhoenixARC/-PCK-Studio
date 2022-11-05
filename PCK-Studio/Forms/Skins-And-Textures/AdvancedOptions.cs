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
        PCKFile currentPCK;

        public AdvancedOptions(PCKFile currentPCKIn)
        {
            InitializeComponent();
            currentPCK = currentPCKIn;
            treeMeta.Nodes.Clear();
            treeMeta.Nodes.AddRange(currentPCK.GatherPropertiesList().Select((s) => new TreeNode(s)).ToArray());
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "All":
                    {
                        foreach (PCKFile.FileData file in currentPCK.Files)
                        {
                            file.properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                        }
                        MessageBox.Show("Data Added to All Entries");
                    }
                    break;
                case "64x64":
                    {
                        foreach (PCKFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.data);
                            if (Path.GetExtension(file.filepath) == ".png" &&
                                Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width)
                            {
                                file.properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                            }
                        }
                        MessageBox.Show("Data Added to 64x64 Image Entries");
                    }
                    break;
                case "64x32":
                    {
                        foreach (PCKFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.data);
                            if (Path.GetExtension(file.filepath) == ".png" &&
                                Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width / 2)
                            {
                                file.properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                            }
                        }
                        MessageBox.Show("Data Added to 64x32 Image Entries");
                    }
                    break;
                case "PNG Files":
                    {
                        foreach (PCKFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.data);
                            if (Path.GetExtension(file.filepath) == ".png")
                            {
                                file.properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                            }
                        }
                        MessageBox.Show("Data Added to All PNG Image Entries");
                    }
                    break;
                default:
                    MessageBox.Show("Please Select an Application Argument");
                    break;
            }
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            entryTypeTextBox.Text = treeMeta.SelectedNode.Text;
        }
    }
}
