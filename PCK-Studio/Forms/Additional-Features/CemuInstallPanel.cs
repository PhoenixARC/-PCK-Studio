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

        private string GetConsoleRegion()
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
            string region = GetConsoleRegion();
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
            DirectoryInfo directoryInfo = new DirectoryInfo($"{GetGameContentPath()}/WiiU/DLC");
            foreach (var subDirectory in directoryInfo.GetDirectories())
            {
                if (subDirectory.GetFileSystemInfos().Length != 0)
                {
                    var node = DLCTreeView.Nodes.Add(subDirectory.Name);
                    var dirs = subDirectory.GetDirectories("Data", SearchOption.TopDirectoryOnly);
                    bool hasDataFolder = dirs.Length != 0
                        && dirs.FirstOrDefault(d => d.GetFiles().FirstOrDefault(f => f.Name.EndsWith(".pck")) is not null) is not null;
                    if (hasDataFolder)
                    {
                        node.Tag = $"{subDirectory.FullName};{subDirectory.FullName}/Data";
                        node.Nodes.Add("Data");
                        continue;
                    }
                    node.Tag = subDirectory.FullName;
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(GetGameContentPath()))
            {
                MessageBox.Show($"Could not find '{GetGameContentPath()}'!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ListDLCs();
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
