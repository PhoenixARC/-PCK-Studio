using System;
using System.Collections.Generic;
using System.Collections;
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
using PckStudio.Forms;
using PckStudio.Forms.Utilities;
using PckStudio.Forms.Editor;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Classes.Misc;
using PckStudio.Classes.IO.PCK;
using PckStudio.Classes.IO._3DST;
using PckStudio.Internal;
using PckStudio.Features;
using PckStudio.Extensions;
using PckStudio.Popups;
using PckStudio.API.Miles;
using PckStudio.Classes.Utils;
using PckStudio.Controls;
using PckStudio.Interfaces;

namespace PckStudio
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		private PckManager PckManager = null;

		private Dictionary<string, TabPage> openFiles = new Dictionary<string, TabPage>();

        IComparer NodeSorter = new PckNodeSorter();

        public MainForm()
		{
			InitializeComponent();
			pckOpen.AllowDrop = true;
			Text = Application.ProductName;
			ChangelogRichTextBox.Text = Resources.CHANGELOG;
            labelVersion.Text = $"{Application.ProductName}: {Application.ProductVersion}";

#if BETA
			labelVersion.Text += $"{ApplicationBuildInfo.BetaBuildVersion}@{CommitInfo.BranchName}";
#endif
#if DEBUG
			labelVersion.Text += $" (Debug build: {CommitInfo.BranchName}@{CommitInfo.CommitHash})";
#endif
		}

		public void LoadPckFromFile(IEnumerable<string> filepaths)
		{
			foreach (string filepath in filepaths)
			{
				LoadPckFromFile(filepath);
			}
		}

		public void LoadPckFromFile(string filepath)
		{
			AddEditorPage(filepath);
		}

		private TabPage AddPage(string caption, Control control)
		{
			control.Dock = DockStyle.Fill;
            var page = new TabPage(caption);
            page.Controls.Add(control);
            tabControl.TabPages.Add(page);
            tabControl.SelectTab(page);
			return page;
        }

        private void AddEditorPage(PckFile pck)
		{
			var editor = new PckEditor();
			editor.Open(pck);
            AddPage("Unsaved pck", editor);
        }

        private void AddEditorPage(string filepath)
        {
			if (openFiles.ContainsKey(filepath))
			{
				tabControl.SelectTab(openFiles[filepath]);
				return;
			}

			var editor = new PckEditor();
			if (editor.Open(filepath, Settings.Default.UseLittleEndianAsDefault ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian))
			{
				var page = AddPage(Path.GetFileName(filepath), editor);
                openFiles.Add(filepath, page);
				return;
			}
            MessageBox.Show(string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		private bool TryGetEditor(TabPage page, out IPckEditor editor)
		{
			if (page.Controls[0] is IPckEditor _editor)
			{
				editor = _editor;
				return true;
			}
			editor = null;
			return false;
		}

        private bool TryGetEditor(out IPckEditor editor)
		{
			return TryGetEditor(tabControl.SelectedTab, out editor);
        }

        private void MainForm_Load(object sender, EventArgs e)
		{
			SettingsManager.RegisterPropertyChangedCallback<bool>(nameof(Settings.Default.UseLittleEndianAsDefault), state =>
			{
				if (TryGetEditor(out IPckEditor editor))
				{
                    //TODO
                    //editor.SetEndianness();	
                }
            });
			SettingsManager.RegisterPropertyChangedCallback(nameof(Settings.Default.LoadSubPcks), () =>
			{
				if (TryGetEditor(out IPckEditor editor))
				{
					editor.UpdateView();
				}
			});
		}

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach (TabPage tab in tabControl.TabPages)
			{
				if (TryGetEditor(tab, out IPckEditor editor))
				{
					editor.Close();
                    tabControl.TabPages.Remove(tab);
				}
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckFileExists = true;
				ofd.Multiselect = true;
				ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					LoadPckFromFile(ofd.FileNames);
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
			zeroFile.Properties.Add("PACKID", packId.ToString());
			zeroFile.Properties.Add("PACKVERSION", packVersion.ToString());

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
					var writer = new PckFileWriter(new PckFile(3, true),
					Settings.Default.UseLittleEndianAsDefault
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
                        Settings.Default.UseLittleEndianAsDefault
                            ? OMI.Endianness.LittleEndian
							: OMI.Endianness.BigEndian);
					writer.WriteToStream(ms);
					return ms.ToArray();
				});
			texturepackInfo.Properties.Add("PACKID", "0");
			texturepackInfo.Properties.Add("DATAPATH", $"{res}Data.pck");

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
			TextPrompt namePrompt = new TextPrompt();
			namePrompt.OKButtonText = "Ok";
			if (namePrompt.ShowDialog() == DialogResult.OK)
			{
				var currentPCK = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, true);
				AddEditorPage(currentPCK);
			}
		}

		private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateTexturePack packPrompt = new CreateTexturePack();
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
				var currentPCK = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
				AddEditorPage(currentPCK);
			}
		}

		private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateTexturePack packPrompt = new CreateTexturePack();
			if (packPrompt.ShowDialog() == DialogResult.OK)
			{
				var newPck = InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
				AddEditorPage(newPck);
			}
		}

		private void quickChangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TryGetEditor(out IPckEditor editor))
			{

				using AdvancedOptions advanced = new AdvancedOptions(editor.Pck);
				//TODO
				//advanced.littleEndian = LittleEndianCheckBox.Checked;
				if (advanced.ShowDialog() == DialogResult.OK)
				{
					// TODO
					//wasModified = true;
					//BuildMainTreeView();
				}
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TryGetEditor(out IPckEditor editor))
			{
				editor.Close();
                RemoveOpenFile();
				tabControl.TabPages.Remove(tabControl.SelectedTab);
			}
		}

        private void RemoveOpenFile()
        {
            var kv = openFiles.First((kv) => kv.Value == tabControl.SelectedTab);
            if (kv.Key != default && kv.Value != default)
            {
                openFiles.Remove(kv.Key);
            }
        }

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using CreditsForm info = new CreditsForm();
			info.ShowDialog();
		}

		private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			// Tries to extract a chosen pck file to a chosen destination
			try
			{
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
							var reader = new PckFileReader(
								Settings.Default.UseLittleEndianAsDefault
									? OMI.Endianness.LittleEndian
									: OMI.Endianness.BigEndian);
							pckfile = reader.FromStream(fs);
						}
						catch (OverflowException ex)
						{
							MessageBox.Show("Error", "Failed to open pck\nTry checking the 'Open/Save as Switch/Vita/PS4 pck' check box in the upper right corner.",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
							Debug.WriteLine(ex.Message);
						}
					}
					foreach (PckFile.FileData file in pckfile.Files)
					{
						string filepath = $"{sfd.SelectedPath}/{file.Filename}";
						Directory.CreateDirectory(filepath);
						File.WriteAllBytes(filepath, file.Data); // writes data to file
						//attempts to generate reimportable metadata file out of minefiles metadata
						string metaData = "";

						foreach (var entry in file.Properties)
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
			throw new NotImplementedException();
			//return Path.Combine(Path.GetDirectoryName(saveLocation), "Data");
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

        private void openPckCenterToolStripMenuItem_Click(object sender, EventArgs e)
		{
            MessageBox.Show("This feature is currently being reworked.", "Currently unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information);
#if false
			DateTime Begin = DateTime.Now;
			//pckCenter open = new pckCenter();
			PckCenterBeta open = new PckCenterBeta();
			open.Show();
			TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

			Debug.WriteLine("Completed in: " + duration);
#endif
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

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Pref setting = new Pref();
			setting.Show();
		}

		private void administrativeToolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PCK_Manager pckm = new PCK_Manager();
			pckm.Show();
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
			string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			LoadPckFromFile(filepaths);
		}

		private void OpenPck_DragLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void OpenPck_Click(object sender, EventArgs e)
		{
			openToolStripMenuItem_Click(sender, e);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (tabControl.SelectedTab is IPckEditor editor)
			{
				editor.Save();
			}
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (tabControl.SelectedTab is IPckEditor editor)
			{
				editor.SaveAs();
			}
		}

		private void addCustomPackIconToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string packID = "0";

			using NumericPrompt numericPrompt = new NumericPrompt(0);
			numericPrompt.Minimum = 0; // TODO: put min pack ID value (keeping this 0 just to be safe)
			numericPrompt.Maximum = int.MinValue; // TODO: put max pack ID value
			numericPrompt.ContextLabel.Text = "Please insert the desired Pack ID";
			numericPrompt.TextLabel.Text = "Pack ID";

            if (TryGetEditor(out IPckEditor editor))
			{
				DialogResult prompt = MessageBox.Show(this, 
					"Would you like to use the current PackID? You can enter any PackID if not.", 
					"", 
					MessageBoxButtons.YesNoCancel);

				switch (prompt)
				{
					case DialogResult.Yes:
						if (!editor.Pck.TryGetFile("0", PckFile.FileData.FileType.InfoFile, out PckFile.FileData file) ||
							string.IsNullOrEmpty(file.Properties.GetPropertyValue("PACKID")))
						{
							MessageBox.Show(this, 
								"No PackID is present in this PCK. " +
								"To avoid this error, ensure that the PCK has a proper PackID property on the \"0\" Info file before trying again.", 
								"Operation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}

						packID = file.Properties.GetPropertyValue("PACKID");
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
					default:
						MessageBox.Show(this, "Operation cancelled");
						return;
				}
			}
			else if (numericPrompt.ShowDialog(this) == DialogResult.OK) packID = numericPrompt.SelectedValue.ToString();
			else
			{
				MessageBox.Show(this, "Operation cancelled");
				return;
			}

			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Minecraft Archive|*.arc";
			if (fileDialog.ShowDialog(this) == DialogResult.OK)
			{
				var reader = new ARCFileReader();
				ConsoleArchive archive = reader.FromFile(fileDialog.FileName);

				fileDialog.Filter = "Pack Icon|*.png";
				if (fileDialog.ShowDialog(this) == DialogResult.OK)
				{
					string key = string.Format("Graphics\\PackGraphics\\{0}.png", packID);

					if (archive.Keys.Contains(key))
					{
						DialogResult prompt = MessageBox.Show(this,
							"This pack already has a pack icon present in the chosen file. Would you like to replace the pack icon?",
							"Icon already exists",
							MessageBoxButtons.YesNoCancel);
						switch (prompt)
						{
							case DialogResult.Yes:
								archive.Remove(key); // remove file so it can be injected
								break;
							case DialogResult.No:
							case DialogResult.Cancel:
							default:
								MessageBox.Show(this, "Operation cancelled");
								return;
						}
					}
					archive.Add(key, File.ReadAllBytes(fileDialog.FileName));
					var writer = new ARCFileWriter(archive);
					writer.WriteToFile(fileDialog.FileName);
					MessageBox.Show($"Successfully added {key} to Archive!", "Successfully Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}

		private void trelloBoardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://trello.com/b/0XLNOEbe/pck-studio");
		}

		private void openPckManagerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckManager ??= new PckManager();
			PckManager.BringToFront();
			PckManager.Focus();
			if (!PckManager.Visible)
			{
				PckManager.FormClosed += delegate { PckManager = null; };
				PckManager.Show(this);
			}
		}

		private async void wavBinkaToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "WAV files (*.wav)|*.wav",
                Title = "Please choose WAV files to convert to BINKA"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK)
				return;

            InProgressPrompt waitDiag = new InProgressPrompt();
			waitDiag.Show(this);
			
			int convertedCount = 0;

			Directory.CreateDirectory(ApplicationScope.DataCacher.CacheDirectory); // create directory in case it doesn't exist

			foreach (string waveFilepath in fileDialog.FileNames)
			{
				string[] a = Path.GetFileNameWithoutExtension(waveFilepath).Split(Path.GetInvalidFileNameChars());

                string songName = string.Join("_", a);
				songName = System.Text.RegularExpressions.Regex.Replace(songName, @"[^\u0000-\u007F]+", "_"); // Replace UTF characters
				string cacheSongFilepath = Path.Combine(ApplicationScope.DataCacher.CacheDirectory, songName + Path.GetExtension(waveFilepath));

				using (var reader = new NAudio.Wave.WaveFileReader(waveFilepath)) //read from original location
				{
					var newFormat = new NAudio.Wave.WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels);
					using (var conversionStream = new NAudio.Wave.WaveFormatConversionStream(newFormat, reader))
					{
						NAudio.Wave.WaveFileWriter.CreateWaveFile(cacheSongFilepath, conversionStream); //write to new location
					}
				}

				Cursor.Current = Cursors.WaitCursor;

				int exitCode = 0;
				await System.Threading.Tasks.Task.Run(() =>
				{
					exitCode = Binka.FromWav(cacheSongFilepath, Path.Combine(Path.GetDirectoryName(waveFilepath), Path.GetFileNameWithoutExtension(waveFilepath) + ".binka"), 4);
				});

				File.Delete(cacheSongFilepath); // delete cache files

				if (exitCode == 0)
					convertedCount++;
			}

			int fileCount = fileDialog.FileNames.Length;

            waitDiag.Close();
			waitDiag.Dispose();
			MessageBox.Show(this, $"Successfully converted {convertedCount}/{fileCount} file{(fileCount != 1 ? "s" : "")}", "Done!");
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
				BinkaConverter.ToWav(fileDialog.FileNames, new DirectoryInfo(Path.GetDirectoryName(fileDialog.FileName)));
		}

        private void fullBoxSupportToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
			if (TryGetEditor(out var editor))
			{
				editor.Pck.SetVersion(fullBoxSupportToolStripMenuItem.Checked);
			}
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
			var appSettings = new AppSettingsForm();
			appSettings.ShowDialog(this);
        }

        private void tabControl_PageClosing(object sender, PageClosingEventArgs e)
        {
            if (TryGetEditor(e.Page, out IPckEditor editor))
			{
				editor.Close();
				RemoveOpenFile();
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
			closeToolStripMenuItem.Visible = tabControl.SelectedIndex > 0;

			if (tabControl.TabPages.Count == 1)
			{
                RPC.SetPresence("An Open Source .PCK File Editor");
            }
        }
    }
}