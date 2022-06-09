
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using PckStudio.Properties;
using Ohana3DS_Rebirth.Ohana;
using PckStudio.Forms;
using System.Drawing.Imaging;
using RichPresenceClient;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.IO;
using PckStudio.Classes.IO.LOC;

namespace PckStudio
{
	public partial class FormMain : MetroFramework.Forms.MetroForm
	{
        string saveLocation = string.Empty; //Save location for pck file
        string PCKFilePath = "";
        string PCKFileBCKUP = "x";

		PCKFile currentPCK;//currently opened pck
		LOCFile l; //Locdata
		PCKFile.FileData mfLoc = new PCKFile.FileData("CURRENTLOCDATA", 6); //LOC minefile
        bool needsUpdate = false;
		bool saved = true;
		bool isTemplateFile = false;
		string appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";

		public FormMain()
		{
			InitializeComponent();
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = new Size(20, 20);
			imageList.Images.Add(Resources.ZZFolder);
			imageList.Images.Add(Resources.BINKA_ICON);
			imageList.Images.Add(Resources.IMAGE_ICON);
			imageList.Images.Add(Resources.LOC_ICON);
			imageList.Images.Add(Resources.PCK_ICON);
			imageList.Images.Add(Resources.ZUnknown);
			treeViewMain.ImageList = imageList;
			FormBorderStyle = FormBorderStyle.None;
			pckOpen.AllowDrop = true;
			RPC.Initialize("825875166574673940");
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.CheckFileExists = true;
				ofd.Filter = "PCK (Minecraft Console Package)|*.pck";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					saveLocation = ofd.FileName;
					currentPCK = openPck(ofd.FileName);
					fileEntryCountLabel.Text = "Files:" + currentPCK.file_entries.Count;
					loadEditor();
				}
			}
		}

		private PCKFile openPck(string filePath)
		{
			PCKFile pck = null;
			using (var fileStream = File.OpenRead(filePath))
			{
				isTemplateFile = false;
				saveLocation = filePath;
				pck = PCKFileReader.Read(fileStream, LittleEndianCheckBox.Checked);
			}
			return pck;
		}

		private void loadEditor()
		{
			treeViewMain.Nodes.Clear();
			treeViewMain.LabelEdit = false;
			addPasswordToolStripMenuItem.Enabled = true;
			foreach (var file_entry in currentPCK.file_entries)
			{
				if (file_entry.name != "0") continue;
				foreach (var pair in file_entry.properties)
				{
					addPasswordToolStripMenuItem.Enabled = !(pair.Item1 == "LOCK");
					if (pair.Item1 == "LOCK" && new pckLocked(pair.Item2).ShowDialog() != DialogResult.OK)
						return;
				}
			}
			foreach (var file_entry in currentPCK.file_entries)
			{
				Console.WriteLine(file_entry.name);
				TreeNode node = new TreeNode(file_entry.name);
				node.Tag = file_entry;
				treeViewMain.Nodes.Add(node);

				if (file_entry.type == 0 || file_entry.type == 1 || file_entry.type == 2) // skins, capes, textures
				{
					node.ImageIndex = 2;
					node.SelectedImageIndex = 2;
				}
                else if (file_entry.type == 5 || file_entry.type == 11) // Skins.pck / x16info.pck
				{
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
				}
				else if (file_entry.type == 6) // .loc 
				{
					node.ImageIndex = 3;
					node.SelectedImageIndex = 3;
				}
				else if (file_entry.type == 8) // audio / binka
				{
					node.ImageIndex = 1;
					node.SelectedImageIndex = 1;
				}
                else if (file_entry.type == 11) // Skins.pck
                {
					using (var stream = new MemoryStream(file_entry.data))
					{
						PCKFile subPCKfile = PCKFileReader.Read(stream, LittleEndianCheckBox.Checked);
						// TODO: load sub pck into tree and make it editable with ease
					}
				}
				else
				{
					node.ImageIndex = 5;
					node.SelectedImageIndex = 5;
				}
			}
            foreach (ToolStripMenuItem dropDownItem in fileToolStripMenuItem.DropDownItems)
            {
                dropDownItem.Enabled = true;
            }
            foreach (ToolStripMenuItem dropDownItem2 in editToolStripMenuItem.DropDownItems)
            {
                dropDownItem2.Enabled = true;
            }
			fileEntryCountLabel.Text = "Files:" + currentPCK.file_entries.Count.ToString();
			tabControl.SelectTab(1);
		}

		private void selectNode(object sender, TreeViewEventArgs e)
		{
			buttonEdit.Visible = false;
			// Sets preview image to "NO IMAGE" by default
			pictureBoxImagePreview.Image = (Image)Resources.NoImageFound;
			int pictureBoxMaxHeight = (tabPage1.Height / 2) - (tabPage1.Height / 10);
			pictureBoxImagePreview.Size = new Size(pictureBoxMaxHeight, pictureBoxMaxHeight);
			labelImageSize.Text = "";
			var node = e.Node;
			if (node.Tag == null || !(node.Tag is PCKFile.FileData)) return;
			PCKFile.FileData file = node.Tag as PCKFile.FileData;
			treeMeta.Nodes.Clear();
			comboBox1.Items.Clear();
			textBox1.Text = "";
			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type); //Adds available metadata names from metadatabase to the metacombo

			//Retrieves metadata for currently selected mineifile and displays it within metatreeview
			int boxes = 0;
			foreach (var entry in file.properties)
			{
				TreeNode meta = new TreeNode(entry.Item1);
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);

				//Check for if file contains model data
				if (entry.Item1 == "BOX")
				{
					boxes += 1;
					buttonEdit.Text = "EDIT BOXES";
					buttonEdit.Visible = true;
				}
				else if (entry.Item1 == "ANIM")
				{
					if ((entry.Item2 == "0x40000") || (entry.Item2 == "0x80000"))
					{
						buttonEdit.Text = "View Skin";
						buttonEdit.Visible = true;
					}
				}
			}

			//Check for Animated Texture
			if ((file.name.StartsWith("res/textures/blocks/") || file.name.StartsWith("res/textures/items/")) &&
				(!file.name.EndsWith("clock.png") && (!file.name.EndsWith("compass.png"))))
			{
				buttonEdit.Text = "EDIT TEXTURE ANIMATION";
				buttonEdit.Visible = true;
			}

			//If selected item is a image, its displayed with proper dimensions in image box
			if (Path.GetExtension(file.name) == ".png" || file.type == 0 || file.type == 1 || file.type == 2)
			{
				MemoryStream png = new MemoryStream(file.data); //Gets image data from minefile data
				Image skinPicture = Image.FromStream(png); //Constructs image data into image
				pictureBoxImagePreview.Image = skinPicture; //Sets image preview to image


				if (skinPicture.Size.Height == skinPicture.Size.Width / 2)
				{
					pictureBoxImagePreview.Size = new Size(pictureBoxMaxHeight * 2, pictureBoxMaxHeight); //Sets 64x32 ratio images to appear at largest relative size to program window size
					labelImageSize.Text = skinPicture.Size.Width.ToString() + "x" + skinPicture.Size.Height.ToString();
					return;
				}
				else if (skinPicture.Size.Height == skinPicture.Size.Width)
				{
					pictureBoxImagePreview.Size = new Size(pictureBoxMaxHeight, pictureBoxMaxHeight); //SWets 64x64 ratio images to appear at largest relative size to program window size
					labelImageSize.Text = skinPicture.Size.Width.ToString() + "x" + skinPicture.Size.Height.ToString();
					return;
				}
				else
				{
					//Sets images to appear at largest relative size to program window size
					Size maxDisplay = new Size((tabPage1.Size.Width / 2 - 5) / 3, (tabPage1.Size.Height / 2 - 5) / 3);
					if (skinPicture.Size.Width > maxDisplay.Width)
					{
						//calculate aspect ratio
						float aspect = skinPicture.Width / (float)skinPicture.Height;
						int newWidth, newHeight;

						//calculate new dimensions based on aspect ratio
						newWidth = (int)(maxDisplay.Height * aspect);
						newHeight = (int)(newWidth / aspect);

						//if one of the two dimensions exceed the box dimensions
						if (newWidth > skinPicture.Width || newHeight > skinPicture.Height)
						{
							//depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
							if (newWidth > newHeight)
							{
								newWidth = maxDisplay.Width;
								newHeight = (int)(newWidth / aspect);
							}
							else
							{
								newHeight = maxDisplay.Height;
								newWidth = (int)(newHeight * aspect);
							}
						}
						pictureBoxImagePreview.Size = new Size(newWidth, newHeight);
					}
					else if (skinPicture.Size.Height > maxDisplay.Height)
					{
						//calculate aspect ratio
						float aspect = skinPicture.Width / (float)skinPicture.Height;
						int newWidth, newHeight;

						//calculate new dimensions based on aspect ratio
						newWidth = (int)(maxDisplay.Width * aspect);
						newHeight = (int)(newWidth / aspect);

						//if one of the two dimensions exceed the box dimensions
						if (newWidth > skinPicture.Width || newHeight > skinPicture.Height)
						{
							//depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
							if (newWidth > newHeight)
							{
								newWidth = maxDisplay.Width;
								newHeight = (int)(newWidth / aspect);
							}
							else
							{
								newHeight = maxDisplay.Height;
								newWidth = (int)(newHeight * aspect);
							}
						}
						pictureBoxImagePreview.Size = new Size(newWidth, newHeight);
					}
					else
					{
						pictureBoxImagePreview.Size = new Size(skinPicture.Size.Width, skinPicture.Size.Height);
					}
					labelImageSize.Text = skinPicture.Size.Width.ToString() + "x" + skinPicture.Size.Height.ToString();
					return;
				}
			}
			else if (file.type == 6) // .loc
			{
				buttonEdit.Text = "EDIT LOC";
				buttonEdit.Visible = true;
			}
			else if (Path.GetExtension(file.name) == ".col" || file.type == 9)
			{
				buttonEdit.Text = "EDIT COLORS";
				buttonEdit.Visible = true;
			}
			else if (Path.GetFileName(file.name) == "audio.pck")
			{
				buttonEdit.Text = "EDIT MUSIC CUES";
				buttonEdit.Visible = true;
			}
		}

        #region Parses boxes and opens model generator
        public void editModel(PCKFile.FileData skin)
		{
			PCKProperties otherData = new PCKProperties();
            PCKProperties generatedData = new PCKProperties();
            foreach (var entry in skin.properties)
            {
                //parses and sorts
                if (entry.Item1 == "BOX" || entry.Item1 == "OFFSET")
                {
                    generatedData.Add(entry);
                    continue;
                }
                otherData.Add(entry);
            }
            skin.properties = otherData;
            generateModel generate = new generateModel(generatedData, new PictureBox());
            generate.ShowDialog(); //Opens Model Generator Dialog
            foreach (var entry in generatedData)
            {
                skin.properties.Add(entry);
            }
            treeMeta.Nodes.Clear();

			comboBox1.Items.Clear();
			textBox1.Text = "";

			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type);

			//Retrieves metadata for currently selected mineifile and displays it within metatreeview
			foreach (var entry in skin.properties)
			{
				TreeNode meta = new TreeNode(entry.Item1);
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}
		#endregion

		#region extracts pck entry
		private void extractToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(treeViewMain.SelectedNode.Nodes.Count > 0)
			{
				MessageBox.Show("Cannot extract folders!");
				return;
			}
			if (!(treeViewMain.SelectedNode.Tag is PCKFile.FileData)) return;
			SaveFileDialog exFile = new SaveFileDialog(); //extract location
			exFile.FileName = treeViewMain.SelectedNode.Text;
			exFile.Filter = Path.GetExtension(treeViewMain.SelectedNode.Text).Replace(".", "") + " File|*" + Path.GetExtension(treeViewMain.SelectedNode.Text);
			exFile.ShowDialog();
			string extractPath = exFile.FileName;

			if (!string.IsNullOrWhiteSpace(Path.GetDirectoryName(extractPath)))//Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
			{
				File.WriteAllBytes(extractPath, ((PCKFile.FileData)treeViewMain.SelectedNode.Tag).data);//extracts minefile data to directory

				//Generates metadata file in form of txt file if metadata for the file exists
				if (treeViewMain.SelectedNode.Tag.ToString() != "")
				{
					try
					{
						string metaData = "";
						PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;

						var ms = new MemoryStream(File.ReadAllBytes(extractPath).ToArray());

						MemoryStream ico = new MemoryStream();
						Bitmap bmp = new Bitmap(Image.FromFile(extractPath));
						bmp.Save(ico, ImageFormat.Png);

						foreach (var entry in file.properties)
						{
							metaData += entry.Item1 + ":" + entry.Item2 + Environment.NewLine;
						}

						File.WriteAllText(extractPath + ".txt", metaData);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					MessageBox.Show("File Extracted");//Verification that file extraction path was successful
				}
			}
		}
		#endregion

		private void SaveTemplate()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "PCK (Minecraft Console Package)|*.pck";
			saveFileDialog.DefaultExt = ".pck";
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				Save(saveFileDialog.FileName);
			}
		}

		private void Save(string FilePath)
        {
			foreach (var file in currentPCK.file_entries)
            {
				foreach(var property in file.properties)
                {
					// make sure the meta is valid
					if (!currentPCK.meta_data.Contains(property.Item1))
						currentPCK.meta_data.Add(property.Item1);
                }
            }

			using (var fs = File.OpenWrite(FilePath))
			{
				PCKFileWriter.Write(fs, currentPCK, LittleEndianCheckBox.Checked);
			}
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
			{
				MessageBox.Show("Invalid PCK File data"); // should never happen unless its a folder
			}
			PCKFile.FileData mf = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
			using (var ofd = new OpenFileDialog())
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					mf.SetData(File.ReadAllBytes(ofd.FileName));
				}
			}
			saved = false;
		}

		private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Removes selected from current pcks minefiles list and nodes
			if (treeViewMain.SelectedNode.Tag is PCKFile.FileData)
			{
				PCKFile.FileData mf = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
				treeViewMain.Nodes.Remove(treeViewMain.SelectedNode);
				currentPCK.file_entries.Remove(mf);
			}
			else
			{
				if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					foreach (TreeNode item in treeViewMain.SelectedNode.Nodes)
					{
						if (item.Tag == null || item.Nodes.Count > 0)
						{
							MessageBox.Show("Can't fully delete directory with subdirectories");
							return;
						}
						if (item.Tag is PCKFile.FileData) //makes sure selected node is a minefile
						{
							//removes minefile from minefile list
							PCKFile.FileData mf = (PCKFile.FileData)item.Tag;
							currentPCK.file_entries.Remove(mf);
							//removes minefile node
							item.Remove();
						}
					}
					treeViewMain.SelectedNode.Remove();
				}
			}
			saved = false;
		}

		#region renames pck entry from treeview and PCKFile.file_entries
		private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = treeViewMain.SelectedNode;
			RenamePrompt diag = new RenamePrompt(node);
			if (diag.ShowDialog(this) == DialogResult.OK)
				treeViewMain.SelectedNode.Text = Path.GetFileName(diag.NewText);
			diag.Dispose();
		}
		#endregion

		#region clones pck entry from treeview and PCKFile.FileDatas
		private void cloneFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag == null || !(treeViewMain.SelectedNode.Tag is PCKFile.FileData)) return;

			PCKFile.FileData mfO = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
			FileInfo mfCO = new FileInfo(mfO.name);

			string name = Path.GetDirectoryName(mfO.name).Replace("\\", "/") + "/" + Path.GetFileNameWithoutExtension(mfO.name) + "_clone" + mfCO.Extension;//sets minfile name to file name
			PCKFile.FileData mf = new PCKFile.FileData(name, mfO.type); //Creates new minefile template
			if (treeViewMain.SelectedNode.Parent == null && mf.name.StartsWith("/")) mf.name = mf.name.Remove(0, 1);
			mf.properties = mfO.properties;
			TreeNode add = new TreeNode(Path.GetFileName(mf.name)) { Tag = mf }; //creates node for minefile

			//Gets proper file icon for minefile
			if (Path.GetExtension(add.Text) == ".binka")
			{
				add.ImageIndex = 1;
				add.SelectedImageIndex = 1;
			}
			else if (Path.GetExtension(add.Text) == ".png")
			{
				add.ImageIndex = 2;
				add.SelectedImageIndex = 2;
			}
			else if (Path.GetExtension(add.Text) == ".loc")
			{
				add.ImageIndex = 3;
				add.SelectedImageIndex = 3;
			}
			else if (Path.GetExtension(add.Text) == ".pck")
			{
				add.ImageIndex = 4;
				add.SelectedImageIndex = 4;
			}
			else
			{
				add.ImageIndex = 5;
				add.SelectedImageIndex = 5;
			}

			currentPCK.file_entries.Insert(currentPCK.file_entries.IndexOf(mfO) + 1, mf); //inserts minefile into proper list index
			if (treeViewMain.SelectedNode.Parent == null) treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.Index + 1, add); //adds generated minefile node
			else treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.Index + 1, add);//adds generated minefile node to selected folder
		}
		#endregion

		#region adds file to treeview and PCKFile.FileDatas
		private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					PCKFile.FileData mf = new PCKFile.FileData(Path.GetFileName(ofd.FileName), 0);//Creates new minefile template
					mf.SetData(File.ReadAllBytes(ofd.FileName));
					TreeNode add = new TreeNode(mf.name) { Tag = mf };

					//Gets proper file icon for minefile
					if (Path.GetExtension(add.Text) == ".binka")
					{
						add.ImageIndex = 1;
						add.SelectedImageIndex = 1;
					}
					else if (Path.GetExtension(add.Text) == ".png")
					{
						add.ImageIndex = 2;
						add.SelectedImageIndex = 2;
					}
					else if (Path.GetExtension(add.Text) == ".loc")
					{
						add.ImageIndex = 3;
						add.SelectedImageIndex = 3;
					}
					else if (Path.GetExtension(add.Text) == ".pck")
					{
						add.ImageIndex = 4;
						add.SelectedImageIndex = 4;
					}
					else
					{
						add.ImageIndex = 5;
						add.SelectedImageIndex = 5;
					}

					if (treeViewMain.SelectedNode.Tag == null)//Detects if user selected a folder to add file to
					{
						treeViewMain.SelectedNode.Nodes.Add(add);//adds generated minefile node to selected folder
						currentPCK.file_entries.Insert(treeViewMain.SelectedNode.Nodes.Count - 1, mf);//inserts minefile into proper list index

						string itemPath = "";//item path template
						List<TreeNode> path = new List<TreeNode>();//directory template
						GetPathToRoot(treeViewMain.SelectedNode, path);//gets all parents nodes
						//generates minefile directory to properly store in minedata
						foreach (TreeNode dire in path)
						{
							itemPath += dire.Text + "/";
						}

						currentPCK.file_entries[treeViewMain.SelectedNode.Nodes.Count - 1].name = itemPath + treeViewMain.SelectedNode.Nodes[treeViewMain.SelectedNode.Nodes.Count - 1].Text;//updates minefile name with directory
					}
					else//adds minefile to root of the pck
					{
						currentPCK.file_entries.Add(mf);
						treeViewMain.Nodes.Add(add);
					}
				}
			}
			saved = false;
		}


		private void GetPathToRoot(TreeNode node, List<TreeNode> path)
		{
			//gets all parents nodes of a file
			if (node == null) return; // previous node was the root.
			else
			{
				path.Insert(0, node);
				GetPathToRoot(node.Parent, path);
			}
		}
		#endregion

		#region starts up form to create and add a new skin
		private void createSkinToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int i = treeViewMain.Nodes.Count - 1;//Gets index of last item in treeview
			int tempIDD; //sets variables for a temporary skin/cape id

			try
			{
				string tempID = treeViewMain.Nodes[i].Text.Remove(treeViewMain.Nodes[i].Text.Length - 4, 4);//gets id of last skin/cape in treeview if the last item is a skin or cape

				tempID = tempID.Remove(0, 8);//removes text from id

				tempIDD = int.Parse(tempID) + 1; //adds to skin/cape id index to presets the next skin/cape id
			}
			catch (Exception)
			{
				tempIDD = 00000000; //sets temporary id to 0 if an id can't be generated off the treeviews last item
			}
			PCKFile.FileData mf = mfLoc;//Sets loc minefile

			try
			{
				using (var stream = new MemoryStream(mf.data))
				{
					l = LOCFileReader.Read(stream);//sets loc data
				}
			}
			catch
			{
				//error handling for if pck doesn't have a loc file
				MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			PckStudio.addnewskin add = new PckStudio.addnewskin(l); //Sets dialog data for skin creator
			add.ShowDialog(); //opens skin creator
			if (add.useCape)
				currentPCK.file_entries.Add(add.cape);
			currentPCK.file_entries.Add(add.skin);
			using (var stream = new MemoryStream())
            {
				LOCFileWriter.Write(stream, l);
				mf.SetData(stream.ToArray());
			}
			add.Dispose();//disposes generated skin creator data
			saved = false;
			loadEditor();
		}
		#endregion

		PCKFile.FileData makeNewAudioPCK(bool isLittle)
		{
			// create actual valid pck file structure
			PCKFile audioPck = new PCKFile(1); // 1 = audio pck
			audioPck.meta_data.Add("CUENAME");
			audioPck.meta_data.Add("CREDIT");
			audioPck.meta_data.Add("CREDITID");
			for (int i = 0; i < 3; i++)
			{
				PCKFile.FileData mf = new PCKFile.FileData("", i);
				audioPck.file_entries.Add(mf);
            }

			// create a file data entry for current open pck file
			PCKFile.FileData audioFileData = new PCKFile.FileData("audio.pck", 8);
			using(var stream = new MemoryStream())
            {
				PCKFileWriter.Write(stream, audioPck, isLittle);
				audioFileData.SetData(stream.ToArray());
            }
            return audioFileData;
		}

		private void audiopckToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//treeViewToMineFiles(treeViewMain, currentPCK);
			List<string> filenames = new List<string>();
			foreach (TreeNode tNode in treeViewMain.Nodes)
			{
				filenames.Add(tNode.Text);
			}

			if (filenames.Contains("audio.pck"))
			{
				MessageBox.Show("There is already an audio.pck present in this file!", "Can't create audio.pck");
				return;
			}
			PCKFile.FileData audioMF = makeNewAudioPCK(false);
			TreeNode node = new TreeNode("audio.pck");
			node.Tag = audioMF;
			node.ImageIndex = 4;
			node.SelectedImageIndex = 4;
			Forms.Utilities.AudioEditor diag = new Forms.Utilities.AudioEditor(audioMF, LittleEndianCheckBox.Checked);
			diag.ShowDialog(this);
			if (diag.saved) treeViewMain.Nodes.Add(node);
			//treeViewToMineFiles(treeViewMain, currentPCK);
			diag.Dispose();
		}

		#region starts up form to create and add a animated texture
		private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "PNG Files | *.png";
				ofd.Title = "Select a PNG File";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						AnimationEditor diag = new AnimationEditor(treeViewMain, ofd.FileName);
						diag.ShowDialog(this);
						diag.Dispose();

						//treeViewToMineFiles(treeViewMain, currentPCK);

						treeMeta.Nodes.Clear();
						foreach (var type in currentPCK.meta_data)
							comboBox1.Items.Add(type);
					}
					catch
					{
						MessageBox.Show("Invalid animation data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}
			saved = false;
		}
		#endregion

		private void treeViewMain_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode == null ||
				treeViewMain.SelectedNode.Tag == null ||
				!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
				return;
			PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
			if (file.type == 6) // .loc
			{
				LOCFile l = null;
				using (var stream = new MemoryStream(file.data))
				{
					l = LOCFileReader.Read(stream);
				}
				var locedit = new LOCEditor(l);
				locedit.ShowDialog(this);
				if (locedit.wasModified)
				{
					using (var stream = new MemoryStream())
					{
						LOCFileWriter.Write(stream, l);
						file.SetData(stream.ToArray());
					}
				}
			}

			if (Path.GetFileName(file.name) == "audio.pck" || file.type == 8) // audio
			{
				try
				{
					Forms.Utilities.AudioEditor diag = new Forms.Utilities.AudioEditor(file, LittleEndianCheckBox.Checked);
					if (LittleEndianCheckBox.Checked) diag.Text += " (PS4/Vita)";
					diag.ShowDialog(this);
					diag.Dispose();
				}
				catch(Exception ex)
				{
					MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK,
					MessageBoxIcon.Error);
					return;
				}
			}

			//Checks to see if selected minefile is a col file
			if (Path.GetExtension(file.name) == ".col" || file.type == 9)
			{
				if (file.size == 0)
                {
					MessageBox.Show("No Color data found.", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}
				COLFile colFile = new COLFile();
				using (var stream = new MemoryStream(file.data))
                {
					colFile.Open(stream);
                }
				Forms.Utilities.COLEditor diag = new Forms.Utilities.COLEditor(colFile);
				if (diag.ShowDialog(this) == DialogResult.OK && diag.data.Length > 0)
					file.SetData(diag.data);
				diag.Dispose();
            }

			//Checks to see if selected minefile is a binka file
			//System.Threading.ThreadStart starter;

			//System.Threading.Thread binkam;
			if (Path.GetExtension(file.name) == ".binka")
			{
				MessageBox.Show(".binka Editor Coming Soon!");
			}
		}

		#region updates combo and text boxes for metadata when a metadata entry is selected
		private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var node = e.Node;
			if (node == null || !(node.Tag is ValueTuple<string, string>)) return;
			comboBox1.Items.Clear(); //Resets metadata combobox of selectable entry names
			var property = (ValueTuple<string, string>)node.Tag;
			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type);
			comboBox1.Text = property.Item1;
			textBox1.Text = property.Item2;
		}
		#endregion

		#region updates metadata when combo option is selected
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			saved = false;
		}
		#endregion

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode == null ||
				treeMeta.SelectedNode.Tag == null ||
				!(treeMeta.SelectedNode.Tag is ValueTuple<string, string>))
				return;
			var valuePair = (ValueTuple<string, string>)treeMeta.SelectedNode.Tag;
			valuePair.Item2 = textBox1.Text;
		}

		private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode != null && treeMeta.SelectedNode.Tag is ValueTuple<string, string> &&
				treeViewMain.SelectedNode.Tag is PCKFile.FileData)
			{
				var file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
				file.properties.Remove((ValueTuple<string, string>)treeMeta.SelectedNode.Tag);
				treeMeta.Nodes.Remove(treeMeta.SelectedNode);
			}
			saved = false;
		}

		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag == null ||
				!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
				return;
			PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;
			addMeta add = new addMeta(file);
			add.ShowDialog();
			add.Dispose(); 

			treeMeta.Nodes.Clear();
			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type);

			foreach (var entry in file.properties)
			{
				TreeNode meta = new TreeNode(entry.Item1);
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}

		#region moves node up and arranges minefile indexes
		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();

			if (treeViewMain.SelectedNode.Parent == null)
			{
				if (treeViewMain.SelectedNode.PrevNode == null) return;
				treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.PrevNode.Index, move);
				//removes node because a clone was inserted into its new index
				treeViewMain.SelectedNode.Remove();
			}
			else
			{
				if (treeViewMain.SelectedNode.PrevNode == null) return;
				treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.PrevNode.Index, move);
				//removes node because a clone was inserted into its new index
				treeViewMain.SelectedNode.Remove();
			}

			//treeViewToMineFiles(treeViewMain, currentPCK);

			treeViewMain.SelectedNode = move;

			saved = false;
		}
		#endregion

		#region moves node down and arranges minefile indexes
		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();

			if (treeViewMain.SelectedNode.Parent == null)
			{
				if (treeViewMain.SelectedNode.NextNode == null) return;
				treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.NextNode.Index + 1, move);
				//removes node because a clone was inserted into its new index
				treeViewMain.SelectedNode.Remove();
			}
			else
			{
				if (treeViewMain.SelectedNode.NextNode == null) return;
				treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.NextNode.Index + 1, move);
				//removes node because a clone was inserted into its new index
				treeViewMain.SelectedNode.Remove();
			}

			//treeViewToMineFiles(treeViewMain.top, currentPCK);

			treeViewMain.SelectedNode = move;

			saved = false;
		}
		#endregion

		#region drag and drop for main tree node

		public static void getChildren(List<TreeNode> Nodes, TreeNode Node)
		{
			foreach (TreeNode thisNode in Node.Nodes)
			{
				Nodes.Add(thisNode);
				getChildren(Nodes, thisNode);
			}
		}

		// Most of the code below is modified code from this link: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
		// - MattNL

		private void treeViewMain_ItemDrag(object sender, ItemDragEventArgs e)
		{

        }

        // Set the target drop effect to the effect 
        // specified in the ItemDrag event handler.
        private void treeViewMain_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		// Select the node under the mouse pointer to indicate the 
		// expected drop location.
		private void treeViewMain_DragOver(object sender, DragEventArgs e)
		{

        }

        private void treeViewMain_DragDrop(object sender, DragEventArgs e)
		{

        }

        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(TreeNode node1, TreeNode node2)
		{
			// Check the parent node of the second node.
			if (node2.Parent == null) return false;
			if (node2.Parent.Equals(node1)) return true;

			// If the parent node is not null or equal to the first node, 
			// call the ContainsNode method recursively using the parent of 
			// the second node.
			return ContainsNode(node1, node2.Parent);
		}

		#endregion

		#region Loads all pck metadata into a main metadatabase and opens manageable dialog for it
		private void metaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				PckStudio.meta edit = new PckStudio.meta(currentPCK);
				edit.TopMost = true;
				edit.TopLevel = true;
				edit.Show();
			}
			catch (Exception)
			{
				MessageBox.Show("No PCK Data Loaded");
			}
			saved = false;
		}
		#endregion

		#region opens presets
		private void addPresetToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			PCKFile.FileData file = (PCKFile.FileData)treeViewMain.SelectedNode.Tag;
			PckStudio.presetMeta add = new PckStudio.presetMeta(file);
			add.ShowDialog();
			add.Dispose();

			//reloads treemeta data
			treeMeta.Nodes.Clear();
			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type);

			foreach (var entry in file.properties)
			{
				TreeNode meta = new TreeNode(entry.Item1);
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}
		#endregion

		#region loads empty pck template

		private void InitializeSkinPack(int packID, int packVersion, string packName)
        {
			currentPCK = new PCKFile(3);
			currentPCK.meta_data.Add("PACKID");
			currentPCK.meta_data.Add("PACKVERSION");
			var zeroFile = new PCKFile.FileData("0", 4);
			zeroFile.properties.Add(("PACKID", packID.ToString()));
			zeroFile.properties.Add(("PACKVERSION", packVersion.ToString()));
			var loc = new PCKFile.FileData("localisation.loc", 6);
			var locFile = new LOCFile();
			locFile.InitializeDefault(packName);
			using (var stream = new MemoryStream())
			{
				LOCFileWriter.Write(stream, locFile);
				loc.SetData(stream.ToArray());
			}
			currentPCK.file_entries.Add(zeroFile);
			currentPCK.file_entries.Add(loc);
		}

		private void InitializeTexturePack()
        {
			InitializeSkinPack(0, 0, "no_name");
			var texturepackInfo = new PCKFile.FileData("x16/x16Info.pck", 5);
			texturepackInfo.properties.Add(("PACKID", "0"));
			texturepackInfo.properties.Add(("DATAPATH", "x16Data.pck"));
			currentPCK.file_entries.Add(texturepackInfo);
		}

		private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// make skin pack template
			InitializeSkinPack(new Random().Next(8000, 8000000), 0, "no_name");
			isTemplateFile = true;
			loadEditor();
		}
		private void texturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
			// make texture pack template
			InitializeTexturePack();
			isTemplateFile = true;
			loadEditor();
        }
		#endregion

		#region open advanced metadata bulk editing window
		private void advancedMetaAddingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//opens dialog for bulk minefile editing
			AdvancedOptions advanced = new AdvancedOptions(currentPCK);
			advanced.ShowDialog();
			advanced.Dispose();
			saved = false;
		}
		#endregion

		#region open program info/credits window
		private void programInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//open program info dialog
			programInfo info = new programInfo();
			info.ShowDialog();
			info.Dispose();
		}
		#endregion

		#region checks for updates
		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				RPC.SetRPC("Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
				timer1.Start();
				timer1.Enabled = true;
			}
			catch
			{
				Console.WriteLine("ERROR WITH RPC");
			}
