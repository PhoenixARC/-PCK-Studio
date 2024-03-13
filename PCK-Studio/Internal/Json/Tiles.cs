using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using PckStudio.Extensions;
using PckStudio.Properties;

namespace PckStudio.Internal.Json
{
    internal class JsonTiles
    {
        [JsonProperty("blocks")]
        public List<JsonTileInfo> Blocks { get; set; }
        [JsonProperty("items")]
        public List<JsonTileInfo> Items { get; set; }
        [JsonProperty("moon_phases")]
        public List<JsonTileInfo> MoonPhases { get; set; }
        [JsonProperty("map_icons")]
        public List<JsonTileInfo> MapIcons { get; set; }
        [JsonProperty("additional_map_icons")]
        public List<JsonTileInfo> AdditionalMapIcons { get; set; }
    }

    internal static class Tiles
    {
        private static JsonTiles _jsonData;
        internal static JsonTiles JsonTileData => _jsonData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.tileData);

        internal static List<JsonTileInfo> ItemTileInfos => JsonTileData.Items;

        internal static List<JsonTileInfo> BlockTileInfos => JsonTileData.Blocks;
        internal static List<JsonTileInfo> MoonPhaseTileInfos => JsonTileData.MoonPhases;
        internal static List<JsonTileInfo> MapIconTileInfos => JsonTileData.MapIcons;
        internal static List<JsonTileInfo> AdditionalMapIconTileInfos => JsonTileData.AdditionalMapIcons;

        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _moonPhaseImages;
        public static Image[] MoonPhaseImages => _moonPhaseImages ??= Resources.moon_phases_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _mapIconImages;
        public static Image[] MapIconImages => _mapIconImages ??= Resources.map_icons_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _additionalMapIconImages;
        public static Image[] AdditionalMapIconImages => _mapIconImages ??= Resources.additional_map_icons_sheet.SplitHorizontal(4).ToArray();

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

        private static ImageList _moonPhaseImageList;
        public static ImageList MoonPhaseImageList
        {
            get
            {
                if (_moonPhaseImageList is null)
                {
                    _moonPhaseImageList = new ImageList();
                    _moonPhaseImageList.ColorDepth = ColorDepth.Depth32Bit;
                    _moonPhaseImageList.Images.AddRange(MoonPhaseImages);
                }
                return _moonPhaseImageList;
            }
        }

        private static ImageList _mapIconImageList;
        public static ImageList MapIconImageList
        {
            get
            {
                if (_mapIconImageList is null)
                {
                    _mapIconImageList = new ImageList();
                    _mapIconImageList.ColorDepth = ColorDepth.Depth32Bit;
                    _mapIconImageList.Images.AddRange(MapIconImages);
                }
                return _mapIconImageList;
            }
        }
    }
}
