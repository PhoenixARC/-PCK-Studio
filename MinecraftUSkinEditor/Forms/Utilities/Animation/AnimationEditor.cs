using MetroFramework.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PckStudio
{
	public partial class AnimationEditor : MetroForm
	{
		TreeView treeViewMain = new TreeView();
		PCKFile.FileData mf = null;
		List<Image> frames = new List<Image>();
		Newtonsoft.Json.Linq.JObject tileData = Newtonsoft.Json.Linq.JObject.Parse(System.Text.Encoding.Default.GetString(Properties.Resources.tileData));
		Image texture;
		int frameCount;
		bool isItem = false;
		string lastFrameTime = "1";
		string newTileName = "";
		bool create = false;

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Tuple<string, string> frameData = e.Node.Tag as Tuple<string, string>;
			Console.WriteLine(frameData.Item1 + " --- " + frameData.Item2);
			if (AnimationPlayBtn.Enabled)
			{
				pictureBoxWithInterpolationMode1.Image = frames[short.Parse(frameData.Item1)];
			}
		}

		public AnimationEditor(TreeView treeViewIn, string createdFileName = "")
		{
			InitializeComponent();
			treeViewMain = treeViewIn;
			if (string.IsNullOrEmpty(createdFileName))
			{
				newTileName = Path.GetFileNameWithoutExtension(treeViewMain.SelectedNode.Text);
				if (treeViewMain.SelectedNode.Parent.Text.ToLower() == "items") isItem = true;
				mf = treeViewMain.SelectedNode.Tag as PCKFile.FileData;
				if (newTileName.EndsWith("MipMapLevel2") || newTileName.EndsWith("MipMapLevel3"))
				{
					string mipMapLvl = newTileName.Last().ToString();
					newTileName = newTileName.Substring(0, newTileName.Length - 12);
					MipMapCheckbox.Checked = true;
					MipMapNumericUpDown.Value = short.Parse(mipMapLvl);
				}
			}
			else
			{
				create = true;
				PCKFile.FileData newMf = new PCKFile.FileData("", 2);
				newMf.properties.Add(new ValueTuple<string, string>("ANIM", ""));
				newMf.SetData(File.ReadAllBytes(createdFileName));
				mf = newMf;
				Forms.Utilities.AnimationEditor.ChangeTile diag = new Forms.Utilities.AnimationEditor.ChangeTile();
				diag.ShowDialog(this);
				Console.WriteLine(diag.SelectedTile);
				newTileName = diag.SelectedTile;
				if (newTileName == "") Close();
				isItem = diag.IsItem;
				diag.Dispose();
			}

			List<string> strEntries = new List<string>();
			List<string> strEntryData = new List<string>();

			string anim_data = string.Empty;
			foreach (var entry in mf.properties)
			{
				if (entry.Item1 == "ANIM") anim_data = entry.Item2;
                strEntries.Add(entry.Item2);
                strEntryData.Add(entry.Item2);
			}

			MemoryStream textureMem = new MemoryStream(mf.data);
			texture = Image.FromStream(textureMem);
			createFrameList();

			Console.WriteLine(newTileName);

			foreach (Newtonsoft.Json.Linq.JObject content in tileData[isItem ? "Items" : "Blocks"].Children())
			{
				foreach (JProperty prop in content.Properties())
				{
					if (prop.Name == newTileName) tileLabel.Text = (string)prop.Value;
				}
			}

			string anim = "";
			if (strEntries.Find(entry => entry == "ANIM") == null) anim = "";
			else anim = strEntryData[strEntries.FindIndex(entry => entry == "ANIM")];
			Console.WriteLine("ANIMATION DATA: " + anim);
			if (InterpolationCheckbox.Checked = anim.StartsWith("#"))
			{
				anim = anim.Remove(0, 1);
			}

			frameCount = texture.Height / texture.Width;

			if (!string.IsNullOrEmpty(anim))
			{
				string[] animData = anim.Split(',');
				if (string.IsNullOrEmpty(animData.Last())) animData = animData.Take(animData.Length - 1).ToArray();
				foreach (string frame in animData)
				{
					string[] frameData = frame.Split(new char[] { '*' });
					string outFrame = "";
					int i = 0;
					string currentFrame = "";
					string currentFrameTime = "";
					foreach (string data in frameData)
					{
						string label;
						string outData;
						if (i == 0)
						{
							outData = data;
							if (string.IsNullOrEmpty(data)) throw new Exception("Invalid animation data");
							label = "Frame: ";
							currentFrame = outData;
						}
						else
						{
							outData = data;
							// Some textures like the Halloween 2015's Lava texture don't have a
							// frame time parameter for certain frames. This will detect that and place the last frame time in its place.
							// This is accurate to console edition behavior.
							// - MattNL
							if (string.IsNullOrEmpty(data)) outData = lastFrameTime;
							label = ", Frame Time: ";
							currentFrameTime = outData;
						}
						outFrame += label + outData;
						i++;
					}
					Console.WriteLine(outFrame);

					TreeNode frameNode = new TreeNode();
					Tuple<string, string> finalFrameData = new Tuple<string, string>(currentFrame, currentFrameTime);
					lastFrameTime = currentFrameTime;
					frameNode.Text = outFrame;
					frameNode.Tag = finalFrameData;
					treeView1.Nodes.Add(frameNode);
				}
			}
			else
			{
				for (int i = 0; i < frameCount; i++)
				{
					TreeNode frameNode = new TreeNode();
					Tuple<string, string> finalFrameData = new Tuple<string, string>(i.ToString(), "1");
					frameNode.Text = "Frame: " + i.ToString() + ", Frame Time: 1";
					frameNode.Tag = finalFrameData;
					treeView1.Nodes.Add(frameNode);
				}
			}

			pictureBoxWithInterpolationMode1.Image = frames[0]; //Sets image preview to the first frame of animation (0 for now)
			Console.WriteLine("Animation Frame Count: " + frameCount);
		}

		void createFrameList()
		{
			frames.Clear();
			int width = texture.Width;
			int height = texture.Height;
			int totalFrames = height / width;
			for (int frameI = 0; frameI < totalFrames; frameI++)
			{
				Rectangle frameArea = new Rectangle(new Point(0, frameI * width), new Size(width, width));

				Bitmap frameImage = new Bitmap(width, width);
				using (Graphics gfx = Graphics.FromImage(frameImage))
				{
					gfx.SmoothingMode = SmoothingMode.None;
					gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

					gfx.DrawImage(texture, new Rectangle(0, 0, frameImage.Width, frameImage.Height), frameArea, GraphicsUnit.Pixel);
				}

				frames.Add(new Bitmap(frameImage, new Size(width, width)));
			}

		}

		private int mix(double ratio, int val1, int val2) // Ported from Java Edition code
		{
			return (int)(ratio * (double)val1 + (1.0D - ratio) * (double)val2);
		}

		int animCurrentFrame = 0;
		Tuple<string, string> currentFrameData = new Tuple<string, string>("", "");
		Image img = null;
		int nextFrame;
		//int frameCounter = 0; // ported directly from Java Edition code -MattNL
		Image imgB = null;
		void animate(object sender, EventArgs e)
		{
			//Console.WriteLine(frameCounter + " $$$ " + frameCount);
			//frameCounter = (frameCounter + 1) % frameCount;
			if (animCurrentFrame > (treeView1.Nodes.Count - 1)) animCurrentFrame = 0;
			currentFrameData = treeView1.Nodes[animCurrentFrame].Tag as Tuple<string, string>;
			pictureBoxWithInterpolationMode1.Image = frames[Int16.Parse(currentFrameData.Item1)];
			//animCurrentTotalFrameTime = Int16.Parse(currentFrameData.Item2);
			timer1.Interval = Int16.Parse(currentFrameData.Item2) * 50;
			animCurrentFrame++;

			if (InterpolationCheckbox.Checked)
			{
				img = frames[short.Parse(currentFrameData.Item1)];
				nextFrame = animCurrentFrame + 1;
				if (nextFrame > frameCount - 1) nextFrame = 0;
				Console.WriteLine(nextFrame);
				imgB = frames[nextFrame];
			}

			#region interpolation code (unoptimized and unused at the moment)
			// Interpolation Code (Very slow, messy, and resource heavy depending on the resolution!!!)

			/*else if(metroCheckBox1.Checked && (img != null && imgB != null))
			{
				double d0 = 1.0D - animCurrentFrame / animCurrentTotalFrameTime;
				int i = animCurrentFrame;
				int j = frameCount;
				int k = (frameCounter + 1) % j;

				for (int l = 0; l < (frames.Count() - 1); ++l)
				{
					int i1 = img.Width;
					int j1 = img.Width;

					Bitmap finalInterpolation = new Bitmap(pictureBoxWithInterpolationMode1.Image);
//					pictureBoxWithInterpolationMode1.Image.Dispose();
//					pictureBoxWithInterpolationMode1.Image = null;

					for (int k1 = 0; k1 < j1; ++k1)
					{
						for (int l1 = 0; l1 < i1; ++l1)
						{
							//Get Both Colours at the pixel point
							Bitmap imgC = new Bitmap(img);
							Bitmap imgBC = new Bitmap(imgB);
							Color col1 = imgC.GetPixel(l1, k1);
							Color col2 = imgBC.GetPixel(l1, k1);
							imgC.Dispose();
							imgC = null;
							imgBC.Dispose();
							imgBC = null;

							int i2 = 0;
							i2 |= col1.A << 24;
							i2 |= col1.R << 16;
							i2 |= col1.G << 8;
							i2 |= col1.B;

							int j2 = 0;
							j2 |= col2.A << 24;
							j2 |= col2.R << 16;
							j2 |= col2.G << 8;
							j2 |= col2.B;

							int k2 = this.mix(d0, i2 >> 16 & 255, j2 >> 16 & 255);
							int l2 = this.mix(d0, i2 >> 8 & 255, j2 >> 8 & 255);
							int i3 = this.mix(d0, i2 & 255, j2 & 255);

							// Create new grayscale RGB colour
							uint finalColor = (uint)(i2 & -16777216 | k2 << 16 | l2 << 8 | i3);

							byte[] values = BitConverter.GetBytes(finalColor);

							int a = values[3];
							int b = values[0];
							int g = values[1];
							int r = values[2];

							Color newcol = Color.FromArgb(a, r, g, b);

							finalInterpolation.SetPixel(l1, k1, newcol);
						}
					}

					pictureBoxWithInterpolationMode1.Image = finalInterpolation;
					//finalInterpolation.Dispose();
					finalInterpolation = null;
				}
			}
			*/
			#endregion

			//Console.WriteLine(animCurrentFrame + " - " + animCurrentFrameTime + " - " + animCurrentTotalFrameTime + " - " + (treeView1.Nodes.Count - 1));
		}

		private void metroButton1_Click(object sender, EventArgs e)
		{
			animCurrentFrame = 0;
			//animCurrentFrameTime = 0;
			//animCurrentTotalFrameTime = -1;
			//frameCounter = 0;
			AnimationPlayBtn.Enabled = false;
			AnimationStopBtn.Enabled = true;
			timer1.Start();
		}

		private void metroButton2_Click(object sender, EventArgs e)
		{
			AnimationPlayBtn.Enabled = true;
			AnimationStopBtn.Enabled = false;
			timer1.Stop();
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete) treeView1.Nodes.Remove(treeView1.SelectedNode);
		}

		private TreeNode FindNodeByName(TreeNode treeNode, string name)
		{
			foreach (TreeNode node in treeNode.Nodes)
			{
				if (node.Text.ToLower() == name.ToLower()) return node;
				return FindNodeByName(node, name);
			}
			return null;
		}

		private TreeNode FindNodeByName(TreeView treeView, string name)
		{
			foreach (TreeNode node in treeView.Nodes)
			{
				if (node.Text.ToLower() == name.ToLower()) return node;
				return FindNodeByName(node, name);
			}
			return null;
		}

		private void addNodeToAnimationsFolder(TreeNode newNode)
		{
			TreeNode parent = FindNodeByName(treeViewMain, isItem ? "items" : "blocks");
			if (parent != null)
			{
				Console.WriteLine("ParentNotNULL");
				TreeNode check = FindNodeByName(treeViewMain, newNode.Text);
				parent.Nodes.Add(newNode);
			}
			else
			{
				TreeNode texturesParent = FindNodeByName(treeViewMain, "textures");
				if (texturesParent != null)
				{
					Console.WriteLine("TextureNotNULL");
					TreeNode newFolder = new TreeNode(isItem ? "items" : "blocks");
					texturesParent.Nodes.Add(newFolder);
					newFolder.Nodes.Add(newNode);
				}
				else
				{
					TreeNode resParent = FindNodeByName(treeViewMain, "res");
					if (resParent != null)
					{
						Console.WriteLine("ResNotNULL");
						TreeNode newFolder = new TreeNode("textures");
						resParent.Nodes.Add(newFolder);
						TreeNode newFolderB = new TreeNode(isItem ? "items" : "blocks");
						newFolder.Nodes.Add(newFolderB);
						newFolderB.Nodes.Add(newNode);
					}
					else
					{
						Console.WriteLine("ResNULL");
						TreeNode newFolder = new TreeNode("res");
						treeViewMain.Nodes.Add(newFolder);
						TreeNode newFolderB = new TreeNode("textures");
						newFolder.Nodes.Add(newFolderB);
						TreeNode newFolderC = new TreeNode(isItem ? "items" : "blocks");
						newFolderB.Nodes.Add(newFolderC);
						newFolderC.Nodes.Add(newNode);
					}
				}
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				texture.Save(stream, texture.RawFormat);
				mf.SetData(stream.ToArray());
			}

			if (MipMapCheckbox.Checked)
			{
				newTileName += "MipMapLevel" + MipMapNumericUpDown.Value.ToString();
			}

			if (!create && treeViewMain.SelectedNode.Tag != null) treeViewMain.SelectedNode.Text = newTileName + ".png";

			string animationData = "";
			if (InterpolationCheckbox.Checked) animationData += "#"; // does the animation interpolate?
			foreach (TreeNode node in treeView1.Nodes)
			{
				Tuple<string, string> frameData = node.Tag as Tuple<string, string>;
				animationData += frameData.Item1 + "*" + frameData.Item2 + ",";
			}
			animationData.TrimEnd(',');
			foreach (var pair in mf.properties)
			{
				if (pair.Item1 == "ANIM")
				{
					//pair.Item2 = animationData; // TODO
					break;
				}
				else
				{
					mf.properties.Add(new ValueTuple<string, string>("ANIM", animationData));
					break;
				}
			};

			if (create)
			{
				mf.name = "res/textures/" + (isItem ? "items" : "blocks");
				TreeNode newNode = new TreeNode(newTileName + ".png") { Tag = mf };//creates node for minefile
				newNode.ImageIndex = 2;
				newNode.SelectedImageIndex = 2;
				addNodeToAnimationsFolder(newNode);
				treeViewMain.SelectedNode = newNode;
				create = false;
			}
			else if (isItem && treeViewMain.SelectedNode.Parent.Text == "blocks")
			{
				Console.WriteLine("block: " + treeViewMain.SelectedNode.Parent.Text);
				TreeNode newNode = treeViewMain.SelectedNode;
				newNode.ImageIndex = 2;
				newNode.SelectedImageIndex = 2;
				treeViewMain.SelectedNode.Remove();
				addNodeToAnimationsFolder(newNode);
			}
			else if (treeViewMain.SelectedNode.Parent.Text == "items")
			{
				Console.WriteLine("item: " + treeViewMain.SelectedNode.Parent.Text);
				TreeNode newNode = treeViewMain.SelectedNode;
				newNode.ImageIndex = 2;
				newNode.SelectedImageIndex = 2;
				treeViewMain.SelectedNode.Remove();
				addNodeToAnimationsFolder(newNode);
			}

			if(MipMapCheckbox.Checked) newTileName = newTileName.Substring(0, newTileName.Length - 12);
		}

		// Most of the code below is modified code from this link: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
		// - MattNL

		private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// Move the dragged node when the left mouse button is used.
			if (e.Button == MouseButtons.Left)
			{
				DoDragDrop(e.Item, DragDropEffects.Move);
			}
		}

		// Set the target drop effect to the effect 
		// specified in the ItemDrag event handler.
		private void treeView1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		// Select the node under the mouse pointer to indicate the 
		// expected drop location.
		private void treeView1_DragOver(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the mouse position.
			Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

			// Select the node at the mouse position.
			treeView1.SelectedNode = treeView1.GetNodeAt(targetPoint);
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the drop location.
			Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

			// Retrieve the node at the drop location.
			TreeNode targetNode = treeView1.GetNodeAt(targetPoint);

			// Retrieve the node that was dragged.
			TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

			// Confirm that the node at the drop location is not 
			// the dragged node or a descendant of the dragged node.
			if (targetNode == null)
			{
				draggedNode.Remove();
				treeView1.Nodes.Add(draggedNode);
			}
			else if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
			{
				// If it is a move operation, remove the node from its current 
				// location and add it to the node at the drop location.

				if (e.Effect == DragDropEffects.Move)
				{
					int draggedIndex = draggedNode.Index;
					int targetIndex = targetNode.Index;
					draggedNode.Remove();

					if (targetNode.Tag == null) // Add to folder
					{
						targetNode.Nodes.Add(draggedNode);
					}
					else // Move file aside
					{
						if (targetNode.Parent != null)
						{
							targetNode.Parent.Nodes.Insert(targetIndex, draggedNode);
						}
						else
						{
							treeView1.Nodes.Insert(targetIndex, draggedNode);
						}
					}
				}

				// Expand the node at the location 
				// to show the dropped node.
				targetNode.Expand();
			}
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

		private void treeView1_doubleClick(object sender, EventArgs e)
		{
			Forms.Utilities.AnimationEditor.FrameEditor diag = new Forms.Utilities.AnimationEditor.FrameEditor(
				treeView1, // animation editor tree
				treeView1.SelectedNode.Tag as Tuple<string, string>, // the current selected frame data
				frameCount - 1, // frame limit
				false, // create new frame?
				treeView1.SelectedNode // the current frame selected
				);
			diag.ShowDialog(this);
			diag.Dispose();
		}

		private void addFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Forms.Utilities.AnimationEditor.FrameEditor diag = new Forms.Utilities.AnimationEditor.FrameEditor(
				treeView1,
				new Tuple<string, string>("", ""),
				frameCount - 1,
				true,
				new TreeNode());
			diag.ShowDialog(this);
			diag.Dispose();
		}

		private void removeFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView1.SelectedNode.Remove();
		}

		private void bulkAnimationSpeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Forms.Utilities.AnimationEditor.SetBulkSpeed diag = new Forms.Utilities.AnimationEditor.SetBulkSpeed(treeView1);
			diag.ShowDialog(this);
			diag.Dispose();
		}

		private void importJavaAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult query = MessageBox.Show("This feature will replace the existing animation data. It might fail if the selected animation script is invalid. Are you sure that you want to continue?", "Warning", MessageBoxButtons.YesNo);
			if (query == DialogResult.No) return;

			// In case the import fails, the user won't lose any data - MattNL
			Image oldImage = texture;
			int oldFrameCount = frameCount;
			List<Image> oldFrames = frames;
			TreeNodeCollection oldAnimData = treeView1.Nodes;

			OpenFileDialog diag = new OpenFileDialog();
			diag.Multiselect = false;
			diag.Filter = "Animation Scripts (*.mcmeta)|*.png.mcmeta"; /* It's marked as .png.mcmeta just in case
																	   some weirdo tries to pass a pack.mcmeta or something
																	   -MattNL */
			diag.Title = "Please select a valid Minecaft: Java Edition animation script";
			diag.ShowDialog(this);
			diag.Dispose();
			if (String.IsNullOrEmpty(diag.FileName)) return; // Return if name is null or if the user cancels
			Console.WriteLine("Selected Animation Script: " + diag.FileName);

			treeView1.Nodes.Clear();

			MemoryStream textureMem = new MemoryStream(File.ReadAllBytes(Path.GetDirectoryName(diag.FileName) + "\\/" + Path.GetFileNameWithoutExtension(diag.FileName)));
			texture = Image.FromStream(textureMem);
			frameCount = texture.Height / texture.Width;
			createFrameList();

			try
			{
				Newtonsoft.Json.Linq.JObject mcmeta = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(diag.FileName));

				if (mcmeta["animation"] != null)
				{
					int frameTime = 1;
					// Some if statements to ensure that the animation is valid.
					if (mcmeta["animation"]["frametime"] != null &&
						mcmeta["animation"]["frametime"].Type == JTokenType.Integer) frameTime = (int)mcmeta["animation"]["frametime"];
					if (mcmeta["animation"]["interpolate"] != null &&
						mcmeta["animation"]["interpolate"].Type == JTokenType.Boolean && (Boolean)mcmeta["animation"]["interpolate"] == true) InterpolationCheckbox.Checked = true;
					if (mcmeta["animation"]["frames"] != null &&
						mcmeta["animation"]["frames"].Type == JTokenType.Array)
					{
						foreach (JToken frame in mcmeta["animation"]["frames"].Children())
						{
							if (frame.Type == JTokenType.Object)
							{
								if (frame["index"] != null && frame["index"].Type == JTokenType.Integer &&
									frame["time"] != null && frame["time"].Type == JTokenType.Integer)
								{
									Console.WriteLine((int)frame["index"] + "*" + (int)frame["time"]);

									TreeNode frameNode = new TreeNode();
									Tuple<string, string> finalFrameData = new Tuple<string, string>(((int)frame["index"]).ToString(), ((int)frame["time"]).ToString());
									frameNode.Text = "Frame: " + ((int)frame["index"]).ToString() + ", Frame Time: " + ((int)frame["time"]).ToString();
									frameNode.Tag = finalFrameData;
									treeView1.Nodes.Add(frameNode);
								}
							}
							else if (frame.Type == JTokenType.Integer)
							{
								Console.WriteLine((int)frame + "*" + frameTime);

								TreeNode frameNode = new TreeNode();
								Tuple<string, string> finalFrameData = new Tuple<string, string>(((int)frame).ToString(), frameTime.ToString());
								frameNode.Text = "Frame: " + ((int)frame).ToString() + ", Frame Time: " + frameTime.ToString();
								frameNode.Tag = finalFrameData;
								treeView1.Nodes.Add(frameNode);
							}
						}
					}
					else
					{
						for (int i = 0; i < frameCount; i++)
						{
							TreeNode frameNode = new TreeNode();
							Tuple<string, string> finalFrameData = new Tuple<string, string>(i.ToString(), frameTime.ToString());
							frameNode.Text = "Frame: " + i.ToString() + ", Frame Time: " + frameTime.ToString();
							frameNode.Tag = finalFrameData;
							treeView1.Nodes.Add(frameNode);
						}
					}
				}
			}
			catch (JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Invalid animation");
				texture = oldImage;
				frameCount = oldFrameCount;
				frames = oldFrames;
				foreach (TreeNode node in oldAnimData)
				{
					treeView1.Nodes.Add(node);
				}
				return;
			}
			pictureBoxWithInterpolationMode1.Image = frames[Int16.Parse((treeView1.Nodes[0].Tag as Tuple<string, string>).Item1)];
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Simply drag and drop frames in the tree to rearrange your animation.\n\n" +
							"The \"Interpolates\" checkbox enables the blending animation seen with some textures in the game, such as Prismarine.\n\n" +
							"You can preview your animation at any time by simply pressing the \"Play Animation\" button!\n\n" +
							"You can edit the frame and its speed by double clicking a frame in the tree. If you'd like to change the entire animation's speed, you can do so with the \"Set Bulk Animation Speed\" button in the \"Tools\" tab.\n\n" +
							"Porting animations from Java packs are made simple with the \"Import Java Animation\" button found in the \"Tools\" tab!", "Help");
		}

		private void changeTileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.AnimationEditor.ChangeTile diag = new Forms.Utilities.AnimationEditor.ChangeTile(newTileName);
			diag.ShowDialog(this);
			Console.WriteLine(diag.SelectedTile);
			if (newTileName != diag.SelectedTile) isItem = diag.IsItem;
			newTileName = diag.SelectedTile;
			diag.Dispose();
			foreach (Newtonsoft.Json.Linq.JObject content in tileData[isItem ? "Items" : "Blocks"].Children())
			{
				foreach (Newtonsoft.Json.Linq.JProperty prop in content.Properties())
				{
					if (prop.Name == newTileName) tileLabel.Text = (string)prop.Value;
				}
			}
		}

		private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
		{
			metroLabel1.Visible = MipMapCheckbox.Checked;
			MipMapNumericUpDown.Visible = MipMapCheckbox.Checked;
		}
	}
}