#if DEBUG
			DBGLabel.Visible = true;
#else
			DBGLabel.Visible = false;
#endif
			//Makes sure appdata exists
			if (!Directory.Exists(appData))
			{
				Directory.CreateDirectory(appData);
			}

			if (!Directory.Exists(appData + "\\cache\\mods\\"))
			{
				Directory.CreateDirectory(appData + "\\cache\\mods\\");
			}
		}
        #endregion

        private void treeViewMain_KeyDown(object sender, KeyEventArgs e)
		{
			// TODO
        }

        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			try
			{
				//Extracts a chosen pck file to a chosen destincation
				OpenFileDialog ofd = new OpenFileDialog();
				FolderBrowserDialog sfd = new FolderBrowserDialog();
				ofd.CheckFileExists = true;
				ofd.Filter = "PCK (Minecraft Wii U Package)|*.pck";

				if (ofd.ShowDialog() == DialogResult.OK && sfd.ShowDialog() == DialogResult.OK)
				{
					PCKFile pckfile = null;
					using (var fs = File.OpenRead(ofd.FileName))
                    {
						pckfile = PCKFileReader.Read(fs, LittleEndianCheckBox.Checked);
					}
					foreach (PCKFile.FileData mf in pckfile.file_entries)
					{
						foreach (var entry in mf.properties)
						{
							// Check for lock on PCK File
							if (entry.Item1 == "LOCK" &&
								new pckLocked(entry.Item2).ShowDialog() != DialogResult.OK)
								return; // cancel extraction if password not provided		
						}
						FileInfo file = new FileInfo(sfd.SelectedPath + @"\" + mf.name);
						file.Directory.Create(); // If the directory already exists, this method does nothing.
						File.WriteAllBytes(sfd.SelectedPath + @"\" + mf.name, mf.data); //writes minefile to file
																						//attempts to generate reimportable metadata file out of minefiles metadata
						string metaData = "";

						foreach (var entry in mf.properties)
						{
							metaData += $"{entry.Item1}: {entry.Item2}{Environment.NewLine}";
						}

						File.WriteAllText(sfd.SelectedPath + @"\" + mf.name + ".txt", metaData);
					}
				}
			} catch (Exception)
			{
				MessageBox.Show("An Error occured while extracting PCK");
			}
		}

#region deletes metadata entries through the del key
		private void treeMeta_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete && treeMeta.SelectedNode != null && treeViewMain.SelectedNode.Tag is PCKFile.FileData)
			{
				//removes selected treemeta entry
				var file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
				file.properties.Remove((ValueTuple<string, string>)treeMeta.SelectedNode.Tag);
				treeMeta.Nodes.Remove(treeMeta.SelectedNode);

				//reloads treemeta data
				treeMeta.Nodes.Clear();
				foreach (var type in currentPCK.meta_data)
					comboBox1.Items.Add(type);

				foreach (var entry in file.properties)
				{
					TreeNode meta = new TreeNode(entry.Item1);
					meta.Tag = entry;
					treeMeta.Nodes.Add(meta);
				}
				saved = false;
			}
		}
