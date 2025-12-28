using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private const string JAVA_RESOURCE_PACK_PATH = "assets/minecraft/";
        private const string JAVA_TEXTURES_PATH = "textures";

        static readonly IReadOnlyDictionary<int, IVersion> _formatVeriosnToGameVersion = new Dictionary<int, IVersion>()
        {
            [1] = new VersionRange("1.6.1", "1.8.9"),
            [2] = new VersionRange("1.9", "1.10.2"),
            [3] = new VersionRange("1.11", "1.12.2"),

            // ----- TARGET ----- //
            [4] = new VersionRange("1.13", "1.14.4"),
            // ------------------ //

            [5] = new VersionRange("1.15", "1.16.1"),
            [6] = new VersionRange("1.16.2", "1.16.5"),
            [7] = new VersionRange("1.17", "1.17.1"),
            [8] = new VersionRange("1.18", "1.18.2"),
            [9] = new VersionRange("1.19", "1.19.2"),
            [12] = new SingleVersion("1.19.3"),
            [13] = new SingleVersion("1.19.4"),
            [15] = new VersionRange("1.20", "1.20.1"),
            [18] = new SingleVersion("1.20.2"),
            [22] = new VersionRange("1.20.3", "1.20.4"),
            [32] = new VersionRange("1.20.5", "1.20.6"),
            [34] = new VersionRange("1.21", "1.21.1"),
            [42] = new VersionRange("1.21.2", "1.21.3"),
            [46] = new SingleVersion("1.21.4"),
            [55] = new SingleVersion("1.21.5"),
            [63] = new SingleVersion("1.21.6"),
            [64] = new VersionRange("1.21.7", "1.21.8"),
            [69] = new VersionRange("1.21.9", "1.21.10"),
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
        private McPackmeta.McPack _packMeta;
        private Dictionary<string, ZipArchiveEntry> _resoursePackData;

        public ResourcePackImporter(LCEGameVersion gameVersion)
        {
            _gameVersion = gameVersion;
        }

        public static bool IsJavaResourcePack(ZipArchive zip) => zip.GetEntry("pack.mcmeta") is not null;

        public bool StartImport(string name, ZipArchive zip, ImportStatusReport importStatus)
        {
            if (!IsJavaResourcePack(zip))
            {
                MessageBoxEx.ShowError("Zip file is not a resource pack", "Import Error");
                return false;
            }
            _name = name;
            _importStatus = importStatus;
            _packMeta = ReadPackMeta(zip);
            string path = Path.Combine(JAVA_RESOURCE_PACK_PATH, JAVA_TEXTURES_PATH);
            _resoursePackData = zip.GetDirectoryContent(path, includeSubDirectories: true).ToDictionary(e => e.FullName.Substring(path.Length + 1));
            return true;
        }

        public McPackmeta.McPack ReadPackMeta(ZipArchive zip)
        {
            StreamReader packmeta = new StreamReader(zip.GetEntry("pack.mcmeta").Open());
            McPackmeta.McPack pack = JsonConvert.DeserializeObject<McPackmeta>(packmeta.ReadToEnd()).Pack;
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

            DLCTexturePackage.MetaData metaData = new DLCTexturePackage.MetaData(null, _packMeta.Icon);

            int id = new Random().Next(0, GameConstants.MAX_PACK_ID - 1);

            TextureImportStats stats = block.Stats + item.Stats + particles.Stats + envData.Stats + armorSets.Stats + entityModelTextures.Stats;

            string name = ConvertJavaTextFormatToLCE(_name.Replace("_", " "));
            string description = ConvertJavaTextFormatToLCE(_packMeta.Description);

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
                    moon: null, parentPackage: null);
            return new ImportResult<DLCTexturePackage, TextureImportStats>(dlcTexturePackage, stats);
        }

        private string ConvertJavaTextFormatToLCE(string text)
        {
            string[] sections = text.Split(['§'], StringSplitOptions.RemoveEmptyEntries);
            if (sections.Length == 0 || sections.Length == 1)
                return text;
            string formatText = string.Join("", sections
                .Select(s => {
                    if (string.IsNullOrWhiteSpace(s) || !(s.Length > 1))
                        return s;
                    string colorFormat = "§" + s[0];
                    string colorText = s.Substring(1);
                    if (JavaConstants.JavaColorCodeToColor.TryGetValue(colorFormat, out (Color foreground, Color background) color))
                    {
                        string htmlColor = color.foreground.ToHTMLColor();
                        if (colorText.EndsWith("\n"))
                            return $"<font color=\"{htmlColor}\">{colorText.Substring(0, colorText.Length - 1)}</font>\n";
                        return $"<font color=\"{htmlColor}\">{colorText}</font>";
                    }
                    return s;
                }
                ));
            return formatText;
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
            _importStatus.Post($"[{nameof(ImportAtlas)}] Importing: '{atlasResource.Path}'");
            Atlas atlas = Atlas.CreateDefault(atlasResource, _gameVersion);
            string path = GetAtlasPathFromFormat(_packMeta.Format, atlasResource.Type);
            IDictionary<string, ZipArchiveEntry> entries = GetEntries(path);

            IReadOnlyDictionary<string, JavaResourceConvertJson> lookUpTable = GetVersionLookUpTable(atlasResource.Type);

            IEnumerable<(int index, JsonTileInfo value)> a = atlasResource.TilesInfo.Enumerate().GroupBy(it => it.value.InternalName).Select(grp => grp.FirstOrDefault());
            IReadOnlyDictionary<string, int> map = a.ToDictionary(tileInfo => string.IsNullOrEmpty(tileInfo.value.InternalName) ? $"{Guid.NewGuid()}.{tileInfo.index}" : tileInfo.value.InternalName.ToLowerInvariant(), it => it.index);

            IReadOnlyDictionary<string, ZipArchiveEntry> javaAnimations = entries.Values.Where(e => e.FullName.EndsWith(".mcmeta")).ToDictionary(entry => entry.FullName);

            TextureImportStats stats = new TextureImportStats(atlasResource.TilesInfo.Where(t => !string.IsNullOrWhiteSpace(t.InternalName)).Count());
            IDictionary<string, Animation> animations = new Dictionary<string, Animation>();
            foreach (ZipArchiveEntry entry in entries.Values)
            {
                _importStatus.Post(entry.FullName);
                if (!entry.FullName.EndsWith(".png"))
                    continue;
                string name = Path.GetFileNameWithoutExtension(entry.FullName);
                if (!map.TryGetValue(name, out int index) && !(lookUpTable.TryGetValue(name, out JavaResourceConvertJson lceKey) && map.TryGetValue(lceKey.LceName.ToLowerInvariant(), out index)) || !index.IsWithinRangeOf(0, atlas.TileCount - 1))
                    continue;

                JsonTileInfo tileInfo = atlas[index]?.GetUserDataOfType<JsonTileInfo>();

                Image img = entry.GetImage();
                bool hasMcMeta = javaAnimations.TryGetValue(entry.FullName + ".mcmeta", out ZipArchiveEntry archiveEntry);
                if (hasMcMeta)
                {
                    string jsonData = archiveEntry.ReadAllText();
                    Debug.WriteLine(jsonData);
                    JObject mcMetaJson = JObject.Parse(jsonData);
                    string animationName = tileInfo?.InternalName ?? name;
                    if (mcMetaJson["animation"] != null && !animations.ContainsKey(animationName))
                    {
                        Animation animation = AnimationDeserializer.DefaultDeserializer.DeserializeJavaAnimation(mcMetaJson, img);
                        if (animation.FrameCount > 0)
                            animations.Add(animationName, animation);
                        stats.animations++;
                        img = img.GetArea(new Rectangle(Point.Empty, new Size(img.Width, img.Width)));
                        stats.textures++;
                    }
                }
                atlas[index].Texture = img;
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
                xSclar = source.Width / textureRemap.SourceSize.Width;
                ySclar = source.Height / textureRemap.SourceSize.Height;
                s = textureRemap.TargetSize;
            }
            Image res = new Bitmap(s.Width * xSclar, s.Height* ySclar);

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
                    if (remapArea.Rotation == -90f)
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

        private static string GetJavaGameVersionFromResourcePackFormat(int format) => _formatVeriosnToGameVersion.TryGetValue(format, out IVersion versionRange) ? versionRange.ToString(" - ") : "unknown";

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

        private ImportResult<(Dictionary<string, Image> mobs, Dictionary<string, Image> items), TextureImportStats> ImportEntityModels()
        {
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

            if (_packMeta.Format > 5)
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
    }
}