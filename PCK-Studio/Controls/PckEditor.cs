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
using PckStudio.Forms.Utilities;
using PckStudio.Classes.IO._3DST;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.Misc;

using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using OMI.Workers.Pck;

namespace PckStudio.Controls
{
    public partial class PckEditor : UserControl, IPckEditor
    {
        public PckFile Pck => _pck;

        private PckFile _pck;
        private string _location = string.Empty;
        private bool _wasModified = false;
        private bool _isTemplateFile = false;
        private int _timesSaved = 0;

        private readonly Dictionary<PckFile.FileData.FileType, Action<PckFile.FileData>> pckFileTypeHandler;

        public PckEditor()
        {
            InitializeComponent();

            skinToolStripMenuItem1.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.SkinFile);
            capeToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.CapeFile);
            textureToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.TextureFile);
            languagesFileLOCToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.LocalisationFile);
            gameRulesFileGRFToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.GameRulesFile);
            audioPCKFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.AudioFile);
            coloursCOLFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.ColourTableFile);
            gameRulesHeaderGRHToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.GameRulesHeader);
            skinsPCKToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.SkinDataFile);
            modelsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.ModelsFile);
            behavioursFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.BehavioursFile);
            entityMaterialsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PckFile.FileData.FileType.MaterialFile);

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

            pckFileTypeHandler = new Dictionary<PckFile.FileData.FileType, Action<PckFile.FileData>>(15)
            {
                [PckFile.FileData.FileType.SkinFile] = HandleSkinFile,
                [PckFile.FileData.FileType.CapeFile] = null,
                [PckFile.FileData.FileType.TextureFile] = HandleTextureFile,
                [PckFile.FileData.FileType.UIDataFile] = _ => throw new NotSupportedException("unused in-game"),
                [PckFile.FileData.FileType.InfoFile] = null,
                [PckFile.FileData.FileType.TexturePackInfoFile] = null,
                [PckFile.FileData.FileType.LocalisationFile] = HandleLocalisationFile,
                [PckFile.FileData.FileType.GameRulesFile] = HandleGameRuleFile,
                [PckFile.FileData.FileType.AudioFile] = HandleAudioFile,
                [PckFile.FileData.FileType.ColourTableFile] = HandleColourFile,
                [PckFile.FileData.FileType.GameRulesHeader] = HandleGameRuleFile,
                [PckFile.FileData.FileType.SkinDataFile] = null,
                [PckFile.FileData.FileType.ModelsFile] = HandleModelsFile,
                [PckFile.FileData.FileType.BehavioursFile] = HandleBehavioursFile,
                [PckFile.FileData.FileType.MaterialFile] = HandleMaterialFile,
            };
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
            if (_pck.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out PckFile.FileData file))
            {
                file.Properties.RemoveAll(t => t.Key.Equals("LOCK"));
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
            foreach (var file in pckFile.Files)
            {
                // fix any file paths that may be incorrect
                if (file.Filename.StartsWith(parentPath))
                    file.Filename = file.Filename.Remove(0, parentPath.Length);
                TreeNode node = BuildNodeTreeBySeperator(root, file.Filename, '/');
                node.Tag = file;
                if (Settings.Default.LoadSubPcks &&
                    (file.Filetype == PckFile.FileData.FileType.SkinDataFile || file.Filetype == PckFile.FileData.FileType.TexturePackInfoFile) &&
                    file.Data.Length > 0)
                {
                    using (var stream = new MemoryStream(file.Data))
                    {
                        try
                        {
                            var reader = new PckFileReader(GetEndianess());
                            PckFile subPCKfile = reader.FromStream(stream);
                            // passes parent path to remove from sub pck filepaths
                            BuildPckTreeView(node.Nodes, subPCKfile, file.Filename + "/");
                        }
                        catch (OverflowException ex)
                        {
                            MessageBox.Show("Failed to open pck\n" +
                                "Try checking the 'Open/Save as Switch/Vita/PS4 pck' checkbox in the upper right corner.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                SetPckFileIcon(node, file.Filetype);
            };
        }

        private void SetPckFileIcon(TreeNode node, PckFile.FileData.FileType type)
        {
            switch (type)
            {
                case PckFile.FileData.FileType.AudioFile:
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    break;
                case PckFile.FileData.FileType.LocalisationFile:
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    break;
                case PckFile.FileData.FileType.TexturePackInfoFile:
                    node.ImageIndex = 4;
                    node.SelectedImageIndex = 4;
                    break;
                case PckFile.FileData.FileType.ColourTableFile:
                    node.ImageIndex = 6;
                    node.SelectedImageIndex = 6;
                    break;
                case PckFile.FileData.FileType.ModelsFile:
                    node.ImageIndex = 8;
                    node.SelectedImageIndex = 8;
                    break;
                case PckFile.FileData.FileType.SkinDataFile:
                    node.ImageIndex = 7;
                    node.SelectedImageIndex = 7;
                    break;
                case PckFile.FileData.FileType.GameRulesFile:
                    node.ImageIndex = 9;
                    node.SelectedImageIndex = 9;
                    break;
                case PckFile.FileData.FileType.GameRulesHeader:
                    node.ImageIndex = 10;
                    node.SelectedImageIndex = 10;
                    break;
                case PckFile.FileData.FileType.InfoFile:
                    node.ImageIndex = 11;
                    node.SelectedImageIndex = 11;
                    break;
                case PckFile.FileData.FileType.SkinFile:
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                    break;
                case PckFile.FileData.FileType.CapeFile:
                    node.ImageIndex = 13;
                    node.SelectedImageIndex = 13;
                    break;
                case PckFile.FileData.FileType.TextureFile:
                    node.ImageIndex = 14;
                    node.SelectedImageIndex = 14;
                    break;
                case PckFile.FileData.FileType.BehavioursFile:
                    node.ImageIndex = 15;
                    node.SelectedImageIndex = 15;
                    break;
                case PckFile.FileData.FileType.MaterialFile:
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

            if (_isTemplateFile && _pck.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile))
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
                if (parent.Tag is PckFile.FileData f &&
                    (f.Filetype is PckFile.FileData.FileType.TexturePackInfoFile ||
                     f.Filetype is PckFile.FileData.FileType.SkinDataFile))
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

            PckFile.FileData parent_file = parent.Tag as PckFile.FileData;
            if (parent_file.Filetype is PckFile.FileData.FileType.TexturePackInfoFile || parent_file.Filetype is PckFile.FileData.FileType.SkinDataFile)
            {
                Console.WriteLine("Rebuilding " + parent_file.Filename);
                PckFile newPCKFile = new PckFile(3, parent_file.Filetype is PckFile.FileData.FileType.SkinDataFile);

                foreach (TreeNode node in GetAllChildNodes(parent.Nodes))
                {
                    if (node.Tag is PckFile.FileData node_file)
                    {
                        PckFile.FileData new_file = newPCKFile.CreateNewFile(node_file.Filename.Replace(parent_file.Filename + "/", String.Empty), node_file.Filetype);
                        foreach (var prop in node_file.Properties) new_file.Properties.Add(prop);
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
            if (!_pck.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locdata) &&
                !_pck.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locdata))
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
            if (!_pck.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locdata) &&
                !_pck.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locdata))
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
                node.Tag is PckFile.FileData file)
            {
                foreach (var property in file.Properties)
                {
                    treeMeta.Nodes.Add(CreateNode(property.Key, property));
                }
            }
        }

        private PckFile.FileData CreateNewAudioFile(bool isLittle)
        {
            PckAudioFile audioPck = new PckAudioFile();
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Overworld);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Nether);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.End);
            PckFile.FileData pckFileData = _pck.CreateNewFile("audio.pck", PckFile.FileData.FileType.AudioFile, () =>
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new PckAudioFileWriter(audioPck, isLittle ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    writer.WriteToStream(stream);
                    return stream.ToArray();
                }
            });
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
                    PckFile.FileData file = _pck.CreateNewFile(
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
                    var file = _pck.CreateNewFile(renamePrompt.NewText, PckFile.FileData.FileType.TextureFile);
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
                    PckFile.FileData mfNew = _pck.CreateNewFile(skinNameImport, PckFile.FileData.FileType.SkinFile);
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
                                mfNew.Properties.Add(new KeyValuePair<string, string>(key, value));
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
                    if (node.Tag is PckFile.FileData fd &&
                        (fd.Filetype != PckFile.FileData.FileType.TexturePackInfoFile &&
                        fd.Filetype != PckFile.FileData.FileType.SkinDataFile))
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

        private void setFileType_Click(object sender, EventArgs e, PckFile.FileData.FileType type)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
            {
                Debug.WriteLine($"Setting {file.Filetype} to {type}");
                file.Filetype = type;
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
            if (e.Node is TreeNode t && t.Tag is PckFile.FileData file)
            {
                viewFileInfoToolStripMenuItem.Visible = true;
                if (file.Properties.HasProperty("BOX"))
                {
                    buttonEdit.Text = "EDIT BOXES";
                    buttonEdit.Visible = true;
                }
                else if (file.Properties.HasProperty("ANIM") &&
                        file.Properties.GetPropertyValue("ANIM", s => SkinANIM.FromString(s) == (ANIM_EFFECTS.RESOLUTION_64x64 | ANIM_EFFECTS.SLIM_MODEL)))
                {
                    buttonEdit.Text = "View Skin";
                    buttonEdit.Visible = true;
                }

                switch (file.Filetype)
                {
                    case PckFile.FileData.FileType.SkinFile:
                    case PckFile.FileData.FileType.CapeFile:
                    case PckFile.FileData.FileType.TextureFile:
                        {
                            // TODO: Add tga support
                            if (Path.GetExtension(file.Filename) == ".tga") break;
                            using MemoryStream stream = new MemoryStream(file.Data);

                            var img = Image.FromStream(stream);

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
                                file.Filetype == PckFile.FileData.FileType.TextureFile
                                && !file.IsMipmappedFile())
                            {
                                buttonEdit.Text = "EDIT TILE ANIMATION";
                                buttonEdit.Visible = true;
                            }
                        }
                        break;

                    case PckFile.FileData.FileType.LocalisationFile:
                        buttonEdit.Text = "EDIT LOC";
                        buttonEdit.Visible = true;
                        break;

                    case PckFile.FileData.FileType.AudioFile:
                        buttonEdit.Text = "EDIT MUSIC CUES";
                        buttonEdit.Visible = true;
                        break;

                    case PckFile.FileData.FileType.ColourTableFile when file.Filename == "colours.col":
                        buttonEdit.Text = "EDIT COLORS";
                        buttonEdit.Visible = true;
                        break;

                    case PckFile.FileData.FileType.BehavioursFile when file.Filename == "behaviours.bin":
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
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
            {
                pckFileTypeHandler[file.Filetype]?.Invoke(file);
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

                    if (_pck.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile)) // Prioritize Skins.pck
                    {
                        TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
                        if (subPCK.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/");
                        add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins.pck/");
                        TreeNode newNode = new TreeNode(Path.GetFileName(add.SkinFile.Filename));
                        newNode.Tag = add.SkinFile;
                        SetPckFileIcon(newNode, PckFile.FileData.FileType.SkinFile);
                        subPCK.Nodes.Add(newNode);
                        RebuildSubPCK(newNode.FullPath);
                    }
                    else
                    {
                        if (treeViewMain.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/"); // Then Skins folder
                        _pck.Files.Add(add.SkinFile);
                    }
                    if (add.HasCape)
                    {
                        if (_pck.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile)) // Prioritize Skins.pck
                        {
                            TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
                            if (subPCK.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/");
                            add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins.pck/");
                            TreeNode newNode = new TreeNode(Path.GetFileName(add.CapeFile.Filename));
                            newNode.Tag = add.CapeFile;
                            SetPckFileIcon(newNode, PckFile.FileData.FileType.SkinFile);
                            subPCK.Nodes.Add(newNode);
                            RebuildSubPCK(newNode.FullPath);
                        }
                        else
                        {
                            if (treeViewMain.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
                            _pck.Files.Add(add.CapeFile);
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

            var file = new PckFile.FileData(
                $"res/textures/{AnimationResources.GetAnimationSection(diag.Category)}/{diag.SelectedTile}.png",
                PckFile.FileData.FileType.TextureFile);

            using AnimationEditor animationEditor = new AnimationEditor(file);
            if (animationEditor.ShowDialog() == DialogResult.OK)
            {
                _wasModified = true;
                _pck.Files.Add(file);
                BuildMainTreeView();
                ReloadMetaTreeView();
            }
        }

        private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (pck.Files.Contains(file => file.Filetype == PckFile.FileData.FileType.AudioFile) != -1)
            //{
            //	MessageBox.Show("There is already an music cues PCK present in this PCK!", "Can't create audio.pck");
            //	return;
            //}
            if (_pck.Files.Contains("audio.pck", PckFile.FileData.FileType.AudioFile))
            {
                // the chance of this happening is really really slim but just in case
                MessageBox.Show($"There is already a file of type \"{nameof(PckFile.FileData.FileType.AudioFile)}\" and name \"audio.pck\" in this PCK!", "Can't create audio.pck");
                return;
            }
            if (string.IsNullOrEmpty(_location))
            {
                MessageBox.Show("You must save your pck before creating or opening a music cues PCK file", "Can't create audio.pck");
                return;
            }

            var file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
            AudioEditor diag = new AudioEditor(file, LittleEndianCheckBox.Checked);
            if (diag.ShowDialog(this) != DialogResult.OK)
            {
                _pck.Files.Remove(file); // delete file if not saved
            }
            diag.Dispose();
            BuildMainTreeView();
        }

        private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetFile("colours.col", PckFile.FileData.FileType.ColourTableFile, out _))
            {
                MessageBox.Show("A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            var newColorFile = _pck.CreateNewFile("colours.col", PckFile.FileData.FileType.ColourTableFile);
            newColorFile.SetData(Resources.tu69colours);
            BuildMainTreeView();
        }

        private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile, out _))
            {
                MessageBox.Show("A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            _pck.CreateNewFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile, () =>
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
            if (_pck.TryGetFile("behaviours.bin", PckFile.FileData.FileType.BehavioursFile, out _))
            {
                MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            _pck.CreateNewFile("behaviours.bin", PckFile.FileData.FileType.BehavioursFile, BehaviourResources.BehaviourFileInitializer);
            BuildMainTreeView();
        }

        private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pck.TryGetFile("entityMaterials.bin", PckFile.FileData.FileType.MaterialFile, out _))
            {
                MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            _pck.CreateNewFile("entityMaterials.bin", PckFile.FileData.FileType.MaterialFile, MaterialResources.MaterialsFileInitializer);
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
                bool hasSkinsPck = _pck.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile);

                foreach (var fullfilename in Directory.GetFiles(contents.SelectedPath, "*.png"))
                {
                    string filename = Path.GetFileNameWithoutExtension(fullfilename);
                    // sets file type based on wether its a cape or skin
                    PckFile.FileData.FileType pckfiletype = filename.StartsWith("dlccape", StringComparison.OrdinalIgnoreCase)
                        ? PckFile.FileData.FileType.CapeFile
                        : PckFile.FileData.FileType.SkinFile;
                    string pckfilepath = (hasSkinsPck ? "Skins/" : string.Empty) + filename + ".png";


                    PckFile.FileData newFile = new PckFile.FileData(pckfilepath, pckfiletype);
                    byte[] filedata = File.ReadAllBytes(fullfilename);
                    newFile.SetData(filedata);

                    if (File.Exists(fullfilename + ".txt"))
                    {
                        string[] properties = File.ReadAllText(fullfilename + ".txt").Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string property in properties)
                        {
                            string[] param = property.Split(':');
                            if (param.Length < 2) continue;
                            newFile.Properties.Add((param[0], param[1]));
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
                        var skinsfile = _pck.GetFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile);
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
                    _pck.Files.Add(newFile);
                }
                BuildMainTreeView();
                _wasModified = true;
            }
        }

        private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckFile.FileData file &&
                file.Filetype == PckFile.FileData.FileType.SkinFile)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "3DS Texture|*.3dst";
                saveFileDialog.DefaultExt = ".3dst";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var ms = new MemoryStream(file.Data))
                    {
                        Image img = Image.FromStream(ms);
                        var writer = new _3DSTextureWriter(img);
                        writer.WriteToFile(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckFile.FileData file && file.Filetype == PckFile.FileData.FileType.TextureFile)
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
                        if (_pck.HasFile(mippedPath, PckFile.FileData.FileType.TextureFile))
                            _pck.Files.Remove(_pck.GetFile(mippedPath, PckFile.FileData.FileType.TextureFile));
                        PckFile.FileData MipMappedFile = new PckFile.FileData(mippedPath, PckFile.FileData.FileType.TextureFile);


                        Image originalTexture = Image.FromStream(new MemoryStream(file.Data));
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

                        _pck.Files.Insert(_pck.Files.IndexOf(file) + i - 1, MipMappedFile);
                    }
                    BuildMainTreeView();
                }
            }
        }

        private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckFile.FileData file)
            {
                MessageBox.Show(
                    "File path: " + file.Filename +
                    "\nAssigned File type: " + (int)file.Filetype + " (" + file.Filetype + ")" +
                    "\nFile size: " + file.Size +
                    "\nProperties count: " + file.Properties.Count
                    , Path.GetFileName(file.Filename) + " file info");
            }
        }

        private void correctSkinDecimalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFile.FileData file && file.Filetype == PckFile.FileData.FileType.SkinFile)
            {
                foreach (var p in file.Properties.FindAll(s => s.Key == "BOX" || s.Key == "OFFSET"))
                {
                    file.Properties[file.Properties.IndexOf(p)] = new KeyValuePair<string, string>(p.Key, p.Value.Replace(',', '.'));
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
            if (node.Tag is PckFile.FileData file)
            {
                using SaveFileDialog exFile = new SaveFileDialog();
                exFile.FileName = Path.GetFileName(file.Filename);
                exFile.Filter = Path.GetExtension(file.Filename).Replace(".", string.Empty) + " File|*" + Path.GetExtension(file.Filename);
                if (exFile.ShowDialog() != DialogResult.OK ||
                    // Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
                    string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName))) return;
                string extractFilePath = exFile.FileName;

                File.WriteAllBytes(extractFilePath, file.Data);
                if (file.Properties.Count > 0)
                {
                    using var fs = File.CreateText($"{extractFilePath}.txt");
                    file.Properties.ForEach(property => fs.WriteLine($"{property.Key}: {property.Value}"));
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
                            if (fileNode.Tag is PckFile.FileData file)
                            {
                                Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(file.Filename)}");
                                File.WriteAllBytes($"{dialog.SelectedPath}/{file.Filename}", file.Data);
                                if (file.Properties.Count > 0)
                                {
                                    using var fs = File.CreateText($"{dialog.SelectedPath}/{file.Filename}.txt");
                                    file.Properties.ForEach(property => fs.WriteLine($"{property.Key}: {property.Value}"));
                                }
                            }
                        }
                        );
                    }
                    else
                    {
                        foreach (var _file in _pck.Files)
                        {
                            if (_file.Filename.StartsWith(selectedFolder))
                            {
                                Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(_file.Filename)}");
                                File.WriteAllBytes($"{dialog.SelectedPath}/{_file.Filename}", _file.Data);
                                if (_file.Properties.Count > 0)
                                {
                                    using var fs = File.CreateText($"{dialog.SelectedPath}/{_file.Filename}.txt");
                                    _file.Properties.ForEach(property => fs.WriteLine($"{property.Key}: {property.Value}"));
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
                if (node.Tag is PckFile.FileData file)
                {
                    TreeNode newNode = new TreeNode();
                    newNode.Text = Path.GetFileName(diag.NewText);
                    var NewFile = new PckFile.FileData(diag.NewText, file.Filetype);
                    file.Properties.ForEach(p => NewFile.Properties.Add(p));
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

                    if (!IsSubPCKNode(node.FullPath)) _pck.Files.Insert(node.Index + 1, NewFile);
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
                if (node.Tag is PckFile.FileData file)
                {
                    file.Filename = diag.NewText;
                }
                else // folders
                {
                    node.Text = diag.NewText;
                    foreach (var childNode in GetAllChildNodes(node.Nodes))
                    {
                        if (childNode.Tag is PckFile.FileData folderFile)
                        {
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
            if (treeViewMain.SelectedNode.Tag is PckFile.FileData file)
            {
                using var ofd = new OpenFileDialog();
                // Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.

                string extra_extensions = "";

                switch (file.Filetype)
                {
                    case PckFile.FileData.FileType.TextureFile:
                        if (Path.GetExtension(file.Filename) == ".png") extra_extensions = ";*.tga";
                        else if (Path.GetExtension(file.Filename) == ".tga") extra_extensions = ";*.png";
                        break;
                }

                string fileExt = Path.GetExtension(file.Filename);

                ofd.Filter = $"{file.Filetype} (*{fileExt}{extra_extensions})|*{fileExt}{extra_extensions}";
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

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = treeViewMain.SelectedNode;
            if (node == null) return;

            string path = node.FullPath;

            if (node.Tag is PckFile.FileData)
            {
                PckFile.FileData file = node.Tag as PckFile.FileData;

                string itemPath = "res/textures/items/";

                // warn the user about deleting compass.png and clock.png
                if (file.Filetype == PckFile.FileData.FileType.TextureFile &&
                    (file.Filename == itemPath + "compass.png" || file.Filename == itemPath + "clock.png"))
                {
                    if (MessageBox.Show("Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
                }

                // remove loc key if its a skin/cape
                if (file.Filetype == PckFile.FileData.FileType.SkinFile || file.Filetype == PckFile.FileData.FileType.CapeFile)
                {
                    if (TryGetLocFile(out LOCFile locFile))
                    {
                        foreach (var property in file.Properties)
                        {
                            if (property.Key == "THEMENAMEID" || property.Key == "DISPLAYNAMEID")
                                locFile.RemoveLocKey(property.Value);
                        }
                        TrySetLocFile(locFile);
                    }
                }
                if (_pck.Files.Remove(file))
                {
                    node.Remove();
                    _wasModified = true;
                }
            }
            else if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string pckFolderDir = node.FullPath;
                _pck.Files.RemoveAll(file => file.Filename.StartsWith(pckFolderDir));
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
            //e.CancelEdit = e.Node.Tag is PckFile.FileData;
            e.CancelEdit = true;
        }

        private void editAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckFile.FileData file)
            {
                var props = file.Properties.Select(p => p.Key + " " + p.Value);
                using (var input = new MultiTextPrompt(props.ToArray()))
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        file.Properties.Clear();
                        foreach (var line in input.TextOutput)
                        {
                            int idx = line.IndexOf(' ');
                            if (idx == -1 || line.Length - 1 == idx)
                                continue;
                            file.Properties.Add((line.Substring(0, idx).Replace(":", string.Empty), line.Substring(idx + 1)));
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
                treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFile.FileData file)
            {
                int i = file.Properties.IndexOf(property);
                if (i != -1)
                {
                    switch (property.Key)
                    {
                        case "ANIM" when file.Filetype == PckFile.FileData.FileType.SkinFile:
                            try
                            {
                                using ANIMEditor diag = new ANIMEditor(property.Value);
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    file.Properties[i] = new KeyValuePair<string, string>("ANIM", diag.ResultAnim.ToString());
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

                        case "BOX" when file.Filetype == PckFile.FileData.FileType.SkinFile:
                            try
                            {
                                using BoxEditor diag = new BoxEditor(property.Value, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    file.Properties[i] = new KeyValuePair<string, string>("BOX", diag.Result.ToString());
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
                            file.Properties[i] = addProperty.Property;
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
                node.Tag is PckFile.FileData file)
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
                            file.Properties.Add((line.Substring(0, idx), line.Substring(idx + 1)));
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
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
            {
                using BoxEditor diag = new BoxEditor(SkinBOX.Empty, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    file.Properties.Add("BOX", diag.Result);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
                return;
            }
        }

        private void addANIMEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
            {
                using ANIMEditor diag = new ANIMEditor(SkinANIM.Empty);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    file.Properties.Add("ANIM", diag.ResultAnim);
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
                treeViewMain.SelectedNode is TreeNode main && main.Tag is PckFile.FileData file &&
                file.Properties.Remove(property))
            {
                treeMeta.SelectedNode.Remove();
                RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                _wasModified = true;
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t &&
                t.Tag is PckFile.FileData file)
            {
                using AddPropertyPrompt addProperty = new AddPropertyPrompt();
                if (addProperty.ShowDialog() == DialogResult.OK)
                {
                    file.Properties.Add(addProperty.Property);
                    RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
            }
        }

        private void HandleTextureFile(PckFile.FileData file)
        {
            if (!(file.Filename.StartsWith("res/textures/blocks/") || file.Filename.StartsWith("res/textures/items/")))
                return;

            if (file.IsMipmappedFile() && _pck.Files.TryGetValue(file.GetNormalPath(), PckFile.FileData.FileType.TextureFile, out PckFile.FileData originalAnimationFile))
            {
                file = originalAnimationFile;
            }

            using (AnimationEditor animationEditor = new AnimationEditor(file))
            {
                if (animationEditor.ShowDialog(this) == DialogResult.OK)
                {
                    _wasModified = true;
                    BuildMainTreeView();
                }
            }
        }

        private void HandleGameRuleFile(PckFile.FileData file)
        {
            using GameRuleFileEditor grfEditor = new GameRuleFileEditor(file);
            _wasModified = grfEditor.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleAudioFile(PckFile.FileData file)
        {
            using AudioEditor audioEditor = new AudioEditor(file, LittleEndianCheckBox.Checked);
            _wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleLocalisationFile(PckFile.FileData file)
        {
            using LOCEditor locedit = new LOCEditor(file);
            _wasModified = locedit.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleColourFile(PckFile.FileData file)
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

        public void HandleSkinFile(PckFile.FileData file)
        {
            if (file.Size <= 0)
                return;
            using (var ms = new MemoryStream(file.Data))
            {
                var texture = Image.FromStream(ms);
                if (file.Properties.HasProperty("BOX"))
                {
                    using generateModel generate = new generateModel(file.Properties, texture);
                    if (generate.ShowDialog() == DialogResult.OK)
                    {
                        entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
                        _wasModified = true;
                        ReloadMetaTreeView();
                    }
                }
                else
                {
                    SkinPreview frm = new SkinPreview(texture, file.Properties.GetPropertyValue("ANIM", SkinANIM.FromString));
                    frm.ShowDialog(this);
                    frm.Dispose();
                }
            }
        }

        public void HandleModelsFile(PckFile.FileData file)
        {
            MessageBox.Show("Models.bin support has not been implemented. You can use the Spark Editor for the time being to edit these files.", "Not implemented yet.");
        }

        public void HandleBehavioursFile(PckFile.FileData file)
        {
            using BehaviourEditor edit = new BehaviourEditor(file);
            _wasModified = edit.ShowDialog(this) == DialogResult.OK;
        }

        public void HandleMaterialFile(PckFile.FileData file)
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
            if (_wasModified &&
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
    }
}