using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO;

// Audio Editor by MattNL
// additional work and optimization by Miku-666

namespace PckStudio.Forms.Utilities
{
	public partial class AudioEditor : MetroForm
	{
		public bool saved = false;
		public string credits = "";
		public string defaultType = "yes";
		string WorkingDir = "";
		string tempDir = "";
		PCKFile audioPCK = null;
		PCKFile.FileData audioPCKFile;
		bool _isLittleEndian;
		public List<string> cats = new List<string>();
		public class NodeSorter : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null || y == null) return -1;
				return ((x as TreeNode).Tag as PCKFile.FileData).type.CompareTo(((y as TreeNode).Tag as PCKFile.FileData).type);
			}
		}

		public static readonly List<string> Categories = new List<string>
		{
			"Overworld",	// 0
			"Nether",		// 1
			"End",			// 2
			"Creative",		// 3
			"Menu/Loading",	// 4
			"Battle",		// 5
			"Tumble",		// 6
			"Glide",		// 7
			"Unused?"		// 8
		};

		internal string GetCategoryFromId(int categoryId)
		{
			return categoryId > -1 && categoryId < Categories.Count ? Categories[categoryId] : "Not valid";
		}

		internal int GetCategoryId(string category)
		{
			return Categories.IndexOf(category);
		}

		public static PCKFile.FileData CreateAudioPck(bool isLittle)
		{
			// create actual valid pck file structure
			PCKFile audioPck = new PCKFile(1); // 1 = audio.pck
			//audioPck.meta_data.Add("CUENAME");
			//audioPck.meta_data.Add("CREDIT");
			//audioPck.meta_data.Add("CREDITID");
			audioPck.Files.Add(new PCKFile.FileData("", 0));
			audioPck.Files.Add(new PCKFile.FileData("", 1));
			audioPck.Files.Add(new PCKFile.FileData("", 2));

			// create a file data entry for current open pck file
			PCKFile.FileData audioFileData = new PCKFile.FileData("audio.pck", 8);
			using (var stream = new MemoryStream())
			{
				PCKFileWriter.Write(stream, audioPck, isLittle);
				audioFileData.SetData(stream.ToArray());
			}
			return audioFileData;
		}

		/// <summary>
		/// Overload that creates a new audio.pck file
		/// </summary>
		/// <param name="locFile"></param>
		/// <param name="isLittleEndian"></param>
		public AudioEditor(LOCFile locFile, bool isLittleEndian) : this(CreateAudioPck(isLittleEndian), locFile, isLittleEndian)
        {

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
			string asiPath = Path.Combine(tempDir, "binkawin.asi");
			string mssPath = Path.Combine(tempDir, "mss32.dll");
			string encoderPath = Path.Combine(tempDir, "binka_encode.exe");

			// Deletes files so that System.IO exceptions are avoided
			if (File.Exists(asiPath)) File.Delete(asiPath);
			if (File.Exists(mssPath)) File.Delete(mssPath);
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
			if (treeView1.SelectedNode.Tag == null || !(treeView1.SelectedNode.Tag is PCKFile.FileData) || !(treeView2.SelectedNode.Tag is ValueTuple<string, string>)) return;
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

		LOCFile loc;
		public AudioEditor(PCKFile.FileData MineFile, LOCFile locFile, bool isLittleEndian)
		{
			loc = locFile;
			WorkingDir = Directory.GetCurrentDirectory();
			tempDir = Path.Combine(WorkingDir, "temp");
			try
			{
				handleUtilFiles();
				//library = LoadLibrary(Path.Combine(tempDir, "mss32.dll"));
			}
			catch (IOException ex)
			{
				MessageBox.Show("Failed to get Binka conversion files", "Exception thrown");
				this.Close();
			}

			_isLittleEndian = isLittleEndian;
			if (isLittleEndian) Text += " (PS4/Vita)";
			InitializeComponent();
			audioPCKFile = MineFile;
			using (var stream = new MemoryStream(audioPCKFile.data))
            {
				audioPCK = PCKFileReader.Read(stream, isLittleEndian);
            }

			//if (!audioPCK.meta_data.Contains("CUENAME") || audioPCK.type != 1)
			//{
			//	throw new Exception("This is not a valid audio.pck file");
			//}
			foreach (PCKFile.FileData categoryFile in audioPCK.Files)
			{
				string CatString = GetCategoryFromId(categoryFile.type);
				Console.WriteLine($"Category Found: {CatString} ({categoryFile.type}).");
				foreach (var property in categoryFile.properties.ToArray())
				{
					if (property.Item1 == "CREDITID")
					{
						loc.RemoveLocKey(property.Item2);
						categoryFile.properties.Remove(property);
					}
					else if (property.Item1 == "CREDIT")
					{
						credits += property.Item2 + "\n";
						categoryFile.properties.Remove(property);
					}
				}
				if (cats.Contains(GetCategoryFromId(categoryFile.type)))
				{
					Console.WriteLine("Duplicate category found, " + CatString + ". Combining...");
					audioPCK.Files.Remove(categoryFile);
					audioPCK.Files.Find(category => category.filepath == GetCategoryFromId(categoryFile.type)).properties = categoryFile.properties;
				}
				else
				{
					if (categoryFile.filepath == "include_overworld" && categoryFile.type == 3)
					{
						PCKFile.FileData overworldMF = audioPCK.Files.Find(file => file.type == 0);

						foreach(ValueTuple<string,string> property in categoryFile.properties.ToList())
						{
							if (overworldMF.properties.Contains(property)) categoryFile.properties.Remove(property);
						}

						playOverworldInCreative.Checked = true;
					}

					TreeNode treeNode = new TreeNode(CatString);
					treeNode.Tag = categoryFile;
					treeNode.ImageIndex = categoryFile.type;
					treeNode.SelectedImageIndex = categoryFile.type;
					treeView1.Nodes.Add(treeNode);
					cats.Add(GetCategoryFromId(categoryFile.type));
				}
			}

			playOverworldInCreative.Enabled = cats.Contains(GetCategoryFromId(3));

			treeView1.TreeViewNodeSorter = new NodeSorter();
			treeView1.Sort();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeView2.Nodes.Clear();
			PCKFile.FileData mineFile = (PCKFile.FileData)e.Node.Tag;
			foreach (var entry in mineFile.properties)
			{
				var property = (ValueTuple<string, string>)entry;
				TreeNode meta = new TreeNode();
				meta.Text = property.Item2;
				meta.Tag = entry;
				treeView2.Nodes.Add(meta);
			}
			credits = credits.TrimEnd('\n');
			if (treeView2.Nodes.Count > 0) treeView2.SelectedNode = treeView2.Nodes[0];
		}

		private void addCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string[] avalible = Categories.FindAll(str =>
				{
					return !cats.Contains(str);
				}).ToArray();
				addCategory add = new addCategory(avalible); //sets category adding dialog
				if (add.ShowDialog() == DialogResult.OK)
					cats.Add(add.Category);
				PCKFile.FileData file = new PCKFile.FileData(add.Category, GetCategoryId(add.Category));
				TreeNode addNode = new TreeNode(file.filepath) { Tag = file };
                audioPCK.Files.Add(file);
				addNode.ImageIndex = GetCategoryId(add.Category);
				addNode.SelectedImageIndex = GetCategoryId(add.Category);
				treeView1.Nodes.Add(addNode);
				treeView1.Sort();
				add.Dispose(); // diposes generated metadata adding dialog data
				playOverworldInCreative.Enabled = cats.Contains(GetCategoryFromId(3));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				MessageBox.Show("All possible categories are used", "There are no more categories that could be added");
			}
		}

		private void addEntryMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null && !(treeView1.SelectedNode.Tag is PCKFile.FileData)) return;

			MainForm parent = Owner.Owner as MainForm; // Gets the MainForm so we can access the Save Location
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
			if (String.IsNullOrEmpty(ofn.FileName)) return; // Return if name is null or if the user cancels

			ProcessEntries(ofn.FileNames, DataDirectory);
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
			cats.Remove(treeView1.SelectedNode.Text);
			if (audioPCK.Files.Remove((PCKFile.FileData)treeView1.SelectedNode.Tag))
			{
				treeView2.Nodes.Clear();
				treeView1.SelectedNode.Remove();
			}
			playOverworldInCreative.Enabled = cats.Contains(GetCategoryFromId(3));
		}

		public void treeView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				removeEntryMenuItem_Click(sender, e);
		}

		private void removeEntryMenuItem_Click(object sender, EventArgs e)
		{
			var mainNode = treeView1.SelectedNode;
			var subNode = treeView2.SelectedNode;
			if (subNode != null &&
				subNode.Tag is ValueTuple<string, string> &&
				mainNode.Tag is PCKFile.FileData)
			{
				var file = mainNode.Tag as PCKFile.FileData;
				var property = (ValueTuple<string, string>)subNode.Tag;
				if (file.properties.Remove(property))
					subNode.Remove();
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
						duplicate_song = File.ReadAllBytes(file).Length == File.ReadAllBytes(new_loc).Length;
						if (!duplicate_song)
						{
							DialogResult user_prompt = MessageBox.Show("\"" + Path.GetFileNameWithoutExtension(file) + ".binka\" already exists. Continuing will replace the existing file. Are you sure you want to continue moving the file? By pressing no, you will skip this file. You can also cancel all pending file operations.", "File already exists", MessageBoxButtons.YesNoCancel);
							if (user_prompt == DialogResult.Cancel) break;
							else if (user_prompt == DialogResult.No) continue;
						}
					}

					if (Path.GetExtension(file) == ".wav") // Convert Wave to BINKA
					{
						Cursor.Current = Cursors.WaitCursor;
						Audio.pleaseWait waitDiag = new Audio.pleaseWait();
						waitDiag.Show(this);

						int error_code = 0;
						await Task.Run(() =>
						{
							var process = new System.Diagnostics.Process();
							process.StartInfo.FileName = Path.Combine(tempDir, "binka_encode.exe");
							process.StartInfo.Arguments = "\"" + file + "\" \"" + new_loc + "\"";
							process.StartInfo.UseShellExecute = true;
							process.StartInfo.CreateNoWindow = true;
							process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
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

					TreeNode meta = new TreeNode(Path.GetFileNameWithoutExtension(file));
					var property = new ValueTuple<string, string>("CUENAME", Path.GetFileNameWithoutExtension(file));
					meta.Tag = property;
					treeView2.Nodes.Add(meta);
					(treeView1.SelectedNode.Tag as PCKFile.FileData).properties.Add(property);
				}
			}
		}

		private void Binka_DragDrop(object sender, DragEventArgs e)
		{
			if (treeView1.SelectedNode == null && !(treeView1.SelectedNode.Tag is PCKFile.FileData)) return;

			MainForm parent = Owner.Owner as MainForm; // Gets the MainForm so we can access the Save Location
			string DataDirectory = Path.Combine(Path.GetDirectoryName(parent.saveLocation), "Data");
			if (!Directory.Exists(DataDirectory))
			{
				MessageBox.Show("There is not a \"Data\" folder present in the pack folder", "Folder missing");
				return;
			}

			ProcessEntries((string[])e.Data.GetData(DataFormats.FileDrop, false), DataDirectory);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if(!cats.Contains(GetCategoryFromId(0)) ||
			   !cats.Contains(GetCategoryFromId(1)) ||
			   !cats.Contains(GetCategoryFromId(2)))
			{
				MessageBox.Show("The game will crash upon loading your pack if the Overworld, Nether and End categories don't all exist.", "Mandatory Categories Missing");
				return;
			}

			bool emptyCat = false;

			List <PCKFile.FileData> new_order = audioPCK.Files.OrderBy(fd => fd.type).ToList();

			audioPCK.Files.Clear();

			foreach (PCKFile.FileData mf in new_order)
			{
				audioPCK.Files.Add(mf);
				if (mf.properties.Count == 0) emptyCat = true;
			}

			if (emptyCat)
			{
				MessageBox.Show("The game will crash upon loading your pack if a category is empty", "Empty Category");
				return;
			}

			using (StringReader reader = new StringReader(credits))
			{
				string line;
				int creditCount = 1;
				while ((line = reader.ReadLine()) != null)
				{
					var credit = ("CREDIT", line);
					string credit_id = "IDS_CREDIT" + (creditCount > 1 ? creditCount.ToString() : "");
					var creditid = ("CREDITID", credit_id);
					audioPCK.Files[0].properties.Add(credit);
					audioPCK.Files[0].properties.Add(creditid);
					loc.AddLocKey(credit_id, line);
					creditCount++;
					Console.WriteLine(line);
				}
			}
			using (var stream = new MemoryStream())
            {
				PCKFileWriter.Write(stream, audioPCK, _isLittleEndian);
				audioPCKFile.SetData(stream.ToArray());
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
			creditsEditor prompt = new creditsEditor(credits);
			prompt.ShowDialog();
			credits = prompt.Credits;
		}

		private void AudioEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (saved != true) return;
			PCKFile.FileData overworldMF = new PCKFile.FileData("", -1);

			foreach (PCKFile.FileData mf in audioPCK.Files)
			{
				mf.filepath = "";
				if (playOverworldInCreative.Checked && mf.type == 0) overworldMF = mf;
				if (playOverworldInCreative.Checked && mf.type == 3 && overworldMF.type != -1)
				{
					foreach (ValueTuple<string, string> property in overworldMF.properties)
					{
						if (property.Item1 == "CUENAME" && !mf.properties.Contains(property))
						{
							mf.properties.Add(property);
							Console.WriteLine(property.Item2);
						}
					}
					mf.filepath = "include_overworld";
				}
			}

			using (var stream = new MemoryStream())
			{
				PCKFileWriter.Write(stream, audioPCK, _isLittleEndian);
				audioPCKFile.SetData(stream.ToArray());
			}
		}
	}
}
