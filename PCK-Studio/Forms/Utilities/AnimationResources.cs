using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

using PckStudio.Properties;
using PckStudio.Extensions;
using PckStudio.Forms.Editor;
using System.Collections.Generic;
using Newtonsoft.Json;
using PckStudio.Internal.Json;

namespace PckStudio.Forms.Utilities
{
    public static class AnimationResources
    {

        private static JsonTiles _jsonData;
        internal static JsonTiles JsonTileData => _jsonData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.tileData);

        internal static List<JsonTileInfo> ItemTileInfos => JsonTileData.Items;

        internal static List<JsonTileInfo> BlockTileInfos => JsonTileData.Blocks;

        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_sheet.CreateImageList(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_sheet.CreateImageList(16).ToArray();

        private static ImageList _itemImageList;
        public static ImageList ItemImageList
        {
            get
            {
                if (_itemImageList is null)
                {
                    _itemImageList = new ImageList();
                    _itemImageList.ColorDepth = ColorDepth.Depth32Bit;
                    _itemImageList.Images.AddRange(ItemImages);
                }
                return _itemImageList;
            }
        }

        private static ImageList _blockImageList;
        public static ImageList BlockImageList
        {
            get
            {
                if (_blockImageList is null)
                {
                    _blockImageList = new ImageList();
                    _blockImageList.ColorDepth = ColorDepth.Depth32Bit;
                    _blockImageList.Images.AddRange(BlockImages);
                }
                return _blockImageList;
            }
        }
    }
}
