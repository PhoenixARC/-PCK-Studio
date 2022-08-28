using MetroFramework.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.Classes.FileTypes;
using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Properties;
using PckStudio.Forms.Utilities;

namespace PckStudio.Forms.Editor
{
	public partial class AnimationEditor : MetroForm
	{
		PCKFile.FileData animationFile;
		Animation currentAnimation;
        AnimationPlayer player;

		bool isItem = false;
        string animationSection => AnimationUtil.GetAnimationSection(isItem);

		public string TileName = string.Empty;

        sealed class Animation
		{
			public const int MinimumFrameTime = 1;

			private readonly List<Image> frameTextures;

			private readonly List<Frame> frames = new List<Frame>();

			public Frame this[int frameIndex] => frames[frameIndex];

			// not implemented rn...
			public bool Interpolate { get; set; } = false;
			
			public Animation(Image image)
			{
                frameTextures = new List<Image>(SplitImageToFrameTextures(image));
            }

			public Animation(Image image, string ANIM) : this(image)
			{
				ParseAnim(ANIM);
            }

            public struct Frame
			{
				public readonly Image Texture;
				public int Ticks;

				public static implicit operator Image(Frame f) => f.Texture;

				public Frame(Image texture) : this(texture, MinimumFrameTime)
				{}

				public Frame(Image texture, int frameTime)
				{
					Texture = texture;
					Ticks = frameTime;
				}
			}

			public void ParseAnim(string ANIM)
			{
				_ = ANIM ?? throw new ArgumentNullException(nameof(ANIM));
				ANIM = (Interpolate = ANIM.StartsWith("#")) ? ANIM.Substring(1) : ANIM;
                string[] animData = ANIM.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int lastFrameTime = MinimumFrameTime;
				if (animData.Length <= 0)
				{
					for(int i = 0; i < FrameTextureCount; i++)
					{
						AddFrame(i, 1);
					}
				}
				else
				{
					foreach (string frameInfo in animData)
					{
						string[] frameData = frameInfo.Split('*');
						//if (frameData.Length < 2)
						//    continue; // shouldn't happen
						int currentFrameIndex = int.TryParse(frameData[0], out currentFrameIndex) ? currentFrameIndex : 0;

						// Some textures like the Halloween 2015's Lava texture don't have a
						// frame time parameter for certain frames.
						// This will detect that and place the last frame time in its place.
						// This is accurate to console edition behavior.
						// - MattNL
						int currentFrameTime = string.IsNullOrEmpty(frameData[1]) ? lastFrameTime : int.Parse(frameData[1]);
						AddFrame(currentFrameIndex, currentFrameTime);
						lastFrameTime = currentFrameTime;
					}
				}
            }
			public Frame AddFrame(int frameTextureIndex) => AddFrame(frameTextureIndex, MinimumFrameTime);
			public Frame AddFrame(int frameTextureIndex, int frameTime)
			{
				if (frameTextureIndex < 0 || frameTextureIndex >= frameTextures.Count)
					throw new ArgumentOutOfRangeException(nameof(frameTextureIndex));
				Frame f = new Frame(frameTextures[frameTextureIndex], frameTime);
                frames.Add(f);
				return f;
			}

			public bool RemoveFrame(int frameIndex)
			{
				frames.RemoveAt(frameIndex);
				return true;
            }

