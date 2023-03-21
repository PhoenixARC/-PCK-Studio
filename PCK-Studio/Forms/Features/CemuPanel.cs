using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Classes.Misc;

namespace PckStudio.Forms.Additional_Features
{
    /// <summary>
    /// Wishlist:
    ///  - add the ability to save the currently open pck file to the desired folder destination.
    ///   (even if the pck file has not yet be saved to disk)
    /// </summary>
    public partial class CemuPanel : UserControl
    {
        public CemuPanel()
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
            if (!IsValidInstallDirectory())
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

            DirectoryInfo dlcDirectory = new DirectoryInfo($"{GetGameContentPath()}/WiiU/DLC");
            
            if (!dlcDirectory.Exists)
            {
                MessageBox.Show($"'{GetGameContentPath()}/WiiU/DLC' does not exist!", "Not Found",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var directoryInfo in dlcDirectory.GetDirectories())
            {
                if (directoryInfo.GetFileSystemInfos().Length != 0)
                {
                    var node = DLCTreeView.Nodes.Add(directoryInfo.Name);
                    var dirs = directoryInfo.GetDirectories("Data", SearchOption.TopDirectoryOnly);
                    bool hasDataFolderAndPckFile = dirs.Length != 0
                        && dirs.FirstOrDefault(
                            d => d.GetFiles().FirstOrDefault(f => f.Name.EndsWith(".pck")) is not null
                        ) is not null;

                    node.Tag = directoryInfo.FullName +
                        (hasDataFolderAndPckFile
                        ? $";{directoryInfo.FullName}/Data"
                        : string.Empty);
                }
            }
        }

        private bool IsValidInstallDirectory()
        {
            return !string.IsNullOrWhiteSpace(GameDirectoryTextBox.Text) && Directory.Exists(GameDirectoryTextBox.Text);
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            ListDLCs();
        }

        private bool IsValidGameDirectory()
        {
            return IsValidInstallDirectory() && Directory.Exists(GetGameContentPath());
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

        private void addCustomPckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt prompt = new RenamePrompt(string.Empty);
            prompt.OKButton.Text = "OK";
            prompt.TextLabel.Text = "Folder:";
            
            if (prompt.ShowDialog(this) != DialogResult.OK)
                return;
            if (string.IsNullOrWhiteSpace(prompt.NewText))
            {
                MessageBox.Show("Invalid Folder name entered!", "Invalid Folder Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists($"{GetGameContentPath()}/WiiU/DLC/{prompt.NewText}"))
            {
                MessageBox.Show("A Folder with the same name already exists!", "Folder Name taken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Directory.CreateDirectory($"{GetGameContentPath()}/WiiU/DLC/{prompt.NewText}");

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PCK (Minecraft Console Package)|*.pck";
            fileDialog.CheckFileExists = true;
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                File.Copy(fileDialog.FileName, $"{GetGameContentPath()}/WiiU/DLC/{prompt.NewText}/{fileDialog.SafeFileName}");
            }
        }

        private void removePckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pckName = DLCTreeView.SelectedNode.Text;
            var result = MessageBox.Show($"Are you sure you want to permanetly delete '{pckName}'?", "Hold up!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Directory.Delete($"{GetGameContentPath()}/WiiU/DLC/{pckName}", recursive: true);
                ListDLCs();
            }
        }
    }
}
