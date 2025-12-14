using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using MetroFramework.Forms;

using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using OMI.Workers.Pck;

using OMI.Workers;
using OMI.Formats.Model;
using OMI.Workers.Model;
using OMI.Formats.GameRule;
using OMI.Workers.GameRule;
using OMI.Formats.Material;
using OMI.Workers.Material;
using OMI.Formats.Behaviour;
using OMI.Workers.Behaviour;
using OMI.Formats.Color;
using OMI.Workers.Color;

using PckStudio.Core.Extensions;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Interfaces;
using PckStudio.Internal;
using PckStudio.Popups;
using PckStudio.Properties;

using PckStudio.Core.Deserializer;
using PckStudio.Core.Serializer;
using PckStudio.Core.Json;
using PckStudio.Core.FileFormats;
using PckStudio.Core.Skin;
using PckStudio.Rendering;
using PckStudio.Core;
using PckStudio.ModelSupport;
using PckStudio.Json;
using PckStudio.Core.IO.PckAudio;
using PckStudio.Core.IO._3DST;
using PckStudio.Core.Misc;

namespace PckStudio.Controls
{
    internal partial class PckEditor : EditorControl<PackInfo>
    {

        private string _location = string.Empty;

        private readonly OMI.ByteOrder _originalEndianness;
        private OMI.ByteOrder _currentEndianness;
        private bool __modified = false;
        private bool _wasModified
        {
            get => __modified;
            set
            {
                if (__modified != value)
                {
                    __modified = value;
                    _onModifiedChangeDelegate?.Invoke(value);
                }
            }
        }

        private delegate void OnModifiedChangeDelegate(bool state);
        private OnModifiedChangeDelegate _onModifiedChangeDelegate;

        private int _timesSaved = 0;

        private readonly Dictionary<PckAssetType, Action<PckAsset>> _pckAssetTypeHandler;

