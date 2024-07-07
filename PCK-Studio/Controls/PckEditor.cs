using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using PckStudio.Extensions;
using PckStudio.Forms;
using PckStudio.Forms.Editor;
using PckStudio.Interfaces;
using PckStudio.Internal;
using PckStudio.Popups;
using PckStudio.Properties;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Internal.IO._3DST;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Internal.IO.PckAudio;
using PckStudio.Internal.Misc;

using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using OMI.Workers.Pck;
using PckStudio.FileFormats;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;
using OMI.Formats.GameRule;
using OMI.Workers.GameRule;
using OMI.Formats.Model;
using OMI.Workers.Model;
using OMI.Workers;
using OMI.Formats.Material;
using OMI.Workers.Material;
using OMI.Formats.Behaviour;
using OMI.Workers.Behaviour;
using PckStudio.Internal.Json;

namespace PckStudio.Controls
{
    public partial class PckEditor : UserControl, IEditor<PckFile>
    {
        public PckFile Value => _pck;

        private PckFile _pck;
        private string _location = string.Empty;
        bool __modified = false;
        bool _wasModified
        {
            get => __modified;
            set
            {
                if (__modified == value)
                    return;
                __modified = value;
                pckFileLabel.Text = !pckFileLabel.Text.StartsWith("*") && __modified ? "*" + pckFileLabel.Text : pckFileLabel.Text.Substring(1);
            }
        }

        private bool _isTemplateFile = false;
        private int _timesSaved = 0;

        private readonly Dictionary<PckAssetType, Action<PckAsset>> pckFileTypeHandler;

