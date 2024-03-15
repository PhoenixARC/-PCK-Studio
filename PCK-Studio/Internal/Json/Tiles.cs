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

        [JsonProperty("experience_orbs")]
        public List<JsonTileInfo> ExperienceOrbs { get; set; }

        [JsonProperty("explosion")]
        public List<JsonTileInfo> Explosion { get; set; }
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
        internal static List<JsonTileInfo> ExperienceOrbTileInfos => JsonTileData.ExperienceOrbs;
        internal static List<JsonTileInfo> ExplosionTileInfos => JsonTileData.Explosion;

        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_sheet.SplitHorizontal(16).ToArray();

        private static Image[] _moonPhaseImages;
        public static Image[] MoonPhaseImages => _moonPhaseImages ??= Resources.moon_phases_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _mapIconImages;
        public static Image[] MapIconImages => _mapIconImages ??= Resources.map_icons_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _additionalMapIconImages;
        public static Image[] AdditionalMapIconImages => _additionalMapIconImages ??= Resources.additional_map_icons_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _experienceOrbIconImages;
        public static Image[] ExperienceOrbImages => _experienceOrbIconImages ??= Resources.experience_orbs_sheet.SplitHorizontal(4).ToArray();

        private static Image[] _explosionImages;
        public static Image[] ExplosionImages => _explosionImages ??= Resources.explosion_sheet.SplitHorizontal(4).ToArray();

        private static ImageList GetImageList(Image[] images)
        {
            ImageList _imageList = new ImageList();
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.Images.AddRange(images);

            return _imageList;
        }

        private static ImageList _itemImageList = GetImageList(ItemImages);
        public static ImageList ItemImageList { get { return _itemImageList; } }

        private static ImageList _blockImageList = GetImageList(BlockImages);
        public static ImageList BlockImageList { get { return _blockImageList; } }

        private static ImageList _moonPhaseImageList = GetImageList(MoonPhaseImages);
        public static ImageList MoonPhaseImageList { get { return _moonPhaseImageList; } }

        private static ImageList _mapIconImageList = GetImageList(MapIconImages);
        public static ImageList MapIconImageList { get { return _mapIconImageList; } }

        private static ImageList _additionalMapIconImageList = GetImageList(AdditionalMapIconImages);
        public static ImageList AdditionalMapIconImageList { get { return _additionalMapIconImageList; } }

        private static ImageList _experienceOrbsImageList = GetImageList(ExperienceOrbImages);
        public static ImageList ExperienceOrbsImageList { get { return _experienceOrbsImageList; } }

        private static ImageList _explosionImageList = GetImageList(ExplosionImages);
        public static ImageList ExplosionImageList { get { return _explosionImageList; } }
    }
}
