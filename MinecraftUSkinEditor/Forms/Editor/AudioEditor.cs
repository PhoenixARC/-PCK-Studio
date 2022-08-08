using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MetroFramework.Forms;
using System.Collections;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO.PCK;
using PckStudio.Forms.Additional_Popups.Audio;

// Audio Editor by MattNL
// additional work and optimization by Miku-666

namespace PckStudio.Forms.Editor
{
	public partial class AudioEditor : MetroForm
	{
		public bool saved = false;
		public string defaultType = "yes";
		string tempDir = "";
		PCKAudioFile audioFile = null;
		PCKFile.FileData audioPCK;
		LOCFile loc;
		bool _isLittleEndian = false;

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

		public static PCKFile.FileData CreateAudioPck(bool isLittle)
		{
			// create actual valid pck file structure
			PCKAudioFile audioPck = new PCKAudioFile();
			audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld);
			audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.Nether);
			audioPck.AddCategory(PCKAudioFile.AudioCategory.EAudioType.End);
			PCKFile.FileData pckFileData = new PCKFile.FileData("audio.pck", 8);
			using (var stream = new MemoryStream())
			{
				PCKAudioFileWriter.Write(stream, audioPck, isLittle);
				pckFileData.SetData(stream.ToArray());
			}
			return pckFileData;
		}

		/// <summary>
		/// Overload that creates a new audio.pck file
		/// </summary>
		public AudioEditor(LOCFile locFile, bool isLittleEndian) : this(CreateAudioPck(isLittleEndian), locFile, isLittleEndian)
        {
        }

		public AudioEditor(PCKFile.FileData file, LOCFile locFile, bool isLittleEndian)
		{
			InitializeComponent();
			loc = locFile;
			tempDir = Path.Combine(Directory.GetCurrentDirectory(), "temp");
			_isLittleEndian = isLittleEndian;
			try
			{
				handleUtilFiles();
				//library = LoadLibrary(Path.Combine(tempDir, "mss32.dll"));
			}
			catch (IOException ex)
			{
				MessageBox.Show("Failed to get Binka conversion files", "Exception thrown");
				Close();
			}

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
					foreach(var name in category.SongNames.ToList())
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

		// https://stackoverflow.com/a/25064568 by Alik Khilazhev -MattNL
		private void ExtractResource(string resName, string fName)
		{
			object ob = Properties.Resources.ResourceManager.GetObject(resName);
			byte[] myResBytes = (byte[])ob;
			using (FileStream fsDst = new FileStream(fName, FileMode.CreateNew, FileAccess.Write))
			{
				byte[] bytes = myResBytes;
				fsDst.Write(bytes, 0, bytes.Length);
				fsDst.Close();
				fsDst.Dispose();
			}
		}

		private void handleUtilFiles(bool extractFiles = true)
		{
			//string asiPath = Path.Combine(tempDir, "binkawin.asi");
			//string mssPath = Path.Combine(tempDir, "mss32.dll");
			string encoderPath = Path.Combine(tempDir, "binka_encode.exe");

			// Deletes files so that System.IO exceptions are avoided
			//if (File.Exists(asiPath)) File.Delete(asiPath);
			//if (File.Exists(mssPath)) File.Delete(mssPath);
			if (File.Exists(encoderPath)) File.Delete(encoderPath);
			if (Directory.Exists(tempDir)) Directory.Delete(tempDir);

			if (extractFiles)
			{
				Directory.CreateDirectory(tempDir);
				ExtractResource("binka_encode", encoderPath);
				//ExtractResource("mss32", mssPath);
				//ExtractResource("binkawin", asiPath);
			}
		}

		private void AudioEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			//FreeLibrary(library);
			handleUtilFiles(false);
		}

		private void verifyFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Tag == null || treeView2.SelectedNode.Tag == null) return;
			var entry = treeView2.SelectedNode;

			MainForm parent = Owner.Owner as MainForm; // Gets the MainForm so we can access the Save Location
			string DataDirectory = Path.Combine(Path.GetDirectoryName(parent.saveLocation), "Data");
			string FileName = Path.Combine(DataDirectory, entry.Text + ".binka");
			Console.WriteLine(FileName);
			if (!Directory.Exists(DataDirectory))
			{
				MessageBox.Show("There is not a \"Data\" folder present in the pack folder", "Folder missing");
				return;
			}
			if (File.Exists(FileName)) MessageBox.Show("\"" + entry.Text + ".binka\" exists in the \"Data\" folder", "File found");
			else MessageBox.Show("\"" + entry.Text + ".binka\" does not exist in the \"Data\" folder", "File missing");
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
			string[] avalible = Categories.FindAll(str => !audioFile.HasCategory(GetCategoryId(str)) ).ToArray();
			if (avalible.Length > 0)
			{ 
				using addCategory add = new addCategory(avalible);
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
			if (treeView1.SelectedNode is TreeNode t && t.Tag is PCKAudioFile.AudioCategory &&
				// Gets the MainForm so we can access the Save Location
				Owner.Owner is MainForm parent)
			{
				string DataDirectory = Path.Combine(Path.GetDirectoryName(parent.saveLocation), "Data");
				if (!Directory.Exists(DataDirectory))
				{
					MessageBox.Show("There is not a \"Data\" folder present in the pack folder", "Folder missing");
					return;
				}

				OpenFileDialog ofn = new OpenFileDialog();
				ofn.Multiselect = true;
				ofn.Filter = "BINKA files (*.binka)|*.binka|WAV files (*.wav)|*.wav";
				ofn.Title = "Please choose WAV or BINKA files to add to your pack";
				ofn.ShowDialog();
				ofn.Dispose();
				if (string.IsNullOrEmpty(ofn.FileName)) return; // Return if name is null or if the user cancels

				ProcessEntries(ofn.FileNames, DataDirectory);
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

		async void ProcessEntries(string[] FileList, string DataDirectory)
		{
			foreach (string file in FileList)
			{
				if (Path.GetExtension(file) == ".binka" || Path.GetExtension(file) == ".wav")
				{
					string new_loc = Path.Combine(DataDirectory, Path.GetFileNameWithoutExtension(file) + ".binka");
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
						pleaseWait waitDiag = new pleaseWait();
						waitDiag.Show(this);

						int error_code = 0;
						await Task.Run(() =>
						{
							var process = Process.Start(new ProcessStartInfo
                            {
								FileName = Path.Combine(tempDir, "binka_encode.exe"),
								Arguments = $"\"{file}\" \"{new_loc}\"",
								UseShellExecute = true,
								CreateNoWindow = true,
								WindowStyle = ProcessWindowStyle.Hidden
							});
							process.Start();
							process.WaitForExit();
						});

						waitDiag.Close();
						waitDiag.Dispose();
						Cursor.Current = Cursors.Default;

						if (error_code != 0) continue;
					}
					else if (!duplicate_song)
					{
						Console.WriteLine(Path.GetFileName(file));
						File.Delete(Path.Combine(DataDirectory, Path.GetFileName(file)));
						File.Copy(file, Path.Combine(DataDirectory, Path.GetFileName(file)));
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
			if (treeView1.SelectedNode != null && Owner.Owner is MainForm parent)
			{
				string DataDirectory = Path.Combine(Path.GetDirectoryName(parent.saveLocation), "Data");
				if (!Directory.Exists(DataDirectory))
				{
					MessageBox.Show("There is not a \"Data\" folder present in the pack folder", "Folder missing");
					return;
				}

				ProcessEntries((string[])e.Data.GetData(DataFormats.FileDrop, false), DataDirectory);
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if(!audioFile.HasCategory(PCKAudioFile.AudioCategory.EAudioType.Overworld) ||
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
			saved = true;
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
			using (creditsEditor prompt = new creditsEditor(credits))
				if (prompt.ShowDialog() == DialogResult.OK)
					audioFile.SetCredits(prompt.Credits.Split('\n'));
		}

		private void AudioEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!saved) return;
			saveToolStripMenuItem1_Click(sender, e);
		}
	}
}
