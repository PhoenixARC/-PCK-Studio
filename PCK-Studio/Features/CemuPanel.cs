using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PckStudio.Extensions;
using PckStudio.Classes.Misc;
using System.Xml.Serialization;
using System.Diagnostics;

namespace PckStudio.Features
{
    /// <summary>
    /// Wishlist:
    ///  - add the ability to save the currently open pck file to the desired folder destination.
    ///   (even if the pck file has not yet be saved to disk)
    /// </summary>
    public partial class CemuPanel : UserControl
    {
        private const string TitleId_EUR = "101d7500";
        private const string TitleId_USA = "101d9d00";
        private const string TitleId_JPN = "101dbe00";

        public CemuPanel()
        {
            InitializeComponent();
            if (!TryApplyPermanentCemuConfig() &&
                MessageBox.Show("Failed to get Cemu perma settings\nDo you want to open your local settings.xml file?",
                "Cemu mlc path not found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes
                )
            {

                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = "Cemu Settings|settings.xml",
                };

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    TryApplyCemuConfig(fileDialog.FileName);
                }
            }
        }

        private bool TryApplyCemuConfig(string settingsPath)
        {
            string cemuPath = Path.Combine(Path.GetDirectoryName(settingsPath), "Cemu.exe");
            if (File.Exists(settingsPath) && File.Exists(cemuPath))
            {
                try
                {
                    var xml = new XmlDocument();
                    xml.Load(settingsPath);
                    GameDirectoryTextBox.Text = xml.SelectSingleNode("content").SelectSingleNode("mlc_path").InnerText;
                    GameDirectoryTextBox.ReadOnly = true;
                    BrowseDirectoryBtn.Enabled = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex, category: $"{nameof(CemuPanel)}:{nameof(TryApplyCemuConfig)}");
                    return false;
                }
            }
            return false;
        }

        private bool TryApplyPermanentCemuConfig()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Cemu");
            string filepath = Path.Combine(path, "perm_setting.xml");
            if (Directory.Exists(path) && File.Exists(filepath))
            {
                try
                {
                    var xml = new XmlDocument();
                    using (var reader = new StreamReader(filepath))
                    {
                        reader.ReadLine();
                        xml.Load(reader);
                    }
                    var configNode = xml.SelectSingleNode("config");
                    var mlcpathNode = configNode.SelectSingleNode("MlcPath");
                    GameDirectoryTextBox.Text = mlcpathNode.InnerText;
                    GameDirectoryTextBox.ReadOnly = true;
                    BrowseDirectoryBtn.Enabled = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex, category: $"{nameof(CemuPanel)}:{nameof(TryApplyPermanentCemuConfig)}");
                    return false;
                }
            }
            return false;
        }

        private string GetSelectedRegionTitleId()
        {
            if (radioButtonEur.Checked)
            {
                return TitleId_EUR;
            }
            if (radioButtonUs.Checked)
            {
                return TitleId_USA;
            }
            if (radioButtonJap.Checked)
            {
                return TitleId_JPN;
            }
            throw new Exception("how did you get here ?");
        }

        private string GetGameContentPath(string region)
        {
            return $"{GameDirectoryTextBox.Text}/usr/title/0005000e/{region}/content";
        }

        private string GetGameContentPath()
        {
            string region = GetSelectedRegionTitleId();
            return GetGameContentPath(region);
        }

        private string GetContentSubDirectory(params string[] subpaths)
        {
            return Path.Combine(GetGameContentPath(), Path.Combine(subpaths));
        }

        private void BrowseDirectoryBtn_Click(object sender, EventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Select Cemu mlc01 Directory"
            };
            if (openFolderDialog.ShowDialog(Handle) == true && Directory.Exists(openFolderDialog.ResultPath))
            {
                GameDirectoryTextBox.Text = openFolderDialog.ResultPath;
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
            TextPrompt prompt = new TextPrompt();
            prompt.OKButtonText = "OK";
            prompt.LabelText = "Folder:";
            
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

        private void GameDirectoryTextBox_TextChanged(object sender, EventArgs e)
        {
            if (IsValidInstallDirectory())
            {
                radioButtonEur.Enabled = Directory.Exists(GetGameContentPath(TitleId_EUR));
                radioButtonUs.Enabled = Directory.Exists(GetGameContentPath(TitleId_USA));
                radioButtonJap.Enabled = Directory.Exists(GetGameContentPath(TitleId_JPN));
            }
            ListDLCs();
        }

        private void GameDirectoryTextBox_Click(object sender, EventArgs e)
        {
            if (GameDirectoryTextBox.ReadOnly)
            {
                Process.Start(GetContentSubDirectory("WiiU", "DLC"));
            }
        }

        private void DLCTreeView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && DLCTreeView.SelectedNode is not null)
            {
                openSkinPackToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
            base.OnPreviewKeyDown(e);
        }

        private void DLCTreeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
