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

namespace PckStudio.Forms.Utilities
{
    public static class AnimationResources
    {
        internal class TileJson
        {
            [JsonProperty("blocks")]
            public List<JsonTileInfo> Blocks { get; set; }
            [JsonProperty("items")]
            public List<JsonTileInfo> Items { get; set; }
        }

        private static TileJson _jsonData;
        internal static TileJson JsonTileData => _jsonData ??= JsonConvert.DeserializeObject<TileJson>(Resources.tileData);

        internal class JsonTileInfo
        {
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("internalName")]
            public string InternalName { get; set; }

            [JsonProperty("hasColourEntry", DefaultValueHandling = DefaultValueHandling.Populate)]
            public bool HasColourEntry { get; set; }

            [JsonProperty("colourEntry", DefaultValueHandling = DefaultValueHandling.Populate)]
            public JsonColorEntry ColourEntry { get; set; }

            public JsonTileInfo(string displayName, string internalName)
            {
                DisplayName = displayName;
                InternalName = internalName;
            }
        }

        internal class JsonColorEntry
        {
            [JsonProperty("defaultName", Required = Required.Always)]
            public string DefaultName { get; set; }

            [JsonProperty("isWaterColour", DefaultValueHandling = DefaultValueHandling.Populate)]
            public bool IsWaterColour { get; set; }

            [JsonProperty("variants", DefaultValueHandling = DefaultValueHandling.Populate)]
            public string[] Variants { get; set; }
        }

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
