using MetroFramework.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PckStudio.Classes.FileTypes;
using PckStudio.Forms.Utilities.AnimationEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PckStudio
{
	public partial class AnimationEditor : MetroForm
	{
		PCKFile.FileData animationFile = null;
		List<Image> frames = new List<Image>();
		int frameCount => frames.Count;
        static JObject tileData = JObject.Parse(Properties.Resources.tileData);
		Image texture = null;
		bool isItem = false;
		int minimumFrameTime = 1;
		string TileName = "";
		int animCurrentFrame = 0;
		Tuple<int, int> currentFrameData = new Tuple<int, int>(0, 1);
		Image img = null;
		Image imgB = null;
		int nextFrame;
		//int frameCounter = 0; // ported directly from Java Edition code -MattNL

		public AnimationEditor(Image imgage, string tileName, bool isItem)
        {
			InitializeComponent();
			this.isItem = isItem;
			TileName = tileName;
			string filePath = $"res/textures/{(isItem ? "items" : "blocks")}/{tileName}.png";
			animationFile = CreateNewAnimationFile(imgage, filePath);
			CreateFrameList(imgage);
		}

		public AnimationEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			isItem = file.filepath.Split('/').Contains("items");
			TileName = Path.GetFileNameWithoutExtension(file.filepath);
			animationFile = file;
			if (TileName.EndsWith("MipMapLevel2") || TileName.EndsWith("MipMapLevel3"))
			{
				string mipMapLvl = TileName.Last().ToString();
				TileName = TileName.Substring(0, TileName.Length - 12);
				MipMapCheckbox.Checked = true;
				MipMapNumericUpDown.Value = short.Parse(mipMapLvl);
			}

			string anim = string.Empty;
			animationFile.properties.FirstOrDefault(x => x.Item1.Equals("ANIM"));

			MemoryStream textureMem = new MemoryStream(animationFile.data);
			texture = new Bitmap(textureMem);
			CreateFrameList(texture);

			Console.WriteLine(TileName);

			foreach (JObject content in tileData[isItem ? "Items" : "Blocks"].Children())
			{
				foreach (JProperty prop in content.Properties())
				{
					if (prop.Name == TileName) tileLabel.Text = (string)prop.Value;
				}
			}

			if (!string.IsNullOrEmpty(anim))
			{
				string[] animData = anim.Split(',');
				if (string.IsNullOrEmpty(animData.Last())) animData = animData.Take(animData.Length - 1).ToArray();
				int lastFrameTime = 0;
				foreach (string frame in animData)
				{
					string[] frameData = frame.Split('*');
					if (frameData.Length < 2)
						continue; // shouldn't happen
					int currentFrame = 0;
					int currentFrameTime = 1;

					if (string.IsNullOrEmpty(frameData[0])) throw new Exception("Invalid animation data");
					currentFrame = int.Parse(frameData[0]);

					// Some textures like the Halloween 2015's Lava texture don't have a
					// frame time parameter for certain frames.
					// This will detect that and place the last frame time in its place.
					// This is accurate to console edition behavior.
					// - MattNL
					currentFrameTime = string.IsNullOrEmpty(frameData[1]) ? lastFrameTime : int.Parse(frameData[1]);
					string label = $"Frame: {currentFrame}, Frame Time: {currentFrameTime}";
					Console.WriteLine(label);

					TreeNode frameNode = new TreeNode(label);
					var finalFrameData = new Tuple<int, int>(currentFrame, currentFrameTime);
					frameNode.Tag = finalFrameData;
					treeView1.Nodes.Add(frameNode);
					lastFrameTime = currentFrameTime;
				}
			}
			else
			{
				for (int i = 0; i < frameCount; i++)
				{
					TreeNode frameNode = new TreeNode($"Frame: {i}, Frame Time: {minimumFrameTime}");
					var finalFrameData = new Tuple<int, int>(i, minimumFrameTime);
					frameNode.Tag = finalFrameData;
					treeView1.Nodes.Add(frameNode);
				}
			}

			pictureBoxWithInterpolationMode1.Image = frames[0]; //Sets image preview to the first frame of animation (0 for now)
			Console.WriteLine("Animation Frame Count: " + frameCount);
		}
		
		private PCKFile.FileData CreateNewAnimationFile(Image imgageFile, string name = "")
        {
			PCKFile.FileData file = new PCKFile.FileData(name, 2);
			file.properties.Add(("ANIM", ""));
			using (var stream = new MemoryStream())
			{
				imgageFile.Save(stream, ImageFormat.Png);
				file.SetData(stream.ToArray());
			}
			return file;
		}
		
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var frameData = e.Node.Tag as Tuple<int, int>;
			Console.WriteLine(frameData.Item1 + " --- " + frameData.Item2);
			if (AnimationPlayBtn.Enabled)
			{
				pictureBoxWithInterpolationMode1.Image = frames[frameData.Item1];
			}
		}

		void CreateFrameList(Image texture)
		{
			frames.Clear();
			frames.AddRange(SplitImageToFrames(texture));
		}

		private IEnumerable<Image> SplitImageToFrames(Image source)
		{
			for (int i = 0; i < source.Height / source.Width; i++)
			{
				Rectangle tileArea = new Rectangle(new Point(0, i * source.Width), new Size(source.Width, source.Width));
				Bitmap tileImage = new Bitmap(source.Width, source.Width);
				using (Graphics gfx = Graphics.FromImage(tileImage))
				{
					gfx.SmoothingMode = SmoothingMode.None;
					gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
					gfx.DrawImage(source, new Rectangle(0, 0, source.Width, source.Width), tileArea, GraphicsUnit.Pixel);
				}
				yield return tileImage;
			}
			yield break;
		}

		private int mix(double ratio, int val1, int val2) // Ported from Java Edition code
		{
			return (int)(ratio * val1 + (1.0D - ratio) * val2);
		}

		void animate(object sender, EventArgs e)
		{
			//Console.WriteLine(frameCounter + " $$$ " + frameCount);
			//frameCounter = (frameCounter + 1) % frameCount;
			if (animCurrentFrame > (treeView1.Nodes.Count - 1)) animCurrentFrame = 0;
			currentFrameData = treeView1.Nodes[animCurrentFrame].Tag as Tuple<int, int>;
			pictureBoxWithInterpolationMode1.Image = frames[currentFrameData.Item1];
			//animCurrentTotalFrameTime = Int16.Parse(currentFrameData.Item2);
			timer1.Interval = currentFrameData.Item2 * 50;
			animCurrentFrame++;

			if (InterpolationCheckbox.Checked)
			{
				img = frames[currentFrameData.Item1];
				nextFrame = animCurrentFrame + 1;
				if (nextFrame > frameCount - 1) nextFrame = 0;
				Console.WriteLine(nextFrame);
				imgB = frames[nextFrame];
			}

            #region interpolation code (unoptimized and unused at the moment)
            // Interpolation Code (Very slow, messy, and resource heavy depending on the resolution!!!)

            else if (InterpolationCheckbox.Checked && (img != null && imgB != null))
            {
                double d0 = 1.0D - animCurrentFrame / frameCount;
                int i = animCurrentFrame;
                int j = frameCount;
                int k = (frameCount + 1) % j;

                for (int l = 0; l < (frameCount - 1); ++l)
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
                }
            }

            #endregion

            //Console.WriteLine(animCurrentFrame + " - " + animCurrentFrameTime + " - " + animCurrentTotalFrameTime + " - " + (treeView1.Nodes.Count - 1));
        }

        private void StartAnimationBtn_Click(object sender, EventArgs e)
		{
			animCurrentFrame = 0;
			//animCurrentFrameTime = 0;
			//animCurrentTotalFrameTime = -1;
			//frameCounter = 0;
			AnimationPlayBtn.Enabled = !(AnimationStopBtn.Enabled = !AnimationStopBtn.Enabled);
			timer1.Start();
		}

		private void StopAnimationBtn_Click(object sender, EventArgs e)
		{
			AnimationPlayBtn.Enabled = !(AnimationStopBtn.Enabled = !AnimationStopBtn.Enabled);
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

		private void addNodeToAnimationsFolder(TreeNode newNode)
		{
			//TreeNode parent = FindNodeByName(treeViewMain, isItem ? "items" : "blocks");
			//if (parent != null)
			//{
			//	Console.WriteLine("ParentNotNULL");
			//	TreeNode check = FindNodeByName(treeViewMain, newNode.Text);
			//	parent.Nodes.Add(newNode);
			//}
			//else
			//{
			//	TreeNode texturesParent = FindNodeByName(treeViewMain, "textures");
			//	if (texturesParent != null)
			//	{
			//		Console.WriteLine("TextureNotNULL");
			//		TreeNode newFolder = new TreeNode(isItem ? "items" : "blocks");
			//		texturesParent.Nodes.Add(newFolder);
			//		newFolder.Nodes.Add(newNode);
			//	}
			//	else
			//	{
			//		TreeNode resParent = FindNodeByName(treeViewMain, "res");
			//		if (resParent != null)
			//		{
			//			Console.WriteLine("ResNotNULL");
			//			TreeNode newFolder = new TreeNode("textures");
			//			resParent.Nodes.Add(newFolder);
			//			TreeNode newFolderB = new TreeNode(isItem ? "items" : "blocks");
			//			newFolder.Nodes.Add(newFolderB);
			//			newFolderB.Nodes.Add(newNode);
			//		}
			//		else
			//		{
			//			Console.WriteLine("ResNULL");
			//			TreeNode newFolder = new TreeNode("res");
			//			treeViewMain.Nodes.Add(newFolder);
			//			TreeNode newFolderB = new TreeNode("textures");
			//			newFolder.Nodes.Add(newFolderB);
			//			TreeNode newFolderC = new TreeNode(isItem ? "items" : "blocks");
			//			newFolderB.Nodes.Add(newFolderC);
			//			newFolderC.Nodes.Add(newNode);
			//		}
			//	}
			//}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				texture.Save(stream, ImageFormat.Png);
				animationFile.SetData(stream.ToArray());
			}

			animationFile.filepath = $"res/textures/{(isItem ? "items" : "blocks")}/{TileName}{(MipMapCheckbox.Checked ? $"MipMapLevel{MipMapNumericUpDown.Value}" : string.Empty)}.png";

			string animationData = InterpolationCheckbox.Checked ? "#" : "";
			foreach (TreeNode node in treeView1.Nodes)
			{
				var frameData = node.Tag as Tuple<int, int>;
				animationData += $"{frameData.Item1}*{frameData.Item2},";
			}
			animationData.TrimEnd(',');
			foreach (var pair in animationFile.properties)
			{
				if (pair.Item1 == "ANIM")
				{
                    animationFile.properties[animationFile.properties.IndexOf(pair)] = ("ANIM", animationData);
                    break;
				}
				else
				{
					animationFile.properties.Add(("ANIM", animationData));
					break;
				}
			};

			//if (create)
			//{
			//	mf.name = "res/textures/" + (isItem ? "items" : "blocks");
			//	TreeNode newNode = new TreeNode(newTileName + ".png") { Tag = mf };//creates node for minefile
			//	newNode.ImageIndex = 2;
			//	newNode.SelectedImageIndex = 2;
			//	addNodeToAnimationsFolder(newNode);
			//	treeViewMain.SelectedNode = newNode;
			//	create = false;
			//}
			//else if (isItem && treeViewMain.SelectedNode.Parent.Text == "blocks")
			//{
			//	Console.WriteLine("block: " + treeViewMain.SelectedNode.Parent.Text);
			//	TreeNode newNode = treeViewMain.SelectedNode;
			//	newNode.ImageIndex = 2;
			//	newNode.SelectedImageIndex = 2;
			//	treeViewMain.SelectedNode.Remove();
			//	addNodeToAnimationsFolder(newNode);
			//}
			//else if (treeViewMain.SelectedNode.Parent.Text == "items")
			//{
			//	Console.WriteLine("item: " + treeViewMain.SelectedNode.Parent.Text);
			//	TreeNode newNode = treeViewMain.SelectedNode;
			//	newNode.ImageIndex = 2;
			//	newNode.SelectedImageIndex = 2;
			//	treeViewMain.SelectedNode.Remove();
			//	addNodeToAnimationsFolder(newNode);
			//}

			if(MipMapCheckbox.Checked) TileName = TileName.Substring(0, TileName.Length - 12);
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
			FrameEditor diag = new FrameEditor(
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
			FrameEditor diag = new FrameEditor(
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
			SetBulkSpeed diag = new SetBulkSpeed(treeView1);
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
			CreateFrameList(texture);

			try
			{
				JObject mcmeta = JObject.Parse(File.ReadAllText(diag.FileName));

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
									var finalFrameData = new Tuple<int, int>(((int)frame["index"]), ((int)frame["time"]));
									frameNode.Text = "Frame: " + ((int)frame["index"]).ToString() + ", Frame Time: " + ((int)frame["time"]).ToString();
									frameNode.Tag = finalFrameData;
									treeView1.Nodes.Add(frameNode);
								}
							}
							else if (frame.Type == JTokenType.Integer)
							{
								Console.WriteLine((int)frame + "*" + frameTime);

								TreeNode frameNode = new TreeNode();
								var finalFrameData = new Tuple<int, int>(((int)frame), frameTime);
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
							var finalFrameData = new Tuple<int, int>(i, frameTime);
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
				frames = oldFrames;
				foreach (TreeNode node in oldAnimData)
				{
					treeView1.Nodes.Add(node);
				}
				return;
			}
			pictureBoxWithInterpolationMode1.Image = frames[(treeView1.Nodes[0].Tag as Tuple<int, int>).Item1];
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
			using (ChangeTile diag = new ChangeTile(TileName))
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					Console.WriteLine(diag.SelectedTile);
					if (TileName != diag.SelectedTile) isItem = diag.IsItem;
					TileName = diag.SelectedTile;
					foreach (JObject content in tileData[isItem ? "Items" : "Blocks"].Children())
					{
						foreach (JProperty prop in content.Properties())
						{
							if (prop.Name == TileName) tileLabel.Text = (string)prop.Value;
						}
					}
				}
		}

		private void metroCheckBox2_CheckedChanged(object sender, EventArgs e)
		{
			MipMapNumericUpDown.Visible = metroLabel1.Visible = MipMapCheckbox.Checked;
		}
	}
}
