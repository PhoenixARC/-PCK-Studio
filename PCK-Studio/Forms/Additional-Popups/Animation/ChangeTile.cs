using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PckStudio.Core;
using PckStudio.Core.Json;
using PckStudio.Json;
using PckStudio.Core.Extensions;
using System.Drawing;
using System.Linq;
using PckStudio.Controls;

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
			var f = new ImmersiveForm();
			filterPrompt.Dock = DockStyle.Fill;
			filterPrompt.Update();
			f.Controls.Add(filterPrompt);
            return f.ShowDialog(owner);
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
                ResourceCategory.BlockAnimation => (Tiles.BlockTileInfos, ((AtlasResource)ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas))).GetDefaultAtlas().GetTiles().Select(t => t.Texture).ToArray(), "Blocks"),
                ResourceCategory.ItemAnimation => (Tiles.ItemTileInfos, ((AtlasResource)ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas))).GetDefaultAtlas().GetTiles().Select(t => t.Texture).ToArray(), "Items"),
				_ => throw new InvalidOperationException(nameof(key))
			};
            TreeView view = filterPrompt.AddFilterPage(name, null, filterPredicate);
			view.ImageList = images.ToImageList();

			int i = 0;
            foreach (JsonTileInfo tileData in textureInfos)
			{
				if (string.IsNullOrEmpty(tileData.InternalName) || view.Nodes.ContainsKey(tileData.InternalName))
					continue;
				TreeNode tileNode = new TreeNode(tileData.DisplayName, i, i)
				{
					Name = tileData.InternalName,
					Tag = tileData
				};
                view.Nodes.Add(tileNode);
				i++;
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
