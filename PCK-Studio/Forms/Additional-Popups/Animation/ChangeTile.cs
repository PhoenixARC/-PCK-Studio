using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Extensions;
using PckStudio.Helper;
using PckStudio.Internal;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	internal partial class ChangeTile : MetroForm
	{
        string selectedTile = "";
        Internal.Animation.AnimationCategory category = Internal.Animation.AnimationCategory.Blocks;

		public string SelectedTile => selectedTile;
		public Internal.Animation.AnimationCategory Category => category;

        List<TreeNode> treeViewBlockCache = new List<TreeNode>();
		List<TreeNode> treeViewItemCache = new List<TreeNode>();

		public ChangeTile()
		{
			InitializeComponent();
			treeViewBlocks.ImageList = AnimationResources.BlockImageList;
			treeViewItems.ImageList = AnimationResources.ItemImageList;
			InitializeTreeviews();
        }

		private void InitializeTreeviews()
		{
            Profiler.Start();
            GetTileDataToView("blocks", treeViewBlocks.Nodes, treeViewBlockCache.Add);
            GetTileDataToView("items", treeViewItems.Nodes, treeViewItemCache.Add);
            Profiler.Stop();
        }

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is string tileData)
			{
				selectedTile = tileData;
				Debug.WriteLine(selectedTile);
                category = e.Node.TreeView == treeViewItems
					? Internal.Animation.AnimationCategory.Items
					: Internal.Animation.AnimationCategory.Blocks;
            }
		}

		private void GetTileDataToView(string key, TreeNodeCollection collection, Action<TreeNode> additinalAction)
		{
			List<JsonTileInfo> textureInfos = key switch
			{
				"blocks" => AnimationResources.BlockTileInfos,
				"items" => AnimationResources.ItemTileInfos,
				_ => throw new InvalidOperationException(key)
			};
			Profiler.Start();
            try
            {
                if (textureInfos is not null)
                {
					foreach ((int i, var content) in textureInfos.enumerate())
					{
						if (!string.IsNullOrEmpty(content.InternalName) && !collection.ContainsKey(content.InternalName))
						{
							TreeNode tileNode = new TreeNode(content.DisplayName)
							{
								Name = content.InternalName,
								Tag = content.DisplayName,
								ImageIndex = i,
								SelectedImageIndex = i,
							};
							collection.Add(tileNode);
							additinalAction(tileNode);
						}
					}
                }
            }
            catch (Newtonsoft.Json.JsonException j_ex)
            {
                MessageBox.Show(j_ex.Message, "Error");
                return;
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
