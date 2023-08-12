using OMI;
using OMI.Formats.Pck;
using OMI.Workers.Pck;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio.Popups
{
    public partial class AdvancedOptions : MetroFramework.Forms.MetroForm
    {
        public bool IsLittleEndian
        {
            set
            {
                _endianness = value ? Endianness.LittleEndian : Endianness.BigEndian;
            }
        }
        private PckFile _pckFile;
        private Endianness _endianness;

        public AdvancedOptions(PckFile pckFile)
        {
            InitializeComponent();
            _pckFile = pckFile;
            propertyTreeview.Nodes.Clear();
            propertyTreeview.Nodes.AddRange(_pckFile.GetPropertyList().Select(s => new TreeNode(s)).ToArray());
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (fileTypeComboBox.SelectedIndex >= 0 && fileTypeComboBox.SelectedIndex <= 13)
            {
                applyBulkProperties(_pckFile.Files, fileTypeComboBox.SelectedIndex - 1);
                DialogResult = DialogResult.OK;
                return;
            }
            MessageBox.Show("Please select a filetype before applying");
        }

        private void applyBulkProperties(FileCollection files, int index)
		{
            foreach (PckFile.FileData file in files)
            {
                if (file.Filetype == PckFile.FileData.FileType.TexturePackInfoFile ||
                file.Filetype == PckFile.FileData.FileType.SkinDataFile)
                {
                    try
                    {
                        var reader = new PckFileReader(_endianness);
                        using var ms = new MemoryStream(file.Data);
                        PckFile subPCK = reader.FromStream(ms);
                        applyBulkProperties(subPCK.Files, index);
                        var writer = new PckFileWriter(subPCK, _endianness);
                        using (var stream = new MemoryStream())
                        {
                            writer.WriteToStream(stream);
                            file.SetData(stream.ToArray());
                        }
                    }
                    catch (OverflowException ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                if (index == -1 || (Enum.IsDefined(typeof(PckFile.FileData.FileType), index) && (int)file.Filetype == index))
                {
                    file.Properties.Add(propertyKeyTextBox.Text, propertyValueTextBox.Text);
                }
            }

            if (Enum.IsDefined(typeof(PckFile.FileData.FileType), index))
            {
                MessageBox.Show($"Data added to {(PckFile.FileData.FileType)index} entries");
                return;
            }
            MessageBox.Show("Data added to all entries");
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyKeyTextBox.Text = propertyTreeview.SelectedNode.Text;
        }
    }
}
