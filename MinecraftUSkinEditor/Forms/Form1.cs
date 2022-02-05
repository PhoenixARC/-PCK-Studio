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

namespace PckStudio
{
	public partial class FormMain : MetroFramework.Forms.MetroForm
	{
		#region Variables
		string saveLocation;//Save location for pck file
		int fileCount = 0;//variable for number of minefiles
		string Version = "6.3";//template for program version
		string hosturl = "";
		string basurl = "";
		string PCKFile = "";
		string PCKFileBCKUP = "x";
		loadedTexture tex = new loadedTexture(); //3DS feature variable


		PCK.MineFile mf;//Template minefile variable
		PCK currentPCK;//currently opened pck
		LOC l;//Locdata
		PCK.MineFile mfLoc;//LOC minefile
		Dictionary<int, string> types;//Template list for metadata of a individual minefiles metadata
		PCK.MineFile file;//template for a selected minefile
		bool needsUpdate = false;
		bool saved = true;
		string appData = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/PCK Studio/";
		public static bool correct = false;
		bool isdebug = false;

		public class displayId
		{
			public string id;
			public string defaultName;
		}
		#endregion

		#region form startup page
		public FormMain()
		{


			Directory.CreateDirectory(Environment.CurrentDirectory + "\\template");
			if (!File.Exists(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck"))
				File.WriteAllBytes(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck", Resources.UntitledSkinPCK);
			if (!File.Exists(Environment.CurrentDirectory + "\\settings.ini"))
				File.WriteAllText(Environment.CurrentDirectory + "\\settings.ini", Resources.settings);
			hosturl = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];


			InitializeComponent();

			if (Program.IsDev)
				isdebug = true;

			FormBorderStyle = FormBorderStyle.None;
			labelVersion.Text += Version;
			pckOpen.AllowDrop = true;
		}
		#endregion

		#region opens and loads pck file





		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.CheckFileExists = true; //makes sure opened pck exists
					ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						PCKFile = Path.GetFileName(ofd.FileName);
							openPck(ofd.FileName);
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("The PCK you're trying to use currently isn't supported\n" + err.StackTrace + "\n\n" + err.Message);//Error handling for PCKs that give errors when trying to be opened
			}
		}

		private void openPck(string filePath)
		{
			new TabPage();
			treeViewMain.Nodes.Clear();
			PCK pCK = (currentPCK = new PCK(filePath));
			foreach (PCK.MineFile mineFile in pCK.mineFiles)
			{
				Console.WriteLine(mineFile.name);
				if (!(mineFile.name == "0"))
				{
					continue;
				}
				foreach (object[] entry in mineFile.entries)
				{
					if (entry[0].ToString() == "LOCK")
					{
						if((new pckLocked(entry[1].ToString(), correct).ShowDialog() != DialogResult.OK || !correct))
						{
							return;
						}
					}
				}
			}
			addPasswordToolStripMenuItem.Enabled = true;
			openedPCKS.SelectedTab.Text = Path.GetFileName(filePath);
			saveLocation = filePath;
			_ = treeViewMain;
			_ = pictureBoxImagePreview;
			_ = treeMeta;
			_ = textBox1;
			_ = label1;
			_ = label2;
			_ = tabDataDisplay;
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
			foreach (PCK.MineFile mineFile2 in pCK.mineFiles)
			{
				TreeNode treeNode = new TreeNode();
				treeNode.Text = mineFile2.name;
				treeNode.Tag = mineFile2;
				string text = "";
				int num = 0;
				new List<string>();
				TreeNodeCollection nodes = treeViewMain.Nodes;
				do
				{
					text = "";
					string name = mineFile2.name;
					for (int i = 0; i < name.Length; i++)
					{
						char c = name[i];
						bool flag = false;
						if (c == '/')
						{
							foreach (TreeNode item in nodes)
							{
								if (item.Text == text)
								{
									nodes = nodes[item.Index].Nodes;
									flag = true;
								}
							}
							if (!flag)
							{
								nodes.Add(text);
								nodes = nodes[nodes.Count - 1].Nodes;
							}
							flag = false;
							text = "";
						}
						else
						{
							text += c;
						}
						num++;
					}
				}
				while (num != mineFile2.name.Length);
				if (Path.GetExtension(text) == ".binka")
				{
					treeNode.ImageIndex = 1;
					treeNode.SelectedImageIndex = 1;
				}
				else if (Path.GetExtension(text) == ".png")
				{
					treeNode.ImageIndex = 2;
					treeNode.SelectedImageIndex = 2;
				}
				else if (Path.GetExtension(text) == ".loc")
				{
					treeNode.ImageIndex = 3;
					treeNode.SelectedImageIndex = 3;
				}
				else if (Path.GetExtension(text) == ".pck")
				{
					treeNode.ImageIndex = 4;
					treeNode.SelectedImageIndex = 4;
				}
				else
				{
					treeNode.ImageIndex = 5;
					treeNode.SelectedImageIndex = 5;
				}
				treeNode.Text = text;
				nodes.Add(treeNode);
				saved = false;
			}
			myTablePanelStartScreen.Visible = false;
			pckOpen.Visible = false;
			label5.Visible = false;
			labelAmount.Visible = true;
			richTextBoxChangelog.Visible = false;
			openedPCKS.Visible = true;
			openedPCKS.AllowDrop = true;
			foreach (ToolStripMenuItem dropDownItem in fileToolStripMenuItem.DropDownItems)
			{
				dropDownItem.Enabled = true;
			}
			foreach (ToolStripMenuItem dropDownItem2 in editToolStripMenuItem.DropDownItems)
			{
				dropDownItem2.Enabled = true;
			}
			foreach (TreeNode node in treeViewMain.Nodes)
			{
				if (node.Text == "languages.loc")
				{
					mfLoc = (PCK.MineFile)treeViewMain.Nodes[node.Index].Tag;
				}
				if (node.Text == "localisation.loc")
				{
					mfLoc = (PCK.MineFile)treeViewMain.Nodes[node.Index].Tag;
				}
			}
			fileCount = 0;
			foreach (PCK.MineFile mineFile3 in currentPCK.mineFiles)
			{
				_ = mineFile3;
				fileCount++;
			}
			labelAmount.Text = "Files:" + fileCount;
			saved = false;
			LittleEndianCheckBox.Visible = true;
			LittleEndianCheckBox.Checked = currentPCK.IsLittleEndian;
		}
		#endregion