#endregion

#region imports a folder of skins to pck
		private void importExtractedSkinsFolder(object sender, EventArgs e)
		{
			FolderBrowserDialog contents = new FolderBrowserDialog();//Creates folder browser instance

			if (contents.ShowDialog() == DialogResult.OK)
			{
				//checks to make sure selected path exist
				if (!Directory.Exists(contents.SelectedPath))
				{
					MessageBox.Show("Directory Lost");
					return;
				}

				string filepath = contents.SelectedPath;//sets filepath to selected path
				DirectoryInfo d = new DirectoryInfo(contents.SelectedPath);//sets directory info

				bool mashupStructure = false;//creates variable to indicate wether current pck skin structure is mashup or regular skin
				int skinsFolder = 0;//temporary index for skins folder for if structure is mashup

				//checks to see if pck contains a skins folder
				foreach (TreeNode item in treeViewMain.Nodes)
				{
					if (item.Text == "Skins")
					{
						mashupStructure = true;//sets mashup structure to true
						skinsFolder = item.Index;//keeps note of skins folder index
					}
				}

				//gets all png files in selected path
				foreach (var file in d.GetFiles("*.png"))
				{
					ListViewItem Import = new ListViewItem();//listviewitem to store temporary data
					Import.Text = file.Name.Remove(file.Name.Length - 4, 4);//gets file name without extension

					//sets minefile type based on wether cape or skin
					int type = 0;
					if (Import.Text.Remove(7, Import.Text.Length - 7) == "dlccape" || Import.Text.Remove(7, Import.Text.Length - 7) == "DLCCAPE")
					{
						type = 1;
					}
					PCKFile.FileData mfNew = new PCKFile.FileData("", type); //new minefile template
					mfNew.SetData(File.ReadAllBytes(contents.SelectedPath + @"\" + file.Name.Remove(file.Name.Length - 4, 4) + ".png"));//sets minefile data to image data of current skin

					TreeNode skin = new TreeNode(); //create template treenode for minefile

					currentPCK.file_entries.Add(mfNew);//adds new minefile to minefile list for skin

					//Sets minefile directory based on pcks structure/type
					if (mashupStructure == true)
					{
						mfNew.name = "Skins/" + Import.Text + ".png";
					}
					else
					{
						mfNew.name = Import.Text + ".png";
					}

					skin.Text = Import.Text + ".png";//adds file extension to minefile
					skin.Tag = mfNew;//sets nodes minefile data

					//presest variables for minefile skin data about to be imported
					string entryName = "";
					string entryValue = "";
					string locNameId = "";
					string locName = "";
					string locThemeId = "";
					string locTheme = "";
					bool entryStart = true;//assistant for parcing through metadata file data to import

					foreach (char entry in File.ReadAllText(contents.SelectedPath + @"\" + Import.Text + ".png.txt").ToList())
					{
						//imports current skins metadata from metadata file
						if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == true)
						{
							entryName += entry.ToString();
						}
						else if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == false)
						{
							entryValue += entry.ToString();
						}
						else if (entry.ToString() == ":" && entryStart == true)
						{
							entryStart = false;
						}
						else
						{
							//adds minefiles metadata and presets loc data for minefile
							mfNew.properties.Add(new ValueTuple<string, string>(entryName, entryValue));

							if (entryName == "DISPLAYNAMEID")
							{
								locNameId = entryValue;
							}

							if (entryName == "DISPLAYNAME")
							{
								locName = entryValue;
							}

							if (entryName == "THEMENAMEID")
							{
								locThemeId = entryValue;
							}

							if (entryName == "THEMENAME")
							{
								locTheme = entryValue;
							}

							//creates displayname id in loc file
							if (locNameId != "" && locName != "")
							{
								LOCFile l;

								try
								{
									//using (var stream = new MemoryStream(mf.data))
									//{
									//	l = LOCFileReader.Read(stream);//sets loc data
									//}
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								//l.AddEntry(locThemeId, locTheme);

								//using (var stream = new MemoryStream())
								//{
								//	LOCFileWriter.Write(stream, l);
								//	mfLoc.SetData(stream.ToArray());
								//}
								locNameId = "";
								locName = "";
							}

							//creates metadata id in loc file
							if (locThemeId != "" && locTheme != "")
							{
								LOCFile l;

								try
								{
									using (var stream = new MemoryStream(mfLoc.data))
									{
										l = LOCFileReader.Read(stream);//sets loc data
									}
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								//l.AddEntry(locThemeId, locTheme);
								//using (var stream = new MemoryStream(mf.data))
								//{
								//	LOCFileWriter.Write(stream, l);
								//	mfLoc.SetData(stream.ToArray());
								//}
								locThemeId = "";
								locTheme = "";
							}
							entryName = "";
							entryValue = "";
							entryStart = true;
						}
					}
					//sets file icon
					skin.ImageIndex = 2;
					skin.SelectedImageIndex = 2;
					//Adds new minefile node to a destination based on pcks skin structure type
					if (mashupStructure == true)
					{
						treeViewMain.Nodes[skinsFolder].Nodes.Add(skin);
					}
					else
					{
						treeViewMain.Nodes.Add(skin);
					}
				}
			}
			contents.Dispose();//disposes temporary data
			saved = false;
		}
#endregion

#region imports individual skin to pck
		private void importSkin(object sender, EventArgs e)
		{
			OpenFileDialog contents = new OpenFileDialog();
			contents.Title = "Select Extracted Skin Data File";
			contents.Filter = "Text Files (*.txt)|*.txt";

			if (contents.ShowDialog() == DialogResult.OK)
			{
				try
				{
					string skinNameImport = Path.GetFileName(contents.FileName); //Gets skin name
					ListViewItem Import = new ListViewItem(); //listviewitem to store temporary data
					Import.Text = skinNameImport.Remove(skinNameImport.Length - 4, 4); //gets file name without extension
					byte[] data = File.ReadAllBytes(contents.FileName.Remove(contents.FileName.Length - 4, 4));
					PCKFile.FileData mfNew = new PCKFile.FileData("no_name", 0);
					mfNew.SetData(data); //sets minefile data to image data of current skin

					bool mashupStructure = false;//creates variable to indicate wether current pck skin structure is mashup or regular skin
					int skinsFolder = 0;//temporary index for skins folder for if structure is mashup

					//checks to see if pck contains a skins folder
					foreach (TreeNode item in treeViewMain.Nodes)
					{
						if (item.Text == "Skins")
						{
							mashupStructure = true;
							skinsFolder = item.Index;
						}
					}

					TreeNode skin = new TreeNode();//create template treenode for minefile

					currentPCK.file_entries.Add(mfNew);//Adds minefile to minefile list
					if (mashupStructure == true)
					{
						mfNew.name = "Skins/" + Import.Text;
					}
					else
					{
						mfNew.name = Import.Text;
					}

					skin.Text = Import.Text;//sets nodes minefile name
					skin.Tag = mfNew;//sets nodes minefile data

					//presest variables for minefile skin data about to be imported
					string entryName = "";
					string entryValue = "";
					string locNameId = "";
					string locName = "";
					string locThemeId = "";
					string locTheme = "";
					bool entryStart = true;//assistant for parcing through metadata file data to import

					foreach (char entry in File.ReadAllText(contents.FileName).ToList())
					{
						//imports current skins metadata from metadata file
						if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == true)
						{
							entryName += entry.ToString();
						}
						else if (entry.ToString() != ":" && entry.ToString() != "\n" && entryStart == false)
						{
							entryValue += entry.ToString();
						}
						else if (entry.ToString() == ":" && entryStart == true)
						{
							entryStart = false;
						}
						else
						{
							mfNew.properties.Add(new ValueTuple<string, string>(entryName, entryValue));

							if (entryName == "DISPLAYNAMEID")
							{
								locNameId = entryValue;
							}

							if (entryName == "DISPLAYNAME")
							{
								locName = entryValue;
							}

							if (entryName == "THEMENAMEID")
							{
								locThemeId = entryValue;
							}

							if (entryName == "THEMENAME")
							{
								locTheme = entryValue;
							}

							//creates displayname id in loc file
							if (locNameId != "" && locName != "")
							{
								//LOCFile l;

								//try
								//{
								//	using (var stream = new MemoryStream(mf.data))
								//	{
								//		l = LOCFileReader.Read(stream); //sets loc data
								//	}
								//}
								//catch
								//{
								//	MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								//	return;
								//}
								//l.AddEntry(locThemeId, locTheme);
								//using (var stream = new MemoryStream())
								//{
								//	LOCFileWriter.Write(stream, l);
								//	mfLoc.SetData(stream.ToArray());
								//}
								locNameId = "";
								locName = "";
							}

							//creates metadata id in loc file
							if (locThemeId != "" && locTheme != "")
							{
								//LOCFile l;

								//try
								//{
								//	using (var stream = new MemoryStream(mf.data))
								//	{
								//		l = LOCFileReader.Read(stream);//sets loc data
								//	}
								//}
								//catch
								//{
								//	MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
								//	return;
								//}
								//l.AddEntry(locThemeId, locTheme);

								//using (var stream = new MemoryStream())
								//{
								//	LOCFileWriter.Write(stream, l);//sets loc data
								//	mfLoc.SetData(stream.ToArray());
								//}
								locThemeId = "";
								locTheme = "";
							}
							entryName = "";
							entryValue = "";
							entryStart = true;
						}
					}
					//sets file icon
					skin.ImageIndex = 2;
					skin.SelectedImageIndex = 2;
					//Adds new minefile node to a destination based on pcks skin structure type
					if (mashupStructure == true)
					{
						treeViewMain.Nodes[skinsFolder].Nodes.Add(skin);
					}
					else
					{
						treeViewMain.Nodes.Add(skin);
					}
				} catch (Exception)
				{
					MessageBox.Show("Something went wrong");//error handling
				}
			}
			contents.Dispose();//disposes temporary data
			saved = false;
		}
#endregion

#region adds folder/directory entry to pck
		private void folderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode NEW = new TreeNode("New Folder");
			NEW.ImageIndex = 0;
			NEW.SelectedImageIndex = 0;
			if (treeViewMain.SelectedNode != null && !(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
			{
				treeViewMain.SelectedNode.Nodes.Add(NEW);
			}
			else
			{
				treeViewMain.Nodes.Add(NEW);
			}
			saved = false;
		}
#endregion

		private void installationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//System.Diagnostics.Process.Start(hosturl + "pckStudio#install");
		}

		private void binkaConversionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
		}

		private void fAQToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			//System.Diagnostics.Process.Start(hosturl + "pckStudio#faq");
		}
// BIG TODO
#region converts and ports all skins in pck to mc bedrock format
		// items class for use in bedrock skin conversion
		public class Item
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}

		private void convertToBedrockToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openedPCKS.Visible == true && MessageBox.Show("Convert " + openedPCKS.SelectedTab.Text + " to a Bedrock Edition format?", "Convert", MessageBoxButtons.YesNo, MessageBoxIcon.None) == DialogResult.Yes)
			{
				try
				{
					string packName = openedPCKS.SelectedTab.Text.Remove(openedPCKS.SelectedTab.Text.Count() - 4, 4);//Determines skin packs name off of pck file name

					//Lets user choose were to put generated pack
					SaveFileDialog convert = new SaveFileDialog();
					convert.Filter = "PCK (Minecarft Bedrock DLC)|*.mcpack";
					convert.FileName = packName;
					
					if (convert.ShowDialog() == DialogResult.OK)
					{
						//creates directory for conversion
						string root = Path.GetDirectoryName(convert.FileName) + "\\" + packName;
						string rootFinal = Path.GetDirectoryName(convert.FileName) + "\\";

						//creates pack uuid off of the last skin id detected
						string uuid = "99999999"; //default

						//creates list of skin display names
						List<Item> skinDisplayNames = new List<Item>();

						//MessageBox.Show(root);//debug thingy to make sure filepath is correct

						//add all skins to a list
						List<PCKFile.FileData> skinsList = new List<PCKFile.FileData>();
						List<PCKFile.FileData> capesList = new List<PCKFile.FileData>();
						foreach (PCKFile.FileData skin in currentPCK.file_entries)
						{
							if (skin.name.Count() == 19)
							{
								if (skin.name.Remove(7, skin.name.Count() - 7) == "dlcskin")
								{
									skinsList.Add(skin);
									uuid = skin.name.Remove(12, 7);
									uuid = uuid.Remove(0, 7);
									uuid = "abcdefa" + uuid;
								}
								if (skin.name.Remove(7, skin.name.Count() - 7) == "dlccape")
								{
									capesList.Add(skin);
								}
							}
						}

						if (skinsList.Count() == 0)
						{
							MessageBox.Show("No skins were found");
							return;
						}

						Directory.CreateDirectory(root);//Creates directory for skin pack
						Directory.CreateDirectory(root + "/texts");//create directory for skin pack text files

						//create skins json file
						using (StreamWriter writeSkins = new StreamWriter(root + "/skins.json"))
						{
							writeSkins.WriteLine("{");
							writeSkins.WriteLine("  \"skins\": [");

							int skinAmount = 0;
							foreach (PCKFile.FileData newSkin in skinsList)
							{
								skinAmount += 1;
								string skinName = "skinName";
								string capePath = "";
								bool hasCape = false;

								foreach (var entry in newSkin.properties)
								{
									if (entry.Item1 == "DISPLAYNAME")
									{
										skinName = entry.Item2;
										skinDisplayNames.Add(new Item() { Id = newSkin.name.Remove(15, 4), Name = skinName });
									}
									if (entry.Item1 == "CAPEPATH")
									{
										hasCape = true;
										capePath = entry.Item2.ToString();
									}
								}

								writeSkins.WriteLine("    {");
								writeSkins.WriteLine("      \"localization_name\": " + "\"" + newSkin.name.Remove(15, 4) + "\",");

								MemoryStream png = new MemoryStream(newSkin.data); //Gets image data from minefile data
								Image skinPicture = Image.FromStream(png); //Constructs image data into image
								if (skinPicture.Height == skinPicture.Width)
								{
									writeSkins.WriteLine("      \"geometry\": \"geometry." + packName + "." + newSkin.name.Remove(15, 4) + "\",");
								}
								writeSkins.WriteLine("      \"texture\": " + "\"" + newSkin.name + "\",");
								if (hasCape == true)
								{
									writeSkins.WriteLine("      \"cape\":" + "\"" + capePath + "\",");
								}
								writeSkins.WriteLine("      \"type\": \"free\"");
								if (skinAmount != skinsList.Count)
								{
									writeSkins.WriteLine("    },");
								}
								else
								{
									writeSkins.WriteLine("    }");
								}
							}

							writeSkins.WriteLine("  ],");
							writeSkins.WriteLine("  \"serialize_name\": \"" + packName + "\",");
							writeSkins.WriteLine("  \"localization_name\": \"" + packName + "\"");
							writeSkins.WriteLine("}");
						}

						//Create geometry file
						using (StreamWriter writeSkins = new StreamWriter(root + "/geometry.json"))
						{
							writeSkins.WriteLine("{");
							int newSkinCount = 0;
							foreach (PCKFile.FileData newSkin in skinsList)
							{

								newSkinCount += 1;
								string skinType = "steve";
								MemoryStream png = new MemoryStream(newSkin.data); //Gets image data from minefile data
								Image skinPicture = Image.FromStream(png); //Constructs image data into image

								if (skinPicture.Height == skinPicture.Width / 2)
								{
									skinType = "64x32";
									continue;
								}

								double offsetHead = 0;
								double offsetBody = 0;
								double offsetArms = 0;
								double offsetLegs = 0;

								//creates list of skin model data
								List<Item> modelDataHead = new List<Item>();
								List<Item> modelDataBody = new List<Item>();
								List<Item> modelDataLeftArm = new List<Item>();
								List<Item> modelDataRightArm = new List<Item>();
								List<Item> modelDataLeftLeg = new List<Item>();
								List<Item> modelDataRightLeg = new List<Item>();
								List<Item> modelData = new List<Item>();


								if (skinPicture.Height == skinPicture.Width)
								{
									//determines skin type based on image dimensions, existence of BOX tags, and the ANIM value
									foreach (var entry in newSkin.properties)
									{
										if (entry.Item1 == "BOX")
										{
											string mClass = "";
											string mData = "";
											foreach (char dCheck in entry.Item2)
											{
												if (dCheck.ToString() != " ")
												{
													mClass += dCheck.ToString();
												}
												else
												{
													mData = entry.Item2.Remove(0, mClass.Count() + 1);
													break;
												}
											}

											if (mClass == "HEAD")
											{
												mClass = "head";
												modelDataHead.Add(new Item() { Id = mClass, Name = mData });
											}
											else if (mClass == "BODY")
											{
												mClass = "body";
												modelDataBody.Add(new Item() { Id = mClass, Name = mData });
											}
											else if (mClass == "ARM0")
											{
												mClass = "rightArm";
												modelDataRightArm.Add(new Item() { Id = mClass, Name = mData });
											}
											else if (mClass == "ARM1")
											{
												mClass = "leftArm";
												modelDataLeftArm.Add(new Item() { Id = mClass, Name = mData });
											}
											else if (mClass == "LEG0")
											{
												mClass = "leftLeg";
												modelDataLeftLeg.Add(new Item() { Id = mClass, Name = mData });
											}
											else if (mClass == "LEG1")
											{
												mClass = "rightLeg";
												modelDataRightLeg.Add(new Item() { Id = mClass, Name = mData });
											}
										}

										if (entry.Item1 == "OFFSET")
										{
											string oClass = "";
											string oData = "";
											foreach (char oCheck in entry.Item2.ToString())
											{
												oData = entry.Item2.ToString();
												if (oCheck.ToString() != " ")
												{
													oClass += oCheck.ToString();
												}
												else
												{
													break;
												}

												if (oClass == "HEAD")
												{
													offsetHead += Double.Parse(oData.Remove(0, 7)) * -1;
												}
												else if (oClass == "BODY")
												{
													offsetBody += Double.Parse(oData.Remove(0, 7)) * -1;
												}
												else if (oClass == "ARM0")
												{
													offsetArms += Double.Parse(oData.Remove(0, 7)) * -1;
												}
												else if (oClass == "LEG0")
												{
													offsetLegs += Double.Parse(oData.Remove(0, 7)) * -1;
												}
											}
										}

										if (entry.Item1 == "ANIM")
										{
											if (entry.Item2 == "0x40000")
											{

											}
											else if (entry.Item2 == "0x80000")
											{
												skinType = "alex";
											}
										}
									}

									if (modelDataHead.Count + modelDataBody.Count + modelDataLeftArm.Count + modelDataRightArm.Count + modelDataLeftLeg.Count + modelDataRightLeg.Count > 0)
									{
										skinType = "custom";
									}
								}

								writeSkins.WriteLine("  \"" + "geometry." + packName + "." + newSkin.name.Remove(15, 4) + "\": {");

								//makes skin model depending on what skin type the skin is
								if (skinType == "custom")
								{
									writeSkins.WriteLine("    \"bones\": [");

									//Head Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ 0, 24, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each head box
									int modelAmount = 0;
									foreach (Item model in modelDataHead)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}

										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetHead + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + (Double.Parse(ys)) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A HEAD BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataHead.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "clothing" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "head" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        },");


									//Body Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ 0, 12, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each body box
									modelAmount = 0;
									foreach (Item model in modelDataBody)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}
										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo)) + ", " + ((Double.Parse(yo) + 0) * -1 + offsetBody + 24 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A BODY BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataBody.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "body" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        },");


									//LeftArm Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ 5, 22, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each arm1 box
									modelAmount = 0;
									foreach (Item model in modelDataLeftArm)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}
										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A ARM0 BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataLeftArm.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "leftArm" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        },");

									//RightArm Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ -5, 22, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each arm0 box
									modelAmount = 0;
									foreach (Item model in modelDataRightArm)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}
										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 5) + ", " + ((Double.Parse(yo)) * -1 + offsetArms + 22 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A ARM1 BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataRightArm.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "rightArm" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        },");

									//LeftLeg Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ 1.9, 12, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each leg1 box
									modelAmount = 0;
									foreach (Item model in modelDataLeftLeg)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}
										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) - 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A LEG1 BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataLeftLeg.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "leftLeg" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        },");

									//RightLeg Data
									writeSkins.WriteLine("      {");
									writeSkins.WriteLine("        \"pivot\": [ -1.9, 12, 0 ],");
									writeSkins.WriteLine("         \"rotation\": [ 0, 0, 0 ],");
									writeSkins.WriteLine("          \"cubes\": [ ");
									//Creates bones for each leg0 box
									modelAmount = 0;
									foreach (Item model in modelDataRightLeg)
									{
										modelAmount += 1;

										string xo = "";
										string yo = "";
										string zo = "";
										string xs = "";
										string ys = "";
										string zs = "";
										string xv = "";
										string yv = "";

										int spaceCheck = 0;

										foreach (char value in model.Name.ToString())
										{
											//0X1Y2Z3X4Y5Z6X7Y
											if (value.ToString() != " " && spaceCheck == 0)
											{
												xo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 1)
											{
												yo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 2)
											{
												zo += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 3)
											{
												xs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 4)
											{
												ys += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 5)
											{
												zs += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 6)
											{
												xv += value.ToString();
											}
											else if (value.ToString() != " " && spaceCheck == 7)
											{
												yv += value.ToString();
											}
											else if (value.ToString() == " ")
											{
												spaceCheck += 1;
											}
										}
										writeSkins.WriteLine("           {");
										try
										{
											writeSkins.WriteLine("            \"origin\": [ " + (Double.Parse(xo) + 1.9) + ", " + ((Double.Parse(yo)) * -1 + offsetLegs + 12 - Double.Parse(ys)) + ", " + (Double.Parse(zo)) + " ],");
											writeSkins.WriteLine("            \"size\": [ " + Double.Parse(xs) + ", " + Double.Parse(ys) + ", " + Double.Parse(zs) + " ],");
											writeSkins.WriteLine("            \"uv\": [ " + Double.Parse(xv) + ", " + Double.Parse(yv) + " ],");
											writeSkins.WriteLine("            \"inflate\": 0,");
											writeSkins.WriteLine("            \"mirror\": false");
										}
										catch (Exception)
										{
											MessageBox.Show("A LEG0 BOX tag in " + newSkin.name + " has an invalid value!");
										}
										if (modelAmount != modelDataRightLeg.Count)
										{
											writeSkins.WriteLine("    },");
										}
										else
										{
											writeSkins.WriteLine("    }");
										}
									}
									writeSkins.WriteLine("        ],");
									writeSkins.WriteLine("        \"META_BoneType\": \"" + "base" + "\",");
									writeSkins.WriteLine("        \"name\": \"" + "rightLeg" + "\",");
									writeSkins.WriteLine("        \"parent\":" + " null");
									writeSkins.WriteLine("        }");
									writeSkins.WriteLine("    ],");
								}
								else if (skinType == "64x32")
								{
									writeSkins.Write("    \"bones\": [ ],");
								}
								else if (skinType == "steve")
								{
									writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -8, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 22, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 15, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
								}
								else if (skinType == "alex")
								{
									writeSkins.Write("    \"bones\": [ " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"body\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"belt\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"head\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 24, -4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 8, 8 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.5, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"hat\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmet\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 32, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArm\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 4, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 48, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -7, 11.5, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 3, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 40, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightSleeve\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 16 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"base\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLeg\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": null " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegging\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -0.1, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 48 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -3.9, 0, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 4, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 0, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightPants\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [  " + Environment.NewLine + "  " + Environment.NewLine + "            { " + Environment.NewLine + "  " + Environment.NewLine + "             \"origin\": [ -4, 12, -2 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"size\": [ 8, 12, 4 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"uv\": [ 16, 32 ], " + Environment.NewLine + "  " + Environment.NewLine + "             \"inflate\": 0.25, " + Environment.NewLine + "  " + Environment.NewLine + "             \"mirror\": false " + Environment.NewLine + "  " + Environment.NewLine + "           } " + Environment.NewLine + "  " + Environment.NewLine + "         ], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"clothing\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"jacket\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"helmetArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"head\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 24, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"bodyArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 5, 21.5, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftArmArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 0, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"waist\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"body\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftLegArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 1.9, 12, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"armor_offset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftBootArmorOffset\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftLeg\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ -6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"rightItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"rightArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       }, " + Environment.NewLine + "  " + Environment.NewLine + "       { " + Environment.NewLine + "  " + Environment.NewLine + "         \"pivot\": [ 6, 14.5, 1 ], " + Environment.NewLine + "  " + Environment.NewLine + "          \"rotation\": [ 0, 0, 0 ], " + Environment.NewLine + "  " + Environment.NewLine + "           \"cubes\": [], " + Environment.NewLine + "  " + Environment.NewLine + "         \"META_BoneType\": \"item\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"name\": \"leftItem\", " + Environment.NewLine + "  " + Environment.NewLine + "         \"parent\": \"leftArm\" " + Environment.NewLine + "  " + Environment.NewLine + "       } " + Environment.NewLine + "  " + Environment.NewLine + "     ],");
								}


								writeSkins.WriteLine("    \"texturewidth\": 64 , ");
								writeSkins.WriteLine("    \"textureheight\": 64,");
								writeSkins.WriteLine("    \"META_ModelVersion\": \"1.0.6\",");
								writeSkins.WriteLine("    \"rigtype\": \"normal\",");
								writeSkins.WriteLine("    \"animationArmsDown\": false,");
								writeSkins.WriteLine("    \"animationArmsOutFront\": false,");
								writeSkins.WriteLine("    \"animationStatueOfLibertyArms\": false,");
								writeSkins.WriteLine("    \"animationSingleArmAnimation\": false,");
								writeSkins.WriteLine("    \"animationStationaryLegs\": false,");
								writeSkins.WriteLine("    \"animationSingleLegAnimation\": false,");
								writeSkins.WriteLine("    \"animationNoHeadBob\": false,");
								writeSkins.WriteLine("    \"animationDontShowArmor\": false,");
								writeSkins.WriteLine("    \"animationUpsideDown\": false,");
								writeSkins.WriteLine("    \"animationInvertedCrouch\": false");
								if (newSkinCount != skinsList.Count)
								{
									writeSkins.WriteLine("  },");
								}
								else
								{
									writeSkins.WriteLine("  }");
								}
							}
							Console.WriteLine(writeSkins);
						}
						Random rnd = new Random();
						int month = rnd.Next(1, 13); // creates a number between 1 and 12
						int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
						int card = rnd.Next(52);

						string randomPlus = month.ToString() + dice.ToString() + card.ToString();
						if (randomPlus.Count() > 12)
						{
							randomPlus.Remove(0, randomPlus.Count() - 12);
						}
						else if (randomPlus.Count() < 12)
						{
							int ii = 12 - randomPlus.Count();
							for (int i = 0; i < ii; i++)
							{
								randomPlus += 0;
							}
						}
						else if (randomPlus.Count() == 12)
						{
						}

						//Create Manifest file
						using (StreamWriter writeSkins = new StreamWriter(root + "/manifest.json"))
						{
							writeSkins.WriteLine("{");
							writeSkins.WriteLine("  \"header\": {");
							writeSkins.WriteLine("    \"version\": [");
							writeSkins.WriteLine("      1,");
							writeSkins.WriteLine("      0,");
							writeSkins.WriteLine("      0");
							writeSkins.WriteLine("    ],");
							writeSkins.WriteLine("    \"description\": \"Template by Ultmate_Mario, Conversion by Nobledez\",");
							writeSkins.WriteLine("    \"name\": \"" + packName + "\",");
							writeSkins.WriteLine("    \"uuid\": \"" + uuid.Remove(0, 4) + "-" + uuid.Remove(0, 8) + "-" + uuid.Remove(1, 8) + "-" + uuid.Remove(2, 8) + "-" + randomPlus + "\""); //8-4-4-4-12
							writeSkins.WriteLine("  },");
							writeSkins.WriteLine("  \"modules\": [");
							writeSkins.WriteLine("    {");
							writeSkins.WriteLine("      \"version\": [");
							writeSkins.WriteLine("        1,");
							writeSkins.WriteLine("        0,");
							writeSkins.WriteLine("        0");
							writeSkins.WriteLine("      ],");
							writeSkins.WriteLine("      \"type\": \"skin_pack\",");
							writeSkins.WriteLine("      \"uuid\": \"8dfd1d65-b3ca-4726-b9e0-9b46a40b72a4\"");
							writeSkins.WriteLine("    }");
							writeSkins.WriteLine("  ],");
							writeSkins.WriteLine("  \"format_version\": 1");
							writeSkins.WriteLine("}");
						}

						//create lang file
						using (StreamWriter writeSkins = new StreamWriter(root + "/texts/en_US.lang"))
						{
							writeSkins.WriteLine("skinpack." + packName + "=" + Path.GetFileNameWithoutExtension(convert.FileName));
							foreach (Item displayName in skinDisplayNames)
							{
								writeSkins.WriteLine("skin." + packName + "." + displayName.Id + "=" + displayName.Name);
							}
						}

						//adds skin textures
						foreach (PCKFile.FileData skinTexture in skinsList)
						{
							var ms = new MemoryStream(skinTexture.data);
							Bitmap saveSkin = new Bitmap(Image.FromStream(ms));
							if (saveSkin.Width == saveSkin.Height)
							{
								ResizeImage(saveSkin, 64, 64);
							}
							else if (saveSkin.Height == saveSkin.Width / 2)
							{
								ResizeImage(saveSkin, 64, 32);
							}
							else
							{
								ResizeImage(saveSkin, 64, 64);
							}
							saveSkin.Save(root + "/" + skinTexture.name, ImageFormat.Png);
						}

						//adds cape textures
						foreach (PCKFile.FileData capeTexture in capesList)
						{
							File.WriteAllBytes(root + "/" + capeTexture.name, capeTexture.data);
						}

						string startPath = root;
						string zipPath = rootFinal + "content.zipe";

						try
						{
							ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
						}catch (Exception)
						{
							File.Delete(zipPath);
							ZipFile.CreateFromDirectory(startPath, zipPath);//Creates contents zipe
						}

						rootFinal = root + "temp/";
						Directory.CreateDirectory(rootFinal);
						File.Move(zipPath, rootFinal + "content.zipe");
						File.Copy(root + "/manifest.json", rootFinal + "/manifest.json");
						ZipFile.CreateFromDirectory(rootFinal, convert.FileName);//Creates mcpack
						Directory.Delete(root, true);
						Directory.Delete(rootFinal, true);

						MessageBox.Show("Conversion Complete");
					}
				}
				catch (Exception convertEr)
				{
					MessageBox.Show(convertEr.ToString());
				}
			}
			else if (openedPCKS.Visible == false)
			{
				MessageBox.Show("Open PCK file first!");
			}
		}


		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
