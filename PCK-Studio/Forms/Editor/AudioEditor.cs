﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MetroFramework.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.PCK;
using PckStudio.Forms.Additional_Popups.Audio;

// Audio Editor by MattNL
// additional work and optimization by Miku-666

namespace PckStudio.Forms.Editor
{
	public partial class AudioEditor : Form
	{
		public string defaultType = "yes";
		Classes.Bink BINK = new Classes.Bink();
		PCKAudioFile audioFile = null;
		PCKFile.FileData audioPCK;
		LOCFile loc;
		bool _isLittleEndian = false;
        MainForm parent = null;

        public static readonly List<string> Categories = new List<string>
		{
			"Overworld",
			"Nether",
			"End",
			"Creative",
			"Menu/Loading",
			"Battle",
			"Tumble",
			"Glide",
			"Unused?"
		};

		private string GetCategoryFromId(PCKAudioFile.AudioCategory.EAudioType categoryId)
			=> categoryId >= PCKAudioFile.AudioCategory.EAudioType.Overworld &&
				categoryId <= PCKAudioFile.AudioCategory.EAudioType.Unused
				? Categories[(int)categoryId]
				: "Not valid";

		private PCKAudioFile.AudioCategory.EAudioType GetCategoryId(string category)
		{
			return (PCKAudioFile.AudioCategory.EAudioType)Categories.IndexOf(category);
		}

		public AudioEditor(PCKFile.FileData file, LOCFile locFile, bool isLittleEndian)
		{
			InitializeComponent();
			loc = locFile;
			_isLittleEndian = isLittleEndian;

			BINK.SetUpBinka();

			audioPCK = file;
			using (var stream = new MemoryStream(file.data))
			{
				audioFile = PCKAudioFileReader.Read(stream, isLittleEndian);
			}

			foreach (var category in audioFile.Categories)
			{
				if (category.Name == "include_overworld" &&
					category.audioType == PCKAudioFile.AudioCategory.EAudioType.Creative &&
					audioFile.TryGetCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld, out PCKAudioFile.AudioCategory overworldCategory))
				{
					foreach (var name in category.SongNames.ToList())
					{
						if (overworldCategory.SongNames.Contains(name))
							category.SongNames.Remove(name);
					}
					playOverworldInCreative.Checked = true;
				}

				TreeNode treeNode = new TreeNode(GetCategoryFromId(category.audioType), (int)category.audioType, (int)category.audioType);
				treeNode.Tag = category;
				treeView1.Nodes.Add(treeNode);
			}
			playOverworldInCreative.Enabled = audioFile.HasCategory(PCKAudioFile.AudioCategory.EAudioType.Creative);
		}

