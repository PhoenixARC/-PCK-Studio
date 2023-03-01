using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.Misc;

namespace PckStudio.Forms.Additional_Features
{
    public partial class CemuInstallPanel : UserControl
    {
        public CemuInstallPanel()
        {
            InitializeComponent();
        }

        private string GetSelectedRegionTitleId()
        {
            if (radioButtonEur.Checked)
            {
                return "101d7500";
            }
            if (radioButtonUs.Checked)
            {
                return "101d9d00";
            }
            if (radioButtonJap.Checked)
            {
                return "101dbe00";
            }
            throw new Exception("how did you get here ?");
        }

        private string GetGameContentPath()
        {
            string region = GetSelectedRegionTitleId();
            return $"{GameDirectoryTextBox.Text}/usr/title/0005000e/{region}/content";
        }

        private void BrowseDirectoryBtn_Click(object sender, EventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Cemu Game Directory";
            if (openFolderDialog.ShowDialog(Handle) == true && Directory.Exists(openFolderDialog.ResultPath))
            {
                GameDirectoryTextBox.Text = openFolderDialog.ResultPath;
                ListDLCs();
            }
        }

        private void ListDLCs()
        {
            DLCTreeView.Nodes.Clear();
            if (string.IsNullOrWhiteSpace(GameDirectoryTextBox.Text) || !Directory.Exists(GameDirectoryTextBox.Text))
            {
                MessageBox.Show("Please select a valid Game Directory!", "Invalid Directory Specified",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidGameDirectory())
            {
                MessageBox.Show($"Could not find '{GetGameContentPath()}'!", "Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DirectoryInfo dlcGameDirectory = new DirectoryInfo($"{GetGameContentPath()}/WiiU/DLC");
            foreach (var subDirectoryInfo in dlcGameDirectory.GetDirectories())
            {
                if (subDirectoryInfo.GetFileSystemInfos().Length != 0)
                {
                    var node = DLCTreeView.Nodes.Add(subDirectoryInfo.Name);
                    var dirs = subDirectoryInfo.GetDirectories("Data", SearchOption.TopDirectoryOnly);
                    bool hasDataFolder = dirs.Length != 0
                        && dirs.FirstOrDefault(
                            d => d.GetFiles().FirstOrDefault(f => f.Name.EndsWith(".pck")) is not null
                        ) is not null;

                    if (hasDataFolder)
                    {
                        node.Tag = $"{subDirectoryInfo.FullName};{subDirectoryInfo.FullName}/Data";
                        node.Nodes.Add("Data");
                        continue;
                    }
                    node.Tag = subDirectoryInfo.FullName;
                }
            }
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            ListDLCs();
        }

        private bool IsValidGameDirectory()
        {
            string contentPath = GetGameContentPath();
            return Directory.Exists(contentPath);
        }

        private void openSkinPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DLCTreeView.SelectedNode.Tag is string path &&
                Directory.Exists(path.Split(';')[0]))
            {
                string pckFilePath = Directory.GetFiles(path.Split(';')[0]).FirstOrDefault(s => s.EndsWith(".pck"));
                if (!string.IsNullOrEmpty(pckFilePath))
                {
                    Program.MainInstance.LoadPckFromFile(pckFilePath);
                }
            }
        }

        private void openTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DLCTreeView.SelectedNode.Tag is string path &&
                path.Split(';').Length > 1 &&
                Directory.Exists(path.Split(';')[1]))
            {
                string pckFilePath = Directory.GetFiles(path.Split(';')[1]).FirstOrDefault(s => s.EndsWith(".pck"));
                if (!string.IsNullOrEmpty(pckFilePath))
                {
                    Program.MainInstance.LoadPckFromFile(pckFilePath);
                }
            }
        }

        private void DLCTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            openTexturePackToolStripMenuItem.Visible = e.Node.Tag is string nodeData && nodeData.Split(';').Length >= 2;
        }
    }
}
