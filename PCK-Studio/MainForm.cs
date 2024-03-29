using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Drawing.Imaging;
using OMI.Formats.Archive;
using OMI.Formats.Pck;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Workers.Archive;
using OMI.Workers.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using PckStudio.Properties;
using PckStudio.FileFormats;
using PckStudio.Forms;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Classes.Misc;
using PckStudio.IO.PckAudio;
using PckStudio.IO._3DST;
using PckStudio.Internal;
using PckStudio.Features;
using PckStudio.Extensions;
using PckStudio.Popups;
using PckStudio.Classes.Utils;
using PckStudio.Helper;
using PCKStudio_Updater;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		private PckManager PckManager = null;
		string saveLocation = string.Empty;
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

		readonly Dictionary<PckFileType, Action<PckFileData>> pckFileTypeHandler;

		public MainForm()
		{
			InitializeComponent();

			skinToolStripMenuItem1.Click += (sender, e) => SetFileType(PckFileType.SkinFile);
			capeToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.CapeFile);
			textureToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.TextureFile);
			languagesFileLOCToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.LocalisationFile);
			gameRulesFileGRFToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.GameRulesFile);
			audioPCKFileToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.AudioFile);
			coloursCOLFileToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.ColourTableFile);
			gameRulesHeaderGRHToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.GameRulesHeader);
			skinsPCKToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.SkinDataFile);
			modelsFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.ModelsFile);
			behavioursFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.BehavioursFile);
			entityMaterialsFileBINToolStripMenuItem.Click += (sender, e) => SetFileType(PckFileType.MaterialFile);

			treeViewMain.TreeViewNodeSorter = new PckNodeSorter();

			pckOpen.AllowDrop = true;

			Text = Application.ProductName;

			labelVersion.Text = $"{Application.ProductName}: {Application.ProductVersion}";
			ChangelogRichTextBox.Text = Resources.CHANGELOG;

			pckFileTypeHandler = new Dictionary<PckFileType, Action<PckFileData>>(15)
			{
				[PckFileType.SkinFile] = HandleSkinFile,
				[PckFileType.CapeFile] = null,
				[PckFileType.TextureFile] = HandleTextureFile,
				[PckFileType.UIDataFile] = _ => throw new NotSupportedException("unused in-game"),
				[PckFileType.InfoFile] = null,
				[PckFileType.TexturePackInfoFile] = HandleInnerPckFile,
                [PckFileType.LocalisationFile] = HandleLocalisationFile,
				[PckFileType.GameRulesFile] = HandleGameRuleFile,
				[PckFileType.AudioFile] = HandleAudioFile,
				[PckFileType.ColourTableFile] = HandleColourFile,
				[PckFileType.GameRulesHeader] = HandleGameRuleFile,
				[PckFileType.SkinDataFile] = HandleInnerPckFile,
                [PckFileType.ModelsFile] = null, //HandleModelsFile, // Note: Uncomment when implemented
				[PckFileType.BehavioursFile] = HandleBehavioursFile,
				[PckFileType.MaterialFile] = HandleMaterialFile,
			};
		}

        private void HandleInnerPckFile(PckFileData file)
        {
			// TODO: decide on how to handle embedded pck files
			return;
			if (Settings.Default.LoadSubPcks &&
				(file.Filetype == PckFileType.SkinDataFile || file.Filetype == PckFileType.TexturePackInfoFile) &&
				file.Size > 0 && treeViewMain.SelectedNode.Nodes.Count == 0)
			{
				using (var stream = new MemoryStream(file.Data))
				{
					try
					{
						var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
						PckFile subPCKfile = reader.FromStream(stream);
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
			checkSaveState();
			treeViewMain.Nodes.Clear();
			currentPCK = openPck(filepath);
			if (currentPCK == null)
			{
				MessageBox.Show(string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error");
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

			isSelectingTab = true;
			tabControl.SelectTab(0);
			isSelectingTab = false;

			UpdateRichPresence();

			if (saveLocation != String.Empty) LoadPckFromFile(saveLocation);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			PckManager?.Close();
			checkSaveState();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckFileExists = true;
				ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					LoadPckFromFile(ofd.FileName);
				}
			}
		}

		private PckFile openPck(string filePath)
		{
			isTemplateFile = false;
			saveLocation = filePath;
			var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
			try
			{
				PckFile pck = reader.FromFile(filePath);
				return pck;
			}
			catch (OverflowException ex)
			{
				MessageBox.Show("Failed to open pck\n" +
					$"Try {(LittleEndianCheckBox.Checked ? "unchecking" : "checking")} the 'Open/Save as Switch/Vita/PS4 pck' check box in the upper right corner.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Debug.WriteLine(ex.Message);
			}
			catch
			{
				MessageBox.Show("Failed to open pck. There's two common reasons for this:\n" +
					"1. The file is audio/music cues PCK file. Please use the specialized editor while inside of a pck file.\n" +
					"2. We're aware of an issue where a pck file might fail to load because it contains multiple entries with the same path.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}

		private void CheckForPasswordAndRemove()
		{
			if (currentPCK.TryGetFile("0", PckFileType.InfoFile, out PckFileData file))
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
			bool alreadyExists = root.ContainsKey(nodeText);
			TreeNode subNode = alreadyExists ? root[nodeText] : CreateNode(nodeText);
			if (!alreadyExists) root.Add(subNode);
			return BuildNodeTreeBySeperator(subNode.Nodes, subPath, seperator);
		}

		private void BuildPckTreeView(TreeNodeCollection root, PckFile pckFile)
		{
			foreach (PckFileData file in pckFile.GetFiles())
			{
				// fix any file paths that may be incorrect
				//if (file.Filename.StartsWith(parentPath))
				//	file.Filename = file.Filename.Remove(0, parentPath.Length);
				TreeNode node = BuildNodeTreeBySeperator(root, file.Filename, '/');
				node.Tag = file;
				SetNodeIcon(node, file.Filetype);
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

			if (isTemplateFile && currentPCK.HasFile("Skins.pck", PckFileType.SkinDataFile))
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

		private void HandleTextureFile(PckFileData file)
		{
			_ = file.IsMipmappedFile() && currentPCK.TryGetValue(file.GetNormalPath(), PckFileType.TextureFile, out file);

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
				if (viewer.ShowDialog() == DialogResult.OK)
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

					file.SetData(texture, ImageFormat.Png);
					wasModified = true;
					BuildMainTreeView();
				}
				return;
			}

			if (!file.Filename.StartsWith("res/textures/blocks/") && !file.Filename.StartsWith("res/textures/items/"))
				return;
            Animation animation = AnimationHelper.GetAnimationFromFile(file);
			using (AnimationEditor animationEditor = new AnimationEditor(animation, Path.GetFileNameWithoutExtension(file.Filename)))
			{
				if (animationEditor.ShowDialog(this) == DialogResult.OK)
				{
					wasModified = true;
					file.Filename = animationEditor.FinalPath;
					AnimationHelper.SaveAnimationToFile(file, animation);
					BuildMainTreeView();
				}
			}
		}

		private void HandleGameRuleFile(PckFileData file)
		{
			using GameRuleFileEditor grfEditor = new GameRuleFileEditor(file);
			wasModified = grfEditor.ShowDialog(this) == DialogResult.OK;
			UpdateRichPresence();
		}

		private void HandleAudioFile(PckFileData file)
		{
			using AudioEditor audioEditor = new AudioEditor(file, LittleEndianCheckBox.Checked);
			wasModified = audioEditor.ShowDialog(this) == DialogResult.OK;
		}

		private void HandleLocalisationFile(PckFileData file)
		{
			using LOCEditor locedit = new LOCEditor(file);
			wasModified = locedit.ShowDialog(this) == DialogResult.OK;
			UpdateRichPresence();
		}

		private void HandleColourFile(PckFileData file)
		{
			using COLEditor diag = new COLEditor(file);
			wasModified = diag.ShowDialog(this) == DialogResult.OK;
		}

		public void HandleSkinFile(PckFileData file)
		{
			var skin = file.GetSkin();
            if (file.HasProperty("CAPEPATH"))
			{
				string capePath = file.GetProperty("CAPEPATH");
				if (currentPCK.TryGetFile(capePath, PckFileType.CapeFile, out var cape))
				{
					skin.CapeTexture = cape.GetTexture();
				}
			}

			using CustomSkinEditor skinEditor = new CustomSkinEditor(skin, currentPCK.HasVerionString);
			if (skinEditor.ShowDialog() == DialogResult.OK)
			{
				if (!TryGetLocFile(out var locFile))
					Debug.Fail("Failed to aquire loc file.");
				file.SetSkin(skinEditor.ResultSkin, locFile);

				entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
				wasModified = true;
				ReloadMetaTreeView(); 
			}
        }

		public void HandleModelsFile(PckFileData file)
		{
			MessageBox.Show("Models.bin support has not been implemented. You can use the Spark Editor for the time being to edit these files.", "Not implemented yet.");
		}

		public void HandleBehavioursFile(PckFileData file)
		{
			using BehaviourEditor edit = new BehaviourEditor(file);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}

		public void HandleMaterialFile(PckFileData file)
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
			if (e.Node.TryGetTagData(out PckFileData file))
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

				switch (file.Filetype)
				{
					case PckFileType.SkinFile:
					case PckFileType.CapeFile:
					case PckFileType.TextureFile:
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

							if ((file.Filename.StartsWith("res/textures/blocks/") || file.Filename.StartsWith("res/textures/items/")) &&
								file.Filetype == PckFileType.TextureFile
								&& !file.IsMipmappedFile())
							{
								buttonEdit.Text = "EDIT TILE ANIMATION";
								buttonEdit.Visible = true;
							}
						}
						break;

					case PckFileType.LocalisationFile:
						buttonEdit.Text = "EDIT LOC";
						buttonEdit.Visible = true;
						break;

					case PckFileType.AudioFile:
						buttonEdit.Text = "EDIT MUSIC CUES";
						buttonEdit.Visible = true;
						break;

					case PckFileType.ColourTableFile when file.Filename == "colours.col":
						buttonEdit.Text = "EDIT COLORS";
						buttonEdit.Visible = true;
						break;

					case PckFileType.BehavioursFile when file.Filename == "behaviours.bin":
						buttonEdit.Text = "EDIT BEHAVIOURS";
						buttonEdit.Visible = true;
						break;
					default:
						buttonEdit.Visible = false;
						break;
				}
			}
		}

		private void extractFile(string outFilePath, PckFileData file)
        {
			File.WriteAllBytes(outFilePath, file.Data);
			if (file.PropertyCount > 0)
			{
				using var fs = File.CreateText($"{outFilePath}.txt");
				foreach (KeyValuePair<string, string> property in file.GetProperties())
				{
					fs.WriteLine($"{property.Key}: {property.Value}");
                }
			}
		}

		private void extractFolderFile(string outPath, PckFileData file)
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
					if (fileNode.TryGetTagData(out PckFileData file))
					{
						extractFolderFile(outPath, file);
					}
				}
				);
			}
			else
			{
				foreach (PckFileData _file in currentPCK.GetFiles())
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

				if (dialog.ShowDialog(Handle) == true) extractFolder(dialog.ResultPath);
			}
			else if (node.TryGetTagData(out PckFileData file))
			{
				using SaveFileDialog exFile = new SaveFileDialog();
				exFile.FileName = Path.GetFileName(file.Filename);
				exFile.Filter = Path.GetExtension(file.Filename).Replace(".", string.Empty) + " File|*" + Path.GetExtension(file.Filename);
				if (exFile.ShowDialog() != DialogResult.OK ||
					// Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
					string.IsNullOrWhiteSpace(Path.GetDirectoryName(exFile.FileName)))
                {
					MessageBox.Show(this, "The chosen directory is invalid. Please choose a different one and try again.", "Node not extracted");

					return;
				}

				extractFile(exFile.FileName, file);
			}

			// Verification that file extraction path was successful
			MessageBox.Show($"\"{node.Text}\" successfully extracted");
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
				pckFileLabel.Text = Path.GetFileName(saveLocation);
				isTemplateFile = false;
			}
		}

		private void Save(string filePath)
		{
			var writer = new PckFileWriter(currentPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
			writer.WriteToFile(filePath);
			wasModified = false;
			MessageBox.Show("Saved Pck file", "File Saved");
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckFileData file)
			{
				using var ofd = new OpenFileDialog();
				// Suddenly, and randomly, this started throwing an exception because it wasn't formatted correctly? So now it's formatted correctly and now displays the file type name in the dialog.

				string extra_extensions = "";

				switch (file.Filetype)
				{
					case PckFileType.TextureFile:
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
					wasModified = true;
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
		private bool BeforeFileRemove(PckFileData file)
		{
			string itemPath = "res/textures/items/";

			// warn the user about deleting compass.png and clock.png
			if (file.Filetype == PckFileType.TextureFile &&
				(file.Filename == itemPath + "compass.png" || file.Filename == itemPath + "clock.png"))
			{
				if (MessageBox.Show("Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return true;
			}

			// remove loc key if its a skin/cape
			if (file.Filetype == PckFileType.SkinFile || file.Filetype == PckFileType.CapeFile)
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

			if (node.TryGetTagData(out PckFileData file))
			{
				if (!BeforeFileRemove(file) && currentPCK.RemoveFile(file))
				{
					node.Remove();
					wasModified = true;
				}
			}
			else if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning",
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

			using TextPrompt diag = new TextPrompt(node.Tag is null ? Path.GetFileName(node.FullPath) : node.FullPath);

			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (node.Tag is PckFileData file)
				{
					if (currentPCK.TryGetFile(diag.NewText, file.Filetype, out _))
					{
						MessageBox.Show($"{diag.NewText} already exists", "File already exists");
						return;
					}
					file.Filename = diag.NewText;
				}
				else // folders
				{
					node.Text = diag.NewText;
					foreach (TreeNode childNode in GetAllChildNodes(node.Nodes))
					{
						if (childNode.Tag is PckFileData folderFile)
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
            using (AddNewSkin addNewSkinDialog = new AddNewSkin())
				if (addNewSkinDialog.ShowDialog() == DialogResult.OK)
				{
					TryGetLocFile(out LOCFile locFile);
					var skinFile = addNewSkinDialog.NewSkin.CreateFile(locFile);
					currentPCK.AddFile(skinFile);
                    if (currentPCK.HasFile("Skins.pck", PckFileType.SkinDataFile)) // Prioritize Skins.pck
					{
						TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
						if (subPCK.Nodes.ContainsKey("Skins"))
							skinFile.Filename = skinFile.Filename.Insert(0, "Skins/");
						skinFile.Filename = skinFile.Filename.Insert(0, "Skins.pck/");
						TreeNode newNode = new TreeNode(Path.GetFileName(skinFile.Filename));
						newNode.Tag = skinFile;
						SetNodeIcon(newNode, PckFileType.SkinFile);
						subPCK.Nodes.Add(newNode);
						RebuildSubPCK(newNode.FullPath);
					}
					else
					{
						if (treeViewMain.Nodes.ContainsKey("Skins"))
							skinFile.Filename = skinFile.Filename.Insert(0, "Skins/"); // Then Skins folder
						currentPCK.AddFile(skinFile);
					}

					if (addNewSkinDialog.NewSkin.HasCape)
					{
						var capeFile = addNewSkinDialog.NewSkin.CreateCapeFile();
                        if (currentPCK.HasFile("Skins.pck", PckFileType.SkinDataFile)) // Prioritize Skins.pck
						{
							TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
							if (subPCK.Nodes.ContainsKey("Skins"))
								capeFile.Filename = capeFile.Filename.Insert(0, "Skins/");
							capeFile.Filename = capeFile.Filename.Insert(0, "Skins.pck/");
							TreeNode newNode = new TreeNode(Path.GetFileName(capeFile.Filename));
							newNode.Tag = capeFile;
							SetNodeIcon(newNode, PckFileType.SkinFile);
							subPCK.Nodes.Add(newNode);
							RebuildSubPCK(newNode.FullPath);
						}
						else
						{
							if (treeViewMain.Nodes.ContainsKey("Skins"))
								capeFile.Filename = capeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
							currentPCK.AddFile(capeFile);
						}
					}

				TrySetLocFile(locFile);
				wasModified = true;
				BuildMainTreeView();
			}
		}

		private static PckFileData CreateNewAudioFile(bool isLittle)
		{
			PckAudioFile audioPck = new PckAudioFile();
			audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Overworld);
			audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Nether);
			audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.End);
			PckFileData pckFileData = new PckFileData("audio.pck", PckFileType.AudioFile);
			pckFileData.SetData(new PckAudioFileWriter(audioPck, isLittle ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
			return pckFileData;
		}

		private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.Contains(PckFileType.AudioFile))
			{
				// the chance of this happening is really really slim but just in case
				MessageBox.Show("There is already an audio file in this PCK!", "Can't create audio.pck");
				return;
			}
			if (string.IsNullOrEmpty(saveLocation))
			{
				MessageBox.Show("You must save your pck before creating or opening a music cues PCK file", "Can't create audio.pck");
				return;
			}

            PckFileData file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
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

			if (currentPCK.Contains($"res/textures/{Animation.GetCategoryName(diag.Category)}/{diag.SelectedTile}.png", PckFileType.TextureFile))
			{
				MessageBox.Show($"{diag.SelectedTile} is already present.", "File already present");
				return;
			}

			var file = new PckFileData(
				$"res/textures/{Animation.GetCategoryName(diag.Category)}/{diag.SelectedTile}.png",
				PckFileType.TextureFile);

            Animation animation = AnimationHelper.GetAnimationFromFile(file);

			using AnimationEditor animationEditor = new AnimationEditor(animation, diag.SelectedTile);
			if (animationEditor.ShowDialog() == DialogResult.OK)
			{
				wasModified = true;
				AnimationHelper.SaveAnimationToFile(file, animation);
				currentPCK.AddFile(file);
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
				if (parent.TryGetTagData(out PckFileData f) &&
					(f.Filetype is PckFileType.TexturePackInfoFile ||
					 f.Filetype is PckFileType.SkinDataFile))
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

			PckFileData parent_file = parent.Tag as PckFileData;
			if (parent_file.Filetype is PckFileType.TexturePackInfoFile || parent_file.Filetype is PckFileType.SkinDataFile)
			{
				Debug.WriteLine("Rebuilding " + parent_file.Filename);
				PckFile newPCKFile = new PckFile(3, parent_file.Filetype is PckFileType.SkinDataFile);

				foreach (TreeNode node in GetAllChildNodes(parent.Nodes))
				{
					if (node.Tag is PckFileData node_file)
					{
						PckFileData new_file = newPCKFile.CreateNewFile(node_file.Filename.Replace(parent_file.Filename + "/", String.Empty), node_file.Filetype);
						foreach (var prop in node_file.GetProperties())
							new_file.AddProperty(prop);
						new_file.SetData(node_file.Data);
					}
				}

				parent_file.SetData(new PckFileWriter(newPCKFile, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
				parent.Tag = parent_file;

				BuildMainTreeView();
			}
		}

		private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file))
			{
				if (file.Size <= 0)
				{
					Trace.WriteLine($"'{file.Filename}' has no data attached.", category: nameof(treeViewMain_DoubleClick));
					return;
				}
				pckFileTypeHandler[file.Filetype]?.Invoke(file);
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
				treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFileData file)
			{
				if (file.HasProperty(property.Key))
				{
					switch (property.Key)
					{
						case "ANIM" when file.Filetype == PckFileType.SkinFile:
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
								MessageBox.Show("Failed to parse ANIM value, aborting to normal functionality. Please make sure the value only includes hexadecimal characters (0-9,A-F) and has no more than 8 characters.");
							}
							break;

						case "BOX" when file.Filetype == PckFileType.SkinFile:
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
			if (node == null || !node.IsTagOfType<PckFileData>())
				return;
			string path = node.FullPath;

			using TextPrompt diag = new TextPrompt(node.Tag is null ? Path.GetFileName(node.FullPath) : node.FullPath);
			diag.contextLabel.Text = $"Creating a clone of \"{path}\". Ensure that the path isn't yet.";
			diag.OKButtonText = "Clone";

			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (node.Tag is PckFileData file)
				{
					TreeNode newNode = new TreeNode();
					newNode.Text = Path.GetFileName(diag.NewText);
					var newFile = new PckFileData(diag.NewText, file.Filetype);
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
				treeViewMain.SelectedNode is TreeNode main && main.Tag is PckFileData file &&
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
				node.Tag is PckFileData file)
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
				t.Tag is PckFileData file)
			{
				using AddPropertyPrompt addProperty = new AddPropertyPrompt();
				if (addProperty.ShowDialog() == DialogResult.OK)
				{
					file.AddProperty(addProperty.Property);
					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
					ReloadMetaTreeView();
					wasModified = true;
				}
			}
		}

		#region drag and drop for main tree node

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

		#endregion

		private PckFile InitializePack(int packId, int packVersion, string packName, bool createSkinsPCK)
		{
			var pack = new PckFile(3);

            PckFileData zeroFile = pack.CreateNewFile("0", PckFileType.InfoFile);
			zeroFile.AddProperty("PACKID", packId);
			zeroFile.AddProperty("PACKVERSION", packVersion);

			var locFile = new LOCFile();
			locFile.InitializeDefault(packName);
			pack.CreateNewFile("localisation.loc", PckFileType.LocalisationFile, new LOCFileWriter(locFile, 2));

			pack.CreateNewFileIf(createSkinsPCK, "Skins.pck", PckFileType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
				LittleEndianCheckBox.Checked
					? OMI.Endianness.LittleEndian
					: OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeTexturePack(int packId, int packVersion, string packName, string res, bool createSkinsPCK)
		{
            PckFile pack = InitializePack(packId, packVersion, packName, createSkinsPCK);

			PckFile infoPCK = new PckFile(3);

            PckFileData icon = infoPCK.CreateNewFile("icon.png", PckFileType.TextureFile);
			icon.SetData(Resources.TexturePackIcon, ImageFormat.Png);

            PckFileData comparison = infoPCK.CreateNewFile("comparison.png", PckFileType.TextureFile);
			comparison.SetData(Resources.Comparison, ImageFormat.Png);

            PckFileData texturepackInfo = pack.CreateNewFile($"{res}/{res}Info.pck", PckFileType.TexturePackInfoFile);
			texturepackInfo.AddProperty("PACKID", "0");
			texturepackInfo.AddProperty("DATAPATH", $"{res}Data.pck");

			texturepackInfo.SetData(new PckFileWriter(infoPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));

			return pack;
		}

		private PckFile InitializeMashUpPack(int packId, int packVersion, string packName, string res)
		{
            PckFile pack = InitializeTexturePack(packId, packVersion, packName, res, true);
            PckFileData gameRuleFile = pack.CreateNewFile("GameRules.grf", PckFileType.GameRulesFile);
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
			checkSaveState();
			TextPrompt namePrompt = new TextPrompt();
			namePrompt.OKButtonText = "Ok";
			if (namePrompt.ShowDialog() == DialogResult.OK)
			{
				currentPCK = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, true);
				isTemplateFile = true;
				wasModified = true;
				LoadEditorTab();
			}
		}

		private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkSaveState();
			CreateTexturePackPrompt packPrompt = new CreateTexturePackPrompt();
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
				currentPCK = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes, packPrompt.CreateSkinsPck);
				isTemplateFile = true;
				wasModified = true;
				LoadEditorTab();
			}
		}

		private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkSaveState();
			CreateTexturePackPrompt packPrompt = new CreateTexturePackPrompt();
			if (packPrompt.ShowDialog() == DialogResult.OK)
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
			if (advanced.ShowDialog() == DialogResult.OK)
			{
				wasModified = true;
				BuildMainTreeView();
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkSaveState();
			CloseEditorTab();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using CreditsForm info = new CreditsForm();
			info.ShowDialog();
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
					PckFile pckfile = null;
					using (FileStream fs = File.OpenRead(ofd.FileName))
					{
						try
						{
							var reader = new PckFileReader(LittleEndianCheckBox.Checked
								? OMI.Endianness.LittleEndian
								: OMI.Endianness.BigEndian);
							pckfile = reader.FromStream(fs);
						}
						catch (OverflowException ex)
						{
							Debug.WriteLine(ex.Message);
							Trace.WriteLine("Failed to open " + ofd.FileName);
							MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Switch/Vita/PS4 pck' check box in the upper right corner.",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
					foreach (PckFileData file in pckfile.GetFiles())
					{
						string filepath = $"{sfd.SelectedPath}/{file.Filename}";
						FileInfo fileinfo = new FileInfo(filepath);
						fileinfo.Directory.Create();
						File.WriteAllBytes(filepath, file.Data); // writes data to file
																 //attempts to generate reimportable metadata file out of minefiles metadata
						string metaData = "";

						foreach (KeyValuePair<string, string> entry in file.GetProperties())
						{
							metaData += $"{entry.Key}: {entry.Value}{Environment.NewLine}";
						}

						File.WriteAllText(sfd.SelectedPath + @"\" + file.Filename + ".txt", metaData);
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show("An Error occured while extracting data");
			}
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
				DialogResult result = MessageBox.Show("There is not a \"Data\" folder present in the pack folder. Would you like to create one?", "Folder missing", MessageBoxButtons.YesNo);
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

		#region imports a folder of skins to pck
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
				bool hasSkinsPck = currentPCK.HasFile("Skins.pck", PckFileType.SkinDataFile);

				foreach (var fullfilename in Directory.GetFiles(contents.SelectedPath, "*.png"))
				{
					string filename = Path.GetFileNameWithoutExtension(fullfilename);
					// sets file type based on wether its a cape or skin
					PckFileType pckfiletype = filename.StartsWith("dlccape", StringComparison.OrdinalIgnoreCase)
						? PckFileType.CapeFile
						: PckFileType.SkinFile;
					string pckfilepath = (hasSkinsPck ? "Skins/" : string.Empty) + filename + ".png";


					PckFileData newFile = new PckFileData(pckfilepath, pckfiletype);
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
						var skinsfile = currentPCK.GetFile("Skins.pck", PckFileType.SkinDataFile);
						using (var ms = new MemoryStream(skinsfile.Data))
						{
							var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
							var skinspck = reader.FromStream(ms);
							skinspck.AddFile(newFile);
							ms.Position = 0;
							var writer = new PckFileWriter(skinspck, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
							writer.WriteToStream(ms);
							skinsfile.SetData(ms.ToArray());
						}
						continue;
					}
					currentPCK.AddFile(newFile);
				}
				BuildMainTreeView();
				wasModified = true;
			}
		}
		#endregion

		private bool TryGetLocFile(out LOCFile locFile)
		{
			if (!currentPCK.TryGetFile("localisation.loc", PckFileType.LocalisationFile, out PckFileData locdata) &&
				!currentPCK.TryGetFile("languages.loc", PckFileType.LocalisationFile, out locdata))
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
			if (!currentPCK.TryGetFile("localisation.loc", PckFileType.LocalisationFile, out PckFileData locdata) &&
				!currentPCK.TryGetFile("languages.loc", PckFileType.LocalisationFile, out locdata))
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

				if (contents.ShowDialog() == DialogResult.OK)
				{
					string skinNameImport = Path.GetFileName(contents.FileName);
					byte[] data = File.ReadAllBytes(contents.FileName);
					PckFileData mfNew = currentPCK.CreateNewFile(skinNameImport, PckFileType.SkinFile);
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
							wasModified = true;
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
					if (node.Tag is PckFileData fd &&
						(fd.Filetype != PckFileType.TexturePackInfoFile &&
						fd.Filetype != PckFileType.SkinDataFile))
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

		private void checkSaveState()
		{
			if (currentPCK is not null &&
				wasModified &&
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
				MessageBox.Show("Only one pck file at a time is currently supported");
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

		private void SetNodeIcon(TreeNode node, PckFileType type)
		{
            switch (type)
			{
				case PckFileType.AudioFile:
					node.ImageIndex = 1;
					node.SelectedImageIndex = 1;
					break;
				case PckFileType.LocalisationFile:
					node.ImageIndex = 3;
					node.SelectedImageIndex = 3;
					break;
				case PckFileType.TexturePackInfoFile:
                    goto default;
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
					break;
				case PckFileType.ColourTableFile:
					node.ImageIndex = 6;
					node.SelectedImageIndex = 6;
					break;
				case PckFileType.ModelsFile:
                    goto default;
					node.ImageIndex = 8;
					node.SelectedImageIndex = 8;
					break;
				case PckFileType.SkinDataFile:
					goto default;
					node.ImageIndex = 7;
					node.SelectedImageIndex = 7;
					break;
				case PckFileType.GameRulesFile:
					node.ImageIndex = 9;
					node.SelectedImageIndex = 9;
					break;
				case PckFileType.GameRulesHeader:
					node.ImageIndex = 10;
					node.SelectedImageIndex = 10;
					break;
				case PckFileType.InfoFile:
					node.ImageIndex = 11;
					node.SelectedImageIndex = 11;
					break;
				case PckFileType.SkinFile:
					node.ImageIndex = 12;
					node.SelectedImageIndex = 12;
					break;
				case PckFileType.CapeFile:
					node.ImageIndex = 13;
					node.SelectedImageIndex = 13;
					break;
				case PckFileType.TextureFile:
					node.ImageIndex = 14;
					node.SelectedImageIndex = 14;
					break;
				case PckFileType.BehavioursFile:
					node.ImageIndex = 15;
					node.SelectedImageIndex = 15;
					break;
				case PckFileType.MaterialFile:
					node.ImageIndex = 16;
					node.SelectedImageIndex = 16;
					break;
				default: // unknown file format
					node.ImageIndex = 5;
					node.SelectedImageIndex = 5;
					break;
			}
		}

		private void SetFileType(PckFileType type)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file))
			{
				Debug.WriteLine($"Setting {file.Filetype} to {type}");
				file.Filetype = type;
				SetNodeIcon(treeViewMain.SelectedNode, type);
				RebuildSubPCK(treeViewMain.SelectedNode.FullPath);
			}
		}

		[Obsolete()]
		private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Texture File(*.png,*.tga)|*.png;*.tga";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				using TextPrompt renamePrompt = new TextPrompt(Path.GetFileName(fileDialog.FileName));
				renamePrompt.LabelText = "Path";
				if (renamePrompt.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(renamePrompt.NewText))
				{
                    if (currentPCK.Contains(renamePrompt.NewText, PckFileType.TextureFile))
                    {
                        MessageBox.Show($"'{renamePrompt.NewText}' already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    PckFileData file = currentPCK.CreateNewFile(renamePrompt.NewText, PckFileType.TextureFile, () => File.ReadAllBytes(fileDialog.FileName));
					BuildMainTreeView();
					wasModified = true;
				}
			}
		}

		private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckFileData file)
			{
				MessageBox.Show(
					"File path: " + file.Filename +
					"\nAssigned File type: " + (int)file.Filetype + " (" + file.Filetype + ")" +
					"\nFile size: " + file.Size +
					"\nProperties count: " + file.PropertyCount
					, Path.GetFileName(file.Filename) + " file info");
			}
		}

		private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckFileData file && file.Filetype == PckFileType.TextureFile)
			{
				string textureDirectory = Path.GetDirectoryName(file.Filename);
				string textureName = Path.GetFileNameWithoutExtension(file.Filename);

				if (file.IsMipmappedFile())
					return;

				string textureExtension = Path.GetExtension(file.Filename);

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
						if (currentPCK.HasFile(mippedPath, PckFileType.TextureFile))
							currentPCK.RemoveFile(currentPCK.GetFile(mippedPath, PckFileType.TextureFile));
						PckFileData MipMappedFile = new PckFileData(mippedPath, PckFileType.TextureFile);


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

						MipMappedFile.SetData(mippedTexture, ImageFormat.Png);

						currentPCK.InsertFile(currentPCK.IndexOfFile(file) + i - 1, MipMappedFile);
					}
					BuildMainTreeView();
				}
			}
		}

		private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("colours.col", PckFileType.ColourTableFile, out _))
			{
				MessageBox.Show("A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
            PckFileData newColorFile = currentPCK.CreateNewFile("colours.col", PckFileType.ColourTableFile);
			newColorFile.SetData(Resources.tu69colours);
			BuildMainTreeView();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			e.Cancel = !isSelectingTab;
		}

		private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file) &&
				file.Filetype == PckFileType.SkinFile)
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

		private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file))
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
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file) &&
				file.Filetype == PckFileType.SkinFile)
			{
				foreach (KeyValuePair<string, string> p in file.GetProperties())
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
			if (currentPCK.TryGetFile("Skins.pck", PckFileType.SkinDataFile, out _))
			{
				MessageBox.Show("A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("Skins.pck", PckFileType.SkinDataFile, new PckFileWriter(new PckFile(3, true),
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
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file))
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

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				using AddFilePrompt diag = new AddFilePrompt("res/" + Path.GetFileName(ofd.FileName));
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					if (currentPCK.Contains(diag.Filepath, diag.Filetype))
					{
						MessageBox.Show($"'{diag.Filepath}' of type {diag.Filetype} already exists.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
					PckFileData file = currentPCK.CreateNewFile(diag.Filepath, diag.Filetype, () => File.ReadAllBytes(ofd.FileName));

					RebuildSubPCK(treeViewMain.SelectedNode.FullPath);

					BuildMainTreeView();
					wasModified = true;
				}
			}
			return;
		}

		private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("behaviours.bin", PckFileType.BehavioursFile, out _))
			{
				MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("behaviours.bin", PckFileType.BehavioursFile, BehaviourResources.BehaviourFileInitializer);
			BuildMainTreeView();
		}

		private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("entityMaterials.bin", PckFileType.MaterialFile, out _))
			{
				MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			currentPCK.CreateNewFile("entityMaterials.bin", PckFileType.MaterialFile, MaterialResources.MaterialsFileInitializer);
			BuildMainTreeView();
		}

		private void trelloBoardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://trello.com/b/0XLNOEbe/pck-studio");
		}

		private void openPckManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckManager ??= new PckManager();
			PckManager.FormClosing += (s, e) =>
			{
				PckManager.Hide();
				e.Cancel = true;
			};
			if (!PckManager.Visible)
			{
				PckManager.Show();
				PckManager.BringToFront();
			}
			if (PckManager.Focus())
				PckManager.BringToFront();
		}

		private void wavBinkaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "WAV files (*.wav)|*.wav",
				Title = "Please choose WAV files to convert to BINKA"
			};
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				BinkaConverter.ToBinka(fileDialog.FileNames, new DirectoryInfo(Path.GetDirectoryName(fileDialog.FileName)));
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
			if (fileDialog.ShowDialog() == DialogResult.OK)
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
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFileData file)
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
			if (treeViewMain.SelectedNode.TryGetTagData(out PckFileData file))
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
			MessageBox.Show("Already up to date.", "No update available");
		}

		[Obsolete] // the move functions are to eventually be removed in favor of drag and drop
		private void moveFile(int amount)
		{
			if (treeViewMain.SelectedNode is not TreeNode t || t.Tag is not PckFileData)
				return;

            PckFileData file = t.Tag as PckFileData;
            string path = t.FullPath;

			// skin and cape files only
			if (!(file.Filetype == PckFileType.SkinFile || file.Filetype == PckFileType.CapeFile)) return;

			PckFile pck = currentPCK;
			bool IsSubPCK = IsSubPCKNode(path);
			if (IsSubPCK)
			{
				using (var stream = new MemoryStream((GetSubPCK(path).Tag as PckFileData).Data))
				{
					var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
					pck = reader.FromStream(stream);
				}
			}

			int index = pck.IndexOfFile(file);

			if (index + amount < 0 || index + amount > pck.FileCount) return;
			pck.RemoveFile(file);
			pck.InsertFile(index + amount, file);

			if (IsSubPCK)
			{
				using (var stream = new MemoryStream())
				{
					var writer = new PckFileWriter(pck, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
					writer.WriteToStream(stream);
					(GetSubPCK(path).Tag as PckFileData).SetData(stream.ToArray());
				}
			}
			BuildMainTreeView();
			wasModified = true;
		}
		[Obsolete]
		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e) => moveFile(-1);
		[Obsolete]
		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e) => moveFile(1);
	}
}