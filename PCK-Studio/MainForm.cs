using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Drawing.Imaging;
using OMI.Formats.Pck;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Formats.Model;
using OMI.Workers.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using OMI.Workers.Model;
using PckStudio.Properties;
using PckStudio.FileFormats;
using PckStudio.Forms;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Classes.Misc;
using PckStudio.Internal.IO.PckAudio;
using PckStudio.Internal.IO._3DST;
using PckStudio.Internal;
using PckStudio.Forms.Features;
using PckStudio.Extensions;
using PckStudio.Popups;
using PckStudio.Classes.Utils;
using PckStudio.Helper;
using System.Text.RegularExpressions;
using PckStudio.Internal.Json;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;
using OMI.Workers;

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

			labelVersion.Text = $"{Application.ProductName}: {Application.ProductVersion}";
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
				[PckAssetType.ModelsFile] = null, //HandleModelsFile, // Note: Uncomment when implemented
				[PckAssetType.BehavioursFile] = HandleBehavioursFile,
				[PckAssetType.MaterialFile] = HandleMaterialFile,
			};
		}

		// TODO: decide on how to handle embedded pck files
        private void HandleInnerPckFile(PckAsset file)
        {
			if (Settings.Default.LoadSubPcks &&
				(file.Type == PckAssetType.SkinDataFile || file.Type == PckAssetType.TexturePackInfoFile) &&
				file.Size > 0 && treeViewMain.SelectedNode.Nodes.Count == 0)
			{
				try
				{
					PckFile subPCKfile = file.GetData(new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
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
			SettingsManager.RegisterPropertyChangedCallback<bool>(nameof(Settings.Default.UseLittleEndianAsDefault), state =>
			{
				LittleEndianCheckBox.Checked = state;
			});
			SettingsManager.RegisterPropertyChangedCallback(nameof(Settings.Default.LoadSubPcks), () =>
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
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckFileExists = true;
				ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					LoadPckFromFile(ofd.FileName);
				}
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
			if (currentPCK.TryGetFile("0", PckAssetType.InfoFile, out PckAsset file))
			{
				file.RemoveProperties("LOCK");
			}
		}

		private void LoadEditorTab()
		{
			fileEntryCountLabel.Text = "Files:" + currentPCK.FileCount;
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
			if (!alreadyExists) root.Add(subNode);
			return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
		}

		private void BuildPckTreeView(TreeNodeCollection root, PckFile pckFile)
		{
			foreach (PckAsset file in pckFile.GetFiles())
			{
				// fix any file paths that may be incorrect
				//if (file.Filename.StartsWith(parentPath))
				//	file.Filename = file.Filename.Remove(0, parentPath.Length);
				TreeNode node = BuildNodeTreeBySeperator(root, file.Filename, '/');
				node.Tag = file;
				SetNodeIcon(node, file.Type);
			};
		}

		private void BuildMainTreeView()
		{
			// In case the Rename function was just used and the selected node name no longer matches the file name
			string selectedNodeText = treeViewMain.SelectedNode is TreeNode node ? node.Text : string.Empty;
			previewPictureBox.Image = Resources.NoImageFound;
			treeMeta.Nodes.Clear();
			treeViewMain.Nodes.Clear();
			BuildPckTreeView(treeViewMain.Nodes, currentPCK);

			if (isTemplateFile && currentPCK.HasFile("Skins.pck", PckAssetType.SkinDataFile))
			{
				TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
				TreeNode folderNode = CreateNode("Skins");
				folderNode.ImageIndex = 0;
				folderNode.SelectedImageIndex = 0;
				if (!skinsNode.Nodes.ContainsKey("Skins"))
					skinsNode.Nodes.Add(folderNode);
			}
			treeViewMain.Sort();

			TreeNode[] selectedNodes;
			if (!string.IsNullOrEmpty(selectedNodeText) &&
				(selectedNodes = treeViewMain.Nodes.Find(selectedNodeText, true)).Length > 0)
			{
				treeViewMain.SelectedNode = selectedNodes[0];
			}
		}

		private void HandleTextureFile(PckAsset file)
		{
			_ = file.IsMipmappedFile() && currentPCK.TryGetValue(file.GetNormalPath(), PckAssetType.TextureFile, out file);

			if (file.Size <= 0)
			{
				Debug.WriteLine($"'{file.Filename}' size is 0.", category: nameof(HandleTextureFile));
				return;
			}

			bool isTerrain = file.Filename == "res/terrain.png";
			bool isItems = file.Filename == "res/items.png";
			bool isParticles = file.Filename == "res/particles.png";
			bool isMoonPhases = file.Filename == "res/terrain/moon_phases.png";
			bool isMapIcons = file.Filename == "res/misc/mapicons.png";
			bool isAdditionalMapIcons = file.Filename == "res/misc/additionalmapicons.png";
			bool isXPOrbs = file.Filename == "res/item/xporb.png";
			bool isExplosions = file.Filename == "res/misc/explosion.png";
			bool isPaintings = file.Filename == "res/art/kz.png";
			bool isBanners = file.Filename == "res/item/banner/Banner_Atlas.png";

			if (
				isTerrain || isItems || isParticles || isMoonPhases || isPaintings ||
				isMapIcons || isAdditionalMapIcons || isXPOrbs || isExplosions || isBanners
				)
			{
                Image img = file.GetTexture();
				var tile_size = new Size();

                int banner_scale = img.Width / Resources.banners_atlas.Width;

				if (isBanners)
				{
					// The banner atlas has extra space on it that has to be truncated for the editor
					img = img.GetArea(new Rectangle(0, 0, img.Width - (4 * banner_scale), img.Height - (1 * banner_scale)));

					// banners are 42x41 because of course they are
					tile_size = new Size(42 * banner_scale, 41 * banner_scale);
				}

                // most atlases have 4 columns
                int columnCount = isBanners ? 6 : 4;

				if (isTerrain || isItems || isParticles || isPaintings) columnCount = 16;

				if (!isBanners)
                {
                    int resolution = img.Width / columnCount;
					tile_size = new Size(resolution, resolution);
				}

				var viewer = new TextureAtlasEditor(currentPCK, file.Filename, img, tile_size);
				if (viewer.ShowDialog(this) == DialogResult.OK)
				{
                    Image texture = viewer.FinalTexture;
					if(isBanners)
                    {
						var graphicsConfig = new GraphicsConfig()
						{
							InterpolationMode = InterpolationMode.NearestNeighbor,
							PixelOffsetMode = PixelOffsetMode.HighQuality
						};

						var _img = new Bitmap((Resources.banners_atlas.Width + 4) * banner_scale,
									(Resources.banners_atlas.Height + 1) * banner_scale);

						using (Graphics g = Graphics.FromImage(_img))
                        {
							g.ApplyConfig(graphicsConfig);
							g.DrawImage(texture, 0, 0, texture.Width, texture.Height);
                        }

						texture = _img;
                    }

					file.SetTexture(texture);
					wasModified = true;
					BuildMainTreeView();
				}
				return;
			}

			if (!file.Filename.StartsWith(ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation)) &&
				!file.Filename.StartsWith(ResourceLocation.GetPathFromCategory(ResourceCategory.BlockAnimation)))
				return;

            Animation animation = file.GetDeserializedData(AnimationDeserializer.DefaultDeserializer);
			string filename = Path.GetFileNameWithoutExtension(file.Filename);

            var textureInfos = ResourceLocation.GetCategoryFromPath(file.Filename) switch
			{
				ResourceCategory.BlockAnimation => Tiles.BlockTileInfos,
				ResourceCategory.ItemAnimation => Tiles.ItemTileInfos,
				_ => Array.Empty<JsonTileInfo>().ToList()
			};
			string displayname = textureInfos.FirstOrDefault(p => p.InternalName == filename)?.DisplayName ?? filename;

            string[] specialTileNames = { "clock", "compass" };

            using (AnimationEditor animationEditor = new AnimationEditor(animation, displayname, filename.ToLower().EqualsAny(specialTileNames)))
			{
				if (animationEditor.ShowDialog(this) == DialogResult.OK)
				{
					wasModified = true;
					file.SetSerializedData(animationEditor.Result, AnimationSerializer.DefaultSerializer);
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
				wasModified = true;
				UpdateRichPresence();
			}
		}

		private void HandleAudioFile(PckAsset file)
		{
			try
            {
				using AudioEditor audioEditor = new AudioEditor(file, LittleEndianCheckBox.Checked);
				wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
			}
			catch (OverflowException)
            {
				MessageBox.Show(this, $"Failed to open {file.Filename}\n" +
					"Try converting the file by using the \"Misc. Functions/Set PCK Endianness\" tool and try again.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to open {file.Filename}\n" + ex.Message,
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void HandleLocalisationFile(PckAsset file)
		{
			using LOCEditor locedit = new LOCEditor(file);
			wasModified = locedit.ShowDialog(this) == DialogResult.OK;
			UpdateRichPresence();
		}

		private void HandleColourFile(PckAsset file)
		{
			using COLEditor diag = new COLEditor(file);
			wasModified = diag.ShowDialog(this) == DialogResult.OK;
		}

		public void HandleSkinFile(PckAsset file)
		{
			if (file.HasProperty("BOX"))
			{
				using generateModel generate = new generateModel(file);
				if (generate.ShowDialog(this) == DialogResult.OK)
				{
					entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
					wasModified = true;
					ReloadMetaTreeView();
				}
				return;
			}

            Image img = file.GetTexture();
			using var skinViewer = new SkinPreview(img, file.GetProperty("ANIM", SkinANIM.FromString));
			skinViewer.ShowDialog(this);
		}

		public void HandleModelsFile(PckAsset file)
		{
			MessageBox.Show(this, "Models.bin support has not been implemented. You can use the Spark Editor for the time being to edit these files.", "Not implemented yet.");
		}

		public void HandleBehavioursFile(PckAsset file)
		{
			using BehaviourEditor edit = new BehaviourEditor(file);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}

		public void HandleMaterialFile(PckAsset file)
		{
			using MaterialsEditor edit = new MaterialsEditor(file);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}

		private void selectNode(object sender, TreeViewEventArgs e)
		{
			ReloadMetaTreeView();
			entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
			buttonEdit.Visible = false;
			previewPictureBox.Image = Resources.NoImageFound;
			viewFileInfoToolStripMenuItem.Visible = false;
			if (e.Node.TryGetTagData(out PckAsset file))
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
							Image img = file.GetTexture();

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

							if ((file.Filename.StartsWith(ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation)) ||
								 file.Filename.StartsWith(ResourceLocation.GetPathFromCategory(ResourceCategory.BlockAnimation))) &&
								file.Type == PckAssetType.TextureFile
								&& !file.IsMipmappedFile())
							{
								buttonEdit.Text = "EDIT TILE ANIMATION";
								buttonEdit.Visible = true;
							}

                            bool isTerrain = file.Filename == "res/terrain.png";
                            bool isItems = file.Filename == "res/items.png";
                            bool isParticles = file.Filename == "res/particles.png";
                            bool isMoonPhases = file.Filename == "res/terrain/moon_phases.png";
                            bool isMapIcons = file.Filename == "res/misc/mapicons.png";
                            bool isAdditionalMapIcons = file.Filename == "res/misc/additionalmapicons.png";
                            bool isXPOrbs = file.Filename == "res/item/xporb.png";
                            bool isExplosions = file.Filename == "res/misc/explosion.png";
                            bool isPaintings = file.Filename == "res/art/kz.png";
                            bool isBanners = file.Filename == "res/item/banner/Banner_Atlas.png";

                            if ((
                                isTerrain || isItems || isParticles || isMoonPhases || isPaintings ||
                                isMapIcons || isAdditionalMapIcons || isXPOrbs || isExplosions || isBanners
                                ) && file.Type == PckAssetType.TextureFile)
							{
								buttonEdit.Text = "EDIT TEXTURE ATLAS";
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

		private void extractFile(string outFilePath, PckAsset file)
        {
			File.WriteAllBytes(outFilePath, file.Data);
			if (file.PropertyCount > 0)
			{
				File.WriteAllText($"{outFilePath}.txt", file.SerializePropertiesToString());
			}
		}

		private void extractFolderFile(string outPath, PckAsset file)
        {
			TreeNode node = treeViewMain.SelectedNode;

			// abb = "Abbreviated Path"
			string abbPath = Path.GetDirectoryName(file.Filename);
			int startIndex = abbPath.IndexOf(node.Text);
			abbPath = abbPath.Substring(startIndex, abbPath.Length - startIndex);
			string finalPath = ($"{outPath}/{abbPath}/").Replace('\\', '/');

			if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);

			extractFile(finalPath + "/" + Path.GetFileName(file.Filename), file);
		}

		private void extractFolder(string outPath)
        {
			TreeNode node = treeViewMain.SelectedNode;

			string selectedFolder = node.FullPath;

			if (IsSubPCKNode(node.FullPath))
			{
				GetAllChildNodes(node.Nodes).ForEach(fileNode =>
				{
					if (fileNode.TryGetTagData(out PckAsset file))
					{
						extractFolderFile(outPath, file);
					}
				}
				);
			}
			else
			{
				foreach (PckAsset _file in currentPCK.GetFiles())
				{
					if (_file.Filename.StartsWith(selectedFolder))
					{
						extractFolderFile(outPath, _file);
					}
				};
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
			else if (node.TryGetTagData(out PckAsset file))
			{
				using SaveFileDialog exFile = new SaveFileDialog();
				exFile.FileName = Path.GetFileName(file.Filename);
				exFile.Filter = Path.GetExtension(file.Filename).Replace(".", string.Empty) + " File|*" + Path.GetExtension(file.Filename);
				if (exFile.ShowDialog(this) != DialogResult.OK ||
					// Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
					string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName)))
                {
					MessageBox.Show(this, "The chosen directory is invalid. Please choose a different one and try again.", "Node not extracted");

					return;
				}

				extractFile(exFile.FileName, file);
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
			if (treeViewMain.SelectedNode.Tag is PckAsset file)
			{
				using var ofd = new OpenFileDialog();
				// Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.

				string extra_extensions = "";

				switch (file.Type)
				{
					case PckAssetType.TextureFile:
						if (Path.GetExtension(file.Filename) == ".png")
							extra_extensions = ";*.tga";
						else if (Path.GetExtension(file.Filename) == ".tga")
							extra_extensions = ";*.png";
						break;
				}

				string fileExt = Path.GetExtension(file.Filename);

				ofd.Filter = $"{file.Type} (*{fileExt}{extra_extensions})|*{fileExt}{extra_extensions}";
				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					string newFileExt = Path.GetExtension(ofd.FileName);
					file.SetData(File.ReadAllBytes(ofd.FileName));
					file.Filename = file.Filename.Replace(fileExt, newFileExt);
					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
					wasModified = true;
					BuildMainTreeView();
				}
				return;
			}
			MessageBox.Show(this, "Can't replace a folder.");
		}

		/// <summary>
		/// Action to run before a file will be deleted
		/// </summary>
		/// <param name="file">File to remove</param>
		/// <returns>True if the remove should be canceled, otherwise False</returns>
		private bool BeforeFileRemove(PckAsset file)
		{
			string itemPath = ResourceLocation.GetPathFromCategory(ResourceCategory.ItemAnimation);

			// warn the user about deleting compass.png and clock.png
			if (file.Type == PckAssetType.TextureFile &&
				(file.Filename == itemPath + "/compass.png" || file.Filename == itemPath + "/clock.png"))
			{
				if (MessageBox.Show(this, "Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
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
            TreeNode node = treeViewMain.SelectedNode;
			if (node == null)
				return;

			string path = node.FullPath;

			if (node.TryGetTagData(out PckAsset file))
			{
				if (!BeforeFileRemove(file) && currentPCK.RemoveFile(file))
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
			RebuildSubPCK(path);
		}

		private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = treeViewMain.SelectedNode;
			if (node == null) return;
			string path = node.FullPath;

			bool isFile = node.TryGetTagData<PckAsset>(out var file);

            using TextPrompt diag = new TextPrompt(isFile ? file.Filename : Path.GetFileName(node.FullPath));

			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (isFile)
				{
					if (currentPCK.Contains(diag.NewText, file.Type))
					{
						MessageBox.Show(this, $"{diag.NewText} already exists", "File already exists");
						return;
					}
					file.Filename = diag.NewText;
				}
				else // folders
				{
					node.Text = diag.NewText;
					foreach (TreeNode childNode in GetAllChildNodes(node.Nodes))
					{
						if (childNode.Tag is PckAsset folderFile)
						{
							if (folderFile.Filename == diag.NewText) continue;
							folderFile.Filename = childNode.FullPath;
						}
					}
				}
				wasModified = true;
				RebuildSubPCK(path);
				BuildMainTreeView();
			}
		}

		private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LOCFile locFile = null;
			TryGetLocFile(out locFile);
			using AddNewSkin add = new AddNewSkin(locFile);
			if (add.ShowDialog(this) == DialogResult.OK)
			{
				if (currentPCK.HasFile("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
				{
					TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
					if (subPCK.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/");
					add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins.pck/");
					TreeNode newNode = new TreeNode(Path.GetFileName(add.SkinFile.Filename));
					newNode.Tag = add.SkinFile;
					SetNodeIcon(newNode, PckAssetType.SkinFile);
					subPCK.Nodes.Add(newNode);
					RebuildSubPCK(newNode.FullPath);
				}
				else
				{
					if (treeViewMain.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/"); // Then Skins folder
					currentPCK.AddFile(add.SkinFile);
				}
				if (add.HasCape)
				{
					if (currentPCK.HasFile("Skins.pck", PckAssetType.SkinDataFile)) // Prioritize Skins.pck
					{
						TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
						if (subPCK.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/");
						add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins.pck/");
						TreeNode newNode = new TreeNode(Path.GetFileName(add.CapeFile.Filename));
						newNode.Tag = add.CapeFile;
						SetNodeIcon(newNode, PckAssetType.SkinFile);
						subPCK.Nodes.Add(newNode);
						RebuildSubPCK(newNode.FullPath);
					}
					else
					{
						if (treeViewMain.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
						currentPCK.AddFile(add.CapeFile);
					}
				}

				TrySetLocFile(locFile);
				wasModified = true;
				BuildMainTreeView();
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

            PckAsset file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
			AudioEditor diag = new AudioEditor(file, LittleEndianCheckBox.Checked);
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				currentPCK.AddFile(file);
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

            using AnimationEditor animationEditor = new AnimationEditor(Animation.CreateEmpty(), diag.SelectedTile.DisplayName, diag.SelectedTile.InternalName.EqualsAny("clock", "compass"));
			if (animationEditor.ShowDialog() == DialogResult.OK)
			{
				wasModified = true;
				PckAsset file = currentPCK.CreateNewFile(animationFilepath, PckAssetType.TextureFile);
				file.SetSerializedData(animationEditor.Result, AnimationSerializer.DefaultSerializer);
				BuildMainTreeView();
				ReloadMetaTreeView();
			}
		}

		[Obsolete()]
		bool IsSubPCKNode(string nodePath, string extention = ".pck")
		{
			// written by miku, implemented and modified by MattNL
			if (nodePath.EndsWith(extention)) return false;

			string[] subpaths = nodePath.Split('/');

			bool isSubFile = subpaths.Any(s => Path.GetExtension(s).Equals(extention));

			Debug.WriteLineIf(isSubFile, $"{nodePath} is a Sub-PCK File", category: nameof(IsSubPCKNode));

			return isSubFile;
		}

		List<TreeNode> GetAllChildNodes(TreeNodeCollection root)
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

		[Obsolete()]
		TreeNode GetSubPCK(string childPath)
		{
			string parentPath = childPath.Replace('\\', '/');
			Debug.WriteLine(parentPath);
			string[] s = parentPath.Split('/');
			Debug.WriteLine(s.Length);
			foreach (string node in s)
			{
				TreeNode parent = treeViewMain.Nodes.Find(node, true)[0];
				if (parent.TryGetTagData(out PckAsset f) &&
					(f.Type is PckAssetType.TexturePackInfoFile ||
					 f.Type is PckAssetType.SkinDataFile))
					return parent;
			}

			return null;
		}

		[Obsolete()]
		void RebuildSubPCK(string childPath)
		{
			// Support for if a file is edited within a nested PCK File (AKA SubPCK)

			if (!IsSubPCKNode(childPath)) return;

			TreeNode parent = GetSubPCK(childPath);
			Debug.WriteLine(parent.Name);
			if (parent == null) return;

			PckAsset parent_file = parent.Tag as PckAsset;
			PckFile parent_file_pck = 
				new PckFileReader(
					LittleEndianCheckBox.Checked ? 
					OMI.Endianness.LittleEndian : 
					OMI.Endianness.BigEndian
					).FromStream(new MemoryStream(parent_file.Data));

			if (parent_file.Type is PckAssetType.TexturePackInfoFile || parent_file.Type is PckAssetType.SkinDataFile)
			{
				Debug.WriteLine("Rebuilding " + parent_file.Filename);
				PckFile newPCKFile = new PckFile(3, parent_file.Type is PckAssetType.SkinDataFile);

				bool hasSkinsFolder = false;

				// add original pck files to prevent data loss
				foreach (PckAsset _fd in parent_file_pck.GetFiles())
				{
					PckAsset new_file = newPCKFile.CreateNewFile(_fd.Filename, _fd.Type);
					// check for skins folder so files are placed consistently in final pck
					if (_fd.Filename.StartsWith("Skins/") && parent_file.Type is PckAssetType.SkinDataFile) hasSkinsFolder = true;
					foreach (var prop in _fd.GetProperties())
						new_file.AddProperty(prop);
					new_file.SetData(_fd.Data);
				}

				foreach (TreeNode node in GetAllChildNodes(parent.Nodes))
				{
					if (node.Tag is PckAsset node_file)
					{
						PckAsset new_file = newPCKFile.CreateNewFile(
							(hasSkinsFolder ? "Skins/" : String.Empty) 
							+ node_file.Filename.Replace(parent_file.Filename + "/", String.Empty), node_file.Type);
						foreach (var prop in node_file.GetProperties())
							new_file.AddProperty(prop);
						new_file.SetData(node_file.Data);
					}
				}

				parent_file.SetData(new PckFileWriter(newPCKFile, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
				parent.Tag = parent_file;

				// erase hidden sub-pck nodes to prevent duplication
				parent.Nodes.Clear();

				BuildMainTreeView();

				MessageBox.Show(this, $"Files added successfully to {parent_file.Filename}");
			}
		}

		private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file))
			{
				if (file.Size <= 0)
				{
					Trace.WriteLine($"'{file.Filename}' has no data attached.", category: nameof(treeViewMain_DoubleClick));
					return;
				}
				pckFileTypeHandler[file.Type]?.Invoke(file);
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
				treeViewMain.SelectedNode is TreeNode node && node.Tag is PckAsset file)
			{
				if (file.HasProperty(property.Key))
				{
					switch (property.Key)
					{
						case "ANIM" when file.Type == PckAssetType.SkinFile:
							try
							{
								using ANIMEditor diag = new ANIMEditor(property.Value);
								if (diag.ShowDialog(this) == DialogResult.OK)
								{
									file.SetProperty(file.GetPropertyIndex(property), new KeyValuePair<string, string>("ANIM", diag.ResultAnim.ToString()));
									RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
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

						case "BOX" when file.Type == PckAssetType.SkinFile:
							try
							{
								using BoxEditor diag = new BoxEditor(property.Value, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
								if (diag.ShowDialog(this) == DialogResult.OK)
								{
									file.SetProperty(file.GetPropertyIndex(property), new KeyValuePair<string, string>("BOX", diag.Result.ToString()));
									RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
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
							file.SetProperty(file.GetPropertyIndex(property), addProperty.Property);
							RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
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
				if (node.Tag is PckAsset file)
				{
					TreeNode newNode = new TreeNode();
					newNode.Text = Path.GetFileName(diag.NewText);
					var newFile = new PckAsset(diag.NewText, file.Type);
					foreach (var property in file.GetProperties())
					{
						newFile.AddProperty(property);
					}
					newFile.SetData(file.Data);
					newFile.Filename = diag.NewText;
					newNode.Tag = newFile;
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

					if (!IsSubPCKNode(node.FullPath)) currentPCK.InsertFile(node.Index + 1, newFile);
					else RebuildSubPCK(node.FullPath);
					BuildMainTreeView();
					wasModified = true;
				}
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
				wasModified = true;
			}
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

		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PckAsset file)
			{
				using AddPropertyPrompt addProperty = new AddPropertyPrompt();
				if (addProperty.ShowDialog(this) == DialogResult.OK)
				{
					file.AddProperty(addProperty.Property);
					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
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

			if ((node.TryGetTagData(out PckAsset file) && currentPCK.Contains(file.Filename, file.Type)) || node.Parent is TreeNode)
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
			if (targetNode is null)
				return;

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
				wasModified = true;
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
                wasModified = true;
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
            foreach (var filepath in files)
            {
                using AddFilePrompt addFile = new AddFilePrompt(Path.GetFileName(filepath));
                if (addFile.ShowDialog(this) != DialogResult.OK)
                    continue;

                if (currentPCK.Contains(addFile.Filepath, addFile.Filetype))
                {
                    MessageBox.Show(this, $"'{addFile.Filepath}' of type {addFile.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                var importedFile = currentPCK.CreateNewFile(addFile.Filepath, addFile.Filetype, () => File.ReadAllBytes(filepath));
                string propertyFile = filepath + ".txt";
				if (File.Exists(propertyFile))
				{
					importedFile.DeserializePropertiesFromString(File.ReadAllText(propertyFile));
				}
                addedCount++;

                BuildMainTreeView();
                wasModified = true;
            }
			Trace.TraceInformation("[{0}] Imported {1} file(s).", nameof(ImportFiles), addedCount);	
        }

        #endregion

        private PckFile InitializePack(int packId, int packVersion, string packName, bool createSkinsPCK)
		{
			var pack = new PckFile(3);

            PckAsset zeroFile = pack.CreateNewFile("0", PckAssetType.InfoFile);
			zeroFile.AddProperty("PACKID", packId);
			zeroFile.AddProperty("PACKVERSION", packVersion);

			var locFile = new LOCFile();
			locFile.InitializeDefault(packName);
			pack.CreateNewFile("localisation.loc", PckAssetType.LocalisationFile, new LOCFileWriter(locFile, 2));

			pack.CreateNewFileIf(createSkinsPCK, "Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
				LittleEndianCheckBox.Checked
					? OMI.Endianness.LittleEndian
					: OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeTexturePack(int packId, int packVersion, string packName, string res, bool createSkinsPCK)
		{
            PckFile pack = InitializePack(packId, packVersion, packName, createSkinsPCK);

			PckFile infoPCK = new PckFile(3);

            PckAsset icon = infoPCK.CreateNewFile("icon.png", PckAssetType.TextureFile);
			icon.SetTexture(Resources.TexturePackIcon);

            PckAsset comparison = infoPCK.CreateNewFile("comparison.png", PckAssetType.TextureFile);
			comparison.SetTexture(Resources.Comparison);

            PckAsset texturepackInfo = pack.CreateNewFile($"{res}/{res}Info.pck", PckAssetType.TexturePackInfoFile);
			texturepackInfo.AddProperty("PACKID", "0");
			texturepackInfo.AddProperty("DATAPATH", $"{res}Data.pck");

			texturepackInfo.SetData(new PckFileWriter(infoPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeMashUpPack(int packId, int packVersion, string packName, string res)
		{
            PckFile pack = InitializeTexturePack(packId, packVersion, packName, res, true);
            PckAsset gameRuleFile = pack.CreateNewFile("GameRules.grf", PckAssetType.GameRulesFile);
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

			gameRuleFile.SetData(new GameRuleFileWriter(grfFile));

			return pack;
		}

		private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CheckSaveState();
			TextPrompt namePrompt = new TextPrompt();
			namePrompt.OKButtonText = "Ok";
			if (namePrompt.ShowDialog(this) == DialogResult.OK)
			{
				currentPCK = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, true);
				isTemplateFile = true;
				wasModified = true;
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
				isTemplateFile = true;
				wasModified = true;
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
				isTemplateFile = true;
				wasModified = false;
				LoadEditorTab();
			}
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
			using CreditsForm info = new CreditsForm();
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

		public string GetDataPath()
		{
			return Path.Combine(Path.GetDirectoryName(saveLocation), "Data");
		}

		public bool HasDataFolder()
		{
			return Directory.Exists(GetDataPath());
		}

		public bool CreateDataFolder()
		{
			if (!HasDataFolder())
			{
				DialogResult result = MessageBox.Show(this, "There is not a \"Data\" folder present in the pack folder. Would you like to create one?", "Folder missing", MessageBoxButtons.YesNo);
				if (result == DialogResult.No) return false;
				else Directory.CreateDirectory(GetDataPath());
			}
			return true;
		}

		private void treeMeta_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
				deleteEntryToolStripMenuItem_Click(sender, e);
		}

		private void importExtractedSkinsFolder(object sender, EventArgs e)
		{
			using FolderBrowserDialog contents = new FolderBrowserDialog();
			if (contents.ShowDialog(this) == DialogResult.OK && Directory.Exists(contents.SelectedPath))
			{
				string filepath = treeViewMain.SelectedNode?.FullPath ?? "";
				if (treeViewMain.SelectedNode is not null && treeViewMain.SelectedNode.IsTagOfType<PckAsset>())
					filepath = treeViewMain.SelectedNode.Parent?.FullPath ?? "";

                foreach (var fullfilename in Directory.GetFiles(contents.SelectedPath, "dlc*.png"))
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

					PckAsset newFile = currentPCK.CreateNewFile(pckfilepath, pckfiletype);
					byte[] filedata = File.ReadAllBytes(fullfilename);
					newFile.SetData(filedata);

					if (File.Exists(fullfilename + ".txt"))
					{
						string propertiesFileContent = File.ReadAllText(fullfilename + ".txt");
						newFile.DeserializePropertiesFromString(propertiesFileContent);
					}
				}
				BuildMainTreeView();
				wasModified = true;
			}
		}

		private bool TryGetLocFile(out LOCFile locFile)
		{
			if (!currentPCK.TryGetFile("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locdata) &&
				!currentPCK.TryGetFile("languages.loc", PckAssetType.LocalisationFile, out locdata))
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
				Debug.WriteLine(ex.Message);
			}
			locFile = null;
			return false;
		}

		private bool TrySetLocFile(in LOCFile locFile)
		{
			if (!currentPCK.TryGetFile("localisation.loc", PckAssetType.LocalisationFile, out PckAsset locdata) &&
				!currentPCK.TryGetFile("languages.loc", PckAssetType.LocalisationFile, out locdata))
			{
				return false;
			}

			try
			{
				locdata.SetData(new LOCFileWriter(locFile, 2));
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return false;
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

                    PckAsset importSkinAsset = currentPCK.CreateNewFile(skinNameImport, PckAssetType.SkinFile);
					importSkinAsset.SetData(data);
					string propertyFile = contents.FileName + ".txt";
					if (File.Exists(propertyFile))
					{
						string txtProperties = File.ReadAllText(propertyFile);
						importSkinAsset.DeserializePropertiesFromString(txtProperties);
						
						// Because extracting/exporting an assest doesn't export
						// the actual loc value we just get an undefined loc key
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
				MessageBox.Show(this, "Save PCK?", "Unsaved PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
			foreach (string file in files)
			{
                string ext = Path.GetExtension(file);
				if (ext.Equals(".pck", StringComparison.CurrentCultureIgnoreCase))
					e.Effect = DragDropEffects.Copy;
				return;
			}
		}

		private void OpenPck_DragDrop(object sender, DragEventArgs e)
		{
			string[] Filepaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (Filepaths.Length > 1)
				MessageBox.Show(this, "Only one pck file at a time is currently supported");
			LoadPckFromFile(Filepaths[0]);
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
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
					break;
				case PckAssetType.ColourTableFile:
					node.ImageIndex = 6;
					node.SelectedImageIndex = 6;
					break;
				case PckAssetType.ModelsFile:
                    goto default;
					node.ImageIndex = 8;
					node.SelectedImageIndex = 8;
					break;
				case PckAssetType.SkinDataFile:
					goto default;
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

		private void SetFileType(PckAssetType type)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file))
			{
				Debug.WriteLine($"Setting {file.Type} to {type}");
				file.Type = type;
				SetNodeIcon(treeViewMain.SelectedNode, type);
				RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
			}
		}

		[Obsolete()]
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
                    PckAsset file = currentPCK.CreateNewFile(renamePrompt.NewText, PckAssetType.TextureFile, () => File.ReadAllBytes(fileDialog.FileName));
					BuildMainTreeView();
					wasModified = true;
				}
			}
		}

		private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckAsset file)
			{
				MessageBox.Show(this,
					"File path: " + file.Filename +
					"\nAssigned File type: " + (int)file.Type + " (" + file.Type + ")" +
					"\nFile size: " + file.Size +
					"\nProperties count: " + file.PropertyCount
					, Path.GetFileName(file.Filename) + " file info");
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
						if (currentPCK.HasFile(mippedPath, PckAssetType.TextureFile))
							currentPCK.RemoveFile(currentPCK.GetFile(mippedPath, PckAssetType.TextureFile));
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

						MipMappedFile.SetTexture(mippedTexture);

						currentPCK.InsertFile(currentPCK.IndexOfFile(file) + i - 1, MipMappedFile);
					}
					BuildMainTreeView();
				}
			}
		}

		private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("colours.col", PckAssetType.ColourTableFile, out _))
			{
				MessageBox.Show(this, "A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
            PckAsset newColorFile = currentPCK.CreateNewFile("colours.col", PckAssetType.ColourTableFile);
			newColorFile.SetData(Resources.tu69colours);
			BuildMainTreeView();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			e.Cancel = !isSelectingTab;
		}

		private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file) &&
				file.Type == PckAssetType.SkinFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "3DS Texture|*.3dst";
				saveFileDialog.DefaultExt = ".3dst";
				if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					Image img = file.GetTexture();
					var writer = new _3DSTextureWriter(img);
					writer.WriteToFile(saveFileDialog.FileName);
				}
			}
		}

		private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file))
			{
				using (var input = new MultiTextPrompt())
				{
					if (input.ShowDialog(this) == DialogResult.OK)
					{
						foreach (string line in input.TextOutput)
						{
							int idx = line.IndexOf(' ');
							if (idx == -1 || line.Length - 1 == idx)
								continue;
							file.AddProperty(line.Substring(0, idx), line.Substring(idx + 1));
						}
						ReloadMetaTreeView();
						RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
						wasModified = true;
					}
				}
			}
		}

		private void correctSkinDecimalsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file) &&
				file.Type == PckAssetType.SkinFile)
			{
				foreach (KeyValuePair<string, string> p in file.GetProperties().ToList())
				{
					if (p.Key == "BOX" || p.Key == "OFFSET")
						file.SetProperty(file.GetPropertyIndex(p), new KeyValuePair<string, string>(p.Key, p.Value.Replace(',', '.')));
				}
				ReloadMetaTreeView();
				RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
				wasModified = true;
			}
		}

		private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("Skins.pck", PckAssetType.SkinDataFile, out _))
			{
				MessageBox.Show(this, "A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
					LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));

			BuildMainTreeView();

			TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
			TreeNode folderNode = CreateNode("Skins");
			folderNode.ImageIndex = 0;
			folderNode.SelectedImageIndex = 0;
			skinsNode.Nodes.Add(folderNode);
		}

		private void editAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file))
			{
                string[] props = file.GetProperties().Select(p => p.Key + " " + p.Value).ToArray();
				using (var input = new MultiTextPrompt(props))
				{
					if (input.ShowDialog(this) == DialogResult.OK)
					{
						file.ClearProperties();
						foreach (string line in input.TextOutput)
						{
							int idx = line.IndexOf(' ');
							if (idx == -1 || line.Length - 1 == idx)
								continue;
							file.AddProperty(line.Substring(0, idx).Replace(":", string.Empty), line.Substring(idx + 1));
						}
						ReloadMetaTreeView();
						RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
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
					PckAsset file = currentPCK.CreateNewFile(diag.Filepath, diag.Filetype, () => File.ReadAllBytes(ofd.FileName));

					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);

					BuildMainTreeView();
					wasModified = true;
				}
			}
			return;
		}

		private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("behaviours.bin", PckAssetType.BehavioursFile, out _))
			{
				MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("behaviours.bin", PckAssetType.BehavioursFile, BehaviourResources.BehaviourFileInitializer);
			BuildMainTreeView();
		}

		private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("entityMaterials.bin", PckAssetType.MaterialFile, out _))
			{
				MessageBox.Show(this, "A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			currentPCK.CreateNewFile("entityMaterials.bin", PckAssetType.MaterialFile, MaterialResources.MaterialsFileInitializer);
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

				if(dialog.ShowDialog(this) == DialogResult.OK)
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
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckAsset file)
			{
				using BoxEditor diag = new BoxEditor(SkinBOX.Empty, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					file.AddProperty("BOX", diag.Result);
					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
					ReloadMetaTreeView();
					wasModified = true;
				}
				return;
			}
		}

		private void addANIMEntryToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset file))
			{
				using ANIMEditor diag = new ANIMEditor(SkinANIM.Empty);
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					file.AddProperty("ANIM", diag.ResultAnim);
					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
					ReloadMetaTreeView();
					wasModified = true;
				}
				return;
			}
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Program.Updater.IsUpdateAvailable(Application.ProductVersion))
			{
				Program.UpdateToLatest("Would you like to download it?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DialogResult.Yes);
				return;
			}
			MessageBox.Show(this, "Already up to date.", "No update available");
		}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

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
					wasModified = true;
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
					wasModified = true;
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
            if (treeViewMain.SelectedNode.TryGetTagData(out PckAsset asset))
			{
				replaceToolStripMenuItem.Visible = true;
                cloneFileToolStripMenuItem.Visible = true;
				setFileTypeToolStripMenuItem.Visible = true;
				miscFunctionsToolStripMenuItem.Visible = true;

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
				miscFunctionsToolStripMenuItem.Visible = false;
			}
        }
    }
}