using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using MetroFramework.Controls;
using System.Xml.Linq;
using MetroFramework.Forms;
using PckStudio.Extensions;
using PckStudio.Internal;
using PckStudio.Internal.App;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Additional_Popups.Animation
{
    /// Wrapper class kept for simplicity
    internal class ChangeTile : IDisposable
	{
        private JsonTileInfo selectedTile;
        private ResourceCategory category = ResourceCategory.BlockAnimation;

		public JsonTileInfo SelectedTile => selectedTile;
		public ResourceCategory Category => category;

		private FilterPrompt filterPrompt;

		public ChangeTile()
		{
			filterPrompt = new FilterPrompt();
			filterPrompt.OnSelectedItemChanged += filterPrompt_OnSelectedItemChanged;
			InitializeTreeviews();
        }

		private void InitializeTreeviews()
		{
            Profiler.Start();
            GetTileDataToView(ResourceCategory.BlockAnimation);
            GetTileDataToView(ResourceCategory.ItemAnimation);
            Profiler.Stop();
        }

		public DialogResult ShowDialog(IWin32Window owner)
		{
			return filterPrompt.ShowDialog(owner);
		}

		private void filterPrompt_OnSelectedItemChanged(object sender, EventArgs e)
		{
			if (filterPrompt.SelectedItem is JsonTileInfo tileData)
			{
				selectedTile = tileData;
                category = filterPrompt.SelectedTabIndex == 0 ? ResourceCategory.BlockAnimation : ResourceCategory.ItemAnimation;
            }
		}

		private void GetTileDataToView(ResourceCategory key)
		{
			(List<JsonTileInfo> textureInfos, ImageList imgList, string name) = key switch
			{
                ResourceCategory.BlockAnimation => (Tiles.BlockTileInfos, Tiles.BlockImageList, "Blocks"),
                ResourceCategory.ItemAnimation => (Tiles.ItemTileInfos, Tiles.ItemImageList, "Items"),
				_ => throw new InvalidOperationException(nameof(key))
			};
			var view = filterPrompt.AddFilterPage(name, null, filterPredicate);
			view.ImageList = imgList;
			foreach ((int i, var tileData) in textureInfos?.enumerate())
			{
				if (string.IsNullOrEmpty(tileData.InternalName) || view.Nodes.ContainsKey(tileData.InternalName))
					continue;
				TreeNode tileNode = new TreeNode(tileData.DisplayName, i, i)
				{
					Name = tileData.InternalName,
					Tag = tileData
				};
                view.Nodes.Add(tileNode);
				(view.Tag as List<TreeNode>).Add(tileNode);
			}
        }

        private bool filterPredicate(string filterText, object nodeTag)
        {
			return nodeTag is JsonTileInfo tileInfo && tileInfo.InternalName.ToLower().Contains(filterText.ToLower());
        }

        public void Dispose()
        {
			filterPrompt.Dispose();
        }
    }
}
