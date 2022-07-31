using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using PckStudio.Properties;
using Ohana3DS_Rebirth.Ohana;
using PckStudio.Forms;
using System.Drawing.Imaging;
using RichPresenceClient;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO;
using PckStudio.Classes.IO.LOC;
using PckStudio.Forms.Utilities;
using PckStudio.Classes.IO.GRF;
using PckStudio.Classes.Utils;
using PckStudio.Forms.Editor;

namespace PckStudio
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        public string saveLocation = string.Empty;
        PCKFile currentPCK = null;
        bool needsUpdate = false;
        bool saved = true;
        bool isTemplateFile = false;
        public MainForm()
        {
            InitializeComponent();
            this.skinToolStripMenuItem1.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 0); };
            this.capeToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 1); };
            this.textureToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 2); };
            this.languagesFileLOCToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 6); };
            this.gameRulesFileGRFToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 7); };
            this.audioPCKFileToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 8); };
            this.coloursCOLFileToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 9); };
            this.gameRulesHeaderGRHToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 10); };
            this.skinsPCKToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 11); };
            this.modelsFileBINToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 12); };
            this.behavioursFileBINToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 13); };
            this.entityMaterialsFileBINToolStripMenuItem.Click += (sender, EventArgs) => { setFileType_Click(sender, EventArgs, 14); };
            imageList.Images.Add(Resources.ZZFolder);
            imageList.Images.Add(Resources.BINKA_ICON);
            imageList.Images.Add(Resources.IMAGE_ICON);
            imageList.Images.Add(Resources.LOC_ICON);
            imageList.Images.Add(Resources.PCK_ICON);
            imageList.Images.Add(Resources.ZUnknown);
            pckOpen.AllowDrop = true;
            tabControl.SelectTab(0);
            labelVersion.Text = "PCK Studio: " + Application.ProductVersion;
#if DEBUG
            labelVersion.Text += " (dev)";
