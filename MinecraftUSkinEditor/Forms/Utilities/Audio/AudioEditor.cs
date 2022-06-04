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

namespace PckStudio.Forms.Utilities
{
	public partial class AudioEditor : MetroForm
	{
		public bool saved = false;
		public string defaultType = "yes";
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

		PCKFile audioPCK;
		bool isVita;
		PCKFile.FileData mf;
		public AudioEditor(PCKFile.FileData MineFile, bool littleEndian)
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
			using (var stream = new MemoryStream(mf.data))
            {
				audioPCK = PCKFileReader.Read(stream, isVita);
            }
			if (!audioPCK.meta_data.ContainsKey("CUENAME"))
			{
				throw new Exception("This is not a valid audio.pck file");
			}
			//defaultType = audioPCK.meta_data["CUENAME"];
			int index = 0;
			List<PCKFile.FileData> tempMineFiles = audioPCK.file_entries;
			foreach (PCKFile.FileData mineFile in tempMineFiles)
			{
				mineFile.name = getCatString(mineFile.type);
				Console.WriteLine("Category Found: " + mineFile.name);
				if (cats.Contains(mineFile.type))
				{
					Console.WriteLine("Duplicate category found, " + getCatString(mineFile.type) + ". Combining...");
					audioPCK.file_entries.Remove(mineFile);
					audioPCK.file_entries.Find(category => category.name == getCatString(mineFile.type)).properties = mineFile.properties;
				}
				else
				{
					TreeNode treeNode = new TreeNode();
					treeNode.Text = mineFile.name;
					treeNode.Tag = mineFile;
					//treeNode.ImageIndex = mineFile.type;
					//treeNode.SelectedImageIndex = mineFile.type;
					treeView1.Nodes.Add(treeNode);
					cats.Add((int)mineFile.type);
				}

				index++;
			}

			treeView1.TreeViewNodeSorter = new NodeSorter();
			treeView1.Sort();
		}

