using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Core;
using PckStudio.Core.Json;
using PckStudio.Json;
using PckStudio.Core.Extensions;
using System.Drawing;
using System.Linq;

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
            GetTileDataToView(ResourceCategory.BlockAnimation);
            GetTileDataToView(ResourceCategory.ItemAnimation);
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
			(List<JsonTileInfo> textureInfos, Image[] images, string name) = key switch
			{
                ResourceCategory.BlockAnimation => (Tiles.BlockTileInfos, ((AtlasResource)ResourceLocations.GetFromCategory(ResourceCategory.BlockAtlas)).GetDefaultAtlas().GetTiles().Select(t => t.Texture).ToArray(), "Blocks"),
                ResourceCategory.ItemAnimation => (Tiles.ItemTileInfos, ((AtlasResource)ResourceLocations.GetFromCategory(ResourceCategory.ItemAtlas)).GetDefaultAtlas().GetTiles().Select(t => t.Texture).ToArray(), "Items"),
				_ => throw new InvalidOperationException(nameof(key))
			};
            TreeView view = filterPrompt.AddFilterPage(name, null, filterPredicate);
			view.ImageList = images.ToImageList();
			foreach ((int i, JsonTileInfo tileData) in textureInfos?.enumerate())
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
