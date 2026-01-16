using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMI.Formats.Archive;
using OMI.Formats.FUI;
using OMI.Workers.FUI;
using PckStudio.Core.Deserializer;
using PckStudio.Core.DLC;
using PckStudio.Core.Extensions;
using PckStudio.Core.Json;
using PckStudio.Core.Properties;

namespace PckStudio.Core.IO.Java
{
    public class ResourcePackImporter
    {
        internal const int TARGET_FORMAT_VERSION = 4;
        private const string JAVA_RESOURCE_PACK_PATH = "assets/minecraft/textures";

        static readonly IReadOnlyDictionary<Version, IMinecraftJavaVersion> _formatVeriosnToGameVersion = new Dictionary<Version, IMinecraftJavaVersion>()
        {
            [new (1, 0)] = new VersionRange("1.6.1", "1.8.9"),
            [new (2, 0)] = new VersionRange("1.9", "1.10.2"),
            [new (3, 0)] = new VersionRange("1.11", "1.12.2"),

            // ----- TARGET ----- //
            [new (4, 0)] = new VersionRange("1.13", "1.14.4"),
            // ------------------ //

            [new (5, 0)] = new VersionRange("1.15", "1.16.1"),
            [new (6, 0)] = new VersionRange("1.16.2", "1.16.5"),
            [new (7, 0)] = new VersionRange("1.17", "1.17.1"),
            [new (8, 0)] = new VersionRange("1.18", "1.18.2"),
            [new (9, 0)] = new VersionRange("1.19", "1.19.2"),
            [new (12, 0)] = new SingleVersion("1.19.3"),
            [new (13, 0)] = new SingleVersion("1.19.4"),
            [new (15, 0)] = new VersionRange("1.20", "1.20.1"),
            [new (18, 0)] = new SingleVersion("1.20.2"),
            [new (22, 0)] = new VersionRange("1.20.3", "1.20.4"),
            [new (32, 0)] = new VersionRange("1.20.5", "1.20.6"),
            [new (34, 0)] = new VersionRange("1.21", "1.21.1"),
            [new (42, 0)] = new VersionRange("1.21.2", "1.21.3"),
            [new (46, 0)] = new SingleVersion("1.21.4"),
            [new (55, 0)] = new SingleVersion("1.21.5"),
            [new (63, 0)] = new SingleVersion("1.21.6"),
            [new (64, 0)] = new VersionRange("1.21.7", "1.21.8"),
            [new (69, 0)] = new VersionRange("1.21.9", "1.21.10"),
        };

        public class TextureImportStats(int maxTextures)
        {
            public int Animations => animations;
            public int Textures => textures;
            public int MissingTextures => _maxTextures - textures;
            public int MaxTextures => _maxTextures;

            internal int animations = 0;
            internal int textures = 0;
            private int _maxTextures = maxTextures;

            public static TextureImportStats operator +(TextureImportStats lhs, TextureImportStats rhs)
            {
                return new TextureImportStats(lhs.MaxTextures + rhs.MaxTextures) { animations = lhs.Animations + rhs.Animations, textures = lhs.Textures + rhs.Textures };
            }
        }

        private readonly LCEGameVersion _gameVersion;
        private string _name;
        private ImportStatusReport _importStatus;
        private McPack _packMeta;
        private Dictionary<string, ZipArchiveEntry> _resoursePackData;

        public ResourcePackImporter(LCEGameVersion gameVersion)
        {
            _gameVersion = gameVersion;
        }

        public static bool IsJavaResourcePack(ZipArchive zip)
            => zip.GetEntry("pack.mcmeta") is not null &&
            McMeta.LoadMcMeta(zip.GetEntry("pack.mcmeta").ReadAllText()).Contains("pack");

        public bool StartImport(string name, ZipArchive zip, ImportStatusReport importStatus)
        {
            if (!IsJavaResourcePack(zip))
            {
                MessageBoxEx.ShowError("Zip file is not a resource pack", "Import Error");
                return false;
            }
            _name = name;
            _importStatus = importStatus ?? ImportStatusReport.CreateEmpty();
            _packMeta = ReadPackMeta(zip);
            _resoursePackData = zip.GetDirectoryContent(JAVA_RESOURCE_PACK_PATH, includeSubDirectories: true).ToDictionary(e => e.FullName.Substring(JAVA_RESOURCE_PACK_PATH.Length + 1));
            return true;
        }

        public McPack ReadPackMeta(ZipArchive zip)
        {
            string json = zip.GetEntry("pack.mcmeta").ReadAllText();
            McPack pack = McMeta.LoadMcMeta(json)["pack"].ToObject<McPack>();
            if (pack.Format == TARGET_FORMAT_VERSION)
                _importStatus.Post("Target format version... less work?");
            _importStatus.Post($"Importing textures from resource pack of version: {GetJavaGameVersionFromResourcePackFormat(pack.Format)}(Format:{pack.Format})");
            pack.Icon = zip.GetEntry("pack.png")?.GetImage();
            return pack;
        }

        private IDictionary<string, ZipArchiveEntry> GetEntries(string path, string extension = "", bool includeSubDirectories = false)
        {
            string sanitisedDirectoryPath = path.Replace("\\", "/");
            return _resoursePackData.Where(kv => kv.Key.StartsWith(sanitisedDirectoryPath))
                .Where(kv => includeSubDirectories || (kv.Key.Substring(sanitisedDirectoryPath.Length).LastIndexOf('/') == 0 || kv.Key.Substring(sanitisedDirectoryPath.Length).LastIndexOf('/') == -1))
                .Where(kv => string.IsNullOrWhiteSpace(extension) || kv.Key.EndsWith(extension))
                .ToDictionary(kv =>
                {
                    string res = kv.Key.Substring(path.Length, kv.Key.Length - path.Length - extension.Length);
                    return res[0] == '/' ? res.Substring(1) : res;
                }, kv => kv.Value);
        }

