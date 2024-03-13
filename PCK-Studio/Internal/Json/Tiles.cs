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
    }

    internal static class Tiles
    {
        private static JsonTiles _jsonData;
        internal static JsonTiles JsonTileData => _jsonData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.tileData);

        internal static List<JsonTileInfo> ItemTileInfos => JsonTileData.Items;

        internal static List<JsonTileInfo> BlockTileInfos => JsonTileData.Blocks;
        internal static List<JsonTileInfo> MoonPhasesTileInfos => JsonTileData.MoonPhases;

        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _moonPhasesImages;
        public static Image[] MoonPhasesImages => _moonPhasesImages ??= Resources.moon_phases_sheet.SplitHorizontal(8).ToArray();

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

        private static ImageList _moonPhasesImageList;
        public static ImageList MoonPhasesImageList
        {
            get
            {
                if (_moonPhasesImageList is null)
                {
                    _moonPhasesImageList = new ImageList();
                    _moonPhasesImageList.ColorDepth = ColorDepth.Depth32Bit;
                    _moonPhasesImageList.Images.AddRange(MoonPhasesImages);
                }
                return _moonPhasesImageList;
            }
        }
    }
}
