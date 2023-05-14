using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.Extensions;
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
			treeViewBlocks.ImageList = AnimationResources.BlockList;
			treeViewItems.ImageList = AnimationResources.ItemList;
			InitializeTreeviews();
        }

		private void InitializeTreeviews()
		{
            Stopwatch stopwatch = Stopwatch.StartNew();
            GetTileDataToView("blocks", treeViewBlocks.Nodes, treeViewBlockCache.Add);
            GetTileDataToView("items", treeViewItems.Nodes, treeViewItemCache.Add);
            stopwatch.Stop();
            Debug.WriteLine($"{nameof(InitializeTreeviews)} took {stopwatch.ElapsedMilliseconds}ms");
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

		private void GetTileDataToView(string key, TreeNodeCollection collection, Action<TreeNode> additinalAction)
		{
            try
            {
                if (AnimationResources.JsonTileData[key] is not null)
                {
                    foreach ( (int i, JToken content) in AnimationResources.JsonTileData[key].Children().enumerate())
                    {
                        foreach (JProperty prop in ((JObject)content).Properties())
                        {
                            if (!string.IsNullOrEmpty((string)prop.Value))
                            {
                                TreeNode tileNode = new TreeNode((string)prop.Value)
                                {
                                    Tag = prop.Name,
                                    ImageIndex = i,
                                    SelectedImageIndex = i,
                                };
                                collection.Add(tileNode);
                                additinalAction(tileNode);
                            }
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
