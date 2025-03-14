using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OMI.Formats.Pck;
using OMI.Formats.GameRule;
using OMI.Formats.Languages;
using OMI.Workers.Pck;
using OMI.Workers.GameRule;
using OMI.Workers.Language;
using PckStudio.Properties;
using PckStudio.Forms;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Internal.Misc;
using PckStudio.Forms.Features;
using PckStudio.Extensions;
using PckStudio.Popups;
using PckStudio.External.API.Miles;
using PckStudio.Internal.App;
using PckStudio.Interfaces;
using PckStudio.Controls;
using PckStudio.Internal;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		private PckManager PckManager = null;

        private Dictionary<string, TabPage> openTabPages = new Dictionary<string, TabPage>();

		public MainForm()
		{
			InitializeComponent();

			Text = Application.ProductName;
			
			labelVersion.Text = $"{Application.ProductName}: {ApplicationScope.CurrentVersion}";

			ChangelogRichTextBox.Text = Resources.CHANGELOG;
			
			pckOpen.AllowDrop = true;
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

		internal void OpenNewPckTab(string caption, string identifier, PckFile pckFile, ISaveContext<PckFile> saveContext)
		{
            if (openTabPages.ContainsKey(identifier))
            {
                tabControl.SelectTab(openTabPages[identifier]);
                return;
            }
            var editor = new PckEditor(pckFile, saveContext);
            AddPage(caption, identifier, editor);
        }

        private void AddEditorPage(string caption, string identifier, PckFile pck, OMI.Endianness endianness = OMI.Endianness.BigEndian, ISaveContext<PckFile> saveContext = null)
        {
            saveContext ??= GetSaveContext("./new.pck", "PCK (Minecraft Console Package)", endianness);
            var editor = new PckEditor(pck, saveContext);
            AddPage(caption, identifier, editor);   
        }


        private PckFile ReadPck(string filePath, OMI.Endianness endianness)
        {
            var pckReader = new PckFileReader(endianness);
            return pckReader.FromFile(filePath);
        }

        private class PackInfo
        {
            public static readonly PackInfo Empty = new PackInfo(default, default, default);
            public bool IsValid { get; }
            public PckFile File { get; }
            public OMI.Endianness Endianness { get; }

            public bool AllowEndianSwap { get; }

            public PackInfo(PckFile file, OMI.Endianness endianness, bool allowEndianSwap)
            {
                File = file;
                Endianness = endianness;
                AllowEndianSwap = allowEndianSwap;
                IsValid = file is not null && Enum.IsDefined(typeof(OMI.Endianness), endianness);
            }
        }

        private bool TryOpenPck(string filepath, out PackInfo packInfo)
        {
            if (!File.Exists(filepath) || !filepath.EndsWith(".pck"))
            {
                packInfo = PackInfo.Empty;
                return false;
            }
            try
            {
                PckFile pckFile = ReadPck(filepath, OMI.Endianness.BigEndian);
                packInfo = new PackInfo(pckFile, OMI.Endianness.BigEndian, true);
                return packInfo.IsValid;
            }
            catch (OverflowException)
            {
                try
                {
                    // if failed, attempt again in the reverse. THEN throw an error if failed
                    PckFile pckFile = ReadPck(filepath, OMI.Endianness.LittleEndian);
                    packInfo = new PackInfo(pckFile, OMI.Endianness.LittleEndian, true);
                    return packInfo.IsValid;
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show(this, "Failed to open pck", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine(ex.Message);
                }
            }
            catch
            {
                MessageBox.Show(this, "Failed to open pck. There's two common reasons for this:\n" +
                    "1. The file is audio/music cues PCK file. Please use the specialized editor while inside of a pck file.\n" +
                    "2. We're aware of an issue where a pck file might fail to load because it contains multiple entries with the same path.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            packInfo = PackInfo.Empty;
            return false;
        }

        private void AddEditorPage(string filepath)
        {
            if (openTabPages.ContainsKey(filepath))
            {
                tabControl.SelectTab(openTabPages[filepath]);
                return;
            }
            SaveToRecentFiles(filepath);

            if (TryOpenPck(filepath, out PackInfo packInfo))
            {
                ISaveContext<PckFile> saveContext = GetSaveContext(filepath, "PCK (Minecraft Console Package)", packInfo.Endianness);
                var editor = new PckEditor(packInfo.File, saveContext);
                TabPage page = AddPage(Path.GetFileName(filepath), filepath, editor);
                return;
            }
            MessageBox.Show(string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private ISaveContext<PckFile> GetSaveContext(string filepath, string description, OMI.Endianness endianness)
        {
            return new DelegatedFileSaveContext<PckFile>(filepath, false, new FileDialogFilter(description, "*"+Path.GetExtension(filepath)),(pck, stream) => new PckFileWriter(pck, endianness).WriteToStream(stream));
        }

        private TabPage AddPage(string caption, string identifier, Control control)
        {
            control.Dock = DockStyle.Fill;
            var page = new TabPage(caption);
            page.Controls.Add(control);
            tabControl.TabPages.Add(page);
            tabControl.SelectTab(page);
            if (!openTabPages.ContainsKey(identifier))
                openTabPages.Add(identifier, page);
            return page;
        }

        private bool TryGetEditor(TabPage page, out IEditor<PckFile> editor)
        {
            if (page.Controls[0] is IEditor<PckFile> outEditor)
            {
                editor = outEditor;
                return true;
            }
            editor = default;
            return false;
        }

        private bool TryGetCurrentEditor(out IEditor<PckFile> editor) => TryGetEditor(tabControl.SelectedTab, out editor);

        private void MainForm_Load(object sender, EventArgs e)
		{
			//SettingsManager.Default.RegisterPropertyChangedCallback(nameof(Settings.Default.LoadSubPcks), () =>
			//{
   //             if (TryGetCurrentEditor(out IEditor<PckFile> editor))
   //             {
   //                 editor.UpdateView();
   //             }
   //         });

			LoadRecentFileList();
		}

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PckManager?.Close();
            closeAllToolStripMenuItem_Click(sender, e);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                CloseTab(tabControl.TabPages, tab);
            }
            closeAllToolStripMenuItem.Visible = false;
        }

        private void CloseTab(TabControl.TabPageCollection collection, TabPage page)
        {
            if (TryGetEditor(page, out IEditor<PckFile> editor))
            {
                editor.Close();
                RemoveOpenFile(page);
                collection.Remove(page);
            }
        }

        private void RemoveOpenFile()
        {
            RemoveOpenFile(tabControl.SelectedTab);
        }

        private void RemoveOpenFile(TabPage page)
        {
            KeyValuePair<string, TabPage> kv = openTabPages.FirstOrDefault((kv) => kv.Value == page);
            if (kv.Key != default)
            {
                openTabPages.Remove(kv.Key);
            }
        }

        private void LoadRecentFileList()
        {
            Settings.Default.RecentFiles ??= new System.Collections.Specialized.StringCollection();
            recentlyOpenToolStripMenuItem.DropDownItems.Clear();
			for (int i = 0; i < Settings.Default.RecentFiles.Count && i < 5; i++)
			{
				string filepath = Settings.Default.RecentFiles[i];
				if (!string.IsNullOrWhiteSpace(filepath) && File.Exists(filepath))
				{
					string displayFilepath = Regex.Replace(filepath, @"([A-Z]{1}\:\\[Uu]sers\\)([^\\]*\\)(.*)", "~\\$3");
                    ToolStripItem item = recentlyOpenToolStripMenuItem.DropDownItems.Add(displayFilepath, null, HandleOpenFile);
					item.Tag = filepath;
				}
            }
			if (recentlyOpenToolStripMenuItem.DropDownItems.Count > 0)
			{
				void clearRecentFileList(object sender, EventArgs e)
				{
					Settings.Default.RecentFiles.Clear();
					LoadRecentFileList();
        }
				recentlyOpenToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
				recentlyOpenToolStripMenuItem.DropDownItems.Add("Clear", null, clearRecentFileList);
			}
        }

		private void HandleOpenFile(object sender, EventArgs e)
		{
			if (sender is ToolStripItem menuItem && menuItem.Tag is string filepath)
			{
				if (!File.Exists(filepath))
				{
					menuItem.Available = false;
					return;
				}				
				LoadPckFromFile(filepath);
        }
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

        private void tabControl_PageClosing(object sender, PageClosingEventArgs e)
        {
            if (TryGetEditor(e.Page, out IEditor<PckFile> editor))
            {
                editor.Close();
                RemoveOpenFile();
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            closeToolStripMenuItem.Visible = tabControl.SelectedIndex > 0;
            closeAllToolStripMenuItem.Visible = tabControl.SelectedIndex == 0 && tabControl.TabCount > 1;
            saveToolStripMenuItem.Visible = tabControl.SelectedIndex > 0;
            saveAsToolStripMenuItem.Visible = tabControl.SelectedIndex > 0;

            if (tabControl.SelectedIndex == 0)
            {
                RPC.SetPresence("An Open Source .PCK File Editor");
            }
        }

        private PckFile InitializePack(int packId, int packVersion, string packName, bool createSkinsPCK)
		{
			var pack = new PckFile(3);

            PckAsset zeroAsset = pack.CreateNewAsset("0", PckAssetType.InfoFile);
			zeroAsset.AddProperty("PACKID", packId);
			zeroAsset.AddProperty("PACKVERSION", packVersion);

			var locFile = new LOCFile();
			locFile.InitializeDefault(packName);
			pack.CreateNewAsset("localisation.loc", PckAssetType.LocalisationFile, new LOCFileWriter(locFile, 2));

			pack.CreateNewAssetIf(createSkinsPCK, "Skins.pck", PckAssetType.SkinDataFile, new PckFileWriter(new PckFile(3, true), OMI.Endianness.BigEndian));

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

			texturepackInfoAsset.SetData(new PckFileWriter(infoPCK, OMI.Endianness.BigEndian));

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
            TextPrompt namePrompt = new TextPrompt();
            namePrompt.OKButtonText = "Ok";
            if (namePrompt.ShowDialog(this) == DialogResult.OK)
            {
                PckFile skinPck = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, true);
                AddEditorPage("Unsaved skin pack", "Unsaved skin pack", skinPck);
            }
        }

        private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var packPrompt = new CreateTexturePackPrompt();
            if (packPrompt.ShowDialog() == DialogResult.OK)
            {
                PckFile texturePackPck = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes, packPrompt.CreateSkinsPck);
                AddEditorPage("Unsaved texture pack", "Unsaved texture pack", texturePackPck);
            }
        }

        private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var packPrompt = new CreateTexturePackPrompt();
            if (packPrompt.ShowDialog() == DialogResult.OK)
            {
                PckFile mashUpPck = InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
                AddEditorPage("Unsaved mash-up pack", "Unsaved mash-up pack", mashUpPck);
            }
        }

		private void quickChangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (TryGetCurrentEditor(out IEditor<PckFile> editor))
            {
                using AdvancedOptions advanced = new AdvancedOptions(editor.EditorValue);
                // TODO: 
                //advanced.IsLittleEndian = LittleEndianCheckBox.Checked;
                if (advanced.ShowDialog() == DialogResult.OK)
                {
                    editor.UpdateView();
                }
            }
        }

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            CloseTab(tabControl.TabPages, tabControl.SelectedTab);
        }

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using ContributorsForm info = new ContributorsForm();
			info.ShowDialog(this);
		}

		[Obsolete("ReMove this")]
		public string GetDataPath()
		{
			return "";
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
			if (e.Data.GetData(DataFormats.FileDrop, false) is string[] filepaths)
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
            if (TryGetCurrentEditor(out IEditor<PckFile> editor))
            {
                editor.Save();
            }
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (TryGetCurrentEditor(out IEditor<PckFile> editor))
            {
                editor.SaveAs();
            }
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
            if (TryGetCurrentEditor(out IEditor<PckFile> editor))
            {
                editor.EditorValue.SetVersion(fullBoxSupportToolStripMenuItem.Checked);
            }
        }

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var appSettings = new AppSettingsForm();
			appSettings.ShowDialog(this);
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
	}
}