        public ImportResult<DLCTexturePackage, TextureImportStats> ImportAsTexturePack()
        {
            ImportResult<(Atlas Atlas, IDictionary<string, Animation> Animations), TextureImportStats> block = ImportAtlas(ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)) as AtlasResource);
            ImportResult<(Atlas Atlas, IDictionary<string, Animation> Animations), TextureImportStats> item = ImportAtlas(ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)) as AtlasResource);
            ImportResult<(Atlas Atlas, IDictionary<string, Animation>), TextureImportStats> particles = ImportAtlas(ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ParticleAtlas)) as AtlasResource);

            if (!item.Result.Animations.ContainsKey("clock"))
            {
                Animation clock = ImportAnimation("clock_", AtlasResource.AtlasType.ItemAtlas);
                item.Result.Animations.Add("clock", clock);
            }

            if (!item.Result.Animations.ContainsKey("compass"))
            {
                Animation compass = ImportAnimation("compass_", AtlasResource.AtlasType.ItemAtlas);
                item.Result.Animations.Add("compass", compass);
            }

            const int cLAVA_TEXTURE = 238;
            const int cWATER_TEXTURE = 206;
            int maxWidth = block.Result.Atlas.GetTiles().Where(t => t.Index != cLAVA_TEXTURE && t.Index != cWATER_TEXTURE).Concat(item.Result.Atlas.GetTiles()).Max(t => t.Texture.Width);

            Size tileSize = new Size(maxWidth, maxWidth);

            DLCTexturePackage.TextureResolution resolution = DLCTexturePackage.GetTextureResolution(tileSize);

            ImportResult<(DLCTexturePackage.EnvironmentData environmentData, Atlas moonPhases, Image sun), TextureImportStats> envData = ImportEnvironmentData();

            ImportResult<(ArmorSet leather, ArmorSet chain, ArmorSet gold, ArmorSet iron, ArmorSet diamond, ArmorSet turtle), TextureImportStats> armorSets = ImportArmorSets();

            ImportResult<(Dictionary<string, Image> mobs, Dictionary<string, Image> items), TextureImportStats> entityModelTextures = ImportEntityModels();

            ImportResult<Dictionary<string, Image>, TextureImportStats> misc = ImportMisc();

            const int cCHEST_BREAK_TEXTURE = 149;
            const int cENDERCHEST_BREAK_TEXTURE = 150;
            if (entityModelTextures.Result.items.TryGetValue("item/chest", out Image chest))
            {
                block.Result.Atlas[cCHEST_BREAK_TEXTURE].Texture = chest.Resize(block.Result.Atlas.TileSize, GraphicsConfig.PixelPerfect());
                block.Stats.textures++;
            }

            if (entityModelTextures.Result.items.TryGetValue("item/enderchest", out Image enderchest))
            {
                block.Result.Atlas[cENDERCHEST_BREAK_TEXTURE].Texture = enderchest.Resize(block.Result.Atlas.TileSize, GraphicsConfig.PixelPerfect());
                block.Stats.textures++;
            }

            ConsoleArchive mediaArc = default;
            if (MessageBoxEx.AskQuestion("Import UI?\nThis will take more time to import!", "Include UI", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                mediaArc = ImportUI();
            }

            DLCTexturePackage.MetaData metaData = new DLCTexturePackage.MetaData(null, _packMeta.Icon);

            int id = new Random().Next(0, GameConstants.MAX_PACK_ID - 1);

            TextureImportStats stats = block.Stats + item.Stats + particles.Stats + envData.Stats + armorSets.Stats + entityModelTextures.Stats + misc.Stats;

            string name = JavaConstants.EsapceMiencarftJavaFormat(_name.Replace("_", " "));
            string description = JavaConstants.ConvertJavaTextFormatToHTML(_packMeta.Description);

            DLCTexturePackage dlcTexturePackage = new DLCTexturePackage(name, description, id,
                    metaData,
                    resolution,
                    block.Result.Atlas,
                    item.Result.Atlas,
                    particles.Result.Atlas,
                    paintingAtlas: null,
                    moonPhaseAtlas: envData.Result.moonPhases,
                    mapIconsAtlas: null,
                    additionalMapIconsAtlas: null,
                    leatherArmorSet: armorSets.Result.leather,
                    chainArmorSet: armorSets.Result.chain,
                    ironArmorSet: armorSets.Result.iron,
                    goldArmorSet: armorSets.Result.gold,
                    diamondArmorSet: armorSets.Result.diamond,
                    turtleArmorSet: armorSets.Result.turtle,
                    environmentData: envData.Result.environmentData,
                    colorContainter: null,
                    entityModelTextures.Result.items,
                    entityModelTextures.Result.mobs,
                    customModels: null,
                    materials: null,
                    blockEntityBreakAnimation: null,
                    itemAnimations: item.Result.Animations,
                    blockAnimations: block.Result.Animations,
                    sun: envData.Result.sun,
                    moon: null,
                    mediaArc: mediaArc,
                    misc: misc.Result,
                    parentPackage: null);
            return new ImportResult<DLCTexturePackage, TextureImportStats>(dlcTexturePackage, stats);
        }

        private ImportResult<Dictionary<string, Image>, TextureImportStats> ImportMisc()
        {
            Dictionary<string, Image> res = new Dictionary<string, Image>();
            TextureImportStats stats = new TextureImportStats(latest2lce_misc.Count);
            foreach (KeyValuePair<string, JavaResourceConvertJson> item in latest2lce_misc)
            {
                if (GetEntries(item.Key).FirstOrDefault().Value is ZipArchiveEntry entry)
                {
                    Image texture = entry.GetImage();
                    res.Add(item.Value.LceName, texture);
                    stats.textures++;
                }
            }
            return new ImportResult<Dictionary<string, Image>, TextureImportStats>(res, stats);
        }

        private Animation ImportAnimation(string suffix, AtlasResource.AtlasType atlasType)
        {
            string atlasPath = GetAtlasPathFromFormat(_packMeta.Format, atlasType);
            string path = Path.Combine(atlasPath, suffix);
            List<(int index, Image texture)> frameTextures = new List<(int, Image)>(64);
            foreach (KeyValuePair<string, ZipArchiveEntry> frameEntry in GetEntries(path, ".png"))
            {
                string name = Path.GetFileNameWithoutExtension(frameEntry.Key);
                if (int.TryParse(name.End(2), out int index))
                    frameTextures.Add((index, frameEntry.Value.GetImage()));
            }
            frameTextures.Sort((a, b) => a.index - b.index);
            return new Animation(frameTextures.Select(t => t.texture), true);
        }

        public ImportResult<(Atlas Atlas, IDictionary<string, Animation> Animations), TextureImportStats> ImportAtlas(AtlasResource atlasResource)
        {
            _importStatus.Post($"[{nameof(ImportAtlas)}] Importing: Atlas('{atlasResource.Path}')");
            Atlas atlas = Atlas.CreateDefault(atlasResource, _gameVersion);
            string path = GetAtlasPathFromFormat(_packMeta.Format, atlasResource.Type);
            IDictionary<string, ZipArchiveEntry> entries = GetEntries(path);

            IReadOnlyDictionary<string, JavaResourceConvertJson> lookUpTable = GetVersionLookUpTable(atlasResource.Type);

            IEnumerable<(int index, JsonTileInfo value)> a = atlasResource.TilesInfo.Select((v, i) => (i, v)).GroupBy(it => it.v.InternalName).Select(grp => grp.FirstOrDefault());
            IReadOnlyDictionary<string, int> map = a.ToDictionary(tileInfo => string.IsNullOrEmpty(tileInfo.value.InternalName) ? $"{Guid.NewGuid()}.{tileInfo.index}" : tileInfo.value.InternalName.ToLowerInvariant(), it => it.index);

            IReadOnlyDictionary<string, ZipArchiveEntry> mcMetaFiles = entries.Values.Where(e => e.FullName.EndsWith(".mcmeta")).ToDictionary(entry => entry.FullName);

            TextureImportStats stats = new TextureImportStats(atlasResource.TilesInfo.Where(t => !string.IsNullOrWhiteSpace(t.InternalName)).Count());
            IDictionary<string, Animation> animations = new Dictionary<string, Animation>();
            foreach (ZipArchiveEntry entry in entries.Values)
            {
                if (!entry.FullName.EndsWith(".png"))
                    continue;
                string name = Path.GetFileNameWithoutExtension(entry.FullName);
                bool isPartOfGroup = atlasResource.AtlasGroups.Any(grp => grp.InternalName == name);
                if (!map.TryGetValue(name, out int index) &&
                    !isPartOfGroup &&
                    !(lookUpTable.TryGetValue(name, out JavaResourceConvertJson lceKey) && map.TryGetValue(lceKey.LceName.ToLowerInvariant(), out index)) || !index.IsWithinRangeOf(0, atlas.TileCount - 1))
                    continue;

                AtlasGroup atlasGroup = atlasResource.AtlasGroups.FirstOrDefault(grp => grp.InternalName == name);
                AtlasTile tile = isPartOfGroup ? atlas[atlasGroup.Row, atlasGroup.Column] : atlas[index];
                JsonTileInfo tileInfo = tile?.GetUserDataOfType<JsonTileInfo>();

                Image img = entry.GetImage();
                bool hasMcMeta = mcMetaFiles.TryGetValue(entry.FullName + ".mcmeta", out ZipArchiveEntry archiveEntry);
                if (hasMcMeta)
                {
                    string jsonData = archiveEntry.ReadAllText();
                    McMeta mcMeta = McMeta.LoadMcMeta(jsonData);
                    string animationName = tileInfo?.InternalName ?? name;
                    if (mcMeta.Contains("animation") && !animations.ContainsKey(animationName))
                    {
                        Animation animation = AnimationDeserializer.DefaultDeserializer.DeserializeJavaAnimation(mcMeta, img);
                        if (animation.FrameCount > 0)
                            animations.Add(animationName, animation);
                        stats.animations++;
                        img = img.GetArea(new Rectangle(Point.Empty, new Size(img.Width, img.Width)));
                        stats.textures++;
                    }
                }
                if (isPartOfGroup && atlasGroup is not null)
                {
                    atlas.SetGroup(atlasGroup, img);
                    continue;
                }

                tile.Texture = img;
                stats.textures++;
            }
            ImportResult<(Atlas atlas, IDictionary<string, Animation> animations), TextureImportStats> result = new ImportResult<(Atlas atlas, IDictionary<string, Animation> animations), TextureImportStats>((atlas, animations), stats);
            _importStatus.Post($"Import Stats of '{atlasResource.Type}'");
            _importStatus.Post($"Textures: {stats.Textures}/{stats.MaxTextures}({stats.MissingTextures} missing)");
            _importStatus.Post($"Animations: {stats.Animations}");
            return result;
        }

        static readonly IReadOnlyDictionary<string, JavaResourceConvertJson> latest2lce_blocks = JsonConvert.DeserializeObject<Dictionary<string, JavaResourceConvertJson>>(Resources.latest2lce_blocks);
        static readonly IReadOnlyDictionary<string, JavaResourceConvertJson> latest2lce_items = JsonConvert.DeserializeObject<Dictionary<string, JavaResourceConvertJson>>(Resources.latest2lce_items);
        static readonly IReadOnlyDictionary<string, JavaResourceConvertJson> latest2lce_entities = JsonConvert.DeserializeObject<Dictionary<string, JavaResourceConvertJson>>(Resources.latest2lce_entities);
        static readonly IReadOnlyDictionary<string, JavaResourceConvertJson> latest2lce_misc = JsonConvert.DeserializeObject<Dictionary<string, JavaResourceConvertJson>>(Resources.latest2lce_misc);

        class JavaResourceConvertJson
        {
            [JsonProperty("lce_name")]
            public string LceName { get; set; }
            [JsonProperty("texture_remap")]
            public JavaTextureRemap TextureRemap { get; set; }
        }

        class JavaTextureRemap
        {
            [JsonProperty("source_size")]
            [JsonConverter(typeof(SizeJsonConverter))]
            public Size SourceSize;

            [JsonProperty("format")]
            public int Format = -1;

            [JsonProperty("target_size")]
            [JsonConverter(typeof(SizeJsonConverter))]
            public Size TargetSize;

            [JsonProperty("areas")]
            public JavaTextureRemapArea[] Areas;
        }

        class JavaTextureRemapArea
        {
            [JsonProperty("size")]
            [JsonConverter(typeof(SizeJsonConverter))]
            public Size Size { get; set; }

            [JsonProperty("from")]
            [JsonConverter(typeof(PointJsonConverter))]
            public Point From { get; set; }

            [JsonProperty("to")]
            [JsonConverter(typeof(PointJsonConverter))]
            public Point To { get; set; }

            [JsonProperty("rotation")]
            public float Rotation { get; set; }

            public enum FlipDirection
            {
                None,
                x,
                y,
                xy,
            }

            [JsonProperty("flip")]
            public FlipDirection Flip { get; set; } = FlipDirection.None;
        }

        private Image RemapTexture(Image source, JavaTextureRemap textureRemap)
        {
            if (textureRemap is null || textureRemap.Format > _packMeta.Format)
                return source;

            int xSclar = 1;
            int ySclar = 1;
            Size s = source.Size;
            if (textureRemap.SourceSize != Size.Empty || textureRemap.TargetSize != Size.Empty)
            {
                xSclar = Math.Max(source.Width / textureRemap.SourceSize.Width, 1);
                ySclar = Math.Max(source.Height / textureRemap.SourceSize.Height, 1);
                s = textureRemap.TargetSize;
            }
            Image res = new Bitmap(s.Width * xSclar, s.Height * ySclar);

            using Graphics g = Graphics.FromImage(res);
            g.ApplyConfig(GraphicsConfig.PixelPerfect());
            foreach (JavaTextureRemapArea remapArea in textureRemap.Areas)
            {
                Point from = new Point(remapArea.From.X * xSclar, remapArea.From.Y * ySclar);
                Point to = new Point(remapArea.To.X * xSclar, remapArea.To.Y * ySclar);
                Size size = new Size(remapArea.Size.Width * xSclar, remapArea.Size.Height * ySclar);
                Rectangle area = new Rectangle(from, size);
                Image sourceAreaImg = source.GetArea(area);
                if (remapArea.Rotation != 0)
                {
                    if (remapArea.Rotation == 90f)
                        sourceAreaImg.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    if (remapArea.Rotation == -90f || remapArea.Rotation == 270f)
                        sourceAreaImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    if (remapArea.Rotation == 180f)
                        sourceAreaImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                switch (remapArea.Flip)
                {
                    case JavaTextureRemapArea.FlipDirection.x:
                        sourceAreaImg.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case JavaTextureRemapArea.FlipDirection.y:
                        sourceAreaImg.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        break;
                    case JavaTextureRemapArea.FlipDirection.xy:
                        sourceAreaImg.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        break;
                    case JavaTextureRemapArea.FlipDirection.None:
                    default:
                        break;
                }
                g.DrawImage(sourceAreaImg, to);
            }
            return res;
        }


        private static IReadOnlyDictionary<string, JavaResourceConvertJson> GetVersionLookUpTable(AtlasResource.AtlasType atlasType)
        {
            return atlasType switch
            {
                AtlasResource.AtlasType.BlockAtlas => latest2lce_blocks,
                AtlasResource.AtlasType.ItemAtlas => latest2lce_items,
                _ => new Dictionary<string, JavaResourceConvertJson>()
            };
        }

        private static string GetJavaGameVersionFromResourcePackFormat(int format) => _formatVeriosnToGameVersion.TryGetValue(new (format, 0), out IMinecraftJavaVersion versionRange) ? versionRange.ToString(" - ") : "unknown";

        private static string GetAtlasPathFromFormat(int format, AtlasResource.AtlasType type)
        {
            return type switch
            {
                AtlasResource.AtlasType.ItemAtlas when format > 3 => "item",
                AtlasResource.AtlasType.BlockAtlas when format > 3 => "block",
                AtlasResource.AtlasType.ItemAtlas => "items",
                AtlasResource.AtlasType.BlockAtlas => "blocks",
                AtlasResource.AtlasType.ParticleAtlas => "particle",
                AtlasResource.AtlasType.BannerAtlas => "entity/banner",
                AtlasResource.AtlasType.PaintingAtlas => "painting",
                AtlasResource.AtlasType.ExplosionAtlas => throw new NotImplementedException(),
                AtlasResource.AtlasType.ExperienceOrbAtlas => throw new NotImplementedException(),
                AtlasResource.AtlasType.MoonPhaseAtlas => throw new NotImplementedException(),
                AtlasResource.AtlasType.MapIconAtlas => throw new NotImplementedException(),
                AtlasResource.AtlasType.AdditionalMapIconsAtlas => throw new NotImplementedException(),
                _ => throw new Exception()
            };
        }

        private ImportResult<(ArmorSet Leather, ArmorSet Chain, ArmorSet Gold, ArmorSet Iron, ArmorSet Diamond, ArmorSet Turtle), TextureImportStats> ImportArmorSets()
        {
            _importStatus.Post($"[{nameof(ImportArmorSets)}] Importing: Armor");
            string path = Path.Combine("models", "armor");
            IDictionary<string, ZipArchiveEntry> entries = GetEntries(path, ".png");

            TextureImportStats stats = new TextureImportStats(13);

            ArmorSet GetArmorSet(string armorName, string topName, string bottomName, bool hasOverlay = false)
            {
                Image topImage = entries.TryGetValue(topName, out ZipArchiveEntry entry) ? entry.GetImage() : default;
                Image bottomImage = default;
                if (!string.IsNullOrWhiteSpace(bottomName))
                    bottomImage = entries.TryGetValue(bottomName, out entry) ? entry.GetImage() : default;

                Image baseTexture = topImage?.Combine(bottomImage ?? topImage, ImageLayoutDirection.Vertical);
                Image overlayTexture = default;

                if (hasOverlay)
                {
                    topName += "_overlay";
                    bottomName += "_overlay";
                    Image topOverlayImage = entries.TryGetValue(topName, out entry) ? entry.GetImage() : default;
                    Image bottomOverlayImage = entries.TryGetValue(bottomName, out entry) ? entry.GetImage() : default;
                    overlayTexture = topOverlayImage?.Combine(bottomOverlayImage ?? topOverlayImage, ImageLayoutDirection.Vertical);
                    stats.textures++;
                    stats.textures += bottomOverlayImage is not null ? 1 : 0;
                }
                if (baseTexture is null)
                    return null;
                stats.textures++;
                stats.textures += bottomImage is not null ? 1 : 0;
                return new ArmorSet(armorName, baseTexture, overlayTexture);
            }

            ArmorSet leather = GetArmorSet(ArmorSetDescription.CLOTH, "leather_layer_1", "leather_layer_2", hasOverlay: true);
            ArmorSet chain = GetArmorSet(ArmorSetDescription.CHAIN, "chainmail_layer_1", "chainmail_layer_2");
            ArmorSet gold = GetArmorSet(ArmorSetDescription.GOLD, "gold_layer_1", "gold_layer_2");
            ArmorSet iron = GetArmorSet(ArmorSetDescription.IRON, "iron_layer_1", "iron_layer_2");
            ArmorSet diamond = GetArmorSet(ArmorSetDescription.DIAMOND, "diamond_layer_1", "diamond_layer_2");
            ArmorSet turtle = GetArmorSet(ArmorSetDescription.TURTLE, "turtle_layer_1", null);
            return new ImportResult<(ArmorSet leather, ArmorSet chain, ArmorSet gold, ArmorSet iron, ArmorSet diamond, ArmorSet turtle), TextureImportStats>((leather, chain, gold, iron, diamond, turtle), stats);
        }

        private ImportResult<(DLCTexturePackage.EnvironmentData environmentData, Atlas moonPhases, Image sun), TextureImportStats> ImportEnvironmentData()
        {
            _importStatus.Post($"[{nameof(ImportEnvironmentData)}] Importing: Environment Data");

            string path = Path.Combine("environment");

            IDictionary<string, ZipArchiveEntry> entries = GetEntries(path, ".png");

            TextureImportStats stats = new TextureImportStats(5);

            stats.textures += entries.TryGetValue("clouds", out ZipArchiveEntry cloudsEntry) ? 1 : 0;
            stats.textures += entries.TryGetValue("rain", out ZipArchiveEntry rainEntry) ? 1 : 0;
            stats.textures += entries.TryGetValue("snow", out ZipArchiveEntry snowEntry) ? 1 : 0;
            stats.textures += entries.TryGetValue("moon_phases", out ZipArchiveEntry moonPhasesEntry) ? 1 : 0;
            stats.textures += entries.TryGetValue("sun", out ZipArchiveEntry sunEntry) ? 1 : 0;

            DLCTexturePackage.EnvironmentData environmentData = new DLCTexturePackage.EnvironmentData(cloudsEntry?.GetImage(), rainEntry?.GetImage(), snowEntry?.GetImage());
            Image moonPhasesTexture = moonPhasesEntry?.GetImage();
            Image sun = sunEntry?.GetImage();
            Atlas moonPhases = default;
            if (moonPhasesTexture is not null)
            {
                moonPhases = Atlas.FromResourceLocation(moonPhasesTexture, ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.MoonPhaseAtlas)) as AtlasResource, _gameVersion);
            }
            return new ImportResult<(DLCTexturePackage.EnvironmentData environmentData, Atlas moonPhases, Image sun), TextureImportStats>((environmentData, moonPhases, sun), stats);
        }

        //! TODO: fix villiager (v1.14 or higher), husk(64x64=>64x32), 
        private ImportResult<(Dictionary<string, Image> mobs, Dictionary<string, Image> items), TextureImportStats> ImportEntityModels()
        {
            _importStatus.Post($"[{nameof(ImportArmorSets)}] Importing: Entity Models");

            string path = "entity";
            IDictionary<string, ZipArchiveEntry> entries = GetEntries(path, ".png", includeSubDirectories: true);

            Dictionary<string, Image> mobs = new Dictionary<string, Image>();
            Dictionary<string, Image> items = new Dictionary<string, Image>();

            TextureImportStats stats = new TextureImportStats(1);
            foreach (KeyValuePair<string, ZipArchiveEntry> kv in entries)
            {
                if (!latest2lce_entities.TryGetValue(kv.Key, out JavaResourceConvertJson resourceConvertJson))
                    continue;

                Image texture = kv.Value.GetImage();
                texture = RemapTexture(texture, resourceConvertJson?.TextureRemap);
                Debug.WriteLine(resourceConvertJson.LceName);
                if (resourceConvertJson.LceName.StartsWith("item") && !items.ContainsKey(resourceConvertJson.LceName))
                    items.Add(resourceConvertJson.LceName, texture);
                if (resourceConvertJson.LceName.StartsWith("mob") && !mobs.ContainsKey(resourceConvertJson.LceName))
                    mobs.Add(resourceConvertJson.LceName, texture);
            }

            bool GetLargeChestTexture(string name, out Image texture)
            {
                if (entries.TryGetValue($"chest/{name}_right", out ZipArchiveEntry rightEntry) &&
                    entries.TryGetValue($"chest/{name}_left", out ZipArchiveEntry leftEntry))
                {
                    Image rightTexture = rightEntry.GetImage();
                    Image leftTexture = leftEntry.GetImage();

                    if (rightTexture.Size != leftTexture.Size)
                    {
                        texture = default;
                        return false;
                    }

                    Image res = new Bitmap(rightTexture.Width * 2, rightTexture.Height);
                    Graphics g = Graphics.FromImage(res);


                    Image GetArea(Rectangle area, RotateFlipType rotateFlipType, bool swap = false)
                    {
                        Image areaImage = rightTexture.GetArea(area).Combine(leftTexture.GetArea(area), ImageLayoutDirection.Horizontal);
                        if (swap)
                            areaImage = leftTexture.GetArea(area).Combine(rightTexture.GetArea(area), ImageLayoutDirection.Horizontal);
                        if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
                            areaImage.RotateFlip(rotateFlipType);
                        return areaImage;
                    }

                    {
                        {
                            Rectangle topArea = new Rectangle(new Point(29, 0), new Size(15, 14));
                            Image top = GetArea(topArea, RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(top, new Point(14, 0));
                        }

                        {
                            Rectangle bottomArea = new Rectangle(new Point(14, 0), new Size(15, 14));
                            Image bottom = GetArea(bottomArea, RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(bottom, new Point(44, 0));
                        }
                    }

                    {
                        {
                            Rectangle frontTopArea = new Rectangle(new Point(43, 14), new Size(15, 5));
                            Image frontTop = GetArea(frontTopArea, RotateFlipType.RotateNoneFlipY, true);
                            g.DrawImage(frontTop, new Point(14, 14));
                        }

                        {
                            Rectangle frontBottomArea = new Rectangle(new Point(43, 33), new Size(15, 10));
                            Image frontBottom = GetArea(frontBottomArea, RotateFlipType.RotateNoneFlipY, true);
                            g.DrawImage(frontBottom, new Point(14, 33));
                        }
                    }

                    {
                        {
                            Rectangle backTopArea = new Rectangle(new Point(14, 14), new Size(15, 5));
                            Image backTop = GetArea(backTopArea, RotateFlipType.RotateNoneFlipXY);
                            g.DrawImage(backTop, new Point(58, 14));
                        }

                        {
                            Rectangle backBottomArea = new Rectangle(new Point(14, 33), new Size(15, 10));
                            Image backBottom = GetArea(backBottomArea, RotateFlipType.RotateNoneFlipXY);
                            g.DrawImage(backBottom, new Point(58, 33));
                        }
                    }

                    {
                        Rectangle bottomTopArea = new Rectangle(new Point(29, 19), new Size(15, 14));
                        Image bottomTop = GetArea(bottomTopArea, RotateFlipType.RotateNoneFlipY);
                        g.DrawImage(bottomTop, new Point(14, 19));
                    }

                    {
                        Rectangle bottomTopArea = new Rectangle(new Point(14, 19), new Size(15, 14));
                        Image bottomTop = GetArea(bottomTopArea, RotateFlipType.RotateNoneFlipY);
                        g.DrawImage(bottomTop, new Point(44, 19));
                    }

                    // SIDES TOP
                    {
                        {
                            Image sideRight = rightTexture.GetArea(0, 14, 14, 5);
                            sideRight.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(sideRight, new Point(0, 14));
                        }

                        {
                            Image sideLeft = leftTexture.GetArea(29, 14, 14, 5);
                            sideLeft.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(sideLeft, new Point(44, 14));
                        }
                    }

                    // SIDES BOTTOM
                    {
                        {
                            Image sideRight = rightTexture.GetArea(0, 33, 14, 10);
                            sideRight.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(sideRight, new Point(0, 33));
                        }

                        {
                            Image sideLeft = leftTexture.GetArea(29, 33, 14, 10);
                            sideLeft.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            g.DrawImage(sideLeft, new Point(44, 33));
                        }
                    }

                    // NOSE
                    {
                        {
                            g.DrawImage(rightTexture.GetArea(0, 1, 1, 4), 0, 1);
                            g.DrawImage(GetArea(new Rectangle(1, 1, 1, 4), RotateFlipType.RotateNoneFlipNone), new Point(1, 1));
                        }

                        {
                            g.DrawImage(leftTexture.GetArea(2, 1, 1, 4), 3, 1);
                            g.DrawImage(GetArea(new Rectangle(3, 1, 1, 4), RotateFlipType.RotateNoneFlipNone), new Point(4, 1));
                        }

                        {
                            g.DrawImage(rightTexture.GetArea(1, 0, 1, 1), 1, 0);
                            g.DrawImage(leftTexture.GetArea(1, 0, 1, 1), 2, 0);

                            g.DrawImage(rightTexture.GetArea(2, 0, 1, 1), 3, 0);
                            g.DrawImage(leftTexture.GetArea(2, 0, 1, 1), 4, 0);
                        }
                    }

                    g.Dispose();

                    texture = res;
                    return true;
                }
                texture = null;
                return false;
            }

            if (_packMeta.Format > TARGET_FORMAT_VERSION)
            {
                if (!items.ContainsKey("item/largechest") && GetLargeChestTexture("normal", out Image texture))
                {
                    items.Add("item/largechest", texture);
                }
                if (!items.ContainsKey("item/trapped_double") && GetLargeChestTexture("trapped", out texture))
                {
                    items.Add("item/trapped_double", texture);
                }
            }

            return new ImportResult<(Dictionary<string, Image> mobs, Dictionary<string, Image> items), TextureImportStats>((mobs, items), stats);
        }

        private ConsoleArchive ImportUI()
        {
            _importStatus.Post($"[{nameof(ImportArmorSets)}] Importing: UI");

            IDictionary<string, ZipArchiveEntry> javaGui = GetEntries("gui", includeSubDirectories: true);

            ConsoleArchive mediaArc = new ConsoleArchive();

            FourjUserInterface skinHud = new FourjUIReader().FromStream(new MemoryStream(Resources.skinHud));


            FourjUserInterface skinGraphicsHud = new FourjUIReader().FromStream(new MemoryStream(Resources.skinGraphicsHud));
            FourjUserInterface skinGraphicsInGame = new FourjUIReader().FromStream(new MemoryStream(Resources.skinGraphicsInGame));

            FourjUserInterface skinPlatform = new FourjUIReader().FromStream(new MemoryStream(Resources.skinWiiU));
            // skinWiiU.fui
            if (
                javaGui.TryGetValue("title/background/panorama_0.png", out ZipArchiveEntry panorama0Entry) &&
                javaGui.TryGetValue("title/background/panorama_1.png", out ZipArchiveEntry panorama1Entry) &&
                javaGui.TryGetValue("title/background/panorama_2.png", out ZipArchiveEntry panorama2Entry) &&
                javaGui.TryGetValue("title/background/panorama_3.png", out ZipArchiveEntry panorama3Entry))
            {

                Image panorama0Texture = panorama0Entry.GetImage();
                Image panorama1Texture = panorama1Entry.GetImage();
                Image panorama2Texture = panorama2Entry.GetImage();
                Image panorama3Texture = panorama3Entry.GetImage();

                //! TODO: make more efficent !? -null
                Image panorama = new Image[] { panorama0Texture, panorama1Texture, panorama2Texture, panorama3Texture }.Combine(ImageLayoutDirection.Horizontal);
                panorama = panorama.Resize(new Size(820, 144), GraphicsConfig.PixelPerfect());
                skinPlatform.SetSymbol("Panorama_Background_S", panorama);
                skinPlatform.SetSymbol("Panorama_Background_N", panorama);

            }
            if (javaGui.TryGetValue("title/minecraft.png", out ZipArchiveEntry minecraftLogo))
                skinPlatform.SetSymbol("MenuTitle", minecraftLogo.GetImage());

            FuiTimeline hudTimeline = skinHud.GetNamedTimeline("fourj.FJ_Hud");

            // base size : 256x256
            if (javaGui.TryGetValue("widgets.png", out ZipArchiveEntry widgetsEntry))
            {
                Image widgetsTexture = widgetsEntry.GetImage();
                Size baseSize = new Size(256, 256);
                Size targetSize = widgetsTexture.Size;
                Size scale = new Size(Math.Max(targetSize.Width / baseSize.Width, 1), Math.Max(targetSize.Height / baseSize.Height, 1));
                Image hotbarItemBackTexture = widgetsTexture.GetArea(0, 0, 182, 22, scale);
                Image hotbarItemSelectedTexture = widgetsTexture.GetArea(0, 22, 24, 24, scale);
                Image hotbarOffhandSlotTexture = widgetsTexture.GetArea(24, 23, 22, 22, scale);
                skinGraphicsHud.SetSymbol("hotbar_item_back", hotbarItemBackTexture);
                skinGraphicsHud.SetSymbol("hotbar_item_selected", hotbarItemSelectedTexture);
                skinGraphicsHud.SetSymbol("hotbar_offhand_slot", hotbarOffhandSlotTexture);

                FuiTimelineEvent fuiTimelineEvent = hudTimeline.FindNamedEvent("HotBar");
                FuiTimeline eventTimelineHotbar = skinHud.GetEventTimeline(fuiTimelineEvent);
                // event 0 is a ref named "hotbar_item_back"
                eventTimelineHotbar.Frames[0].Events[0].Matrix = System.Numerics.Matrix3x2.CreateScale(3f / scale.Width, 3f / scale.Height);
                eventTimelineHotbar.FindNamedEvent("HotbarSelector").Matrix *= System.Numerics.Matrix3x2.CreateScale(1f / scale.Width, 1f / scale.Height);
            }

            // base size : 256x256
            if (javaGui.TryGetValue("icons.png", out ZipArchiveEntry iconsEntry))
            {
                Image iconsTexture = iconsEntry.GetImage();
                Size iconSize = new Size(9, 9);

                Size baseSize = new Size(256, 256);
                Size targetSize = iconsTexture.Size;
                Size scale = new Size(Math.Max(targetSize.Width / baseSize.Width, 1), Math.Max(targetSize.Height / baseSize.Height, 1));
                
                skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("FJ_ArmourBar")).Frames[0].Events[0].Matrix *= System.Numerics.Matrix3x2.CreateScale(1f / scale.Width, 1f / scale.Height);
                skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("ExpBar")).Frames[0].Events[0].Matrix *= System.Numerics.Matrix3x2.CreateScale(1f / scale.Width, 1f / scale.Height);
                skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("FJ_FoodBar")).Frames[0].Events[0].Matrix *= System.Numerics.Matrix3x2.CreateScale(1f / scale.Width, 1f / scale.Height);

                DebugEx.WriteLine(skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("FJ_ArmourBar")).Frames[0].Events);
                DebugEx.WriteLine(skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("ExpBar")).Frames[0].Events);
                DebugEx.WriteLine(skinHud.GetEventTimeline(hudTimeline.FindNamedEvent("FJ_FoodBar")).Frames[0].Events);
         
                skinGraphicsHud.SetSymbol("HUD_Crosshair", iconsTexture.GetArea(0, 0, 15, 15, scale));

                skinGraphicsHud.SetSymbol("experience_bar_empty", iconsTexture.GetArea(0, 64, 182, 5, scale));

                skinGraphicsHud.SetSymbol("experience_bar_full", iconsTexture.GetArea(0, 69, 182, 5, scale));

                skinGraphicsHud.SetSymbol("HorseJump_bar_empty", iconsTexture.GetArea(0, 84, 182, 5, scale));

                skinGraphicsHud.SetSymbol("HorseJump_bar_full", iconsTexture.GetArea(0, 89, 182, 5, scale));

                skinGraphicsHud.SetSymbol("Health_Background", iconsTexture.GetArea(new Point(16, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Background_Flash", iconsTexture.GetArea(new Point(25, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full", iconsTexture.GetArea(new Point(52, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half", iconsTexture.GetArea(new Point(61, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Flash", iconsTexture.GetArea(new Point(70, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Flash", iconsTexture.GetArea(new Point(79, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Poison", iconsTexture.GetArea(new Point(88, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Poison", iconsTexture.GetArea(new Point(97, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Poison_Flash", iconsTexture.GetArea(new Point(106, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Poison_Flash", iconsTexture.GetArea(new Point(115, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Wither", iconsTexture.GetArea(new Point(124, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Wither", iconsTexture.GetArea(new Point(133, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Wither_Flash", iconsTexture.GetArea(new Point(142, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Wither_Flash", iconsTexture.GetArea(new Point(151, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Full_Absorb", iconsTexture.GetArea(new Point(160, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("Health_Half_Absorb", iconsTexture.GetArea(new Point(169, 0), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Armour_Empty", iconsTexture.GetArea(new Point(16, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Armour_Half", iconsTexture.GetArea(new Point(25, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Armour_Full", iconsTexture.GetArea(new Point(34, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Air_Bubble", iconsTexture.GetArea(new Point(16, 18), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Air_Pop", iconsTexture.GetArea(new Point(25, 18), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Background", iconsTexture.GetArea(new Point(16, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Background_Flash", iconsTexture.GetArea(new Point(25, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Full", iconsTexture.GetArea(new Point(52, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Half", iconsTexture.GetArea(new Point(61, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Full_Flash", iconsTexture.GetArea(new Point(70, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Half_Flash", iconsTexture.GetArea(new Point(79, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Full_Poison", iconsTexture.GetArea(new Point(88, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Half_Poison", iconsTexture.GetArea(new Point(97, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Full_Poison_Flash", iconsTexture.GetArea(new Point(106, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Half_Poison_Flash", iconsTexture.GetArea(new Point(115, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HUD_Food_Background_Poison", iconsTexture.GetArea(new Point(133, 27), iconSize, scale));

                skinGraphicsHud.SetSymbol("HorseHealth_Full", iconsTexture.GetArea(new Point(88, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HorseHealth_Half", iconsTexture.GetArea(new Point(97, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HorseHealth_Full_Flash", iconsTexture.GetArea(new Point(106, 9), iconSize, scale));

                skinGraphicsHud.SetSymbol("HorseHealth_Half_Flash", iconsTexture.GetArea(new Point(115, 9), iconSize, scale));

            }

            if (javaGui.TryGetValue("sprites/hud/crosshair.png", out ZipArchiveEntry crosshairEntry))
                skinGraphicsHud.SetSymbol("HUD_Crosshair", crosshairEntry.GetImage());

            if (javaGui.TryGetValue("sprites/hud/hotbar.png", out ZipArchiveEntry hotbar))
                skinGraphicsHud.SetSymbol("hotbar_item_back", hotbar.GetImage());

            if (javaGui.TryGetValue("sprites/hud/hotbar_selection.png", out ZipArchiveEntry hotbar_selection))
                skinGraphicsHud.SetSymbol("hotbar_item_selected", hotbar_selection.GetImage());

            if (javaGui.TryGetValue("sprites/hud/hotbar_offhand_left.png", out ZipArchiveEntry hotbar_offhand_left))
                skinGraphicsHud.SetSymbol("hotbar_offhand_slot", hotbar_offhand_left.GetImage());

            if (javaGui.TryGetValue("sprites/hud/experience_bar_background.png", out ZipArchiveEntry experienceBarBackgroundEntry))
                skinGraphicsHud.SetSymbol("experience_bar_empty", experienceBarBackgroundEntry.GetImage());

            if (javaGui.TryGetValue("sprites/hud/experience_bar_progress.png", out ZipArchiveEntry experience_bar_progress))
                skinGraphicsHud.SetSymbol("experience_bar_full", experience_bar_progress.GetImage());

            if (javaGui.TryGetValue("sprites/hud/jump_bar_background.png", out ZipArchiveEntry jump_bar_background))
                skinGraphicsHud.SetSymbol("HorseJump_bar_empty", jump_bar_background.GetImage());

            if (javaGui.TryGetValue("sprites/hud/jump_bar_progress.png", out ZipArchiveEntry jump_bar_progress))
                skinGraphicsHud.SetSymbol("HorseJump_bar_full", jump_bar_progress.GetImage());

            if (javaGui.TryGetValue("sprites/hud/armor_empty.png", out ZipArchiveEntry armor_empty))
                skinGraphicsHud.SetSymbol("HUD_Armour_Empty", armor_empty.GetImage());

            if (javaGui.TryGetValue("sprites/hud/armor_half.png", out ZipArchiveEntry armor_half))
                skinGraphicsHud.SetSymbol("HUD_Armour_Half", armor_half.GetImage());

            if (javaGui.TryGetValue("sprites/hud/armor_full.png", out ZipArchiveEntry armor_full))
                skinGraphicsHud.SetSymbol("HUD_Armour_Full", armor_full.GetImage());

            if (javaGui.TryGetValue("sprites/hud/air.png", out ZipArchiveEntry air))
                skinGraphicsHud.SetSymbol("HUD_Air_Bubble", air.GetImage());

            if (javaGui.TryGetValue("sprites/hud/air_bursting.png", out ZipArchiveEntry air_bursting))
                skinGraphicsHud.SetSymbol("HUD_Air_Pop", air_bursting.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/container.png", out ZipArchiveEntry container))
                skinGraphicsHud.SetSymbol("Health_Background", container.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/container_blinking.png", out ZipArchiveEntry container_blinking))
                skinGraphicsHud.SetSymbol("Health_Background_Flash", container_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/full.png", out ZipArchiveEntry full))
                skinGraphicsHud.SetSymbol("Health_Full", full.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/full_blinking.png", out ZipArchiveEntry full_blinking))
                skinGraphicsHud.SetSymbol("Health_Full_Flash", full_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/half.png", out ZipArchiveEntry half))
                skinGraphicsHud.SetSymbol("Health_Half", half.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/half_blinking.png", out ZipArchiveEntry half_blinking))
                skinGraphicsHud.SetSymbol("Health_Half_Flash", half_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/poisoned_full.png", out ZipArchiveEntry poisoned_full))
                skinGraphicsHud.SetSymbol("Health_Full_Poison", poisoned_full.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/poisoned_half.png", out ZipArchiveEntry poisoned_half))
                skinGraphicsHud.SetSymbol("Health_Half_Poison", poisoned_half.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/poisoned_full_blinking.png", out ZipArchiveEntry poisoned_full_blinking))
                skinGraphicsHud.SetSymbol("Health_Full_Poison_Flash", poisoned_full_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/poisoned_half_blinking.png", out ZipArchiveEntry poisoned_half_blinking))
                skinGraphicsHud.SetSymbol("Health_Half_Poison_Flash", poisoned_half_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/withered_full.png", out ZipArchiveEntry withered_full))
                skinGraphicsHud.SetSymbol("Health_Full_Wither", withered_full.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/withered_half.png", out ZipArchiveEntry withered_half))
                skinGraphicsHud.SetSymbol("Health_Half_Wither", withered_half.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/withered_full_blinking.png", out ZipArchiveEntry withered_full_blinking))
                skinGraphicsHud.SetSymbol("Health_Full_Wither_Flash", withered_full_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/withered_half_blinking.png", out ZipArchiveEntry withered_half_blinking))
                skinGraphicsHud.SetSymbol("Health_Half_Wither_Flash", withered_half_blinking.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/absorbing_full.png", out ZipArchiveEntry absorbing_full))
                skinGraphicsHud.SetSymbol("Health_Full_Absorb", absorbing_full.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/absorbing_half.png", out ZipArchiveEntry absorbing_half))
                skinGraphicsHud.SetSymbol("Health_Half_Absorb", absorbing_half.GetImage());

            if (javaGui.TryGetValue("sprites/hud/food_empty.png", out ZipArchiveEntry food_empty))
            {
                Image food_emptyTexture = food_empty.GetImage();
                skinGraphicsHud.SetSymbol("HUD_Food_Background", food_emptyTexture);
                skinGraphicsHud.SetSymbol("HUD_Food_Background_Flash", food_emptyTexture);
            }

            if (javaGui.TryGetValue("sprites/hud/food_full.png", out ZipArchiveEntry food_full))
            {
                Image food_fullTexture = food_full.GetImage();
                skinGraphicsHud.SetSymbol("HUD_Food_Full", food_fullTexture);
                skinGraphicsHud.SetSymbol("HUD_Food_Full_Flash", food_fullTexture);
            }

            if (javaGui.TryGetValue("sprites/hud/food_half.png", out ZipArchiveEntry food_half))
            {
                Image food_halfTexture = food_half.GetImage();
                skinGraphicsHud.SetSymbol("HUD_Food_Half", food_halfTexture);
                skinGraphicsHud.SetSymbol("HUD_Food_Half_Flash", food_halfTexture);
            }
            if (javaGui.TryGetValue("sprites/hud/food_full_hunger.png", out ZipArchiveEntry food_full_hunger))
            {
                Image food_full_hungerTexture = food_full_hunger.GetImage();
                skinGraphicsHud.SetSymbol("HUD_Food_Full_Poison", food_full_hungerTexture);
                skinGraphicsHud.SetSymbol("HUD_Food_Full_Poison_Flash", food_full_hungerTexture);
            }

            if (javaGui.TryGetValue("sprites/hud/food_half_hunger.png", out ZipArchiveEntry food_half_hunger))
            {
                Image food_half_hungerTexture = food_half_hunger.GetImage();
                skinGraphicsHud.SetSymbol("HUD_Food_Half_Poison", food_half_hungerTexture);
                skinGraphicsHud.SetSymbol("HUD_Food_Half_Poison_Flash", food_half_hungerTexture);
            }


            if (javaGui.TryGetValue("sprites/hud/food_empty_hunger.png", out ZipArchiveEntry food_empty_hunger))
                skinGraphicsHud.SetSymbol("HUD_Food_Background_Poison", food_empty_hunger.GetImage());

            if (javaGui.TryGetValue("sprites/hud/heart/vehicle_full.png", out ZipArchiveEntry vehicle_full))
            {
                Image vehicle_fullfTexture = vehicle_full.GetImage();
                skinGraphicsHud.SetSymbol("HorseHealth_Full", vehicle_fullfTexture);
                skinGraphicsHud.SetSymbol("HorseHealth_Full_Flash", vehicle_fullfTexture);
            }
            if (javaGui.TryGetValue("sprites/hud/heart/vehicle_half.png", out ZipArchiveEntry vehicle_half))
            {
                Image vehicle_halfTexture = vehicle_half.GetImage();
                skinGraphicsHud.SetSymbol("HorseHealth_Half", vehicle_halfTexture);
                skinGraphicsHud.SetSymbol("HorseHealth_Half_Flash", vehicle_halfTexture);
            }

            mediaArc.Add("skinHud.fui", new FourjUIWriter(skinHud));
            mediaArc.Add("skinWiiU.fui", new FourjUIWriter(skinPlatform));
            mediaArc.Add("skinGraphicsHud.fui", new FourjUIWriter(skinGraphicsHud));

            return mediaArc;
        }
    }
}