        public PckEditor(PackInfo packInfo, ISaveContext<PackInfo> saveContext)
            : base(packInfo, saveContext)
        {
            InitializeComponent();
            _onModifiedChangeDelegate = OnModify;
            _originalEndianness = packInfo.Endianness;
            _currentEndianness = packInfo.Endianness;

            LittleEndianCheckBox.Visible = packInfo.AllowEndianSwap;

            treeViewMain.TreeViewNodeSorter = new PckNodeSorter();

            skinToolStripMenuItem1.Click += (sender, e) => SetFileType(PckAssetType.SkinFile);
            capeToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.CapeFile);
            textureToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.TextureFile);
            languagesFileLOCToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.LocalisationFile);
            gameRulesFileGRFToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.GameRulesFile);
            audioPCKFileToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.AudioFile);
            coloursCOLFileToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.ColourTableFile);
            gameRulesHeaderGRHToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.GameRulesHeader);
            skinsPCKToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.SkinDataFile);
            modelsFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.ModelsFile);
            behavioursFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.BehavioursFile);
            entityMaterialsFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckAssetType.MaterialFile);

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

            _pckAssetTypeHandler = new Dictionary<PckAssetType, Action<PckAsset>>(15)
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
        
        public new void Save()
        {
            base.Save();
            _timesSaved++;
            _wasModified = false;
        }

        public override void SaveAs()
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

        public override void Close()
        {
            if (_wasModified && MessageBox.Show("Save PCK?", "Modified PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                Save();
        }

        private void OnModify(bool state)
        {
            pckFileLabel.Text = state && !pckFileLabel.Text.StartsWith("*") ? "*" + pckFileLabel.Text : pckFileLabel.Text.Substring(1);
        }

        public override void UpdateView()
        {
            BuildMainTreeView();
        }

        private void SaveTo(string filepath)
        {
            _location = filepath;
            Save();
        }

        private void HandleInnerPckFile(PckAsset asset)
        {
            if (asset.Type != PckAssetType.SkinDataFile && asset.Type != PckAssetType.TexturePackInfoFile || asset.Size <= 0 || !Settings.Default.LoadSubPcks)
                return;
            
            ISaveContext<PackInfo> saveContext = new DelegatedSaveContext<PackInfo>(false, (packInfo) =>
            {
                if (packInfo.IsValid)
                {
                    asset.SetData(new PckFileWriter(packInfo.File, _currentEndianness));
                    _wasModified = true;
                }
            });

            string caption = Path.GetFileName(asset.Filename);
            string identifier = _location + Path.GetFileName(asset.Filename);
            PckFile pckFile = asset.GetData(new PckFileReader(_originalEndianness));
            PackInfo packInfo = PackInfo.Create(pckFile, _originalEndianness, false);

            // TODO: may change to use a new tab page that will be closed when the main pck is closed
            //Program.MainInstance.OpenNewPckTab(caption, identifier, packInfo, saveContext);
        }

        private void HandleTextureFile(PckAsset asset)
        {
            _ = asset.IsMipmappedFile() && EditorValue.File.TryGetAsset(asset.GetNormalPath(), PckAssetType.TextureFile, out asset);

            if (asset.Size <= 0)
            {
                Trace.TraceInformation($"[{nameof(PckEditor)}:{nameof(HandleTextureFile)}] '{asset.Filename}' size is 0.");
                return;
            }

            ResourceLocation resourceLocation = ResourceLocation.GetFromPath(asset.Filename);
            Debug.WriteLine("Handling Resource file: " + resourceLocation?.ToString());

            switch (resourceLocation.Category)
            {
                case ResourceCategory.Unknown:
                    Debug.WriteLine($"Unknown Resource Category.");
                    break;
                case ResourceCategory.MobEntityTextures:
                case ResourceCategory.ItemEntityTextures:
                {
                    string texturePath = asset.Filename.Substring(0, asset.Filename.Length - Path.GetExtension(asset.Filename).Length);
                    string[] modelNames = GameModelImporter.ModelMetaData.Where(kv => kv.Value.TextureLocations.Contains(texturePath)).Select(kv => kv.Key).ToArray();

                    if (modelNames.Length == 0)
                    {
                        MessageBox.Show("No Model info found");
                        return;
                    }

                    string modelName = modelNames[0];
                    if (modelNames.Length > 1)
                    {
                        using ItemSelectionPopUp itemSelectionPopUp = new ItemSelectionPopUp(modelNames.ToArray());
                        itemSelectionPopUp.ButtonText = "View";
                        itemSelectionPopUp.LabelText = "Models:";
                        if (itemSelectionPopUp.ShowDialog() != DialogResult.OK || !modelNames.IndexInRange(itemSelectionPopUp.SelectedIndex))
                        {
                            return;
                        }
                        modelName = modelNames[itemSelectionPopUp.SelectedIndex];
                    }

                    Image texture = asset.GetTexture();
                    string textureName = Path.GetFileName(texturePath);

                    NamedData<Image> modelTexture = new NamedData<Image>(textureName, texture);

                    bool hasCustomModel = false;
                    bool hasDefaultModel = TryGetDefaultEntityModel(modelName, out Model model);
                    if (EditorValue.File.TryGetAsset("models.bin", PckAssetType.ModelsFile, out PckAsset modelsAsset))
                    {
                        ModelContainer models = modelsAsset.GetData(new ModelFileReader());
                        hasCustomModel = models.ContainsModel(modelName);
                        if (hasCustomModel)
                        {
                            Debug.WriteLine($"Custom model for '{modelName}' found.");
                            model = models.GetModelByName(modelName);
                        }
                    }
                    if (!hasDefaultModel && !hasCustomModel)
                    {
                        MessageBox.Show(this, $"Not Model found for: {modelName}");
                        return;
                    }

                    ShowSimpleModelRender(model, modelTexture);
                }
                break;

                case ResourceCategory.ItemAnimation:
                case ResourceCategory.BlockAnimation:
                    Animation animation = asset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
                    string internalName = Path.GetFileNameWithoutExtension(asset.Filename);
                    IList<JsonTileInfo> textureInfos = resourceLocation.Category == ResourceCategory.ItemAnimation ? Tiles.ItemTileInfos : Tiles.BlockTileInfos;
                    string displayname = textureInfos.FirstOrDefault(p => p.InternalName == internalName)?.DisplayName ?? internalName;

                    string[] specialTileNames = { "clock", "compass" };

                    ISaveContext<Animation> saveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
                    {
                        asset.SetSerializedData(animation, AnimationSerializer.DefaultSerializer);
                    });

                    using (AnimationEditor animationEditor = new AnimationEditor(animation, saveContext, displayname, !internalName.ToLower().EqualsAny(specialTileNames)))
                    {
                        if (animationEditor.ShowDialog(this) == DialogResult.OK)
                        {
                            _wasModified = true;
                            BuildMainTreeView();
                        }
                    }
                    break;
                case ResourceCategory.ParticleAtlas:
                case ResourceCategory.MoonPhaseAtlas:
                case ResourceCategory.ItemAtlas:
                case ResourceCategory.BlockAtlas:
                case ResourceCategory.BannerAtlas:
                case ResourceCategory.PaintingAtlas:
                case ResourceCategory.ExplosionAtlas:
                case ResourceCategory.ExperienceOrbAtlas:
                case ResourceCategory.MapIconAtlas:
                case ResourceCategory.AdditionalMapIconsAtlas:
                    Atlas atlas = asset.GetDeserializedData(new AtlasDeserializer(resourceLocation));
                    ColorContainer colorContainer = default;
                    if (EditorValue.File.TryGetAsset("colours.col", PckAssetType.ColourTableFile, out PckAsset colAsset))
                        colorContainer = colAsset.GetData(new COLFileReader());

                    ITryGet<string, Animation> tryGetAnimation = TryGet<string, Animation>.FromDelegate((string key, out Animation animation) =>
                    {
                        bool found = EditorValue.File.TryGetAsset(key + ".png", PckAssetType.TextureFile, out PckAsset foundAsset) ||
                                     EditorValue.File.TryGetAsset(key + ".tga", PckAssetType.TextureFile, out foundAsset);
                        if (found)
                        {
                            animation = foundAsset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
                            return true;
                        }
                        animation = default;
                        return false;
                    });

                    ITryGet<string, ISaveContext<Animation>> tryGetAnimationSaveContext = TryGet<string, ISaveContext<Animation>>
                        .FromDelegate((string key, out ISaveContext<Animation> animationSaveContext) =>
                        {
                            bool found = EditorValue.File.TryGetAsset(key + ".png", PckAssetType.TextureFile, out PckAsset foundAsset) ||
                                         EditorValue.File.TryGetAsset(key + ".tga", PckAssetType.TextureFile, out foundAsset);

                            if (found)
                            {
                                animationSaveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
                                    foundAsset.SetSerializedData(animation, AnimationSerializer.DefaultSerializer));
                                return true;
                            }

                            // you could validate the key(animationAssetPath) for validity. -miku
                            animationSaveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
                            {
                                if (animation.FrameCount == 0)
                                {
                                    Debug.WriteLine("New animation has 0 frames. Aborting saving.");
                                    return;
                                }
                                PckAsset newAnimationAsset = EditorValue.File.CreateNewAsset(key + ".png", PckAssetType.TextureFile);
                                newAnimationAsset.SetSerializedData(animation, AnimationSerializer.DefaultSerializer);
                                BuildMainTreeView();
                            });
                            return true;
                        });

                    ISaveContext<Atlas> textureAtlasSaveContext = new DelegatedSaveContext<Atlas>(Settings.Default.AutoSaveChanges, atlas => asset.SetTexture(atlas));

                    var viewer = new TextureAtlasEditor(atlas, textureAtlasSaveContext, resourceLocation, colorContainer, tryGetAnimation, tryGetAnimationSaveContext);
                    if (viewer.ShowDialog(this) == DialogResult.OK)
                    {
                        _wasModified = true;
                        BuildMainTreeView();
                    }
                    break;
                default:
                    Debug.WriteLine($"Unhandled Resource Category: {resourceLocation.Category}");
                    break;
            }
        }

        private void HandleGameRuleFile(PckAsset asset)
        {
            const string cDEFLATE = "PS3";
            const string cXMEM = "Xbox 360";
            const string cZLIB = "Other Platforms";

            ItemSelectionPopUp dialog = new ItemSelectionPopUp(cZLIB, cDEFLATE, cXMEM);
            dialog.LabelText = "Type";
            dialog.ButtonText = "Ok";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            GameRuleFile.CompressionType compressiontype = dialog.SelectedItem switch
            {
                cDEFLATE => GameRuleFile.CompressionType.Deflate,
                cXMEM => GameRuleFile.CompressionType.XMem,
                cZLIB => GameRuleFile.CompressionType.Zlib,
                _ => GameRuleFile.CompressionType.Unknown
            };

            GameRuleFile grf = asset.GetData(new GameRuleFileReader(compressiontype));

            ISaveContext<GameRuleFile> saveContext = new DelegatedSaveContext<GameRuleFile>(Settings.Default.AutoSaveChanges, (gameRuleFile) =>
            {
                asset.SetData(new GameRuleFileWriter(gameRuleFile));
            });

            using GameRuleFileEditor grfEditor = new GameRuleFileEditor(grf, saveContext);
            if (grfEditor.ShowDialog(this) == DialogResult.OK)
            {
                _wasModified = true;
                UpdateRichPresence();
            }
        }

        private void HandleAudioFile(PckAsset asset)
        {
            try
            {
                ISaveContext<PckAudioFile> saveContext = new DelegatedSaveContext<PckAudioFile>(Settings.Default.AutoSaveChanges, (audioFile) =>
                {
                    asset.SetData(new PckAudioFileWriter(audioFile, _currentEndianness));
                });
                PckAudioFile audioFile = asset.GetData(new PckAudioFileReader(_originalEndianness));
                using AudioEditor audioEditor = new AudioEditor(audioFile, saveContext);
                _wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
            }
            catch (OverflowException)
            {
                MessageBox.Show(this, $"Failed to open {asset.Filename}\n" +
                    "Try converting the file by using the \"Misc. Functions/Set PCK Endianness\" tool and try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open {asset.Filename}\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleLocalisationFile(PckAsset asset)
        {
            LOCFile locFile = asset.GetData(new LOCFileReader());
            ISaveContext<LOCFile> saveContext = new DelegatedSaveContext<LOCFile>(Settings.Default.AutoSaveChanges, (locFile) =>
            {
                asset.SetData(new LOCFileWriter(locFile, 2));
            });
            using LOCEditor locedit = new LOCEditor(locFile, saveContext);
            _wasModified = locedit.ShowDialog(this) == DialogResult.OK;
            UpdateRichPresence();
        }

        private void HandleColourFile(PckAsset asset)
        {
            ColorContainer colorContainer = asset.GetData(new COLFileReader());
            ISaveContext<ColorContainer> saveContext = new DelegatedSaveContext<ColorContainer>(Settings.Default.AutoSaveChanges, (colorContainer) =>
            {
                asset.SetData(new COLFileWriter(colorContainer));
            });
            using COLEditor diag = new COLEditor(colorContainer, saveContext);
            _wasModified = diag.ShowDialog(this) == DialogResult.OK;
        }

        private void HandleSkinFile(PckAsset asset)
        {
            Skin skin = asset.GetSkin();
            if (asset.HasProperty("CAPEPATH"))
            {
                string capeAssetPath = asset.GetProperty("CAPEPATH");
                if (EditorValue.File.TryGetAsset(capeAssetPath, PckAssetType.CapeFile, out PckAsset capeAsset))
                {
                    skin.CapeTexture = capeAsset.GetTexture();
                }
            }

            ISaveContext<Skin> saveContext = new DelegatedSaveContext<Skin>(Settings.Default.AutoSaveChanges, (customSkin) =>
            {
                if (!TryGetLocFile(out LOCFile locFile))
                    Debug.WriteLine("Failed to aquire loc file.");
                asset.SetSkin(customSkin, locFile);
            });

            using CustomSkinEditor skinEditor = new CustomSkinEditor(skin, saveContext, EditorValue.File.HasVerionString);
            if (skinEditor.ShowDialog() == DialogResult.OK)
            {
                entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
                _wasModified = true;
                ReloadMetaTreeView();
            }
        }

        private void HandleModelsFile(PckAsset asset)
        {
            ModelContainer modelContainer = asset.GetData(new ModelFileReader());
            if (modelContainer.ModelCount == 0)
            {
                MessageBox.Show("No models found.", "Empty Model file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TryGetDelegate<string, Image> tryGetTexture = (string path, out Image img) =>
            {
                bool found = EditorValue.File.TryGetAsset(path + ".png", PckAssetType.TextureFile, out PckAsset asset) ||
                             EditorValue.File.TryGetAsset(path + ".tga", PckAssetType.TextureFile, out asset);
                img = found ? asset.GetTexture() : default;
                return found;
            };

            TrySetDelegate<string, Image> trySetTexture = (string path, Image img) =>
            {
                bool found = EditorValue.File.TryGetAsset(path + ".png", PckAssetType.TextureFile, out PckAsset foundAsset) ||
                             EditorValue.File.TryGetAsset(path + ".tga", PckAssetType.TextureFile, out foundAsset);
                PckAsset asset = foundAsset ?? EditorValue.File.CreateNewAsset(path + ".png", PckAssetType.TextureFile);
                asset.SetTexture(img);
                return true;
            };

            bool hasMaterialAsset = EditorValue.File.TryGetAsset("entityMaterials.bin", PckAssetType.MaterialFile, out PckAsset entityMaterialAsset);
            IReadOnlyDictionary<string, MaterialContainer.Material> entityMaterials =
                hasMaterialAsset
                ? entityMaterialAsset?.GetData(new MaterialFileReader()).ToDictionary(mat => mat.Name)
                : new Dictionary<string, MaterialContainer.Material>();

            ISaveContext<ModelContainer> saveContext = new DelegatedSaveContext<ModelContainer>(Settings.Default.AutoSaveChanges, (modelContainer) =>
            {
                asset.SetData(new ModelFileWriter(modelContainer, modelContainer.Version));
            });

            var editor = new ModelEditor(modelContainer, saveContext, TryGetSet<string, Image>.FromDelegates(tryGetTexture, trySetTexture), TryGet<string, MaterialContainer.Material>.FromDelegate(entityMaterials.TryGetValue));
            if (editor.ShowDialog() == DialogResult.OK)
            {
                BuildMainTreeView();
                _wasModified = true;
                return;
            }
        }

        private void HandleBehavioursFile(PckAsset asset)
        {
            BehaviourFile behaviourFile = asset.GetData(new BehavioursReader());
            ISaveContext<BehaviourFile> saveContext = new DelegatedSaveContext<BehaviourFile>(Settings.Default.AutoSaveChanges, (behaviourFile) =>
            {
                asset.SetData(new BehavioursWriter(behaviourFile));
            });
            using BehaviourEditor edit = new BehaviourEditor(behaviourFile, saveContext);
            _wasModified = edit.ShowDialog(this) == DialogResult.OK;
        }

        private void HandleMaterialFile(PckAsset asset)
        {
            MaterialContainer materials = asset.GetData(new MaterialFileReader());
            ISaveContext<MaterialContainer> saveContext = new DelegatedSaveContext<MaterialContainer>(Settings.Default.AutoSaveChanges, (materials) =>
            {
                asset.SetData(new MaterialFileWriter(materials));
            });
            using MaterialsEditor edit = new MaterialsEditor(materials, saveContext);
            _wasModified = edit.ShowDialog(this) == DialogResult.OK;
        }

        private void CheckForPasswordAndRemove()
        {
            if (EditorValue.File.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset asset))
            {
                asset.RemoveProperties("LOCK");
            }
        }

        /// <summary>
        /// wrapper that allows the use of <paramref name="name"/> in <code>TreeNode.Nodes.Find(<paramref name="name"/>, ...)</code> and <code>TreeNode.Nodes.ContainsKey(<paramref name="name"/>)</code>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns>new Created TreeNode</returns>
        private static TreeNode CreateNode(string name, object tag = null)
        {
            TreeNode node = new TreeNode(name);
            node.Name = name;
            node.Tag = tag;
            return node;
        }

        private TreeNode BuildNodeTreeBySeperator(TreeNodeCollection root, string path, char seperator)
        {
            _ = root ?? throw new ArgumentNullException(nameof(root));
            if (!path.Contains(seperator))
            {
                TreeNode finalNode = CreateNode(path);
                root.Add(finalNode);
                return finalNode;
            }
            string nodeText = path.Substring(0, path.IndexOf(seperator));
            string subPath = path.Substring(path.IndexOf(seperator) + 1);

            if (string.IsNullOrWhiteSpace(nodeText))
            {
                return BuildNodeTreeBySeperator(root, subPath, seperator);
            }

            bool alreadyExists = root.ContainsKey(nodeText);
            TreeNode subNode = alreadyExists ? root[nodeText] : CreateNode(nodeText);
            if (!alreadyExists)
                root.Add(subNode);
            return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
        }

        private void BuildPckTreeView(TreeNodeCollection root, PckFile pckFile)
        {
            foreach (PckAsset asset in pckFile.GetAssets())
            {
                TreeNode node = BuildNodeTreeBySeperator(root, asset.Filename, '/');
                node.Tag = asset;
                int nodeIconId = GetNodeIconId(asset.Type);
                node.ImageIndex = nodeIconId;
                node.SelectedImageIndex = nodeIconId;
            }
        }

        private void BuildMainTreeView()
        {
            // In case the Rename function was just used and the selected node name no longer matches the file name
            string selectedNodeText = treeViewMain.SelectedNode is TreeNode node ? node.FullPath : string.Empty;
            previewPictureBox.Image = Resources.NoImageFound;
            treeMeta.Nodes.Clear();
            treeViewMain.Nodes.Clear();
            BuildPckTreeView(treeViewMain.Nodes, EditorValue.File);
            treeViewMain.Sort();

            TreeNode[] selectedNodes = treeViewMain.FindPath(selectedNodeText);
            if (selectedNodes.Length > 0)
            {
                treeViewMain.SelectedNode = selectedNodes[0];
            }
        }

        private int GetNodeIconId(PckAssetType type)
        {
            return type switch
            {
                PckAssetType.AudioFile           => 1,
                PckAssetType.LocalisationFile    => 3,
                PckAssetType.TexturePackInfoFile => 4,
                PckAssetType.ColourTableFile     => 6,
                PckAssetType.ModelsFile          => 8,
                PckAssetType.SkinDataFile        => 7,
                PckAssetType.GameRulesFile       => 9,
                PckAssetType.GameRulesHeader     => 10,
                PckAssetType.InfoFile            => 11,
                PckAssetType.SkinFile            => 12,
                PckAssetType.CapeFile            => 13,
                PckAssetType.TextureFile         => 14,
                PckAssetType.BehavioursFile      => 15,
                PckAssetType.MaterialFile        => 16,
                // unknown file format
                _ => 5,
            };
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

        private bool TryGetLocFile(out LOCFile locFile)
        {
            if (!EditorValue.File.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locAsset) &&
                !EditorValue.File.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locAsset))
            {
                locFile = null;
                return false;
            }

            try
            {
                locFile = locAsset.GetData(new LOCFileReader());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            locFile = null;
            return false;
        }

        private bool TrySetLocFile(in LOCFile locFile)
        {
            if (!EditorValue.File.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locAsset) &&
                !EditorValue.File.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locAsset))
            {
                return false;
            }

            try
            {
                locAsset.SetData(new LOCFileWriter(locFile, 2));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        private void ReloadMetaTreeView()
        {
            treeMeta.Nodes.Clear();
            if (treeViewMain.SelectedNode is TreeNode node &&
                node.Tag is PckAsset asset)
            {
                foreach (KeyValuePair<string, string> property in asset.GetProperties())
                {
                    treeMeta.Nodes.Add(CreateNode(property.Key, property));
                }
            }
        }

        private void UpdateRichPresence()
        {
            if (EditorValue is not null &&
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

        private static PckAsset CreateNewAudioAsset(bool isLittle, PckAudioFile audioFile)
        {
            PckAsset newAsset = new PckAsset("audio.pck", PckAssetType.AudioFile);
            newAsset.SetData(new PckAudioFileWriter(audioFile, isLittle ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian));
            return newAsset;
        }

        private static PckAudioFile CreateNewAudioFile()
        {
            PckAudioFile audioFile = new PckAudioFile();
            audioFile.AddCategory(PckAudioFile.AudioCategory.EAudioType.Overworld);
            audioFile.AddCategory(PckAudioFile.AudioCategory.EAudioType.Nether);
            audioFile.AddCategory(PckAudioFile.AudioCategory.EAudioType.End);
            return audioFile;
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            // Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly?
            // So now it's formatted correctly and now displays the file type name in the dialog.
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = false;

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                using AddFilePrompt diag = new AddFilePrompt("res/" + Path.GetFileName(ofd.FileName));
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    if (EditorValue.File.Contains(diag.Filepath, diag.Filetype))
                    {
                        MessageBox.Show(this, $"'{diag.Filepath}' of type {diag.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    PckAsset asset = EditorValue.File.CreateNewAsset(diag.Filepath, diag.Filetype, () => File.ReadAllBytes(ofd.FileName));

                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
            return;
        }

        private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Texture File(*.png,*.tga)|*.png;*.tga";
            if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                using TextPrompt renamePrompt = new TextPrompt(Path.GetFileName(fileDialog.FileName));
                renamePrompt.LabelText = "Path";
                if (renamePrompt.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(renamePrompt.NewText))
                {
                    if (EditorValue.File.Contains(renamePrompt.NewText, PckAssetType.TextureFile))
                    {
                        MessageBox.Show(this, $"'{renamePrompt.NewText}' already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    PckAsset asset = EditorValue.File.CreateNewAsset(renamePrompt.NewText, PckAssetType.TextureFile, () => File.ReadAllBytes(fileDialog.FileName));
                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
        }

        [Obsolete("Refactor or remove this")]
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
                    PckAsset mfNew = EditorValue.File.CreateNewAsset(skinNameImport, PckAssetType.SkinFile);
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
                                if (arg.Length < 2)
                                    continue;
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
            if (treeViewMain.SelectedNode is not null)
                folderNamePrompt.contextLabel.Text =
                    $"New folder at the location of \"{(
                    !treeViewMain.SelectedNode.IsTagOfType<PckAsset>()
                    ? "/" + treeViewMain.SelectedNode.FullPath
                    : treeViewMain.SelectedNode.Parent == null ? "/" : "/" + treeViewMain.SelectedNode.Parent.FullPath)}\"";
            folderNamePrompt.OKButtonText = "Add";
            if (folderNamePrompt.ShowDialog(this) == DialogResult.OK)
            {
                TreeNode folerNode = CreateNode(folderNamePrompt.NewText);
                folerNode.ImageIndex = 0;
                folerNode.SelectedImageIndex = 0;

                TreeNodeCollection nodeCollection = treeViewMain.Nodes;
                if (treeViewMain.SelectedNode is TreeNode node)
                {
                    if (node.Tag is PckAsset asset &&
                        asset.Type != PckAssetType.TexturePackInfoFile &&
                        asset.Type != PckAssetType.SkinDataFile)
                    {
                        if (node.Parent is TreeNode parentNode)
                        {
                            nodeCollection = parentNode.Nodes;
                        }
                    }
                    else
                        nodeCollection = node.Nodes;
                }
                nodeCollection.Add(folerNode);
            }
        }

        private void SetFileType(PckAssetType type)
        {
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
            {
                Debug.WriteLine($"Setting {asset.Type} to {type}");
                asset.Type = type;
                int nodeIconId = GetNodeIconId(type);
                treeViewMain.SelectedNode.ImageIndex = nodeIconId;
                treeViewMain.SelectedNode.SelectedImageIndex = nodeIconId;
            }
        }

        private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ReloadMetaTreeView();

            entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
            buttonEdit.Visible = false;

            previewPictureBox.Image = Resources.NoImageFound;
            viewFileInfoToolStripMenuItem.Visible = false;

            if (!e.Node.TryGetTagData(out PckAsset asset))
            {
                return;
            }

            viewFileInfoToolStripMenuItem.Visible = true;
            if (asset.HasProperty("BOX"))
            {
                buttonEdit.Text = "EDIT BOXES";
                buttonEdit.Visible = true;
            }
            else if (asset.HasProperty("ANIM") &&
                    asset.GetProperty("ANIM", s => SkinANIM.FromString(s) == (SkinAnimMask.RESOLUTION_64x64 | SkinAnimMask.SLIM_MODEL)))
            {
                buttonEdit.Text = "View Skin";
                buttonEdit.Visible = true;
            }

            switch (asset.Type)
            {
                case PckAssetType.SkinFile:
                case PckAssetType.CapeFile:
                case PckAssetType.TextureFile:
                {
                    Image img = asset.GetTexture();

                    previewPictureBox.Image = img;
                    labelImageSize.Text = $"{previewPictureBox.Image.Size.Width}x{previewPictureBox.Image.Size.Height}";

                    if (asset.Type != PckAssetType.TextureFile)
                        break;

                    ResourceLocation resourceLocation = ResourceLocation.GetFromPath(asset.Filename);
                    if (resourceLocation is null || resourceLocation.Category == ResourceCategory.Unknown)
                        break;

                    if (resourceLocation.Category == ResourceCategory.ItemAnimation ||
                        resourceLocation.Category == ResourceCategory.BlockAnimation &&
                        !asset.IsMipmappedFile())
                    {
                        buttonEdit.Text = "EDIT TILE ANIMATION";
                        buttonEdit.Visible = true;
                        break;
                    }

                    buttonEdit.Text = "EDIT TEXTURE ATLAS";
                    buttonEdit.Visible = true;
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

                case PckAssetType.ColourTableFile when asset.Filename == "colours.col":
                    buttonEdit.Text = "EDIT COLORS";
                    buttonEdit.Visible = true;
                    break;

                case PckAssetType.BehavioursFile when asset.Filename == "behaviours.bin":
                    buttonEdit.Text = "EDIT BEHAVIOURS";
                    buttonEdit.Visible = true;
                    break;
                default:
                    buttonEdit.Visible = false;
                    break;
            }
        }

        private void treeViewMain_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset asset)
            {
                if (asset.Size <= 0)
                {
                    Trace.WriteLine($"'{asset.Filename}' has no data attached.", category: nameof(treeViewMain_DoubleClick));
                    return;
                }
                _pckAssetTypeHandler[asset.Type]?.Invoke(asset);
            }
        }

        // Most of the code below is modified code from this link:
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
        // - MattNL

        private void treeViewMain_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Item is not TreeNode node)
                return;

            if ((node.TryGetTagData(out PckAsset asset) && EditorValue.File.Contains(asset.Filename, asset.Type)) || node.Parent is TreeNode)
            {
                // TODO: add (mouse) scrolling while dragging item(s)
                treeViewMain.DoDragDrop(node, DragDropEffects.Scroll | DragDropEffects.Move);
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
            BringToFront();
            Focus();
            treeViewMain.Focus();
        }

        private void treeViewMain_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point dragLocation = new Point(e.X, e.Y);
            Point targetPoint = treeViewMain.PointToClient(dragLocation);

            if (!treeViewMain.ClientRectangle.Contains(targetPoint))
                return;

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeViewMain.GetNodeAt(targetPoint);

            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetData(DataFormats.FileDrop) is string[] filesDropped)
            {
                IEnumerable<string> files = filesDropped.Where(File.Exists);
                IEnumerable<string> directoryFiles = filesDropped
                    .Where(f => (File.GetAttributes(f) & FileAttributes.Directory) != 0)
                    .SelectMany(dir => Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories));

                string baseDirectory = Path.GetDirectoryName(filesDropped.First());

                IEnumerable<string> importPaths = files.Concat(directoryFiles);

                ImportFiles(baseDirectory, importPaths, string.IsNullOrWhiteSpace(targetNode?.FullPath) ? string.Empty : targetNode?.FullPath);
                return;
            }

            string dataFormat = typeof(TreeNode).FullName;

            if (targetNode is null)
                return;

            if (!e.Data.GetDataPresent(dataFormat))
                return;

            bool isTargetPckFile = targetNode.IsTagOfType<PckAsset>();
            TreeNode draggedNode = e.Data.GetData(dataFormat) as TreeNode;
            if (draggedNode == null)
            {
                Debug.WriteLine("Dragged node is null.");
                return;
            }

            if (targetNode.Equals(draggedNode))
            {
                Debug.WriteLine("Dragged node was not moved.");
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
            if (draggedNode.TryGetTagData(out PckAsset draggedAsset) &&
                targetNode.FullPath != draggedAsset.Filename)
            {
                Debug.WriteLine(draggedAsset.Filename + " was droped onto " + targetNode.FullPath);
                string newFilePath = Path.Combine(isTargetPckFile
                    ? Path.GetDirectoryName(targetNode.FullPath)
                    : targetNode.FullPath, Path.GetFileName(draggedAsset.Filename));
                Debug.WriteLine("New filepath: " + newFilePath);
                draggedAsset.Filename = newFilePath;
                _wasModified = true;
                BuildMainTreeView();
                return;
            }
            else
            {
                IEnumerable<PckAsset> pckFiles = GetAllChildNodes(draggedNode.Nodes).Where(t => t.IsTagOfType<PckAsset>()).Select(t => t.Tag as PckAsset);
                string oldPath = draggedNode.FullPath;
                string newPath = Path.Combine(isTargetPckFile ? Path.GetDirectoryName(targetNode.FullPath) : targetNode.FullPath, draggedNode.Text).Replace('\\', '/');
                foreach (PckAsset pckFile in pckFiles)
                {
                    pckFile.Filename = Path.Combine(newPath, pckFile.Filename.Substring(oldPath.Length + 1)).Replace('\\', '/');
                }
                _wasModified = true;
                BuildMainTreeView();
            }
        }

        private void ImportFiles(string baseDirectory, IEnumerable<string> files, string prefix)
        {
            int fileCount = files.Count();
            int addedCount = 0;
            int skippedFiles = 0;
            int skipAttempts = 3;
            int typeDuplication = 0;
            PckAssetType lastSelectedAssetType = PckAssetType.SkinFile;
            bool askForAssetType = true;
            foreach (var filepath in files)
            {
                string assetPath = Path.Combine(prefix + filepath.Substring(baseDirectory.Length)).TrimStart('/', '\\');
                PckAssetType assetType = lastSelectedAssetType;

                if (askForAssetType)
                {
                    using AddFilePrompt addFile = new AddFilePrompt(assetPath);
                    if (addFile.ShowDialog(this) != DialogResult.OK)
                    {
                        skippedFiles++;
                        skipAttempts--;
                        if (skipAttempts > 0)
                            continue;

                        int remainingFileCount = fileCount - addedCount - skippedFiles;
                        DialogResult abortFurtherImport = MessageBox.Show($"Do you wan't to abort further file imports?\n{remainingFileCount} file(s) left.", "Abort further import", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (abortFurtherImport == DialogResult.Yes)
                        {
                            skippedFiles += remainingFileCount;
                            break;
                        }
                        skipAttempts = 3;
                        continue;
                    }

                    assetType = addFile.Filetype;
                    assetPath = addFile.Filepath;

                    if (lastSelectedAssetType == assetType)
                        typeDuplication++;
                    lastSelectedAssetType = addFile.Filetype;
                    if (typeDuplication > 1)
                    {
                        DialogResult useSameTypeForRest = MessageBox.Show($"Do you want to import all remaining files as {lastSelectedAssetType}?", "Import all as", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        if (useSameTypeForRest == DialogResult.Yes)
                        {
                            askForAssetType = false;
                        }
                    }
                }

                if (EditorValue.File.Contains(filepath, assetType))
                {
                    if (askForAssetType)
                        MessageBox.Show(this, $"'{assetPath}' of type {assetType} already exists.\nSkiping file.", "File already exists", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    Debug.WriteLine($"'{assetPath}' of type {assetType} already exists.\nSkiping file.");
                    continue;
                }
                PckAsset importedAsset = EditorValue.File.CreateNewAsset(assetPath, assetType, () => File.ReadAllBytes(filepath));
                string propertyFile = filepath + ".txt";
                if (File.Exists(propertyFile))
                {
                    importedAsset.DeserializeProperties(File.ReadAllLines(propertyFile));
                }
                addedCount++;
            }
            Trace.TraceInformation("[{0}] Imported {1} file(s).", nameof(ImportFiles), addedCount);
            Trace.TraceInformation("[{0}] Skipped {1} file(s).", nameof(ImportFiles), skippedFiles);
            if (addedCount > 0)
            {
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

                if (EditorValue.File.Contains(addFile.Filepath, addFile.Filetype))
                {
                    MessageBox.Show(this, $"'{addFile.Filepath}' of type {addFile.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                EditorValue.File.CreateNewAsset(addFile.Filepath, addFile.Filetype, () => File.ReadAllBytes(file));
                addedCount++;

                BuildMainTreeView();
                _wasModified = true;
            }
            Trace.TraceInformation("[{0}] Imported {1} file(s).", nameof(ImportFiles), addedCount);
        }

        private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AddSkinPrompt addNewSkinDialog = new AddSkinPrompt())
                if (addNewSkinDialog.ShowDialog() == DialogResult.OK)
                {
                    TryGetLocFile(out LOCFile locFile);
                    PckAsset skinAsset = addNewSkinDialog.NewSkin.CreateFile(locFile);
                    EditorValue.File.AddAsset(skinAsset);

                    bool hasSkinsFolder = treeViewMain.Nodes.ContainsKey("Skins");
                    if (hasSkinsFolder)
                        skinAsset.Filename = skinAsset.Filename.Insert(0, "Skins/"); // Then Skins folder
                    EditorValue.File.AddAsset(skinAsset);

                    if (addNewSkinDialog.NewSkin.HasCape)
                    {
                        PckAsset capeFile = addNewSkinDialog.NewSkin.CreateCapeFile();
                        if (hasSkinsFolder)
                            capeFile.Filename = capeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
                        EditorValue.File.AddAsset(capeFile);
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

            string animationFilepath = $"{ResourceLocation.GetPathFromCategory(diag.Category)}/{diag.SelectedTile.InternalName}.png";

            if (EditorValue.File.Contains(animationFilepath, PckAssetType.TextureFile))
            {
                MessageBox.Show(this, $"{diag.SelectedTile.DisplayName} is already present.", "File already present");
                return;
            }

            Animation newAnimation = default;
            ISaveContext<Animation> saveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
            {
                newAnimation = animation;
            });

            using AnimationEditor animationEditor = new AnimationEditor(Animation.CreateEmpty(), saveContext, diag.SelectedTile.DisplayName, !diag.SelectedTile.InternalName.EqualsAny("clock", "compass"));
            if (animationEditor.ShowDialog() == DialogResult.OK && newAnimation is not null)
            {
                _wasModified = true;
                PckAsset asset = EditorValue.File.CreateNewAsset(animationFilepath, PckAssetType.TextureFile);
                asset.SetSerializedData(newAnimation, AnimationSerializer.DefaultSerializer);
                BuildMainTreeView();
                ReloadMetaTreeView();
            }
        }

        private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorValue.File.Contains(PckAssetType.AudioFile))
            {
                // the chance of this happening is really really slim but just in case
                MessageBox.Show(this, "There is already an audio file in this PCK!", "Can't create audio.pck");
                return;
            }

            if (string.IsNullOrEmpty(_location))
            {
                MessageBox.Show(this, "You must save your pck before creating or opening a music cues PCK file", "Can't create audio.pck");
                return;
            }

            PckAudioFile newAudioFile = CreateNewAudioFile();
            PckAsset newAudioAsset = CreateNewAudioAsset(LittleEndianCheckBox.Checked, newAudioFile);

            ISaveContext<PckAudioFile> saveContext = new DelegatedSaveContext<PckAudioFile>(Settings.Default.AutoSaveChanges, (audioFile) =>
            {
                newAudioAsset.SetData(new PckAudioFileWriter(audioFile, LittleEndianCheckBox.Checked ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian));
            });

            AudioEditor diag = new AudioEditor(newAudioFile, saveContext);
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                EditorValue.File.AddAsset(newAudioAsset);
            }
            diag.Dispose();
            BuildMainTreeView();
        }

        private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorValue.File.TryGetAsset("colours.col", PckAssetType.ColourTableFile, out _))
            {
                MessageBox.Show(this, "A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            PckAsset newColorAsset = EditorValue.File.CreateNewAsset("colours.col", PckAssetType.ColourTableFile);
            newColorAsset.SetData(Resources.tu69colours);
            BuildMainTreeView();
        }

        private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (EditorValue.File.TryGetAsset("Skins.pck", PckAssetType.SkinDataFile, out _))
            {
                MessageBox.Show(this, "A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            EditorValue.File.CreateNewAsset("Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
                    LittleEndianCheckBox.Checked ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian));

            BuildMainTreeView();
        }

        private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorValue.File.TryGetAsset("behaviours.bin", PckAssetType.BehavioursFile, out _))
            {
                MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }

            EditorValue.File.CreateNewAsset("behaviours.bin", PckAssetType.BehavioursFile, new BehavioursWriter(new BehaviourFile()));
            BuildMainTreeView();
        }

        private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EditorValue.File.TryGetAsset("entityMaterials.bin", PckAssetType.MaterialFile, out _))
            {
                MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
                return;
            }
            var materialContainer = new MaterialContainer();
            materialContainer.InitializeDefault();
            EditorValue.File.CreateNewAsset("entityMaterials.bin", PckAssetType.MaterialFile, new MaterialFileWriter(materialContainer));
            BuildMainTreeView();
        }

        [Obsolete("Refactor or remove this")]
        private void importExtractedSkinsFolder(object sender, EventArgs e)
        {
            OpenFolderDialog contents = new OpenFolderDialog();
            if (contents.ShowDialog(Handle) == true)
            {
                //checks to make sure selected path exist
                if (!Directory.Exists(contents.ResultPath))
                {
                    MessageBox.Show("Directory Lost");
                    return;
                }
                // creates variable to indicate wether current pck skin structure is mashup or regular skin
                bool hasSkinsPck = EditorValue.File.HasAsset("Skins.pck", PckAssetType.SkinDataFile);

                foreach (var fullfilename in Directory.GetFiles(contents.ResultPath, "*.png"))
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
                            if (param.Length < 2)
                                continue;
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
                        PckAsset skinsFileAsset = EditorValue.File.GetAsset("Skins.pck", PckAssetType.SkinDataFile);
                        using (var ms = new MemoryStream(skinsFileAsset.Data))
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
                    EditorValue.File.AddAsset(newFile);
                }
                BuildMainTreeView();
                _wasModified = true;
            }
        }

        private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset) &&
                asset.Type == PckAssetType.SkinFile)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "3DS Texture|*.3dst";
                saveFileDialog.DefaultExt = ".3dst";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Image img = asset.GetTexture();
                    var writer = new _3DSTextureWriter(img);
                    writer.WriteToFile(saveFileDialog.FileName);
                }
            }
        }

        private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset asset && asset.Type == PckAssetType.TextureFile)
            {
                string textureDirectory = Path.GetDirectoryName(asset.Filename);
                string textureName = Path.GetFileNameWithoutExtension(asset.Filename);

                if (asset.IsMipmappedFile())
                    return;

                string textureExtension = Path.GetExtension(asset.Filename);

                using NumericPrompt numericPrompt = new NumericPrompt(0);
                numericPrompt.Minimum = 1;
                numericPrompt.Maximum = 4; // 5 is the presumed max MipMap level
                numericPrompt.ToolTipText = "You can enter the amount of MipMap levels that you would like to generate. " +
                    "For example: if you enter 2, MipMapLevel1.png and MipMapLevel2.png will be generated";
                numericPrompt.TextLabel.Text = "Levels";

                if (numericPrompt.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 2; i < 2 + numericPrompt.SelectedValueAsInt; i++)
                    {
                        string mippedPath = $"{textureDirectory}/{textureName}MipMapLevel{i}{textureExtension}";
                        Debug.WriteLine(mippedPath);
                        if (EditorValue.File.HasAsset(mippedPath, PckAssetType.TextureFile))
                            EditorValue.File.RemoveAsset(EditorValue.File.GetAsset(mippedPath, PckAssetType.TextureFile));
                        PckAsset mipMappedAsset = new PckAsset(mippedPath, PckAssetType.TextureFile);

                        Image originalTexture = asset.GetTexture();
                        int newWidth = Math.Max(originalTexture.Width / (int)Math.Pow(2, i - 1), 1);
                        int newHeight = Math.Max(originalTexture.Height / (int)Math.Pow(2, i - 1), 1);

                        Rectangle tileArea = new Rectangle(0, 0, newWidth, newHeight);
                        Image mippedTexture = new Bitmap(newWidth, newHeight);
                        using (Graphics gfx = Graphics.FromImage(mippedTexture))
                        {
                            gfx.SmoothingMode = SmoothingMode.None;
                            gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gfx.DrawImage(originalTexture, tileArea);
                        }

                        mipMappedAsset.SetTexture(mippedTexture);

                        EditorValue.File.InsertAsset(EditorValue.File.IndexOfAsset(asset) + i - 1, mipMappedAsset);
                    }
                    BuildMainTreeView();
                }
            }
        }

        private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset asset)
            {
                MessageBox.Show(
                    $"Asset path: {asset.Filename}" +
                    $"\nAsset type: {(int)asset.Type} ({asset.Type})" +
                    $"\nAsset size: {asset.Size}" + 
                    $"\nProperties count: {asset.PropertyCount}"
                    , Path.GetFileName(asset.Filename) + " Asset info");
            }
        }

        private void correctSkinDecimalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset) &&
                asset.Type == PckAssetType.SkinFile)
            {
                foreach (KeyValuePair<string, string> p in asset.GetProperties().ToList())
                {
                    if (p.Key == "BOX" || p.Key == "OFFSET")
                        asset.SetProperty(asset.GetPropertyIndex(p), new KeyValuePair<string, string>(p.Key, p.Value.Replace(',', '.')));
                }
                ReloadMetaTreeView();
                _wasModified = true;
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;

            if (node == null)
            {
                MessageBox.Show(this, "The selected node was null. Please select a node and try again.", "Node not extracted");

                return;
            }

            if (node.Tag == null)
            {
                OpenFolderDialog dialog = new OpenFolderDialog();
                dialog.Title = @"Select destination folder";

                if (dialog.ShowDialog(Handle) == true)
                    extractFolder(dialog.ResultPath);
            }
            else if (node.TryGetTagData(out PckAsset asset))
            {
                using SaveFileDialog exFile = new SaveFileDialog();
                exFile.FileName = Path.GetFileName(asset.Filename);
                exFile.Filter = Path.GetExtension(asset.Filename).Replace(".", string.Empty) + " File|*" + Path.GetExtension(asset.Filename);
                if (exFile.ShowDialog(this) != DialogResult.OK ||
                    // Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
                    string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName)))
                {
                    MessageBox.Show(this, "The chosen directory is invalid. Please choose a different one and try again.", "Node not extracted");

                    return;
                }

                extractFile(exFile.FileName, asset);
            }

            // Verification that file extraction path was successful
            MessageBox.Show(this, $"\"{node.Text}\" successfully extracted");
        }

        private void extractFolder(string outPath)
        {
            TreeNode node = treeViewMain.SelectedNode;

            string selectedFolder = node.FullPath;

            foreach (PckAsset asset in EditorValue.File.GetAssets().Where(asset => asset.Filename.StartsWith(selectedFolder)))
            {
                extractFolderFile(outPath, asset);
            }
        }

        private void extractFolderFile(string outPath, PckAsset asset)
        {
            TreeNode node = treeViewMain.SelectedNode;

            // abb = "Abbreviated Path"
            string abbPath = Path.GetDirectoryName(asset.Filename);
            int startIndex = abbPath.IndexOf(node.Text);
            abbPath = abbPath.Substring(startIndex, abbPath.Length - startIndex);
            string finalPath = ($"{outPath}/{abbPath}/").Replace('\\', '/');

            if (!Directory.Exists(finalPath))
                Directory.CreateDirectory(finalPath);

            extractFile(finalPath + "/" + Path.GetFileName(asset.Filename), asset);
        }

        private void extractFile(string outFilePath, PckAsset asset)
        {
            File.WriteAllBytes(outFilePath, asset.Data);
            if (asset.PropertyCount > 0)
            {
                File.WriteAllLines($"{outFilePath}.txt", asset.SerializeProperties());
            }
        }

        private void cloneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null || !node.IsTagOfType<PckAsset>())
                return;
            string path = node.FullPath;

            using TextPrompt diag = new TextPrompt(node.Tag is null ? Path.GetFileName(node.FullPath) : node.FullPath);
            diag.contextLabel.Text = $"Creating a clone of \"{path}\". Ensure that the path isn't yet.";
            diag.OKButtonText = "Clone";

            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                if (node.Tag is PckAsset asset)
                {
                    TreeNode newNode = new TreeNode();
                    newNode.Text = Path.GetFileName(diag.NewText);
                    var newFile = new PckAsset(diag.NewText, asset.Type);
                    foreach (KeyValuePair<string, string> property in asset.GetProperties())
                    {
                        newFile.AddProperty(property);
                    }
                    newFile.SetData(asset.Data);
                    newFile.Filename = diag.NewText;
                    newNode.Tag = newFile;
                    newNode.ImageIndex = node.ImageIndex;
                    newNode.SelectedImageIndex = node.SelectedImageIndex;

                    if (GetAllChildNodes(treeViewMain.Nodes).FirstOrDefault(n => n.FullPath == diag.NewText) is not null)
                    {
                        MessageBox.Show(
                            this,
                            $"A file with the path \"{diag.NewText}\" already exists. " +
                            $"Please try again with a different name.",
                            "Key already exists");
                        return;
                    }

                    TreeNodeCollection nodeCollection = node.Parent?.Nodes ?? treeViewMain.Nodes;
                    nodeCollection.Insert(node.Index + 1, newNode);

                    EditorValue.File.InsertAsset(node.Index + 1, newFile);
                    BuildMainTreeView();
                    _wasModified = true;
                }
            }
        }

        private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null)
                return;
            string path = node.FullPath;

            bool isFile = node.TryGetTagData(out PckAsset asset);

            using TextPrompt diag = new TextPrompt(isFile ? asset.Filename : Path.GetFileName(node.FullPath));

            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                if (isFile)
                {
                    if (EditorValue.File.Contains(diag.NewText, asset.Type))
                    {
                        MessageBox.Show(this, $"{diag.NewText} already exists", "File already exists");
                        return;
                    }
                    asset.Filename = diag.NewText;
                }
                else // folders
                {
                    node.Text = diag.NewText;
                    foreach (TreeNode childNode in GetAllChildNodes(node.Nodes))
                    {
                        if (childNode.Tag is PckAsset folderAsset)
                        {
                            if (folderAsset.Filename == diag.NewText)
                                continue;
                            folderAsset.Filename = childNode.FullPath;
                        }
                    }
                }
                _wasModified = true;
                BuildMainTreeView();
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset asset)
            {
                using var ofd = new OpenFileDialog();
                // Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.

                string extra_extensions = "";

                switch (asset.Type)
                {
                    case PckAssetType.TextureFile:
                        if (Path.GetExtension(asset.Filename) == ".png")
                            extra_extensions = ";*.tga";
                        else if (Path.GetExtension(asset.Filename) == ".tga")
                            extra_extensions = ";*.png";
                        break;
                }

                string fileExt = Path.GetExtension(asset.Filename);

                ofd.Filter = $"{asset.Type} (*{fileExt}{extra_extensions})|*{fileExt}{extra_extensions}";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string newFileExt = Path.GetExtension(ofd.FileName);
                    asset.SetData(File.ReadAllBytes(ofd.FileName));
                    asset.Filename = asset.Filename.Replace(fileExt, newFileExt);
                    _wasModified = true;
                    BuildMainTreeView();
                }
                return;
            }
            MessageBox.Show(this, "Can't replace a folder.");
        }

        /// <summary>
        /// Action to run before an asset will be deleted
        /// </summary>
        /// <param name="asset">Asset to remove</param>
        /// <returns>True if the remove should be canceled, otherwise False</returns>
        private bool BeforeFileRemove(PckAsset asset)
        {
            string itemPath = ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation);

            // warn the user about deleting compass.png and clock.png
            if (asset.Type == PckAssetType.TextureFile &&
                (asset.Filename == itemPath + "/compass.png" || asset.Filename == itemPath + "/clock.png"))
            {
                if (MessageBox.Show(this, "Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return true;
            }

            // remove loc key if its a skin/cape
            if (asset.Type == PckAssetType.SkinFile || asset.Type == PckAssetType.CapeFile)
            {
                if (TryGetLocFile(out LOCFile locFile))
                {
                    if (asset.TryGetProperty("THEMENAMEID", out string value))
                        locFile.RemoveLocKey(value);
                    if (asset.TryGetProperty("DISPLAYNAMEID", out value))
                        locFile.RemoveLocKey(value);
                    TrySetLocFile(locFile);
                }
            }
            return false;
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewMain.SelectedNode;
            if (node == null)
                return;

            string path = node.FullPath;

            if (node.TryGetTagData(out PckAsset asset))
            {
                if (!BeforeFileRemove(asset) && EditorValue.File.RemoveAsset(asset))
                {
                    node.Remove();
                    _wasModified = true;
                }
            }
            else if (MessageBox.Show(this, "Are you sure want to delete this folder? All contents will be deleted", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string pckFolderDir = node.FullPath;
                EditorValue.File.RemoveAll(file => file.Filename.StartsWith(pckFolderDir) && !BeforeFileRemove(file));
                node.Remove();
                _wasModified = true;
            }
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
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
            {
                IEnumerable<string> props = asset.SerializeProperties(seperater: " ");
                using (var input = new MultiTextPrompt(props))
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        asset.ClearProperties();
                        asset.DeserializeProperties(input.TextOutput);
                        ReloadMetaTreeView();
                        _wasModified = true;
                    }
                }
            }
        }

        private void treeMeta_DoubleClick(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode subnode && subnode.Tag is KeyValuePair<string, string> property &&
                treeViewMain.SelectedNode is TreeNode node && node.Tag is PckAsset asset)
            {
                if (asset.HasProperty(property.Key))
                {
                    switch (property.Key)
                    {
                        case "ANIM" when asset.Type == PckAssetType.SkinFile:
                            try
                            {
                                using ANIMEditor diag = new ANIMEditor(SkinANIM.FromString(property.Value));
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    asset.SetProperty(asset.GetPropertyIndex(property), new KeyValuePair<string, string>("ANIM", diag.ResultAnim.ToString()));
                                    ReloadMetaTreeView();
                                    _wasModified = true;
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                Trace.WriteLine("Invalid ANIM value: " + property.Value);
                                MessageBox.Show(this, "Failed to parse ANIM value, aborting to normal functionality. Please make sure the value only includes hexadecimal characters (0-9,A-F) and has no more than 8 characters.");
                            }
                            break;

                        case "BOX" when asset.Type == PckAssetType.SkinFile:
                            try
                            {
                                using BoxEditor diag = new BoxEditor(property.Value, false);
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    asset.SetProperty(asset.GetPropertyIndex(property), new KeyValuePair<string, string>("BOX", diag.Result.ToString()));
                                    ReloadMetaTreeView();
                                    _wasModified = true;
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                Trace.WriteLine("Invalid BOX value: " + property.Value);
                                MessageBox.Show(this, "Failed to parse BOX value, aborting to normal functionality.");
                            }
                            break;

                        default:
                            break;

                    }

                    using (AddPropertyPrompt addProperty = new AddPropertyPrompt(property))
                    {
                        if (addProperty.ShowDialog(this) == DialogResult.OK)
                        {
                            asset.SetProperty(asset.GetPropertyIndex(property), addProperty.Property);
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
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
            {
                using var input = new MultiTextPrompt();
                if (input.ShowDialog(this) == DialogResult.OK)
                {
                    asset.DeserializeProperties(input.TextOutput);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
            }
        }

        private void addBOXEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset asset)
            {
                using BoxEditor diag = new BoxEditor(SkinBOX.DefaultHead, false);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    asset.AddProperty("BOX", diag.Result);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
                return;
            }
        }

        private void addANIMEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
            {
                using ANIMEditor diag = new ANIMEditor(SkinANIM.Empty);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    asset.AddProperty("ANIM", diag.ResultAnim);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
                return;
            }
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode t && t.Tag is KeyValuePair<string, string> property &&
                treeViewMain.SelectedNode is TreeNode main && main.Tag is PckAsset asset &&
                asset.RemoveProperty(property))
            {
                treeMeta.SelectedNode.Remove();
                _wasModified = true;
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode is TreeNode t &&
                t.Tag is PckAsset asset)
            {
                using AddPropertyPrompt addProperty = new AddPropertyPrompt();
                if (addProperty.ShowDialog(this) == DialogResult.OK)
                {
                    asset.AddProperty(addProperty.Property);
                    ReloadMetaTreeView();
                    _wasModified = true;
                }
            }
        }

        private static bool TryGetDefaultEntityModel(string modelName, out Model model)
        {
            if (!GameModelImporter.DefaultModels.TryGetValue(modelName, out DefaultModel defaultModel) || defaultModel is null)
            {
                model = default;
                return false;
            }
            model = new Model(modelName, new Size((int)defaultModel.TextureSize.X, (int)defaultModel.TextureSize.Y));

            foreach (DefaultPart defaultPart in defaultModel.Parts)
            {
                ModelPart modelPart = new ModelPart(defaultPart.Name, "", defaultPart.Translation, defaultPart.Rotation, System.Numerics.Vector3.Zero);
                modelPart.AddBoxes(defaultPart.Boxes.Select(defaultBox => new ModelBox(defaultBox.Position, defaultBox.Size, defaultBox.Uv, defaultBox.Inflate, defaultBox.Mirror)));
                model.AddPart(modelPart);
            }

            return true;
        }

        private void ShowSimpleModelRender(Model model, NamedData<Image> modelTexture)
        {
            MetroForm form = new MetroForm();
            form.Icon = Resources.ProjectLogo;
            form.Theme = MetroFramework.MetroThemeStyle.Dark;
            form.Style = MetroFramework.MetroColorStyle.Silver;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Text = $"{model.Name} - {modelTexture.Name}";
            form.Size = new Size(600, 500);
            form.MinimumSize = new Size(300, 300);

            void ExportToolStripItem_Click(object sender, EventArgs e)
            {
                GameModelImporter.Default.ExportSettings.CreateModelOutline =
                    MessageBox.Show(
                        $"Do you wish to have all model parts contained in a group called '{model.Name}'?",
                        "Group model parts", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                using SaveFileDialog openFileDialog = new SaveFileDialog();
                openFileDialog.FileName = model.Name;
                openFileDialog.Filter = GameModelImporter.Default.SupportedModelFileFormatsFilter;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var modelInfo = new GameModelInfo(model, new NamedData<Image>[1] { modelTexture });
                    GameModelImporter.Default.Export(openFileDialog.FileName, modelInfo);
                }
            }
            ToolStripItem exportToolStripItem = new ToolStripButton("Export");
            exportToolStripItem.Click += ExportToolStripItem_Click;

            MenuStrip menu = new MenuStrip();
            menu.BackColor = Color.FromArgb(35, 35, 35);
            menu.ForeColor = Color.WhiteSmoke;
            menu.Anchor = AnchorStyles.Top;
            menu.Dock = DockStyle.Top;
            menu.Items.Add(exportToolStripItem);

            ModelRenderer renderer = new ModelRenderer();
            form.Controls.Add(menu);
            form.Controls.Add(renderer);

            renderer.VSync = true;
            renderer.BackColor = Color.FromArgb(30, 30, 30);
            renderer.Dock = DockStyle.Fill;
            renderer.Texture = modelTexture.Value;
            renderer.LoadModel(model);
            renderer.ResetCamera();

            form.ShowDialog(this);

            renderer.Dispose();
            form.Dispose();
        }

        private void PckEditor_Load(object sender, EventArgs e)
        {
            CheckForPasswordAndRemove();
            BuildMainTreeView();
            UpdateRichPresence();
        }

        private void SetEndianess(OMI.ByteOrder endianness)
        {
            LittleEndianCheckBox.Checked = endianness == OMI.ByteOrder.LittleEndian;
        }

        private OMI.ByteOrder GetEndianess()
        {
            return LittleEndianCheckBox.Checked ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            treeViewMain_DoubleClick(sender, e);
        }

        private void SetPckEndianness(OMI.ByteOrder endianness)
        {
            try
            {
                if (treeViewMain.SelectedNode.Tag is PckAsset asset && (asset.Type is PckAssetType.AudioFile || asset.Type is PckAssetType.SkinDataFile || asset.Type is PckAssetType.TexturePackInfoFile))
                {
                    IDataFormatReader reader = asset.Type is PckAssetType.AudioFile
                        ? new PckAudioFileReader(endianness == OMI.ByteOrder.BigEndian ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian)
                        : new PckFileReader(endianness == OMI.ByteOrder.BigEndian ? OMI.ByteOrder.LittleEndian : OMI.ByteOrder.BigEndian);
                    object pck = reader.FromStream(new MemoryStream(asset.Data));

                    IDataFormatWriter writer = asset.Type is PckAssetType.AudioFile
                        ? new PckAudioFileWriter((PckAudioFile)pck, endianness)
                        : new PckFileWriter((PckFile)pck, endianness);
                    asset.SetData(writer);
                    _wasModified = true;
                    MessageBox.Show($"\"{asset.Filename}\" successfully converted to {(endianness == OMI.ByteOrder.LittleEndian ? "little" : "big")} endian.", "Converted PCK file");
                }
            }
            catch (OverflowException)
            {
                MessageBox.Show(this, $"File was not a valid {(endianness != OMI.ByteOrder.LittleEndian ? "little" : "big")} endian PCK File.", "Not a valid PCK file");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Not a valid PCK file");
                return;
            }
        }

        private void littleEndianToolStripMenuItem_Click(object sender, EventArgs e) => SetPckEndianness(OMI.ByteOrder.LittleEndian);

        private void bigEndianToolStripMenuItem_Click(object sender, EventArgs e) => SetPckEndianness(OMI.ByteOrder.BigEndian);

        private void SetModelVersion(int version)
        {
            if (treeViewMain.SelectedNode.Tag is PckAsset asset && asset.Type is PckAssetType.ModelsFile)
            {
                try
                {
                    ModelContainer container = asset.GetData(new ModelFileReader());

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

                    asset.SetData(new ModelFileWriter(container, version));
                    _wasModified = true;
                    MessageBox.Show(
                        this,
                        $"\"{asset.Filename}\" successfully converted to Version {version + 1} format.",
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

        private void treeViewMain_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is not null)
                treeViewMain.SelectedNode = e.Node;
        }

        private void contextMenuPCKEntries_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            correctSkinDecimalsToolStripMenuItem.Visible = false;
            generateMipMapTextureToolStripMenuItem1.Visible = false;
            setModelContainerFormatToolStripMenuItem.Visible = false;
            setSubPCKEndiannessToolStripMenuItem.Visible = false;
            exportToolStripMenuItem.Visible = false;
            toolStripSeparator5.Visible = false;
            toolStripSeparator6.Visible = false;
            if (treeViewMain?.SelectedNode.TryGetTagData(out PckAsset asset) ?? false)
            {
                replaceToolStripMenuItem.Visible = true;
                cloneFileToolStripMenuItem.Visible = true;
                setFileTypeToolStripMenuItem.Visible = true;
                toolStripSeparator5.Visible = true;
                toolStripSeparator6.Visible = true;

                if (asset.Type == PckAssetType.SkinFile)
                {
                    correctSkinDecimalsToolStripMenuItem.Visible = true;
                    exportToolStripMenuItem.Visible = true;
                }
                if (asset.Type == PckAssetType.TextureFile)
                    generateMipMapTextureToolStripMenuItem1.Visible = true;
                if (asset.Type == PckAssetType.ModelsFile)
                    setModelContainerFormatToolStripMenuItem.Visible = true;
                if (asset.Type == PckAssetType.SkinDataFile || asset.Type == PckAssetType.TexturePackInfoFile || asset.Type == PckAssetType.AudioFile)
                    setSubPCKEndiannessToolStripMenuItem.Visible = true;
            }
            else
            {
                replaceToolStripMenuItem.Visible = false;
                cloneFileToolStripMenuItem.Visible = false;
                setFileTypeToolStripMenuItem.Visible = false;
            }
        }
    }
}