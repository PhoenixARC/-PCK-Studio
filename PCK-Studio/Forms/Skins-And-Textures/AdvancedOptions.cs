using OMI;
using OMI.Formats.Pck;
using PckStudio.ToolboxItems;
using OMI.Workers.Pck;
using PckStudio.Extensions;
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
    public partial class AdvancedOptions : ThemeForm
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
                applyBulkProperties(_pckFile.GetFiles(), fileTypeComboBox.SelectedIndex - 1);
                DialogResult = DialogResult.OK;
                return;
            }
            MessageBox.Show("Please select a filetype before applying");
        }

        private void applyBulkProperties(IReadOnlyCollection<PckFileData> files, int index)
		{
            foreach (PckFileData file in files)
            {
                if (file.Filetype == PckFileType.TexturePackInfoFile ||
                file.Filetype == PckFileType.SkinDataFile)
                {
                    try
                    {
                        var reader = new PckFileReader(_endianness);
                        using var ms = new MemoryStream(file.Data);
                        PckFile subPCK = reader.FromStream(ms);
                        applyBulkProperties(subPCK.GetFiles(), index);
                        file.SetData(new PckFileWriter(subPCK, _endianness));
                    }
                    catch (OverflowException ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                if (index == -1 || (Enum.IsDefined(typeof(PckFileType), index) && (int)file.Filetype == index))
                {
                    file.Properties.Add(propertyKeyTextBox.Text, propertyValueTextBox.Text);
                }
            }

            if (Enum.IsDefined(typeof(PckFileType), index))
            {
                MessageBox.Show($"Data added to {(PckFileType)index} entries");
                return;
            }
            MessageBox.Show("Data added to all entries");
        }

        private void propertyTreeview_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyKeyTextBox.Text = propertyTreeview.SelectedNode.Text;
        }
    }
}