#endif
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RPC.Initialize();
            RPC.SetPresence("An Open Source .PCK File Editor", "Program by PhoenixARC");

            // Makes sure appdata exists
            try
            {
                Directory.CreateDirectory(Program.Appdata + "\\cache\\mods\\");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Could not Create directory due to Unauthorized Access");
                Console.WriteLine(ex.Message);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            checkSaveState();
            RPC.Deinitialize();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentPCK = openPck(ofd.FileName);
                    if (addPasswordToolStripMenuItem.Enabled = checkForPassword())
                    {
                        LoadEditorTab();
                    }
                }
            }
        }

        private PCKFile openPck(string filePath)
        {
            PCKFile pck = null;
            using (var fileStream = File.OpenRead(filePath))
            {
                isTemplateFile = false;
                saveLocation = filePath;
                try
                {
                    pck = PCKFileReader.Read(fileStream, LittleEndianCheckBox.Checked);
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex.Message);
                }
            }
            if (pck.type < 3) throw new Exception("Can't open pck file of type: " + pck.type.ToString());
            return pck;
        }

        private bool checkForPassword()
        {
            if (currentPCK.HasFile("0", 4))
            {
                var file = currentPCK.GetFile("0", 4);
                if (file.properties.HasProperty("LOCK"))
                    return new pckLocked(file.properties.GetProperty("LOCK").Item2).ShowDialog() == DialogResult.OK;
            }
            return true;
        }

        private void LoadEditorTab()
        {
            fileEntryCountLabel.Text = "Files:" + currentPCK.Files.Count;
            treeViewMain.Enabled = treeMeta.Enabled = true;
            closeToolStripMenuItem.Visible = true;
            saveToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem1.Enabled = true;
            metaToolStripMenuItem.Enabled = true;
            advancedMetaAddingToolStripMenuItem.Enabled = true;
            convertToBedrockToolStripMenuItem.Enabled = true;
            BuildMainTreeView();
            tabControl.SelectTab(1);
        }

        private void CloseEditorTab()
        {
            pictureBoxImagePreview.Image = Resources.NoImageFound;
            treeViewMain.Nodes.Clear();
            treeMeta.Nodes.Clear();
            currentPCK = null;
            treeViewMain.Enabled = false;
            treeMeta.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem1.Enabled = false;
            metaToolStripMenuItem.Enabled = false;
            addPasswordToolStripMenuItem.Enabled = false;
            advancedMetaAddingToolStripMenuItem.Enabled = false;
            convertToBedrockToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Visible = false;
            fileEntryCountLabel.Text = "Files:0";
            tabControl.SelectTab(0);
        }

        /// <summary>
        /// wrapper that allows the use of <paramref name="name"/> in <code>TreeNode.Nodes.Find(<paramref name="name"/>, ...)</code> and <code>TreeNode.Nodes.ContainsKey(<paramref name="name"/>)</code>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns>new Created TreeNode</returns>
        public static TreeNode CreateNode(string name, object tag = null)
        {
            TreeNode node = new TreeNode(name);
            node.Name = name;
            node.Tag = tag;
            return node;
        }

        private TreeNode BuildNodeTreeBySeperator(TreeNodeCollection root, string path, char seperator)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (!path.Contains(seperator))
            {
                var finalNode = CreateNode(path);
                root.Add(finalNode);
                return finalNode;
            }
            string nodeText = path.Substring(0, path.IndexOf(seperator));
            string subPath = path.Substring(path.IndexOf(seperator) + 1);
            bool alreadyExists = root.ContainsKey(nodeText);
            TreeNode subNode = alreadyExists ? root[nodeText] : CreateNode(nodeText);
            if (!alreadyExists) root.Add(subNode);
            return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
        }

        private void BuildPckTreeView(TreeNodeCollection root, PCKFile pckFile)
        {
            pckFile.Files.ForEach(file =>
            {
                TreeNode node = BuildNodeTreeBySeperator(root, file.filepath, '/');
                node.Tag = file;
                switch (file.type)
                {
                    case 0:
                    case 1:
                    case 2:
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                        break;
                    case 6:
                        node.ImageIndex = 3;
                        node.SelectedImageIndex = 3;
                        break;
                    case 8:
                        node.ImageIndex = 1;
                        node.SelectedImageIndex = 1;
                        break;
                    case 5:
                    case 11:
                        node.ImageIndex = 4;
                        node.SelectedImageIndex = 4;
                        // TODO: load sub pck into tree and make it editable with ease
                        // works but not currently included...
                        using (var stream = new MemoryStream(file.data))
                        {
                            try
                            {
                                PCKFile subPCKfile = PCKFileReader.Read(stream, LittleEndianCheckBox.Checked);
                                BuildPckTreeView(node.Nodes, subPCKfile);
                            }
                            catch (OverflowException ex)
                            {
                                MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    default:
                        node.ImageIndex = 5;
                        node.SelectedImageIndex = 5;
                        break;
                }
            });
        }

        private void BuildMainTreeView()
        {
            treeViewMain.Nodes.Clear();
            BuildPckTreeView(treeViewMain.Nodes, currentPCK);
        }

        private void selectNode(object sender, TreeViewEventArgs e)
        {
            ReloadMetaTreeView();
            entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = "";
            buttonEdit.Visible = false;
            pictureBoxImagePreview.Image = Resources.NoImageFound;
            var node = e.Node;
            if (node.Tag == null || !(node.Tag is PCKFile.FileData)) return;
            PCKFile.FileData file = node.Tag as PCKFile.FileData;

            if (file.properties.HasProperty("BOX"))
            {
                buttonEdit.Text = "EDIT BOXES";
                buttonEdit.Visible = true;
            }
            else if (file.properties.HasProperty("ANIM") &&
                (file.properties.GetProperty("ANIM").Item2 == "0x40000" ||
                 file.properties.GetProperty("ANIM").Item2 == "0x80000"))
            {
                buttonEdit.Text = "View Skin";
                buttonEdit.Visible = true;
            }

            switch (file.type)
            {
                case 2 when (file.filepath.StartsWith("res/textures/blocks/") || file.filepath.StartsWith("res/textures/items/")) &&
                            !file.filepath.EndsWith("clock.png") && (!file.filepath.EndsWith("compass.png")):
                    buttonEdit.Text = "EDIT TEXTURE ANIMATION";
                    buttonEdit.Visible = true;
                    using (MemoryStream png = new MemoryStream(file.data))
                    {
                        Image skinPicture = Image.FromStream(png);
                        pictureBoxImagePreview.Image = skinPicture;
                        labelImageSize.Text = $"{skinPicture.Size.Width}x{skinPicture.Size.Height}";
                    }
                    break;

                case 0:
                case 1:
                case 2:
                    using (MemoryStream png = new MemoryStream(file.data))
                    {
                        Image skinPicture = Image.FromStream(png);
                        pictureBoxImagePreview.Image = skinPicture;
                        labelImageSize.Text = $"{skinPicture.Size.Width}x{skinPicture.Size.Height}";
                    }
                    break;

                case 6:
                    buttonEdit.Text = "EDIT LOC";
                    buttonEdit.Visible = true;
                    break;

                case 8 when file.filepath == "audio.pck":
                    buttonEdit.Text = "EDIT MUSIC CUES";
                    buttonEdit.Visible = true;
                    break;

                case 9 when file.filepath == "colours.col":
                    buttonEdit.Text = "EDIT COLORS";
                    buttonEdit.Visible = true;
                    break;
                default:
                    buttonEdit.Visible = false;
                    break;
            }
        }

        public void editModel(PCKFile.FileData skin)
        {
            generateModel generate = new generateModel(skin.properties, new PictureBox());
            //Opens Model Generator Dialog
            if (generate.ShowDialog() == DialogResult.OK)
            {
                entryTypeTextBox.Text = "";
                entryDataTextBox.Text = "";
                ReloadMetaTreeView();
                saved = false;
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeViewMain.SelectedNode;
            if (node == null) return;
            if (node.Tag is PCKFile.FileData)
            {
                var file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
                using SaveFileDialog exFile = new SaveFileDialog();
                exFile.FileName = Path.GetFileName(file.filepath);
                exFile.Filter = Path.GetExtension(file.filepath).Replace(".", "") + " File|*" + Path.GetExtension(file.filepath);
                if (exFile.ShowDialog() != DialogResult.OK ||
                    // Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
                    string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName))) return;
                string extractFilePath = exFile.FileName;

                File.WriteAllBytes(extractFilePath, file.data);
                if (file.properties.Count > 0)
                {
                    using var fs = File.CreateText($"{extractFilePath}.txt");
                    file.properties.ForEach(property => fs.WriteLine($"{property.Item1}: {property.Item2}"));
                }
                // Verification that file extraction path was successful
                MessageBox.Show("File Extracted");
                return;
            }

            string selectedFolder = node.FullPath;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = @"Select destination folder";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    currentPCK.Files.ForEach(file =>
                    {
                        if (file.filepath.StartsWith(selectedFolder))
                        {
                            Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(file.filepath)}");
                            File.WriteAllBytes($"{dialog.SelectedPath}/{file.filepath}", file.data);
                        }
                    });
                    MessageBox.Show("Folder Extracted");
                }
            }
        }

        private void SaveTemplate()
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PCK (Minecraft Console Package)|*.pck";
            saveFileDialog.DefaultExt = ".pck";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Save(saveFileDialog.FileName);
                saveLocation = saveFileDialog.FileName;
                isTemplateFile = false;
            }
        }

        private void Save(string FilePath)
        {
            using (var fs = File.OpenWrite(FilePath))
            {
                PCKFileWriter.Write(fs, currentPCK, LittleEndianCheckBox.Checked);
            }
            saved = true;
            MessageBox.Show("Saved Pck file", "File Saved");
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PCKFile.FileData)
            {
                PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
                using (var ofd = new OpenFileDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        file.SetData(File.ReadAllBytes(ofd.FileName));
                        saved = false;
                    }
                }
                return;
            }
            // should never happen unless its a folder
            MessageBox.Show("Can't replace a folder.");
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeViewMain.SelectedNode;
            if (node == null) return;
            if (node.Tag is PCKFile.FileData)
            {
                PCKFile.FileData file = node.Tag as PCKFile.FileData;
                // remove loc key if its a skin/cape
                if (file.type == 0 || file.type == 1)
                {
                    LOCFile locFile = null;
                    if (TryGetLocFile(out locFile))
                    {
                        foreach (var property in file.properties)
                        {
                            if (property.Item1 == "THEMENAMEID" || property.Item1 == "DISPLAYNAMEID")
                                locFile.RemoveLocKey(property.Item2);
                        }
                        TrySetLocFile(locFile);
                    }
                }
                currentPCK.Files.Remove(file);
                node.Remove();
                saved = false;
            }
            else if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string pckFolderDir = node.FullPath;
                currentPCK.Files.RemoveAll(file => file.filepath.StartsWith(pckFolderDir));
                node.Remove();
                saved = false;
            }
        }

        private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null) return;
            using RenamePrompt diag = new RenamePrompt(node.FullPath);
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                if (node.Tag is PCKFile.FileData)
                {
                    var file = node.Tag as PCKFile.FileData;
                    file.filepath = diag.NewText;
                }
                else // folder
                {
                    currentPCK.Files.ForEach(file =>
                    {
                        if (file.filepath.StartsWith(node.FullPath))
                            file.filepath = diag.NewText + file.filepath.Substring(node.FullPath.Length);
                    });
                }
                saved = false;
                BuildMainTreeView();
            }
        }

        private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TryGetLocFile(out LOCFile locFile))
            {
                MessageBox.Show("No .loc file found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (addNewSkin add = new addNewSkin(locFile))
                if (add.ShowDialog() == DialogResult.OK)
                {
                    if (add.useCape)
                        currentPCK.Files.Add(add.Cape);
                    if (!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
                        add.Skin.filepath = $"{treeViewMain.SelectedNode.FullPath}/{add.Skin.filepath}";
                    currentPCK.Files.Add(add.Skin);
                    TrySetLocFile(locFile);
                    saved = false;
                    BuildMainTreeView();
                }
        }

        private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Should work because all nodes in the main treeview are created with the 'CreateNode' method
            if (treeViewMain.Nodes.Find("audio.pck", true).Length > 0)
            {
                MessageBox.Show("There is already an audio.pck present in this file!", "Can't create audio.pck");
                return;
            }
            if (!TryGetLocFile(out LOCFile locFile))
                throw new Exception("No .loc file found.");
            AudioEditor diag = new AudioEditor(locFile, LittleEndianCheckBox.Checked);
            diag.ShowDialog(this);
            //if (diag.saved) treeViewMain.Nodes.Add(node);
            diag.Dispose();
        }


        private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Files | *.png";
                ofd.Title = "Select a PNG File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Forms.Utilities.AnimationEditor.ChangeTile diag = new Forms.Utilities.AnimationEditor.ChangeTile())
                            if (diag.ShowDialog(this) == DialogResult.OK)
                            {
                                Console.WriteLine(diag.SelectedTile);
                                using (Image img = new Bitmap(ofd.FileName))
                                using (AnimationEditor animationEditor = new AnimationEditor(img, diag.SelectedTile, diag.IsItem))
                                {
                                    if (animationEditor.ShowDialog() == DialogResult.OK)
                                    {
                                        treeMeta.Nodes.Clear();
                                        saved = false;
                                    }
                                }
                            }
                    }
                    catch
                    {
                        MessageBox.Show("Invalid animation data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void treeViewMain_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode == null ||
                treeViewMain.SelectedNode.Tag == null ||
                !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
                return;
            PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;

            switch (file.type)
            {
                case 6:
                    LOCFile l = null;
                    using (var stream = new MemoryStream(file.data))
                    {
                        l = LOCFileReader.Read(stream);
                    }
                    var locedit = new LOCEditor(l);
                    locedit.ShowDialog(this);
                    if (locedit.WasModified)
                    {
                        using (var stream = new MemoryStream())
                        {
                            LOCFileWriter.Write(stream, l);
                            file.SetData(stream.ToArray());
                        }
                        saved = false;
                    }
                    break;

                case 7:
                case 10 when (Path.GetExtension(file.filepath) == ".grf" && file.type == 7) ||
                                (Path.GetExtension(file.filepath) == ".grh" && file.type == 10):
                    using (GRFEditor grfEditor = new GRFEditor(file))
                        grfEditor.ShowDialog();
                    break;

                case 8 when file.filepath == "audio.pck":
                    try
                    {
                        if (!TryGetLocFile(out LOCFile locFile))
                            throw new Exception("No .loc File found.");
                        AudioEditor audioEditor = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
                        audioEditor.ShowDialog(this);
                        audioEditor.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;

                case 9 when file.filepath == "colours.col":
                    if (file.size == 0)
                    {
                        MessageBox.Show("No Color data found.", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    COLFile colFile = new COLFile();
                    using (var stream = new MemoryStream(file.data))
                    {
                        colFile.Open(stream);
                    }
                    COLEditor diag = new COLEditor(colFile);
                    if (diag.ShowDialog(this) == DialogResult.OK && diag.data.Length > 0)
                        file.SetData(diag.data);
                    diag.Dispose();
                    break;
            }

            //System.Threading.ThreadStart starter;

            //System.Threading.Thread binkam;
            //Checks to see if selected minefile is a binka file
            if (Path.GetExtension(file.filepath) == ".binka")
            {
                MessageBox.Show(".binka Editor Coming Soon!");
            }
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null || !(node.Tag is ValueTuple<string, string>)) return;
            var property = (ValueTuple<string, string>)node.Tag;
            entryTypeTextBox.Text = property.Item1;
            entryDataTextBox.Text = property.Item2;
        }

        private void treeMeta_DoubleClick(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode == null || !(treeMeta.SelectedNode.Tag is ValueTuple<string, string>) ||
                treeViewMain.SelectedNode == null || !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
                return;
            PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;
            var property = (ValueTuple<string, string>)treeMeta.SelectedNode.Tag;
            int i = file.properties.IndexOf(property);
            using addMeta add = new addMeta(property.Item1, property.Item2);
            if (add.ShowDialog() == DialogResult.OK && i != -1)
            {
                file.properties[i] = new ValueTuple<string, string>(add.PropertyName, add.PropertyValue);
                ReloadMetaTreeView();
                saved = false;
            }
        }

        private void cloneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            PCKFile.FileData mfO = node.Tag as PCKFile.FileData;

            PCKFile.FileData mf = new PCKFile.FileData("", mfO.type); // Creates new minefile template
            mf.SetData(mfO.data); // adds file data to minefile
            string dirName = Path.GetDirectoryName(mfO.filepath).Replace("\\", "/");

            int clone_number = 0;
            string prev_clone_str = "_clone1";
            string nameWithoutExt = Path.GetFileNameWithoutExtension(mfO.filepath);
            string newFileName = mfO.filepath;
            do // Checks for existing clones and names it accordingly
			{
                clone_number++;
                string clone_str = "_clone" + clone_number.ToString();
                bool isClone = nameWithoutExt.Contains("_clone");
                if(isClone) newFileName = nameWithoutExt.Remove(nameWithoutExt.Length - 7) + clone_str + Path.GetExtension(mfO.filepath);
                else newFileName = nameWithoutExt + clone_str + Path.GetExtension(mfO.filepath);
                prev_clone_str = clone_str;
            }
            while (currentPCK.HasFile(dirName + (string.IsNullOrEmpty(dirName) ? "" : "/") + newFileName, mf.type));

            mf.filepath = dirName + (string.IsNullOrEmpty(dirName) ? "" : "/") + newFileName; //sets minfile name to file name
            foreach (var entry in mfO.properties)
            {
                var property = (ValueTuple<string, string>)entry;
                mf.properties.Add(property);
            }

            TreeNode newNode = new TreeNode();
            newNode.Text = newFileName;
            newNode.Tag = mf;
            newNode.ImageIndex = node.ImageIndex;
            newNode.SelectedImageIndex = node.SelectedImageIndex;

            if (node.Parent == null) treeViewMain.Nodes.Insert(node.Index + 1, newNode); //adds generated minefile node
            else node.Parent.Nodes.Insert(node.Index + 1, newNode);//adds generated minefile node to selected folder
            currentPCK.Files.Insert(node.Index + 1, mf);
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode == null || !(treeMeta.SelectedNode.Tag is ValueTuple<string, string>) ||
                !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
                return;
            var file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
            if (file.properties.Remove((ValueTuple<string, string>)treeMeta.SelectedNode.Tag))
            {
                treeMeta.SelectedNode.Remove();
                saved = false;
            }
        }

        private void ReloadMetaTreeView()
        {
            treeMeta.Nodes.Clear();
            if (treeViewMain.SelectedNode == null ||
                !(treeViewMain.SelectedNode.Tag is PCKFile.FileData)) return;
            var file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
            foreach (var property in file.properties)
            {
                TreeNode node = new TreeNode(property.Item1);
                node.Tag = property;
                treeMeta.Nodes.Add(node);
            }
        }

        private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag == null ||
                !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
                return;
            PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;
            addMeta add = new addMeta();
            if (add.ShowDialog() == DialogResult.OK)
            {
                var property = new ValueTuple<string, string>(add.PropertyName, add.PropertyValue);
                file.properties.Add(property);
                ReloadMetaTreeView();
                saved = false;
            }
            add.Dispose();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode == null) return;

            //if (treeViewMain.SelectedNode.Tag is PCKFile.FileData)
            //{
            //	PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
            //	int file_index = currentPCK.Files.IndexOf(file);
            //	currentPCK.Files.Swap(file_index, file_index - 1);
            //	saved = false;
            //}
            //return;

            TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();

            if (treeViewMain.SelectedNode.Parent == null)
            {
                if (treeViewMain.SelectedNode.PrevNode == null) return;
                treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.PrevNode.Index, move);
                treeViewMain.SelectedNode.Remove();
            }
            else
            {
                if (treeViewMain.SelectedNode.PrevNode == null) return;
                treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.PrevNode.Index, move);
                //removes node because a clone was inserted into its new index
                treeViewMain.SelectedNode.Remove();
            }
            treeViewMain.SelectedNode = move;
            saved = false;
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode == null) return;

            //if (treeViewMain.SelectedNode.Tag is PCKFile.FileData)
            //{
            //    PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
            //    int file_index = currentPCK.Files.IndexOf(file);
            //    currentPCK.Files.Swap(file_index, file_index + 1);
            //    saved = false;
            //}
            //return;

            TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();
            if (treeViewMain.SelectedNode.Parent == null)
            {
                if (treeViewMain.SelectedNode.NextNode == null) return;
                treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.NextNode.Index + 1, move);
                //removes node because a clone was inserted into its new index
                treeViewMain.SelectedNode.Remove();
            }
            else
            {
                if (treeViewMain.SelectedNode.NextNode == null) return;
                treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.NextNode.Index + 1, move);
                //removes node because a clone was inserted into its new index
                treeViewMain.SelectedNode.Remove();
            }
            treeViewMain.SelectedNode = move;
        }

        #region drag and drop for main tree node

        public static void getChildren(List<TreeNode> Nodes, TreeNode Node)
        {
            foreach (TreeNode thisNode in Node.Nodes)
            {
                Nodes.Add(thisNode);
                getChildren(Nodes, thisNode);
            }
        }

        // Most of the code below is modified code from this link: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
        // - MattNL

        private void treeViewMain_ItemDrag(object sender, ItemDragEventArgs e)
        {

        }

        // Set the target drop effect to the effect 
        // specified in the ItemDrag event handler.
        private void treeViewMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        // Select the node under the mouse pointer to indicate the 
        // expected drop location.
        private void treeViewMain_DragOver(object sender, DragEventArgs e)
        {

        }

        private void treeViewMain_DragDrop(object sender, DragEventArgs e)
        {

        }

        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;
            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }

        #endregion

        private void metaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meta edit = new meta(currentPCK.GatherPropertiesList());
            edit.TopMost = true;
            edit.TopLevel = true;
            edit.Show();
        }

        private void addPresetToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!(treeViewMain.SelectedNode.Tag is PCKFile.FileData)) return;
            PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
            using presetMeta add = new presetMeta(file);
            if (add.ShowDialog() == DialogResult.OK)
            {
                ReloadMetaTreeView();
                saved = false;
            }
        }

        private void InitializeSkinPack(int packId, int packVersion, string packName)
        {
            currentPCK = new PCKFile(3);
            var zeroFile = new PCKFile.FileData("0", 4);
            zeroFile.properties.Add(("PACKID", packId.ToString()));
            zeroFile.properties.Add(("PACKVERSION", packVersion.ToString()));
            var loc = new PCKFile.FileData("localisation.loc", 6);
            var locFile = new LOCFile();
            locFile.InitializeDefault(packName);
            using (var stream = new MemoryStream())
            {
                LOCFileWriter.Write(stream, locFile);
                loc.SetData(stream.ToArray());
            }
            currentPCK.Files.Add(zeroFile);
            currentPCK.Files.Add(loc);
        }

        private void InitializeTexturePack(int packId, int packVersion, string packName)
        {
            InitializeSkinPack(packId, packVersion, packName);
            string res = "x16"; // TODO: add reselotions selection prompt
            var texturepackInfo = new PCKFile.FileData($"{res}/{res}Info.pck", 5);
            texturepackInfo.properties.Add(("PACKID", "0"));
            texturepackInfo.properties.Add(("DATAPATH", $"{res}Data.pck"));
            currentPCK.Files.Add(texturepackInfo);
        }

        private void InitializeMashUpPack(int packId, int packVersion, string packName)
        {
            InitializeTexturePack(packId, packVersion, packName);
            var gameRuleFile = new PCKFile.FileData("GameRules.grf", 7);
            var grfFile = new GRFFile();
            grfFile.AddTag("MapOptions",
                new KeyValuePair<string, string>("seed", "0"),
                new KeyValuePair<string, string>("baseSaveName", string.Empty),
                new KeyValuePair<string, string>("flatworld", "false"),
                new KeyValuePair<string, string>("texturePackId", packId.ToString())
                );
            grfFile.AddTag("LevelRules")
                .AddTag("UpdatePlayer",
                new KeyValuePair<string, string>("yRot", "0"),
                new KeyValuePair<string, string>("xRot", "0"),
                new KeyValuePair<string, string>("spawnX", "0"),
                new KeyValuePair<string, string>("spawnY", "0"),
                new KeyValuePair<string, string>("spawnZ", "0")
                );
            using (var stream = new MemoryStream())
            {
                GRFFileWriter.Write(stream, grfFile, GRFFile.eCompressionType.ZlibRleCrc);
                gameRuleFile.SetData(stream.ToArray());
            }
            currentPCK.Files.Add(gameRuleFile);
        }

        private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt namePrompt = new RenamePrompt("");
            namePrompt.OKButton.Text = "Ok";
            if (namePrompt.ShowDialog() == DialogResult.OK)
            {
                InitializeSkinPack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText);
                isTemplateFile = true;
                LoadEditorTab();
            }
        }
        private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt namePrompt = new RenamePrompt("");
            namePrompt.OKButton.Text = "Ok";
            if (namePrompt.ShowDialog() == DialogResult.OK)
            {
                InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText);
                isTemplateFile = true;
                LoadEditorTab();
            }
        }

        private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt namePrompt = new RenamePrompt("");
            namePrompt.OKButton.Text = "Ok";
            if (namePrompt.ShowDialog() == DialogResult.OK)
            {
                InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText);
                isTemplateFile = true;
                LoadEditorTab();
            }
        }

        private void advancedMetaAddingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens dialog for bulk minefile editing
            using AdvancedOptions advanced = new AdvancedOptions(currentPCK);
            if (advanced.ShowDialog() == DialogResult.OK)
                saved = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkSaveState();
            CloseEditorTab();
        }

        private void programInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using programInfo info = new programInfo();
            info.ShowDialog();
        }

        private void treeViewMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteFileToolStripMenuItem_Click(sender, e);
        }

        private void treeViewMain_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // for now name edits are done through the 'rename' context menu item
            // TODO: add folder renaming
            //e.CancelEdit = e.Node.Tag is PCKFile.FileData;
            e.CancelEdit = true;
        }

        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                //Extracts a chosen pck file to a chosen destincation
                using OpenFileDialog ofd = new OpenFileDialog();
                using FolderBrowserDialog sfd = new FolderBrowserDialog();
                ofd.CheckFileExists = true;
                ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

                if (ofd.ShowDialog() == DialogResult.OK && sfd.ShowDialog() == DialogResult.OK)
                {
                    PCKFile pckfile = null;
                    using (var fs = File.OpenRead(ofd.FileName))
                    {
                        try
                        {
                            pckfile = PCKFileReader.Read(fs, LittleEndianCheckBox.Checked);
                        }
                        catch (OverflowException ex)
                        {
                            MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (pckfile.HasFile("0", 4) &&
                        pckfile.GetFile("0", 4).properties.HasProperty("LOCK") &&
                        new pckLocked(pckfile.GetFile("0", 4).properties.GetProperty("LOCK").Item2).ShowDialog() != DialogResult.OK)
                        return; // cancel extraction if password not provided
                    foreach (PCKFile.FileData mf in pckfile.Files)
                    {
                        string filepath = $"{sfd.SelectedPath}/{mf.filepath}";
                        FileInfo file = new FileInfo(filepath);
                        file.Directory.Create();
                        File.WriteAllBytes(filepath, mf.data); // writes data to file
                        //attempts to generate reimportable metadata file out of minefiles metadata
                        string metaData = "";

                        foreach (var entry in mf.properties)
                        {
                            metaData += $"{entry.Item1}: {entry.Item2}{Environment.NewLine}";
                        }

                        File.WriteAllText(sfd.SelectedPath + @"\" + Path.GetFileNameWithoutExtension(mf.filepath) + ".properties", metaData);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An Error occured while extracting data");
            }
        }

        private void treeMeta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                deleteEntryToolStripMenuItem_Click(sender, e);
        }

        #region imports a folder of skins to pck
        private void importExtractedSkinsFolder(object sender, EventArgs e)
        {
            FolderBrowserDialog contents = new FolderBrowserDialog();//Creates folder browser instance

            if (contents.ShowDialog() == DialogResult.OK)
            {
                //checks to make sure selected path exist
                if (!Directory.Exists(contents.SelectedPath))
                {
                    MessageBox.Show("Directory Lost");
                    return;
                }

                string filepath = contents.SelectedPath;//sets filepath to selected path
                DirectoryInfo d = new DirectoryInfo(contents.SelectedPath);//sets directory info

                bool mashupStructure = false;//creates variable to indicate wether current pck skin structure is mashup or regular skin
                int skinsFolder = 0;//temporary index for skins folder for if structure is mashup

                //checks to see if pck contains a skins folder
                foreach (TreeNode item in treeViewMain.Nodes)
                {
                    if (item.Text == "Skins")
                    {
                        mashupStructure = true;//sets mashup structure to true
                        skinsFolder = item.Index;//keeps note of skins folder index
                    }
                }

                //gets all png files in selected path
                foreach (var file in d.GetFiles("*.png"))
                {
                    ListViewItem Import = new ListViewItem();//listviewitem to store temporary data
                    Import.Text = file.Name.Remove(file.Name.Length - 4, 4);//gets file name without extension

                    //sets minefile type based on wether cape or skin
                    int type = 0;
                    if (Import.Text.Remove(7, Import.Text.Length - 7) == "dlccape" || Import.Text.Remove(7, Import.Text.Length - 7) == "DLCCAPE")
                    {
                        type = 1;
                    }
                    PCKFile.FileData mfNew = new PCKFile.FileData("", type); //new minefile template
                    mfNew.SetData(File.ReadAllBytes(contents.SelectedPath + @"\" + file.Name.Remove(file.Name.Length - 4, 4) + ".png"));//sets minefile data to image data of current skin

                    TreeNode skin = new TreeNode(); //create template treenode for minefile

                    currentPCK.Files.Add(mfNew);//adds new minefile to minefile list for skin

                    //Sets minefile directory based on pcks structure/type
                    if (mashupStructure == true)
                    {
                        mfNew.filepath = "Skins/" + Import.Text + ".png";
                    }
                    else
                    {
                        mfNew.filepath = Import.Text + ".png";
                    }

                    skin.Text = Import.Text + ".png";//adds file extension to minefile
                    skin.Tag = mfNew;//sets nodes minefile data

                    //presest variables for minefile skin data about to be imported
                    string entryName = "";
                    string entryValue = "";
                    string locNameId = "";
                    string locName = "";
                    string locThemeId = "";
                    string locTheme = "";
                    bool entryStart = true;//assistant for parcing through metadata file data to import

                    foreach (char entry in File.ReadAllText(contents.SelectedPath + @"\" + Import.Text + ".png.txt").ToList())
                    {
                        //imports current skins metadata from metadata file
                        if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == true)
                        {
                            entryName += entry.ToString();
                        }
                        else if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == false)
                        {
                            entryValue += entry.ToString();
                        }
                        else if (entry.ToString() == ":" && entryStart == true)
                        {
                            entryStart = false;
                        }
                        else
                        {
                            //adds minefiles metadata and presets loc data for minefile
                            mfNew.properties.Add(new ValueTuple<string, string>(entryName, entryValue));

                            if (entryName == "DISPLAYNAMEID")
                            {
                                locNameId = entryValue;
                            }

                            if (entryName == "DISPLAYNAME")
                            {
                                locName = entryValue;
                            }

                            if (entryName == "THEMENAMEID")
                            {
                                locThemeId = entryValue;
                            }

                            if (entryName == "THEMENAME")
                            {
                                locTheme = entryValue;
                            }

                            //creates metadata id in loc file
                            if (locThemeId != "" && locTheme != "")
                            {
                                PCKFile.FileData locfile = currentPCK.GetFile("localisation.loc", 6);
                                if (locfile == null)
                                    locfile = currentPCK.GetFile("languages.loc", 6);
                                if (locfile == null)
                                    throw new Exception("counld not find .loc file");
                                LOCFile l = null;
                                try
                                {
                                    using (var stream = new MemoryStream(locfile.data))
                                    {
                                        l = LOCFileReader.Read(stream);//sets loc data
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                l.AddLocKey(locThemeId, locTheme);
                                using (var stream = new MemoryStream())
                                {
                                    LOCFileWriter.Write(stream, l);
                                    locfile.SetData(stream.ToArray());
                                }
                                locThemeId = "";
                                locTheme = "";
                            }
                            entryName = "";
                            entryValue = "";
                            entryStart = true;
                        }
                    }
                    //sets file icon
                    skin.ImageIndex = 2;
                    skin.SelectedImageIndex = 2;
                    //Adds new minefile node to a destination based on pcks skin structure type
                    if (mashupStructure == true)
                    {
                        treeViewMain.Nodes[skinsFolder].Nodes.Add(skin);
                    }
                    else
                    {
                        treeViewMain.Nodes.Add(skin);
                    }
                }
            }
            contents.Dispose(); //disposes temporary data
            saved = false;
        }
        #endregion

        private bool TryGetLocFile(out LOCFile locFile)
        {
            PCKFile.FileData locdata = null;
            if (currentPCK.HasFile("localisation.loc", 6))
                locdata = currentPCK.GetFile("localisation.loc", 6);
            else if (currentPCK.HasFile("languages.loc", 6))
                locdata = currentPCK.GetFile("languages.loc", 6);
            else
            {
                locFile = null;
                return false;
            }

            try
            {
                using (var stream = new MemoryStream(locdata.data))
                {
                    locFile = LOCFileReader.Read(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            locFile = null;
            return false;
        }

        private bool TrySetLocFile(in LOCFile locFile)
        {
            PCKFile.FileData locdata = null;
            if (currentPCK.HasFile("localisation.loc", 6))
                locdata = currentPCK.GetFile("localisation.loc", 6);
            else if (currentPCK.HasFile("languages.loc", 6))
                locdata = currentPCK.GetFile("languages.loc", 6);
            else
                return false;

            try
            {
                using (var stream = new MemoryStream())
                {
                    LOCFileWriter.Write(stream, locFile);
                    locdata.SetData(stream.ToArray());
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        private void importSkin(object sender, EventArgs e)
        {
            using (OpenFileDialog contents = new OpenFileDialog())
            {
                contents.Title = "Select Extracted Skin File";
                contents.Filter = "Skin File (*.png)|*.png";

                if (contents.ShowDialog() == DialogResult.OK)
                {
                    string skinNameImport = Path.GetFileName(contents.FileName);
                    byte[] data = File.ReadAllBytes(contents.FileName);
                    PCKFile.FileData mfNew = new PCKFile.FileData(skinNameImport, 0);
                    mfNew.SetData(data);
                    string propertyFile = Path.GetFileNameWithoutExtension(contents.FileName) + ".properties";
                    if (File.Exists(propertyFile))
                    {
                        string[] txtProperties = File.ReadAllLines(propertyFile);
                        if ((txtProperties.Contains("DISPLAYNAMEID") && txtProperties.Contains("DISPLAYNAME")) ||
                        (txtProperties.Contains("THEMENAMEID") && txtProperties.Contains("THEMENAME")) &&
                        TryGetLocFile(out LOCFile locFile))
                        {
                            // do stuff 
                            //l.AddLocKey(locThemeId, locTheme);
                            //using (var stream = new MemoryStream())
                            //{
                            //	LOCFileWriter.Write(stream, locFile);
                            //	locdata.SetData(stream.ToArray());
                            //}
                        }

                        try
                        {
                            foreach (string prop in txtProperties)
                            {
                                string[] arg = prop.Split(':');
                                if (arg.Length < 2) continue;
                                string key = arg[0];
                                string value = arg[1];
                                if (key == "DISPLNAMEID" || key == "THEMENAMEID")
                                {

                                }
                                mfNew.properties.Add(new ValueTuple<string, string>(key, value));
                            }
                            saved = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenamePrompt folderNamePrompt = new RenamePrompt("");
            folderNamePrompt.OKButton.Text = "Add";
            if (folderNamePrompt.ShowDialog() == DialogResult.OK)
            {
                TreeNode folerNode = CreateNode(folderNamePrompt.NewText);
                folerNode.ImageIndex = 0;
                folerNode.SelectedImageIndex = 0;
                TreeNode node = treeViewMain.SelectedNode;
                TreeNodeCollection nodeCollection = node != null &&
                    !(node.Tag is PCKFile.FileData)
                    ? node.Nodes : treeViewMain.Nodes;
                if (node.Tag is PCKFile.FileData && node.Parent != null)
                    nodeCollection = node.Parent.Nodes;
                nodeCollection.Add(folerNode);
            }
        }

        private void addPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!currentPCK.HasFile("0", 4)) throw new Exception("0 file not found");
            PCKFile.FileData file = currentPCK.GetFile("0", 4);
            if (checkForPassword())
            {
                AddPCKPassword add = new AddPCKPassword();
                if (add.ShowDialog() == DialogResult.OK)
                    file.properties.Add(("LOCK", add.Password));
                add.Dispose();
                ReloadMetaTreeView();
                saved = false;
            }
        }

        private void binkaConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
        }

        private void fAQToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(hosturl + "pckStudio#faq");
        }
        // BIG TODO
        #region converts and ports all skins in pck to mc bedrock format
        // items class for use in bedrock skin conversion
        public class Item
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private void convertToBedrockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (openedPCKS.Visible == true && MessageBox.Show("Convert " + openedPCKS.SelectedTab.Text + " to a Bedrock Edition format?", "Convert", MessageBoxButtons.YesNo, MessageBoxIcon.None) == DialogResult.Yes)
            //{
            //	try
            //	{
            //		string packName = openedPCKS.SelectedTab.Text.Remove(openedPCKS.SelectedTab.Text.Count() - 4, 4);//Determines skin packs name off of pck file name

            //		//Lets user choose were to put generated pack
            //		SaveFileDialog convert = new SaveFileDialog();
            //		convert.Filter = "PCK (Minecarft Bedrock DLC)|*.mcpack";
            //		convert.FileName = packName;

            //		if (convert.ShowDialog() == DialogResult.OK)
            //		{
            //			//creates directory for conversion
            //			string root = Path.GetDirectoryName(convert.FileName) + "\\" + packName;
            //			string rootFinal = Path.GetDirectoryName(convert.FileName) + "\\";

            //			//creates pack uuid off of the last skin id detected
            //			string uuid = "99999999"; //default

            //			//creates list of skin display names
            //			List<Item> skinDisplayNames = new List<Item>();

            //			//MessageBox.Show(root);//debug thingy to make sure filepath is correct

            //			//add all skins to a list
            //			List<PCKFile.FileData> skinsList = new List<PCKFile.FileData>();
            //			List<PCKFile.FileData> capesList = new List<PCKFile.FileData>();
            //			foreach (PCKFile.FileData skin in currentPCK.Files)
            //			{
            //				if (skin.name.Count() == 19)
            //				{
            //					if (skin.name.Remove(7, skin.name.Count() - 7) == "dlcskin")
            //					{
            //						skinsList.Add(skin);
            //						uuid = skin.name.Remove(12, 7);
            //						uuid = uuid.Remove(0, 7);
            //						uuid = "abcdefa" + uuid;
            //					}
            //					if (skin.name.Remove(7, skin.name.Count() - 7) == "dlccape")
            //					{
            //						capesList.Add(skin);
            //					}
            //				}
            //			}

            //			if (skinsList.Count() == 0)
            //			{
            //				MessageBox.Show("No skins were found");
            //				return;
            //			}

            //			Directory.CreateDirectory(root);//Creates directory for skin pack
            //			Directory.CreateDirectory(root + "/texts");//create directory for skin pack text files

            //			//create skins json file
            //			using (StreamWriter writeSkins = new StreamWriter(root + "/skins.json"))
            //			{
            //				writeSkins.WriteLine("{");
            //				writeSkins.WriteLine("  \"skins\": [");

            //				int skinAmount = 0;
            //				foreach (PCKFile.FileData newSkin in skinsList)
            //				{
            //					skinAmount += 1;
            //					string skinName = "skinName";
            //					string capePath = "";
            //					bool hasCape = false;

            //					foreach (var entry in newSkin.properties)
            //					{
            //						if (entry.Item1 == "DISPLAYNAME")
            //						{
            //							skinName = entry.Item2;
            //							skinDisplayNames.Add(new Item() { Id = newSkin.name.Remove(15, 4), Name = skinName });
            //						}
            //						if (entry.Item1 == "CAPEPATH")
            //						{
            //							hasCape = true;
            //							capePath = entry.Item2.ToString();
            //						}
            //					}

            //					writeSkins.WriteLine("    {");
            //					writeSkins.WriteLine("      \"localization_name\": " + "\"" + newSkin.name.Remove(15, 4) + "\",");

            //					MemoryStream png = new MemoryStream(newSkin.data); //Gets image data from minefile data
            //					Image skinPicture = Image.FromStream(png); //Constructs image data into image
            //					if (skinPicture.Height == skinPicture.Width)
            //					{
            //						writeSkins.WriteLine("      \"geometry\": \"geometry." + packName + "." + newSkin.name.Remove(15, 4) + "\",");
            //					}
            //					writeSkins.WriteLine("      \"texture\": " + "\"" + newSkin.name + "\",");
            //					if (hasCape == true)
            //					{
            //						writeSkins.WriteLine("      \"cape\":" + "\"" + capePath + "\",");
            //					}
            //					writeSkins.WriteLine("      \"type\": \"free\"");
            //					if (skinAmount != skinsList.Count)
            //					{
            //						writeSkins.WriteLine("    },");
            //					}
            //					else
            //					{
            //						writeSkins.WriteLine("    }");
            //					}
            //				}

            //				writeSkins.WriteLine("  ],");
            //				writeSkins.WriteLine("  \"serialize_name\": \"" + packName + "\",");
            //				writeSkins.WriteLine("  \"localization_name\": \"" + packName + "\"");
            //				writeSkins.WriteLine("}");
            //			}

            //			//Create geometry file
            //			using (StreamWriter writeSkins = new StreamWriter(root + "/geometry.json"))
            //			{
            //				writeSkins.WriteLine("{");
            //				int newSkinCount = 0;
            //				foreach (PCKFile.FileData newSkin in skinsList)
            //				{

            //					newSkinCount += 1;
            //					string skinType = "steve";
            //					MemoryStream png = new MemoryStream(newSkin.data); //Gets image data from minefile data
            //					Image skinPicture = Image.FromStream(png); //Constructs image data into image

            //					if (skinPicture.Height == skinPicture.Width / 2)
            //					{
            //						skinType = "64x32";
            //						continue;
            //					}

            //					double offsetHead = 0;
            //					double offsetBody = 0;
            //					double offsetArms = 0;
            //					double offsetLegs = 0;

            //					//creates list of skin model data
            //					List<Item> modelDataHead = new List<Item>();
            //					List<Item> modelDataBody = new List<Item>();
            //					List<Item> modelDataLeftArm = new List<Item>();
            //					List<Item> modelDataRightArm = new List<Item>();
            //					List<Item> modelDataLeftLeg = new List<Item>();
            //					List<Item> modelDataRightLeg = new List<Item>();
            //					List<Item> modelData = new List<Item>();


            //					if (skinPicture.Height == skinPicture.Width)
            //					{
            //						//determines skin type based on image dimensions, existence of BOX tags, and the ANIM value
            //						foreach (var entry in newSkin.properties)
            //						{
            //							if (entry.Item1 == "BOX")
            //							{
            //								string mClass = "";
            //								string mData = "";
            //								foreach (char dCheck in entry.Item2)
            //								{
            //									if (dCheck.ToString() != " ")
            //									{
            //										mClass += dCheck.ToString();
            //									}
            //									else
            //									{
            //										mData = entry.Item2.Remove(0, mClass.Count() + 1);
            //										break;
            //									}
            //								}

            //								if (mClass == "HEAD")
            //								{
            //									mClass = "head";
            //									modelDataHead.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //								else if (mClass == "BODY")
            //								{
            //									mClass = "body";
            //									modelDataBody.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //								else if (mClass == "ARM0")
            //								{
            //									mClass = "rightArm";
            //									modelDataRightArm.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //								else if (mClass == "ARM1")
            //								{
            //									mClass = "leftArm";
            //									modelDataLeftArm.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //								else if (mClass == "LEG0")
            //								{
            //									mClass = "leftLeg";
            //									modelDataLeftLeg.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //								else if (mClass == "LEG1")
            //								{
            //									mClass = "rightLeg";
            //									modelDataRightLeg.Add(new Item() { Id = mClass, Name = mData });
            //								}
            //							}

            //							if (entry.Item1 == "OFFSET")
            //							{
            //								string oClass = "";
            //								string oData = "";
            //								foreach (char oCheck in entry.Item2.ToString())
            //								{
            //									oData = entry.Item2.ToString();
            //									if (oCheck.ToString() != " ")
            //									{
            //										oClass += oCheck.ToString();
            //									}
            //									else
            //									{
            //										break;
            //									}

            //									if (oClass == "HEAD")
            //									{
            //										offsetHead += Double.Parse(oData.Remove(0, 7)) * -1;
            //									}
            //									else if (oClass == "BODY")
            //									{
            //										offsetBody += Double.Parse(oData.Remove(0, 7)) * -1;
            //									}
            //									else if (oClass == "ARM0")
            //									{
            //										offsetArms += Double.Parse(oData.Remove(0, 7)) * -1;
            //									}
            //									else if (oClass == "LEG0")
            //									{
            //										offsetLegs += Double.Parse(oData.Remove(0, 7)) * -1;
            //									}
            //								}
            //							}

            //							if (entry.Item1 == "ANIM")
            //							{
            //								if (entry.Item2 == "0x40000")
            //								{

            //								}
            //								else if (entry.Item2 == "0x80000")
            //								{
            //									skinType = "alex";
            //								}
            //							}
            //						}

            //						if (modelDataHead.Count + modelDataBody.Count + modelDataLeftArm.Count + modelDataRightArm.Count + modelDataLeftLeg.Count + modelDataRightLeg.Count > 0)
            //						{
            //							skinType = "custom";
            //						}
            //					}

            //					writeSkins.WriteLine("  \"" + "geometry." + packName + "." + newSkin.name.Remove(15, 4) + "\": {");

            //					//makes skin model depending on what skin type the skin is
            //					if (skinType == "custom")
            //					{
            //						writeSkins.WriteLine("    \"bones\": [");

            //						//Head Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ 0, 24, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each head box
            //						int modelAmount = 0;
            //						foreach (Item model in modelDataHead)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}

            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetHead + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + (Double.Parse(ys)) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A HEAD BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataHead.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "clothing" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "head" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        },");


            //						//Body Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ 0, 12, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each body box
            //						modelAmount = 0;
            //						foreach (Item model in modelDataBody)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}
            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetBody + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A BODY BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataBody.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "body" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        },");


            //						//LeftArm Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ 5, 22, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each arm1 box
            //						modelAmount = 0;
            //						foreach (Item model in modelDataLeftArm)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}
            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A ARM0 BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataLeftArm.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "leftArm" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        },");

            //						//RightArm Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ -5, 22, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each arm0 box
            //						modelAmount = 0;
            //						foreach (Item model in modelDataRightArm)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}
            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A ARM1 BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataRightArm.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "rightArm" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        },");

            //						//LeftLeg Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ 1.9, 12, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each leg1 box
            //						modelAmount = 0;
            //						foreach (Item model in modelDataLeftLeg)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}
            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A LEG1 BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataLeftLeg.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "leftLeg" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        },");

            //						//RightLeg Data
            //						writeSkins.WriteLine("      {");
            //						writeSkins.WriteLine("        \"pivot\": [ -1.9, 12, 0 ],");
            //						writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
            //						writeSkins.WriteLine("          \"cubes\": [ ");
            //						//Creates bones for each leg0 box
            //						modelAmount = 0;
            //						foreach (Item model in modelDataRightLeg)
            //						{
            //							modelAmount += 1;

            //							string xo = "";
            //							string yo = "";
            //							string zo = "";
            //							string xs = "";
            //							string ys = "";
            //							string zs = "";
            //							string xv = "";
            //							string yv = "";

            //							int spaceCheck = 0;

            //							foreach (char value in model.Name.ToString())
            //							{
            //								//0X1Y2Z3X4Y5Z6X7Y
            //								if (value.ToString() != " " && spaceCheck == 0)
            //								{
            //									xo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 1)
            //								{
            //									yo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 2)
            //								{
            //									zo += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 3)
            //								{
            //									xs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 4)
            //								{
            //									ys += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 5)
            //								{
            //									zs += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 6)
            //								{
            //									xv += value.ToString();
            //								}
            //								else if (value.ToString() != " " && spaceCheck == 7)
            //								{
            //									yv += value.ToString();
            //								}
            //								else if (value.ToString() == " ")
            //								{
            //									spaceCheck += 1;
            //								}
            //							}
            //							writeSkins.WriteLine("           {");
            //							try
            //							{
            //								writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
            //								writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
            //								writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
            //								writeSkins.WriteLine("            \"inflate\": 0,");
            //								writeSkins.WriteLine("            \"mirror\": false");
            //							}
            //							catch (Exception)
            //							{
            //								MessageBox.Show("A LEG0 BOX tag in " + newSkin.name + " has an invalid value!");
            //							}
            //							if (modelAmount != modelDataRightLeg.Count)
            //							{
            //								writeSkins.WriteLine("    },");
            //							}
            //							else
            //							{
            //								writeSkins.WriteLine("    }");
            //							}
            //						}
            //						writeSkins.WriteLine("        ],");
            //						writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
            //						writeSkins.WriteLine("        \"name\": \"" + "rightLeg" + "\",");
            //						writeSkins.WriteLine("        \"parent\":" + " null");
            //						writeSkins.WriteLine("        }");
            //						writeSkins.WriteLine("    ],");
            //					}
            //					else if (skinType == "64x32")
            //					{
            //						writeSkins.Write("    \"bones\": [ ],");
            //					}
            //					else if (skinType == "steve")
            //					{
            //						writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
            //					}
            //					else if (skinType == "alex")
            //					{
            //						writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
            //					}


            //					writeSkins.WriteLine("    \"texturewidth\": 64 , ");
            //					writeSkins.WriteLine("    \"textureheight\": 64,");
            //					writeSkins.WriteLine("    \"META_ModelVersion\": \"1.0.6\",");
            //					writeSkins.WriteLine("    \"rigtype\": \"normal\",");
            //					writeSkins.WriteLine("    \"animationArmsDown\": false,");
            //					writeSkins.WriteLine("    \"animationArmsOutFront\": false,");
            //					writeSkins.WriteLine("    \"animationStatueOfLibertyArms\": false,");
            //					writeSkins.WriteLine("    \"animationSingleArmAnimation\": false,");
            //					writeSkins.WriteLine("    \"animationStationaryLegs\": false,");
            //					writeSkins.WriteLine("    \"animationSingleLegAnimation\": false,");
            //					writeSkins.WriteLine("    \"animationNoHeadBob\": false,");
            //					writeSkins.WriteLine("    \"animationDontShowArmor\": false,");
            //					writeSkins.WriteLine("    \"animationUpsideDown\": false,");
            //					writeSkins.WriteLine("    \"animationInvertedCrouch\": false");
            //					if (newSkinCount != skinsList.Count)
            //					{
            //						writeSkins.WriteLine("  },");
            //					}
            //					else
            //					{
            //						writeSkins.WriteLine("  }");
            //					}
            //				}
            //				Console.WriteLine(writeSkins);
            //			}
            //			Random rnd = new Random();
            //			int month = rnd.Next(1, 13); // creates a number between 1 and 12
            //			int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            //			int card = rnd.Next(52);

            //			string randomPlus = month.ToString() + dice.ToString() + card.ToString();
            //			if (randomPlus.Count() > 12)
            //			{
            //				randomPlus.Remove(0, randomPlus.Count() - 12);
            //			}
            //			else if (randomPlus.Count() < 12)
            //			{
            //				int ii = 12 - randomPlus.Count();
            //				for (int i = 0; i < ii; i++)
            //				{
            //					randomPlus += 0;
            //				}
            //			}
            //			else if (randomPlus.Count() == 12)
            //			{
            //			}

            //			//Create Manifest file
            //			using (StreamWriter writeSkins = new StreamWriter(root + "/manifest.json"))
            //			{
            //				writeSkins.WriteLine("{");
            //				writeSkins.WriteLine("  \"header\": {");
            //				writeSkins.WriteLine("    \"version\": [");
            //				writeSkins.WriteLine("      1,");
            //				writeSkins.WriteLine("      0,");
            //				writeSkins.WriteLine("      0");
            //				writeSkins.WriteLine("    ],");
            //				writeSkins.WriteLine("    \"description\": \"Template by Ultmate_Mario, Conversion by Nobledez\",");
            //				writeSkins.WriteLine("    \"name\": \"" + packName + "\",");
            //				writeSkins.WriteLine("    \"uuid\": \"" + uuid.Remove(0, 4) + "-" + uuid.Remove(0, 8) + "-" + uuid.Remove(1, 8) + "-" + uuid.Remove(2, 8) + "-" + randomPlus + "\""); //8-4-4-4-12
            //				writeSkins.WriteLine("  },");
            //				writeSkins.WriteLine("  \"modules\": [");
            //				writeSkins.WriteLine("    {");
            //				writeSkins.WriteLine("      \"version\": [");
            //				writeSkins.WriteLine("        1,");
            //				writeSkins.WriteLine("        0,");
            //				writeSkins.WriteLine("        0");
            //				writeSkins.WriteLine("      ],");
            //				writeSkins.WriteLine("      \"type\": \"skin_pack\",");
            //				writeSkins.WriteLine("      \"uuid\": \"8dfd1d65-b3ca-4726-b9e0-9b46a40b72a4\"");
            //				writeSkins.WriteLine("    }");
            //				writeSkins.WriteLine("  ],");
            //				writeSkins.WriteLine("  \"format_version\": 1");
            //				writeSkins.WriteLine("}");
            //			}

            //			//create lang file
            //			using (StreamWriter writeSkins = new StreamWriter(root + "/texts/en_US.lang"))
            //			{
            //				writeSkins.WriteLine("skinpack." + packName + "=" + Path.GetFileNameWithoutExtension(convert.FileName));
            //				foreach (Item displayName in skinDisplayNames)
            //				{
            //					writeSkins.WriteLine("skin." + packName + "." + displayName.Id + "=" + displayName.Name);
            //				}
            //			}

            //			//adds skin textures
            //			foreach (PCKFile.FileData skinTexture in skinsList)
            //			{
            //				var ms = new MemoryStream(skinTexture.data);
            //				Bitmap saveSkin = new Bitmap(Image.FromStream(ms));
            //				if (saveSkin.Width == saveSkin.Height)
            //				{
            //					ResizeImage(saveSkin, 64, 64);
            //				}
            //				else if (saveSkin.Height == saveSkin.Width / 2)
            //				{
            //					ResizeImage(saveSkin, 64, 32);
            //				}
            //				else
            //				{
            //					ResizeImage(saveSkin, 64, 64);
            //				}
            //				saveSkin.Save(root + "/" + skinTexture.name, ImageFormat.Png);
            //			}

            //			//adds cape textures
            //			foreach (PCKFile.FileData capeTexture in capesList)
            //			{
            //				File.WriteAllBytes(root + "/" + capeTexture.name, capeTexture.data);
            //			}

            //			string startPath = root;
            //			string zipPath = rootFinal + "content.zipe";

            //			try
            //			{
            //				ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
            //			}catch (Exception)
            //			{
            //				File.Delete(zipPath);
            //				ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
            //			}

            //			rootFinal = root + "temp/";
            //			Directory.CreateDirectory(rootFinal);
            //			File.Move(zipPath, rootFinal + "content.zipe");
            //			File.Copy(root + "/manifest.json", rootFinal + "/manifest.json");
            //			ZipFile.CreateFromDirectory(rootFinal, convert.FileName);//Creates mcpack
            //			Directory.Delete(root, true);
            //			Directory.Delete(rootFinal, true);

            //			MessageBox.Show("Conversion Complete");
            //		}
            //	}
            //	catch (Exception convertEr)
            //	{
            //		MessageBox.Show(convertEr.ToString());
            //	}
            //}
            //else if (openedPCKS.Visible == false)
            //{
            //	MessageBox.Show("Open PCK file first!");
            //}
        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion

        #region 3ds feature in testing

        private struct loadedTexture
        {
            public bool modified;
            public uint gpuCommandsOffset;
            public uint gpuCommandsWordCount;
            public uint offset;
            public int length;
            public RenderBase.OTexture texture;
        }

        private struct loadedMaterial
        {
            public string texture0;
            public string texture1;
            public string texture2;
            public uint gpuCommandsOffset;
            public uint gpuCommandsWordCount;
        }

        private class loadedBCH
        {
            public uint mainHeaderOffset;
            public uint gpuCommandsOffset;
            public uint dataOffset;
            public uint relocationTableOffset;
            public uint relocationTableLength;
            public List<loadedTexture> textures;
            public List<loadedMaterial> materials;

            public loadedBCH()
            {
                textures = new List<loadedTexture>();
                materials = new List<loadedMaterial>();
            }
        }

        private byte[] align(byte[] input)
        {
            int length = input.Length;
            while ((length & 0x7f) > 0) length++;
            byte[] output = new byte[length];
            Buffer.BlockCopy(input, 0, output, 0, input.Length);
            return output;
        }

        private void replaceData(Stream data, uint offset, int length, byte[] newData)
        {
            data.Seek(offset + length, SeekOrigin.Begin);
            byte[] after = new byte[data.Length - data.Position];
            data.Read(after, 0, after.Length);
            data.SetLength(offset);
            data.Seek(offset, SeekOrigin.Begin);
            data.Write(newData, 0, newData.Length);
            data.Write(after, 0, after.Length);
        }

        private void updateTexture(int index, loadedTexture newTex)
        {
            bch.textures.RemoveAt(index);
            bch.textures.Insert(index, newTex);
        }

        private void replaceCommand(Stream data, BinaryWriter output, uint newVal)
        {
            data.Seek(-8, SeekOrigin.Current);
            output.Write(newVal);
            data.Seek(4, SeekOrigin.Current);
        }

        private void updateAddress(Stream data, BinaryReader input, BinaryWriter output, int diff)
        {
            uint offset = input.ReadUInt32();
            offset = (uint)(offset + diff);
            data.Seek(-4, SeekOrigin.Current);
            output.Write(offset);
        }

        loadedBCH bch;

        private void create3dstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode != null)
            {
                loadedTexture tex = new loadedTexture();

                SaveFileDialog exportDs = new SaveFileDialog();
                exportDs.ShowDialog();
                string currentFile = exportDs.FileName;

                bch = new loadedBCH();

                using (FileStream data = new FileStream(currentFile, FileMode.Open))
                {
                    BinaryReader input = new BinaryReader(data);
                    BinaryWriter output = new BinaryWriter(data);

                    MemoryStream png = new MemoryStream(((PCKFile.FileData)(treeViewMain.SelectedNode.Tag)).data); //Gets image data from minefile data
                    Image skinPicture = Image.FromStream(png); //Constructs image data into image
                    pictureBoxImagePreview.Image = skinPicture; //Sets image preview to image

                    byte[] buffer = new byte[skinPicture.Width * skinPicture.Height * 4];
                    input.Read(buffer, 0, buffer.Length);
                    Bitmap texture = TextureCodec.decode(buffer, skinPicture.Width, skinPicture.Height, RenderBase.OTextureFormat.rgba8);
                    tex.texture = new RenderBase.OTexture(texture, "Texure");

                    //tex.texture = treeViewMain.SelectedNode.Tag;

                    for (int i = 0; i < bch.textures.Count; i++)
                    {
                        tex = bch.textures[i];
                        tex.modified = true;

                        if (tex.modified)
                        {
                            byte[] bufferx = align(TextureCodec.encode(tex.texture.texture, RenderBase.OTextureFormat.rgba8));
                            int diff = bufferx.Length - tex.length;

                            replaceData(data, tex.offset, tex.length, bufferx);

                            //Update offsets of next textures
                            tex.length = bufferx.Length;
                            tex.modified = false;
                            updateTexture(i, tex);
                            for (int j = i; j < bch.textures.Count; j++)
                            {
                                loadedTexture next = bch.textures[j];
                                next.offset = (uint)(next.offset + diff);
                                updateTexture(j, next);
                            }

                            //Update all addresses poiting after the replaced data
                            bch.relocationTableOffset = (uint)(bch.relocationTableOffset + diff);
                            for (int index = 0; index < bch.relocationTableLength; index += 4)
                            {
                                data.Seek(bch.relocationTableOffset + index, SeekOrigin.Begin);
                                uint value = input.ReadUInt32();
                                uint offset = value & 0x1ffffff;
                                byte flags = (byte)(value >> 25);

                                if ((flags & 0x20) > 0 || flags == 7 || flags == 0xc)
                                {
                                    if ((flags & 0x20) > 0)
                                        data.Seek((offset * 4) + bch.gpuCommandsOffset, SeekOrigin.Begin);
                                    else
                                        data.Seek((offset * 4) + bch.mainHeaderOffset, SeekOrigin.Begin);

                                    uint address = input.ReadUInt32();
                                    if (address + bch.dataOffset > tex.offset)
                                    {
                                        address = (uint)(address + diff);
                                        data.Seek(-4, SeekOrigin.Current);
                                        output.Write(address);
                                    }
                                }
                            }

                            uint newSize = (uint)((tex.texture.texture.Width << 16) | tex.texture.texture.Height);

                            //Update texture format
                            data.Seek(tex.gpuCommandsOffset, SeekOrigin.Begin);
                            for (int index = 0; index < tex.gpuCommandsWordCount * 3; index++)
                            {
                                uint command = input.ReadUInt32();

                                switch (command)
                                {
                                    case 0xf008e:
                                    case 0xf0096:
                                    case 0xf009e:
                                        replaceCommand(data, output, 0); //Set texture format to 0 = RGBA8888
                                        break;
                                    case 0xf0082:
                                    case 0xf0092:
                                    case 0xf009a:
                                        replaceCommand(data, output, newSize); //Set new texture size
                                        break;
                                }
                            }

                            //Update material texture format
                            foreach (loadedMaterial mat in bch.materials)
                            {
                                data.Seek(mat.gpuCommandsOffset, SeekOrigin.Begin);
                                for (int index = 0; index < mat.gpuCommandsWordCount; index++)
                                {
                                    uint command = input.ReadUInt32();

                                    switch (command)
                                    {
                                        case 0xf008e: if (mat.texture0 == tex.texture.name || mat.texture0 == "") replaceCommand(data, output, 0); break;
                                        case 0xf0096: if (mat.texture1 == tex.texture.name || mat.texture1 == "") replaceCommand(data, output, 0); break;
                                        case 0xf009e: if (mat.texture2 == tex.texture.name || mat.texture2 == "") replaceCommand(data, output, 0); break;
                                    }
                                }
                            }

                            //Patch up BCH header for new offsets and lengths
                            data.Seek(4, SeekOrigin.Begin);
                            byte backwardCompatibility = input.ReadByte();
                            byte forwardCompatibility = input.ReadByte();

                            //Update Data Extended and Relocation Table offsets
                            data.Seek(18, SeekOrigin.Current);
                            if (backwardCompatibility > 0x20) updateAddress(data, input, output, diff);
                            updateAddress(data, input, output, diff);

                            //Update data length
                            data.Seek(12, SeekOrigin.Current);
                            updateAddress(data, input, output, diff);
                        }
                    }
                    using (Stream file = File.Create(currentFile + ".tmp"))
                    {
                        CopyStream(output.BaseStream, file);
                    }

                }

                MessageBox.Show("Done!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        #endregion

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DateTime Begin = DateTime.Now;
            //pckCenter open = new pckCenter();
            Forms.Utilities.PckCenterBeta open = new Forms.Utilities.PckCenterBeta();
            open.Show();
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
        }

        private void wiiUPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            installWiiU install = new installWiiU(null);
            install.ShowDialog();
        }

        private void howToMakeABasicSkinPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/watch?v=A43aHRHkKxk");
        }

        private void howToMakeACustomSkinModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/watch?v=pEC_ug55lag");
        }

        private void howToMakeCustomSkinModelsbedrockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/watch?v=6z8NTogw5x4");
		}

		private void howToMakeCustomMusicToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
		}

		private void howToInstallPcksDirectlyToWiiUToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=hRQagnEplec");
		}

		private void pCKCenterReleaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=E_6bXSh6yqw");
		}

		private void howPCKsWorkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=hTlImrRrCKQ");
		}

		private void PS3PCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			installPS3 install = new installPS3(null);
			install.ShowDialog();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Pref setting = new Pref();
			setting.Show();
		}

		private void administrativeToolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PCK_Manager pckm = new PCK_Manager();
			pckm.Show();
		}

		private void VitaPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{

			installVita install = new installVita(null);
			install.ShowDialog();
		}

		private void toPhoenixARCDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://cash.app/$PhoenixARC");
		}

		private void toNobledezJackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.me/realnobledez");
		}

		private void joinDevelopmentDiscordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/aJtZNFVQTv");
		}

		private void convertPCTextrurePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TextureConverterUtility tex = new TextureConverterUtility(treeViewMain, currentPCK);
			tex.ShowDialog();
		}


		private void buttonEditModel_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode == null ||
				treeViewMain.SelectedNode.Tag == null ||
				!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
				return;
			PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
			if (file.type == 0 || file.type == 1 || file.type == 2)
			{
				if (buttonEdit.Text == "EDIT BOXES")
					editModel(file);
				else if (buttonEdit.Text == "View Skin")
				{
					using (var ms = new MemoryStream(file.data))
					{
						SkinPreview frm = new SkinPreview(Image.FromStream(ms));
						frm.ShowDialog(this);
						frm.Dispose();
					}
				}
			}

			//Check for Animated Texture
			if (file.filepath.StartsWith("res/textures/blocks/") || file.filepath.StartsWith("res/textures/items/"))
			{
				try
				{
					AnimationEditor diag = new AnimationEditor(file);
					diag.ShowDialog(this);
					diag.Dispose();
					ReloadMetaTreeView();
				}
				catch
				{
					MessageBox.Show("Invalid animation data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			if (Path.GetFileName(file.filepath) == "audio.pck")
			{
				try
				{
					if (!TryGetLocFile(out LOCFile locFile))
						throw new Exception("No .loc File found.");
					AudioEditor diag = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
					diag.ShowDialog(this);
					diag.Dispose();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK,
					MessageBoxIcon.Error);
					return;
				}
			}

			if (file.type == 6 && (file.filepath == "languages.loc" || file.filepath == "localisation.loc"))
			{
				LOCFile locFile = null;
				using (var stream = new MemoryStream(file.data))
				{
					locFile = LOCFileReader.Read(stream);
				}
				var locEditor = new LOCEditor(locFile);
				locEditor.ShowDialog();
				using (var stream = new MemoryStream())
                {
					LOCFileWriter.Write(stream, locFile);
					file.SetData(stream.ToArray());
                }
			}

			// Checks to see if selected minefile is a col file
			if (file.type == 9 && file.filepath == "colours.col") // .col file
			{
				COLFile colFile = new COLFile();
				using (var stream = new MemoryStream(file.data))
				{
					colFile.Open(stream);
				}
                using (COLEditor diag = new COLEditor(colFile))
                    if (diag.ShowDialog(this) == DialogResult.OK && diag.data.Length > 0)
                        file.SetData(diag.data);
			}
		}

		private void OpenPck_MouseEnter(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckOpen;
		}

		private void OpenPck_MouseLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (needsUpdate && File.Exists(Program.Appdata + @"\nobleUpdater.exe"))
			{
				Process.Start(Program.Appdata + @"\nobleUpdater.exe"); // starts updater
				Application.Exit(); // closes PCK Studio to let updatear finish the job
			}
		}

		private void checkSaveState()
        {
			if ((!saved || isTemplateFile) &&
				MessageBox.Show("Save PCK?", "Unsaved PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				if (isTemplateFile || string.IsNullOrEmpty(saveLocation))
				{
					SaveTemplate();
					return;
				}
				Save(saveLocation);
			}
		}

		private void OpenPck_DragEnter(object sender, DragEventArgs e)
		{
			pckOpen.Image = Resources.pckDrop;
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
			{
				var ext = Path.GetExtension(file);
				if (ext.Equals(".pck", StringComparison.CurrentCultureIgnoreCase))
				e.Effect = DragDropEffects.Copy;
				return;
			}
		}

		private void OpenPck_DragDrop(object sender, DragEventArgs e)
		{
			string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (FileList.Length > 1)
				MessageBox.Show("Only one pck file at a time is currently supported");
			currentPCK = openPck(FileList[0]);
            if (addPasswordToolStripMenuItem.Enabled = checkForPassword())
            {
                LoadEditorTab();
            }
		}

		private void OpenPck_DragLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void savePCK(object sender, EventArgs e)
		{
            if (!string.IsNullOrEmpty(saveLocation))
			    Save(saveLocation);
		}

		private void saveAsPCK(object sender, EventArgs e)
		{
			SaveTemplate();
		}

		private void forMattNLContributorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/mattnl");
		}

		private void setFileType_Click(object sender, EventArgs e, int type)
		{
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null || node.Tag == null || !(node.Tag is PCKFile.FileData)) return;
            var file = node.Tag as PCKFile.FileData;
            Console.WriteLine($"Setting {file.type} to {type}");
            file.type = type;
        }
    }
}