#endregion

#region 3ds feature in testing

		private struct loadedTexture
		{
			public bool modified;
			public uint gpuCommandsOffset;
			public uint gpuCommandsWordCount;
			public uint offset;
			public int length;
			public RenderBase.OTexture texture;
		}

		private struct loadedMaterial
		{
			public string texture0;
			public string texture1;
			public string texture2;
			public uint gpuCommandsOffset;
			public uint gpuCommandsWordCount;
		}

		private class loadedBCH
		{
			public uint mainHeaderOffset;
			public uint gpuCommandsOffset;
			public uint dataOffset;
			public uint relocationTableOffset;
			public uint relocationTableLength;
			public List<loadedTexture> textures;
			public List<loadedMaterial> materials;

			public loadedBCH()
			{
				textures = new List<loadedTexture>();
				materials = new List<loadedMaterial>();
			}
		}

		private byte[] align(byte[] input)
		{
			int length = input.Length;
			while ((length & 0x7f) > 0) length++;
			byte[] output = new byte[length];
			Buffer.BlockCopy(input, 0, output, 0, input.Length);
			return output;
		}

		private void replaceData(Stream data, uint offset, int length, byte[] newData)
		{
			data.Seek(offset + length, SeekOrigin.Begin);
			byte[] after = new byte[data.Length - data.Position];
			data.Read(after, 0, after.Length);
			data.SetLength(offset);
			data.Seek(offset, SeekOrigin.Begin);
			data.Write(newData, 0, newData.Length);
			data.Write(after, 0, after.Length);
		}

		private void updateTexture(int index, loadedTexture newTex)
		{
			bch.textures.RemoveAt(index);
			bch.textures.Insert(index, newTex);
		}

		private void replaceCommand(Stream data, BinaryWriter output, uint newVal)
		{
			data.Seek(-8, SeekOrigin.Current);
			output.Write(newVal);
			data.Seek(4, SeekOrigin.Current);
		}

		private void updateAddress(Stream data, BinaryReader input, BinaryWriter output, int diff)
		{
			uint offset = input.ReadUInt32();
			offset = (uint)(offset + diff);
			data.Seek(-4, SeekOrigin.Current);
			output.Write(offset);
		}

		loadedBCH bch;

        private void create3dstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewMain.SelectedNode != null)
            {
				loadedTexture tex = new loadedTexture();

				SaveFileDialog exportDs = new SaveFileDialog();
                exportDs.ShowDialog();
                string currentFile = exportDs.FileName;

                bch = new loadedBCH();

                using (FileStream data = new FileStream(currentFile, FileMode.Open))
                {
                    BinaryReader input = new BinaryReader(data);
                    BinaryWriter output = new BinaryWriter(data);

                    MemoryStream png = new MemoryStream(((PCKFile.FileData)(treeViewMain.SelectedNode.Tag)).data); //Gets image data from minefile data
                    Image skinPicture = Image.FromStream(png); //Constructs image data into image
                    pictureBoxImagePreview.Image = skinPicture; //Sets image preview to image

                    byte[] buffer = new byte[skinPicture.Width * skinPicture.Height * 4];
                    input.Read(buffer, 0, buffer.Length);
                    Bitmap texture = TextureCodec.decode(buffer, skinPicture.Width, skinPicture.Height, RenderBase.OTextureFormat.rgba8);
                    tex.texture = new RenderBase.OTexture(texture, "Texure");

                    //tex.texture = treeViewMain.SelectedNode.Tag;

                    for (int i = 0; i < bch.textures.Count; i++)
                    {
                        tex = bch.textures[i];
                        tex.modified = true;

                        if (tex.modified)
                        {
                            byte[] bufferx = align(TextureCodec.encode(tex.texture.texture, RenderBase.OTextureFormat.rgba8));
                            int diff = bufferx.Length - tex.length;

                            replaceData(data, tex.offset, tex.length, bufferx);

                            //Update offsets of next textures
                            tex.length = bufferx.Length;
                            tex.modified = false;
                            updateTexture(i, tex);
                            for (int j = i; j < bch.textures.Count; j++)
                            {
                                loadedTexture next = bch.textures[j];
                                next.offset = (uint)(next.offset + diff);
                                updateTexture(j, next);
                            }

                            //Update all addresses poiting after the replaced data
                            bch.relocationTableOffset = (uint)(bch.relocationTableOffset + diff);
                            for (int index = 0; index < bch.relocationTableLength; index += 4)
                            {
                                data.Seek(bch.relocationTableOffset + index, SeekOrigin.Begin);
                                uint value = input.ReadUInt32();
                                uint offset = value & 0x1ffffff;
                                byte flags = (byte)(value >> 25);

                                if ((flags & 0x20) > 0 || flags == 7 || flags == 0xc)
                                {
                                    if ((flags & 0x20) > 0)
                                        data.Seek((offset * 4) + bch.gpuCommandsOffset, SeekOrigin.Begin);
                                    else
                                        data.Seek((offset * 4) + bch.mainHeaderOffset, SeekOrigin.Begin);

                                    uint address = input.ReadUInt32();
                                    if (address + bch.dataOffset > tex.offset)
                                    {
                                        address = (uint)(address + diff);
                                        data.Seek(-4, SeekOrigin.Current);
                                        output.Write(address);
                                    }
                                }
                            }

                            uint newSize = (uint)((tex.texture.texture.Width << 16) | tex.texture.texture.Height);

                            //Update texture format
                            data.Seek(tex.gpuCommandsOffset, SeekOrigin.Begin);
                            for (int index = 0; index < tex.gpuCommandsWordCount * 3; index++)
                            {
                                uint command = input.ReadUInt32();

                                switch (command)
                                {
                                    case 0xf008e:
                                    case 0xf0096:
                                    case 0xf009e:
                                        replaceCommand(data, output, 0); //Set texture format to 0 = RGBA8888
                                        break;
                                    case 0xf0082:
                                    case 0xf0092:
                                    case 0xf009a:
                                        replaceCommand(data, output, newSize); //Set new texture size
                                        break;
                                }
                            }

                            //Update material texture format
                            foreach (loadedMaterial mat in bch.materials)
                            {
                                data.Seek(mat.gpuCommandsOffset, SeekOrigin.Begin);
                                for (int index = 0; index < mat.gpuCommandsWordCount; index++)
                                {
                                    uint command = input.ReadUInt32();

                                    switch (command)
                                    {
                                        case 0xf008e: if (mat.texture0 == tex.texture.name || mat.texture0 == "") replaceCommand(data, output, 0); break;
                                        case 0xf0096: if (mat.texture1 == tex.texture.name || mat.texture1 == "") replaceCommand(data, output, 0); break;
                                        case 0xf009e: if (mat.texture2 == tex.texture.name || mat.texture2 == "") replaceCommand(data, output, 0); break;
                                    }
                                }
                            }

                            //Patch up BCH header for new offsets and lengths
                            data.Seek(4, SeekOrigin.Begin);
                            byte backwardCompatibility = input.ReadByte();
                            byte forwardCompatibility = input.ReadByte();

                            //Update Data Extended and Relocation Table offsets
                            data.Seek(18, SeekOrigin.Current);
                            if (backwardCompatibility > 0x20) updateAddress(data, input, output, diff);
                            updateAddress(data, input, output, diff);

                            //Update data length
                            data.Seek(12, SeekOrigin.Current);
                            updateAddress(data, input, output, diff);
                        }
                    }
                    using (Stream file = File.Create(currentFile + ".tmp"))
                    {
                        CopyStream(output.BaseStream, file);
                    }

                }

                MessageBox.Show("Done!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        public static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[8 * 1024];
			int len;
			while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, len);
			}
		}

