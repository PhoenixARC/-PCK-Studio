using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Drawing.Imaging;

using PckStudio.Properties;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO;
using PckStudio.Classes.IO.LOC;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.IO.GRF;
using PckStudio.Classes.Utils;
using PckStudio.Forms;
using PckStudio.Forms.Utilities;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;

using RichPresenceClient;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		string saveLocation = string.Empty;
		PCKFile currentPCK = null;
		bool needsUpdate = false;
		bool saved = true;
		bool isTemplateFile = false;
		bool isSelectingTab = false;

		readonly Dictionary<PCKFile.FileData.FileType, Action<PCKFile.FileData>> pckFileTypeHandler;

		public MainForm()
		{
			InitializeComponent();
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
			pckOpen.AllowDrop = true;

            isSelectingTab = true;
			tabControl.SelectTab(0);
            isSelectingTab = false;

			labelVersion.Text = "PCK Studio: " + Application.ProductVersion;
			ChangelogRichTextBox.Text = Resources.CHANGELOG;
#if DEBUG
			labelVersion.Text += " (Debug build)";
#endif

			pckFileTypeHandler = new Dictionary<PCKFile.FileData.FileType, Action<PCKFile.FileData>>(15)
			{
				[PCKFile.FileData.FileType.SkinFile]            = HandleSkinFile,
				[PCKFile.FileData.FileType.CapeFile]            = null,
				[PCKFile.FileData.FileType.TextureFile]         = HandleTextureFile,
				[PCKFile.FileData.FileType.UIDataFile]          = _ => throw new NotSupportedException("unused in-game"),
				[PCKFile.FileData.FileType.InfoFile]            = null,
				[PCKFile.FileData.FileType.TexturePackInfoFile] = null,
				[PCKFile.FileData.FileType.LocalisationFile]    = HandleLocalisationFile,
				[PCKFile.FileData.FileType.GameRulesFile]       = HandleGameRuleFile,
				[PCKFile.FileData.FileType.AudioFile]           = HandleAudioFile,
				[PCKFile.FileData.FileType.ColourTableFile]     = HandleColourFile,
				[PCKFile.FileData.FileType.GameRulesHeader]     = HandleGameRuleFile,
				[PCKFile.FileData.FileType.SkinDataFile]        = null,
				[PCKFile.FileData.FileType.ModelsFile]          = HandleModelsFile,
				[PCKFile.FileData.FileType.BehavioursFile]      = null,
				[PCKFile.FileData.FileType.MaterialFile]        = null,
			};
		}

		public void LoadFromPath(string filepath)
		{
            currentPCK = openPck(filepath);
			if (currentPCK == null)
			{
				MessageBox.Show(string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error");
				return;
			}
            if (addPasswordToolStripMenuItem.Enabled = checkForPassword())
            {
                LoadEditorTab();
            }
        }

		private void Form1_Load(object sender, EventArgs e)
		{
			RPC.Initialize();
			if (currentPCK == null)
				RPC.SetPresence("An Open Source .PCK File Editor", "Program by PhoenixARC");

			skinToolStripMenuItem1.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.SkinFile);
			capeToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.CapeFile);
			textureToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.TextureFile);
			languagesFileLOCToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.LocalisationFile);
			gameRulesFileGRFToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.GameRulesFile);
			audioPCKFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.AudioFile);
			coloursCOLFileToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.ColourTableFile);
			gameRulesHeaderGRHToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.GameRulesHeader);
			skinsPCKToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.SkinDataFile);
			modelsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.ModelsFile);
			behavioursFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.BehavioursFile);
			entityMaterialsFileBINToolStripMenuItem.Click += (sender, e) => setFileType_Click(sender, e, PCKFile.FileData.FileType.MaterialFile);

			try
			{
				Directory.CreateDirectory(Program.AppDataCache + "\\mods\\");
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show("Could not Create directory due to Unauthorized Access");
				Debug.WriteLine(ex.Message);
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
					LoadFromPath(ofd.FileName);
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
					MessageBox.Show("Failed to open pck\n" +
						$"Try {(LittleEndianCheckBox.Checked ? "unchecking" : "checking")} the 'Open/Save as Vita/PS4 pck' check box in the upper right corner.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Debug.WriteLine(ex.Message);
				}
			}
			if (pck?.type < 3) throw new Exception("Can't open pck file of type: " + pck.type.ToString());
			return pck;
		}

		private bool checkForPassword()
		{
			if (currentPCK.TryGetFile("0", PCKFile.FileData.FileType.InfoFile, out PCKFile.FileData file))
			{
				if (file.properties.Contains("LOCK"))
					return new pckLocked(file.properties.GetPropertyValue("LOCK")).ShowDialog() == DialogResult.OK;
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
			isSelectingTab = true;
			tabControl.SelectTab(1);
			isSelectingTab = false;
			if (TryGetLocFile(out LOCFile locfile) &&
				locfile.HasLocEntry("IDS_DISPLAY_NAME") &&
				locfile.Languages.Contains("en-EN"))
				RPC.SetPresence($"Editing a Pack: {locfile.GetLocEntry("IDS_DISPLAY_NAME", "en-EN")}", "Program by PhoenixARC");
		}

		private void CloseEditorTab()
		{
			isSelectingTab = true;
			tabControl.SelectTab(0);
			isSelectingTab = false;
			currentPCK = null;
			saved = true;
			isTemplateFile = false;
			saveLocation = string.Empty;
			pictureBoxImagePreview.Image = Resources.NoImageFound;
			treeViewMain.Nodes.Clear();
			treeMeta.Nodes.Clear();
			treeViewMain.Enabled = false;
			treeMeta.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem1.Enabled = false;
			metaToolStripMenuItem.Enabled = false;
			addPasswordToolStripMenuItem.Enabled = false;
			advancedMetaAddingToolStripMenuItem.Enabled = false;
			convertToBedrockToolStripMenuItem.Enabled = false;
			closeToolStripMenuItem.Visible = false;
			fileEntryCountLabel.Text = string.Empty;
			RPC.SetPresence("An Open Source .PCK File Editor", "Program by PhoenixARC");
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

		private void BuildPckTreeView(TreeNodeCollection root, PCKFile pckFile)
		{
			foreach (var file in pckFile.Files)
			{
				// Replace backward slashes('\') with forward slashes('/') since some filepath use backward slashes
				file.filepath = file.filepath.Replace('\\', '/'); // fix any file paths that may be incorrect
				TreeNode node = BuildNodeTreeBySeperator(root, file.filepath, '/');
				node.Tag = file;
				switch (file.filetype)
				{
					case PCKFile.FileData.FileType.SkinDataFile:
					case PCKFile.FileData.FileType.TexturePackInfoFile:
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
				SetPckFileIcon(node, file.filetype);
			};
		}

		private void BuildMainTreeView()
		{
			treeViewMain.Nodes.Clear();
			BuildPckTreeView(treeViewMain.Nodes, currentPCK);
        }

		bool IsFilePathMipMapped(string filepath)
		{
			string name = Path.GetFileNameWithoutExtension(filepath); // We only want to test the file name itself. ex: "terrainMipMapLevel2"
			if (!char.IsDigit(name[name.Length - 1])) return false; // check if last character is a digit (0-9). If not return false
			// If string does not end with MipMapLevel, then it's not MipMapped
			if (!name.Remove(name.Length - 1, 1).EndsWith("MipMapLevel")) return false;
			return true;
		}

		private void HandleTextureFile(PCKFile.FileData file)
        {
			if (file.filepath.StartsWith("res/textures/blocks/") || file.filepath.StartsWith("res/textures/items/") &&
				!file.filepath.EndsWith("clock.png") && !file.filepath.EndsWith("compass.png"))
			{
				if (IsFilePathMipMapped(file.filepath) &&
					currentPCK.Files.Find(pckfile => 
						// todo write cleaner ?
						pckfile.filepath.Equals(file.filepath.Remove(file.filepath.Length - 12 - Path.GetExtension(file.filepath).Length) + Path.GetExtension(file.filepath)))
					is PCKFile.FileData originalAnimationFile)
				{
					file = originalAnimationFile;
				}
				using (AnimationEditor animationEditor = new AnimationEditor(file))
				{
					if (animationEditor.ShowDialog(this) == DialogResult.OK)
					{
						file.filepath = animationEditor.TileName;
						BuildMainTreeView();
					}
				}
			}
        }

        private void HandleGameRuleFile(PCKFile.FileData file)
        {
			using GRFEditor grfEditor = new GRFEditor(file);
            if (grfEditor.ShowDialog(this) == DialogResult.OK)
                saved = false;
        }

		private void HandleAudioFile(PCKFile.FileData file)
		{
            if (!TryGetLocFile(out LOCFile locFile))
                throw new Exception("No .loc File found.");
            using AudioEditor audioEditor = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
			if (audioEditor.ShowDialog(this) == DialogResult.OK)
			{
				saved = false;
				TrySetLocFile(locFile);
			}
        }

		private void HandleLocalisationFile(PCKFile.FileData file)
		{
            using LOCEditor locedit = new LOCEditor(file);
			if (locedit.ShowDialog(this) == DialogResult.OK)
				saved = false;
        }

		private void HandleColourFile(PCKFile.FileData file)
		{
            if (file.size == 0)
            {
                MessageBox.Show("No Color data found.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            using COLEditor diag = new COLEditor(file);
			if (diag.ShowDialog(this) == DialogResult.OK)
				saved = false;
        }

		public void HandleSkinFile(PCKFile.FileData file)
		{
			if (file.properties.HasProperty("BOX"))
			{
				using (generateModel generate = new generateModel(file.properties, new PictureBox()))
					if (generate.ShowDialog() == DialogResult.OK)
					{
						entryDataTextBox.Text = entryTypeTextBox.Text = string.Empty;
						saved = false;
						ReloadMetaTreeView();
					}
			}
			else
			{
				using (var ms = new MemoryStream(file.data))
				{
					var texture = Image.FromStream(ms);
					SkinPreview frm = new SkinPreview(texture);
					frm.ShowDialog(this);
					frm.Dispose();
				}
			}
        }

		public void HandleModelsFile(PCKFile.FileData file)
		{
			MessageBox.Show("Models.bin support has not been implemented. You can use the Spark Editor for the time being to edit these files.", "Not implemented yet.");
			//throw new NotImplementedException();
		}

		private void selectNode(object sender, TreeViewEventArgs e)
		{
			ReloadMetaTreeView();
			entryTypeTextBox.Text = entryDataTextBox.Text = labelImageSize.Text = string.Empty;
			buttonEdit.Visible = false;
			pictureBoxImagePreview.Image = Resources.NoImageFound;
			var node = e.Node;
			viewFileInfoToolStripMenuItem.Visible = false;
			if (node is TreeNode t && t.Tag is PCKFile.FileData file)
			{
				viewFileInfoToolStripMenuItem.Visible = true;
				if (file.properties.HasProperty("BOX"))
				{
					buttonEdit.Text = "EDIT BOXES";
					buttonEdit.Visible = true;
				}
				else if (file.properties.HasProperty("ANIM") &&
						(file.properties.GetPropertyValue("ANIM") == "0x40000" ||
						 file.properties.GetPropertyValue("ANIM") == "0x80000"))
				{
					buttonEdit.Text = "View Skin";
					buttonEdit.Visible = true;
				}

				switch (file.filetype)
				{
					case PCKFile.FileData.FileType.SkinFile:
					case PCKFile.FileData.FileType.CapeFile:
					case PCKFile.FileData.FileType.TextureFile:
						// TODO: Add tga support
						if (Path.GetExtension(file.filepath) == ".tga") break;
						using (MemoryStream png = new MemoryStream(file.data))
						{
							try
							{
								pictureBoxImagePreview.Image = Image.FromStream(png);
								labelImageSize.Text = $"{pictureBoxImagePreview.Image.Size.Width}x{pictureBoxImagePreview.Image.Size.Height}";
							}
							catch (Exception ex)
							{
								labelImageSize.Text = "";
								pictureBoxImagePreview.Image = Resources.NoImageFound;
								Debug.WriteLine("Not a supported image format. Setting back to default");
							}
						}

						if ((file.filepath.StartsWith("res/textures/blocks/") || file.filepath.StartsWith("res/textures/items/")) &&
							!file.filepath.EndsWith("clock.png") && !file.filepath.EndsWith("compass.png") &&
							file.filetype == PCKFile.FileData.FileType.TextureFile 
							&& !IsFilePathMipMapped(file.filepath))
						{
							buttonEdit.Text = "EDIT TEXTURE ANIMATION";
							buttonEdit.Visible = true;
						}
						break;

					case PCKFile.FileData.FileType.LocalisationFile:
						buttonEdit.Text = "EDIT LOC";
						buttonEdit.Visible = true;
						break;

					case PCKFile.FileData.FileType.AudioFile when file.filepath == "audio.pck":
						buttonEdit.Text = "EDIT MUSIC CUES";
						buttonEdit.Visible = true;
						break;

					case PCKFile.FileData.FileType.ColourTableFile when file.filepath == "colours.col":
						buttonEdit.Text = "EDIT COLORS";
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
			if (node.Tag is PCKFile.FileData file)
			{
				using SaveFileDialog exFile = new SaveFileDialog();
				exFile.FileName = Path.GetFileName(file.filepath);
				exFile.Filter = Path.GetExtension(file.filepath).Replace(".", string.Empty) + " File|*" + Path.GetExtension(file.filepath);
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

		private void Save(string filePath)
		{
			bool isSkinsPCK = false;
			if (!currentPCK.TryGetFile("0", PCKFile.FileData.FileType.InfoFile, out PCKFile.FileData _))
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
			using (var fs = File.OpenWrite(filePath))
			{
				PCKFileWriter.Write(fs, currentPCK, LittleEndianCheckBox.Checked, isSkinsPCK);
			}
			saved = true;
			MessageBox.Show("Saved Pck file", "File Saved");
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PCKFile.FileData file)
			{
				using var ofd = new OpenFileDialog();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					file.SetData(File.ReadAllBytes(ofd.FileName));
					saved = false;
				}
				return;
			}
			//deleteEntryToolStripMenuItem_Click(sender, e);
			//using FolderBrowserDialog folderDialog = new FolderBrowserDialog();
			//folderDialog.Description = "Select Folder";
			//if (folderDialog.ShowDialog() == DialogResult.OK)
			//{
			//	string[] FilePaths = Directory.GetFiles(folderDialog.SelectedPath, "*.png");
			//	Array.ForEach(FilePaths, filePath => currentPCK.Files.Add(new PCKFile.FileData(filePath, 2)));
			//}
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
				if (file.filetype == PCKFile.FileData.FileType.SkinFile || file.filetype == PCKFile.FileData.FileType.CapeFile)
                {
                    if (TryGetLocFile(out LOCFile locFile))
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
					if (add.HasCape)
						currentPCK.Files.Add(add.CapeFile);
					if (!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
						add.SkinFile.filepath = $"{treeViewMain.SelectedNode.FullPath}/{add.SkinFile.filepath}";
					currentPCK.Files.Add(add.SkinFile);
					TrySetLocFile(locFile);
					saved = false;
					BuildMainTreeView();
				}
		}

        private static PCKFile.FileData CreateNewAudioFile(bool isLittle)
        {
            // create actual valid pck file structure
            PCKAudioFile audioPck = new PCKAudioFile();
            audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld);
            audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.Nether);
            audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.End);
            PCKFile.FileData pckFileData = new PCKFile.FileData("audio.pck", PCKFile.FileData.FileType.AudioFile);
            using (var stream = new MemoryStream())
            {
                PCKAudioFileWriter.Write(stream, audioPck, isLittle);
                pckFileData.SetData(stream.ToArray());
            }
            return pckFileData;
        }

		private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (currentPCK.Files.FindIndex(file => file.filepath.Contains("audio.pck")) != -1)
			{
				MessageBox.Show("There is already an audio.pck present in this file!", "Can't create audio.pck");
				return;
			}
			if (!TryGetLocFile(out LOCFile locFile))
				throw new Exception("No .loc file found.");
			var file = CreateNewAudioFile(LittleEndianCheckBox.Checked);
			currentPCK.Files.Add(file);
			AudioEditor diag = new AudioEditor(file, locFile, LittleEndianCheckBox.Checked);
			if (diag.ShowDialog(this) == DialogResult.OK)
				TrySetLocFile(locFile);
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
					using ChangeTile diag = new ChangeTile();
					if (diag.ShowDialog(this) == DialogResult.OK)
					{
						using Image img = new Bitmap(ofd.FileName);
						var file = AnimationUtil.CreateNewAnimationFile(img, diag.SelectedTile, diag.IsItem);
						using AnimationEditor animationEditor = new AnimationEditor(file);
						if (animationEditor.ShowDialog() == DialogResult.OK)
						{
							file.filepath = animationEditor.TileName;
							currentPCK.Files.Add(file);
							ReloadMetaTreeView();
							BuildMainTreeView();
							saved = false;
						}
					}
				}
			}
		}

		private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PCKFile.FileData file)
				pckFileTypeHandler[file.filetype]?.Invoke(file);
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
			if (treeMeta.SelectedNode == null || !(treeMeta.SelectedNode.Tag is ValueTuple<string, string>) ||
				treeViewMain.SelectedNode == null || !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
				return;
			PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;
			var property = (ValueTuple<string, string>)treeMeta.SelectedNode.Tag;
			int i = file.properties.IndexOf(property);
			if (property.Item1 == "ANIM" && i != -1 && file.filetype == PCKFile.FileData.FileType.SkinFile)
			{
				using Forms.Utilities.Skins.ANIMEditor diag = new Forms.Utilities.Skins.ANIMEditor(property.Item2);
				try
				{
					if (diag.ShowDialog(this) == DialogResult.OK && diag.saved)
					{
						file.properties[i] = new ValueTuple<string, string>("ANIM", diag.outANIM);
						ReloadMetaTreeView();
						saved = false;
					}
					return;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					MessageBox.Show("Failed to parse ANIM value, aborting to normal functionality. Please make sure the value only includes hexadecimal characters (0-9,A-F) and has no more than 8 characters. It can have an optional prefix of \"0x\".");
				}
			}
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

			PCKFile.FileData mf = new PCKFile.FileData("", mfO.filetype); // Creates new minefile template
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
				if (isClone) newFileName = nameWithoutExt.Remove(nameWithoutExt.Length - 7) + clone_str + Path.GetExtension(mfO.filepath);
				else newFileName = nameWithoutExt + clone_str + Path.GetExtension(mfO.filepath);
				prev_clone_str = clone_str;
			}
			while (currentPCK.HasFile(dirName + (string.IsNullOrEmpty(dirName) ? "" : "/") + newFileName, mf.filetype));

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
			if (treeMeta.SelectedNode is TreeNode t && t.Tag is ValueTuple<string, string> property &&
				treeViewMain.SelectedNode is TreeNode main && main.Tag is PCKFile.FileData file &&
				file.properties.Remove(property))
			{
				treeMeta.SelectedNode.Remove();
				saved = false;
			}
		}

		private void ReloadMetaTreeView()
		{
			treeMeta.Nodes.Clear();
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PCKFile.FileData file)
				file.properties.ForEach(p =>
			{
					TreeNode node = new TreeNode(p.Item1);
					node.Tag = p;
				treeMeta.Nodes.Add(node);
				});
		}

		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PCKFile.FileData file)
			{
			using addMeta add = new addMeta();
			if (add.ShowDialog() == DialogResult.OK)
			{
					file.properties.Add((add.PropertyName, add.PropertyValue));
				ReloadMetaTreeView();
				saved = false;
			}
		}
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

		private void InitializeSkinPack(int packId, int packVersion, string packName)
		{
			currentPCK = new PCKFile(3);
			var zeroFile = new PCKFile.FileData("0", PCKFile.FileData.FileType.InfoFile);
			zeroFile.properties.Add(("PACKID", packId.ToString()));
			zeroFile.properties.Add(("PACKVERSION", packVersion.ToString()));
			var loc = new PCKFile.FileData("localisation.loc", PCKFile.FileData.FileType.LocalisationFile);
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

		private void InitializeTexturePack(int packId, int packVersion, string packName, string res)
		{
			InitializeSkinPack(packId, packVersion, packName);
			var texturepackInfo = new PCKFile.FileData($"{res}/{res}Info.pck", PCKFile.FileData.FileType.TexturePackInfoFile);
			texturepackInfo.properties.Add(("PACKID", "0"));
			texturepackInfo.properties.Add(("DATAPATH", $"{res}Data.pck"));
			currentPCK.Files.Add(texturepackInfo);
		}

		private void InitializeMashUpPack(int packId, int packVersion, string packName, string res)
		{
			InitializeTexturePack(packId, packVersion, packName, res);
			var gameRuleFile = new PCKFile.FileData("GameRules.grf", PCKFile.FileData.FileType.GameRulesFile);
			var grfFile = new GRFFile();
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
			CreateTexturePack packPrompt = new CreateTexturePack("");
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
				InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.packName, packPrompt.packRes);
				isTemplateFile = true;
				LoadEditorTab();
			}
		}

		private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateTexturePack packPrompt = new CreateTexturePack("");
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
				InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.packName, packPrompt.packRes);
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
							Debug.WriteLine(ex.Message);
						}
					}
					if (pckfile.HasFile("0", PCKFile.FileData.FileType.InfoFile) &&
						pckfile.GetFile("0", PCKFile.FileData.FileType.InfoFile).properties.HasProperty("LOCK") &&
						new pckLocked(pckfile.GetFile("0", PCKFile.FileData.FileType.InfoFile).properties.GetProperty("LOCK").Item2).ShowDialog() != DialogResult.OK)
						return; // cancel extraction if password not provided
					foreach (PCKFile.FileData file in pckfile.Files)
					{
						string filepath = $"{sfd.SelectedPath}/{file.filepath}";
						FileInfo fileinfo = new FileInfo(filepath);
						fileinfo.Directory.Create();
						File.WriteAllBytes(filepath, file.data); // writes data to file
						//attempts to generate reimportable metadata file out of minefiles metadata
						string metaData = "";

						foreach (var entry in file.properties)
						{
							metaData += $"{entry.Item1}: {entry.Item2}{Environment.NewLine}";
						}

						File.WriteAllText(sfd.SelectedPath + @"\" + file.filepath + ".txt", metaData);
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
				bool hasSkinsPck = currentPCK.HasFile("Skins.pck", PCKFile.FileData.FileType.SkinDataFile);

				foreach (var fullfilename in Directory.GetFiles(contents.SelectedPath, "*.png"))
				{
					string filename = Path.GetFileNameWithoutExtension(fullfilename);
					// sets file type based on wether its a cape or skin
                    PCKFile.FileData.FileType pckfiletype = filename.StartsWith("dlccape", StringComparison.OrdinalIgnoreCase)
						? PCKFile.FileData.FileType.CapeFile
						: PCKFile.FileData.FileType.SkinFile;
					string pckfilepath = (hasSkinsPck ? "Skins/" : string.Empty) + filename + ".png";


                    PCKFile.FileData newFile = new PCKFile.FileData(pckfilepath, pckfiletype);
					byte[] filedata = File.ReadAllBytes(fullfilename);
                    newFile.SetData(filedata);

					if (File.Exists(fullfilename + ".txt"))
					{
                        string[] properties = File.ReadAllText(fullfilename + ".txt").Split(new string[]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string property in properties)
						{
                            string[] param = property.Split(':');
                            if (param.Length < 2) continue;
                            newFile.properties.Add((param[0], param[1]));
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
						var skinsfile = currentPCK.GetFile("Skins.pck", PCKFile.FileData.FileType.SkinDataFile);
                        using (var ms = new MemoryStream(skinsfile.data))
						{
							var skinspck = PCKFileReader.Read(ms, LittleEndianCheckBox.Checked);
							skinspck.Files.Add(newFile);
							ms.Position = 0;
							PCKFileWriter.Write(ms, skinspck, LittleEndianCheckBox.Checked);
							skinsfile.SetData(ms.ToArray());
						}
						continue;
                    }
					currentPCK.Files.Add(newFile);
				}
				BuildMainTreeView();
				saved = false;
			}
		}
		#endregion

		private bool TryGetLocFile(out LOCFile locFile)
		{
			if (!currentPCK.TryGetFile("localisation.loc", PCKFile.FileData.FileType.LocalisationFile, out PCKFile.FileData locdata) &&
				!currentPCK.TryGetFile("languages.loc", PCKFile.FileData.FileType.LocalisationFile, out locdata))
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
				Debug.WriteLine(ex.Message);
			}
			locFile = null;
			return false;
		}

		private bool TrySetLocFile(in LOCFile locFile)
		{
            if (!currentPCK.TryGetFile("localisation.loc", PCKFile.FileData.FileType.LocalisationFile, out PCKFile.FileData locdata) &&
                !currentPCK.TryGetFile("languages.loc", PCKFile.FileData.FileType.LocalisationFile, out locdata))
            {
                return false;
            }

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
				Debug.WriteLine(ex.Message);
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

				TreeNodeCollection nodeCollection = treeViewMain.Nodes;
				if (treeViewMain.SelectedNode is TreeNode node)
				{
                    if (node.Tag is PCKFile.FileData)
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

		private void addPasswordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!currentPCK.HasFile("0", PCKFile.FileData.FileType.InfoFile)) throw new FileNotFoundException("0 file not found");
			PCKFile.FileData file = currentPCK.GetFile("0", PCKFile.FileData.FileType.InfoFile);
			if (checkForPassword())
			{
				AddPCKPassword add = new AddPCKPassword();
				if (add.ShowDialog() == DialogResult.OK)
					file.properties.SetProperty("LOCK", add.Password);
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

		private void openToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DateTime Begin = DateTime.Now;
			//pckCenter open = new pckCenter();
			Forms.Utilities.PckCenterBeta open = new Forms.Utilities.PckCenterBeta();
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
			string[] Filepaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (Filepaths.Length > 1)
				MessageBox.Show("Only one pck file at a time is currently supported");
			LoadFromPath(Filepaths[0]);
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

		private void SetPckFileIcon(TreeNode node, PCKFile.FileData.FileType type)
		{
            switch (type)
			{
				case PCKFile.FileData.FileType.AudioFile:
					node.ImageIndex = 1;
					node.SelectedImageIndex = 1;
					break;
				case PCKFile.FileData.FileType.LocalisationFile:
					node.ImageIndex = 3;
					node.SelectedImageIndex = 3;
					break;
				case PCKFile.FileData.FileType.TexturePackInfoFile:
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
					break;
				case PCKFile.FileData.FileType.ColourTableFile:
					node.ImageIndex = 6;
					node.SelectedImageIndex = 6;
					break;
				case PCKFile.FileData.FileType.ModelsFile:
					node.ImageIndex = 8;
					node.SelectedImageIndex = 8;
					break;
				case PCKFile.FileData.FileType.SkinDataFile:
					node.ImageIndex = 7;
					node.SelectedImageIndex = 7;
					break;
				case PCKFile.FileData.FileType.GameRulesFile:
					node.ImageIndex = 9;
					node.SelectedImageIndex = 9;
					break;
				case PCKFile.FileData.FileType.GameRulesHeader:
					node.ImageIndex = 10;
					node.SelectedImageIndex = 10;
					break;
				case PCKFile.FileData.FileType.InfoFile:
					node.ImageIndex = 11;
					node.SelectedImageIndex = 11;
					break;
				case PCKFile.FileData.FileType.SkinFile:
					node.ImageIndex = 12;
					node.SelectedImageIndex = 12;
					break;
				case PCKFile.FileData.FileType.CapeFile:
					node.ImageIndex = 13;
					node.SelectedImageIndex = 13;
					break;
				case PCKFile.FileData.FileType.TextureFile:
					node.ImageIndex = 14;
					node.SelectedImageIndex = 14;
					break;
				case PCKFile.FileData.FileType.BehavioursFile:
					node.ImageIndex = 15;
					node.SelectedImageIndex = 15;
					break;
				default: // unknown file format
					node.ImageIndex = 5;
					node.SelectedImageIndex = 5;
					break;
			}
		}

		private void setFileType_Click(object sender, EventArgs e, PCKFile.FileData.FileType type)
		{
			if (treeViewMain.SelectedNode is TreeNode t && t.Tag is PCKFile.FileData file)
            {
				Debug.WriteLine($"Setting {file.filetype} to {type}");
				file.filetype = type;
				SetPckFileIcon(t, type);
			}
		}

		private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Texture File(*.png)|*.png";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				using RenamePrompt renamePrompt = new RenamePrompt(Path.GetFileName(fileDialog.FileName));
				renamePrompt.TextLabel.Text = "Path";
				if (renamePrompt.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(renamePrompt.NewText))
				{
					var file = new PCKFile.FileData(renamePrompt.NewText, PCKFile.FileData.FileType.TextureFile);
					file.SetData(File.ReadAllBytes(fileDialog.FileName));
					currentPCK.Files.Add(file);
					BuildMainTreeView();
					saved = false;
				}
			}
		}

		private void viewFileInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PCKFile.FileData file)
			{
				MessageBox.Show(
					"File path: " + file.filepath +
					"\nAssigned File type: " + (int)file.filetype + " (" + file.filetype + ")" +
					"\nFile size: " + file.size +
					"\nProperties count: " + file.properties.Count
					, Path.GetFileName(file.filepath) + " file info");
			}
		}

		private void generateMipMapTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PCKFile.FileData file && file.filetype == PCKFile.FileData.FileType.TextureFile)
			{
				string textureDirectory = Path.GetDirectoryName(file.filepath);
				string textureName = Path.GetFileNameWithoutExtension(file.filepath);

				if (IsFilePathMipMapped(textureName)) return;

				string textureExtension = Path.GetExtension(file.filepath);

				// TGA is not yet supported
				if (textureExtension == ".tga") return;

				using MipMapPrompt diag = new MipMapPrompt();
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					for (int i = 2; i < 2 + diag.Levels; i++)
					{
						string mippedPath = textureDirectory + "/" + textureName + "MipMapLevel" + i + textureExtension;
						Debug.WriteLine(mippedPath);
						if (currentPCK.HasFile(mippedPath, PCKFile.FileData.FileType.TextureFile)) 
							currentPCK.Files.Remove(currentPCK.GetFile(mippedPath, PCKFile.FileData.FileType.TextureFile));
						PCKFile.FileData MipMappedFile = new PCKFile.FileData(mippedPath, PCKFile.FileData.FileType.TextureFile);

						Image originalTexture = Bitmap.FromStream(new MemoryStream(file.data));
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
			PCKFile.FileData NewColorFile;
			if (currentPCK.TryGetFile("colours.col", PCKFile.FileData.FileType.ColourTableFile, out NewColorFile))
			{
				MessageBox.Show("A color table file already exists in this PCK and a new one cannot be created.", "Operation aborted");
				return;
			}
			NewColorFile = new PCKFile.FileData("colours.col", PCKFile.FileData.FileType.ColourTableFile);
			NewColorFile.SetData(Resources.tu69colours);
			currentPCK.Files.Add(NewColorFile);
			BuildMainTreeView();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (!isSelectingTab) e.Cancel = true;
		}

		private void as3DSTextureFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode node &&
				node.Tag is PCKFile.FileData file &&
				file.filetype == PCKFile.FileData.FileType.SkinFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "3DS Texture | *.3dst";
				saveFileDialog.DefaultExt = ".3dst";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					using (var fs = saveFileDialog.OpenFile())
					{
						using var ms = new MemoryStream(file.data);
						Image img = Image.FromStream(ms);
						_3DSUtil.SetImageTo3DST(fs, img);
					}
				}
			}
		}

		private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode is TreeNode t &&
				t.Tag is PCKFile.FileData file)
			{
				using (var input = new TextPrompt())
				{
					if (input.ShowDialog(this) == DialogResult.OK)
					{
						foreach (var line in input.TextOutput)
						{
							file.properties.Add((line.Substring(0, line.IndexOf(' ')), line.Substring(line.IndexOf(' ') + 1)));
							ReloadMetaTreeView();
							saved = false;
						}
					}
				}
			}
		}
	}
}