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
		PCKFile audioPCK = null;
		PCKFile.FileData audioPCKFile;
		bool _isLittleEndian;
		public List<string> cats = new List<string>();

		public class NodeSorter : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null || y == null) return -1;
				return (x as TreeNode).ImageIndex.CompareTo((x as TreeNode).ImageIndex);
			}
		}

		public static readonly List<string> Categories = new List<string>
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


		public AudioEditor(PCKFile.FileData MineFile, LOCFile locFile, bool isLittleEndian)
		{
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
						locFile.RemoveLocKey(property.Item2);
					else if (property.Item1 == "CREDIT")
					{
						credits += property.Item2 + "\n";
						categoryFile.properties.Remove(property);
					}
					else if (property.Item1 == "CREDITID")
					{
						categoryFile.properties.Remove(property);
					}
				}
				if (cats.Contains(GetCategoryFromId(categoryFile.type)))
				{
					Console.WriteLine("Duplicate category found, " + CatString + ". Combining...");
					audioPCK.Files.Remove(categoryFile);
					audioPCK.Files.Find(category => category.name == GetCategoryFromId(categoryFile.type)).properties = categoryFile.properties;
				}
				else
				{
					TreeNode treeNode = new TreeNode(CatString);
					treeNode.Tag = categoryFile;
					treeNode.ImageIndex = categoryFile.type;
					treeNode.SelectedImageIndex = categoryFile.type;
					treeView1.Nodes.Add(treeNode);
					cats.Add(GetCategoryFromId(categoryFile.type));
				}
			}

			treeView1.TreeViewNodeSorter = new NodeSorter();
			treeView1.Sort();
		}

        private void AudioEditor_Load(object sender, EventArgs e)
        {

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
				string[] avalible = Categories.FindAll(str =>
				{
					return !cats.Contains(str);
				}).ToArray();
				addCategory add = new addCategory(avalible); //sets category adding dialog
				if (add.ShowDialog() == DialogResult.OK)
					cats.Add(add.Category);
				PCKFile.FileData file = new PCKFile.FileData(add.Category, GetCategoryId(add.Category));
				TreeNode addNode = new TreeNode(file.name) { Tag = file };
                audioPCK.Files.Add(file);
                treeView1.Nodes.Add(addNode);
				treeView1.Sort();
				add.Dispose(); // diposes generated metadata adding dialog data
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

		private void removeCategoryStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null) return; // makes sure you don't run this if there is nothing to delete
			cats.Remove(treeView1.SelectedNode.Text);
			if (audioPCK.Files.Remove((PCKFile.FileData)treeView1.SelectedNode.Tag))
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
			if(!cats.Contains(GetCategoryFromId(0)) ||
			   !cats.Contains(GetCategoryFromId(1)) ||
			   !cats.Contains(GetCategoryFromId(2)))
			{
				MessageBox.Show("The game will crash upon loading your pack if the Overworld, Nether and End categories don't all exist.", "Mandatory Categories Missing");
				return;
			}

			bool emptyCat = false;

			foreach (PCKFile.FileData mf in audioPCK.Files)
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
					audioPCK.Files[0].properties.Add(credit_entry);
					//Console.WriteLine(line);
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
			MessageBox.Show("Simply drag and drop BINKA audio files into the right tree to add them to the category selected on the left tree.\n\n" +
				"The \"Menu\" category will only play once when loading the pack, and never again.\n\n" +
				"The \"Creative\" category will only play songs listed in that category, and unlike other editions of Minecraft, will NOT play songs from the Overworld category. You can fix this by adding your overworld songs to the Creative category too.\n\n" +
				"The mini game categories will only play if you have your pack loaded in those mini games.\n\n" +
				"You can edit the credits for the PCK in the Credits editor! No more managing credit IDs!", "Help");
		}

		private void creditsEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			creditsEditor prompt = new creditsEditor(credits);
			prompt.ShowDialog();
			credits = prompt.Credits;
		}
    }
}
