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
		public string defaultType;
		public string cat;
		public List<int> cats = new List<int>();

		public string getCatString(int cat)
		{
			switch (cat)
			{
				case 0: return "Overworld"; break;
				case 1: return "Nether"; break;
				case 2: return "End"; break;
				case 3: return "Creative"; break;
				case 4: return "Menu"; break;
				case 5: return "Battle"; break;
				case 6: return "Tumble"; break;
				case 7: return "Glide"; break;
				case 8: return "Unused"; break;
				default: return "Not valid"; break;
			}
		}

		public int getCatID(string cat)
		{
			switch (cat)
			{
				case "Overworld": return 0; break;
				case "Nether": return 1; break;
				case "End": return 2; break;
				case "Creative": return 3; break;
				case "Menu": return 4; break;
				case "Battle": return 5; break;
				case "Tumble": return 6; break;
				case "Glide": return 7; break;
				case "Unused": return 8; break;
				default: return -1; break;
			}
		}

		PCK audioPCK = new PCK();
		PCK.MineFile mf;
		public AudioEditor(byte[] data, PCK.MineFile MineFile)
		{
			InitializeComponent();
			audioPCK.Read(data);
			mf = MineFile;
			TreeNode treeViewMain = new TreeNode();
			foreach (PCK.MineFile mineFile in audioPCK.mineFiles)
			{
				Console.WriteLine(mineFile.name);
				TreeNode treeNode = new TreeNode();
				if (!cats.Contains<int>(mineFile.type)) cats.Add(mineFile.type);
				treeNode.Text = getCatString(mineFile.type);
				treeNode.Tag = mineFile;
				treeView1.Nodes.Add(treeNode);
				continue;
			}
		}

		private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
		{
			comboBox1.Items.Clear();//Resets metadata combobox of selectable entry names
			object[] strings = (object[])e.Node.Tag;
			string type = audioPCK.types[0];
			defaultType = type;
			string value = "";
			if(strings != null)
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
				continue;
			}
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
					cats.Add(getCatID(cat));
					PCK.MineFile mf = new PCK.MineFile();//Creates new minefile template

					var emptyBytes = new List<byte>(); // the category files are empty to not take up space
					byte[] emptyBytesArray = emptyBytes.ToArray();

					mf.data = emptyBytesArray;//adds file data to minefile
					mf.filesize = mf.data.Length;//gets filesize for minefile
					mf.name = cat;//sets minfile name to file name
					mf.type = getCatID(cat);//sets minefile type to default
					TreeNode addNode = new TreeNode(mf.name) { Tag = mf };//creates node for minefile
					audioPCK.mineFiles.Add(mf);
					treeView1.Nodes.Add(addNode);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("All possible categories are used", "There are no more categories that could be added");
			}
		}

		private void addEntryMenuItem_Click(object sender, EventArgs e)
		{
			object[] obj = { defaultType, "New Entry" };
			
			TreeNode meta = new TreeNode();
			meta.Text = "New Entry";
			meta.Tag = obj;
			treeView2.Nodes.Insert(treeView2.SelectedNode == null ? 0 : treeView2.SelectedNode.Index + 1, meta);
			((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Add(obj);
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			cats.Remove(getCatID(treeView1.SelectedNode.Text));
			audioPCK.mineFiles.Remove((PCK.MineFile)treeView1.SelectedNode.Tag);
			treeView1.SelectedNode.Remove();
		}

		private void removeEntryMenuItem_Click(object sender, EventArgs e)
		{
			((PCK.MineFile)treeView1.SelectedNode.Tag).entries.Remove((object[])treeView2.SelectedNode.Tag);
			treeView2.SelectedNode.Remove();
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

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			FileOutput f = new FileOutput();
			f.Endian = Endianness.Big;

			f.writeInt(1);
			f.writeInt(audioPCK.types.Count);
			foreach (int type in audioPCK.types.Keys)
			{
				f.writeInt(type);
				writeMinecraftString(f, audioPCK.types[type]);
			}

			f.writeInt(audioPCK.mineFiles.Count);
			foreach (PCK.MineFile mf in audioPCK.mineFiles)
			{
				f.writeInt(mf.data.Length);
				f.writeInt(mf.type);
				writeMinecraftString(f, mf.name);
			}

			foreach (PCK.MineFile mf in audioPCK.mineFiles)
			{
				string missing = "";
				try
				{
					f.writeInt(mf.entries.Count);
					foreach (object[] entry in mf.entries)
					{
						missing = entry[0].ToString();
						f.writeInt(audioPCK.typeCodes[(string)entry[0]]);
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


			mf.data = f.getBytes();
		}
	}
}
