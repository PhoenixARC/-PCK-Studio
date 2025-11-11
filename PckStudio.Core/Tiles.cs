using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using PckStudio.Core.Extensions;
using PckStudio.Core.Json;
using PckStudio.Core.Properties;

namespace PckStudio.Json
{
    public class JsonTiles
    {
        [JsonProperty("entries")]
        public List<JsonTileInfo> Entries { get; set; }

        [JsonProperty("additional_entries")]
        public List<JsonTileInfo> AdditionalEntries { get; set; }
    }

    public static class Tiles
    {
        private static JsonTiles
            _jsonBlockData, _jsonItemData,
            _jsonParticleData, _jsonMoonPhaseData,
            _jsonMapIconData, _jsonExplosionData,
            _jsonExperienceOrbData, _jsonPaintingData, _jsonBannerData;
        public static JsonTiles JsonBlockData => _jsonBlockData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.blockData);
        public static JsonTiles JsonItemData => _jsonItemData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.itemData);
        public static JsonTiles JsonParticleData => _jsonParticleData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.particleData);
        public static JsonTiles JsonMoonPhaseData => _jsonMoonPhaseData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.moonPhaseData);
        public static JsonTiles JsonMapIconData => _jsonMapIconData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.mapIconData);
        public static JsonTiles JsonExplosionData => _jsonExplosionData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.explosionData);
        public static JsonTiles JsonExperienceOrbData => _jsonExperienceOrbData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.experienceOrbData);
        public static JsonTiles JsonPaintingData => _jsonPaintingData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.paintingData);
        public static JsonTiles JsonBannerData => _jsonBannerData ??= JsonConvert.DeserializeObject<JsonTiles>(Resources.bannerData);

        public static List<JsonTileInfo> ItemTileInfos => JsonItemData.Entries;
        public static List<JsonTileInfo> BlockTileInfos => JsonBlockData.Entries;
        public static List<JsonTileInfo> ParticleTileInfos => JsonParticleData.Entries;
        public static List<JsonTileInfo> MoonPhaseTileInfos => JsonMoonPhaseData.Entries;
        public static List<JsonTileInfo> MapIconTileInfos => JsonMapIconData.Entries;
        public static List<JsonTileInfo> AdditionalMapIconTileInfos => JsonMapIconData.AdditionalEntries;
        public static List<JsonTileInfo> ExperienceOrbTileInfos => JsonExperienceOrbData.Entries;
        public static List<JsonTileInfo> ExplosionTileInfos => JsonExplosionData.Entries;
        public static List<JsonTileInfo> PaintingTileInfos => JsonPaintingData.Entries;
        public static List<JsonTileInfo> BannerTileInfos => JsonBannerData.Entries;

        private static Image[] _itemImages;
        public static Image[] ItemImages => _itemImages ??= Resources.items_atlas.SplitHorizontal(16).ToArray();

        private static Image[] _blockImages;
        public static Image[] BlockImages => _blockImages ??= Resources.terrain_atlas.SplitHorizontal(16).ToArray();

        private static Image[] _particleImages;
        public static Image[] ParticleImages => _particleImages ??= Resources.particles_atlas.SplitHorizontal(16).ToArray();

        private static Image[] _moonPhaseImages;
        public static Image[] MoonPhaseImages => _moonPhaseImages ??= Resources.moon_phases_atlas.SplitHorizontal(4).ToArray();

        private static Image[] _mapIconImages;
        public static Image[] MapIconImages => _mapIconImages ??= Resources.map_icons_atlas.SplitHorizontal(4).ToArray();

        private static Image[] _additionalMapIconImages;
        public static Image[] AdditionalMapIconImages => _additionalMapIconImages ??= Resources.additional_map_icons_atlas.SplitHorizontal(4).ToArray();

        private static Image[] _experienceOrbIconImages;
        public static Image[] ExperienceOrbImages => _experienceOrbIconImages ??= Resources.experience_orbs_atlas.SplitHorizontal(4).ToArray();

        private static Image[] _explosionImages;
        public static Image[] ExplosionImages => _explosionImages ??= Resources.explosions_atlas.SplitHorizontal(4).ToArray();

        private static Image[] _paintingImages;
        public static Image[] PaintingImages => _paintingImages ??= Resources.paintings_atlas.SplitHorizontal(16).ToArray();

        private static Image[] _bannerImages;
        public static Image[] BannerImages => _bannerImages ??= Resources.banners_atlas.SplitHorizontal(6).ToArray();

        private static ImageList GetImageList(Image[] images)
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.AddRange(images);

            return imageList;
        }

        private static ImageList _blockImageList = GetImageList(BlockImages);
        public static ImageList BlockImageList { get { return _blockImageList; } }

        private static ImageList _itemImageList = GetImageList(ItemImages);
        public static ImageList ItemImageList { get { return _itemImageList; } }

        private static ImageList _particleImageList = GetImageList(ParticleImages);
        public static ImageList ParticleImageList { get { return _particleImageList; } }

        private static ImageList _moonPhaseImageList = GetImageList(MoonPhaseImages);
        public static ImageList MoonPhaseImageList { get { return _moonPhaseImageList; } }

        private static ImageList _mapIconImageList = GetImageList(MapIconImages);
        public static ImageList MapIconImageList { get { return _mapIconImageList; } }

        private static ImageList _additionalMapIconImageList = GetImageList(AdditionalMapIconImages);
        public static ImageList AdditionalMapIconImageList { get { return _additionalMapIconImageList; } }

        private static ImageList _experienceOrbsImageList = GetImageList(ExperienceOrbImages);
        public static ImageList ExperienceOrbImageList { get { return _experienceOrbsImageList; } }

        private static ImageList _explosionImageList = GetImageList(ExplosionImages);
        public static ImageList ExplosionImageList { get { return _explosionImageList; } }

        private static ImageList _paintingImageList = GetImageList(PaintingImages);
        public static ImageList PaintingImageList { get { return _paintingImageList; } }

        private static ImageList _bannerImageList = GetImageList(BannerImages);
        public static ImageList BannerImageList { get { return _bannerImageList; } }
    }
}