            private static IEnumerable<Image> SplitImageToFrameTextures(Image source)
            {
                for (int i = 0; i < source.Height / source.Width; i++)
                {
                    Rectangle tileArea = new Rectangle(0, i * source.Width, source.Width, source.Width);
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

			public Frame GetFrame(int index) => frames[index];

			public List<Frame> GetFrames()
			{
				return frames;
			}

			public int GetFrameIndex(Image frameTexture)
			{
				_ = frameTexture ?? throw new ArgumentNullException(nameof(frameTexture));
				return frameTextures.IndexOf(frameTexture);
			}

			public int FrameCount => frames.Count;
			public int FrameTextureCount => frameTextures.Count;

			public void SetFrame(Frame frame, int frameTextureIndex, int frameTime = MinimumFrameTime)
				=> SetFrame(frames.IndexOf(frame), frameTextureIndex, frameTime);
			public void SetFrame(int frameIndex, int frameTextureIndex, int frameTime = MinimumFrameTime)
			{
				frames[frameIndex] = new Frame(frameTextures[frameTextureIndex], frameTime);
			}

			public string BuildAnim()
			{
                string animationData = Interpolate ? "#" : string.Empty;
				frames.ForEach(frame => animationData += $"{GetFrameIndex(frame)}*{frame.Ticks},");
				return animationData.TrimEnd(',');
            }

			public Image BuildTexture()
			{
				int width = frameTextures[0].Width;
				int height = frameTextures[0].Height;
				if (width != height) throw new Exception("Invalid size");
                var img = new Bitmap(width, height * FrameTextureCount);
				int pos_y = 0;
				using (var g = Graphics.FromImage(img))
				frameTextures.ForEach(texture =>
				{
					g.DrawImage(texture, 0, pos_y);
					pos_y += height;
				});
				return img;
			}
		}

        sealed class AnimationPlayer
        {
            public const int BaseTickSpeed = 48;
            public bool IsPlaying { get; private set; } = false;

            private int currentAnimationFrameIndex = 0;
            private PictureBox display;
            private Animation _animation;
            private CancellationTokenSource cts = new CancellationTokenSource();

            public AnimationPlayer(PictureBox display)
            {
                SetContext(display);
            }

            private async void DoAnimate()
            {
                _ = display ?? throw new ArgumentNullException(nameof(display));
                _ = _animation ?? throw new ArgumentNullException(nameof(_animation));
				IsPlaying = true;
				while (!cts.IsCancellationRequested)
				{
					if (currentAnimationFrameIndex >= _animation.FrameCount)
						currentAnimationFrameIndex = 0;
					Animation.Frame frame = SetFrameDisplayed(currentAnimationFrameIndex++);
					await Task.Delay(BaseTickSpeed * frame.Ticks);
				}
                IsPlaying = false;
            }

			public void Start(Animation animation)
			{
				_animation = animation;
				cts = new CancellationTokenSource();
				Task.Run(DoAnimate, cts.Token);
			}

            public void Stop() => cts.Cancel();

            public Animation.Frame GetCurrentFrame() => _animation[currentAnimationFrameIndex];

            public void SetContext(PictureBox display) => this.display = display;

			public void SelectFrame(Animation animation, int index)
			{
				_animation = animation;
                if (IsPlaying) Stop();
                SetFrameDisplayed(index);
                currentAnimationFrameIndex = index;
            }

			private Animation.Frame SetFrameDisplayed(int i)
			{
				Monitor.Enter(_animation);
				Animation.Frame frame = _animation[i];
				display.Image = frame;
				Monitor.Exit(_animation);
				return frame;
            }
		}

		public AnimationEditor(PCKFile.FileData file)
		{
			InitializeComponent();
			isItem = file.filepath.Split('/').Contains("items");
			TileName = Path.GetFileNameWithoutExtension(file.filepath);
			animationFile = file;

			// sanity check
			if (TileName.EndsWith("MipMapLevel2") || TileName.EndsWith("MipMapLevel3"))
			{
				string mipMapLvl = TileName.Last().ToString();
				TileName = TileName.Substring(0, TileName.Length - 12);
				MipMapCheckbox.Checked = true;
				MipMapNumericUpDown.Value = short.Parse(mipMapLvl);
			}

			using MemoryStream textureMem = new MemoryStream(animationFile.data);
			var texture = new Bitmap(textureMem);
			currentAnimation = animationFile.properties.HasProperty("ANIM")
				? new Animation(texture, animationFile.properties.GetProperty("ANIM").Item2)
				: new Animation(texture);
			player = new AnimationPlayer(pictureBoxWithInterpolationMode1);

			foreach (JObject content in AnimationUtil.tileData[animationSection].Children())
			{
				var prop = content.Properties().FirstOrDefault(prop => prop.Name == TileName);
				if (prop is JProperty)
				{
					tileLabel.Text = (string)prop.Value;
					break;
				}
			}
            LoadAnimationTreeView();
		}
		
		private void LoadAnimationTreeView()
		{
			InterpolationCheckbox.Checked = currentAnimation.Interpolate;
			frameTreeView.Nodes.Clear();
			// $"Frame: {i}, Frame Time: {Animation.MinimumFrameTime}"
            currentAnimation.GetFrames().ForEach(f => frameTreeView.Nodes.Add($"Frame: {currentAnimation.GetFrameIndex(f.Texture)}, Frame Time: {f.Ticks}"));
			player.SelectFrame(currentAnimation, 0);
		}

		private void frameTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (player.IsPlaying && !AnimationPlayBtn.Enabled)
                AnimationPlayBtn.Enabled = !(AnimationStopBtn.Enabled = !AnimationStopBtn.Enabled);
			player.SelectFrame(currentAnimation, frameTreeView.SelectedNode.Index);
		}

		private int mix(double ratio, int val1, int val2) // Ported from Java Edition code
		{
			return (int)(ratio * val1 + (1.0D - ratio) * val2);
		}

