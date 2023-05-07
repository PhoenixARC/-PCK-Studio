using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Extensions;
using PckStudio.Classes.Misc;

namespace PckStudio.Features
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

        private string GetContentSubDirectory(params string[] subpaths)
        {
            return Path.Combine(GetGameContentPath(), Path.Combine(subpaths));
        }

        private void BrowseDirectoryBtn_Click(object sender, EventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Select Cemu Game Directory"
            };
            if (openFolderDialog.ShowDialog(Handle) == true && Directory.Exists(openFolderDialog.ResultPath))
            {
                GameDirectoryTextBox.Text = openFolderDialog.ResultPath;
                ListDLCs();
            }
        }

        private class DLCDirectoryInfo
        {
            private readonly bool _hasTexturePack;
            private readonly string _basePckPath;
            private readonly string _texturePackPath;

            public bool HasTexturePack => _hasTexturePack;
            public string PackPath => _basePckPath;
            public string TexturePackPath => _texturePackPath;

            public DLCDirectoryInfo(DirectoryInfo directory)
            {
                _basePckPath = directory.GetFiles().FirstOrDefault(f => f.Name.EndsWith(".pck")).FullName;
                _ = _basePckPath ?? throw new NullReferenceException($"Could not find any '.pck' inside {directory.Name}");
                if (TryGetDataDirectory(directory, out var dataDir))
                {
                    var tpFileInfo = dataDir.GetFiles().FirstOrDefault(f => !f.Name.Equals("audio.pck") && f.Name.EndsWith(".pck"));
                    _hasTexturePack = tpFileInfo is not null;
                    _texturePackPath = _hasTexturePack ? tpFileInfo.FullName : string.Empty;
                }
            }

            public DLCDirectoryInfo(string path)
                : this(new DirectoryInfo(path))
            {
            }

            private bool TryGetDataDirectory(DirectoryInfo directory, out DirectoryInfo dataDirectory)
            {
                var dirs = directory.GetDirectories("Data", SearchOption.TopDirectoryOnly);
                dataDirectory = dirs.Length != 0 ? dirs[0] : null;
                return dirs.Length != 0;
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

            string dirPath = GetContentSubDirectory("WiiU", "DLC");
            DirectoryInfo dlcDirectory = new DirectoryInfo(dirPath);

            if (!dlcDirectory.Exists)
            {
                MessageBox.Show($"'{dirPath}' does not exist!", "Not Found",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var directoryInfo in dlcDirectory.GetDirectories())
            {
                if (directoryInfo.GetFileSystemInfos().Length != 0)
                {
                    var node = DLCTreeView.Nodes.Add(directoryInfo.Name);
                    node.Tag = new DLCDirectoryInfo(directoryInfo);
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
            if (DLCTreeView.SelectedNode.Tag is DLCDirectoryInfo dlcDir)
            {
                Program.MainInstance.LoadPckFromFile(dlcDir.PackPath);
            }
        }

        private void openTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DLCTreeView.SelectedNode.Tag is DLCDirectoryInfo dlcDir && dlcDir.HasTexturePack)
            {
                Program.MainInstance.LoadPckFromFile(dlcDir.TexturePackPath);
            }
        }

        private void DLCTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            openTexturePackToolStripMenuItem.Visible = e.Node.Tag is DLCDirectoryInfo dlcDir && dlcDir.HasTexturePack;
        }

        private void addCustomPckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt prompt = new RenamePrompt(string.Empty);
            prompt.OKButton.Text = "OK";
            prompt.TextLabel.Text = "Folder:";
            
            if (prompt.ShowDialog(this) != DialogResult.OK)
                return;
            
            if (prompt.NewText.ContainsAny(Path.GetInvalidPathChars()))
            {
                MessageBox.Show("Invalid Folder name entered!", "Invalid Folder Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string directoryPath = GetContentSubDirectory("WiiU", "DLC", prompt.NewText);

            if (Directory.Exists(directoryPath))
            {
                MessageBox.Show("A Folder with the same name already exists!", "Folder Name taken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Directory.CreateDirectory(directoryPath);

            using OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "PCK (Minecraft Console Package)|*.pck"
            };
            if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                File.Copy(fileDialog.FileName, Path.Combine(directoryPath, fileDialog.SafeFileName));
            }
        }

        private void removePckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pckName = DLCTreeView.SelectedNode.Text;
            var result = MessageBox.Show($"Are you sure you want to permanently delete '{pckName}'?", "Hold up!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string directoryPath = GetContentSubDirectory("WiiU", "DLC", pckName);
                Directory.Delete(directoryPath, recursive: true);
                DLCTreeView.SelectedNode.Remove();
            }
        }

        private void DLCTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            openSkinPackToolStripMenuItem_Click(sender, e);
        }
    }
}
