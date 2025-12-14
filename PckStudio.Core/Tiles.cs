using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }

    public static class Tiles
    {
        public static JsonTiles JsonBlockData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.blockData);
        public static JsonTiles JsonItemData { get; } =  JsonConvert.DeserializeObject<JsonTiles>(Resources.itemData);
        public static JsonTiles JsonParticleData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.particleData);
        public static JsonTiles JsonMoonPhaseData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.moonPhaseData);
        public static JsonTiles JsonMapIconData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.mapIconData);
        public static JsonTiles JsonAdditionalMapIconData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.additionalMapiconsData);
        public static JsonTiles JsonExplosionData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.explosionData);
        public static JsonTiles JsonExperienceOrbData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.experienceOrbData);
        public static JsonTiles JsonPaintingData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.paintingData);
        public static JsonTiles JsonBannerData { get; } = JsonConvert.DeserializeObject<JsonTiles>(Resources.bannerData);

        public static ReadOnlyDictionary<string, JsonColorEntry> ColorEntries { get; } = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, JsonColorEntry>>(Resources.colorEntries);
        public static List<JsonTileInfo> ItemTileInfos => JsonItemData.Entries;
        public static List<JsonTileInfo> BlockTileInfos => JsonBlockData.Entries;
        public static List<JsonTileInfo> ParticleTileInfos => JsonParticleData.Entries;
        public static List<JsonTileInfo> MoonPhaseTileInfos => JsonMoonPhaseData.Entries;
        public static List<JsonTileInfo> MapIconTileInfos => JsonMapIconData.Entries;
        public static List<JsonTileInfo> AdditionalMapIconTileInfos => JsonAdditionalMapIconData.Entries;
        public static List<JsonTileInfo> ExperienceOrbTileInfos => JsonExperienceOrbData.Entries;
        public static List<JsonTileInfo> ExplosionTileInfos => JsonExplosionData.Entries;
        public static List<JsonTileInfo> PaintingTileInfos => JsonPaintingData.Entries;
        public static List<JsonTileInfo> BannerTileInfos => JsonBannerData.Entries;
    }
}
