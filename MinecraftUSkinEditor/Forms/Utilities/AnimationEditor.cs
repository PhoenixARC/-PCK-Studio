using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio;

namespace PckStudio
{
	public partial class AnimationEditor : MetroForm
	{
		PCK.MineFile mf = new PCK.MineFile();
		List<Image> frames = new List<Image>();
		Image texture;
		int frameCount;
		string lastFrameTime = "1";

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Tuple< string, string > frameData = e.Node.Tag as Tuple<string, string>;
			Console.WriteLine(frameData.Item1 + " --- " + frameData.Item2);
			if (metroButton1.Enabled)
			{
				pictureBoxWithInterpolationMode1.Image = frames[Int16.Parse(frameData.Item1)];
			}
		}

		public AnimationEditor(PCK.MineFile MineFile)
		{
			mf = MineFile;
			List<string> strEntries = new List<string>();
			List<string> strEntryData = new List<string>();

			foreach (object[] entry in mf.entries) //object = metadata entry(name:value)
			{
				object[] strings = (object[])entry;
				TreeNode meta = new TreeNode();

				foreach (object[] entryy in mf.entries)
					strEntries.Add((string)strings[0]);
				strEntryData.Add((string)strings[1]);
			}

			//if (strEntries.Find(entry => entry == "ANIM") == null) throw new System.Exception("ANIM tag is missing. No animation code is present.");

			InitializeComponent();
			MemoryStream textureMem = new MemoryStream(mf.data);
			texture = Image.FromStream(textureMem);
			createFrameList();

			string anim = "";
			if (strEntries.Find(entry => entry == "ANIM") == null) anim = "";
			else anim = strEntryData[strEntries.FindIndex(entry => entry == "ANIM")];
			Console.WriteLine("ANIMATION DATA: " + anim);
			if (anim.StartsWith("#"))
			{
				Console.WriteLine("Interpolate: true");
				metroCheckBox1.Checked = true;
				anim = anim.Remove(0, 1);
			}
			else
			{
				Console.WriteLine("Interpolate: false");
				metroCheckBox1.Checked = false;
			}

			frameCount = texture.Height / texture.Width;

			if (!String.IsNullOrEmpty(anim))
			{
				string[] animData = anim.Split(new char[] { ',' });
				if (String.IsNullOrEmpty(animData.Last())) animData = animData.Take(animData.Length - 1).ToArray();
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
							if (String.IsNullOrEmpty(data)) throw new System.Exception("Invalid animation data");
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
							if (String.IsNullOrEmpty(data)) outData = lastFrameTime;
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
				for(int i = 0; i < frameCount; i++)
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
		int animCurrentFrameTime = 0;
		int animCurrentTotalFrameTime = -1;
		Tuple<string, string> currentFrameData = new Tuple<string, string>("","");
		Image img = null;
		int nextFrame;
		int frameCounter = 0; // ported directly from Java Edition code -MattNL
		Image imgB = null;
		void animate(object sender, EventArgs e)
		{
			if (animCurrentFrameTime > animCurrentTotalFrameTime)
			{
				Console.WriteLine(frameCounter + " $$$ " + frameCount);
				frameCounter = (frameCounter + 1) % frameCount;
				animCurrentTotalFrameTime = 0;
				animCurrentFrameTime = 0;
				if (animCurrentFrame > (treeView1.Nodes.Count - 1)) animCurrentFrame = 0;
				currentFrameData = treeView1.Nodes[animCurrentFrame].Tag as Tuple<string, string>;
				pictureBoxWithInterpolationMode1.Image = frames[Int16.Parse(currentFrameData.Item1)];
				animCurrentTotalFrameTime = Int16.Parse(currentFrameData.Item2);
				animCurrentFrame++;

				if(metroCheckBox1.Checked)
				{
					img = frames[Int16.Parse(currentFrameData.Item1)];
					nextFrame = animCurrentFrame + 1;
					if (nextFrame > frameCount - 1) nextFrame = 0;
					Console.WriteLine(nextFrame);
					imgB = frames[nextFrame];
				}
			}

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
			Console.WriteLine(animCurrentFrame + " - " + animCurrentFrameTime + " - " + animCurrentTotalFrameTime + " - " + (treeView1.Nodes.Count - 1));
			animCurrentFrameTime++;
		}

		private void metroButton1_Click(object sender, EventArgs e)
		{
			animCurrentFrame = 0;
			animCurrentFrameTime = 0;
			animCurrentTotalFrameTime = -1;
			frameCounter = 0;
			metroButton1.Enabled = false;
			metroButton2.Enabled = true;
			timer1.Start();
		}

		private void metroButton2_Click(object sender, EventArgs e)
		{
			metroButton1.Enabled = true;
			metroButton2.Enabled = false;
			timer1.Stop();
		}

		private void treeView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete) treeView1.Nodes.Remove(treeView1.SelectedNode);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			int animIndex = mf.entries.FindIndex(entry => (string)entry[0] == "ANIM");
			string animationData = "";
			if(metroCheckBox1.Checked) animationData += "#"; // does the animation interpolate?
			foreach (TreeNode node in treeView1.Nodes)
			{
				Tuple<string, string> frameData = node.Tag as Tuple<string, string>;
				animationData += frameData.Item1 + "*" + frameData.Item2 + ",";
			}
			animationData.TrimEnd(',');
			object[] newEntry = new object[]
			{
				"ANIM",
				animationData
			};
			if (animIndex != -1) mf.entries[animIndex] = newEntry;
			else mf.entries.Add(newEntry);
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

		private void addFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.AnimationEditor.FrameEditor diag = new PckStudio.Forms.Utilities.AnimationEditor.FrameEditor(
				treeView1,
				new Tuple<string, string>("",""),
				frameCount - 1,
				true,
				new TreeNode());
			diag.ShowDialog(this);
			diag.Dispose();
		}

		private void treeView1_doubleClick(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.AnimationEditor.FrameEditor diag = new PckStudio.Forms.Utilities.AnimationEditor.FrameEditor(
				treeView1,
				treeView1.SelectedNode.Tag as Tuple<string, string>,
				frameCount - 1,
				false,
				treeView1.SelectedNode
				);
			diag.ShowDialog(this);
			diag.Dispose();
		}

		private void removeFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeView1.SelectedNode.Remove();
		}

		private void metroCheckBox1_CheckedChanged(object sender, EventArgs e) {}

		private void bulkAnimationSpeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PckStudio.Forms.Utilities.AnimationEditor.SetBulkSpeed diag = new PckStudio.Forms.Utilities.AnimationEditor.SetBulkSpeed(treeView1);
			diag.ShowDialog(this);
			diag.Dispose();
		}
	}
}
