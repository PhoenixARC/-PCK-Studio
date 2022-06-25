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
		public string credits = "";
		public string defaultType = "yes";
		public string cat;
		public List<int> cats = new List<int>();

		public class NodeSorter : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null || y == null) return -1;
				return (x as TreeNode).ImageIndex.CompareTo((x as TreeNode).ImageIndex);
			}
		}

		internal static readonly List<string> categories = new List<string>
		{
			"Overworld",
			"Nether",
			"End",
			"Creative",
			"Menu",
			"Battle",
			"Tumble",
			"Glide",
			"Unused",
		};

		internal string getCatString(int cat)
		{
			if (cat > -1 && cat < categories.Count)
				return categories[cat];
			return "Not valid";
		}

		internal int getCatID(string cat)
		{
			return categories.IndexOf(cat);

			//switch (cat)
			//{
			//	case "Overworld": return 0;
			//	case "Nether": return 1;
			//	case "End": return 2;
			//	case "Creative": return 3;
			//	case "Menu": return 4;
			//	case "Battle": return 5;
			//	case "Tumble": return 6;
			//	case "Glide": return 7;
			//	case "Unused": return 8; // Unknown what this is used for. Probably the scrapped Mini Game 4 referenced in the code
			//	default: return -1;
			//}
		}

		PCKFile audioPCK;
		PCKFile.FileData mf;
		bool _isLittleEndian;
		public AudioEditor(PCKFile.FileData MineFile, LOCFile locFile, bool isLittleEndian)
		{
			_isLittleEndian = isLittleEndian;
			if (isLittleEndian) Text += " (PS4/Vita)";
			InitializeComponent();
			mf = MineFile;
			using (var stream = new MemoryStream(mf.data))
            {
				audioPCK = PCKFileReader.Read(stream, isLittleEndian);
            }
			if (!audioPCK.meta_data.Contains("CUENAME") || audioPCK.type != 1)
			{
				throw new Exception("This is not a valid audio.pck file");
			}
			foreach (PCKFile.FileData mineFile in audioPCK.file_entries)
			{
				string CatString = getCatString(mineFile.type);
				Console.WriteLine("Category Found: " + CatString + ". " + mineFile.type);
				foreach (var entry in mineFile.properties.ToArray())
				{
					var property = (ValueTuple<string, string>)entry;
                    if (property.Item1 == "CREDITID")
						locFile.RemoveEntry(property.Item2);
                    else if (property.Item1 == "CREDIT")
					{
						credits += property.Item2 + "\n";
						mineFile.properties.Remove(property);
					}
					else if (property.Item1 == "CREDITID")
					{
						mineFile.properties.Remove(property);
					}
				}
				if (cats.Contains(mineFile.type))
				{
					Console.WriteLine("Duplicate category found, " + CatString + ". Combining...");
					audioPCK.file_entries.Remove(mineFile);
					audioPCK.file_entries.Find(category => category.name == getCatString(mineFile.type)).properties = mineFile.properties;
				}
				else
				{
					TreeNode treeNode = new TreeNode(CatString);
					treeNode.Tag = mineFile;
                    treeNode.ImageIndex = mineFile.type;
                    treeNode.SelectedImageIndex = mineFile.type;
                    treeView1.Nodes.Add(treeNode);
					cats.Add(mineFile.type);
				}
			}

			treeView1.TreeViewNodeSorter = new NodeSorter();
			treeView1.Sort();
		}

		private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (treeView2.SelectedNode.Tag == null || !(treeView2.SelectedNode.Tag is ValueTuple<string, string>)) return;
			PCKFile.FileData file = (PCKFile.FileData)treeView1.SelectedNode.Tag;
			var property = (ValueTuple<string, string>)treeView2.SelectedNode.Tag;
			int i = file.properties.IndexOf(property);

			//foreach (var metaType in audioPCK.meta_data)
				//comboBox1.Items.Add(metaType);
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

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (treeView2.SelectedNode != null)
			{
                //var strings = (Tuple<string, string>)treeView2.SelectedNode.Tag;
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
					addCategory add = new addCategory(this); //sets category adding dialog
					if (add.ShowDialog() == DialogResult.OK)

					add.Dispose();//diposes generated metadata adding dialog data
					if (!cats.Contains(getCatID(cat))) cats.Add(getCatID(cat));
					else return;
					PCKFile.FileData mf = new PCKFile.FileData(cat, getCatID(cat));
					TreeNode addNode = new TreeNode(mf.name) { Tag = mf };
                    audioPCK.file_entries.Add(mf);
                    treeView1.Nodes.Add(addNode);
					treeView1.Sort();
				}
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

			var file = treeView1.SelectedNode.Tag as PCKFile.FileData;
			TreeNode meta = new TreeNode("CUENAME");
            meta.Tag = file;
            treeView2.Nodes.Add(meta);
            file.properties.Add(new ValueTuple<string, string>(defaultType, "CUENAME"));
        }
		public void treeView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && treeView2.SelectedNode != null &&
				treeView2.SelectedNode.Tag is ValueTuple<string, string> &&
				treeView1.SelectedNode.Tag is PCKFile.FileData)
			{
				var file = treeView1.SelectedNode.Tag as PCKFile.FileData;
                var property = (ValueTuple<string, string>)treeView2.SelectedNode.Tag;
                if (file.properties.Remove(property))
					treeView2.SelectedNode.Remove();
            }
		}

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
			cats.Remove(getCatID(treeView1.SelectedNode.Text));
			if (audioPCK.file_entries.Remove((PCKFile.FileData)treeView1.SelectedNode.Tag))
			{
				treeView1.SelectedNode.Remove();
				treeView2.Nodes.Clear();
			}
			if(treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is PCKFile.FileData)
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
			if (treeView1.SelectedNode.Tag == null || !(treeView1.SelectedNode.Tag is PCKFile.FileData) ||
				!(treeView2.SelectedNode.Tag is ValueTuple<string, string>)) return;
			var file = treeView1.SelectedNode.Tag as PCKFile.FileData;
			file.properties.Remove((ValueTuple<string, string>)treeView2.SelectedNode.Tag);
            treeView2.SelectedNode.Remove();
		}

		private void Binka_DragDrop(object sender, DragEventArgs e)
		{
			if (treeView1.SelectedNode != null &&
				treeView1.SelectedNode.Tag is PCKFile.FileData)
			{
				string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
				var file = treeView1.SelectedNode.Tag as PCKFile.FileData;
				foreach (string binka in FileList)
				{
					if(Path.GetExtension(binka) == ".binka")
					{
						TreeNode meta = new TreeNode("CUENAME");
						var property = new ValueTuple<string, string>("CUENAME", Path.GetFileNameWithoutExtension(binka));
                        meta.Tag = property;
                        treeView2.Nodes.Add(meta);
                        file.properties.Add(property);
                    }
				}
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if(!cats.Contains(0) || !cats.Contains(1) || !cats.Contains(2))
			{
				MessageBox.Show("The game will crash upon loading your pack if the Overworld, Nether and End categories don't all exist.", "Mandatory Categories Missing");
				return;
			}

			bool emptyCat = false;

			foreach (PCKFile.FileData mf in audioPCK.file_entries)
				if(mf.properties.Count == 0) emptyCat = true;

			if (emptyCat)
			{
				MessageBox.Show("The game will crash upon loading your pack if a category is empty", "Empty Category");
				return;
			}

			using (StringReader reader = new StringReader(credits))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					ValueTuple<string, string> credit_entry = new ValueTuple<string, string>("CREDIT", line);
					audioPCK.file_entries[0].properties.Add(credit_entry);
					//Console.WriteLine(line);
				}
			}
			using (var stream = new MemoryStream())
            {
				PCKFileWriter.Write(stream, audioPCK, _isLittleEndian);
				mf.SetData(stream.ToArray());
            }
			saved = true;
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
				"You can edit the credits for the PCK in the Credits editor! No more managing credit IDs!\n\n" +
				"You can modify and create PSVita and PS4 audio pcks by clicking \"PS4/Vita\" in the \"Create -> Audio.pck\" context menu", "Help");
		}

		private void creditsEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			creditsEditor prompt = new creditsEditor(credits);
			prompt.ShowDialog();
			credits = prompt.Credits;
		}
	}
}