#endregion

#region Tool/MenuStrips

		private void openToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DateTime Begin = DateTime.Now;
			//pckCenter open = new pckCenter();
			Forms.Utilities.PckCenterBeta open = new Forms.Utilities.PckCenterBeta();
			open.Show();
			TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

			Console.WriteLine("Completed in: " + duration);
		}

        private void wiiUPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			installWiiU install = new installWiiU(null);
			install.ShowDialog();
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

		private void pCKCenterReleaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=E_6bXSh6yqw");
		}

		private void howPCKsWorkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.youtube.com/watch?v=hTlImrRrCKQ");
		}

		private void PS3PCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{

			installPS3 install = new installPS3(null);
			install.ShowDialog();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Pref setting = new Pref();
			setting.Show();
		}

		private void administrativeToolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PCK_Manager pckm = new PCK_Manager();
			pckm.Show();
		}

		private void VitaPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{

			installVita install = new installVita(null);
			install.ShowDialog();
		}

		private void toPhoenixARCDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://cash.app/$PhoenixARC");
		}

		private void toNobledezJackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.me/realnobledez");
		}

		private void addPasswordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeViewMain.SelectedNode = treeViewMain.Nodes[0];
			PCKFile.FileData file = (PCKFile.FileData)treeViewMain.Nodes[0].Tag;//Sets minefile to selected node
				foreach (var entry in file.properties)
				{
					if (entry.Item1 == "LOCK")
					{
						MessageBox.Show("Remove current LOCK before adding a new one!");
						return;
					}
				}
			AddPCKPassword add = new AddPCKPassword(file, currentPCK); //sets metadata adding dialog
			add.ShowDialog();
			add.Dispose();

			//Sets up combobox for metadata entries from main metadatabase
			treeMeta.Nodes.Clear();
			foreach (var type in currentPCK.meta_data)
				comboBox1.Items.Add(type);

			//loads all of selected minefiles metadata into metadata treeview
			foreach (var entry in file.properties)
			{
				TreeNode meta = new TreeNode(entry.Item1);
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}

		private void joinDevelopmentDiscordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/aJtZNFVQTv");
		}

		private void convertPCTextrurePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.TextureConverterUtility tex = new PckStudio.Forms.Utilities.TextureConverterUtility(treeViewMain, currentPCK);
			tex.ShowDialog();
		}