		private void StartAnimationBtn_Click(object sender, EventArgs e)
		{
			AnimationPlayBtn.Enabled = !(AnimationStopBtn.Enabled = !AnimationStopBtn.Enabled);
			if (currentAnimation.FrameCount > 1)
			{
				player.SetContext(pictureBoxWithInterpolationMode1);
				player.Start(currentAnimation);
			}
		}

		private void StopAnimationBtn_Click(object sender, EventArgs e)
		{
            AnimationPlayBtn.Enabled = !(AnimationStopBtn.Enabled = !AnimationStopBtn.Enabled);
            player.Stop();
		}

		private void frameTreeView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
				removeFrameToolStripMenuItem_Click(sender, e);
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

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{

			string anim = currentAnimation.BuildAnim();
			animationFile.properties.SetProperty("ANIM", anim);
            using (var stream = new MemoryStream())
			{
				currentAnimation.BuildTexture().Save(stream, ImageFormat.Png);
				animationFile.SetData(stream.ToArray());
			}
			//Reusing this for the tile path
			TileName = "res/textures/" + (isItem ? "items/" : "blocks/") + TileName + ".png" ;
			DialogResult = DialogResult.OK;
		}

		// Most of the code below is modified code from this link: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.itemdrag?view=windowsdesktop-6.0
		// - MattNL

		private void frameTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// Move the dragged node when the left mouse button is used.
			if (e.Button == MouseButtons.Left)
			{
				DoDragDrop(e.Item, DragDropEffects.Move);
			}
		}

		// Set the target drop effect to the effect 
		// specified in the ItemDrag event handler.
		private void frameTreeView_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		// Select the node under the mouse pointer to indicate the 
		// expected drop location.
		private void frameTreeView_DragOver(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the mouse position.
			Point targetPoint = frameTreeView.PointToClient(new Point(e.X, e.Y));

			// Select the node at the mouse position.
			frameTreeView.SelectedNode = frameTreeView.GetNodeAt(targetPoint);
		}

		private void frameTreeView_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the drop location.
			Point targetPoint = frameTreeView.PointToClient(new Point(e.X, e.Y));

			// Retrieve the node at the drop location.
			TreeNode targetNode = frameTreeView.GetNodeAt(targetPoint);

			// Retrieve the node that was dragged.
			TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

			// Confirm that the node at the drop location is not 
			// the dragged node or a descendant of the dragged node.
			if (targetNode == null)
			{
				draggedNode.Remove();
				frameTreeView.Nodes.Add(draggedNode);
			}
			else if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
			{
				// If it is a move operation, remove the node from its current 
				// location and add it to the node at the drop location.

				if (e.Effect == DragDropEffects.Move)
				{
					int draggedIndex = draggedNode.Index;
					int targetIndex = targetNode.Index;
					currentAnimation.GetFrames().Swap(draggedIndex, targetIndex);
					LoadAnimationTreeView();
				}
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
            var frame = currentAnimation.GetFrame(frameTreeView.SelectedNode.Index);
            using FrameEditor diag = new FrameEditor(frame.Ticks, currentAnimation.GetFrameIndex(frame.Texture), currentAnimation.FrameTextureCount-1);
            if (diag.ShowDialog(this) == DialogResult.OK)
            {
                currentAnimation.SetFrame(frame, diag.FrameTextureIndex, diag.FrameTime);
                LoadAnimationTreeView();
            }
        }