		private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
		{
			comboBox1.Items.Clear(); //Resets metadata combobox of selectable entry names
			if (e.Node.Tag == null) return;
			var strings = (KeyValuePair<string, string>)e.Node.Tag;
			//string type = audioPCK.meta_data[0];
			//defaultType = type;
			//type = strings.Key;
			string value = strings.Value;
			

			foreach (var metaType in audioPCK.meta_data)
				comboBox1.Items.Add(metaType.Key);
			comboBox1.Text = "TODO";//Sets currently selected metadata type to type selected in selected metadata node
			textBox1.Text = value;//Sets currently selected metadata value to value selected in selected metadata node
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeView2.Nodes.Clear();
			PCKFile.FileData mineFile = (PCKFile.FileData)e.Node.Tag;
			foreach (var entry in mineFile.properties)
			{
				TreeNode meta = new TreeNode();
				meta.Text = entry.Item1;
				meta.Tag = entry;
				treeView2.Nodes.Add(meta);
			}
			if (treeView2.Nodes.Count > 0) treeView2.SelectedNode = treeView2.Nodes[0];
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null)
			{
				//object[] strings = (object[])treeView2.SelectedNode.Tag;
				//strings[1] = textBox1.Text;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null)
			{
				//Sets metadata type to new chosen one
				//object[] strings = (object[])treeView2.SelectedNode.Tag;
				//treeView2.SelectedNode.Text = comboBox1.Text;
				//strings[0] = comboBox1.Text;
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
					PCKFile.FileData mf = new PCKFile.FileData(cat, getCatID(cat)); //Creates new minefile template
					TreeNode addNode = new TreeNode(mf.name) { Tag = mf };//creates node for minefile
					//addNode.ImageIndex = mf.type;
					//addNode.SelectedImageIndex = mf.type;
					//audioPCKFile.FileDatas.Add(mf);
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

			TreeNode meta = new TreeNode("New Entry");
			//meta.Tag = obj;
			treeView2.Nodes.Add(meta);
			//((PCKFile.FileData)treeView1.SelectedNode.Tag).properties.Add(defaultType, new List<string> { "New Entry" });
		}
		public void treeView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && treeView2.SelectedNode != null)
			{
				if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
				//((PCKFile.FileData)treeView1.SelectedNode.Tag).properties.Remove((string)treeView2.SelectedNode.Tag);
				treeView2.SelectedNode.Remove();
			}
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
			cats.Remove(getCatID(treeView1.SelectedNode.Text));
			//audioPCKFile.FileDatas.Remove((PCKFile.FileData)treeView1.SelectedNode.Tag);
			treeView1.SelectedNode.Remove();
			treeView2.Nodes.Clear();
			if(treeView1.SelectedNode != null)
			{
				PCKFile.FileData mineFile = (PCKFile.FileData)treeView1.SelectedNode.Tag;
				foreach (var entry in mineFile.properties)
				{
					TreeNode meta = new TreeNode(entry.Item1);
					meta.Tag = entry;
					treeView2.Nodes.Add(meta);
				}
			}
		}

		private void removeEntryMenuItem_Click(object sender, EventArgs e)
		{
			//((PCKFile.FileData)treeView1.SelectedNode.Tag).properties.Remove((object[])treeView2.SelectedNode.Tag);
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
						object[] obj = {  };

						TreeNode meta = new TreeNode();
						meta.Text = "CUENAME";
						meta.Tag = obj;
						treeView2.Nodes.Add(meta);
                        ((PCKFile.FileData)treeView1.SelectedNode.Tag).properties.Add(new Tuple<string, string>("CUENAME", Path.GetFileNameWithoutExtension(binka)));
                    }
				}
			}
		}

		//private static void writeMinecraftString(FileOutput f, string str)
		//{
		//	byte[] d = Encoding.BigEndianUnicode.GetBytes(str);
		//	f.writeInt(d.Length / 2);
		//	f.writeBytes(d);
		//	f.writeInt(0);
		//}

		//private static void writeMinecraftStringVita(FileOutput f, string str)
		//{
		//	Console.WriteLine("WriteVita -- " + str);
		//	byte[] bytes = Encoding.Unicode.GetBytes(str);
		//	f.writeIntVita(bytes.Length / 2);
		//	f.writeBytes((bytes));
		//	f.writeIntVita(0);
		//}

		//public static byte[] buildAudioPCKVita(PCKFile pck)
		//{
		//	FileOutput fileOutput = new FileOutput();
		//	fileOutput.Endian = Endianness.Big;
		//	fileOutput.writeIntVita(1);
		//	fileOutput.writeIntVita(pck.meta_data.Count);
			//foreach (int num in pck.meta_data.Keys)
			//{
			//	fileOutput.writeIntVita(num);
			//	writeMinecraftStringVita(fileOutput, pck.meta_data[num]);
			//}
			//fileOutput.writeIntVita(PCKFile.FileDatas.Count);
			//foreach (PCKFile.FileData mineFile in PCKFile.FileDatas)
			//{
			//	mineFile.name = "";
			//	fileOutput.writeIntVita(mineFile.data.Length);
			//	fileOutput.writeIntVita(mineFile.type);
			//	writeMinecraftStringVita(fileOutput, mineFile.name);
			//}
			//foreach (PCKFile.FileData mineFile2 in PCKFile.FileDatas)
			//{
			//	string str = "";
			//	try
			//	{
			//		fileOutput.writeIntVita(mineFile2.properties.Count);
			//		foreach (var array in mineFile2.properties)
			//		{
			//			str = array.Key;
			//			fileOutput.writeIntVita(pck.typeCodes[array.Key]);
			//			writeMinecraftStringVita(fileOutput, array.Value);
			//		}
			//		fileOutput.writeBytes(mineFile2.data);
			//	}
			//	catch (Exception)
			//	{
			//		MessageBox.Show(str + " is not in the main metadatabase");
			//		break;
			//	}
			//}
		//	return fileOutput.getBytes();
		//}

		public static byte[] buildAudioPCK(PCKFile pck)
		{
			//FileOutput f = new FileOutput();
			//f.Endian = pck.isLittleEndian ? Endianness.Little : Endianness.Big;

			//f.writeInt(1);
			//f.writeInt(pck.meta_data.Count);
			//foreach (int type in pck.meta_data.Keys)
			//{
			//	f.writeInt(type);
			//	writeMinecraftString(f, pck.meta_data[type]);
			//}

			//f.writeInt(PCKFile.FileDatas.Count);
			//Console.WriteLine(PCKFile.FileDatas.Count);
			//foreach (PCKFile.FileData mf in PCKFile.FileDatas)
			//{
			//	mf.name = "";
			//	f.writeInt(mf.data.Length);
			//	f.writeInt(mf.type);
			//	writeMinecraftString(f, mf.name);
			//}

			//foreach (PCKFile.FileData mf in PCKFile.FileDatas)
			//{
			//	string missing = "";
			//	try
			//	{
			//		f.writeInt(mf.properties.Count);
			//		foreach (var entry in mf.properties)
			//		{
			//			missing = entry.Key;
			//			f.writeInt(pck.typeCodes[entry.Key]);
			//			writeMinecraftString(f, entry.Value);
			//		}

			//		f.writeBytes(mf.data);
			//	}
			//	catch (Exception)
			//	{
			//		MessageBox.Show(missing + " is not in the main metadatabase");
			//		break;
			//	}
			//}
			//return f.getBytes();
			return new byte[0];
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

			foreach (PCKFile.FileData mf in audioPCK.file_entries)
				emptyCat = mf.properties.Count == 0;

			if (emptyCat)
			{
				MessageBox.Show("The game will crash upon loading your pack if a category is empty", "Empty Category");
				return;
			}

			//mf.data = isVita ? buildAudioPCKVita(audioPCK) : buildAudioPCK(audioPCK);
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