		#region deciphers what happens when certain pck entries are selected
		private void selectNode(object sender, TreeViewEventArgs e)
		{
			treeMeta.Enabled = true;
			int pictureBoxMaxHeight = (tabPage1.Height / 2) - (tabPage1.Height / 10);
			if (treeViewMain.SelectedNode.Tag != null) //"Selects" node if it has data/isn't a folder
			{
				fileCount = 0;//Resets file count
				//Gets file count based of all existing minefiles
				foreach (PCK.MineFile file in currentPCK.mineFiles)
				{
					fileCount += 1;
				}
				labelAmount.Text = "Files:" + fileCount;//Displays amount
				Dictionary<int, string> pckTypes = currentPCK.types; //Retrieves metadatabase

				PCK.MineFile mf = (PCK.MineFile)e.Node.Tag; //Sets current minefile being read

				types = currentPCK.types; //metadatabase
				file = mf; //minefile

				treeMeta.Nodes.Clear(); //clears minefile metadata treeview

				comboBox1.Items.Clear(); //clears metacombo(entry name)
				textBox1.Text = ""; //clears metatextbox(entry value)

				foreach (int type in types.Keys)
					comboBox1.Items.Add(types[type]); //Adds available metadata names from metadatabase to the metacombo

				//Retrieves metadata for currently selected mineifile and displays it within metatreeview
				int boxes = 0;
				foreach (object[] entry in file.entries) //object = metadata entry(name:value)
				{
					object[] strings = (object[])entry;
					TreeNode meta = new TreeNode();

					foreach (object[] entryy in file.entries)
						meta.Text = (string)strings[0];
					meta.Tag = entry;
					treeMeta.Nodes.Add(meta);

					//Check for if file contains model data
					if (entry[0].ToString()=="BOX")
					{
						boxes += 1;
						buttonEdit.Text = "EDIT BOXES";
						buttonEdit.Visible = true;
					}
					else if (entry[0].ToString() == "ANIM")
					{
						Console.WriteLine(entry[1]);
						Console.WriteLine((entry[1].ToString() == "0x80000").ToString() + " - " + entry[1]);
						Console.WriteLine((entry[1].ToString() == "0x40000").ToString() + " - "+ entry[1]);


						if ((entry[1].ToString() == "0x40000") || (entry[1].ToString() == "0x80000"))
						{
							buttonEdit.Text = "View Skin";
							boxes += 1;
							buttonEdit.Visible = true;
						}
					}
					else if(boxes == 0)
					{
						buttonEdit.Visible = false;
					}
				}

				//If selected item is a image, its displayed with proper dimensions in image box
				if (Path.GetExtension(mf.name) == ".png")
				{
					pictureBoxImagePreview.SizeMode = PictureBoxSizeMode.StretchImage;
					pictureBoxImagePreview.InterpolationMode = InterpolationMode.NearestNeighbor;
					MemoryStream png = new MemoryStream(mf.data); //Gets image data from minefile data
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
						Size maxDisplay = new Size(tabPage1.Size.Width / 2 - 5, tabPage1.Size.Height / 2 - 5);
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
				else if (Path.GetExtension(mf.name) == ".loc")
				{
					buttonEdit.Text = "EDIT LOC";
					buttonEdit.Visible = true;
				}
				else if (Path.GetFileName(mf.name) == "audio.pck")
				{
					buttonEdit.Text = "EDIT AUDIO";
					buttonEdit.Visible = true;
				}
				else
				{
					buttonEdit.Visible = false;
					//Sets preview image to "NO IMAGE" if selected file data isn't image data
					pictureBoxImagePreview.Image = (Image)Resources.NoImageFound;
					pictureBoxImagePreview.Size = new Size(pictureBoxMaxHeight, pictureBoxMaxHeight);
					labelImageSize.Text = "";
				}
			}
			else
			{
				//Sets preview image to "NO IMAGE" if selected file data isn't image data
				pictureBoxImagePreview.Image = (Image)Resources.NoImageFound;
				pictureBoxImagePreview.Size = new Size(pictureBoxMaxHeight, pictureBoxMaxHeight);
			}
			labelImageSize.Text = "";//Resets image size display if theres no image
		}
		#endregion

		#region Parses boxes and opens model generator
		public void editModel(PCK.MineFile skin)
		{
			List<object[]> otherData = new List<object[]>();//Creates list for backup data to be added to
			List<object[]> generatedData = new List<object[]>();//Creates list for model data to be added to
			foreach (object[] entry in skin.entries) //object = metadata entry(name:value)
			{
				//parses and sorts
				if (entry[0].ToString() == "BOX")
				{
					generatedData.Add(entry);
				}
				else if (entry[0].ToString() == "OFFSET")
				{
					generatedData.Add(entry);
				}
				else if (entry[0].ToString() != "BOX" && entry[0].ToString() != "OFFSET")
				{
					otherData.Add(entry);
				}
			}
			skin.entries = otherData;
			generateModel generate = new generateModel(generatedData, new PictureBox());
			generate.ShowDialog();//Opens Model Generator Dialog
								  //Adds model data
			foreach (object[] entry in generatedData) //object = metadata entry(name:value)
			{
				skin.entries.Add(entry);
			}

			treeMeta.Nodes.Clear(); //clears minefile metadata treeview

			comboBox1.Items.Clear(); //clears metacombo(entry name)
			textBox1.Text = ""; //clears metatextbox(entry value)

			foreach (int type in types.Keys)
				comboBox1.Items.Add(types[type]); //Adds available metadata names from metadatabase to the metacombo

			//Retrieves metadata for currently selected mineifile and displays it within metatreeview
			foreach (object[] entry in file.entries) //object = metadata entry(name:value)
			{
				object[] strings = (object[])entry;
				TreeNode meta = new TreeNode();

				foreach (object[] entryy in file.entries)
					meta.Text = (string)strings[0];
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
			if (treeViewMain.SelectedNode.Tag is PCK.MineFile)//Makes sure item being extracted is minefile and not folder or null item
			{
				SaveFileDialog exFile = new SaveFileDialog();//extract location
				exFile.FileName = treeViewMain.SelectedNode.Text;
				exFile.Filter = Path.GetExtension(treeViewMain.SelectedNode.Text).Replace(".", "") + " File|*" + Path.GetExtension(treeViewMain.SelectedNode.Text);
				exFile.ShowDialog();

				string appPath = exFile.FileName;//Chosen file path
				string extractPath = exFile.FileName;

				if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName(extractPath)))//Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
				{
					File.WriteAllBytes(extractPath, ((PCK.MineFile)treeViewMain.SelectedNode.Tag).data);//extracts minefile data to directory

					//Generates metadata file in form of txt file if metadata for the file exists
					if (treeViewMain.SelectedNode.Tag.ToString() != "")
					{
						try
						{
							string metaData = "";
							types = currentPCK.types;
							file = (PCK.MineFile)treeViewMain.SelectedNode.Tag;

							var ms = new MemoryStream(File.ReadAllBytes(extractPath).ToArray());

							MemoryStream ico = new MemoryStream();
							Bitmap bmp = new Bitmap(Image.FromFile(extractPath));
							bmp.Save(ico, System.Drawing.Imaging.ImageFormat.Png);

							foreach (object[] entry in file.entries)
							{
								object[] strings = (object[])entry;
								metaData += (string)strings[0] + ":" + (string)strings[1] + Environment.NewLine;
							}

							File.WriteAllText(extractPath + ".txt", metaData);
						}
						catch (Exception)
						{

						}
						MessageBox.Show("File Extracted");//Verification that file extraction path was successful
					}
				}
			}
			else if (treeViewMain.SelectedNode != null)
			{
				SaveFileDialog exFile = new SaveFileDialog();//extract location
				exFile.ShowDialog();
				string appPath = exFile.FileName;//Chosen file path

				foreach (TreeNode item in treeViewMain.SelectedNode.Nodes)
				{
					if (item.Tag is PCK.MineFile)//Makes sure item being extracted is minefile and not folder or null item
					{
						string extractPath = Path.Combine(appPath, ((PCK.MineFile)item.Tag).name);//combines file path with file path & name of minefile being extracted

						if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName(extractPath)))//Makes sure chosen directory isn't null or whitespace AKA makes sure its usable
						{
							Directory.CreateDirectory(Path.GetDirectoryName(extractPath));//Creates directory variable out of generated/chosen extract path
							File.WriteAllBytes(extractPath, ((PCK.MineFile)item.Tag).data);//extracts minefile data to directory

							//Generates metadata file in form of txt file if metadata for the file exists
							if (item.Tag.ToString() != "")
							{
								try
								{
									string metaData = "";
									types = currentPCK.types;
									file = mf;

									var ms = new MemoryStream(File.ReadAllBytes(extractPath).ToArray());

									MemoryStream ico = new MemoryStream();
									Bitmap bmp = new Bitmap(Image.FromFile(extractPath));
									bmp.Save(ico, System.Drawing.Imaging.ImageFormat.Png);

									foreach (object[] entry in file.entries)
									{
										object[] strings = (object[])entry;
										metaData += (string)strings[0] + ":" + (string)strings[1] + Environment.NewLine;
									}

									File.WriteAllText(extractPath + ".txt", metaData);
								}
								catch (Exception)
								{

								}
								MessageBox.Show("Path Extracted");//Verification that file extraction path was successful
							}
						}
					}
				}
			}
		}
		#endregion

		#region saves pck
		private void save(string saveType)
		{
			TreeView saveStructure = new TreeView();//Temporary new treeview to properly store minefiles in writable form
			//structures minefile data based on wether it has parent nodes or not and with its proper minefile data
			foreach (TreeNode item in treeViewMain.Nodes)
			{
				TreeNode add = new TreeNode();
				if (item.Parent != null)
				{
					string itemPath = "";//item path template
					List<TreeNode> path = new List<TreeNode>();//directory template
					GetPathToRoot(treeViewMain.SelectedNode, path);//gets all parents nodes
					//generates minefile directory to properly store in minedata
					foreach (TreeNode dire in path)
					{
						itemPath += dire.Text + "/";
					}
					add.Text = itemPath + item.Text;
				}
				else
				{
					add.Text = item.Text;
				}
				add.Tag = item.Tag;
				saveStructure.Nodes.Add(add);
				add.Remove();
			}

			//Reassignes each node with its minefile data to make sure everything is synced
			foreach (TreeNode item in saveStructure.Nodes)
			{
				currentPCK.mineFiles[item.Index] = (PCK.MineFile)item.Tag;
			}

			//Syncs minefile name with nodes name
			for (int i = 0; i < saveStructure.Nodes.Count; i++)
				currentPCK.mineFiles[i].name = saveStructure.Nodes[i].Text;

			if (saveLocation == Application.StartupPath + @"\templates\UntitledSkinPCK.pck")
			{
				//writes pck data if pck is actually opened
				using (var ofd = new SaveFileDialog())
				{
					ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						try
						{
							Console.WriteLine(currentPCK.IsLittleEndian.ToString() + "--");
							if (LittleEndianCheckBox.Checked)
							{
								byte[] oouput = currentPCK.RebuildVita();
								oouput[0] = 0x03;
								File.WriteAllBytes(ofd.FileName, currentPCK.RebuildVita());
							}
							else
							{
								byte[] oouput = currentPCK.Rebuild();
								File.WriteAllBytes(ofd.FileName, currentPCK.Rebuild());
							}
							saveLocation = ofd.FileName;
							openedPCKS.SelectedTab.Text = Path.GetFileName(ofd.FileName);
							saved = true;
							MessageBox.Show("PCK Saved!");
							PCKFile = Path.GetFileName(ofd.FileName);
						}
						catch (Exception)
						{
							MessageBox.Show("No PCK loaded");
						}
					}
				}
			}
			else if (saveType == "Save As")
			{
				//writes pck data if pck is actually opened
				using (var ofd = new SaveFileDialog())
				{
					ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						try
						{
							Console.WriteLine(currentPCK.IsLittleEndian.ToString() + "--");
							if (LittleEndianCheckBox.Checked)
							{
								byte[] oouput = currentPCK.RebuildVita();
								oouput[0] = 0x03;
								File.WriteAllBytes(ofd.FileName, currentPCK.RebuildVita());
							}
							else
							{
								byte[] oouput = currentPCK.Rebuild();
								File.WriteAllBytes(ofd.FileName, currentPCK.Rebuild());
							}
							saveLocation = ofd.FileName;
							openedPCKS.SelectedTab.Text = Path.GetFileName(ofd.FileName);
							saved = true;
							MessageBox.Show("PCK Saved!");
						}
						catch (Exception)
						{
							MessageBox.Show("No PCK loaded");
						}
					}
				}
			}
			else
			{
				if (MessageBox.Show("Are you sure you wanna save?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					try
					{
						Console.WriteLine(currentPCK.IsLittleEndian.ToString() + "--");
						if (LittleEndianCheckBox.Checked)
						{
							byte[] oouput = currentPCK.RebuildVita();
							oouput[0] = 0x03;
							File.WriteAllBytes(saveLocation, currentPCK.RebuildVita());
						}
						else
						{
							byte[] oouput = currentPCK.Rebuild();
							File.WriteAllBytes(saveLocation, currentPCK.Rebuild());
						}
					}
					catch (Exception)
					{
						for (int i = 0; i < saveStructure.Nodes.Count; i++)
							currentPCK.mineFiles[i].name = saveStructure.Nodes[i].Text;

						using (var ofd = new SaveFileDialog())
						{
							ofd.Filter = "PCK (Minecraft Console Package)|*.pck";

							if (ofd.ShowDialog() == DialogResult.OK)
							{
								try
								{
									File.WriteAllBytes(ofd.FileName, currentPCK.Rebuild());
									saved = true;
									MessageBox.Show("PCK Saved!");
								}
								catch (Exception)
								{
									MessageBox.Show("No PCK loaded");
								}
							}
						}
					}
				}
			}
			saveStructure.Dispose();//disposes temporarily made treeview
		}
		#endregion

		#region replaces pck entry with selected file
		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag is PCK.MineFile)//Makes sure file being replaced is an actual minefile or not null
			{
				PCK.MineFile mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;//backups minefile data for node
				using (var ofd = new OpenFileDialog())
				{
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						mf.data = File.ReadAllBytes(ofd.FileName);//overwrites minefile data with chosen files data
						mf.filesize = mf.data.Length;//updates file size
					}
				}
			}
			saved = false;
		}
		#endregion

		#region ignore
		private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//Does not work as intended. Renaming moved to save function
			saved = false;
		}
		#endregion

		#region deletes pck entry from treeview and pck.minefiles
		private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Removes selected from current pcks minefiles list and nodes
			if (treeViewMain.SelectedNode.Tag is PCK.MineFile)
			{
				PCK.MineFile mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
				treeViewMain.Nodes.Remove(treeViewMain.SelectedNode);
				currentPCK.mineFiles.Remove(mf);
			}
			else
			{
				if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					foreach (TreeNode item in treeViewMain.SelectedNode.Nodes)
					{
						if (item.Tag == null)
						{
							MessageBox.Show("Can't fully delete directory with subdirectories");
							return;
						}
						if (item.Tag is PCK.MineFile)//makes sure selected node is a minefile
						{
							//removes minefile from minefile list
							PCK.MineFile mf = (PCK.MineFile)item.Tag;
							currentPCK.mineFiles.Remove(mf);
							//removes minefile node
							item.Remove();
						}
					}
					treeViewMain.SelectedNode.Remove();
				}
			}
			saved = false;
		}
		#endregion

		#region renames pck entry from treeview and pck.minefiles
		private void renameFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PCK.MineFile mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
			PckStudio.rename diag = new PckStudio.rename(mf);
			diag.ShowDialog(this);
			diag.Dispose();//diposes generated metadata adding dialog data
			treeViewMain.SelectedNode.Text = Path.GetFileName(mf.name);
		}
		#endregion

		#region clones pck entry from treeview and pck.minefiles
		private void cloneFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag == null) return;

			PCK.MineFile mfO = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
			FileInfo mfCO = new FileInfo(mfO.name);

			PCK.MineFile mf = new PCK.MineFile();//Creates new minefile template
			mf.data = mfO.data;//adds file data to minefile
			mf.filesize = mfO.data.Length;//gets filesize for minefile
			mf.name = Path.GetDirectoryName(mfO.name).Replace("\\", "/") + "/" + Path.GetFileNameWithoutExtension(mfO.name) + "_clone" + mfCO.Extension;//sets minfile name to file name
			if (treeViewMain.SelectedNode.Parent == null && mf.name.StartsWith("/")) mf.name = mf.name.Remove(0, 1);
			mf.entries = mfO.entries;
			mf.type = 0;//sets minefile type to default
			TreeNode add = new TreeNode(Path.GetFileName(mf.name)) { Tag = mf };//creates node for minefile

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

			currentPCK.mineFiles.Insert(currentPCK.mineFiles.IndexOf(mfO) + 1, mf); //inserts minefile into proper list index
			if (treeViewMain.SelectedNode.Parent == null) treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.Index + 1, add); //adds generated minefile node
			else treeViewMain.SelectedNode.Parent.Nodes.Insert(treeViewMain.SelectedNode.Index + 1, add);//adds generated minefile node to selected folder
		}
		#endregion

		#region adds file to treeview and pck.minefiles
		private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					PCK.MineFile mf = new PCK.MineFile();//Creates new minefile template
					mf.data = File.ReadAllBytes(ofd.FileName);//adds file data to minefile
					mf.filesize = mf.data.Length;//gets filesize for minefile
					mf.name = Path.GetFileName(ofd.FileName);//sets minfile name to file name
					mf.type = 0;//sets minefile type to default
					TreeNode add = new TreeNode(mf.name) { Tag = mf };//creates node for minefile

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
						currentPCK.mineFiles.Insert(treeViewMain.SelectedNode.Nodes.Count - 1, mf);//inserts minefile into proper list index

						string itemPath = "";//item path template
						List<TreeNode> path = new List<TreeNode>();//directory template
						GetPathToRoot(treeViewMain.SelectedNode, path);//gets all parents nodes
						//generates minefile directory to properly store in minedata
						foreach (TreeNode dire in path)
						{
							itemPath += dire.Text + "/";
						}

						currentPCK.mineFiles[treeViewMain.SelectedNode.Nodes.Count - 1].name = itemPath + treeViewMain.SelectedNode.Nodes[treeViewMain.SelectedNode.Nodes.Count - 1].Text;//updates minefile name with directory
					}
					else//adds minefile to root of the pck
					{
						currentPCK.mineFiles.Add(mf);
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
			int tempIDD;//sets variables for a temporary skin/cape id

			try
			{
				string tempID = treeViewMain.Nodes[i].Text.Remove(treeViewMain.Nodes[i].Text.Length - 4, 4);//gets id of last skin/cape in treeview if the last item is a skin or cape

				tempID = tempID.Remove(0, 8);//removes text from id

				tempIDD = int.Parse(tempID) + 1;//adds to skin/cape id index to presets the next skin/cape id
			}
			catch (Exception)
			{
				tempIDD = 00000000;//sets temporary id to 0 if an id can't be generated off the treeviews last item
			}
			PCK.MineFile mf = mfLoc;//Sets loc minefile

			try
			{
				l = new LOC(mf.data);//sets loc data
			}
			catch
			{
				//error handling for if pck doesn't have a loc file
				MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			PckStudio.addnewskin add = new PckStudio.addnewskin(currentPCK, treeViewMain, tempIDD.ToString(), l);//Sets dialog data for skin creator
			add.ShowDialog();//opens skin creator
			mf.data = l.Rebuild();//rebuilds loc data
			add.Dispose();//disposes generated skin creator data
			saved = false;
		}
		#endregion

		#region starts up form to create and add a animated texture
		private void createAnimatedTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Filter = "PNG Files | *.png";
				ofd.Title = "Select a PNG File";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					PckStudio.addAnimatedTexture add = new PckStudio.addAnimatedTexture(currentPCK, treeViewMain, ofd.FileName, Path.GetFileName(ofd.FileName).Remove(Path.GetFileName(ofd.FileName).Length - 4, 4));//presets texture generator dialog with needed data including selected picture
					add.ShowDialog();//Shows dialog
					add.Dispose();//Diposes generated dialog data
				}
			}
			saved = false;
		}
		#endregion

		#region deciphers what happens when certain pck entries are double clicked
		private void treeView1_DoubleClick(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode.Tag != null)
			{
				mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;

				//Checks to see if selected minefile is a loc file
				if (Path.GetExtension(mf.name) == ".loc")
				{
					if (treeViewMain.SelectedNode.Tag is PCK.MineFile)
					{
						LOC l;
						try
						{
							l = new LOC(mf.data);
						}
						catch
						{
							MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK,
								MessageBoxIcon.Error);
							return;
						}
						(new LOCEditor(l)).ShowDialog();//Opens LOC Editor
						mf.data = l.Rebuild();//Rebuilds loc file with locdata in grid view after closing dialog
					}
				}

				//Checks to see if selected minefile is an audio file
				if (Path.GetFileName(mf.name) == "audio.pck")
				{
					if (treeViewMain.SelectedNode.Tag is PCK.MineFile)
					{
						try
						{
							PckStudio.Forms.Utilities.AudioEditor diag = new PckStudio.Forms.Utilities.AudioEditor(mf.data, mf);
							diag.ShowDialog(this);
						}
						catch(Exception ex)
						{
							MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK,
							MessageBoxIcon.Error);
							return;
						}
					}
				}

				//Checks to see if selected minefile is a col file
				if (Path.GetExtension(mf.name) == ".col")
				{
					//MessageBox.Show(".COL Editor Coming Soon!");

					if (treeViewMain.SelectedNode.Tag is PCK.MineFile)
					{
						try
						{
							PckStudio.Forms.Utilities.COLEditor diag = new PckStudio.Forms.Utilities.COLEditor(mf.data, mf);
							diag.Show();
						}
						catch
						{
							MessageBox.Show("No Color data found.", "Error", MessageBoxButtons.OK,
								MessageBoxIcon.Error);
							return;
						}
						//mf.data = l.Rebuild();//Rebuilds loc file with locdata in grid view after closing dialog
					}
				}

				//Checks to see if selected minefile is a binka file
				System.Threading.ThreadStart starter;

				System.Threading.Thread binkam;
				if (Path.GetExtension(mf.name) == ".binka")
				{
					MessageBox.Show(".binka Editor Coming Soon!");
				}

			}
		}
		#endregion

		#region updates combo and text boxes for metadata when a metadata entry is selected
		private void treeMeta_AfterSelect(object sender, TreeViewEventArgs e)
		{
			comboBox1.Items.Clear();//Resets metadata combobox of selectable entry names
			object[] strings = (object[])e.Node.Tag;
			foreach (int type in types.Keys)
				comboBox1.Items.Add(types[type]);//fills combobox with metadata from the main metadatabase
			comboBox1.Text = (string)strings[0];//Sets currently selected metadata type to type selected in selected metadata node
			textBox1.Text = (string)strings[1];//Sets currently selected metadata value to value selected in selected metadata node
		}
		#endregion

		#region updates metadata when combo option is selected
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode != null)
			{
				//Sets metadata type to new chosen one
				object[] strings = (object[])treeMeta.SelectedNode.Tag;
				strings[0] = comboBox1.Text;
			}
			saved = false;
		}
		#endregion

		#region updates metadata value when text box value changes
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode != null)
			{
				//sets metadata value to new value
				object[] strings = (object[])treeMeta.SelectedNode.Tag;
				strings[1] = textBox1.Text;
			}
			saved = false;
		}
		#endregion

		#region deletes metadata entry
		private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeMeta.SelectedNode != null)//Makes sure selected node is a minefile
			{
				object[] temp = (object[])treeMeta.SelectedNode.Tag;
				file.entries.Remove(temp);//removes minefile from minefile list
				treeMeta.Nodes.Remove(treeMeta.SelectedNode);//removes minefile node

//                treeMeta.Nodes.Clear();//Resets metadata treeview
			}
			saved = false;
		}
		#endregion

		#region adds metadata entry
		private void addEntryToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;//Sets minefile to selected node
			PckStudio.addMeta add = new PckStudio.addMeta(mf, currentPCK);//sets metadata adding dialog
			add.ShowDialog();//displays metadata adding dialog
			add.Dispose();//diposes generated metadata adding dialog data

			//Sets up combobox for metadata entries from main metadatabase
			treeMeta.Nodes.Clear();
			foreach (int type in types.Keys)
				comboBox1.Items.Add(types[type]);

			//loads all of selected minefiles metadata into metadata treeview
			foreach (object[] entry in file.entries)
			{
				object[] strings = (object[])entry; TreeNode meta = new TreeNode();

				foreach (object[] entryy in file.entries)
					meta.Text = (string)strings[0];
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}
		#endregion

		#region moves node up and arranges minefile indexes
		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode != null)//makes sure selected node is a minefile
			{
				if (treeViewMain.SelectedNode.Tag != null)
				{
					if (treeViewMain.SelectedNode.Index - 1 >= 0)//Makes sure selected node isn't already at the top
					{
						//rearranges nodes minefile data indexes in minefiles list
						currentPCK.mineFiles[treeViewMain.SelectedNode.Index - 1] = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
						currentPCK.mineFiles[treeViewMain.SelectedNode.Index] = (PCK.MineFile)treeViewMain.Nodes[treeViewMain.SelectedNode.Index - 1].Tag;
						//switches selected node with node above it
						TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();
						treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.Index - 1, move);
						//removes node because a clone was inserted into its new index
						treeViewMain.SelectedNode.Remove();
					}
				}
			}
			saved = false;
		}
		#endregion

		#region moves node down and arranges minefile indexes
		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewMain.SelectedNode != null)//makes sure selected node is a minefile
			{
				if (treeViewMain.SelectedNode.Tag != null)
				{
					if (treeViewMain.Nodes[treeViewMain.SelectedNode.Index + 1] != null)//Makes sure selected node isn't already at the bottom
					{
						//rearranges nodes minefile data indexes in minefiles list
						currentPCK.mineFiles[treeViewMain.SelectedNode.Index + 1] = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
						currentPCK.mineFiles[treeViewMain.SelectedNode.Index] = (PCK.MineFile)treeViewMain.Nodes[treeViewMain.SelectedNode.Index + 1].Tag;
						//switches selected node with node below it
						TreeNode move = (TreeNode)treeViewMain.SelectedNode.Clone();
						treeViewMain.Nodes.Insert(treeViewMain.SelectedNode.Index + 2, move);
						//removes node because a clone was inserted into its new index
						treeViewMain.SelectedNode.Remove();
					}
				}
			}
			saved = false;
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
			mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;//Sets selected minefile from node
			PckStudio.presetMeta add = new PckStudio.presetMeta(mf, currentPCK);//sets data for preset adding dialog
			add.ShowDialog();//displays preset adding dialog
			add.Dispose();//disposes generated preset adding data

			//reloads treemeta data
			treeMeta.Nodes.Clear();
			foreach (int type in types.Keys)
				comboBox1.Items.Add(types[type]);

			foreach (object[] entry in file.entries)
			{
				object[] strings = (object[])entry;
				TreeNode meta = new TreeNode();

				foreach (object[] entryy in file.entries)
					meta.Text = (string)strings[0];
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}
		#endregion

		#region loads empty pck template
		private void skinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Loads skin pack template
			PCKFile = Path.GetFileName(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck");
			openPck(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck");
			saveLocation = "";
			saved = false;
		}
		#endregion

		#region open advanced metadata bulk editing window
		private void advancedMetaAddingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openedPCKS.Visible == true)
			{
				//opens dialog for bulk minefile editing
				PckStudio.AdvancedOptions advanced = new PckStudio.AdvancedOptions(currentPCK);
				advanced.ShowDialog();
				advanced.Dispose();
				saved = false;
			}
			else if (openedPCKS.Visible == false)
			{
				MessageBox.Show("Open PCK file first!");
			}
		}
		#endregion

		#region closes tool
		private void buttonShutdown_Click(object sender, EventArgs e)
		{
			this.Close();//closes PCK Studio
		}
		#endregion

		#region open program info/credits window
		private void programInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//open program info dialog
			PckStudio.programInfo info = new PckStudio.programInfo();
			info.ShowDialog();
			info.Dispose();
		}
		#endregion

		#region checks for updates
		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				RPC.SetRPC("825875166574673940", "Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
				timer1.Start();
				timer1.Enabled = true;
			}
			catch
			{
				Console.WriteLine("ERROR WITH RPC");
			}
			try
			{
				label1.Theme = this.Theme;
				labelVersion.Theme = this.Theme;
				label2.Theme = this.Theme;
				label3.Theme = this.Theme;
				labelImageSize.Theme = this.Theme;
				labelAmount.Theme = this.Theme;
				labelEntryType.Theme = this.Theme;
				labelEntryData.Theme = this.Theme;
				DBGLabel.Theme = this.Theme;
				label4.Theme = this.Theme;
				label6.Theme = this.Theme;
				label7.Theme = this.Theme;
				label8.Theme = this.Theme;
				label9.Theme = this.Theme;
				label10.Theme = this.Theme;
				label11.Theme = this.Theme;
				ChangeURL.Theme = this.Theme;
				label5.Theme = this.Theme;
				openedPCKS.Theme = this.Theme;
				tabPage1.Theme = this.Theme;
				metroTabControl1.Theme = this.Theme;
				metroTabPage1.Theme = this.Theme;
				LittleEndianCheckBox.Theme = this.Theme;

				new WebClient().DownloadString(Program.baseurl + ChangeURL.Text);
				basurl = Program.baseurl;
				Console.WriteLine(basurl + ChangeURL.Text);
			}
			catch
			{
				try
				{
					new WebClient().DownloadString(Program.backurl + ChangeURL.Text);
					basurl = Program.backurl;
					Console.WriteLine(basurl + ChangeURL.Text);
				}
				catch
				{
					try
					{
						new WebClient().DownloadString("https://google.com");
						MessageBox.Show("PCK Studio Service is offline, the domain may have changed.\nOpening website");
						Process.Start("https://phoenixarc.github.io/pckstudio.tk/");
					}
					catch
					{
						MessageBox.Show("Could not connect to service, internet may be offline");
					}
				}
			}


			Directory.CreateDirectory(Environment.CurrentDirectory + "\\template");
			if (!File.Exists(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck"))
				File.WriteAllBytes(Environment.CurrentDirectory + "\\template\\UntitledSkinPCK.pck", Resources.UntitledSkinPCK);


			if (isdebug)
				DBGLabel.Visible = true;
			//runs creator spotlight once per day
			//if (!File.Exists(appData + "date.txt"))
			//{
			//    File.WriteAllText(appData + "date.txt", DateTime.Now.ToString("MM/dd/yyyy"));
			//    creatorSpotlight shoutout = new creatorSpotlight();
			//    shoutout.ShowDialog();
			//}
			//else if (DateTime.Now.ToString("MM/dd/yyyy") != File.ReadAllText(appData + "date.txt"))
			//{
			//    creatorSpotlight shoutout = new creatorSpotlight();
			//    File.WriteAllText(appData + "date.txt", DateTime.Now.ToString("MM/dd/yyyy"));
			//    shoutout.ShowDialog();
			//}


			//Promo shoutout = new Promo();
			//shoutout.ShowDialog();


			//Makes sure appdata exists
			if (!Directory.Exists(appData))
			{
				Directory.CreateDirectory(appData);
			}

			if (!Directory.Exists(Environment.CurrentDirectory + "\\cache\\mods\\"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\cache\\mods\\");
			}


			//Checks to see if program version file exists, and creates one if it doesn't
			//Latest changelog on program start-up
			try
			{
				using (WebClient client = new WebClient())
				{
					if(isdebug)
						File.WriteAllText(appData + "pckStudioChangelog.txt", File.ReadAllText("C:\\WEBSITES\\PCKStudio\\studio\\PCK\\api\\" + ChangeURL.Text));
					else
						File.WriteAllText(appData + "pckStudioChangelog.txt", client.DownloadString(basurl + ChangeURL.Text));
						richTextBoxChangelog.Text = File.ReadAllText(appData + "pckStudioChangelog.txt");
				}
			}
			catch
			{
				MessageBox.Show("Could not load changelog");
			}

			if (!File.Exists(Application.StartupPath + @"\ver.txt"))
			{
				File.WriteAllText(Application.StartupPath + @"\ver.txt", Version);
			}
			try
			{
				if (float.Parse(new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "")) > float.Parse(Version) && !System.Diagnostics.Debugger.IsAttached)
				{
					Console.WriteLine(new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "") + " != " + Version);
					if (MessageBox.Show("Update avaliable!\ndo you want to update?", "UPDATE", MessageBoxButtons.YesNo) == DialogResult.Yes)
						Process.Start(Environment.CurrentDirectory + "\\nobleUpdater.exe");
					else
						uPDATEToolStripMenuItem1.Visible = true;
				}
				else
				{
					uPDATEToolStripMenuItem1.Visible = false;
				}
			}
			catch
			{
				MessageBox.Show("Could not load Version Information");
			}
		}
		#endregion

		#region deletes pck entires through the del key
		private void treeViewMain_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)//checks to make sure pressed key was del
			{
				if (treeViewMain.SelectedNode.Tag is PCK.MineFile)//makes sure selected node is a minefile
				{
					//removes minefile from minefile list
					PCK.MineFile mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;
					currentPCK.mineFiles.Remove(mf);
					//removes minefile node
					treeViewMain.Nodes.Remove(treeViewMain.SelectedNode);
				}
				else
				{
					if (MessageBox.Show("Are you sure want to delete this folder? All contents will be deleted", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
					{
						foreach (TreeNode item in treeViewMain.SelectedNode.Nodes)
						{
							if (item.Tag == null)
							{
								MessageBox.Show("Can't fully delete directory with subdirectories");
								return;
							}
							if (item.Tag is PCK.MineFile)//makes sure selected node is a minefile
							{
								//removes minefile from minefile list
								PCK.MineFile mf = (PCK.MineFile)item.Tag;
								currentPCK.mineFiles.Remove(mf);
								//removes minefile node
								item.Remove();
							}
						}
						treeViewMain.SelectedNode.Remove();
					}
				}
			}
			saved = false;
		}
		#endregion

		#region extracts a selected pck without opening the pck
		private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			try
			{
				//Extracts a chosen pck file to a chosen destincation
				OpenFileDialog ofd = new OpenFileDialog();
				FolderBrowserDialog sfd = new FolderBrowserDialog();
				ofd.CheckFileExists = true;
				ofd.Filter = "PCK (Minecraft Wii U Package)|*.pck";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (sfd.ShowDialog() == DialogResult.OK)
					{
						foreach (PCK.MineFile mf in new PCK(ofd.FileName).mineFiles)
						{
							foreach (object[] entry in mf.entries)
							{
								if (entry[0].ToString() == "LOCK") // Check for lock on PCK File
								{
									if ((new pckLocked(entry[1].ToString(), correct).ShowDialog() != DialogResult.OK || !correct))
									{
										return; // cancel extraction if password not provided
									}
								}
							}
							System.IO.FileInfo file = new System.IO.FileInfo(sfd.SelectedPath + @"\" + mf.name);
							file.Directory.Create(); // If the directory already exists, this method does nothing.
							File.WriteAllBytes(sfd.SelectedPath + @"\" + mf.name, mf.data); //writes minefile to file
																							//attempts to generate reimportable metadata file out of minefiles metadata
							string metaData = "";

							foreach (object[] entry in mf.entries)
							{
								object[] strings = (object[])entry;
								metaData += (string)strings[0] + ":" + (string)strings[1] + Environment.NewLine;
							}

							File.WriteAllText(sfd.SelectedPath + @"\" + mf.name + ".txt", metaData);
						}
					}
				}
			} catch (Exception)
			{
				MessageBox.Show("Unsupported PCK");
			}
		}
		#endregion

		#region deletes metadata entries through the del key
		private void treeMeta_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)//makes sure pressed key was del
			{
				if (treeMeta.SelectedNode != null)//makes sure selected node is a minefile
				{
					//removes selected treemeta entry
					object[] temp = (object[])treeMeta.SelectedNode.Tag;
					file.entries.Remove(temp);
					treeMeta.Nodes.Remove(treeMeta.SelectedNode);

					//reloads treemeta data
					treeMeta.Nodes.Clear();
					foreach (int type in types.Keys)
						comboBox1.Items.Add(types[type]);

					foreach (object[] entry in file.entries)
					{
						object[] strings = (object[])entry;
						TreeNode meta = new TreeNode();

						foreach (object[] entryy in file.entries)
							meta.Text = (string)strings[0];
						meta.Tag = entry;
						treeMeta.Nodes.Add(meta);
					}
				}
			}
			saved = false;
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
					PCK.MineFile mfNew = new PCK.MineFile();//new minefile template
					ListViewItem Import = new ListViewItem();//listviewitem to store temporary data
					Import.Text = file.Name.Remove(file.Name.Length - 4, 4);//gets file name without extension
					mfNew.data = File.ReadAllBytes(contents.SelectedPath + @"\" + file.Name.Remove(file.Name.Length - 4, 4) + ".png");//sets minefile data to image data of current skin

					TreeNode skin = new TreeNode();//create template treenode for minefile

					currentPCK.mineFiles.Add(mfNew);//adds new minefile to minefile list for skin
					mfNew.filesize = mfNew.data.Length;//gets filesize of the skin image

					//Sets minefile directory based on pcks structure/type
					if (mashupStructure == true)
					{
						mfNew.name = "Skins/" + Import.Text + ".png";
					}
					else
					{
						mfNew.name = Import.Text + ".png";
					}

					//sets minefile type based on wether cape or skin
					if (Import.Text.Remove(7, Import.Text.Length - 7) == "dlccape")
					{
						mfNew.type = 1;
					}
					else if (Import.Text.Remove(7, Import.Text.Length - 7) == "DLCCAPE")
					{
						mfNew.type = 1;
					}
					else
					{
						mfNew.type = 0;
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
					int i = 0;

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
							object[] ENTRY = { entryName, entryValue };
							mfNew.entries.Add(ENTRY);

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
								LOC l;

								try
								{
									l = new LOC(mfLoc.data);
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								displayId dis = new displayId();
								dis.id = locNameId;
								dis.defaultName = locName;

								l.ids.names.Add(dis.id);

								foreach (LOC.Language lo in l.langs)
									lo.names.Add(dis.defaultName);
								mfLoc.data = l.Rebuild();
								locNameId = "";
								locName = "";
							}

							//creates metadata id in loc file
							if (locThemeId != "" && locTheme != "")
							{
								LOC l;

								try
								{
									l = new LOC(mfLoc.data);
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								displayId b = new displayId();
								b.id = locThemeId;
								b.defaultName = locTheme;

								l.ids.names.Add(b.id);

								foreach (LOC.Language lo in l.langs)
									lo.names.Add(b.defaultName);

								mfLoc.data = l.Rebuild();
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
					string skinNameImport = System.IO.Path.GetFileName(contents.FileName);//Gets skin name
					PCK.MineFile mfNew = new PCK.MineFile();//new minefile template
					ListViewItem Import = new ListViewItem();//listviewitem to store temporary data
					Import.Text = skinNameImport.Remove(skinNameImport.Length - 4, 4);//gets file name without extension
					mfNew.data = File.ReadAllBytes(contents.FileName.Remove(contents.FileName.Length - 4, 4));//sets minefile data to image data of current skin

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

					currentPCK.mineFiles.Add(mfNew);//Adds minefile to minefile list
					mfNew.filesize = mfNew.data.Length;//gets and sets minefile filesize
					if (mashupStructure == true)
					{
						mfNew.name = "Skins/" + Import.Text;
					}
					else
					{
						mfNew.name = Import.Text;
					}
					mfNew.type = 0;//sets file type to default

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
					int i = 0;

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
							//adds minefiles metadata and presets loc data for minefile
							object[] ENTRY = { entryName, entryValue };
							mfNew.entries.Add(ENTRY);

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
								LOC l;

								try
								{
									l = new LOC(mfLoc.data);
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								displayId dis = new displayId();
								dis.id = locNameId;
								dis.defaultName = locName;

								l.ids.names.Add(dis.id);

								foreach (LOC.Language lo in l.langs)
									lo.names.Add(dis.defaultName);
								mfLoc.data = l.Rebuild();
								locNameId = "";
								locName = "";
							}

							//creates metadata id in loc file
							if (locThemeId != "" && locTheme != "")
							{
								LOC l;

								try
								{
									l = new LOC(mfLoc.data);
								}
								catch
								{
									MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}

								displayId b = new displayId();
								b.id = locThemeId;
								b.defaultName = locTheme;

								l.ids.names.Add(b.id);

								foreach (LOC.Language lo in l.langs)
									lo.names.Add(b.defaultName);

								mfLoc.data = l.Rebuild();
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
			TreeNode NEW = new TreeNode();
			NEW.ImageIndex = 0;
			NEW.SelectedImageIndex = 0;
			NEW.Text = "New Folder";
			if (treeViewMain.SelectedNode.Tag == null)
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

		#region opens pck installation page
		private void installationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(hosturl + "pckStudio#install");
		}
		#endregion

		#region opens pck binka tutorial video
		private void binkaConversionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
		}
		#endregion

		#region opens pck donation page
		private void donateToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}
		#endregion

		#region opens pck faq page
		private void fAQToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(hosturl + "pckStudio#faq");
		}
		#endregion

		#region items class for use in bedrock skin conversion
		public class Item
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}
		#endregion

		#region converts and ports all skins in pck to mc bedrock format
		private void convertToBedrockToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openedPCKS.Visible == true && MessageBox.Show("Convert " + openedPCKS.SelectedTab.Text + " to a Bedrock Edition format?", "Convert", MessageBoxButtons.YesNo, MessageBoxIcon.None) == DialogResult.Yes)
			{
				try
				{
					bool latest = true;

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
						string uuid = "99999999";//default

						//creates list of skin display names
						List<Item> skinDisplayNames = new List<Item>();

						//MessageBox.Show(root);//debug thingy to make sure filepath is correct

						//add all skins to a list
						List<PCK.MineFile> skinsList = new List<PCK.MineFile>();
						List<PCK.MineFile> capesList = new List<PCK.MineFile>();
						foreach (PCK.MineFile skin in currentPCK.mineFiles)
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
							foreach (PCK.MineFile newSkin in skinsList)
							{
								skinAmount += 1;
								string skinName = "skinName";
								string capePath = "";
								bool hasCape = false;

								foreach (Object[] entry in newSkin.entries)
								{
									if (entry[0].ToString() == "DISPLAYNAME")
									{
										skinName = entry[1].ToString();
										skinDisplayNames.Add(new Item() { Id = newSkin.name.Remove(15, 4), Name = entry[1].ToString() });
									}
									if (entry[0].ToString() == "CAPEPATH")
									{
										hasCape = true;
										capePath = entry[1].ToString();
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
							foreach (PCK.MineFile newSkin in skinsList)
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
									foreach (Object[] entry in newSkin.entries)
									{
										if (entry[0].ToString() == "BOX")
										{
											string mClass = "";
											string mData = "";
											foreach (char dCheck in entry[1].ToString())
											{
												if (dCheck.ToString() != " ")
												{
													mClass += dCheck.ToString();
												}
												else
												{
													mData = entry[1].ToString().Remove(0, mClass.Count() + 1);
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

										if (entry[0].ToString() == "OFFSET")
										{
											string oClass = "";
											string oData = "";
											foreach (char oCheck in entry[1].ToString())
											{
												oData = entry[1].ToString();
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

										if (entry[0].ToString() == "ANIM")
										{
											if (entry[1].ToString() == "0x40000")
											{

											}
											else if (entry[1].ToString() == "0x80000")
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
						foreach (PCK.MineFile skinTexture in skinsList)
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
						foreach (PCK.MineFile capeTexture in capesList)
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

				SaveFileDialog exportDs = new SaveFileDialog();
				exportDs.ShowDialog();
				string currentFile = exportDs.FileName;

				bch = new loadedBCH();

				using (FileStream data = new FileStream(currentFile, FileMode.Open))
				{
					BinaryReader input = new BinaryReader(data);
					BinaryWriter output = new BinaryWriter(data);
					
					MemoryStream png = new MemoryStream(((PCK.MineFile)(treeViewMain.SelectedNode.Tag)).data); //Gets image data from minefile data
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
			pckCenter open = new pckCenter();
			open.Show();
		}

		private void tutorialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(hosturl + "pckStudio#tutorials");
		}

		private void wiiUPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			installWiiU install = new installWiiU(null);
			install.ShowDialog();
		}

		private void howToMakeABasicSkinPackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=A43aHRHkKxk");
		}

		private void howToMakeACustomSkinModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=pEC_ug55lag");
		}

		private void howToMakeCustomSkinModelsbedrockToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=6z8NTogw5x4");
		}

		private void howToMakeCustomMusicToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=v6EYr4zc7rI");
		}

		private void howToInstallPcksDirectlyToWiiUToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=hRQagnEplec");
		}

		private void pCKCenterReleaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=E_6bXSh6yqw");
		}

		private void howPCKsWorkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=hTlImrRrCKQ");
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

		private void uPDATEToolStripMenuItem1_Click(object sender, EventArgs e)
		{

			if (new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "") != Version)
			{
				Console.WriteLine(new WebClient().DownloadString(basurl + "updatePCKStudio.txt").Replace("\n", "") + " != " + Version);
				if (MessageBox.Show("Update avaliable!\ndo you want to update?", "UPDATE", MessageBoxButtons.YesNo) == DialogResult.Yes)
					Process.Start(Environment.CurrentDirectory + "\\nobleUpdater.exe");
				else
					uPDATEToolStripMenuItem1.Visible = true;
			}
			else
			{
				uPDATEToolStripMenuItem1.Visible = false;
			}
		}

		private void VitaPCKInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{

			installVita install = new installVita(null);
			install.ShowDialog();
		}

		private void toPhoenixARCDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://cash.app/$PhoenixARC");
		}

		private void toNobledezJackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.paypal.me/realnobledez");
		}

		private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void addPasswordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeViewMain.SelectedNode = treeViewMain.Nodes[0];
			mf = (PCK.MineFile)treeViewMain.Nodes[0].Tag;//Sets minefile to selected node
				foreach (object[] entry in mf.entries)
				{
					if (entry[0].ToString() == "LOCK")
					{
						MessageBox.Show("Remove current LOCK before adding a new one!");
					return;
					}
				}
			AddPCKPassword add = new AddPCKPassword(mf, currentPCK);//sets metadata adding dialog
			add.ShowDialog();//displays metadata adding dialog
			add.Dispose();//diposes generated metadata adding dialog data

			//Sets up combobox for metadata entries from main metadatabase
			treeMeta.Nodes.Clear();
			foreach (int type in types.Keys)
				comboBox1.Items.Add(types[type]);

			//loads all of selected minefiles metadata into metadata treeview
			foreach (object[] entry in file.entries)
			{
				object[] strings = (object[])entry; TreeNode meta = new TreeNode();

				foreach (object[] entryy in file.entries)
					meta.Text = (string)strings[0];
				meta.Tag = entry;
				treeMeta.Nodes.Add(meta);
			}
			saved = false;
		}

		private void joinDevelopmentDiscordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://discord.gg/Byh4hcq25w");
		}

		private void tSTToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Testx_12 form1 = new Testx_12();
			form1.Show();
		}

		private void convertPCTextrurePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.TextureConverterUtility tex = new PckStudio.Forms.Utilities.TextureConverterUtility(treeViewMain, currentPCK);
			tex.ShowDialog();
		}

