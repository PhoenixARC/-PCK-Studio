﻿using System;
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
using System.Windows.Forms.Design;
using PckStudio.Controls;

namespace PckStudio
{
	public partial class MainForm : MetroFramework.Forms.MetroForm
	{
		private PckManager PckManager = null;

        private Dictionary<string, TabPage> openFiles = new Dictionary<string, TabPage>();

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

        private void AddEditorPage(PckFile pck)
        {
            var editor = new PckEditor();
            editor.Open(pck);
            AddPage("Unsaved pck", editor);
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

        private void AddEditorPage(string filepath)
        {
            if (openFiles.ContainsKey(filepath))
            {
                tabControl.SelectTab(openFiles[filepath]);
                return;
            }
            SaveToRecentFiles(filepath);

            var editor = new PckEditor();
            if (editor.Open(filepath, Settings.Default.UseLittleEndianAsDefault ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian))
            {
                TabPage page = AddPage(Path.GetFileName(filepath), editor);
                openFiles.Add(filepath, page);
                return;
            }
            MessageBox.Show(string.Format("Failed to load {0}", Path.GetFileName(filepath)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool TryGetEditor(out IEditor<PckFile> editor)
        {
            return TryGetEditor(tabControl.SelectedTab, out editor);
        }

        // TODO: decide on how to handle embedded pck files
   //     private void HandleInnerPckFile(PckAsset asset)
   //     {
			//if (Settings.Default.LoadSubPcks &&
			//	(asset.Type == PckAssetType.SkinDataFile || asset.Type == PckAssetType.TexturePackInfoFile) &&
			//	asset.Size > 0 && treeViewMain.SelectedNode.Nodes.Count == 0)
			//{
			//	try
			//	{
			//		PckFile subPCKfile = asset.GetData(new PckFileReader(LittleEndianCheckBox.Checked ? OMI.Endianness.LittleEndian : OMI.Endianness.BigEndian));
			//		BuildPckTreeView(treeViewMain.SelectedNode.Nodes, subPCKfile);
			//		treeViewMain.SelectedNode.ExpandAll();
   //             }
			//	catch (OverflowException ex)
			//	{
			//		MessageBox.Show(this, "Failed to open pck\n" +
			//			"Try checking the 'Open/Save as Switch/Vita/PS4/Xbox One pck' checkbox in the upper right corner.",
			//			"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//		Debug.WriteLine(ex.Message);
			//	}
			//	return;
			//}
			//treeViewMain.SelectedNode.Nodes.Clear();
			//treeViewMain.SelectedNode.Collapse();
   //     }

		private void MainForm_Load(object sender, EventArgs e)
		{
			SettingsManager.Default.RegisterPropertyChangedCallback<bool>(nameof(Settings.Default.UseLittleEndianAsDefault), state =>
			{
				LittleEndianCheckBox.Checked = state;
			});
			SettingsManager.Default.RegisterPropertyChangedCallback(nameof(Settings.Default.LoadSubPcks), () =>
			{
                if (TryGetEditor(out IEditor<PckFile> editor))
                {
                    editor.UpdateView();
                }
            });

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
            KeyValuePair<string, TabPage> kv = openFiles.FirstOrDefault((kv) => kv.Value == page);
            if (kv.Key != null && kv.Value != null)
            {
                openFiles.Remove(kv.Key);
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
            saveAsToolStripMenuItem.Visible = tabControl.SelectedIndex > 0;
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
            TextPrompt namePrompt = new TextPrompt();
            namePrompt.OKButtonText = "Ok";
            if (namePrompt.ShowDialog(this) == DialogResult.OK)
            {
                PckFile skinPck = InitializePack(new Random().Next(8000, int.MaxValue), 0, namePrompt.NewText, true);
                AddEditorPage(skinPck);
            }
        }

        private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var packPrompt = new CreateTexturePackPrompt();
            if (packPrompt.ShowDialog() == DialogResult.OK)
            {
                PckFile texturePackPck = InitializeTexturePack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes, packPrompt.CreateSkinsPck);
                AddEditorPage(texturePackPck);
            }
        }

        private void mashUpPackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var packPrompt = new CreateTexturePackPrompt();
            if (packPrompt.ShowDialog() == DialogResult.OK)
            {
                PckFile mashUpPck = InitializeMashUpPack(new Random().Next(8000, int.MaxValue), 0, packPrompt.PackName, packPrompt.PackRes);
                AddEditorPage(mashUpPck);
            }
        }

		private void quickChangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (TryGetEditor(out IEditor<PckFile> editor))
            {
                using AdvancedOptions advanced = new AdvancedOptions(editor.Value);
                advanced.IsLittleEndian = LittleEndianCheckBox.Checked;
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

		[Obsolete("Move this")]
		public string GetDataPath()
		{
			if (TryGetEditor(out IEditor<PckFile> editor))
			{
                return Path.Combine(Path.GetDirectoryName(editor.SavePath), "Data");
            }
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
            if (TryGetEditor(out IEditor<PckFile> editor))
            {
                editor.Save();
            }
        }

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (TryGetEditor(out IEditor<PckFile> editor))
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
            if (TryGetEditor(out IEditor<PckFile> editor))
            {
                editor.Value.SetVersion(fullBoxSupportToolStripMenuItem.Checked);
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