		private void AudioEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Clean up is throwing an error of some kind? FreeLibrary maybe??
			//BINK.CleanUpBinka();
		}

		private void verifyFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null || treeView2.SelectedNode == null) return;
			var entry = treeView2.SelectedNode;

			if (!parent.CreateDataFolder()) return;
			string FileName = Path.Combine(parent.GetDataPath(), entry.Text + ".binka");

			if (File.Exists(FileName)) MessageBox.Show("\"" + entry.Text + ".binka\" exists in the \"Data\" folder", "File found");
			else MessageBox.Show("\"" + entry.Text + ".binka\" does not exist in the \"Data\" folder. The game will crash when attempting to load this track.", "File missing");
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeView2.Nodes.Clear();
			if (e.Node.Tag is PCKAudioFile.AudioCategory category)
				foreach (var name in category.SongNames)
				{
					treeView2.Nodes.Add(name);
				}
			if (treeView2.Nodes.Count > 0) treeView2.SelectedNode = treeView2.Nodes[0];
		}

		private void addCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			string[] avalible = Categories.FindAll(str => !audioFile.HasCategory(GetCategoryId(str))).ToArray();
			if (avalible.Length > 0)
			{
				using AddCategory add = new AddCategory(avalible);
				if (add.ShowDialog() == DialogResult.OK)
					audioFile.AddCategory(GetCategoryId(add.Category));
				else return;

				var category = audioFile.GetCategory(GetCategoryId(add.Category));
				TreeNode treeNode = new TreeNode(GetCategoryFromId(category.audioType), (int)category.audioType, (int)category.audioType);
				treeNode.Tag = category;
				treeView1.Nodes.Add(treeNode);
			}
			else
			{
				MessageBox.Show("All possible categories are used", "There are no more categories that could be added");
			}
		}

		private void addEntryMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is TreeNode t && t.Tag is PCKAudioFile.AudioCategory)
			{
				if (!parent.CreateDataFolder()) return;

				OpenFileDialog ofn = new OpenFileDialog();
				ofn.Multiselect = true;
				ofn.Filter = "Supported audio files (*.binka,*.wav)|*.binka;*.wav";
				ofn.Title = "Please choose WAV or BINKA files to add to your pack";
				ofn.ShowDialog();
				ofn.Dispose();
				if (string.IsNullOrEmpty(ofn.FileName)) return; // Return if name is null or if the user cancels

				ProcessEntries(ofn.FileNames);
			}
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is TreeNode main &&
				audioFile.RemoveCategory(GetCategoryId(treeView1.SelectedNode.Text)))
			{
				treeView2.Nodes.Clear();
				main.Remove();
			}
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				removeCategoryStripMenuItem_Click(sender, e);
		}

		public void treeView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				removeEntryMenuItem_Click(sender, e);
		}

		private void removeEntryMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null && treeView1.SelectedNode.Tag is PCKAudioFile.AudioCategory category)
			{
				category.SongNames.Remove(treeView2.SelectedNode.Text);
				treeView2.SelectedNode.Remove();
			}
		}

		async void ProcessEntries(string[] FileList)
		{
			foreach (string file in FileList)
			{
				if (Path.GetExtension(file) == ".binka" || Path.GetExtension(file) == ".wav")
				{
					string new_loc = Path.Combine(parent.GetDataPath(), Path.GetFileNameWithoutExtension(file) + ".binka");
					bool duplicate_song = false; // To handle if a file already in the pack is dropped back in
					if (File.Exists(new_loc))
					{
						duplicate_song = File.ReadAllBytes(file) == File.ReadAllBytes(new_loc);
						if (duplicate_song)
						{
							DialogResult user_prompt = MessageBox.Show("\"" + Path.GetFileNameWithoutExtension(file) + ".binka\" already exists. Continuing will replace the existing file. Are you sure you want to continue moving the file? By pressing no, the song entry will be added without moving the file. You can also cancel this operation and all files in queue.", "File already exists", MessageBoxButtons.YesNoCancel);
							while (user_prompt == DialogResult.None) ; // Stops the editor from adding or processing the file until the user had made their choice
							if (user_prompt == DialogResult.Cancel)
							{
								break;
							}
							else if (user_prompt == DialogResult.No) continue;
						}
					}
					
					if (Path.GetExtension(file) == ".wav") // Convert Wave to BINKA
					{
						Cursor.Current = Cursors.WaitCursor;
						PleaseWait waitDiag = new PleaseWait();
						waitDiag.Show(this);

						await Task.Run(() =>
						{
							BINK.WavToBinka(file, new_loc, (int)compressionUpDown.Value);
						});

						waitDiag.Close();
						waitDiag.Dispose();
						Cursor.Current = Cursors.Default;

						if (BINK.temp_error_code != 0) continue;
					}
					else if (!duplicate_song)
					{
						Console.WriteLine(Path.GetFileName(file));
						File.Delete(Path.Combine(parent.GetDataPath(), Path.GetFileName(file)));
						File.Copy(file, Path.Combine(parent.GetDataPath(), Path.GetFileName(file)));
					}

					var songName = Path.GetFileNameWithoutExtension(file);
					if (treeView1.SelectedNode is TreeNode t && t.Tag is PCKAudioFile.AudioCategory category)
					{
						category.SongNames.Add(songName);
						treeView2.Nodes.Add(songName);
					}
				}
			}
		}

		private void Binka_DragDrop(object sender, DragEventArgs e)
		{
			//MessageBox.Show((Owner.Owner as MainForm).saveLocation);
			// Gets the MainForm so we can access the Save Location
			if (treeView1.SelectedNode != null)
			{
				if (!parent.CreateDataFolder()) return;

				ProcessEntries((string[])e.Data.GetData(DataFormats.FileDrop, false));
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (!audioFile.HasCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld) ||
			   !audioFile.HasCategory(PCKAudioFile.AudioCategory.EAudioType.Nether) ||
			   !audioFile.HasCategory(PCKAudioFile.AudioCategory.EAudioType.End))
			{
				MessageBox.Show("Your changes were not saved. The game will crash when loading your pack if the Overworld, Nether and End categories don't all exist with at least one valid song.", "Mandatory Categories Missing");
				return;
			}

			PCKAudioFile.AudioCategory overworldCategory = audioFile.GetCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld);
			foreach (var category in audioFile.Categories)
			{
				category.Name = "";
				if (playOverworldInCreative.Checked && category.audioType == PCKAudioFile.AudioCategory.EAudioType.Creative)
				{
					foreach (var name in overworldCategory.SongNames)
					{
						if (!category.SongNames.Contains(name))
						{
							category.SongNames.Add(name);
							Console.WriteLine(name);
						}
					}
					category.Name = "include_overworld";
				}
			}

			bool emptyCat = false;

			if (emptyCat)
			{
				MessageBox.Show("The game will crash upon loading your pack if a category is empty", "Empty Category");
				return;
			}

			using (var stream = new MemoryStream())
			{
				PCKAudioFileWriter.Write(stream, audioFile, _isLittleEndian);
				audioPCK.SetData(stream.ToArray());
			}
			DialogResult = DialogResult.OK;
		}

		private void treeView2_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.All;
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Simply drag and drop BINKA or WAV audio files into the right tree to add them to the category selected on the left tree.\n\n" +
				"The \"Menu\" category will only play once when loading the pack, and never again.\n\n" +
				"The \"Creative\" category will only play songs listed in that category, and unlike other editions of Minecraft, will NOT play songs from the Overworld category. You can fix this by clicking the checkbox found at the top of the form.\n\n" +
				"The mini game categories will only play if you have your pack loaded in those mini games.\n\n" +
				"You can edit the credits for the PCK in the Credits editor! No more managing credit IDs!\n\n" +
				"You can modify and create PSVita and PS4 audio pcks by clicking \"PS4/Vita\" in the \"Create -> Audio.pck\" context menu", "Help");
		}

		private void creditsEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var credits = audioFile.GetCreditsString();
			using (CreditsEditor prompt = new CreditsEditor(credits))
				if (prompt.ShowDialog() == DialogResult.OK)
					audioFile.SetCredits(prompt.Credits.Split('\n'));
		}

		private void deleteUnusedBINKAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dr = MessageBox.Show("This will delete all unused BINKA songs in the Data directory. This cannot be undone. Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNo);
			if (dr != DialogResult.Yes) return;
			var totalSongList = new List<string>();
			foreach (string song in audioFile.Categories.SelectMany(cat => cat.SongNames))
			{
				Console.WriteLine(song);
				totalSongList.Add(song);
			}

			if (!parent.CreateDataFolder()) return;
			int totalDeleted = 0;
			foreach (string song in Directory.GetFiles(parent.GetDataPath(), "*.binka"))
			{
				if (!totalSongList.Contains(Path.GetFileNameWithoutExtension(song)))
				{
					Console.WriteLine("Deleted " + song);
					try
					{
						File.Delete(song);
					}
					catch (IOException ex)
					{
						Console.WriteLine(ex.Message);
						continue;
					}
					totalDeleted++;
				}
			}
			MessageBox.Show("Successfully deleted " + totalDeleted + " files", "Done");
		}

		private void howToAddSongsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Right click the right window and press \"Add Entry\" or drag and drop a valid WAV file into the editor's right window. You can also drop other BINKA files, either from the main game or using a tool like BinkMan. The editor will automatically put the song in the Data folder for you.", "How to add a song");
		}

		private void whatIsEachCategoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Categories are pretty self explanatory. The game controls when each category should play.\n" +
				"\nGAMEPLAY - Plays in the specified dimensions and game modes.\n" +
				"-Overworld: Plays in survival mode and in Creative if no songs are set\n" +
				"-Nether: Nothing special to note.\n" +
				"-End: Prioritizes the final track when the dragon is alive.\n" +
				"-Creative: Does not play survival tracks unless they're included.\n" +
				"-Menu: Plays on the title screen and only once when the pack is loading. Perfect for intro songs.\n" +
				"\nMINI GAMES - Will only play if you change the map grf files to load your pack and set the ThemeID to 0 for Vanilla maps.\n" +
				"-Battle: Plays in the Battle Mini Game.\n" +
				"-Tumble: Plays in the Tumble Mini Game.\n" +
				"-Glide: Plays in the Glide Mini Game.\n", 
				"What is each category?");
		}

		private void howToEditCreditsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Click Tools -> Credits Editor. This will allow you to edit all the credits easily in the pack easily. Only supports English credits at the moment. ","How to edit credits?");
		}

		private void optimizeDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Click Tools -> Delete Unused BINKA files. This will clean your folder of any unused songs.", "How to optimize the Data folder");
		}

		private void BINKACompressionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("The numerical up/down control is responsible for the level of compression used when converting WAV files. The default is 4, which was commonly used by 4J for the game's files.","BINKA Compression Level");
		}

		private void openDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!parent.CreateDataFolder()) return;
			Process.Start("explorer.exe", parent.GetDataPath());
		}

		private void AudioEditor_Shown(object sender, EventArgs e)
		{
			if (Owner.Owner is MainForm p) parent = p;
			else Close();
		}

		private async void bulkReplaceExistingFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!parent.CreateDataFolder()) return;

			OpenFileDialog ofn = new OpenFileDialog();
			ofn.Multiselect = true;
			ofn.Filter = "Supported audio files (*.binka,*.wav)|*.binka;*.wav";
			ofn.Title = "Please choose WAV or BINKA files to replace existing track files";
			ofn.ShowDialog();
			ofn.Dispose();
			if (string.IsNullOrEmpty(ofn.FileName)) return; // Return if name is null or if the user cancels

			var totalSongList = new List<string>();
			foreach (string song in audioFile.Categories.SelectMany(cat => cat.SongNames))
			{
				totalSongList.Add(song);
			}

			foreach (string file in ofn.FileNames)
			{
				string song_name = Path.GetFileNameWithoutExtension(file);
				string file_ext = Path.GetExtension(file).ToLower();
				string new_loc = Path.Combine(parent.GetDataPath(), Path.GetFileNameWithoutExtension(file) + ".binka");
				if (!totalSongList.Contains(song_name) || file == new_loc) continue;

				Console.WriteLine(file);
				File.Delete(new_loc);

				if (file_ext == ".wav") // Convert Wave to BINKA
				{
					Cursor.Current = Cursors.WaitCursor;
					PleaseWait waitDiag = new PleaseWait();
					waitDiag.Show(this);

					await Task.Run(() =>
					{
						BINK.WavToBinka(file, new_loc, (int)compressionUpDown.Value);
					});

					waitDiag.Close();
					waitDiag.Dispose();
					Cursor.Current = Cursors.Default;

					if (BINK.temp_error_code != 0) continue;
				}
				else if(file_ext == ".binka") File.Copy(file, Path.Combine(parent.GetDataPath(), Path.GetFileName(file)));
			}
		}

		private void convertToWAVToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null && treeView1.SelectedNode.Tag is PCKAudioFile.AudioCategory)
			{
				BINK.BinkaToWav(Path.Combine(parent.GetDataPath(), treeView2.SelectedNode.Text + ".binka"), Path.Combine(parent.GetDataPath()));
			}
		}
	}
}