		private void addFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using FrameEditor diag = new FrameEditor(currentAnimation.FrameTextureCount-1);
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
                currentAnimation.AddFrame(diag.FrameTextureIndex, diag.FrameTime);
                LoadAnimationTreeView();
			}
		}

		private void removeFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (frameTreeView.SelectedNode is TreeNode t &&
				currentAnimation.RemoveFrame(t.Index))
				frameTreeView.SelectedNode.Remove();

		}

		private void bulkAnimationSpeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetBulkSpeed diag = new SetBulkSpeed(frameTreeView);
			if(diag.ShowDialog(this) == DialogResult.OK)
			{
				var list = currentAnimation.GetFrames();
				for (int i = 0; i < list.Count; i++)
				{
					Animation.Frame f = list[i];
					currentAnimation.SetFrame(f, currentAnimation.GetFrameIndex(f), diag.time);
				}
				LoadAnimationTreeView();
			}
			diag.Dispose();
		}

		// Reworked import tool with new Animation classes by Miku
		private void importJavaAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult query = MessageBox.Show("This feature will replace the existing animation data. It might fail if the selected animation script is invalid. Are you sure that you want to continue?", "Warning", MessageBoxButtons.YesNo);
			if (query == DialogResult.No) return;

			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = false;
			fileDialog.Title = "Please select a valid Minecaft: Java Edition animation script";

			// It's marked as .png.mcmeta just in case
			// some weirdo tries to pass a pack.mcmeta or something
			// -MattNL
			fileDialog.Filter = "Animation Scripts (*.mcmeta)|*.png.mcmeta";
			fileDialog.CheckPathExists = true;
			fileDialog.CheckFileExists = true;
			if (fileDialog.ShowDialog(this) != DialogResult.OK) return;
			Console.WriteLine("Selected Animation Script: " + fileDialog.FileName);

			string textureFile = fileDialog.FileName.Substring(0, fileDialog.FileName.Length - ".mcmeta".Length);
            if (!File.Exists(textureFile))
			{
				MessageBox.Show(textureFile + " was not found", "Texture not found");
				return;
			}
			using MemoryStream textureMem = new MemoryStream(File.ReadAllBytes(textureFile));
			var new_animation = new Animation(Image.FromStream(textureMem));

			try
			{
				JObject mcmeta = JObject.Parse(File.ReadAllText(fileDialog.FileName));
				if (mcmeta["animation"] is JToken animation)
				{
					int frameTime = Animation.MinimumFrameTime;
					// Some if statements to ensure that the animation is valid.
					if (animation["frametime"] is JToken frametime_token && frametime_token.Type == JTokenType.Integer)
						frameTime = (int)frametime_token;
					if (animation["interpolate"] is JToken interpolate_token && interpolate_token.Type == JTokenType.Boolean)
						new_animation.Interpolate = (bool)interpolate_token;
					if (animation["frames"] is JToken frames_token &&
						frames_token.Type == JTokenType.Array)
					{
						foreach (JToken frame in frames_token.Children())
						{
							if (frame.Type == JTokenType.Object)
							{
								if (frame["index"] is JToken frame_index && frame_index.Type == JTokenType.Integer &&
									frame["time"] is JToken frame_time && frame_time.Type == JTokenType.Integer)
								{
									Console.WriteLine("{0}*{1}", (int)frame["index"], (int)frame["time"]);
									new_animation.AddFrame((int)frame["index"], (int)frame["time"]);
								}
							}
							else if (frame.Type == JTokenType.Integer)
							{
								Console.WriteLine("{0}*{1}", (int)frame, frameTime);
								new_animation.AddFrame((int)frame, frameTime);
							}
						}
					}
					else
					{
						for (int i = 0; i < new_animation.FrameTextureCount; i++)
						{
							new_animation.AddFrame(i, frameTime);
						}
					}
				}

				currentAnimation = new_animation;
				LoadAnimationTreeView();
			}
			catch (JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Invalid animation");
				return;
			}
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
			using (ChangeTile diag = new ChangeTile())
				if (diag.ShowDialog(this) == DialogResult.OK)
				{
					Console.WriteLine(diag.SelectedTile);
					if (TileName != diag.SelectedTile) isItem = diag.IsItem;
					TileName = diag.SelectedTile;
					foreach (JObject content in AnimationUtil.tileData[animationSection].Children())
					{
						var first = content.Properties().FirstOrDefault(p => p.Name == TileName);
						if (first is JProperty p) tileLabel.Text = (string)p.Value;
                    }
				}
		}

		private void MipMapCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			MipMapNumericUpDown.Visible = MipMapLabel.Visible = MipMapCheckbox.Checked;
		}

		private void exportJavaAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Title = "Please choose where you want to save your new animation";

			fileDialog.Filter = "Animation Scripts (*.mcmeta)|*.png.mcmeta";
			fileDialog.CheckPathExists = true;
			if (fileDialog.ShowDialog(this) != DialogResult.OK) return;

			JObject mcmeta = new JObject();
			JObject animation = new JObject();
			JArray frames = new JArray();
			currentAnimation.GetFrames().ForEach(f => {
				JObject frame = new JObject();
				frame["index"] = currentAnimation.GetFrameIndex(f.Texture);
				frame["time"] = f.Ticks;
				frames.Add(frame);
			});
			animation["interpolation"] = InterpolationCheckbox.Checked;
			animation["frames"] = frames;
			mcmeta["comment"] = "Animation converted via PCK Studio";
			mcmeta["animation"] = animation;
			File.WriteAllText(fileDialog.FileName, JsonConvert.SerializeObject(mcmeta, Formatting.Indented));
			string fn = fileDialog.FileName;
			currentAnimation.BuildTexture().Save(fn.Remove(fn.Length - 7));
			MessageBox.Show("Your animation was successfully exported at " + fn, "Successful export");
		}
	}
}
