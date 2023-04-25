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
using OMI.Workers.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;
using PckStudio.Classes.Utils.ARC;
using PckStudio.Forms;
using PckStudio.Forms.Utilities;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Classes.Misc;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.IO._3DST;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		string saveLocation = string.Empty;
		PckFile currentPCK = null;
		bool wasModified = false;
		bool isTemplateFile = false;

		bool needsUpdate = false;
		bool isSelectingTab = false;

		readonly Dictionary<PckFile.FileData.FileType, Action<PckFile.FileData>> pckFileTypeHandler;

		public MainForm()
		{
			InitializeComponent();
			treeViewMain.TreeViewNodeSorter = new PckNodeSorter();
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
			pckOpen.AllowDrop = true;

            isSelectingTab = true;
			tabControl.SelectTab(0);
            isSelectingTab = false;

			labelVersion.Text = "PCK Studio: " + Application.ProductVersion;
			ChangelogRichTextBox.Text = Resources.CHANGELOG;
#if DEBUG
			labelVersion.Text += $" (Debug build: {Program.Info.BuildVersion}@{Program.Info.LastCommitHash})";
#endif

            pckFileTypeHandler = new Dictionary<PckFile.FileData.FileType, Action<PckFile.FileData>>(15)
			{
				[PckFile.FileData.FileType.SkinFile]            = HandleSkinFile,
				[PckFile.FileData.FileType.CapeFile]            = null,
				[PckFile.FileData.FileType.TextureFile]         = HandleTextureFile,
				[PckFile.FileData.FileType.UIDataFile]          = _ => throw new NotSupportedException("unused in-game"),
				[PckFile.FileData.FileType.InfoFile]            = null,
				[PckFile.FileData.FileType.TexturePackInfoFile] = null,
				[PckFile.FileData.FileType.LocalisationFile]    = HandleLocalisationFile,
				[PckFile.FileData.FileType.GameRulesFile]       = HandleGameRuleFile,
				[PckFile.FileData.FileType.AudioFile]           = HandleAudioFile,
				[PckFile.FileData.FileType.ColourTableFile]     = HandleColourFile,
				[PckFile.FileData.FileType.GameRulesHeader]     = HandleGameRuleFile,
				[PckFile.FileData.FileType.SkinDataFile]        = null,
				[PckFile.FileData.FileType.ModelsFile]          = HandleModelsFile,
				[PckFile.FileData.FileType.BehavioursFile]      = HandleBehavioursFile,
				[PckFile.FileData.FileType.MaterialFile]        = HandleMaterialFile,
			};
		}

		public void LoadPck(string filepath)
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

		private void Form1_Load(object sender, EventArgs e)
		{
			RPC.Initialize();
			UpdateRPC();

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
					LoadPck(ofd.FileName);
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
					$"Try {(LittleEndianCheckBox.Checked ? "unchecking" : "checking")} the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Debug.WriteLine(ex.Message);
			}
			catch
			{
				MessageBox.Show("Failed to open pck\n" +
					"If this is an Audio/Music Cues pck, please use the specialized editor while inside of the parent pck.",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}

		private void CheckForPasswordAndRemove()
		{
			if (currentPCK.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out PckFile.FileData file))
			{
				file.Properties.RemoveAll(t => t.property.Equals("LOCK"));
			}
		}

		private void LoadEditorTab()
		{
			fileEntryCountLabel.Text = "Files:" + currentPCK.Files.Count;
			if (isTemplateFile)
				pckFileLabel.Text = "Unsaved File!";
			else
				pckFileLabel.Text = "Current PCK File: " + Path.GetFileName(saveLocation);
            treeViewMain.Enabled = treeMeta.Enabled = true;
			closeToolStripMenuItem.Visible = true;
			saveToolStripMenuItem.Enabled = true;
			saveToolStripMenuItem1.Enabled = true;
			advancedMetaAddingToolStripMenuItem.Enabled = true;
			convertToBedrockToolStripMenuItem.Enabled = true;
			addCustomPackImageToolStripMenuItem.Enabled = true;
			BuildMainTreeView();
			isSelectingTab = true;
			tabControl.SelectTab(1);
			isSelectingTab = false;
			UpdateRPC();
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
			pictureBoxImagePreview.Image = Resources.NoImageFound;
			treeViewMain.Nodes.Clear();
			treeMeta.Nodes.Clear();
			treeViewMain.Enabled = false;
			treeMeta.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem1.Enabled = false;
			advancedMetaAddingToolStripMenuItem.Enabled = false;
			closeToolStripMenuItem.Visible = false;
			convertToBedrockToolStripMenuItem.Enabled = false;
            addCustomPackImageToolStripMenuItem.Enabled = false;
            fileEntryCountLabel.Text = string.Empty;
            pckFileLabel.Text = string.Empty;
            UpdateRPC();

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
				switch (file.Filetype)
				{
					case PckFile.FileData.FileType.SkinDataFile:
					case PckFile.FileData.FileType.TexturePackInfoFile:
						using (var stream = new MemoryStream(file.Data))
						{
							try
							{
								var writer = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                                PckFile subPCKfile = writer.FromStream(stream);
								// passes parent path to remove from sub pck filepaths
								BuildPckTreeView(node.Nodes, subPCKfile, file.Filename + "/");
							}
							catch (OverflowException ex)
							{
								MessageBox.Show("Failed to open pck\n" +
									"Try checking the 'Open/Save as Vita/PS4 pck' checkbox in the upper right corner.",
									"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								Debug.WriteLine(ex.Message);
							}
						}
						break;
					default: // unknown file format
						//throw new InvalidDataException(nameof(file.filetype));
						break;
				}
				SetPckFileIcon(node, file.Filetype);
			};
		}

		private void BuildMainTreeView()
		{
			// In case the Rename function was just used and the selected node name no longer matches the file name
			string filepath = string.Empty;
			if(treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFile.FileData file)
				filepath = file.Filename;
			treeViewMain.Nodes.Clear();
			BuildPckTreeView(treeViewMain.Nodes, currentPCK);

			if (isTemplateFile && currentPCK.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile))
			{
				TreeNode skinsNode = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
				TreeNode folderNode = CreateNode("Skins");
				folderNode.ImageIndex = 0;
				folderNode.SelectedImageIndex = 0;
				if (!skinsNode.Nodes.ContainsKey("Skins"))
					skinsNode.Nodes.Add(folderNode);
			}

			if (!string.IsNullOrEmpty(filepath))
			{
				// Looks kinda nuts but this line of code is responsible for finding the correct node that was originally selected
				treeViewMain.SelectedNode =
					treeViewMain.Nodes.Find(Path.GetFileName(filepath), true).ToList()
					.Find(t  => (t.Tag as PckFile.FileData).Filename == filepath);
			}
	    }

		bool IsFilePathMipMapped(string filepath)
		{
			// We only want to test the file name itself. ex: "terrainMipMapLevel2"
			string name = Path.GetFileNameWithoutExtension(filepath);
			// check if last character is a digit (0-9). If not return false
			if (!char.IsDigit(name[name.Length - 1])) return false;
			// If string does not end with MipMapLevel, then it's not MipMapped
			if (!name.Remove(name.Length - 1, 1).EndsWith("MipMapLevel")) return false;
			return true;
		}

		private void HandleTextureFile(PckFile.FileData file)
		{
			if (IsFilePathMipMapped(file.Filename) &&
				currentPCK.Files.Find(pckfile =>
					// todo write cleaner ?
					pckfile.Filename.Equals(
						file.Filename.Remove(file.Filename.Length - 12 - Path.GetExtension(file.Filename).Length)
						+ Path.GetExtension(file.Filename)))
				is PckFile.FileData originalAnimationFile)
			{
				file = originalAnimationFile;
			}

			if (!(file.Filename.StartsWith("res/textures/blocks/") || file.Filename.StartsWith("res/textures/items/"))) return;

			using (AnimationEditor animationEditor = new AnimationEditor(file))
			{
				if (animationEditor.ShowDialog(this) == DialogResult.OK)
				{
					file.Filename = animationEditor.TileName;
					BuildMainTreeView();
				}
			}
		}

		private void HandleGameRuleFile(PckFile.FileData file)
		{
			using GameRuleFileEditor grfEditor = new GameRuleFileEditor(file);
			wasModified = grfEditor.ShowDialog(this) == DialogResult.OK;
			UpdateRPC();
		}

		private void UpdateRPC()
		{
			if (currentPCK == null)
			{
				RPC.SetPresence("An Open Source .PCK File Editor");
				return;
			};

			if (TryGetLocFile(out LOCFile locfile) &&
					locfile.HasLocEntry("IDS_DISPLAY_NAME") &&
					locfile.Languages.Contains("en-EN"))
			{
				RPC.SetPresence($"Editing a Pack: {locfile.GetLocEntry("IDS_DISPLAY_NAME", "en-EN")}");
				return;
			}
			// default
			RPC.SetPresence("An Open Source .PCK File Editor");
		}

		private void HandleAudioFile(PckFile.FileData file)
		{
            if (!TryGetLocFile(out LOCFile locFile))
                throw new Exception("No .loc File found.");
            using AudioEditor audioEditor = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
			if (audioEditor.ShowDialog(this) == DialogResult.OK)
			{
				wasModified = true;
				TrySetLocFile(locFile);
			}
        }

		private void HandleLocalisationFile(PckFile.FileData file)
		{
            using LOCEditor locedit = new LOCEditor(file);
			wasModified = locedit.ShowDialog(this) == DialogResult.OK;
			UpdateRPC();
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
			wasModified = diag.ShowDialog(this) == DialogResult.OK;
        }

		public void HandleSkinFile(PckFile.FileData file)
		{
			if (file.Properties.HasProperty("BOX"))
			{
				using (generateModel generate = new generateModel(file.Properties, new PictureBox()))
					if (generate.ShowDialog() == DialogResult.OK)
					{
						entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
						wasModified = true;
						ReloadMetaTreeView();
					}
			}
			else
			{
				using (var ms = new MemoryStream(file.Data))
				{
					var texture = Image.FromStream(ms);
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
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}
		public void HandleMaterialFile(PckFile.FileData file)
		{
			using MaterialsEditor edit = new MaterialsEditor(file);
			wasModified = edit.ShowDialog(this) == DialogResult.OK;
		}
		private void selectNode(object sender, TreeViewEventArgs e)
		{
			ReloadMetaTreeView();
			entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
			buttonEdit.Visible = false;
			pictureBoxImagePreview.Image = Resources.NoImageFound;
			var node = e.Node;
			viewFileInfoToolStripMenuItem.Visible = false;
			if (node is TreeNode t && t.Tag is PckFile.FileData file)
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
						// TODO: Add tga support
						if (Path.GetExtension(file.Filename) == ".tga") break;
						using (MemoryStream stream = new MemoryStream(file.Data))
						{
							try
							{
								pictureBoxImagePreview.Image = Image.FromStream(stream);
								labelImageSize.Text = $"{pictureBoxImagePreview.Image.Size.Width}x{pictureBoxImagePreview.Image.Size.Height}";
							}
							catch (Exception ex)
							{
								labelImageSize.Text = "";
								pictureBoxImagePreview.Image = Resources.NoImageFound;
								Debug.WriteLine("Not a supported image format. Setting back to default");
								Debug.WriteLine(string.Format("An error occured of type: {0} with message: {1}", ex.GetType(), ex.Message), "Exception");
							}
						}

						if ((file.Filename.StartsWith("res/textures/blocks/") || file.Filename.StartsWith("res/textures/items/")) &&
							!file.Filename.EndsWith("clock.png") && !file.Filename.EndsWith("compass.png") &&
							file.Filetype == PckFile.FileData.FileType.TextureFile 
							&& !IsFilePathMipMapped(file.Filename))
						{
							buttonEdit.Text = "EDIT TEXTURE ANIMATION";
							buttonEdit.Visible = true;
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
					file.Properties.ForEach(property => fs.WriteLine($"{property.Item1}: {property.Item2}"));
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
					if(IsSubPCKNode(node.FullPath) && node.Tag == null)
					{
						GetAllChildNodes(node.Nodes).ForEach(fileNode =>
						{
							if(fileNode.Tag is PckFile.FileData file)
							{
								Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(file.Filename)}");
								File.WriteAllBytes($"{dialog.SelectedPath}/{file.Filename}", file.Data);
								if (file.Properties.Count > 0)
								{
									using var fs = File.CreateText($"{dialog.SelectedPath}/{file.Filename}.txt");
									file.Properties.ForEach(property => fs.WriteLine($"{property.Item1}: {property.Item2}"));
								}
							}
						}
						);
					}
					else
					{
						currentPCK.Files.ForEach(file =>
						{
							if (file.Filename.StartsWith(selectedFolder))
							{
								Directory.CreateDirectory($"{dialog.SelectedPath}/{Path.GetDirectoryName(file.Filename)}");
								File.WriteAllBytes($"{dialog.SelectedPath}/{file.Filename}", file.Data);
								if (file.Properties.Count > 0)
								{
									using var fs = File.CreateText($"{dialog.SelectedPath}/{file.Filename}.txt");
									file.Properties.ForEach(property => fs.WriteLine($"{property.Item1}: {property.Item2}"));
								}
							}
						});
					}
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

		private void Save(string filePath)
		{
			bool isSkinsPCK = false;
			if (!currentPCK.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out PckFile.FileData _))
			{
				switch(MessageBox.Show(this, "The info file, \"0\", was not detected. Would you like to save as a Skins.pck archive?", "Save as Skins archive?", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						isSkinsPCK = true;
						break;
					case DialogResult.No:
						isSkinsPCK = false;
						break;
					case DialogResult.Cancel:
					default:
						return; // Cancel operation
				}
			}
			currentPCK.HasVerionString = isSkinsPCK;

			var writer = new PckFileWriter(currentPCK, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
			writer.WriteToFile(filePath);
			wasModified = false;
			MessageBox.Show("Saved Pck file", "File Saved");
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
					if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath)) RebuildSubPCK(treeViewMain.SelectedNode);
					wasModified = true;
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
					if(MessageBox.Show("Are you sure want to delete this file? If \"compass.png\" or \"clock.png\" are missing, your game will crash upon loading this pack.", "Warning",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
				}

                // remove loc key if its a skin/cape
				if (file.Filetype == PckFile.FileData.FileType.SkinFile || file.Filetype == PckFile.FileData.FileType.CapeFile)
                {
                    if (TryGetLocFile(out LOCFile locFile))
                    {
                        foreach (var property in file.Properties)
                        {
                            if (property.Item1 == "THEMENAMEID" || property.Item1 == "DISPLAYNAMEID")
                                locFile.RemoveLocKey(property.Item2);
                        }
                        TrySetLocFile(locFile);
                    }
                }
                currentPCK.Files.Remove(file);
                node.Remove();
                wasModified = true;
            }
            else if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string pckFolderDir = node.FullPath;
                currentPCK.Files.RemoveAll(file => file.Filename.StartsWith(pckFolderDir));
                node.Remove();
                wasModified = true;
            }
			if (IsSubPCKNode(path)) RebuildSubPCK(node);
        }

		private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = treeViewMain.SelectedNode;
			if (node == null) return;
			string path = node.FullPath;
			using RenamePrompt diag = new RenamePrompt(node.FullPath);
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (node.Tag is PckFile.FileData file 
					&& file.Filetype is not PckFile.FileData.FileType.TexturePackInfoFile 
					&& file.Filetype is not PckFile.FileData.FileType.SkinDataFile)
				{
					file.Filename = diag.NewText;
				}
				else if(!IsSubPCKNode(path)) // folder
				{
					foreach (var childNode in GetAllChildNodes(node.Nodes))
					{
						if (childNode.Tag is PckFile.FileData folderFile &&
							childNode.FullPath.StartsWith(childNode.FullPath))
						{
							folderFile.Filename = diag.NewText + childNode.FullPath.Substring(childNode.FullPath.Length);
						}
					}
				}
				else
				{
					currentPCK.Files.ForEach(file =>
					{
						if (file.Filename.StartsWith(node.FullPath))
							file.Filename = diag.NewText + file.Filename.Substring(node.FullPath.Length);
					});
				}
				wasModified = true;
				if (IsSubPCKNode(path)) RebuildSubPCK(node);
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

					if (currentPCK.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile)) // Prioritize Skins.pck
					{
						TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
						if (subPCK.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/");
						add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins.pck/");
						TreeNode newNode = new TreeNode(Path.GetFileName(add.SkinFile.Filename));
						newNode.Tag = add.SkinFile;
						SetPckFileIcon(newNode, PckFile.FileData.FileType.SkinFile);
						subPCK.Nodes.Add(newNode);
						RebuildSubPCK(newNode);
					}
					else
					{
						if (treeViewMain.Nodes.ContainsKey("Skins")) add.SkinFile.Filename = add.SkinFile.Filename.Insert(0, "Skins/"); // Then Skins folder
						currentPCK.Files.Add(add.SkinFile);
					}
					if (add.HasCape)
					{
						if (currentPCK.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile)) // Prioritize Skins.pck
						{
							TreeNode subPCK = treeViewMain.Nodes.Find("Skins.pck", false).FirstOrDefault();
							if (subPCK.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/");
							add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins.pck/");
							TreeNode newNode = new TreeNode(Path.GetFileName(add.CapeFile.Filename));
							newNode.Tag = add.CapeFile;
							SetPckFileIcon(newNode, PckFile.FileData.FileType.SkinFile);
							subPCK.Nodes.Add(newNode);
							RebuildSubPCK(newNode);
						}
						else
						{
							if (treeViewMain.Nodes.ContainsKey("Skins")) add.CapeFile.Filename = add.CapeFile.Filename.Insert(0, "Skins/"); // Then Skins folder
							currentPCK.Files.Add(add.CapeFile);
						}
					}

					TrySetLocFile(locFile);
					wasModified = true;
					BuildMainTreeView();
				}
		}

        private PckFile.FileData CreateNewAudioFile(bool isLittle)
        {
            // create actual valid pck file structure
            PckAudioFile audioPck = new PckAudioFile();
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Overworld);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.Nether);
            audioPck.AddCategory(PckAudioFile.AudioCategory.EAudioType.End);
			PckFile.FileData pckFileData = currentPCK.CreateNewFile("audio.pck", PckFile.FileData.FileType.AudioFile, () =>
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

		private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.Files.FindIndex(file => file.Filename.Contains("audio.pck")) != -1)
			{
				MessageBox.Show("There is already an audio.pck present in this file!", "Can't create audio.pck");
				return;
			}
			if (!TryGetLocFile(out LOCFile locFile))
				throw new Exception("No .loc file found.");
			var file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
			AudioEditor diag = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
			if (diag.ShowDialog(this) == DialogResult.OK)
				TrySetLocFile(locFile);
			diag.Dispose();
			BuildMainTreeView();
		}

		private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using var ofd = new OpenFileDialog()
			{
				Filter = "PNG Files | *.png",
				Title = "Select a PNG File",
			};
			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			using ChangeTile diag = new ChangeTile();
			if (diag.ShowDialog(this) != DialogResult.OK)
				return;

			using Image img = new Bitmap(ofd.FileName);
			var file = currentPCK.CreateNewFile(
				$"res/textures/{AnimationResources.GetAnimationSection(diag.IsItem)}/{diag.SelectedTile}.png",
				PckFile.FileData.FileType.TextureFile,
				() =>
				{
					using var stream = new MemoryStream();
					img.Save(stream, ImageFormat.Png);
					return stream.ToArray();
				});
			file.Properties.Add(("ANIM", string.Empty));

			using AnimationEditor animationEditor = new AnimationEditor(file);
			if (animationEditor.ShowDialog() == DialogResult.OK)
			{
				file.Filename = animationEditor.TileName;
				ReloadMetaTreeView();
				BuildMainTreeView();
				wasModified = true;
			}
		}

		bool IsSubPCKNode(string nodePath, string extention = ".pck")
		{
			// written by miku, implemented and modified by me - MNL
			if (nodePath.EndsWith(extention)) return false;

			string[] subpaths = nodePath.Split('/');
			var conditions = subpaths.Select(s => Path.GetExtension(s).Equals(extention));

			bool isSubFile = conditions.Contains(true);

			if(isSubFile) Console.WriteLine($"{nodePath} is a Sub-PCK File");

			return isSubFile;
		}

		List<TreeNode> GetAllChildNodes(TreeNodeCollection root)
		{
			List<TreeNode> childNodes = new List<TreeNode>();
			foreach(TreeNode node in root)
			{
				childNodes.Add(node);
				if (node.Nodes.Count > 0)
				{
					childNodes.AddRange(GetAllChildNodes(node.Nodes));
				}
			}
			return childNodes;
		}

		TreeNode GetSubPCK(TreeNode child)
		{
			TreeNode parent = child;
			while (parent.Parent != null)
			{
				parent = parent.Parent;
				Console.WriteLine(parent.Text);
                if (parent.Tag is PckFile.FileData f &&
                    (f.Filetype is PckFile.FileData.FileType.TexturePackInfoFile ||
					 f.Filetype is PckFile.FileData.FileType.SkinDataFile))
					return parent;
			}
			return null;
		}

		void RebuildSubPCK(TreeNode childNode)
		{
			// Support for if a file is edited within a PCK File

			TreeNode parent = GetSubPCK(childNode);
			if (parent == null) return;

			PckFile.FileData parent_file = parent.Tag as PckFile.FileData;
			if (parent_file.Filetype is PckFile.FileData.FileType.TexturePackInfoFile || parent_file.Filetype is PckFile.FileData.FileType.SkinDataFile)
			{
				Console.WriteLine("Rebuilding " + parent_file.Filename);
				PckFile newPCKFile = new PckFile(3)
				{
					HasVerionString = parent_file.Filetype is PckFile.FileData.FileType.SkinDataFile
				};

				foreach (TreeNode node in GetAllChildNodes(parent.Nodes))
				{
					if (node.Tag is PckFile.FileData node_file)
					{
						PckFile.FileData new_file = newPCKFile.CreateNewFile(node_file.Filename, node_file.Filetype);
						foreach (var prop in node_file.Properties) new_file.Properties.Add(prop);
						new_file.SetData(node_file.Data);
					}
				}

				using (MemoryStream ms = new MemoryStream())
				{
					var writer = new PckFileWriter(newPCKFile, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
					writer.WriteToStream(ms);
					parent_file.SetData(ms.ToArray());
					parent.Tag = parent_file;
				}

				BuildMainTreeView();
			}
		}

		private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
			{
				pckFileTypeHandler[file.Filetype]?.Invoke(file);
			}
		}

		private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node is TreeNode t && t.Tag is ValueTuple<string, string> property)
			{
				entryTypeTextBox.Text = property.Item1;
				entryDataTextBox.Text = property.Item2;
			}
		}

		private void treeMeta_DoubleClick(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode is TreeNode subnode && subnode.Tag is ValueTuple<string, string> property &&
				treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFile.FileData file)
			{
				int i = file.Properties.IndexOf(property);
				if (i != -1)
				{
					switch (property.Item1)
					{
						case "ANIM" when file.Filetype == PckFile.FileData.FileType.SkinFile:
							try
							{
								using ANIMEditor diag = new ANIMEditor(property.Item2);
								if (diag.ShowDialog(this) == DialogResult.OK)
								{
									file.Properties[i] = ("ANIM", diag.ResultAnim.ToString());
									if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath))
										RebuildSubPCK(treeViewMain.SelectedNode);
									ReloadMetaTreeView();
									wasModified = true;
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
								using BoxEditor diag = new BoxEditor(property.Item2, IsSubPCKNode(treeViewMain.SelectedNode.FullPath));
								if (diag.ShowDialog(this) == DialogResult.OK)
								{
									file.Properties[i] = ("BOX", diag.Result);
									if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath))
										RebuildSubPCK(treeViewMain.SelectedNode);
									ReloadMetaTreeView();
									wasModified = true;
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

					using (addMeta addDialog = new addMeta(property.Item1, property.Item2))
					{
						if (addDialog.ShowDialog() == DialogResult.OK)
						{
							file.Properties[i] = (addDialog.PropertyName, addDialog.PropertyValue);
							if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath))
								RebuildSubPCK(treeViewMain.SelectedNode);
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
			PckFile.FileData mfO = node.Tag as PckFile.FileData;

			// Creates new empty file entry
			PckFile.FileData mf = new PckFile.FileData(string.Empty, mfO.Filetype);
			mf.SetData(mfO.Data);
			string dirName = Path.GetDirectoryName(mfO.Filename);

			int clone_number = 0;
			string nameWithoutExt = Path.GetFileNameWithoutExtension(mfO.Filename);
			string newFileName = mfO.Filename;
			do
			{
				clone_number++;
				string clone_str = "_clone" + clone_number.ToString();
				bool isClone = nameWithoutExt.Contains("_clone");
				if (isClone) newFileName = nameWithoutExt.Remove(nameWithoutExt.Length - 7) + clone_str + Path.GetExtension(mfO.Filename);
				else newFileName = nameWithoutExt + clone_str + Path.GetExtension(mfO.Filename);
			}
			while (currentPCK.HasFile(dirName + (string.IsNullOrEmpty(dirName) ? "" : "/") + newFileName, mf.Filetype));

			mf.Filename = dirName + (string.IsNullOrEmpty(dirName) ? "" : "/") + newFileName; //sets minfile name to file name
			foreach (var entry in mfO.Properties)
			{
				var property = (ValueTuple<string, string>)entry;
				mf.Properties.Add(property);
			}

            TreeNode newNode = new TreeNode();
            newNode.Text = newFileName;
            newNode.Tag = mf;
            newNode.ImageIndex = node.ImageIndex;
            newNode.SelectedImageIndex = node.SelectedImageIndex;

            if (node.Parent == null) treeViewMain.Nodes.Insert(node.Index + 1, newNode); //adds generated minefile node
            else node.Parent.Nodes.Insert(node.Index + 1, newNode);//adds generated minefile node to selected folder

			if (!IsSubPCKNode(node.FullPath)) currentPCK.Files.Insert(node.Index + 1, mf);
			else RebuildSubPCK(node);
        }

		private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode is TreeNode t && t.Tag is ValueTuple<string, string> property &&
				treeViewMain.SelectedNode is TreeNode main && main.Tag is PckFile.FileData file &&
				file.Properties.Remove(property))
			{
				treeMeta.SelectedNode.Remove();
				if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath)) RebuildSubPCK(treeViewMain.SelectedNode);
				wasModified = true;
			}
		}

		private void ReloadMetaTreeView()
		{
			treeMeta.Nodes.Clear();
			if (treeViewMain.SelectedNode is TreeNode node &&
				node.Tag is PckFile.FileData file)
			{
				foreach (var property in file.Properties)
				{
					treeMeta.Nodes.Add(CreateNode(property.Item1, property));
		}
			}
		}

		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PckFile.FileData file)
			{
				using addMeta add = new addMeta();
				if (add.ShowDialog() == DialogResult.OK)
				{
					file.Properties.Add((add.PropertyName, add.PropertyValue));
					if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath)) RebuildSubPCK(treeViewMain.SelectedNode);
					ReloadMetaTreeView();
					wasModified = true;
				}
			}
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

		private PckFile InitializePack(int packId, int packVersion, string packName, bool createSkinsPCK)
		{
			var newPck = new PckFile(3);
			
			var zeroFile = newPck.CreateNewFile("0", PckFile.FileData.FileType.InfoFile);
			zeroFile.Properties.Add(("PACKID", packId.ToString()));
			zeroFile.Properties.Add(("PACKVERSION", packVersion.ToString()));

			var loc = newPck.CreateNewFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, () =>
			{
				var locFile = new LOCFile();
				locFile.InitializeDefault(packName);
				using var stream = new MemoryStream();
				var writer = new LOCFileWriter(locFile, 2);
                writer.WriteToStream(stream);
				return stream.ToArray();
            });

			if (createSkinsPCK)
			{
                PckFile.FileData skinsPCKFile = newPck.CreateNewFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile, () =>
				{
					using var stream = new MemoryStream();
					var writer = new PckFileWriter(new PckFile(3)
					{
						HasVerionString = true
					},
					LittleEndianCheckBox.Checked
						? OMI.Endianness.LittleEndian
						: OMI.Endianness.BigEndian);
					writer.WriteToStream(stream);
                    return stream.ToArray();
                });
            }
			return newPck;
		}

		private PckFile InitializeTexturePack(int packId, int packVersion, string packName, string res, bool createSkinsPCK = false)
		{
            var newPck = InitializePack(packId, packVersion, packName, createSkinsPCK);
			var texturepackInfo = newPck.CreateNewFile($"{res}/{res}Info.pck", PckFile.FileData.FileType.TexturePackInfoFile,
				() =>
				{
                    using var ms = new MemoryStream();
                    var writer = new PckFileWriter(new PckFile(3),
                        LittleEndianCheckBox.Checked
                            ? OMI.Endianness.LittleEndian
                            : OMI.Endianness.BigEndian);
                    writer.WriteToStream(ms);
                    return ms.ToArray();
                });
			texturepackInfo.Properties.Add(("PACKID", "0"));
			texturepackInfo.Properties.Add(("DATAPATH", $"{res}Data.pck"));

			PckFile infoPCK = new PckFile(3);

			using (var ms = new MemoryStream())
			{
				var icon = infoPCK.CreateNewFile("icon.png", PckFile.FileData.FileType.TextureFile);
				Resources.TexturePackIcon.Save(ms, ImageFormat.Png);
				icon.SetData(ms.ToArray());
			}

			using (var ms = new MemoryStream())
			{
				var comparison = infoPCK.CreateNewFile("comparison.png", PckFile.FileData.FileType.TextureFile);
				Resources.Comparison.Save(ms, ImageFormat.Png);
				comparison.SetData(ms.ToArray());
			}

			return newPck;
		}

		private PckFile InitializeMashUpPack(int packId, int packVersion, string packName, string res)
		{
            var newPck = InitializeTexturePack(packId, packVersion, packName, res, true);
			var gameRuleFile = newPck.CreateNewFile("GameRules.grf", PckFile.FileData.FileType.GameRulesFile);
			var grfFile = new GameRuleFile();
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
			using (var stream = new MemoryStream())
            {	
				var writer = new GameRuleFileWriter(grfFile);
				writer.WriteToStream(stream);
				gameRuleFile.SetData(stream.ToArray());
			}
			return newPck;
		}

		private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkSaveState();
			RenamePrompt namePrompt = new RenamePrompt("");
			namePrompt.OKButton.Text = "Ok";
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
            CreateTexturePack packPrompt = new CreateTexturePack();
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
                currentPCK = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
				isTemplateFile = true;
                wasModified = true;
                LoadEditorTab();
			}
		}

		private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
            checkSaveState();
            CreateTexturePack packPrompt = new CreateTexturePack();
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
                currentPCK = InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
				isTemplateFile = true;
				wasModified = false;
				LoadEditorTab();
			}
		}

		private void advancedMetaAddingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//opens dialog for bulk minefile editing
			using AdvancedOptions advanced = new AdvancedOptions(currentPCK);
			if (advanced.ShowDialog() == DialogResult.OK)
				wasModified = true;
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkSaveState();
			CloseEditorTab();
		}

		private void programInfoToolStripMenuItem_Click(object sender, EventArgs e)
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
					using (var fs = File.OpenRead(ofd.FileName))
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
							MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
							Debug.WriteLine(ex.Message);
						}
					}
					foreach (PckFile.FileData file in pckfile.Files)
					{
						string filepath = $"{sfd.SelectedPath}/{file.Filename}";
						FileInfo fileinfo = new FileInfo(filepath);
						fileinfo.Directory.Create();
						File.WriteAllBytes(filepath, file.Data); // writes data to file
						//attempts to generate reimportable metadata file out of minefiles metadata
						string metaData = "";

						foreach (var entry in file.Properties)
						{
							metaData += $"{entry.Item1}: {entry.Item2}{Environment.NewLine}";
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
				bool hasSkinsPck = currentPCK.HasFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile);

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
                        string[] properties = File.ReadAllText(fullfilename + ".txt").Split(new string[]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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
						var skinsfile = currentPCK.GetFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile);
                        using (var ms = new MemoryStream(skinsfile.Data))
						{
							var reader = new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
                            var skinspck = reader.FromStream(ms);
							skinspck.Files.Add(newFile);
							ms.Position = 0;
							var writer = new PckFileWriter(skinspck, LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
							writer.WriteToStream(ms);
							skinsfile.SetData(ms.ToArray());
						}
						continue;
                    }
					currentPCK.Files.Add(newFile);
				}
				BuildMainTreeView();
				wasModified = true;
			}
		}
		#endregion

		private bool TryGetLocFile(out LOCFile locFile)
		{
			if (!currentPCK.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locdata) &&
				!currentPCK.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locdata))
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
            if (!currentPCK.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locdata) &&
                !currentPCK.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locdata))
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
				Debug.WriteLine(ex.Message);
			}
			return false;
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
					PckFile.FileData mfNew = currentPCK.CreateNewFile(skinNameImport, PckFile.FileData.FileType.SkinFile);
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
								mfNew.Properties.Add(new ValueTuple<string, string>(key, value));
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
			RenamePrompt folderNamePrompt = new RenamePrompt("");
			folderNamePrompt.OKButton.Text = "Add";
			if (folderNamePrompt.ShowDialog() == DialogResult.OK)
			{
				TreeNode folerNode = CreateNode(folderNamePrompt.NewText);
				folerNode.ImageIndex = 0;
				folerNode.SelectedImageIndex = 0;

				TreeNodeCollection nodeCollection = treeViewMain.Nodes;
				if (treeViewMain.SelectedNode is TreeNode node)
				{
                    if (node.Tag is PckFile.FileData)
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

		private void binkaConversionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
		}

		private void fAQToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			//System.Diagnostics.Process.Start(hosturl + "pckStudio#faq");
		}

		private void convertToBedrockToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("This feature is currently being reworked.", "Currently unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void openToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DateTime Begin = DateTime.Now;
            //pckCenter open = new pckCenter();
            PckCenterBeta open = new PckCenterBeta();
			open.Show();
			TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

			Debug.WriteLine("Completed in: " + duration);
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
			if (needsUpdate && File.Exists(Program.AppData + @"\nobleUpdater.exe"))
			{
				Process.Start(Program.AppData + @"\nobleUpdater.exe"); // starts updater
				Application.Exit(); // closes PCK Studio to let updatear finish the job
			}
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
			string[] Filepaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (Filepaths.Length > 1)
				MessageBox.Show("Only one pck file at a time is currently supported");
			LoadPck(Filepaths[0]);
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

		private void forMattNLContributorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/mattnl");
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

		private void setFileType_Click(object sender, EventArgs e, PckFile.FileData.FileType type)
		{
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PckFile.FileData file)
			{
				Debug.WriteLine($"Setting {file.Filetype} to {type}");
				file.Filetype = type;
				SetPckFileIcon(t, type);
				if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath)) RebuildSubPCK(treeViewMain.SelectedNode);
			}
		}

		private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Texture File(*.png;*.tga)|*.png;*.tga";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				using RenamePrompt renamePrompt = new RenamePrompt(Path.GetFileName(fileDialog.FileName));
				renamePrompt.TextLabel.Text = "Path";
				if (renamePrompt.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(renamePrompt.NewText))
				{
					var file = currentPCK.CreateNewFile(renamePrompt.NewText, PckFile.FileData.FileType.TextureFile);
					file.SetData(File.ReadAllBytes(fileDialog.FileName));
					BuildMainTreeView();
					wasModified = true;
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

		private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PckFile.FileData file && file.Filetype == PckFile.FileData.FileType.TextureFile)
			{
				string textureDirectory = Path.GetDirectoryName(file.Filename);
				string textureName = Path.GetFileNameWithoutExtension(file.Filename);

				if (IsFilePathMipMapped(textureName)) return;

				string textureExtension = Path.GetExtension(file.Filename);

				// TGA is not yet supported
				if (textureExtension == ".tga") return;

				using MipMapPrompt diag = new MipMapPrompt();
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					for (int i = 2; i < 2 + diag.Levels; i++)
					{
						string mippedPath = textureDirectory + "/" + textureName + "MipMapLevel" + i + textureExtension;
						Debug.WriteLine(mippedPath);
						if (currentPCK.HasFile(mippedPath, PckFile.FileData.FileType.TextureFile)) 
							currentPCK.Files.Remove(currentPCK.GetFile(mippedPath, PckFile.FileData.FileType.TextureFile));
						PckFile.FileData MipMappedFile = new PckFile.FileData(mippedPath, PckFile.FileData.FileType.TextureFile);

						Image originalTexture = Image.FromStream(new MemoryStream(file.Data));
						int NewWidth = originalTexture.Width / (int)Math.Pow(2,i - 1);
						int NewHeight = originalTexture.Height / (int)Math.Pow(2, i - 1);
						Rectangle tileArea = new Rectangle(0, 0,
							NewWidth < 1 ? 1 : NewWidth, 
							NewHeight < 1 ? 1 : NewHeight);
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

						currentPCK.Files.Insert(currentPCK.Files.IndexOf(file) + i - 1, MipMappedFile);
					}
					BuildMainTreeView();
				}
			}
		}

		private void colourscolToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("colours.col", PckFile.FileData.FileType.ColourTableFile, out _))
			{
				MessageBox.Show("A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			var newColorFile = currentPCK.CreateNewFile("colours.col", PckFile.FileData.FileType.ColourTableFile);
			newColorFile.SetData(Resources.tu69colours);
			BuildMainTreeView();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (!isSelectingTab) e.Cancel = true;
		}

		private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode node &&
				node.Tag is PckFile.FileData file &&
				file.Filetype == PckFile.FileData.FileType.SkinFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "3DS Texture | *.3dst";
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

		private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode node &&
				node.Tag is PckFile.FileData file)
			{
				using (var input = new TextPrompt())
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
						if (IsSubPCKNode(node.FullPath)) RebuildSubPCK(node);
						wasModified = true;
					}
				}
			}
		}

		private void correctSkinDecimalsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode node && node.Tag is PckFile.FileData file && file.Filetype == PckFile.FileData.FileType.SkinFile)
			{
				foreach(var p in file.Properties.FindAll(s => s.property == "BOX" || s.property == "OFFSET"))
				{
					file.Properties[file.Properties.IndexOf(p)] = (p.property, p.value.Replace(',','.'));
				}
				ReloadMetaTreeView();
				if (IsSubPCKNode(node.FullPath)) RebuildSubPCK(node);
				wasModified = true;
			}
		}

		private void addCustomPackIconToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (!currentPCK.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out PckFile.FileData file) ||
				string.IsNullOrEmpty(file.Properties.GetPropertyValue("PACKID"))
				)
			{
				MessageBox.Show("No PackID is present in this pack. To avoid this error, please open a PCK with a PackID before trying again.", "Operation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Minecraft Archive|*.arc";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				string filepath = dialog.FileName;
                dialog.Filter = "Pack Icon|*.png";
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					using (var fs = File.OpenRead(filepath))
					{
						ARCUtil.Inject(fs, (
							string.Format("Graphics\\PackGraphics\\{0}.png", file.Properties.GetPropertyValue("PACKID")),
							File.ReadAllBytes(dialog.FileName))
							);
						MessageBox.Show("Successfully added Pack Icon to Archive!", "Successfully Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}
		}

		private void CreateSkinsPCKToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile, out _))
			{
				MessageBox.Show("A Skins.pck file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("Skins.pck", PckFile.FileData.FileType.SkinDataFile, () =>
			{
				using var stream = new MemoryStream();
				var writer = new PckFileWriter(new PckFile(3) { HasVerionString = true },
					LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian);
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

		private void editAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode node &&
				node.Tag is PckFile.FileData file)
			{
				var props = file.Properties.Select(l => l.property + " " + l.value);
				using (var input = new TextPrompt(props.ToArray()))
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
						if (IsSubPCKNode(node.FullPath)) RebuildSubPCK(node);
						wasModified = true;
					}
				}
			}
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
					PckFile.FileData file = currentPCK.CreateNewFile(diag.Filepath, (PckFile.FileData.FileType)diag.Filetype);
					file.SetData(File.ReadAllBytes(ofd.FileName));

					if (IsSubPCKNode(treeViewMain.SelectedNode.FullPath)) RebuildSubPCK(treeViewMain.SelectedNode);
					//else treeViewMain.Nodes.Add();

					BuildMainTreeView();
					wasModified = true;
				}
			}
			return;
		}

		private void behavioursbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("behaviours.bin", PckFile.FileData.FileType.BehavioursFile, out _))
			{
				MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}

			currentPCK.CreateNewFile("behaviours.bin", PckFile.FileData.FileType.BehavioursFile, BehaviourResources.BehaviourFileInitializer);
			BuildMainTreeView();
		}

		private void entityMaterialsbinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.TryGetFile("entityMaterials.bin", PckFile.FileData.FileType.MaterialFile, out _))
			{
				MessageBox.Show("A behaviours file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			currentPCK.CreateNewFile("entityMaterials.bin", PckFile.FileData.FileType.MaterialFile, MaterialResources.MaterialsFileInitializer);
			BuildMainTreeView();
		}
    }

    public class PckNodeSorter : System.Collections.IComparer, IComparer<TreeNode>
	{
		private bool CheckForSkinAndCapeFiles(TreeNode node)
		{
            if (node.Tag is PckFile.FileData file)
            {
				return file.Filetype == PckFile.FileData.FileType.SkinFile ||
					file.Filetype == PckFile.FileData.FileType.CapeFile;
            }
			return false;
        }

        public int Compare(TreeNode first, TreeNode second)
        {
			// ignore these files in order to preserve skin(and cape) files
            if (CheckForSkinAndCapeFiles(first))
            {
				return 0;
            }
            if (CheckForSkinAndCapeFiles(second))
            {
				return 0;
            }

            int result = first.Text.CompareTo(second.Text);
            if (result != 0) return result;
            return first.ImageIndex.CompareTo(second.ImageIndex);
        }

        int System.Collections.IComparer.Compare(object x, object y)
		{
			return x is TreeNode NodeX && y is TreeNode NodeY ? Compare(NodeX, NodeY) : 0;
		}
	}
}