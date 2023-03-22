using OMI.Formats.Pck;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio
{
    public partial class AdvancedOptions : MetroFramework.Forms.MetroForm
    {
        PckFile currentPCK;

        public AdvancedOptions(PckFile currentPCKIn)
        {
            InitializeComponent();
            currentPCK = currentPCKIn;
            treeMeta.Nodes.Clear();
            treeMeta.Nodes.AddRange(currentPCK.GetPropertyList().Select((s) => new TreeNode(s)).ToArray());
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "All":
                    {
                        foreach (PckFile.FileData file in currentPCK.Files)
                        {
                            file.Properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                        }
                        MessageBox.Show("Data Added to All Entries");
                    }
                    break;
                case "64x64":
                    {
                        foreach (PckFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.Data);
                            if (Path.GetExtension(file.Filename) == ".png" &&
                                Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width)
                            {
                                file.Properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                            }
                        }
                        MessageBox.Show("Data Added to 64x64 Image Entries");
                    }
                    break;
                case "64x32":
                    {
                        foreach (PckFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.Data);
                            if (Path.GetExtension(file.Filename) == ".png" &&
                                Image.FromStream(png).Size.Height == Image.FromStream(png).Size.Width / 2)
                            {
                                file.Properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
                            }
                        }
                        MessageBox.Show("Data Added to 64x32 Image Entries");
                    }
                    break;
                case "PNG Files":
                    {
                        foreach (PckFile.FileData file in currentPCK.Files)
                        {
                            MemoryStream png = new MemoryStream(file.Data);
                            if (Path.GetExtension(file.Filename) == ".png")
                            {
                                file.Properties.Add((entryTypeTextBox.Text, entryDataTextBox.Text));
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
