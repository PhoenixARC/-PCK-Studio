using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Extensions;
using PckStudio.Internal;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	internal partial class ChangeTile : MetroForm
	{
        string selectedTile = "";
        Internal.AnimationCategory category = Internal.AnimationCategory.Blocks;

		public string SelectedTile => selectedTile;
		public Internal.AnimationCategory Category => category;

        List<TreeNode> treeViewBlockCache = new List<TreeNode>();
		List<TreeNode> treeViewItemCache = new List<TreeNode>();

		public ChangeTile()
		{
			InitializeComponent();
			treeViewBlocks.ImageList = Tiles.BlockImageList;
			treeViewItems.ImageList = Tiles.ItemImageList;
			InitializeTreeviews();
        }

		private void InitializeTreeviews()
		{
            Profiler.Start();
            GetTileDataToView(Internal.AnimationCategory.Blocks, treeViewBlocks.Nodes, treeViewBlockCache.Add);
            GetTileDataToView(Internal.AnimationCategory.Items, treeViewItems.Nodes, treeViewItemCache.Add);
            Profiler.Stop();
        }

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is JsonTileInfo tileData)
			{
				selectedTile = tileData.InternalName;
				Debug.WriteLine(selectedTile);
                category = e.Node.TreeView == treeViewItems
					? Internal.AnimationCategory.Items
					: Internal.AnimationCategory.Blocks;
            }
		}

		private void GetTileDataToView(Internal.AnimationCategory key, TreeNodeCollection collection, Action<TreeNode> additinalAction)
		{
			List<JsonTileInfo> textureInfos = key switch
			{
                Internal.AnimationCategory.Blocks => Tiles.BlockTileInfos,
                Internal.AnimationCategory.Items => Tiles.ItemTileInfos,
				_ => throw new InvalidOperationException(nameof(key))
			};
			Profiler.Start();
            if (textureInfos is not null)
            {
				foreach ((int i, var content) in textureInfos.enumerate())
				{
					if (string.IsNullOrEmpty(content.InternalName) || collection.ContainsKey(content.InternalName))
						continue;
					TreeNode tileNode = new TreeNode(content.DisplayName, i, i)
					{
						Name = content.InternalName,
						Tag = content
					};
					collection.Add(tileNode);
					additinalAction(tileNode);
				}
            }
            Profiler.Stop();
        }

		void filter_TextChanged(object sender, EventArgs e)
		{
			// Some code in this function is modified code from this StackOverflow answer - MattNL
			//https://stackoverflow.com/questions/8260322/filter-a-treeview-with-a-textbox-in-a-c-sharp-winforms-app

			//blocks repainting tree until all objects loaded
			treeViewBlocks.BeginUpdate();
			treeViewBlocks.Nodes.Clear();
			treeViewItems.BeginUpdate();
			treeViewItems.Nodes.Clear();
			if (!string.IsNullOrEmpty(metroTextBox1.Text))
			{
				foreach (TreeNode _node in treeViewBlockCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()) || 
						(_node.Tag as string).ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeViewBlocks.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in treeViewItemCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()) ||
						(_node.Tag as string).ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeViewItems.Nodes.Add((TreeNode)_node.Clone());
					}
				}
			}
			else
			{
				foreach (TreeNode _node in treeViewBlockCache)
				{
					treeViewBlocks.Nodes.Add((TreeNode)_node.Clone());
				}
				foreach (TreeNode _node in treeViewItemCache)
				{
					treeViewItems.Nodes.Add((TreeNode)_node.Clone());
				}
			}
			//enables redrawing tree after all objects have been added
			treeViewBlocks.EndUpdate();
			treeViewItems.EndUpdate();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void AcceptBtn_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(selectedTile))
			{
                DialogResult = DialogResult.Cancel;
				return;
            }
            DialogResult = DialogResult.OK;
		}
	}
}
