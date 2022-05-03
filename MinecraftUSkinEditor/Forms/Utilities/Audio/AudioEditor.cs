using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio;

// Audio Editor by MattNL

namespace PckStudio.Forms.Utilities
{
	public partial class AudioEditor : MetroForm
	{
		public bool saved = false;
		public string defaultType;
		public string cat;
		public List<int> cats = new List<int>();
		public List<int> totalCats = new List<int>();

		public class NodeSorter : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null || y == null) return -1;
				return (x as TreeNode).ImageIndex.CompareTo((x as TreeNode).ImageIndex);
			}
		}

		public string getCatString(int cat)
		{
			switch (cat)
			{
				case 0: return "Overworld";
				case 1: return "Nether";
				case 2: return "End";
				case 3: return "Creative";
				case 4: return "Menu";
				case 5: return "Battle";
				case 6: return "Tumble";
				case 7: return "Glide";
				case 8: return "Unused"; // Unknown what this is used for. Probably the scrapped Mini Game 4 referenced in the code
				default: return "Not valid";
			}
		}

		public int getCatID(string cat)
		{
			switch (cat)
			{
				case "Overworld": return 0;
				case "Nether": return 1;
				case "End": return 2;
				case "Creative": return 3;
				case "Menu": return 4;
				case "Battle": return 5;
				case "Tumble": return 6;
				case "Glide": return 7;
				case "Unused": return 8; // Unknown what this is used for. Probably the scrapped Mini Game 4 referenced in the code
				default: return -1;
			}
		}

		PCK audioPCK = new PCK();
		bool isVita;
		PCK.MineFile mf;
		public AudioEditor(PCK.MineFile MineFile, bool littleEndian)
		{
			isVita = littleEndian;
			ImageList catImages = new ImageList();
			catImages.ColorDepth = ColorDepth.Depth32Bit;
			catImages.Images.Add(Properties.Resources.audio_0_overworld);
			catImages.Images.Add(Properties.Resources.audio_1_nether);
			catImages.Images.Add(Properties.Resources.audio_2_end);
			catImages.Images.Add(Properties.Resources.audio_3_creative);
			catImages.Images.Add(Properties.Resources.audio_4_menu);
			catImages.Images.Add(Properties.Resources.audio_5_mg01);
			catImages.Images.Add(Properties.Resources.audio_6_mg02);
			catImages.Images.Add(Properties.Resources.audio_7_mg03);

			InitializeComponent();

			treeView1.ImageList = catImages;

			mf = MineFile;
			if (isVita) audioPCK.ReadVita(mf.data, true);
			else audioPCK.Read(mf.data, true);
			defaultType = audioPCK.types[0];
			int check; // This is needed for the TryGetValue function which is annoying
			if (!audioPCK.typeCodes.TryGetValue("CUENAME", out check))
			{
				throw new System.Exception("This is not a valid audio.pck file");
			}
			int index = 0;
			List<PCK.MineFile> tempMineFiles = audioPCK.mineFiles;
			foreach (PCK.MineFile mineFile in tempMineFiles)
			{
				mineFile.name = getCatString(mineFile.type);
				Console.WriteLine("Category Found: " + mineFile.name);
				if (cats.Contains<int>(mineFile.type))
				{
					Console.WriteLine("Duplicate category found, " + getCatString(mineFile.type) + ". Combining...");
					List<object[]> newEntries = mineFile.entries;
					audioPCK.mineFiles.Remove(mineFile);
					audioPCK.mineFiles.Find(category => category.name == getCatString(mineFile.type)).entries.AddRange(newEntries);
				}
				else
				{
					TreeNode treeNode = new TreeNode();
					treeNode.Text = mineFile.name;
					treeNode.Tag = mineFile;
					treeNode.ImageIndex = mineFile.type;
					treeNode.SelectedImageIndex = mineFile.type;
					treeView1.Nodes.Add(treeNode);
					cats.Add(mineFile.type);
				}

				index++;
			}

			treeView1.TreeViewNodeSorter = new NodeSorter();
			treeView1.Sort();
		}

		private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
		{
			comboBox1.Items.Clear();//Resets metadata combobox of selectable entry names
			object[] strings = (object[])e.Node.Tag;
			string type = audioPCK.types[0];
			defaultType = type;
			string value = "";
			if (strings != null)
			{
				type = (string)strings[0];
				value = (string)strings[1];
			}

			foreach (int metaType in audioPCK.types.Keys)
				comboBox1.Items.Add(audioPCK.types[metaType]);//fills combobox with metadata from the main metadatabase
			comboBox1.Text = type;//Sets currently selected metadata type to type selected in selected metadata node
			textBox1.Text = value;//Sets currently selected metadata value to value selected in selected metadata node
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeView2.Nodes.Clear();
			PCK.MineFile mineFile = (PCK.MineFile)e.Node.Tag;
			foreach (object[] entry in mineFile.entries) //object = metadata entry(name:value)
			{
				object[] strings = (object[])entry;
				TreeNode meta = new TreeNode();

				foreach (object[] entryy in mineFile.entries)
					meta.Text = (string)strings[0];
				meta.Tag = entry;
				treeView2.Nodes.Add(meta);
			}
			if (treeView2.Nodes.Count > 0) treeView2.SelectedNode = treeView2.Nodes[0];
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null)
			{
				object[] strings = (object[])treeView2.SelectedNode.Tag;
				strings[1] = textBox1.Text;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null)
			{
				//Sets metadata type to new chosen one
				object[] strings = (object[])treeView2.SelectedNode.Tag;
				treeView2.SelectedNode.Text = comboBox1.Text;
				strings[0] = comboBox1.Text;
			}
		}

		private void addCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				int check = Enumerable.Range(0, 8).Except(cats).First(); // Exclude Unused for now
				if (0 <= check && check <= 8)
				{
					PckStudio.addCategory add = new PckStudio.addCategory(this);//sets category adding dialog
					add.ShowDialog();//displays metadata adding dialog
					add.Dispose();//diposes generated metadata adding dialog data
					if (!cats.Contains(getCatID(cat))) cats.Add(getCatID(cat));
					else return;
					PCK.MineFile mf = new PCK.MineFile();//Creates new minefile template

					var emptyBytes = new List<byte>(); // the category files are empty to not take up space
					byte[] emptyBytesArray = emptyBytes.ToArray();

					mf.data = emptyBytesArray;//adds file data to minefile
					mf.filesize = mf.data.Length;//gets filesize for minefile
					mf.name = cat;//sets minfile name to file name
					mf.type = getCatID(cat);//sets minefile type to default
					TreeNode addNode = new TreeNode(mf.name) { Tag = mf };//creates node for minefile
					addNode.ImageIndex = mf.type;
					addNode.SelectedImageIndex = mf.type;
					//audioPCK.mineFiles.Add(mf);
					treeView1.Nodes.Add(addNode);
					treeView1.Sort();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("All possible categories are used", "There are no more categories that could be added");
			}
		}

		private void addEntryMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return;
			object[] obj = { defaultType, "New Entry" };

			TreeNode meta = new TreeNode();
			meta.Text = "New Entry";
			meta.Tag = obj;
			treeView2.Nodes.Add(meta);
			((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Add(obj);
		}
		public void treeView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && treeView2.SelectedNode != null)
			{
				if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
				((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Remove((object[])treeView2.SelectedNode.Tag);
				treeView2.SelectedNode.Remove();
			}
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
			cats.Remove(getCatID(treeView1.SelectedNode.Text));
			//audioPCK.mineFiles.Remove((PCK.MineFile)treeView1.SelectedNode.Tag);
			treeView1.SelectedNode.Remove();
			treeView2.Nodes.Clear();
			if(treeView1.SelectedNode != null)
			{
				PCK.MineFile mineFile = (PCK.MineFile)treeView1.SelectedNode.Tag;
				foreach (object[] entry in mineFile.entries) //object = metadata entry(name:value)
				{
					object[] strings = (object[])entry;
					TreeNode meta = new TreeNode();

					foreach (object[] entryy in mineFile.entries)
						meta.Text = (string)strings[0];
					meta.Tag = entry;
					treeView2.Nodes.Add(meta);
				}
			}
		}

		private void removeEntryMenuItem_Click(object sender, EventArgs e)
		{
			((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Remove((object[])treeView2.SelectedNode.Tag);
			treeView2.SelectedNode.Remove();
		}

		private void Binka_DragDrop(object sender, DragEventArgs e)
		{
			if (treeView1.SelectedNode != null)
			{
				string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

				foreach (string binka in FileList)
				{
					if(System.IO.Path.GetExtension(binka) == ".binka")
					{
						object[] obj = { "CUENAME", System.IO.Path.GetFileNameWithoutExtension(binka) };

						TreeNode meta = new TreeNode();
						meta.Text = "CUENAME";
						meta.Tag = obj;
						treeView2.Nodes.Add(meta);
						((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Add(obj);
					}
				}
			}
		}

		private static byte[] endianReverseUnicode(byte[] str)
		{
			byte[] newStr = new byte[str.Length];
			for (int i = 0; i < str.Length; i += 2)
			{
				newStr[i] = str[i + 1];
				newStr[i + 1] = str[i];
			}
			return newStr;
		}

		private static void writeMinecraftString(FileOutput f, string str)
		{
			byte[] d = Encoding.Unicode.GetBytes(str);
			f.writeInt(d.Length / 2);
			f.writeBytes(endianReverseUnicode(d));
			f.writeInt(0);
		}

		private static void writeMinecraftStringVita(FileOutput f, string str)
		{
			Console.WriteLine("WriteVita -- " + str);
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			f.writeIntVita(bytes.Length / 2);
			f.writeBytes((bytes));
			f.writeIntVita(0);
		}

		public static byte[] buildAudioPCKVita(PCK pck)
		{
			FileOutput fileOutput = new FileOutput();
			fileOutput.Endian = Endianness.Big;
			fileOutput.writeIntVita(1);
			fileOutput.writeIntVita(pck.types.Count);
			foreach (int num in pck.types.Keys)
			{
				fileOutput.writeIntVita(num);
				writeMinecraftStringVita(fileOutput, pck.types[num]);
			}
			fileOutput.writeIntVita(pck.mineFiles.Count);
			foreach (PCK.MineFile mineFile in pck.mineFiles)
			{
				mineFile.name = "";
				fileOutput.writeIntVita(mineFile.data.Length);
				fileOutput.writeIntVita(mineFile.type);
				writeMinecraftStringVita(fileOutput, mineFile.name);
			}
			foreach (PCK.MineFile mineFile2 in pck.mineFiles)
			{
				string str = "";
				try
				{
					fileOutput.writeIntVita(mineFile2.entries.Count);
					foreach (object[] array in mineFile2.entries)
					{
						str = array[0].ToString();
						fileOutput.writeIntVita(pck.typeCodes[(string)array[0]]);
						writeMinecraftStringVita(fileOutput, (string)array[1]);
					}
					fileOutput.writeBytes(mineFile2.data);
				}
				catch (Exception)
				{
					MessageBox.Show(str + " is not in the main metadatabase");
					break;
				}
			}
			return fileOutput.getBytes();
		}

		public static byte[] buildAudioPCK(PCK pck)
		{
			FileOutput f = new FileOutput();
			f.Endian = pck.IsLittleEndian ? Endianness.Little : Endianness.Big;

			f.writeInt(1);
			f.writeInt(pck.types.Count);
			foreach (int type in pck.types.Keys)
			{
				f.writeInt(type);
				writeMinecraftString(f, pck.types[type]);
			}

			f.writeInt(pck.mineFiles.Count);
			Console.WriteLine(pck.mineFiles.Count);
			foreach (PCK.MineFile mf in pck.mineFiles)
			{
				mf.name = "";
				f.writeInt(mf.data.Length);
				f.writeInt(mf.type);
				writeMinecraftString(f, mf.name);
			}

			foreach (PCK.MineFile mf in pck.mineFiles)
			{
				string missing = "";
				try
				{
					f.writeInt(mf.entries.Count);
					foreach (object[] entry in mf.entries)
					{
						missing = entry[0].ToString();
						f.writeInt(pck.typeCodes[(string)entry[0]]);
						writeMinecraftString(f, (string)entry[1]);
					}

					f.writeBytes(mf.data);
				}
				catch (Exception)
				{
					MessageBox.Show(missing + " is not in the main metadatabase");
					break;
				}
			}
			return f.getBytes();
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			FormMain.treeViewToMineFiles(treeView1, audioPCK);

			if(!cats.Contains(0) || !cats.Contains(1) || !cats.Contains(2))
			{
				MessageBox.Show("The game will crash upon loading your pack if the Overworld, Nether and End categories don't all exist.", "Mandatory Categories Missing");
				return;
			}

			bool emptyCat = false;

			foreach (PCK.MineFile mf in audioPCK.mineFiles) if (mf.entries.Count == 0) emptyCat = true;

			if (emptyCat)
			{
				MessageBox.Show("The game will crash upon loading your pack if a category is empty", "Empty Category");
				return;
			}

			mf.data = isVita ? buildAudioPCKVita(audioPCK) : buildAudioPCK(audioPCK);
			saved = true;
		}

		private void metroLabel2_Click(object sender, EventArgs e)
		{

		}

		private void treeView2_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.All;
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Simply drag and drop BINKA audio files into the right tree to add them to the category selected on the left tree.\n\n" +
				"The \"Menu\" category will only play once when loading the pack, and never again.\n\n" +
				"The \"Creative\" category will only play songs listed in that category, and unlike other editions of Minecraft, will NOT play songs from the Overworld category. You can fix this by adding your overworld songs to the Creative category too.\n\n" +
				"The mini game categories will only play if you have your pack loaded in those mini games.\n\n" +
				"You can modify and create PSVita and PS4 audio pcks by clicking \"PS4/Vita\" in the \"Create -> Audio.pck\" context menu", "Help");
		}
	}
}