#endregion


		private void buttonEditModel_Click(object sender, EventArgs e)
		{
			PCK.MineFile mf = (PCK.MineFile)treeViewMain.SelectedNode.Tag;

			if (Path.GetExtension(mf.name) == ".png")
			{
				if (buttonEdit.Text == "EDIT BOXES")
					editModel(mf);
				else if (buttonEdit.Text == "View Skin")
				{
					using (var ms = new MemoryStream(mf.data))
					{
						SkinPreview frm = new SkinPreview(Image.FromStream(ms));
						frm.Show();
					}
				}
			}

			if (Path.GetFileName(mf.name) == "audio.pck")
			{
					try
					{
						PckStudio.Forms.Utilities.AudioEditor diag = new PckStudio.Forms.Utilities.AudioEditor(mf.data, mf);
						diag.Show();
					}
					catch
					{
						MessageBox.Show("Invalid data", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
						return;
					}
			}

			if (Path.GetExtension(mf.name) == ".loc")
			{
				LOC l;
				try
				{
					l = new LOC(mf.data);
				}
				catch
				{
					MessageBox.Show("No localization data found.", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}
						(new LOCEditor(l)).ShowDialog();//Opens LOC Editor
				mf.data = l.Rebuild();//Rebuilds loc file with locdata in grid view after closing dialog
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
			if (saved == false)
			{
				if (MessageBox.Show("Save PCK?", "Unsaved PCK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					if (saveLocation == Application.StartupPath + @"\templates\UntitledSkinPCK.pck")
					{
						save("Save As");
					}
					else
					{
						save("Save");
					}
				}
			}
			if (needsUpdate)
			{
				Process UPDATE = new Process();//sets up updater
				UPDATE.StartInfo.FileName = Application.StartupPath + @"\nobleUpdater.exe";//updater program path
				UPDATE.Start();//starts updater
				Application.Exit();//closes PCK Studio to let updatear finish the job
			}
		}

		private void OpenPck_DragEnter(object sender, DragEventArgs e)
		{
			pckOpen.Image = Resources.pckDrop;
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
			{
				var ext = System.IO.Path.GetExtension(file);
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
				openPck(pck);
			}
		}

		private void OpenPck_DragLeave(object sender, EventArgs e)
		{
			pckOpen.Image = Resources.pckClosed;
		}

		private void savePCK(object sender, EventArgs e)
		{
			save("Save");
		}

		private void saveAsPCK(object sender, EventArgs e)
		{
			save("Save As");
		}

		private void openPck(object sender, EventArgs e)
		{

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (PCKFile != PCKFileBCKUP)
			{
				RPC.CloseRPC();
				if (string.IsNullOrWhiteSpace(PCKFile))
					try
					{
						RPC.SetRPC("825875166574673940", "Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
					}
					catch
					{
						Console.WriteLine("ERROR WITH RPC");
					}
				else

					try
					{
						RPC.SetRPC("825875166574673940", "Developing " + PCKFile, "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
					}
					catch
					{
						Console.WriteLine("ERROR WITH RPC");
					}
				PCKFileBCKUP = PCKFile;
			}
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				RPC.CloseRPC();
			}
			catch { }
		}

		private void FormMain_Deactivate(object sender, EventArgs e)
		{
			try
			{
				RPC.CloseRPC();
				timer1.Stop();
				timer1.Enabled = false;
			}
			catch { }
		}

		private void FormMain_Activated(object sender, EventArgs e)
		{
			try
			{
			RPC.SetRPC("825875166574673940", "Sitting alone", "Program by PhoenixARC", "pcklgo", "PCK Studio", "pcklgo");
			timer1.Start();
			timer1.Enabled = true;
			}
			catch { }
		}
	}
}




