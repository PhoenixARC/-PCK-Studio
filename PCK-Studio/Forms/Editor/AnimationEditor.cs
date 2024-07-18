/* Copyright (c) 2023-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using MetroFramework.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PckStudio.Forms.Additional_Popups.Animation;
using PckStudio.Extensions;
using PckStudio.Properties;
using PckStudio.Internal;
using PckStudio.Internal.Deserializer;

namespace PckStudio.Forms.Editor
{
    public partial class AnimationEditor : MetroForm
	{
        public Animation Result => _animation;

		private Animation _animation;
		private bool _isSpecialTile;

		private AnimationEditor()
		{
            InitializeComponent();
            toolStripSeparator1.Visible = saveToolStripMenuItem1.Visible = !Settings.Default.AutoSaveChanges;
        }

        internal AnimationEditor(Animation animation, string displayName, bool isSpecialTile = false)
			: this()
		{
			_ = animation ?? throw new ArgumentNullException(nameof(animation));
			_animation = animation;
            tileLabel.Text = displayName;
            _isSpecialTile = isSpecialTile;
            animationPictureBox.Image = animation.CreateAnimationImage();
        }

        internal AnimationEditor(Animation animation, string displayName, Color blendColor)
			: this(animation, displayName)
        {
			animationPictureBox.UseBlendColor = true;
			animationPictureBox.BlendColor = blendColor;
        }

		private void ValidateToolStrip()
        {
			bulkAnimationSpeedToolStripMenuItem.Enabled =
			importToolStripMenuItem.Enabled =
			exportAsToolStripMenuItem.Enabled =
			InterpolationCheckbox.Visible = !_isSpecialTile;
		}

        private void AnimationEditor_Load(object sender, EventArgs e)
        {
			ValidateToolStrip();
            LoadAnimationTreeView();
        }

        private void LoadAnimationTreeView()
        {
            if (_animation is null)
            {
                AnimationStartStopBtn.Enabled = false;
                return;
            }
            AnimationStartStopBtn.Enabled = true;
            InterpolationCheckbox.Checked = _animation.Interpolate;
            TextureIcons.Images.Clear();
            TextureIcons.Images.AddRange(_animation.GetTextures().ToArray());
            UpdateTreeView();

			animationPictureBox.Image ??= _animation.CreateAnimationImage();

            if (_animation.FrameCount > 0)
            {
				animationPictureBox.Image.SelectActiveFrame(FrameDimension.Page, 0);
            }
        }

        private void UpdateTreeView()
        {
            frameTreeView.Nodes.Clear();
            frameTreeView.Nodes.AddRange(
                _animation.GetFrames()
                .Select(frame =>
                {
                    var imageIndex = _animation.GetTextureIndex(frame.Texture);
                    return new TreeNode($"for {frame.Ticks} ticks", imageIndex, imageIndex);
                })
                .ToArray()
                );
		}

        private void frameTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (animationPictureBox.IsPlaying)
			{
				StopAnimation();
			}
            animationPictureBox.Image = _animation.GetFrame(frameTreeView.SelectedNode.Index).Texture;
		}

		private void StopAnimation()
        {
			animationPictureBox.Stop();
			AnimationStartStopBtn.Text = "Play Animation";
		}

		private void AnimationStartStopBtn_Click(object sender, EventArgs e)
		{
			if (animationPictureBox.IsPlaying)
			{
				StopAnimation();
				return;
			}

            if (_animation.FrameCount > 1)
			{
				animationPictureBox.Image = _animation.CreateAnimationImage();
                animationPictureBox.Start();
				AnimationStartStopBtn.Text = "Stop Animation";
			}
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
				if (node.Text.ToLower() == name.ToLower())
					return node;
				return FindNodeByName(node, name);
			}
			return null;
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (!_isSpecialTile && _animation is not null && _animation.FrameCount > 0)
			{
				DialogResult = DialogResult.OK;
				return;
			}
			DialogResult = DialogResult.Cancel;
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
					_animation.SwapFrames(draggedIndex, targetIndex);
					UpdateTreeView();
				}
			}
		}

		// Determine whether one node is a parent 
		// or ancestor of a second node.
		private bool ContainsNode(TreeNode node1, TreeNode node2)
		{
			// Check the parent node of the second node.
			if (node2.Parent == null)
				return false;
			if (node2.Parent.Equals(node1))
				return true;

			// If the parent node is not null or equal to the first node, 
			// call the ContainsNode method recursively using the parent of 
			// the second node.
			return ContainsNode(node1, node2.Parent);
		}

		private void treeView1_doubleClick(object sender, EventArgs e)
		{
            Animation.Frame frame = _animation.GetFrame(frameTreeView.SelectedNode.Index);
            using FrameEditor diag = new FrameEditor(frame.Ticks, _animation.GetTextureIndex(frame.Texture), TextureIcons);
			if (diag.ShowDialog(this) == DialogResult.OK)
            {
				/* Found a bug here. When passing the frame variable,
				 * it would replace the first instance of that frame and time
				 * rather than the actual frame that was clicked.
				 * I've just switched to passing the index to fix this for now.
				 * - Matt
				*/

                _animation.SetFrame(frameTreeView.SelectedNode.Index, diag.FrameTextureIndex, diag.FrameTime);
                UpdateTreeView();
            }
        }

		private void addFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using FrameEditor diag = new FrameEditor(TextureIcons);
			diag.SaveBtn.Text = "Add";
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
                _animation.AddFrame(diag.FrameTextureIndex, _isSpecialTile ? Animation.MinimumFrameTime : diag.FrameTime);
                UpdateTreeView();
			}
		}

		private void removeFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (frameTreeView.SelectedNode is TreeNode t && _animation.RemoveFrame(t.Index))
			{
				frameTreeView.SelectedNode.Remove();
			}
		}

		private void bulkAnimationSpeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetBulkSpeed diag = new SetBulkSpeed();
			if (diag.ShowDialog(this) == DialogResult.OK)
			{
				if (animationPictureBox.IsPlaying)
					animationPictureBox.Stop();
                _animation.SetFrameTicks(diag.Ticks);
				UpdateTreeView();
			}
			diag.Dispose();
		}

		private void importJavaAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
				this,
				"This feature will replace the existing animation data. " +
				"It might fail if the selected animation script is invalid. " +
				"Are you sure that you want to continue?",
				"Warning",
				MessageBoxButtons.YesNo) == DialogResult.No)
				return;

            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Please select a valid Minecaft: Java Edition animation script",
                // It's marked as .png.mcmeta just in case
                // some weirdo tries to pass a pack.mcmeta or something
                // -MattNL
                Filter = "Animation Scripts (*.mcmeta)|*.png.mcmeta"
            };
            if (fileDialog.ShowDialog(this) != DialogResult.OK)
				return;
			Debug.WriteLine("Selected Animation Script: " + fileDialog.FileName);

			string textureFile = fileDialog.FileName.Substring(0, fileDialog.FileName.Length - ".mcmeta".Length);
            if (!File.Exists(textureFile))
			{
				MessageBox.Show(this, textureFile + " was not found", "Texture not found");
				return;
			}
            try
			{
				var img = Image.FromFile(textureFile).ReleaseFromFile();
				JObject mcmeta = JObject.Parse(File.ReadAllText(fileDialog.FileName));
                Animation javaAnimation = AnimationDeserializer.DefaultDeserializer.DeserializeJavaAnimation(mcmeta, img);
				//javaAnimation.Category = _animation.Category;
				_animation = javaAnimation;
				LoadAnimationTreeView();
			}
			catch (JsonException j_ex)
			{
				MessageBox.Show(this, j_ex.Message, "Invalid animation");
				return;
			}
		}

		private void exportJavaAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Title = "Please choose where you want to save your new animation";
			fileDialog.Filter = "Animation Scripts (*.mcmeta)|*.png.mcmeta";
			if (fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                JObject mcmeta = _animation.ConvertToJavaAnimation();
                string jsondata = JsonConvert.SerializeObject(mcmeta, Formatting.Indented);
                string filename = fileDialog.FileName;
                File.WriteAllText(filename, jsondata);
                Image finalTexture = _animation.BuildTexture();
				// removes ".mcmeta" from filename
				string texturePath = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
                finalTexture.Save(texturePath);
                MessageBox.Show(this, "Animation was successfully exported as " + Path.GetFileName(filename), "Export successful!");
            }
        }

        private void howToInterpolation_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "The Interpolation effect is when the animtion smoothly translates between the frames instead of simply displaying the next one. This can be seen with some vanilla Minecraft textures such as Magma and Prismarine.", "Interpolation");
		}

		private void editorControlsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Simply drag and drop frames in the tree to rearrange your animation.\n\n" +
				"You can also preview your animation at any time by simply pressing the button under the animation display.", "Editor Controls");
		}

		private void setBulkSpeedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "You can edit the frame and its speed by double clicking a frame in the tree. If you'd like to change the entire animation's speed, you can do so with the \"Set Bulk Animation Speed\" button in the \"Tools\" tab", "How to use Bulk Animation tool");
		}

		private void javaAnimationSupportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "You can import any valid Java Edition tile animations into your pck by opening an mcmeta.\n\n" +
				"You can also export your animation as an Java Edition tile animation. It will also export the actual texture in the same spot.", "Java Edition Support");
		}

		private void InterpolationCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (_animation is not null)
				_animation.Interpolate = InterpolationCheckbox.Checked;
		}

        private void AnimationEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (animationPictureBox.IsPlaying)
			{
                animationPictureBox.Stop();
			}
			if (Settings.Default.AutoSaveChanges)
			{
				saveToolStripMenuItem1_Click(sender, EventArgs.Empty);
			}
        }

        private void importGifToolStripMenuItem_Click(object sender, EventArgs e)
        {
			OpenFileDialog fileDialog = new OpenFileDialog()
			{
				Filter = "GIF file|*.gif"
			};
			if (fileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			var gif = Image.FromFile(fileDialog.FileName).ReleaseFromFile();
			if (!gif.RawFormat.Equals(ImageFormat.Gif))
			{
				MessageBox.Show(this, "Selected file is not a gif", "Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var oldResolution = _animation.BuildTexture().Width;

            FrameDimension dimension = new FrameDimension(gif.FrameDimensionsList[0]);
            int frameCount = gif.GetFrameCount(dimension);

			var textures = new List<Image>(frameCount);

			for (int i = 0; i < frameCount; i++)
			{
				gif.SelectActiveFrame(dimension, i);

				textures.Add(new Bitmap(gif, oldResolution, oldResolution));
			}

            // TODO: Add function or a other way to initialize the frames by textures.
            // Currently single frames only get added when an anim has an invalid format or is empty.
            // -Miku
            _animation = new Animation(textures, "");
            _animation.Interpolate = InterpolationCheckbox.Checked;
			LoadAnimationTreeView();
        }

        private void importAnimationTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog()
            {
                Filter = "PNG Files | *.png",
                Title = "Select a PNG File",
            };
            if (ofd.ShowDialog(this) != DialogResult.OK)
                return;
            Image img = Image.FromFile(ofd.FileName).ReleaseFromFile();
            var textures = img.Split(ImageLayoutDirection.Vertical);
			_animation = new Animation(textures, string.Empty);
			LoadAnimationTreeView();
        }

		private void gifToolStripMenuItem_Click(object sender, EventArgs e)
        {
			var fileDialog = new SaveFileDialog()
			{
				FileName = tileLabel.Text,
				Filter = "GIF file|*.gif"
			};
			if (fileDialog.ShowDialog(this) != DialogResult.OK)
				return;
			_animation.CreateAnimationImage().Save(fileDialog.FileName);
		}

		private void frameTimeandTicksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "The frame time is the time that the current frame is displayed for. This unit is measured in ticks. " +
				"All time related functions in Minecraft use ticks, notably redstone repeaters. There are 20 ticks in 1 second, so " +
				"1 tick is 1/20 of a second. To find how long your frame is, divide the frame time by 20", "Frame Time and Ticks");
		}
    }
}
