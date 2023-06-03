using OMI.Formats.Pck;
using OMI.Workers.Pck;
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
        public bool littleEndian;

        public AdvancedOptions(PckFile currentPCKIn)
        {
            InitializeComponent();
            currentPCK = currentPCKIn;
            treeMeta.Nodes.Clear();
            treeMeta.Nodes.AddRange(currentPCK.GetPropertyList().Select((s) => new TreeNode(s)).ToArray());
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        applyBulkProperties();
                        DialogResult = DialogResult.OK;
                    }
                    break;
                case > 0 and <= 13:
                    {
                        applyBulkProperties((PckFile.FileData.FileType)(comboBox1.SelectedIndex - 1));
                        DialogResult = DialogResult.OK;
                    }
                    break;
                default:
                    MessageBox.Show("Please select a filetype before applying");
                    break;
            }
        }

        private void applyBulkProperties()
		{
            foreach (PckFile.FileData file in currentPCK.Files)
            {
                if (file.Filetype == PckFile.FileData.FileType.TexturePackInfoFile ||
                    file.Filetype == PckFile.FileData.FileType.SkinDataFile)
                {
                    try
                    {
                        var reader = new PckFileReader(littleEndian
                            ? OMI.Endianness.LittleEndian
                            : OMI.Endianness.BigEndian);
                        PckFile SubPCK = reader.FromStream(new MemoryStream(file.Data));
                        foreach (PckFile.FileData SubFile in SubPCK.Files)
                        {
                            SubFile.Properties.Add(entryTypeTextBox.Text, entryDataTextBox.Text);
                        }
                        var writer = new PckFileWriter(SubPCK, littleEndian
                            ? OMI.Endianness.LittleEndian
                            : OMI.Endianness.BigEndian);
                        var stream = new MemoryStream();
                        writer.WriteToStream(stream);
                        file.SetData(stream.ToArray());
                        stream.Dispose();
                    }
                    catch (OverflowException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

                file.Properties.Add(entryTypeTextBox.Text, entryDataTextBox.Text);
            }
            MessageBox.Show("Data added to all entries");
        }

        private void applyBulkProperties(PckFile.FileData.FileType filetype)
		{
            foreach (PckFile.FileData file in currentPCK.Files)
            {
                if (file.Filetype == PckFile.FileData.FileType.TexturePackInfoFile ||
                    file.Filetype == PckFile.FileData.FileType.SkinDataFile)
                {
                    try
                    {
                        var reader = new PckFileReader(littleEndian
                            ? OMI.Endianness.LittleEndian
                            : OMI.Endianness.BigEndian);
                        PckFile SubPCK = reader.FromStream(new MemoryStream(file.Data));
                        foreach (PckFile.FileData SubFile in SubPCK.Files)
                        {
                            if (SubFile.Filetype == filetype)
                            {
                                SubFile.Properties.Add(entryTypeTextBox.Text, entryDataTextBox.Text);
                            }
                        }
                        var writer = new PckFileWriter(SubPCK, littleEndian
                            ? OMI.Endianness.LittleEndian
                            : OMI.Endianness.BigEndian);
                        var stream = new MemoryStream();
                        writer.WriteToStream(stream);
                        file.SetData(stream.ToArray());
                        stream.Dispose();
                    }
                    catch (OverflowException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

                if (file.Filetype == filetype)
                {
                    file.Properties.Add(entryTypeTextBox.Text, entryDataTextBox.Text);
                }
            }
            MessageBox.Show($"Data Added to {filetype} File Entries");
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            entryTypeTextBox.Text = treeMeta.SelectedNode.Text;
        }
    }
}
