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
        private JsonTileInfo selectedTile;
        private ResourceCategory category = ResourceCategory.BlockAnimation;

		public JsonTileInfo SelectedTile => selectedTile;
		public ResourceCategory Category => category;

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
            GetTileDataToView(ResourceCategory.BlockAnimation, treeViewBlocks.Nodes, treeViewBlockCache.Add);
            GetTileDataToView(ResourceCategory.ItemAnimation, treeViewItems.Nodes, treeViewItemCache.Add);
            Profiler.Stop();
        }

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is JsonTileInfo tileData)
			{
				selectedTile = tileData;
                category = e.Node.TreeView == treeViewItems
					? ResourceCategory.ItemAnimation
					: ResourceCategory.BlockAnimation;
            }
		}

		private void GetTileDataToView(ResourceCategory key, TreeNodeCollection collection, Action<TreeNode> additionalAction)
		{
			List<JsonTileInfo> textureInfos = key switch
			{
                ResourceCategory.BlockAnimation => Tiles.BlockTileInfos,
                ResourceCategory.ItemAnimation => Tiles.ItemTileInfos,
				_ => throw new InvalidOperationException(nameof(key))
			};
			Profiler.Start();
            if (textureInfos is not null)
            {
				foreach ((int i, var tileData) in textureInfos.enumerate())
				{
					if (string.IsNullOrEmpty(tileData.InternalName) || collection.ContainsKey(tileData.InternalName))
						continue;
					TreeNode tileNode = new TreeNode(tileData.DisplayName, i, i)
					{
						Name = tileData.InternalName,
						Tag = tileData
					};
					collection.Add(tileNode);
					additionalAction(tileNode);
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
						(_node.Tag as JsonTileInfo).InternalName.ToLower().Contains(metroTextBox1.Text.ToLower()))
					{
						treeViewBlocks.Nodes.Add((TreeNode)_node.Clone());
					}
				}
				foreach (TreeNode _node in treeViewItemCache)
				{
					if (_node.Text.ToLower().Contains(metroTextBox1.Text.ToLower()) ||
						(_node.Tag as JsonTileInfo).InternalName.ToLower().Contains(metroTextBox1.Text.ToLower()))
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
			if (string.IsNullOrEmpty(selectedTile.InternalName))
			{
                DialogResult = DialogResult.Cancel;
				return;
            }
            DialogResult = DialogResult.OK;
		}
	}
}
