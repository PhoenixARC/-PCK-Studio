using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using OMI.Formats.Pck;
using OMI.Formats.Color;
using OMI.Formats.Model;
using OMI.Formats.GameRule;
using OMI.Formats.Material;
using OMI.Formats.Behaviour;
using OMI.Formats.Languages;
using OMI.Workers;
using OMI.Workers.Pck;
using OMI.Workers.Color;
using OMI.Workers.Model;
using OMI.Workers.GameRule;
using OMI.Workers.Material;
using OMI.Workers.Language;
using OMI.Workers.Behaviour;
using PckStudio.Properties;
using PckStudio.Internal.FileFormats;
using PckStudio.Forms;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Internal.Misc;
using PckStudio.Internal.IO.PckAudio;
using PckStudio.Internal.IO._3DST;
using PckStudio.Internal;
using PckStudio.Forms.Features;
using PckStudio.Extensions;
using PckStudio.Popups;
using PckStudio.External.API.Miles;
using PckStudio.Internal.Json;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;
using PckStudio.Internal.App;
using PckStudio.Internal.Skin;
using PckStudio.Interfaces;
using PckStudio.Rendering;
using MetroFramework.Forms;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		private PckManager PckManager = null;
		string saveLocation;

		PckFile currentPCK = null;

		bool __modified = false;
		bool wasModified
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

		bool isTemplateFile = false;

		bool isSelectingTab = false;

		readonly Dictionary<PckAssetType, Action<PckAsset>> pckFileTypeHandler;

		public MainForm()
		{
			InitializeComponent();

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

			treeViewMain.TreeViewNodeSorter = new PckNodeSorter();

			pckOpen.AllowDrop = true;

			Text = Application.ProductName;

			labelVersion.Text = $"{Application.ProductName}: {ApplicationScope.CurrentVersion}";
			ChangelogRichTextBox.Text = Resources.CHANGELOG;

			pckFileTypeHandler = new Dictionary<PckAssetType, Action<PckAsset>>(15)
			{
				[PckAssetType.SkinFile] = HandleSkinFile,
				[PckAssetType.CapeFile] = null,
				[PckAssetType.TextureFile] = HandleTextureFile,
				[PckAssetType.UIDataFile] = _ => throw new NotSupportedException("unused in-game"),
				[PckAssetType.InfoFile] = null,
				[PckAssetType.TexturePackInfoFile] = null, // HandleInnerPckFile,
                [PckAssetType.LocalisationFile] = HandleLocalisationFile,
				[PckAssetType.GameRulesFile] = HandleGameRuleFile,
				[PckAssetType.AudioFile] = HandleAudioFile,
				[PckAssetType.ColourTableFile] = HandleColourFile,
				[PckAssetType.GameRulesHeader] = HandleGameRuleFile,
				[PckAssetType.SkinDataFile] = null, // HandleInnerPckFile,
				[PckAssetType.ModelsFile] = HandleModelsFile, // Note: Uncomment when implemented
				[PckAssetType.BehavioursFile] = HandleBehavioursFile,
				[PckAssetType.MaterialFile] = HandleMaterialFile,
			};
		}

		// TODO: decide on how to handle embedded pck files
        private void HandleInnerPckFile(PckAsset asset)
        {
			if (Settings.Default.LoadSubPcks &&
				(asset.Type == PckAssetType.SkinDataFile || asset.Type == PckAssetType.TexturePackInfoFile) &&
				asset.Size > 0 && treeViewMain.SelectedNode.Nodes.Count == 0)
			{
				try
				{
					PckFile subPCKfile = asset.GetData(new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
					BuildPckTreeView(treeViewMain.SelectedNode.Nodes, subPCKfile);
					treeViewMain.SelectedNode.ExpandAll();
                }
				catch (OverflowException ex)
				{
					MessageBox.Show(this, "Failed to open pck\n" +
						"Try checking the 'Open/Save as Switch/Vita/PS4 pck' checkbox in the upper right corner.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Debug.WriteLine(ex.Message);
				}
				return;
			}
			treeViewMain.SelectedNode.Nodes.Clear();
			treeViewMain.SelectedNode.Collapse();
        }

		public void InitPckFromFile(string filepath)
		{
			saveLocation = filepath;
		}

		public void LoadPckFromFile(string filepath)
		{
			CheckSaveState();
			treeViewMain.Nodes.Clear();
			currentPCK = OpenPck(filepath);
			if (currentPCK == null)
			{
				MessageBox.Show(this, string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error");
				return;
			}

			CheckForPasswordAndRemove();
			LoadEditorTab();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			SettingsManager.Default.RegisterPropertyChangedCallback<bool>(nameof(Settings.Default.UseLittleEndianAsDefault), state =>
			{
				LittleEndianCheckBox.Checked = state;
			});
			SettingsManager.Default.RegisterPropertyChangedCallback(nameof(Settings.Default.LoadSubPcks), () =>
			{
				if (currentPCK is not null)
				{
					BuildMainTreeView();
				}
			});

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

			LoadRecentFileList();

			isSelectingTab = true;
			tabControl.SelectTab(0);
			isSelectingTab = false;

			UpdateRichPresence();

			if (!string.IsNullOrWhiteSpace(saveLocation))
				LoadPckFromFile(saveLocation);
		}

        private void LoadRecentFileList()
        {
            Settings.Default.RecentFiles ??= new System.Collections.Specialized.StringCollection();
            recentlyOpenToolStripMenuItem.DropDownItems.Clear();
			for (int i = 0; i < Settings.Default.RecentFiles.Count && i < 5; i++)
			{
				string filepath = Settings.Default.RecentFiles[i];
				if (!string.IsNullOrWhiteSpace(filepath))
				{
					string displayFilepath = Regex.Replace(filepath, @"([A-Z]{1}\:\\[Uu]sers\\)([^\\]*\\)(.*)", "~\\$3");
                    ToolStripItem item = recentlyOpenToolStripMenuItem.DropDownItems.Add(displayFilepath, null, HandleOpenFile);
					item.Tag = filepath;
				}
            }
        }

		private void HandleOpenFile(object sender, EventArgs e)
		{
			if (sender is ToolStripItem menuItem && menuItem.Tag is string filepath && File.Exists(filepath))
				LoadPckFromFile(filepath);
        }

		private void SaveToRecentFiles(string filepath)
		{
			if (filepath is null || string.IsNullOrWhiteSpace(filepath))
				return;
            if (Settings.Default.RecentFiles.Contains(filepath))
                Settings.Default.RecentFiles.Remove(filepath);
			Settings.Default.RecentFiles.Insert(0, filepath);

			for (int i = Settings.Default.RecentFiles.Count - 1; i >= 5; i--)
			{
                Settings.Default.RecentFiles.RemoveAt(i);
            }
			Settings.Default.Save();
			LoadRecentFileList();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			PckManager?.Close();
			CheckSaveState();
		}

        private void openPckManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PckManager ??= new PckManager();
            PckManager.FormClosing += (s, e) => PckManager = null;
            if (!PckManager.Visible)
            {
                // Passing in a parent form will make it stay on top of every other form. -miku
                PckManager.Show();
            }
            if (PckManager.Focus())
                PckManager.BringToFront();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                LoadPckFromFile(ofd.FileName);
            }
        }

		private PckFile OpenPck(string filePath)
		{
			isTemplateFile = false;
			saveLocation = filePath;
			SaveToRecentFiles(filePath);
            var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
			try
			{
				PckFile pck = reader.FromFile(filePath);
				return pck;
			}
			catch (OverflowException ex)
			{
				MessageBox.Show(this, "Failed to open pck\n" +
					$"Try {(LittleEndianCheckBox.Checked ? "unchecking" : "checking")} the 'Open/Save as Switch/Vita/PS4 pck' check box in the upper right corner.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Debug.WriteLine(ex.Message);
			}
			catch
			{
				MessageBox.Show(this, "Failed to open pck. There's two common reasons for this:\n" +
					"1. The file is audio/music cues PCK file. Please use the specialized editor while inside of a pck file.\n" +
					"2. We're aware of an issue where a pck file might fail to load because it contains multiple entries with the same path.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}

		private void CheckForPasswordAndRemove()
		{
			if (currentPCK.TryGetAsset("0", PckAssetType.InfoFile, out PckAsset asset))
			{
				asset.RemoveProperties("LOCK");
			}
		}

		private void LoadEditorTab()
		{
			fileEntryCountLabel.Text = "Files:" + currentPCK.AssetCount;
			if (isTemplateFile)
				pckFileLabel.Text = "Unsaved File!";
			else
				pckFileLabel.Text = Path.GetFileName(saveLocation);
			treeViewMain.Enabled = treeMeta.Enabled = true;
			closeToolStripMenuItem.Visible = true;
			fullBoxSupportToolStripMenuItem.Checked = currentPCK.HasVerionString;
			packSettingsToolStripMenuItem.Visible = true;

			saveToolStripMenuItem.Enabled = true;
			saveToolStripMenuItem1.Enabled = true;
			quickChangeToolStripMenuItem.Enabled = true;
			BuildMainTreeView();
			isSelectingTab = true;
			tabControl.SelectTab(1);
			isSelectingTab = false;
			UpdateRichPresence();
		}

		private void CloseEditorTab()
		{
			isSelectingTab = true;
			tabControl.SelectTab(0);
			isSelectingTab = false;
			currentPCK = null;
			wasModified = false;
			isTemplateFile = false;
			saveLocation = string.Empty;
			previewPictureBox.Image = Resources.NoImageFound;
			treeViewMain.Nodes.Clear();
			treeMeta.Nodes.Clear();
			treeViewMain.Enabled = false;
			treeMeta.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem1.Enabled = false;
			quickChangeToolStripMenuItem.Enabled = false;
			closeToolStripMenuItem.Visible = false;
			packSettingsToolStripMenuItem.Visible = false;
			fileEntryCountLabel.Text = string.Empty;
			pckFileLabel.Text = string.Empty;
			UpdateRichPresence();
		}

		private void UpdateRichPresence()
		{
			if (currentPCK is not null &&
				TryGetLocFile(out LOCFile locfile) &&
				locfile.HasLocEntry("IDS_DISPLAY_NAME") &&
				locfile.Languages.Contains("en-EN"))
			{
				RPC.SetPresence($"Editing a Pack: {locfile.GetLocEntry("IDS_DISPLAY_NAME", "en-EN")}");
				return;
			}
			// default
			RPC.SetPresence("An Open Source .PCK File Editor");
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
				// fix any file paths that may be incorrect
				//if (file.Filename.StartsWith(parentPath))
				//	file.Filename = file.Filename.Remove(0, parentPath.Length);
				TreeNode node = BuildNodeTreeBySeperator(root, asset.Filename, '/');
				node.Tag = asset;
				SetNodeIcon(node, asset.Type);
			}
		}

		private void BuildMainTreeView()
		{
			// In case the Rename function was just used and the selected node name no longer matches the file name
			string selectedNodeText = treeViewMain.SelectedNode is TreeNode node ? node.FullPath : string.Empty;
			previewPictureBox.Image = Resources.NoImageFound;
			treeMeta.Nodes.Clear();
			treeViewMain.Nodes.Clear();
			BuildPckTreeView(treeViewMain.Nodes, currentPCK);

			//if (isTemplateFile && currentPCK.HasAsset("Skins.pck", PckAssetType.SkinDataFile))
			//{
			//	TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
			//	TreeNode folderNode = CreateNode("Skins");
			//	folderNode.ImageIndex = 0;
			//	folderNode.SelectedImageIndex = 0;
			//	if (!skinsNode.Nodes.ContainsKey("Skins"))
			//		skinsNode.Nodes.Add(folderNode);
			//}
			treeViewMain.Sort();

			TreeNode[] selectedNodes = treeViewMain.FindPath(selectedNodeText);
			if (selectedNodes.Length > 0)
			{
				treeViewMain.SelectedNode = selectedNodes[0];
			}
		}

		private static Model HandleDefaultEntityModel(string modelName)
        {
			if (!GameModelImporter.DefaultModels.TryGetValue(modelName, out DefaultModel defaultModel) || defaultModel is null)
            {
                MessageBox.Show("No Default Model found.");
				return null;
            }
            Model model = new Model(modelName, new Size((int)defaultModel.TextureSize.X, (int)defaultModel.TextureSize.Y));

            foreach (DefaultPart defaultPart in defaultModel.Parts)
            {
                ModelPart modelPart = new ModelPart(defaultPart.Name, "", defaultPart.Translation, defaultPart.Rotation, System.Numerics.Vector3.Zero);
                modelPart.AddBoxes(defaultPart.Boxes.Select(defaultBox => new ModelBox(defaultBox.Position, defaultBox.Size, defaultBox.Uv, defaultBox.Inflate, defaultBox.Mirror)));
                model.AddPart(modelPart);
            }

			return model;
        }

        private void ShowSimpleModelRender(Model model, NamedTexture modelTexture)
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
                    var modelInfo = new GameModelInfo(model, new NamedTexture[1] { modelTexture });
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
            renderer.Texture = modelTexture.Texture;
            renderer.LoadModel(model);
            renderer.ResetCamera();
			
			form.ShowDialog(this);

            renderer.Dispose();
            form.Dispose();
        }

        private void HandleTextureFile(PckAsset asset)
		{
			_ = asset.IsMipmappedFile() && currentPCK.TryGetValue(asset.GetNormalPath(), PckAssetType.TextureFile, out asset);

			if (asset.Size <= 0)
			{
				Debug.WriteLine($"'{asset.Filename}' size is 0.", category: nameof(HandleTextureFile));
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

					NamedTexture modelTexture = new NamedTexture(Path.GetFileName(texturePath), asset.GetTexture());

                    Model model = HandleDefaultEntityModel(modelName);
                    if (currentPCK.TryGetAsset("models.bin", PckAssetType.ModelsFile, out PckAsset modelsAsset))
					{
						ModelContainer models = modelsAsset.GetData(new ModelFileReader());
						if (models.ContainsModel(modelName))
						{
							Debug.WriteLine($"Custom model for '{modelName}' found.");
							model = models.GetModelByName(modelName);
						}
					}

					if (model is not null)
					{
						ShowSimpleModelRender(model, modelTexture);
					}
				}
				break;

				case ResourceCategory.ItemAnimation:
				case ResourceCategory.BlockAnimation:
                    Animation animation = asset.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
                    string internalName = Path.GetFileNameWithoutExtension(asset.Filename);
					IList<JsonTileInfo> textureInfos = resourceLocation.Category == ResourceCategory.ItemAnimation ? Tiles.ItemTileInfos : Tiles.BlockTileInfos;
                    string displayname = textureInfos.FirstOrDefault(p => p.InternalName == internalName)?.DisplayName ?? internalName;

                    string[] specialTileNames = { "clock", "compass" };

					DelegatedSaveContext<Animation> saveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
					{
                        asset.SetSerializedData(animation, AnimationSerializer.DefaultSerializer);
					});

                    using (AnimationEditor animationEditor = new AnimationEditor(animation, saveContext, displayname, internalName.ToLower().EqualsAny(specialTileNames)))
                    {
                        if (animationEditor.ShowDialog(this) == DialogResult.OK)
                        {
                            wasModified = true;
                            BuildMainTreeView();
                        }
                    }
                    break;
				case ResourceCategory.ItemAtlas:
				case ResourceCategory.BlockAtlas:
				case ResourceCategory.ParticleAtlas:
				case ResourceCategory.BannerAtlas:
				case ResourceCategory.PaintingAtlas:
				case ResourceCategory.ExplosionAtlas:
				case ResourceCategory.ExperienceOrbAtlas:
				case ResourceCategory.MoonPhaseAtlas:
				case ResourceCategory.MapIconAtlas:
				case ResourceCategory.AdditionalMapIconsAtlas:
                    Image img = asset.GetTexture();
                    var viewer = new TextureAtlasEditor(currentPCK, resourceLocation, img);
                    if (viewer.ShowDialog(this) == DialogResult.OK)
                    {
                        Image texture = viewer.FinalTexture;
                        asset.SetTexture(texture);
                        wasModified = true;
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
			const string use_deflate = "PS3";
			const string use_xmem = "Xbox 360";
			const string use_zlib = "Wii U, PS Vita";

	        ItemSelectionPopUp dialog = new ItemSelectionPopUp(use_zlib, use_deflate, use_xmem);
            dialog.LabelText = "Type";
            dialog.ButtonText = "Ok";
			if (dialog.ShowDialog() != DialogResult.OK)
				return;

            GameRuleFile.CompressionType compressiontype = dialog.SelectedItem switch
			{
				use_deflate => GameRuleFile.CompressionType.Deflate,
                use_xmem => GameRuleFile.CompressionType.XMem,
                use_zlib => GameRuleFile.CompressionType.Zlib,
				_ => GameRuleFile.CompressionType.Unknown
            };

			GameRuleFile grf = asset.GetData(new GameRuleFileReader(compressiontype));

			DelegatedSaveContext<GameRuleFile> saveContext = new DelegatedSaveContext<GameRuleFile>(Settings.Default.AutoSaveChanges, (gameRuleFile) =>
			{
				asset.SetData(new GameRuleFileWriter(gameRuleFile));
			});

			using GameRuleFileEditor grfEditor = new GameRuleFileEditor(grf, saveContext);
			if (grfEditor.ShowDialog(this) == DialogResult.OK)
			{
				wasModified = true;
				UpdateRichPresence();
			}
		}

		private void HandleAudioFile(PckAsset asset)
		{
			try
            {
				OMI.Endianness endianness = LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian;
                DelegatedSaveContext<PckAudioFile> saveContext = new DelegatedSaveContext<PckAudioFile>(Settings.Default.AutoSaveChanges, (audioFile) =>
                {
                    asset.SetData(new PckAudioFileWriter(audioFile, endianness));
                });
				PckAudioFile audioFile = asset.GetData(new PckAudioFileReader(endianness));
                using AudioEditor audioEditor = new AudioEditor(audioFile, saveContext);
				wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
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
			DelegatedSaveContext<LOCFile> saveContext = new DelegatedSaveContext<LOCFile>(Settings.Default.AutoSaveChanges, (locFile) =>
			{
				asset.SetData(new LOCFileWriter(locFile, 2));
			});
			using LOCEditor locedit = new LOCEditor(locFile, saveContext);
			wasModified = locedit.ShowDialog(this) == DialogResult.OK;
			UpdateRichPresence();
		}

		private void HandleColourFile(PckAsset asset)
		{
			ColorContainer colorContainer = asset.GetData(new COLFileReader());
			DelegatedSaveContext<ColorContainer> saveContext = new DelegatedSaveContext<ColorContainer>(Settings.Default.AutoSaveChanges, (colorContainer) =>
			{
				asset.SetData(new COLFileWriter(colorContainer));
			});
			using COLEditor diag = new COLEditor(colorContainer, saveContext);
			wasModified = diag.ShowDialog(this) == DialogResult.OK;
        }

		public void HandleSkinFile(PckAsset asset)
		{
            Skin skin = asset.GetSkin();
            if (asset.HasProperty("CAPEPATH"))
			{
				string capePath = asset.GetProperty("CAPEPATH");
				if (currentPCK.TryGetAsset(capePath, PckAssetType.CapeFile, out PckAsset cape))
				{
					skin.CapeTexture = cape.GetTexture();
				}
			}

			using CustomSkinEditor skinEditor = new CustomSkinEditor(skin, currentPCK.HasVerionString);
			if (skinEditor.ShowDialog() == DialogResult.OK)
			{
				if (!TryGetLocFile(out LOCFile locFile))
					Debug.Fail("Failed to aquire loc file.");
                asset.SetSkin(skinEditor.ResultSkin, locFile);

				entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
				wasModified = true;
				ReloadMetaTreeView(); 
			}
        }

		public void HandleModelsFile(PckAsset asset)
		{
			ModelContainer modelContainer = asset.GetData(new ModelFileReader());
			if (modelContainer.ModelCount == 0)
			{
				MessageBox.Show("No models found.", "Empty Model file", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

            TryGetDelegate<string, Image> tryGetTexture = (string path, out Image img) =>
			{
				bool found = currentPCK.TryGetAsset(path + ".png", PckAssetType.TextureFile, out PckAsset asset) ||
							 currentPCK.TryGetAsset(path + ".tga", PckAssetType.TextureFile, out asset);
				img = found ? asset.GetTexture() : default;
				return found;
			};

            TrySetDelegate<string, Image> trySetTexture = (string path, Image img) =>
			{
                bool found = currentPCK.TryGetAsset(path + ".png", PckAssetType.TextureFile, out PckAsset foundAsset) ||
                             currentPCK.TryGetAsset(path + ".tga", PckAssetType.TextureFile, out foundAsset);
                PckAsset asset = foundAsset ?? currentPCK.CreateNewAsset(path + ".png", PckAssetType.TextureFile);
                asset.SetTexture(img);
				return true;
            };

			bool hasMaterialAsset = currentPCK.TryGetAsset("entityMaterials.bin", PckAssetType.MaterialFile, out PckAsset entityMaterialAsset);
			IReadOnlyDictionary<string, MaterialContainer.Material> entityMaterials =
				hasMaterialAsset
				? entityMaterialAsset?.GetData(new MaterialFileReader()).ToDictionary(mat => mat.Name)
				: new Dictionary<string, MaterialContainer.Material>();

            var editor = new ModelEditor(modelContainer, TryGetSet<string, Image>.FromDelegates(tryGetTexture, trySetTexture), TryGet<string, MaterialContainer.Material>.FromDelegate(entityMaterials.TryGetValue));
			if (editor.ShowDialog() == DialogResult.OK)
			{
				asset.SetData(new ModelFileWriter(editor.GetModelContainer(), modelContainer.Version));
				BuildMainTreeView();
                wasModified = true;
				return;
			}
		}

		public void HandleBehavioursFile(PckAsset asset)
		{
			BehaviourFile behaviourFile = asset.GetData(new BehavioursReader());
			DelegatedSaveContext<BehaviourFile> saveContext = new DelegatedSaveContext<BehaviourFile>(Settings.Default.AutoSaveChanges, (behaviourFile) =>
			{
				asset.SetData(new BehavioursWriter(behaviourFile));
			});
			using BehaviourEditor edit = new BehaviourEditor(behaviourFile, saveContext);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}

		public void HandleMaterialFile(PckAsset asset)
		{
			MaterialContainer materials = asset.GetData(new MaterialFileReader());
			DelegatedSaveContext<MaterialContainer> saveContext = new DelegatedSaveContext<MaterialContainer>(Settings.Default.AutoSaveChanges, (materials) =>
			{
				asset.SetData(new MaterialFileWriter(materials));
			});
            using MaterialsEditor edit = new MaterialsEditor(materials, saveContext);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}

		private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ReloadMetaTreeView();

			entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
			buttonEdit.Visible = false;

			previewPictureBox.Image = Resources.NoImageFound;
			viewFileInfoToolStripMenuItem.Visible = false;
			
			if (e.Node.TryGetTagData(out PckAsset asset))
			{
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
		}

		private void extractFile(string outFilePath, PckAsset asset)
        {
			File.WriteAllBytes(outFilePath, asset.Data);
			if (asset.PropertyCount > 0)
			{
				File.WriteAllLines($"{outFilePath}.txt", asset.SerializeProperties());
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

		private void extractFolder(string outPath)
        {
			TreeNode node = treeViewMain.SelectedNode;

			string selectedFolder = node.FullPath;

            foreach (PckAsset asset in currentPCK.GetAssets().Where(asset => asset.Filename.StartsWith(selectedFolder)))
            {
				extractFolderFile(outPath, asset);
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

		private void SaveTemplate()
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "PCK (Minecraft Console Package)|*.pck";
			saveFileDialog.DefaultExt = ".pck";
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				Save(saveFileDialog.FileName);
				saveLocation = saveFileDialog.FileName;
				SaveToRecentFiles(saveFileDialog.FileName);
                pckFileLabel.Text = Path.GetFileName(saveLocation);
				isTemplateFile = false;
			}
		}

		private void Save(string filePath)
		{
			var writer = new PckFileWriter(currentPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
			writer.WriteToFile(filePath);
			wasModified = false;
			MessageBox.Show(this, "Saved Pck file", "File Saved");
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
					wasModified = true;
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
				if (!BeforeFileRemove(asset) && currentPCK.RemoveAsset(asset))
				{
					node.Remove();
					wasModified = true;
				}
			}
			else if (MessageBox.Show(this, "Are you sure want to delete this folder? All contents will be deleted", "Warning",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				string pckFolderDir = node.FullPath;
				currentPCK.RemoveAll(file => file.Filename.StartsWith(pckFolderDir) && !BeforeFileRemove(file));
				node.Remove();
				wasModified = true;
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
					if (currentPCK.Contains(diag.NewText, asset.Type))
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
				wasModified = true;
				BuildMainTreeView();
			}
		}

		private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (AddSkinPrompt addNewSkinDialog = new AddSkinPrompt())
				if (addNewSkinDialog.ShowDialog() == DialogResult.OK)
				{
					TryGetLocFile(out LOCFile locFile);
                    PckAsset skinAsset = addNewSkinDialog.NewSkin.CreateFile(locFile);
					currentPCK.AddAsset(skinAsset);
     //               if (currentPCK.HasAsset("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
					//{
					//	TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
					//	if (subPCK.Nodes.ContainsKey("Skins"))
					//		skinAsset.Filename = skinAsset.Filename.Insert(0, "Skins/");
					//	skinAsset.Filename = skinAsset.Filename.Insert(0, "Skins.pck/");
					//	TreeNode newNode = new TreeNode(Path.GetFileName(skinAsset.Filename));
					//	newNode.Tag = skinAsset;
					//	SetNodeIcon(newNode, PckAssetType.SkinFile);
					//	subPCK.Nodes.Add(newNode);
					//	RebuildSubPCK(newNode.FullPath);
					//}
					//else
					{
						if (treeViewMain.Nodes.ContainsKey("Skins"))
							skinAsset.Filename = skinAsset.Filename.Insert(0, "Skins/"); // Then Skins folder
						currentPCK.AddAsset(skinAsset);
					}

					if (addNewSkinDialog.NewSkin.HasCape)
					{
                        PckAsset capeFile = addNewSkinDialog.NewSkin.CreateCapeFile();
                        //if (currentPCK.HasAsset("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
						//{
						//	TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
						//	if (subPCK.Nodes.ContainsKey("Skins"))
						//		capeFile.Filename = capeFile.Filename.Insert(0, "Skins/");
						//	capeFile.Filename = capeFile.Filename.Insert(0, "Skins.pck/");
						//	TreeNode newNode = new TreeNode(Path.GetFileName(capeFile.Filename));
						//	newNode.Tag = capeFile;
						//	SetNodeIcon(newNode, PckAssetType.SkinFile);
						//	subPCK.Nodes.Add(newNode);
						//	RebuildSubPCK(newNode.FullPath);
						//}
						//else
						{
							if (treeViewMain.Nodes.ContainsKey("Skins"))
								capeFile.Filename = capeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
							currentPCK.AddAsset(capeFile);
						}
					}

				TrySetLocFile(locFile);
				wasModified = true;
				BuildMainTreeView();
			}
		}

		private static PckAsset CreateNewAudioAsset(bool isLittle, PckAudioFile audioFile)
		{
			PckAsset newAsset = new PckAsset("audio.pck", PckAssetType.AudioFile);
			newAsset.SetData(new PckAudioFileWriter(audioFile, isLittle ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
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

		private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.Contains(PckAssetType.AudioFile))
			{
				// the chance of this happening is really really slim but just in case
				MessageBox.Show(this, "There is already an audio file in this PCK!", "Can't create audio.pck");
				return;
			}
			if (string.IsNullOrEmpty(saveLocation))
			{
				MessageBox.Show(this, "You must save your pck before creating or opening a music cues PCK file", "Can't create audio.pck");
				return;
			}

            PckAudioFile newAudioFile = CreateNewAudioFile();
            PckAsset newAudioAsset = CreateNewAudioAsset(LittleEndianCheckBox.Checked, newAudioFile);

			DelegatedSaveContext<PckAudioFile> saveContext = new DelegatedSaveContext<PckAudioFile>(Settings.Default.AutoSaveChanges, (audioFile) =>
			{
				newAudioAsset.SetData(new PckAudioFileWriter(audioFile, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
			});

			AudioEditor diag = new AudioEditor(newAudioFile, saveContext);
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				currentPCK.AddAsset(newAudioAsset);
			}
			diag.Dispose();
			BuildMainTreeView();
		}

		private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using ChangeTile diag = new ChangeTile();
			if (diag.ShowDialog(this) != DialogResult.OK)
				return;

			string animationFilepath = $"{ResourceLocation.GetPathFromCategory(diag.Category)}/{diag.SelectedTile.InternalName}.png";

            if (currentPCK.Contains(animationFilepath, PckAssetType.TextureFile))
			{
				MessageBox.Show(this, $"{diag.SelectedTile.DisplayName} is already present.", "File already present");
				return;
			}

			Animation newAnimation = default;
			DelegatedSaveContext<Animation> saveContext = new DelegatedSaveContext<Animation>(Settings.Default.AutoSaveChanges, (animation) =>
			{
				newAnimation = animation;
            });

            using AnimationEditor animationEditor = new AnimationEditor(Animation.CreateEmpty(), saveContext, diag.SelectedTile.DisplayName, diag.SelectedTile.InternalName.EqualsAny("clock", "compass"));
			if (animationEditor.ShowDialog() == DialogResult.OK && newAnimation is not null)
			{
				wasModified = true;
				PckAsset asset = currentPCK.CreateNewAsset(animationFilepath, PckAssetType.TextureFile);
				asset.SetSerializedData(newAnimation, AnimationSerializer.DefaultSerializer);
				BuildMainTreeView();
				ReloadMetaTreeView();
			}
		}

        private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
			{
				if (asset.Size <= 0)
				{
					Trace.WriteLine($"'{asset.Filename}' has no data attached.", category: nameof(treeViewMain_DoubleClick));
					return;
				}
				pckFileTypeHandler[asset.Type]?.Invoke(asset);
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
									wasModified = true;
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
									wasModified = true;
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
							wasModified = true;
						}
					}
				}
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

					currentPCK.InsertAsset(node.Index + 1, newFile);
					BuildMainTreeView();
					wasModified = true;
				}
			}
		}

		private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode is TreeNode t && t.Tag is KeyValuePair<string, string> property &&
				treeViewMain.SelectedNode is TreeNode main && main.Tag is PckAsset asset &&
				asset.RemoveProperty(property))
			{
				treeMeta.SelectedNode.Remove();
				wasModified = true;
			}
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

		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PckAsset asset)
			{
				using AddPropertyPrompt addProperty = new AddPropertyPrompt();
				if (addProperty.ShowDialog(this) == DialogResult.OK)
				{
					asset.AddProperty(addProperty.Property);
					ReloadMetaTreeView();
					wasModified = true;
				}
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

			if ((node.TryGetTagData(out PckAsset asset) && currentPCK.Contains(asset.Filename, asset.Type)) || node.Parent is TreeNode)
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
			FocusMe();
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
				wasModified = true;
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
                wasModified = true;
                BuildMainTreeView();
            }
		}

		IEnumerable<TreeNode> GetAllChildNodes(TreeNodeCollection root)
		{
			List<TreeNode> childNodes = new List<TreeNode>(root.Count);
			foreach (TreeNode childNode in root)
			{
				childNodes.Add(childNode);
				childNodes.AddRange(GetAllChildNodes(childNode.Nodes));
			}
			return childNodes;
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

                if (currentPCK.Contains(filepath, assetType))
                {
                    if (askForAssetType)
						MessageBox.Show(this, $"'{assetPath}' of type {assetType} already exists.\nSkiping file.", "File already exists", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    Debug.WriteLine($"'{assetPath}' of type {assetType} already exists.\nSkiping file.");
                    continue;
                }
                PckAsset importedAsset = currentPCK.CreateNewAsset(assetPath, assetType, () => File.ReadAllBytes(filepath));
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
                wasModified = true;
				BuildMainTreeView();
			}
        }

        #endregion

        private PckFile InitializePack(int packId, int packVersion, string packName, bool createSkinsPCK)
		{
			var pack = new PckFile(3);

            PckAsset zeroAsset = pack.CreateNewAsset("0", PckAssetType.InfoFile);
			zeroAsset.AddProperty("PACKID", packId);
			zeroAsset.AddProperty("PACKVERSION", packVersion);

			var locFile = new LOCFile();
			locFile.InitializeDefault(packName);
			pack.CreateNewAsset("localisation.loc", PckAssetType.LocalisationFile, new LOCFileWriter(locFile, 2));

			pack.CreateNewAssetIf(createSkinsPCK, "Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
				LittleEndianCheckBox.Checked
					? OMI.Endianness.LittleEndian
					: OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeTexturePack(int packId, int packVersion, string packName, string res, bool createSkinsPCK)
		{
            PckFile pack = InitializePack(packId, packVersion, packName, createSkinsPCK);

			PckFile infoPCK = new PckFile(3);

            PckAsset iconAsset = infoPCK.CreateNewAsset("icon.png", PckAssetType.TextureFile);
			iconAsset.SetTexture(Resources.TexturePackIcon);

            PckAsset comparisonAsset = infoPCK.CreateNewAsset("comparison.png", PckAssetType.TextureFile);
			comparisonAsset.SetTexture(Resources.Comparison);

            PckAsset texturepackInfoAsset = pack.CreateNewAsset($"{res}/{res}Info.pck", PckAssetType.TexturePackInfoFile);
			texturepackInfoAsset.AddProperty("PACKID", "0");
			texturepackInfoAsset.AddProperty("DATAPATH", $"{res}Data.pck");

			texturepackInfoAsset.SetData(new PckFileWriter(infoPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeMashUpPack(int packId, int packVersion, string packName, string res)
		{
            PckFile pack = InitializeTexturePack(packId, packVersion, packName, res, true);
            PckAsset gameRuleAsset = pack.CreateNewAsset("GameRules.grf", PckAssetType.GameRulesFile);
            GameRuleFile grfFile = new GameRuleFile();
			grfFile.AddRule("MapOptions",
				new KeyValuePair<string, string>("seed", "0"),
				new KeyValuePair<string, string>("baseSaveName", string.Empty),
				new KeyValuePair<string, string>("flatworld", "false"),
				new KeyValuePair<string, string>("texturePackId", packId.ToString())
				);
			grfFile.AddRule("LevelRules")
				.AddRule("UpdatePlayer",
				new KeyValuePair<string, string>("yRot", "0"),
				new KeyValuePair<string, string>("xRot", "0"),
				new KeyValuePair<string, string>("spawnX", "0"),
				new KeyValuePair<string, string>("spawnY", "0"),
				new KeyValuePair<string, string>("spawnZ", "0")
				);

			gameRuleAsset.SetData(new GameRuleFileWriter(grfFile));

			return pack;
		}

		private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckSaveState();
			TextPrompt namePrompt = new TextPrompt();
			namePrompt.OKButtonText = "Ok";
			if (namePrompt.ShowDialog(this) == DialogResult.OK)
			{
				currentPCK = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, false);
                MarkTemplateFile();
                LoadEditorTab();
			}
		}

		private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckSaveState();
			CreateTexturePackPrompt packPrompt = new CreateTexturePackPrompt();
			if (packPrompt.ShowDialog(this) == DialogResult.OK)
            {
                currentPCK = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes, packPrompt.CreateSkinsPck);
                MarkTemplateFile();
                LoadEditorTab();
            }
        }

        private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckSaveState();
			CreateTexturePackPrompt packPrompt = new CreateTexturePackPrompt();
			if (packPrompt.ShowDialog(this) == DialogResult.OK)
			{
				currentPCK = InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
                MarkTemplateFile();
                LoadEditorTab();
			}
		}

        private void MarkTemplateFile()
        {
            isTemplateFile = true;
            wasModified = true;
            saveLocation = string.Empty;
        }

		private void quickChangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using AdvancedOptions advanced = new AdvancedOptions(currentPCK);
			advanced.IsLittleEndian = LittleEndianCheckBox.Checked;
			if (advanced.ShowDialog(this) == DialogResult.OK)
			{
				wasModified = true;
				BuildMainTreeView();
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckSaveState();
			CloseEditorTab();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using ContributorsForm info = new ContributorsForm();
			info.ShowDialog(this);
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

		[Obsolete("Move this")]
		public string GetDataPath()
		{
			return Path.Combine(Path.GetDirectoryName(saveLocation), "Data");
		}

		[Obsolete("Move this")]
		public bool HasDataFolder()
		{
			return Directory.Exists(GetDataPath());
		}

		[Obsolete("Move this")]
		public bool CreateDataFolder()
		{
			if (!HasDataFolder())
			{
				DialogResult result = MessageBox.Show(this, "There is not a \"Data\" folder present in the pack folder. Would you like to create one?", "Folder missing", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
					return false;
				else
					Directory.CreateDirectory(GetDataPath());
			}
			return true;
		}

		private void treeMeta_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
				deleteEntryToolStripMenuItem_Click(sender, e);
		}
		
		private bool TryGetLocFile(out LOCFile locFile)
		{
			if (!currentPCK.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locAsset) &&
				!currentPCK.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locAsset))
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
			if (!currentPCK.TryGetAsset("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locAsset) &&
				!currentPCK.TryGetAsset("languages.loc", PckAssetType.LocalisationFile, out locAsset))
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

        [Obsolete("Refactor or remove this")]
		private void importExtractedSkinsFolder(object sender, EventArgs e)
		{
			OpenFolderDialog contents = new OpenFolderDialog();
			if (contents.ShowDialog(Handle) == true && Directory.Exists(contents.ResultPath))
			{
				string filepath = treeViewMain.SelectedNode?.FullPath ?? "";
				if (treeViewMain.SelectedNode is not null && treeViewMain.SelectedNode.IsTagOfType<PckAsset>())
					filepath = treeViewMain.SelectedNode.Parent?.FullPath ?? "";

                foreach (var fullfilename in Directory.GetFiles(contents.ResultPath, "dlc*.png"))
				{
					string filename = Path.GetFileName(fullfilename);
					// only accept skin or cape named files
					if (!filename.StartsWith("dlcskin") && !filename.StartsWith("dlccape"))
						continue;
					// sets file type based on wether its a cape or skin
					PckAssetType pckfiletype = filename.StartsWith("dlccape", StringComparison.OrdinalIgnoreCase)
						? PckAssetType.CapeFile
						: PckAssetType.SkinFile;
					string pckfilepath = Path.Combine(filepath, filename);

					if (currentPCK.Contains(pckfilepath, pckfiletype))
					{
						Trace.TraceInformation("[{0}] {1} already exists.", nameof(importExtractedSkinsFolder), pckfilepath);
						continue;
					}

					PckAsset newAsset = currentPCK.CreateNewAsset(pckfilepath, pckfiletype);
					byte[] filedata = File.ReadAllBytes(fullfilename);
					newAsset.SetData(filedata);

					if (File.Exists(fullfilename + ".txt"))
					{
						string[] propertiesFileContent = File.ReadAllLines(fullfilename + ".txt");
						newAsset.DeserializeProperties(propertiesFileContent);
					}
				}
				BuildMainTreeView();
				wasModified = true;
			}
		}

		[Obsolete("Refactor or remove this")]
		private void importSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog contents = new OpenFileDialog())
			{
				contents.Title = "Select Extracted Skin File";
				contents.Filter = "Skin File (*.png)|*.png";

				if (contents.ShowDialog(this) == DialogResult.OK)
				{
					string skinNameImport = Path.GetFileName(contents.FileName);
					byte[] data = File.ReadAllBytes(contents.FileName);

                    if (currentPCK.Contains(skinNameImport, PckAssetType.SkinFile))
                    {
                        Trace.TraceInformation("[{0}] {1} already exists.", nameof(importExtractedSkinsFolder), skinNameImport);
                        return;
                    }

                    PckAsset importSkinAsset = currentPCK.CreateNewAsset(skinNameImport, PckAssetType.SkinFile);
					importSkinAsset.SetData(data);
					string propertyFile = contents.FileName + ".txt";
					if (File.Exists(propertyFile))
					{
						string[] txtProperties = File.ReadAllLines(propertyFile);
						importSkinAsset.DeserializeProperties(txtProperties);

                        // Because extracting/exporting an assest doesn't export
                        // the actual loc value we just get an undefined loc key.
                        // That's why why remove them after deserializing.
                        // - Miku
                        importSkinAsset.RemoveProperty("DISPLAYNAMEID");
						importSkinAsset.RemoveProperty("THEMENAMEID");
                        BuildMainTreeView();
                        wasModified = true;
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

		private void pckCenterReleaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=E_6bXSh6yqw");
		}

		private void howPCKsWorkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=hTlImrRrCKQ");
		}

		private void toPhoenixARCDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://cash.app/$PhoenixARC");
		}

		private void toNobledezJackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.me/realnobledez");
		}

		private void forMattNLContributorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/mattnl");
		}

		private void joinDevelopmentDiscordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/aJtZNFVQTv");
		}

		private void OpenPck_MouseEnter(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckOpen;
		}

		private void OpenPck_MouseLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void CheckSaveState()
		{
			if (currentPCK is not null &&
				wasModified &&
				MessageBox.Show(this, "Save PCK?", "Unsaved PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop) ?? Array.Empty<string>();
			e.Effect = files.Any(file => Path.GetExtension(file).Equals(".pck", StringComparison.CurrentCultureIgnoreCase))
				? DragDropEffects.Copy
				: DragDropEffects.None;
		}

		private void OpenPck_DragDrop(object sender, DragEventArgs e)
		{
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (filepaths.Length > 1)
				MessageBox.Show(this, "Only one pck file at a time is currently supported");
			LoadPckFromFile(filepaths[0]);
		}

		private void OpenPck_DragLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void OpenPck_Click(object sender, EventArgs e)
		{
			openToolStripMenuItem_Click(sender, e);
		}

		private void savePCK(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(saveLocation))
				Save(saveLocation);
			if (string.IsNullOrWhiteSpace(saveLocation) || isTemplateFile)
				SaveTemplate();
		}

		private void saveAsPCK(object sender, EventArgs e)
		{
			SaveTemplate();
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
                    goto default;
					//node.ImageIndex = 4;
					//node.SelectedImageIndex = 4;
					//break;
				case PckAssetType.ColourTableFile:
					node.ImageIndex = 6;
					node.SelectedImageIndex = 6;
					break;
				case PckAssetType.ModelsFile:
					node.ImageIndex = 8;
					node.SelectedImageIndex = 8;
					break;
				case PckAssetType.SkinDataFile:
					goto default;
					//node.ImageIndex = 7;
					//node.SelectedImageIndex = 7;
					//break;
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

		private void SetFileType(PckAssetType type)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
			{
				Debug.WriteLine($"Setting {asset.Type} to {type}");
				asset.Type = type;
				SetNodeIcon(treeViewMain.SelectedNode, type);
			}
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
                    if (currentPCK.Contains(renamePrompt.NewText, PckAssetType.TextureFile))
                    {
                        MessageBox.Show(this, $"'{renamePrompt.NewText}' already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    PckAsset asset = currentPCK.CreateNewAsset(renamePrompt.NewText, PckAssetType.TextureFile, () => File.ReadAllBytes(fileDialog.FileName));
					BuildMainTreeView();
					wasModified = true;
				}
			}
		}

		private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckAsset asset)
			{
				MessageBox.Show(this,
					"File path: " + asset.Filename +
					"\nAssigned File type: " + (int)asset.Type + " (" + asset.Type + ")" +
					"\nFile size: " + asset.Size +
					"\nProperties count: " + asset.PropertyCount
					, Path.GetFileName(asset.Filename) + " file info");
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
						if (currentPCK.HasAsset(mippedPath, PckAssetType.TextureFile))
							currentPCK.RemoveAsset(currentPCK.GetAsset(mippedPath, PckAssetType.TextureFile));
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

						currentPCK.InsertAsset(currentPCK.IndexOfAsset(asset) + i - 1, mipMappedAsset);
					}
					BuildMainTreeView();
				}
			}
		}

		private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetAsset("colours.col", PckAssetType.ColourTableFile, out _))
			{
				MessageBox.Show(this, "A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
            PckAsset newColorAsset = currentPCK.CreateNewAsset("colours.col", PckAssetType.ColourTableFile);
			newColorAsset.SetData(Resources.tu69colours);
			BuildMainTreeView();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			e.Cancel = !isSelectingTab;
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

		private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
			{
                using var input = new MultiTextPrompt();
                if (input.ShowDialog(this) == DialogResult.OK)
                {
                    asset.DeserializeProperties(input.TextOutput);
                    ReloadMetaTreeView();
                    wasModified = true;
                }
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
				wasModified = true;
			}
		}

		private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetAsset("Skins.pck", PckAssetType.SkinDataFile, out _))
			{
				MessageBox.Show(this, "A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewAsset("Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
					LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));

			BuildMainTreeView();
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
						wasModified = true;
					}
				}
			}
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
					if (currentPCK.Contains(diag.Filepath, diag.Filetype))
					{
						MessageBox.Show(this, $"'{diag.Filepath}' of type {diag.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
					PckAsset asset = currentPCK.CreateNewAsset(diag.Filepath, diag.Filetype, () => File.ReadAllBytes(ofd.FileName));

					BuildMainTreeView();
					wasModified = true;
				}
			}
			return;
		}

		private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetAsset("behaviours.bin", PckAssetType.BehavioursFile, out _))
			{
				MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewAsset("behaviours.bin", PckAssetType.BehavioursFile, new BehavioursWriter(new BehaviourFile()));
			BuildMainTreeView();
		}

		private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetAsset("entityMaterials.bin", PckAssetType.MaterialFile, out _))
			{
				MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			var materialContainer = new MaterialContainer();
			materialContainer.InitializeDefault();
			currentPCK.CreateNewAsset("entityMaterials.bin", PckAssetType.MaterialFile, new MaterialFileWriter(materialContainer));
			BuildMainTreeView();
		}

		private void trelloBoardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://trello.com/b/0XLNOEbe/pck-studio");
		}

		private void wavBinkaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "WAV files (*.wav)|*.wav",
				Title = "Please choose WAV files to convert to BINKA"
			};
			if (fileDialog.ShowDialog(this) == DialogResult.OK)
			{
				using ItemSelectionPopUp dialog = new ItemSelectionPopUp(
					"Level 1 (Best Quality)", "Level 2", "Level 3", "Level 4", "Level 5", 
					"Level 6", "Level 7", "Level 8", "Level 9 (Worst Quality)")
				{
					LabelText = "Compression",
					ButtonText = "OK"
				};

				if (dialog.ShowDialog(this) == DialogResult.OK)
                {
					BinkaConverter.ToBinka(
						fileDialog.FileNames, 
						new DirectoryInfo(Path.GetDirectoryName(fileDialog.FileName)), 
						dialog.SelectedIndex + 1 // compression level
						);
				}
			}
		}

		private void binkaWavToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "BINKA files (*.binka)|*.binka",
				Title = "Please choose BINKA files to convert to WAV"
			};
			if (fileDialog.ShowDialog(this) == DialogResult.OK)
			{
				BinkaConverter.ToWav(fileDialog.FileNames, new DirectoryInfo(Path.GetDirectoryName(fileDialog.FileName)));
			}
		}

		private void fullBoxSupportToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			currentPCK.SetVersion(fullBoxSupportToolStripMenuItem.Checked);
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var appSettings = new AppSettingsForm();
			appSettings.ShowDialog(this);
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
					wasModified = true;
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
					wasModified = true;
				}
				return;
			}
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Updater.UpdateToLatest();
		}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
			PckManager?.Close();
            Application.Exit();
        }

		private void SetPckEndianness(OMI.Endianness endianness)
		{
			try
			{
				if (treeViewMain.SelectedNode.Tag is PckAsset asset && (asset.Type is PckAssetType.AudioFile || asset.Type is PckAssetType.SkinDataFile || asset.Type is PckAssetType.TexturePackInfoFile))
				{
					IDataFormatReader reader = asset.Type is PckAssetType.AudioFile
						? new PckAudioFileReader(endianness == OMI.Endianness.BigEndian ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian)
						: new PckFileReader(endianness == OMI.Endianness.BigEndian ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                    object pck = reader.FromStream(new MemoryStream(asset.Data));

                    IDataFormatWriter writer = asset.Type is PckAssetType.AudioFile
						? new PckAudioFileWriter((PckAudioFile)pck, endianness)
						: new PckFileWriter((PckFile)pck, endianness);
					asset.SetData(writer);
					wasModified = true;
					MessageBox.Show($"\"{asset.Filename}\" successfully converted to {(endianness == OMI.Endianness.LittleEndian ? "little" : "big")} endian.", "Converted PCK file");
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
					wasModified = true;
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

        private void contextMenuPCKEntries_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
			if (treeViewMain?.SelectedNode == null)
			{
				e.Cancel = true;
				return;
			}

			correctSkinDecimalsToolStripMenuItem.Visible = false;
			generateMipMapTextureToolStripMenuItem1.Visible = false;
            setModelContainerFormatToolStripMenuItem.Visible = false;
            setSubPCKEndiannessToolStripMenuItem.Visible = false;
            exportToolStripMenuItem.Visible = false;
			toolStripSeparator5.Visible = false;
			toolStripSeparator6.Visible = false;
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
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

        private void treeViewMain_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			if (e.Node is not null)
				treeViewMain.SelectedNode = e.Node;
        }
    }
}