        public PckEditor()
        {
            InitializeComponent();

            skinToolStripMenuItem1.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.SkinFile);
            capeToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.CapeFile);
            textureToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.TextureFile);
            languagesFileLOCToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.LocalisationFile);
            gameRulesFileGRFToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.GameRulesFile);
            audioPCKFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.AudioFile);
            coloursCOLFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.ColourTableFile);
            gameRulesHeaderGRHToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.GameRulesHeader);
            skinsPCKToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.SkinDataFile);
            modelsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.ModelsFile);
            behavioursFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.BehavioursFile);
            entityMaterialsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckAssetType.MaterialFile);

            imageList.Images.Add(Resources.ZZFolder); // Icon for folders
            imageList.Images.Add(Resources.BINKA_ICON); // Icon for music cue file (audio.pck)
            imageList.Images.Add(Resources.IMAGE_ICON); // Icon for images (unused for now)
            imageList.Images.Add(Resources.LOC_ICON); // Icon for string localization files (languages.loc;localisation.loc)
            imageList.Images.Add(Resources.PCK_ICON); // Icon for generic PCK files (*.pck)
            imageList.Images.Add(Resources.ZUnknown); // Icon for Unknown formats
            imageList.Images.Add(Resources.COL_ICON); // Icon for color palette files (colours.col)
            imageList.Images.Add(Resources.SKINS_ICON); // Icon for Skin.pck archives (skins.pck)
            imageList.Images.Add(Resources.MODELS_ICON); // Icon for Model files (models.bin)
            imageList.Images.Add(Resources.GRF_ICON); // Icon for Game Rule files (*.grf)
            imageList.Images.Add(Resources.GRH_ICON); // Icon for Game Rule Header files (*.grh)
            imageList.Images.Add(Resources.INFO_ICON); // Icon for Info files (0)
            imageList.Images.Add(Resources.SKIN_ICON); // Icon for Skin files (*.png)
            imageList.Images.Add(Resources.CAPE_ICON); // Icon for Cape files (*.png)
            imageList.Images.Add(Resources.TEXTURE_ICON); // Icon for Texture files (*.png;*.tga)
            imageList.Images.Add(Resources.BEHAVIOURS_ICON); // Icon for Behaviour files (behaviours.bin)
            imageList.Images.Add(Resources.ENTITY_MATERIALS_ICON); // Icon for Entity Material files (entityMaterials.bin)

            pckFileTypeHandler = new Dictionary<PckAssetType, Action<PckAsset>>(15)
            {
                [PckAssetType.SkinFile] = HandleSkinFile,
                [PckAssetType.CapeFile] = null,
                [PckAssetType.TextureFile] = HandleTextureFile,
                [PckAssetType.UIDataFile] = _ => throw new NotSupportedException("unused in-game"),
                [PckAssetType.InfoFile] = null,
                [PckAssetType.TexturePackInfoFile] = HandleInnerPckFile,
                [PckAssetType.LocalisationFile] = HandleLocalisationFile,
                [PckAssetType.GameRulesFile] = HandleGameRuleFile,
                [PckAssetType.AudioFile] = HandleAudioFile,
                [PckAssetType.ColourTableFile] = HandleColourFile,
                [PckAssetType.GameRulesHeader] = HandleGameRuleFile,
                [PckAssetType.SkinDataFile] = HandleInnerPckFile,
                [PckAssetType.ModelsFile] = HandleModelsFile,
                [PckAssetType.BehavioursFile] = HandleBehavioursFile,
                [PckAssetType.MaterialFile] = HandleMaterialFile,
            };
        }

        private void HandleInnerPckFile(PckAsset file)
        {
            if (Settings.Default.LoadSubPcks &&
                (file.Type == PckAssetType.SkinDataFile || file.Type == PckAssetType.TexturePackInfoFile) &&
                file.Size > 0 && treeViewMain.SelectedNode.Nodes.Count == 0)
            {
                try
                {
                    var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    PckFile subPCKfile = file.GetData(reader);
                    BuildPckTreeView(treeViewMain.SelectedNode.Nodes, subPCKfile);
                    treeViewMain.SelectedNode.ExpandAll();

                }
                catch (OverflowException ex)
                {
                    MessageBox.Show("Failed to open pck\n" +
                        "Try checking the 'Open/Save as Switch/Vita/PS4 pck' checkbox in the upper right corner.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine(ex.Message);
                }
                
                return;
            }
            treeViewMain.SelectedNode.Nodes.Clear();
            treeViewMain.SelectedNode.Collapse();
        }

        /// <summary>
        /// wrapper that allows the use of <paramref name="name"/> in <code>TreeNode.Nodes.Find(<paramref name="name"/>, ...)</code> and <code>TreeNode.Nodes.ContainsKey(<paramref name="name"/>)</code>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns>new Created TreeNode</returns>
        internal static TreeNode CreateNode(string name, object tag = null)
        {
            TreeNode node = new TreeNode(name);
            node.Name = name;
            node.Tag = tag;
            return node;
        }

        private void CheckForPasswordAndRemove()
        {
            if (_pck.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset file))
            {
                file.RemoveProperties("LOCK");
            }
        }

        private TreeNode BuildNodeTreeBySeperator(TreeNodeCollection root, string path, char seperator)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
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

        private void BuildPckTreeView(TreeNodeCollection root, PckFile pckFile, string parentPath = "")
        {
            foreach (var assets in pckFile.GetAssets())
            {
                // fix any file paths that may be incorrect
                //if (file.Filename.StartsWith(parentPath))
                //    file.Filename = file.Filename.Remove(0, parentPath.Length);
                TreeNode node = BuildNodeTreeBySeperator(root, assets.Filename, '/');
                node.Tag = assets;
                if (Settings.Default.LoadSubPcks &&
                    (assets.Type == PckAssetType.SkinDataFile || assets.Type == PckAssetType.TexturePackInfoFile) &&
                    assets.Data.Length > 0)
                {
                    try
                    {
                        var reader = new PckFileReader(GetEndianess());
                        PckFile subPCKfile = assets.GetData(reader);
                        // passes parent path to remove from sub pck filepaths
                        BuildPckTreeView(node.Nodes, subPCKfile, assets.Filename + "/");
                    }
                    catch (OverflowException ex)
                    {
                        MessageBox.Show("Failed to open pck\n" +
                            "Try checking the 'Open/Save as Switch/Vita/PS4 pck' checkbox in the upper right corner.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Debug.WriteLine(ex.Message);
                    }
                }
                SetPckFileIcon(node, assets.Type);
            };
        }

        private void SetPckFileIcon(TreeNode node, PckAssetType type)
        {
            switch (type)
            {
                case PckAssetType.AudioFile:
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    break;
                case PckAssetType.LocalisationFile:
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    break;
                case PckAssetType.TexturePackInfoFile:
                    node.ImageIndex = 4;
                    node.SelectedImageIndex = 4;
                    break;
                case PckAssetType.ColourTableFile:
                    node.ImageIndex = 6;
                    node.SelectedImageIndex = 6;
                    break;
                case PckAssetType.ModelsFile:
                    node.ImageIndex = 8;
                    node.SelectedImageIndex = 8;
                    break;
                case PckAssetType.SkinDataFile:
                    node.ImageIndex = 7;
                    node.SelectedImageIndex = 7;
                    break;
                case PckAssetType.GameRulesFile:
                    node.ImageIndex = 9;
                    node.SelectedImageIndex = 9;
                    break;
                case PckAssetType.GameRulesHeader:
                    node.ImageIndex = 10;
                    node.SelectedImageIndex = 10;
                    break;
                case PckAssetType.InfoFile:
                    node.ImageIndex = 11;
                    node.SelectedImageIndex = 11;
                    break;
                case PckAssetType.SkinFile:
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                    break;
                case PckAssetType.CapeFile:
                    node.ImageIndex = 13;
                    node.SelectedImageIndex = 13;
                    break;
                case PckAssetType.TextureFile:
                    node.ImageIndex = 14;
                    node.SelectedImageIndex = 14;
                    break;
                case PckAssetType.BehavioursFile:
                    node.ImageIndex = 15;
                    node.SelectedImageIndex = 15;
                    break;
                case PckAssetType.MaterialFile:
                    node.ImageIndex = 16;
                    node.SelectedImageIndex = 16;
                    break;
                default: // unknown file format
                    node.ImageIndex = 5;
                    node.SelectedImageIndex = 5;
                    break;
            }
        }

        internal void BuildMainTreeView()
        {
            // In case the Rename function was just used and the selected node name no longer matches the file name
            string selectedNodeText = treeViewMain.SelectedNode is TreeNode node ? node.Text : string.Empty;
            previewPictureBox.Image = Resources.NoImageFound;
            treeMeta.Nodes.Clear();
            treeViewMain.Nodes.Clear();
            BuildPckTreeView(treeViewMain.Nodes, _pck);

            if (_isTemplateFile && _pck.HasAsset("Skins.pck", PckAssetType.SkinDataFile))
            {
                TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
                TreeNode folderNode = CreateNode("Skins");
                folderNode.ImageIndex = 0;
                folderNode.SelectedImageIndex = 0;
                if (!skinsNode.Nodes.ContainsKey("Skins"))
                    skinsNode.Nodes.Add(folderNode);
            }

            TreeNode[] selectedNodes;
            if (!string.IsNullOrEmpty(selectedNodeText) &&
                (selectedNodes = treeViewMain.Nodes.Find(selectedNodeText, true)).Length > 0)
            {
                treeViewMain.SelectedNode = selectedNodes[0];
            }
        }
        
        private bool IsSubPCKNode(string nodePath, string extention = ".pck")
        {
            // written by miku, implemented and modified by MattNL
            if (nodePath.EndsWith(extention)) return false;

            string[] subpaths = nodePath.Split('/');

            bool isSubFile = subpaths.Any(s => Path.GetExtension(s).Equals(extention));

            Debug.WriteLineIf(isSubFile, $"{nodePath} is a Sub-PCK File");

            return isSubFile;
        }

        private List<TreeNode> GetAllChildNodes(TreeNodeCollection root)
        {
            List<TreeNode> childNodes = new List<TreeNode>();
            foreach (TreeNode node in root)
            {
                childNodes.Add(node);
                if (node.Nodes.Count > 0)
                {
                    childNodes.AddRange(GetAllChildNodes(node.Nodes));
                }
            }
            return childNodes;
        }

        private TreeNode GetSubPCK(string childPath)
        {
            string parentPath = childPath.Replace('\\', '/');
            Console.WriteLine(parentPath);
            string[] s = parentPath.Split('/');
            Console.WriteLine(s.Length);
            foreach (var node in s)
            {
                TreeNode parent = treeViewMain.Nodes.Find(node, true)[0];
                if (parent.Tag is PckAsset f &&
                    (f.Type is PckAssetType.TexturePackInfoFile ||
                     f.Type is PckAssetType.SkinDataFile))
                    return parent;
            }

            return null;
        }

        private void RebuildSubPCK(string childPath)
        {
            // Support for if a file is edited within a nested PCK File (AKA SubPCK)

            if (!IsSubPCKNode(childPath)) return;

            TreeNode parent = GetSubPCK(childPath);
            Console.WriteLine(parent.Name);
            if (parent == null) return;

            PckAsset parent_file = parent.Tag as PckAsset;
            if (parent_file.Type is PckAssetType.TexturePackInfoFile || parent_file.Type is PckAssetType.SkinDataFile)
            {
                Console.WriteLine("Rebuilding " + parent_file.Filename);
                PckFile newPCKFile = new PckFile(3, parent_file.Type is PckAssetType.SkinDataFile);

                foreach (TreeNode node in GetAllChildNodes(parent.Nodes))
                {
                    if (node.Tag is PckAsset node_file)
                    {
                        PckAsset new_file = newPCKFile.CreateNewAsset(node_file.Filename.Replace(parent_file.Filename + "/", String.Empty), node_file.Type);
                        foreach (var prop in node_file.GetProperties())
                            new_file.AddProperty(prop);
                        new_file.SetData(node_file.Data);
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new PckFileWriter(newPCKFile, GetEndianess());
                    writer.WriteToStream(ms);
                    parent_file.SetData(ms.ToArray());
                    parent.Tag = parent_file;
                }

                BuildMainTreeView();
            }
        }

        private bool TryGetLocFile(out LOCFile locFile)
        {
            if (!_pck.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locdata) &&
                !_pck.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locdata))
            {
                locFile = null;
                return false;
            }

            try
            {
                using (var stream = new MemoryStream(locdata.Data))
                {
                    var reader = new LOCFileReader();
                    locFile = reader.FromStream(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, category: $"{nameof(MainForm)}{nameof(TryGetLocFile)}");
            }
            locFile = null;
            return false;
        }

        private bool TrySetLocFile(in LOCFile locFile)
        {
            if (!_pck.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locdata) &&
                !_pck.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locdata))
            {
                return false;
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new LOCFileWriter(locFile, 2);
                    writer.WriteToStream(stream);
                    locdata.SetData(stream.ToArray());
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, category: $"{nameof(MainForm)}{nameof(TrySetLocFile)}");
            }
            return false;
        }
        
        private void ReloadMetaTreeView()
        {
            treeMeta.Nodes.Clear();
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckAsset file)
            {
                foreach (var property in file.GetProperties())
                {
                    treeMeta.Nodes.Add(CreateNode(property.Key, property));
                }
            }
        }

        private static PckAsset CreateNewAudioFile(bool isLittle)
        {
            PckAudioFile audioPck = new PckAudioFile();
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Overworld);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Nether);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.End);
            PckAsset pckFileData = new PckAsset("audio.pck", PckAssetType.AudioFile);
            pckFileData.SetData(new PckAudioFileWriter(audioPck, isLittle ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
            return pckFileData;
        }

        private void UpdateRichPresence()
        {
            if (_pck is not null &&
                TryGetLocFile(out LOCFile locfile) &&
                locfile.HasLocEntry("IDS_DISPLAY_NAME") &&
                locfile.Languages.Contains("en-EN"))
            {
                RPC.SetPresence("Editing a Pack:", $" > {locfile.GetLocEntry("IDS_DISPLAY_NAME", "en-EN")}");
                return;
            }
            // default
            RPC.SetPresence("An Open Source .PCK File Editor");
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            // Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using AddFilePrompt diag = new AddFilePrompt("res/" + Path.GetFileName(ofd.FileName));
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    PckAsset file = _pck.CreateNewAsset(
                        diag.Filepath,
                        diag.Filetype,
                        () => File.ReadAllBytes(ofd.FileName));

                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    //else treeViewMain.Nodes.Add();

                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
            return;
        }

        private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Texture File(*.png;*.tga)|*.png;*.tga";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                using TextPrompt renamePrompt = new TextPrompt(Path.GetFileName(fileDialog.FileName));
                renamePrompt.LabelText = "Path";
                if (renamePrompt.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(renamePrompt.NewText))
                {
                    var file = _pck.CreateNewAsset(renamePrompt.NewText, PckAssetType.TextureFile);
                    file.SetData(File.ReadAllBytes(fileDialog.FileName));
                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
        }

        private void importSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog contents = new OpenFileDialog())
            {
                contents.Title = "Select Extracted Skin File";
                contents.Filter = "Skin File (*.png)|*.png";

                if (contents.ShowDialog() == DialogResult.OK)
                {
                    string skinNameImport = Path.GetFileName(contents.FileName);
                    byte[] data = File.ReadAllBytes(contents.FileName);
                    PckAsset mfNew = _pck.CreateNewAsset(skinNameImport, PckAssetType.SkinFile);
                    mfNew.SetData(data);
                    string propertyFile = Path.GetFileNameWithoutExtension(contents.FileName) + ".txt";
                    if (File.Exists(propertyFile))
                    {
                        string[] txtProperties = File.ReadAllLines(propertyFile);
                        if ((txtProperties.Contains("DISPLAYNAMEID") && txtProperties.Contains("DISPLAYNAME")) ||
                            txtProperties.Contains("THEMENAMEID") && txtProperties.Contains("THEMENAME") &&
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
                                mfNew.AddProperty(key, value);
                            }
                            _wasModified = true;
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
            TextPrompt folderNamePrompt = new TextPrompt();
            if (treeViewMain.SelectedNode is not null) folderNamePrompt.contextLabel.Text = $"New folder at the location of \"{treeViewMain.SelectedNode.FullPath}\"";
            folderNamePrompt.OKButtonText = "Add";
            if (folderNamePrompt.ShowDialog() == DialogResult.OK)
            {
                TreeNode folerNode = CreateNode(folderNamePrompt.NewText);
                folerNode.ImageIndex = 0;
                folerNode.SelectedImageIndex = 0;

                TreeNodeCollection nodeCollection = treeViewMain.Nodes;
                if (treeViewMain.SelectedNode is TreeNode node)
                {
                    if (node.Tag is PckAsset fd &&
                        (fd.Type != PckAssetType.TexturePackInfoFile &&
                         fd.Type != PckAssetType.SkinDataFile))
                    {
                        if (node.Parent is TreeNode parentNode)
                        {
                            nodeCollection = parentNode.Nodes;
                        }
                    }
                    else nodeCollection = node.Nodes;
                }
                nodeCollection.Add(folerNode);
            }
        }

        private void setFileType_Click(object sender, EventArgs e, PckAssetType type)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset file)
            {
                Debug.WriteLine($"Setting {file.Type} to {type}");
                file.Type = type;
                SetPckFileIcon(t, type);
                RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
            }
        }

        private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ReloadMetaTreeView();
            entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
            buttonEdit.Visible = false;
            previewPictureBox.Image = Resources.NoImageFound;
            viewFileInfoToolStripMenuItem.Visible = false;
            if (e.Node is TreeNode t && t.Tag is PckAsset file)
            {
                viewFileInfoToolStripMenuItem.Visible = true;
                if (file.HasProperty("BOX"))
                {
                    buttonEdit.Text = "EDIT BOXES";
                    buttonEdit.Visible = true;
                }
                else if (file.HasProperty("ANIM") &&
                        file.GetProperty("ANIM", s => SkinANIM.FromString(s) == (SkinAnimMask.RESOLUTION_64x64 | SkinAnimMask.SLIM_MODEL)))
                {
                    buttonEdit.Text = "View Skin";
                    buttonEdit.Visible = true;
                }

                switch (file.Type)
                {
                    case PckAssetType.SkinFile:
                    case PckAssetType.CapeFile:
                    case PckAssetType.TextureFile:
                        {
                            var img = file.GetTexture();

                            if (img.RawFormat != ImageFormat.Jpeg || img.RawFormat != ImageFormat.Png)
                            {
                                img = new Bitmap(img);
                            }

                            try
                            {
                                previewPictureBox.Image = img;
                                labelImageSize.Text = $"{previewPictureBox.Image.Size.Width}x{previewPictureBox.Image.Size.Height}";
                            }
                            catch (Exception ex)
                            {
                                labelImageSize.Text = "";
                                previewPictureBox.Image = Resources.NoImageFound;
                                Debug.WriteLine("Not a supported image format. Setting back to default");
                                Debug.WriteLine(string.Format("An error occured of type: {0} with message: {1}", ex.GetType(), ex.Message), "Exception");
                            }


                            if ((file.Filename.StartsWith("res/textures/blocks/") || file.Filename.StartsWith("res/textures/items/")) &&
                                file.Type == PckAssetType.TextureFile
                                && !file.IsMipmappedFile())
                            {
                                buttonEdit.Text = "EDIT TILE ANIMATION";
                                buttonEdit.Visible = true;
                            }
                        }
                        break;

                    case PckAssetType.LocalisationFile:
                        buttonEdit.Text = "EDIT LOC";
                        buttonEdit.Visible = true;
                        break;

                    case PckAssetType.AudioFile:
                        buttonEdit.Text = "EDIT MUSIC CUES";
                        buttonEdit.Visible = true;
                        break;

                    case PckAssetType.ColourTableFile when file.Filename == "colours.col":
                        buttonEdit.Text = "EDIT COLORS";
                        buttonEdit.Visible = true;
                        break;

                    case PckAssetType.BehavioursFile when file.Filename == "behaviours.bin":
                        buttonEdit.Text = "EDIT BEHAVIOURS";
                        buttonEdit.Visible = true;
                        break;
                    default:
                        buttonEdit.Visible = false;
                        break;
                }
            }
        }

        private void treeViewMain_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset file)
            {
                pckFileTypeHandler[file.Type]?.Invoke(file);
            }
        }

        #region drag and drop for main tree node

        // Most of the code below is modified code from this link:
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
        // - MattNL

        private void treeViewMain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Item is not TreeNode node)
                return;

            if ((node.TryGetTagData(out PckAsset file) && _pck.Contains(file.Filename, file.Type)) || node.Parent is TreeNode)
            {
                treeViewMain.DoDragDrop(node, DragDropEffects.Move);
            }
        }

        private void treeViewMain_DragOver(object sender, DragEventArgs e)
        {
            Point dragLocation = new Point(e.X, e.Y);
            TreeNode node = treeViewMain.GetNodeAt(treeViewMain.PointToClient(dragLocation));
            treeViewMain.SelectedNode = node.IsTagOfType<PckAsset>() ? null : node;
        }

        private void treeViewMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : e.AllowedEffect;
        }

        private void treeViewMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                ImportFiles(files);
                return;
            }

            string dataFormat = typeof(TreeNode).FullName;

            if (!e.Data.GetDataPresent(dataFormat))
                return;

            // Retrieve the client coordinates of the drop location.
            Point dragLocation = new Point(e.X, e.Y);
            Point targetPoint = treeViewMain.PointToClient(dragLocation);

            if (!treeViewMain.ClientRectangle.Contains(targetPoint))
                return;

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeViewMain.GetNodeAt(targetPoint);
            bool isTargetPckFile = targetNode.IsTagOfType<PckAsset>();

            if (e.Data.GetData(dataFormat) is not TreeNode draggedNode)
            {
                Debug.WriteLine("Dragged data was not of type TreeNode.");
                return;
            }

            if (targetNode.Equals(draggedNode.Parent))
            {
                Debug.WriteLine("target node is parent of dragged node... nothing done.");
                return;
            }

            if (draggedNode.Equals(targetNode.Parent))
            {
                Debug.WriteLine("dragged node is parent of target node... nothing done.");
                return;
            }

            if (targetNode.Parent == null && isTargetPckFile && draggedNode.Parent == null)
            {
                Debug.WriteLine("target node is file and is in the root... nothing done.");
                return;
            }

            if ((targetNode.Parent?.Equals(draggedNode.Parent) ?? false) && isTargetPckFile)
            {
                Debug.WriteLine("target node and dragged node have the same parent... nothing done.");
                return;
            }

            Debug.WriteLine($"Target drop location is {(isTargetPckFile ? "file" : "folder")}.");

            // Retrieve the node that was dragged.
            if (draggedNode.TryGetTagData(out PckAsset draggedFile) &&
                targetNode.FullPath != draggedFile.Filename)
            {
                Debug.WriteLine(draggedFile.Filename + " was droped onto " + targetNode.FullPath);
                string newFilePath = Path.Combine(isTargetPckFile
                    ? Path.GetDirectoryName(targetNode.FullPath)
                    : targetNode.FullPath, Path.GetFileName(draggedFile.Filename));
                Debug.WriteLine("New filepath: " + newFilePath);
                draggedFile.Filename = newFilePath;
                _wasModified = true;
                BuildMainTreeView();
                return;
            }
            else
            {
                List<PckAsset> pckFiles = GetEndingNodes(draggedNode.Nodes).Where(t => t.IsTagOfType<PckAsset>()).Select(t => t.Tag as PckAsset).ToList();
                string oldPath = draggedNode.FullPath;
                string newPath = Path.Combine(isTargetPckFile ? Path.GetDirectoryName(targetNode.FullPath) : targetNode.FullPath, draggedNode.Text).Replace('\\', '/');
                foreach (var pckFile in pckFiles)
                {
                    pckFile.Filename = Path.Combine(newPath, pckFile.Filename.Substring(oldPath.Length + 1)).Replace('\\', '/');
                }
                _wasModified = true;
                BuildMainTreeView();
            }
        }

        private IEnumerable<TreeNode> GetEndingNodes(TreeNodeCollection collection)
        {
            List<TreeNode> trailingNodes = new List<TreeNode>(collection.Count);
            foreach (TreeNode node in collection)
            {
                if (node.Nodes.Count > 0)
                {
                    trailingNodes.AddRange(GetEndingNodes(node.Nodes));
                    continue;
                }
                trailingNodes.Add(node);
            }
            return trailingNodes;
        }

        private void ImportFiles(string[] files)
        {
            int addedCount = 0;
            foreach (var file in files)
            {
                using AddFilePrompt addFile = new AddFilePrompt(Path.GetFileName(file));
                if (addFile.ShowDialog(this) != DialogResult.OK)
                    continue;

                if (_pck.Contains(addFile.Filepath, addFile.Filetype))
                {
                    MessageBox.Show(this, $"'{addFile.Filepath}' of type {addFile.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                _pck.CreateNewAsset(addFile.Filepath, addFile.Filetype, () => File.ReadAllBytes(file));
                addedCount++;

                BuildMainTreeView();
                _wasModified = true;
            }
            Trace.TraceInformation("[{0}] Imported {1} file(s).", nameof(ImportFiles), addedCount);
        }

        #endregion

        private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LOCFile locFile = null;
            TryGetLocFile(out locFile);
            using AddSkinPrompt add = new AddSkinPrompt(locFile);
            if (add.ShowDialog() == DialogResult.OK)
            {

                if (_pck.HasAsset("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
                {
                    TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
                    if (subPCK.Nodes.ContainsKey("Skins")) add.SkinAsset.Filename = add.SkinAsset.Filename.Insert(0, "Skins/");
                    add.SkinAsset.Filename = add.SkinAsset.Filename.Insert(0, "Skins.pck/");
                    TreeNode newNode = new TreeNode(Path.GetFileName(add.SkinAsset.Filename));
                    newNode.Tag = add.SkinAsset;
                    SetNodeIcon(newNode, PckAssetType.SkinFile);
                    subPCK.Nodes.Add(newNode);
                    RebuildSubPCK(newNode.FullPath);
                }
                else
                {
                    if (treeViewMain.Nodes.ContainsKey("Skins")) add.SkinAsset.Filename = add.SkinAsset.Filename.Insert(0, "Skins/"); // Then Skins folder
                    _pck.AddAsset(add.SkinAsset);
                }
                if (add.HasCape)
                {
                    if (_pck.HasAsset("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
                    {
                        TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
                        if (subPCK.Nodes.ContainsKey("Skins")) add.CapeAsset.Filename = add.CapeAsset.Filename.Insert(0, "Skins/");
                        add.CapeAsset.Filename = add.CapeAsset.Filename.Insert(0, "Skins.pck/");
                        TreeNode newNode = new TreeNode(Path.GetFileName(add.CapeAsset.Filename));
                        newNode.Tag = add.CapeAsset;
                        SetNodeIcon(newNode, PckAssetType.SkinFile);
                        subPCK.Nodes.Add(newNode);
                        RebuildSubPCK(newNode.FullPath);
                    }
                    else
                    {
                        if (treeViewMain.Nodes.ContainsKey("Skins")) add.CapeAsset.Filename = add.CapeAsset.Filename.Insert(0, "Skins/"); // Then Skins folder
                        _pck.AddAsset(add.CapeAsset);
                    }
                }

                TrySetLocFile(locFile);
                _wasModified = true;
                BuildMainTreeView();
            }
        }

        private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using ChangeTile diag = new ChangeTile();
            if (diag.ShowDialog(this) != DialogResult.OK)
                return;

            var file = new PckAsset(
                $"{ResourceLocation.GetPathFromCategory(diag.Category)}/{diag.SelectedTile}.png",
                PckAssetType.TextureFile);

            var animation = file.GetDeserializedData<Animation>(AnimationDeserializer.DefaultDeserializer);
            using AnimationEditor animationEditor = new AnimationEditor(animation, Path.GetFileNameWithoutExtension(file.Filename));
            if (animationEditor.ShowDialog() == DialogResult.OK)
            {
                _wasModified = true;
                _pck.AddAsset(file);
                BuildMainTreeView();
                ReloadMetaTreeView();
            }
        }

        private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.Contains("audio.pck", PckAssetType.AudioFile))
            {
                // the chance of this happening is really really slim but just in case
                MessageBox.Show($"There is already a file of type \"{nameof(PckAssetType.AudioFile)}\" and name \"audio.pck\" in this PCK!", "Can't create audio.pck");
                return;
            }
            if (string.IsNullOrEmpty(_location))
            {
                MessageBox.Show("You must save your pck before creating or opening a music cues PCK file", "Can't create audio.pck");
                return;
            }

            var file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
            AudioEditor diag = new AudioEditor(file, LittleEndianCheckBox.Checked);
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                _pck.AddAsset(file);
            }
            diag.Dispose();
            BuildMainTreeView();
        }

        private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetAsset("colours.col", PckAssetType.ColourTableFile, out _))
            {
                MessageBox.Show("A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            var newColorFile = _pck.CreateNewAsset("colours.col", PckAssetType.ColourTableFile);
            newColorFile.SetData(Resources.tu69colours);
            BuildMainTreeView();
        }

        private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetAsset("Skins.pck", PckAssetType.SkinDataFile, out _))
            {
                MessageBox.Show("A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            _pck.CreateNewAsset("Skins.pck", PckAssetType.SkinDataFile, () =>
            {
                using var stream = new MemoryStream();
                var writer = new PckFileWriter(new PckFile(3, true), GetEndianess());
                writer.WriteToStream(stream);
                return stream.ToArray();
            });

            BuildMainTreeView();

            TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
            TreeNode folderNode = CreateNode("Skins");
            folderNode.ImageIndex = 0;
            folderNode.SelectedImageIndex = 0;
            skinsNode.Nodes.Add(folderNode);
        }

        private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetAsset("behaviours.bin", PckAssetType.BehavioursFile, out _))
            {
                MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            _pck.CreateNewAsset("behaviours.bin", PckAssetType.BehavioursFile, new BehavioursWriter(new BehaviourFile()));
            BuildMainTreeView();
        }

        private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetAsset("entityMaterials.bin", PckAssetType.MaterialFile, out _))
            {
                MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            var materialContainer = new MaterialContainer();
            materialContainer.InitializeDefault();
            _pck.CreateNewAsset("entityMaterials.bin", PckAssetType.MaterialFile, new MaterialFileWriter(materialContainer));
            BuildMainTreeView();
        }

        private void importExtractedSkinsFolder(object sender, EventArgs e)
        {
            using FolderBrowserDialog contents = new FolderBrowserDialog();
            if (contents.ShowDialog() == DialogResult.OK)
            {
                //checks to make sure selected path exist
                if (!Directory.Exists(contents.SelectedPath))
                {
                    MessageBox.Show("Directory Lost");
                    return;
                }
                // creates variable to indicate wether current pck skin structure is mashup or regular skin
                bool hasSkinsPck = _pck.HasAsset("Skins.pck", PckAssetType.SkinDataFile);

                foreach (var fullfilename in Directory.GetFiles(contents.SelectedPath, "*.png"))
                {
                    string filename = Path.GetFileNameWithoutExtension(fullfilename);
                    // sets file type based on wether its a cape or skin
                    PckAssetType pckfiletype = filename.StartsWith("dlccape", StringComparison.OrdinalIgnoreCase)
                        ? PckAssetType.CapeFile
                        : PckAssetType.SkinFile;
                    string pckfilepath = (hasSkinsPck ? "Skins/" : string.Empty) + filename + ".png";


                    PckAsset newFile = new PckAsset(pckfilepath, pckfiletype);
                    byte[] filedata = File.ReadAllBytes(fullfilename);
                    newFile.SetData(filedata);

                    if (File.Exists(fullfilename + ".txt"))
                    {
                        string[] properties = File.ReadAllText(fullfilename + ".txt").Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string property in properties)
                        {
                            string[] param = property.Split(':');
                            if (param.Length < 2) continue;
                            newFile.AddProperty(param[0], param[1]);
                            //switch (param[0])
                            //{
                            //    case "DISPLAYNAMEID":
                            //        locNameId = param[1];
                            //        continue;

                            //    case "DISPLAYNAME":
                            //        locName = param[1];
                            //        continue;

                            //    case "THEMENAMEID":
                            //        locThemeId = param[1];
                            //        continue;

                            //    case "THEMENAME":
                            //        locTheme = param[1];
                            //        continue;
                            //}
                        }
                    }
                    if (hasSkinsPck)
                    {
                        var skinsfile = _pck.GetAsset("Skins.pck", PckAssetType.SkinDataFile);
                        using (var ms = new MemoryStream(skinsfile.Data))
                        {
                            //var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                            //var skinspck = reader.FromStream(ms);
                            //skinspck.Files.Add(newFile);
                            //ms.Position = 0;
                            //var writer = new PckFileWriter(skinspck, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                            //writer.WriteToStream(ms);
                            //skinsfile.SetData(ms.ToArray());
                        }
                        continue;
                    }
                    _pck.AddAsset(newFile);
                }
                BuildMainTreeView();
                _wasModified = true;
            }
        }

        private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckAsset file &&
                file.Type == PckAssetType.SkinFile)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "3DS Texture|*.3dst";
                saveFileDialog.DefaultExt = ".3dst";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image img = file.GetTexture();
                    var writer = new _3DSTextureWriter(img);
                    writer.WriteToFile(saveFileDialog.FileName);
                }
            }
        }

        private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset file && file.Type == PckAssetType.TextureFile)
            {
                string textureDirectory = Path.GetDirectoryName(file.Filename);
                string textureName = Path.GetFileNameWithoutExtension(file.Filename);

                if (file.IsMipmappedFile())
                    return;

                string textureExtension = Path.GetExtension(file.Filename);

                // TGA is not yet supported
                if (textureExtension == ".tga") return;

                using NumericPrompt numericPrompt = new NumericPrompt(0);
                numericPrompt.Minimum = 1;
                numericPrompt.Maximum = 4; // 5 is the presumed max MipMap level
                numericPrompt.ContextLabel.Text = "You can enter the amount of MipMap levels that you would like to generate. " +
                    "For example: if you enter 2, MipMapLevel1.png and MipMapLevel2.png will be generated";
                numericPrompt.TextLabel.Text = "Levels";

                if (numericPrompt.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 2; i < 2 + numericPrompt.SelectedValue; i++)
                    {
                        string mippedPath = $"{textureDirectory}/{textureName}MipMapLevel{i}{textureExtension}";
                        Debug.WriteLine(mippedPath);
                        if (_pck.HasAsset(mippedPath, PckAssetType.TextureFile))
                            _pck.RemoveAsset(_pck.GetAsset(mippedPath, PckAssetType.TextureFile));
                        PckAsset MipMappedFile = new PckAsset(mippedPath, PckAssetType.TextureFile);


                        Image originalTexture = file.GetTexture();
                        int NewWidth = Math.Max(originalTexture.Width / (int)Math.Pow(2, i - 1), 1);
                        int NewHeight = Math.Max(originalTexture.Height / (int)Math.Pow(2, i - 1), 1);

                        Rectangle tileArea = new Rectangle(0, 0, NewWidth, NewHeight);
                        Image mippedTexture = new Bitmap(NewWidth, NewHeight);
                        using (Graphics gfx = Graphics.FromImage(mippedTexture))
                        {
                            gfx.SmoothingMode = SmoothingMode.None;
                            gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gfx.DrawImage(originalTexture, tileArea);
                        }
                        MemoryStream texStream = new MemoryStream();
                        mippedTexture.Save(texStream, ImageFormat.Png);
                        MipMappedFile.SetData(texStream.ToArray());
                        texStream.Dispose();

                        _pck.InsertAsset(_pck.IndexOfAsset(file) + i - 1, MipMappedFile);
                    }
                    BuildMainTreeView();
                }
            }
        }

        private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset file)
            {
                MessageBox.Show(
                    "File path: " + file.Filename +
                    "\nAssigned File type: " + (int)file.Type + " (" + file.Type + ")" +
                    "\nFile size: " + file.Size +
                    "\nProperties count: " + file.PropertyCount
                    , Path.GetFileName(file.Filename) + " file info");
            }
        }

        private void correctSkinDecimalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node && node.Tag is PckAsset file && file.Type == PckAssetType.SkinFile)
            {
                foreach (var p in file.GetProperties())
                {
                    if (p.Key == "BOX" || p.Key == "OFFSET")
                        file.SetProperty(file.GetPropertyIndex(p), new KeyValuePair<string, string>(p.Key, p.Value.Replace(',', '.')));
                }
                ReloadMetaTreeView();
                RebuildSubPCK(node.FullPath);
                _wasModified = true;
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeViewMain.SelectedNode;
            if (node == null) return;
            if (node.Tag is PckAsset file)
            {
                using SaveFileDialog exFile = new SaveFileDialog();
                exFile.FileName = Path.GetFileName(file.Filename);
                exFile.Filter = Path.GetExtension(file.Filename).Replace(".", string.Empty) + " File|*" + Path.GetExtension(file.Filename);
                if (exFile.ShowDialog() != DialogResult.OK ||
                    // Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
                    string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName))) return;
                string extractFilePath = exFile.FileName;

                File.WriteAllBytes(extractFilePath, file.Data);
                if (file.PropertyCount > 0)
                {
                    using var fs = File.CreateText($"{extractFilePath}.txt");
                    foreach (var property in file.GetProperties())
                    {
                        fs.WriteLine($"{property.Key}: {property.Value}");
                    }
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
                    if (IsSubPCKNode(node.FullPath) && node.Tag == null)
                    {
                        GetAllChildNodes(node.Nodes).ForEach(fileNode =>
                        {
                            if (fileNode.Tag is PckAsset file)
                            {
                                Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(file.Filename)}");
                                File.WriteAllBytes($"{dialog.SelectedPath}/{file.Filename}", file.Data);
                                if (file.PropertyCount > 0)
                                {
                                    using var fs = File.CreateText($"{dialog.SelectedPath}/{file.Filename}.txt");
                                    foreach (var property in file.GetProperties())
                                    {
                                        fs.WriteLine($"{property.Key}: {property.Value}");
                                    }
                                }
                            }
                        }
                        );
                    }
                    else
                    {
                        foreach (var _file in _pck.GetAssets())
                        {
                            if (_file.Filename.StartsWith(selectedFolder))
                            {
                                Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(_file.Filename)}");
                                File.WriteAllBytes($"{dialog.SelectedPath}/{_file.Filename}", _file.Data);
                                if (_file.PropertyCount > 0)
                                {
                                    using var fs = File.CreateText($"{dialog.SelectedPath}/{_file.Filename}.txt");
                                    foreach (var property in _file.GetProperties())
                                    {
                                        fs.WriteLine($"{property.Key}: {property.Value}");
                                    }
                                }
                            }
                        };
                    }
                    MessageBox.Show("Folder Extracted");
                }
            }
        }

        private void cloneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null) return;
            string path = node.FullPath;

            using TextPrompt diag = new TextPrompt(node.Tag is null ? Path.GetFileName(node.FullPath) : node.FullPath);
            diag.contextLabel.Text = $"Creating a clone of \"{path}\". Ensure that the path isn't yet.";
            diag.OKButtonText = "Clone";

            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                if (node.Tag is PckAsset file)
                {
                    TreeNode newNode = new TreeNode();
                    newNode.Text = Path.GetFileName(diag.NewText);
                    var NewFile = new PckAsset(diag.NewText, file.Type);
                    foreach (var property in file.GetProperties())
                    {
                        NewFile.AddProperty(property);
                    }
                    NewFile.SetData(file.Data);
                    NewFile.Filename = diag.NewText;
                    newNode.Tag = NewFile;
                    newNode.ImageIndex = node.ImageIndex;
                    newNode.SelectedImageIndex = node.SelectedImageIndex;

                    if (GetAllChildNodes(treeViewMain.Nodes).Find(n => n.FullPath == diag.NewText) != null)
                    {
                        MessageBox.Show(
                            this,
                            $"A file with the path \"{diag.NewText}\" already exists. " +
                            $"Please try again with a different name.",
                            "Key already exists");
                        return;
                    }

                    if (node.Parent == null) treeViewMain.Nodes.Insert(node.Index + 1, newNode); //adds generated file node
                    else node.Parent.Nodes.Insert(node.Index + 1, newNode);//adds generated file node to selected folder

                    if (!IsSubPCKNode(node.FullPath)) _pck.InsertAsset(node.Index + 1, NewFile);
                    else RebuildSubPCK(node.FullPath);
                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
        }

        private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null) return;
            string path = node.FullPath;

            using TextPrompt diag = new TextPrompt(node.Tag is null ? Path.GetFileName(node.FullPath) : node.FullPath);

            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                if (node.Tag is PckAsset file)
                {
                    if (_pck.TryGetAsset(diag.NewText, file.Type, out _))
                    {
                        MessageBox.Show($"{diag.NewText} already exists", "File already exists");
                        return;
                    }
                    file.Filename = diag.NewText;
                }
                else // folders
                {
                    node.Text = diag.NewText;
                    foreach (var childNode in GetAllChildNodes(node.Nodes))
                    {
                        if (childNode.Tag is PckAsset folderFile)
                        {
                            if (folderFile.Filename == diag.NewText) continue;
                            folderFile.Filename = childNode.FullPath;
                        }
                    }
                }
                _wasModified = true;
                RebuildSubPCK(path);
                BuildMainTreeView();
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset file)
            {
                using var ofd = new OpenFileDialog();
                // Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.

                string extra_extensions = "";

                switch (file.Type)
                {
                    case PckAssetType.TextureFile:
                        if (Path.GetExtension(file.Filename) == ".png") extra_extensions = ";*.tga";
                        else if (Path.GetExtension(file.Filename) == ".tga") extra_extensions = ";*.png";
                        break;
                }

                string fileExt = Path.GetExtension(file.Filename);

                ofd.Filter = $"{file.Type} (*{fileExt}{extra_extensions})|*{fileExt}{extra_extensions}";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string newFileExt = Path.GetExtension(ofd.FileName);
                    file.SetData(File.ReadAllBytes(ofd.FileName));
                    file.Filename = file.Filename.Replace(fileExt, newFileExt);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    _wasModified = true;
                    BuildMainTreeView();
                }
                return;
            }
            MessageBox.Show("Can't replace a folder.");
        }

        /// <summary>
		/// Action to run before a file will be deleted
		/// </summary>
		/// <param name="file">File to remove</param>
		/// <returns>True if the remove should be canceled, otherwise False</returns>
		private bool BeforeFileRemove(PckAsset file)
        {
            string itemPath = "res/textures/items/";

            // warn the user about deleting compass.png and clock.png
            if (file.Type == PckAssetType.TextureFile &&
                (file.Filename == itemPath + "compass.png" || file.Filename == itemPath + "clock.png"))
            {
                if (MessageBox.Show("Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return true;
            }

            // remove loc key if its a skin/cape
            if (file.Type == PckAssetType.SkinFile || file.Type == PckAssetType.CapeFile)
            {
                if (TryGetLocFile(out LOCFile locFile))
                {
                    if (file.TryGetProperty("THEMENAMEID", out string value))
                        locFile.RemoveLocKey(value);
                    if (file.TryGetProperty("DISPLAYNAMEID", out value))
                        locFile.RemoveLocKey(value);
                    TrySetLocFile(locFile);
                }
            }
            return false;
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeViewMain.SelectedNode;
            if (node == null)
                return;

            string path = node.FullPath;

            if (node.TryGetTagData(out PckAsset file))
            {
                if (!BeforeFileRemove(file) && _pck.RemoveAsset(file))
                {
                    node.Remove();
                    _wasModified = true;
                }
            }
            else if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string pckFolderDir = node.FullPath;
                _pck.RemoveAll(file => !BeforeFileRemove(file) && file.Filename.StartsWith(pckFolderDir));
                node.Remove();
                _wasModified = true;
            }
            RebuildSubPCK(path);
        }

        private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNode t && t.Tag is KeyValuePair<string, string> property)
            {
                entryTypeTextBox.Text = property.Key;
                entryDataTextBox.Text = property.Value;
            }
        }

        private void treeViewMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    deleteFileToolStripMenuItem_Click(sender, e);
                    break;
                case Keys.F2:
                    renameFileToolStripMenuItem_Click(sender, e);
                    break;
            }
        }

        private void treeViewMain_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // for now name edits are done through the 'rename' context menu item
            // TODO: add folder renaming
            //e.CancelEdit = e.Node.Tag is PckAsset;
            e.CancelEdit = true;
        }

        private void editAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckAsset file)
            {
                var props = file.GetProperties().Select(p => p.Key + " " + p.Value);
                using (var input = new MultiTextPrompt(props.ToArray()))
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        file.ClearProperties();
                        foreach (var line in input.TextOutput)
                        {
                            int idx = line.IndexOf(' ');
                            if (idx == -1 || line.Length - 1 == idx)
                                continue;
                            file.AddProperty(line.Substring(0, idx).Replace(":", string.Empty), line.Substring(idx + 1));
                        }
                        ReloadMetaTreeView();
                        RebuildSubPCK(node.FullPath);
                        _wasModified = true;
                    }
                }
            }
        }

        private void treeMeta_DoubleClick(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode subnode && subnode.Tag is KeyValuePair<string, string> property &&
                treeViewMain.SelectedNode is TreeNode node && node.Tag is PckAsset file)
            {
                int i = file.GetPropertyIndex(property);
                if (i != -1)
                {
                    switch (property.Key)
                    {
                        case "ANIM" when file.Type == PckAssetType.SkinFile:
                            try
                            {
                                using ANIMEditor diag = new ANIMEditor(property.Value);
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    file.SetProperty(i, new KeyValuePair<string, string>("ANIM", diag.ResultAnim.ToString()));
                                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                                    ReloadMetaTreeView();
                                    _wasModified = true;
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                MessageBox.Show("Failed to parse ANIM value, aborting to normal functionality. Please make sure the value only includes hexadecimal characters (0-9,A-F) and has no more than 8 characters.");
                            }
                            break;

                        case "BOX" when file.Type == PckAssetType.SkinFile:
                            try
                            {
                                using BoxEditor diag = new BoxEditor(property.Value, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    file.SetProperty(i, new KeyValuePair<string, string>("BOX", diag.Result.ToString()));
                                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                                    ReloadMetaTreeView();
                                    _wasModified = true;
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                MessageBox.Show("Failed to parse BOX value, aborting to normal functionality.");
                            }
                            break;

                        default:
                            break;

                    }

                    using (AddPropertyPrompt addProperty = new AddPropertyPrompt(property))
                    {
                        if (addProperty.ShowDialog() == DialogResult.OK)
                        {
                            file.SetProperty(i, addProperty.Property);
                            RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                            ReloadMetaTreeView();
                            _wasModified = true;
                        }
                    }
                }
            }
        }

        private void treeMeta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                deleteEntryToolStripMenuItem_Click(sender, e);
        }

        private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckAsset file)
            {
                using (var input = new MultiTextPrompt())
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        foreach (var line in input.TextOutput)
                        {
                            int idx = line.IndexOf(' ');
                            if (idx == -1 || line.Length - 1 == idx)
                                continue;
                            file.AddProperty(line.Substring(0, idx), line.Substring(idx + 1));
                        }
                        ReloadMetaTreeView();
                        RebuildSubPCK(node.FullPath);
                        _wasModified = true;
                    }
                }
            }
        }

        private void addBOXEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset file)
            {
                using BoxEditor diag = new BoxEditor(SkinBOX.Empty, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    file.AddProperty("BOX", diag.Result);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
                return;
            }
        }

        private void addANIMEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset file)
            {
                using ANIMEditor diag = new ANIMEditor(SkinANIM.Empty);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    file.AddProperty("ANIM", diag.ResultAnim);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
                return;
            }
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode t && t.Tag is KeyValuePair<string, string> property &&
                treeViewMain.SelectedNode is TreeNode main && main.Tag is PckAsset file &&
                file.RemoveProperty(property))
            {
                treeMeta.SelectedNode.Remove();
                RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                _wasModified = true;
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t &&
                t.Tag is PckAsset file)
            {
                using AddPropertyPrompt addProperty = new AddPropertyPrompt();
                if (addProperty.ShowDialog() == DialogResult.OK)
                {
                    file.AddProperty(addProperty.Property);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
            }
        }

        private void HandleTextureFile(PckAsset asset)
        {
            _ = asset.IsMipmappedFile() && _pck.TryGetValue(asset.GetNormalPath(), PckAssetType.TextureFile, out asset);

            if (asset.Size <= 0)
            {
                Debug.WriteLine($"'{asset.Filename}' size is 0.", category: nameof(HandleTextureFile));
                return;
            }

            ResourceLocation resourceLocation = ResourceLocation.GetFromPath(asset.Filename);
            Debug.WriteLine("Handling Resource file: " + resourceLocation?.ToString());
            if (resourceLocation is null || resourceLocation.Category == ResourceCategory.Unknown)
                return;

            if (resourceLocation.Category != ResourceCategory.BlockAnimation &&
                resourceLocation.Category != ResourceCategory.ItemAnimation)
            {
                Image img = asset.GetTexture();
                var viewer = new TextureAtlasEditor(_pck, resourceLocation, img);
                if (viewer.ShowDialog(this) == DialogResult.OK)
                {
                    Image texture = viewer.FinalTexture;
                    asset.SetTexture(texture);
                    _wasModified = true;
                    BuildMainTreeView();
                }
                return;
            }

            if (resourceLocation.Category != ResourceCategory.ItemAnimation &&
                resourceLocation.Category != ResourceCategory.BlockAnimation)
                return;

            Animation animation = asset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
            string internalName = Path.GetFileNameWithoutExtension(asset.Filename);

            var textureInfos = resourceLocation.Category switch
            {
                ResourceCategory.BlockAnimation => Tiles.BlockTileInfos,
                ResourceCategory.ItemAnimation => Tiles.ItemTileInfos,
                _ => Array.Empty<JsonTileInfo>().ToList()
            };
            string displayname = textureInfos.FirstOrDefault(p => p.InternalName == internalName)?.DisplayName ?? internalName;

            string[] specialTileNames = { "clock", "compass" };

            using (AnimationEditor animationEditor = new AnimationEditor(animation, displayname, internalName.ToLower().EqualsAny(specialTileNames)))
            {
                if (animationEditor.ShowDialog(this) == DialogResult.OK)
                {
                    _wasModified = true;
                    asset.SetSerializedData(animationEditor.Result, AnimationSerializer.DefaultSerializer);
                    BuildMainTreeView();
                }
            }
        }

        private void HandleGameRuleFile(PckAsset file)
        {
            const string use_deflate = "PS3";
            const string use_xmem = "Xbox 360";
            const string use_zlib = "Wii U, PS Vita";

            ItemSelectionPopUp dialog = new ItemSelectionPopUp(use_zlib, use_deflate, use_xmem);
            dialog.LabelText = "Type";
            dialog.ButtonText = "Ok";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var compressiontype = dialog.SelectedItem switch
            {
                use_deflate => GameRuleFile.CompressionType.Deflate,
                use_xmem => GameRuleFile.CompressionType.XMem,
                use_zlib => GameRuleFile.CompressionType.Zlib,
                _ => GameRuleFile.CompressionType.Unknown
            };

            GameRuleFile grf = file.GetData(new GameRuleFileReader(compressiontype));

            using GameRuleFileEditor grfEditor = new GameRuleFileEditor(grf);
            if (grfEditor.ShowDialog(this) == DialogResult.OK)
            {
                file.SetData(new GameRuleFileWriter(grfEditor.Result));
                _wasModified = true;
                UpdateRichPresence();
            }
        }

        private void HandleAudioFile(PckAsset file)
        {
            using AudioEditor audioEditor = new AudioEditor(file, LittleEndianCheckBox.Checked);
            _wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleLocalisationFile(PckAsset file)
        {
            using LOCEditor locedit = new LOCEditor(file);
            _wasModified = locedit.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleColourFile(PckAsset file)
        {
            if (file.Size == 0)
            {
                MessageBox.Show("No Color data found.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            using COLEditor diag = new COLEditor(file);
            _wasModified = diag.ShowDialog(this) == DialogResult.OK;
        }

        public void HandleSkinFile(PckAsset file)
        {
            if (file.Size <= 0)
                return;
            var texture = file.GetTexture();
            if (file.HasProperty("BOX"))
            {
                using generateModel generate = new generateModel(file);
                if (generate.ShowDialog() == DialogResult.OK)
                {
                    entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
                    _wasModified = true;
                    ReloadMetaTreeView();
                }
            }
            else
            {
                SkinPreview frm = new SkinPreview(texture, file.GetProperty("ANIM", SkinANIM.FromString));
                frm.ShowDialog(this);
                frm.Dispose();
            }
        }

        public void HandleModelsFile(PckAsset file)
        {
            MessageBox.Show("Models.bin support has not been implemented. You can use the Spark Editor for the time being to edit these files.", "Not implemented yet.");
        }

        public void HandleBehavioursFile(PckAsset file)
        {
            using BehaviourEditor edit = new BehaviourEditor(file);
            _wasModified = edit.ShowDialog(this) == DialogResult.OK;
        }

        public void HandleMaterialFile(PckAsset file)
        {
            using MaterialsEditor edit = new MaterialsEditor(file);
            _wasModified = edit.ShowDialog(this) == DialogResult.OK;
        }

        private void PckEditor_Load(object sender, EventArgs e)
        {
            CheckForPasswordAndRemove();
            BuildMainTreeView();
            UpdateRichPresence();
        }

        public void Close()
        {
            if ((_wasModified || _isTemplateFile) &&
                MessageBox.Show("Save PCK?", _isTemplateFile ? "Unsaved PCK" : "Modified PCK",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (_isTemplateFile || string.IsNullOrEmpty(_location) || !File.Exists(_location))
                {
                    SaveAs();
                    return;
                }
                Save();
            }
        }

        public void SaveAs()
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PCK (Minecraft Console Package)|*.pck",
                DefaultExt = ".pck",
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveTo(saveFileDialog.FileName);
                pckFileLabel.Text = "Current PCK File: " + Path.GetFileName(_location);
            }
        }

        public void SaveTo(string filepath)
        {
            _location = filepath;
            _isTemplateFile = false;
            Save();
        }

        public void Save()
        {
            var writer = new PckFileWriter(_pck, GetEndianess());
            writer.WriteToFile(_location);
            _timesSaved++;
            _wasModified = false;
        }

        public bool Open(string filepath, OMI.Endianness endianness)
        {
            SetEndianess(endianness);
            _location = filepath;
            try
            {
                var reader = new PckFileReader(endianness);
                _pck = reader.FromFile(filepath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, category: $"{nameof(PckEditor)}.{nameof(Open)}");
            }
            return false;
        }

        private void SetEndianess(OMI.Endianness endianness)
        {
            LittleEndianCheckBox.Checked = endianness == OMI.Endianness.LittleEndian;
        }

        private OMI.Endianness GetEndianess()
        {
            return LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian;
        }

        public bool Open(PckFile pck)
        {
            _pck = pck;
            _isTemplateFile = true;
            return true;
        }

        public void UpdateView()
        {
            BuildMainTreeView();
        }

        private void SetNodeIcon(TreeNode node, PckAssetType type)
        {
            switch (type)
            {
                case PckAssetType.AudioFile:
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    break;
                case PckAssetType.LocalisationFile:
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    break;
                case PckAssetType.TexturePackInfoFile:
                    node.ImageIndex = 4;
                    node.SelectedImageIndex = 4;
                    break;
                case PckAssetType.ColourTableFile:
                    node.ImageIndex = 6;
                    node.SelectedImageIndex = 6;
                    break;
                case PckAssetType.ModelsFile:
                    node.ImageIndex = 8;
                    node.SelectedImageIndex = 8;
                    break;
                case PckAssetType.SkinDataFile:
                    node.ImageIndex = 7;
                    node.SelectedImageIndex = 7;
                    break;
                case PckAssetType.GameRulesFile:
                    node.ImageIndex = 9;
                    node.SelectedImageIndex = 9;
                    break;
                case PckAssetType.GameRulesHeader:
                    node.ImageIndex = 10;
                    node.SelectedImageIndex = 10;
                    break;
                case PckAssetType.InfoFile:
                    node.ImageIndex = 11;
                    node.SelectedImageIndex = 11;
                    break;
                case PckAssetType.SkinFile:
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                    break;
                case PckAssetType.CapeFile:
                    node.ImageIndex = 13;
                    node.SelectedImageIndex = 13;
                    break;
                case PckAssetType.TextureFile:
                    node.ImageIndex = 14;
                    node.SelectedImageIndex = 14;
                    break;
                case PckAssetType.BehavioursFile:
                    node.ImageIndex = 15;
                    node.SelectedImageIndex = 15;
                    break;
                case PckAssetType.MaterialFile:
                    node.ImageIndex = 16;
                    node.SelectedImageIndex = 16;
                    break;
                default: // unknown file format
                    node.ImageIndex = 5;
                    node.SelectedImageIndex = 5;
                    break;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            treeViewMain_DoubleClick(sender, e);
        }

        [Obsolete] // the move functions are to eventually be removed in favor of drag and drop
        private void moveFile(int amount)
        {
            if (treeViewMain.SelectedNode is not TreeNode t || t.Tag is null) return;

            var file = t.Tag as PckAsset;
            var path = t.FullPath;

            // skin and cape files only
            if (!(file.Type == PckAssetType.SkinFile || file.Type == PckAssetType.CapeFile)) return;

            PckFile pck = _pck;
            bool IsSubPCK = IsSubPCKNode(path);
            if (IsSubPCK)
            {
                using (var stream = new MemoryStream((GetSubPCK(path).Tag as PckAsset).Data))
                {
                    var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    pck = reader.FromStream(stream);
                }
            }

            int index = pck.IndexOfAsset(file);

            if (index + amount < 0 || index + amount > pck.AssetCount)
                return;
            pck.RemoveAsset(file);
            pck.InsertAsset(index + amount, file);

            if (IsSubPCK)
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new PckFileWriter(pck, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    writer.WriteToStream(stream);
                    (GetSubPCK(path).Tag as PckAsset).SetData(stream.ToArray());
                }
            }
            BuildMainTreeView();
            _wasModified = true;
        }

        [Obsolete]
        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e) => moveFile(-1);
        [Obsolete]
        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e) => moveFile(1);


        private void SetPckEndianness(OMI.Endianness endianness)
        {
            try
            {
                if (treeViewMain.SelectedNode.Tag is PckAsset file && (file.Type is PckAssetType.AudioFile || file.Type is PckAssetType.SkinDataFile || file.Type is PckAssetType.TexturePackInfoFile))
                {
                    IDataFormatReader reader = file.Type is PckAssetType.AudioFile
                        ? new PckAudioFileReader(endianness == OMI.Endianness.BigEndian ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian)
                        : new PckFileReader(endianness == OMI.Endianness.BigEndian ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    object pck = reader.FromStream(new MemoryStream(file.Data));

                    IDataFormatWriter writer = file.Type is PckAssetType.AudioFile
                        ? new PckAudioFileWriter((PckAudioFile)pck, endianness)
                        : new PckFileWriter((PckFile)pck, endianness);
                    file.SetData(writer);
                    _wasModified = true;
                    MessageBox.Show($"\"{file.Filename}\" successfully converted to {(endianness == OMI.Endianness.LittleEndian ? "little" : "big")} endian.", "Converted PCK file");
                }
            }
            catch (OverflowException)
            {
                MessageBox.Show(this, $"File was not a valid {(endianness != OMI.Endianness.LittleEndian ? "little" : "big")} endian PCK File.", "Not a valid PCK file");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Not a valid PCK file");
                return;
            }
        }

        private void littleEndianToolStripMenuItem_Click(object sender, EventArgs e) => SetPckEndianness(OMI.Endianness.LittleEndian);
        private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e) => SetPckEndianness(OMI.Endianness.BigEndian);

        private void SetModelVersion(int version)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset file && file.Type is PckAssetType.ModelsFile)
            {
                try
                {
                    ModelContainer container = file.GetData(new ModelFileReader());

                    if (container.Version == version)
                    {
                        MessageBox.Show(
                            this,
                            $"This model container is already Version {version + 1}.",
                            "Can't convert", MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                        return;
                    }

                    if (version == 2 &&
                        MessageBox.Show(
                            this,
                            "Conversion to 1.14 models.bin format does not yet support parent declaration and may not be 100% accurate.\n" +
                            "Would you like to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes
                        )
                    {
                        return;
                    }

                    if (container.Version > 1 &&
                        MessageBox.Show(
                            this,
                            "Conversion from 1.14 models.bin format does not yet support parent parts and may not be 100% accurate.\n" +
                            "Would you like to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes
                            )
                    {
                        return;
                    }

                    file.SetData(new ModelFileWriter(container, version));
                    _wasModified = true;
                    MessageBox.Show(
                        this,
                        $"\"{file.Filename}\" successfully converted to Version {version + 1} format.",
                        "Converted model container file"
                        );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Not a valid model container file.");
                    return;
                }
            }
        }

        private void setModelVersion1ToolStripMenuItem_Click(object sender, EventArgs e) => SetModelVersion(0);

        private void setModelVersion2ToolStripMenuItem_Click(object sender, EventArgs e) => SetModelVersion(1);

        private void setModelVersion3ToolStripMenuItem_Click(object sender, EventArgs e) => SetModelVersion(2);

    }
}