#endregion


		private void buttonEditModel_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode == null ||
				treeViewMain.SelectedNode.Tag == null ||
				!(treeViewMain.SelectedNode.Tag is PCKFile.FileData))
				return;
			PCKFile.FileData file = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
			if (file.type == 0 || file.type == 1 || file.type == 2)
			{
				if (buttonEdit.Text == "EDIT BOXES")
					editModel(file);
				else if (buttonEdit.Text == "View Skin")
				{
					using (var ms = new MemoryStream(file.data))
					{
						SkinPreview frm = new SkinPreview(Image.FromStream(ms));
						frm.ShowDialog(this);
						frm.Dispose();
					}
				}
			}

			//Check for Animated Texture
			if (file.name.StartsWith("res/textures/blocks/") || file.name.StartsWith("res/textures/items/"))
			{
				try
				{
					AnimationEditor diag = new AnimationEditor(treeViewMain);
					diag.ShowDialog(this);
					diag.Dispose();

					//treeViewToMineFiles(treeViewMain, currentPCK);

					MemoryStream png = new MemoryStream(file.data); //Gets image data from minefile data
					Image skinPicture = Image.FromStream(png); //Constructs image data into image
					pictureBoxImagePreview.Image = skinPicture;

					treeMeta.Nodes.Clear();
					foreach (var type in currentPCK.meta_data)
						comboBox1.Items.Add(type);

					//loads all of selected minefiles metadata into metadata treeview
					foreach (var entry in file.properties)
					{
						TreeNode meta = new TreeNode(entry.Item1);
						meta.Tag = entry;
						treeMeta.Nodes.Add(meta);
					}
				}
				catch
				{
					MessageBox.Show("Invalid animation data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			if (Path.GetFileName(file.name) == "audio.pck")
			{
				try
				{
					PckStudio.Forms.Utilities.AudioEditor diag = new PckStudio.Forms.Utilities.AudioEditor(file, LittleEndianCheckBox.Checked);
					if (file.data[0] != 0x00) diag.Text += " (PS4/Vita)";
					diag.ShowDialog(this);
					diag.Dispose();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK,
					MessageBoxIcon.Error);
					return;
				}
			}

			if (file.type == 6) // .loc file
			{
				LOCFile l = null;
				using (var stream = new MemoryStream(file.data))
				{
					l = LOCFileReader.Read(stream);
				}
				var locEditor = new LOCEditor(l);
				locEditor.ShowDialog();
				using (var stream = new MemoryStream())
                {
					LOCFileWriter.Write(stream, l);
					file.SetData(stream.ToArray());
                }
			}

			//Checks to see if selected minefile is a col file
			if (file.type == 9) // .col file
			{
				COLFile colFile = new COLFile();
				using (var stream = new MemoryStream(file.data))
				{
					colFile.Open(stream);
				}
				Forms.Utilities.COLEditor diag = new Forms.Utilities.COLEditor(colFile);
				if (diag.ShowDialog(this) == DialogResult.OK && diag.data.Length > 0)
					file.SetData(diag.data);
				diag.Dispose();
			}
		}


		private void OpenPck_MouseEnter(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckOpen;
		}

		private void OpenPck_MouseLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			checkSaveState();
			if (needsUpdate)
			{
				Process UPDATE = new Process(); //sets up updater
				UPDATE.StartInfo.FileName = appData + @"\nobleUpdater.exe"; //updater program path
				UPDATE.Start(); //starts updater
				Application.Exit(); //closes PCK Studio to let updatear finish the job
			}
		}

		private void checkSaveState()
        {
			if (!saved || isTemplateFile)
			{
				if (MessageBox.Show("Save PCK?", "Unsaved PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					if (isTemplateFile || string.IsNullOrEmpty(saveLocation))
					{
						SaveTemplate();
					}
					else
					{
						Save(saveLocation);
					}
				}
			}
		}

		private void OpenPck_DragEnter(object sender, DragEventArgs e)
		{
			pckOpen.Image = Resources.pckDrop;
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
			{
				var ext = Path.GetExtension(file);
				if (ext.Equals(".pck", StringComparison.CurrentCultureIgnoreCase))
				e.Effect = DragDropEffects.Copy;
				return;
			}
		}

		private void OpenPck_DragDrop(object sender, DragEventArgs e)
		{
			string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			foreach (string pck in FileList)
			{
				currentPCK = openPck(pck);
			}
			loadEditor();
		}

		private void OpenPck_DragLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void savePCK(object sender, EventArgs e)
		{
			checkSaveState();
		}

		private void saveAsPCK(object sender, EventArgs e)
		{
			SaveTemplate();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (PCKFilePath != PCKFileBCKUP)
			{
				if (string.IsNullOrWhiteSpace(PCKFilePath))
					try
					{
						RPC.SetRPC("Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
					}
					catch
					{
						Console.WriteLine("ERROR WITH RPC");
					}
				else

					try
					{
						RPC.SetRPC("Developing " + PCKFilePath, "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
					}
					catch
					{
						Console.WriteLine("ERROR WITH RPC");
					}
				PCKFileBCKUP = PCKFilePath;
			}
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			RPC.CloseRPC();
		}

		private void FormMain_Deactivate(object sender, EventArgs e)
		{
			RPC.CloseRPC();
		}

		private void FormMain_Activated(object sender, EventArgs e)
		{
			try
			{
				RPC.SetRPC("Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
				timer1.Start();
				timer1.Enabled = true;
			}
			catch { }
		}

		private void forMattNLContributorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://ko-fi.com/mattnl");
		}
	}
}