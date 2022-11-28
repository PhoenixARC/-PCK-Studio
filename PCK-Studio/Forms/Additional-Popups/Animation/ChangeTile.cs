using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.Forms.Utilities;

namespace PckStudio.Forms.Additional_Popups.Animation
{
	public partial class ChangeTile : MetroForm
	{
        string selectedTile = "";

        bool isItem = false;
		public string SelectedTile => selectedTile;
		public bool IsItem => isItem;

		List<TreeNode> treeViewBlockCache = new List<TreeNode>();
		List<TreeNode> treeViewItemCache = new List<TreeNode>();

		public ChangeTile()
		{
			InitializeComponent();
			ImageList tiles = new ImageList();
			tiles.ColorDepth = ColorDepth.Depth32Bit;
			tiles.Images.AddRange(AnimationUtil.tileImages);
			treeViewBlocks.ImageList = tiles;
			treeViewItems.ImageList = tiles;

			try
			{
				int i = 0;

				if (AnimationUtil.tileData["blocks"] != null)
				{
					foreach (JObject content in AnimationUtil.tileData["blocks"].Children())
					{
						foreach (JProperty prop in content.Properties())
						{
							if (!string.IsNullOrEmpty((string)prop.Value))
							{
								TreeNode tileNode = new TreeNode((string)prop.Value)
								{
									Tag = prop.Name,
									ImageIndex = i,
									SelectedImageIndex = i,
								};
								treeViewBlocks.Nodes.Add(tileNode);
								treeViewBlockCache.Add(tileNode);
							}
							i++;
						}
					}
				}
				if (AnimationUtil.tileData["items"] != null)
				{
					foreach (JObject content in AnimationUtil.tileData["items"].Children())
					{
                        foreach (JProperty prop in content.Properties())
						{
							if (!string.IsNullOrEmpty((string)prop.Value))
							{
                                TreeNode tileNode = new TreeNode((string)prop.Value)
                                {
                                    Tag = prop.Name,
                                    ImageIndex = i,
                                    SelectedImageIndex = i,
                                };
                                treeViewItems.Nodes.Add(tileNode);
								treeViewItemCache.Add(tileNode);
							}
							i++;
						}
					}
				}
			}
			catch (Newtonsoft.Json.JsonException j_ex)
			{
				MessageBox.Show(j_ex.Message, "Error");
				return;
			}
		}

		private void treeViews_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is string tileData)
			{
				selectedTile = tileData;
				Console.WriteLine(selectedTile);
				isItem = e.Node.TreeView == treeViewItems;
			}
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
			Close();
		}

		private void AcceptBtn_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(selectedTile)) CancelBtn_Click(sender, e);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
