using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PckStudio.Core.Deserializer;
using PckStudio.Core.DLC;
using PckStudio.Core.Extensions;
using PckStudio.Core.Properties;

namespace PckStudio.Core.IO.Java
{
    public class ResourcePackImporter
    {
        internal const int TARGET_FORMAT_VERSION = 4;
        internal const string JAVA_RESOURCE_PACK_PATH = "assets/minecraft/textures";

        static readonly IReadOnlyDictionary<int, IVersion> _formatToVersion = new Dictionary<int, IVersion>()
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

        public class ImportStats(int maxTextures)
        {
            public int Animations => animations;
            public int Textures => textures;
            public int MissingTextures => _maxTextures - textures;
            public int MaxTextures => _maxTextures;

            internal int animations = 0;
            internal int textures = 0;
            private int _maxTextures = maxTextures;
        }

        LCEGameVersion _gameVersion;
        ZipArchive _zip;
        McPackmeta.McPack packMeta;

        public ResourcePackImporter(ZipArchive zip, LCEGameVersion gameVersion)
        {
            _zip = zip;
            _gameVersion = gameVersion;
            packMeta = ReadPackMeta(zip);
        }

        public static bool IsJavaResourcePack(ZipArchive zip) => zip.GetEntry("pack.mcmeta") is ZipArchiveEntry;

        public McPackmeta.McPack ReadPackMeta(ZipArchive zip)
        {
            StreamReader packmeta = new StreamReader(zip.GetEntry("pack.mcmeta").Open());
            McPackmeta.McPack pack = JsonConvert.DeserializeObject<McPackmeta>(packmeta.ReadToEnd()).Pack;
            Debug.WriteLineIf(pack.Format == TARGET_FORMAT_VERSION, "Target format version... less work?");
            Debug.WriteLine($"Importing textures from resource pack of version: {GetVersionFromFormat(pack.Format)}(Format:{pack.Format})");
            return pack;
        }

        public DLCTexturePackage ImportAsTexturePack(ZipArchive zip)
        {
            ImportResult<Atlas, ImportStats> blocks = ImportAtlas(ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.BlockAtlas)) as AtlasResource);
            ImportResult<Atlas, ImportStats> items = ImportAtlas(ResourceLocations.GetFromCategory(AtlasResource.GetId(AtlasResource.AtlasType.ItemAtlas)) as AtlasResource);

            return null;
        }

        public ImportResult<Atlas, ImportStats> ImportAtlas(AtlasResource atlasResource)
        {
            Atlas atlas = Atlas.CreateDefault(atlasResource, _gameVersion);
            string atlasPath = GetAtlasPathFromFormat(packMeta.Format, atlasResource.Type);
            string path = Path.Combine(JAVA_RESOURCE_PACK_PATH, atlasPath).Replace('\\', '/');

            IReadOnlyDictionary<string, string> lookUpTable = GetVersionLookUpTable(atlasResource.Type);

            IReadOnlyDictionary<string, int> map =
                atlasResource.TilesInfo.enumerate()
                .ToDictionary(tileInfo => string.IsNullOrEmpty(tileInfo.value.InternalName) ? $"{Guid.NewGuid()}.{tileInfo.index}" : tileInfo.value.InternalName, it => it.index);

            IEnumerable<ZipArchiveEntry> entries = _zip.Entries.Where(e => e.FullName.StartsWith(path) && !e.FullName.EndsWith("/"));

            IReadOnlyDictionary<string, ZipArchiveEntry> javaAnimations = entries.Where(e => e.FullName.EndsWith(".mcmeta")).ToDictionary(entry => entry.FullName);

            int maxWidth = 0;
            ImportStats stats = new ImportStats(atlas.TileCount);
            IDictionary<string, Animation> animations = new Dictionary<string, Animation>();
            foreach (ZipArchiveEntry t in entries)
            {
                if (!t.FullName.EndsWith(".png"))
                    continue;
                string name = Path.GetFileNameWithoutExtension(t.FullName);
                if (!map.TryGetValue(name, out int i) && !(lookUpTable.TryGetValue(name, out string lceKey) && map.TryGetValue(lceKey, out i)))
                    continue;

                if (i >= atlas.TileCount)
                    continue;

                Image img = Image.FromStream(t.Open());
                bool isAnimation = false;
                if ((isAnimation = javaAnimations.TryGetValue(t.FullName + ".mcmeta", out ZipArchiveEntry animationEntry)))
                {
                    string jsonData = animationEntry.ReadAllText();
                    animations.Add(name, AnimationDeserializer.DefaultDeserializer.DeserializeJavaAnimation(JObject.Parse(jsonData), img));
                    stats.animations++;
                    img = img.GetArea(new Rectangle(Point.Empty, new Size(img.Width, img.Width)));
                }

                if (img.Width > maxWidth)
                    maxWidth = img.Width;
                atlas[i].Texture = img;
                stats.textures++;
            }
            atlas.SetTileSize(new Size(maxWidth, maxWidth));
            ImportResult<Atlas, ImportStats> result = new ImportResult<Atlas, ImportStats>(atlas, stats);
            Debug.WriteLine("Import Stats");
            Debug.WriteLine($"Textures: {stats.Textures}/{stats.MaxTextures}({stats.MissingTextures} missing)");
            Debug.WriteLine($"Animations: {stats.Animations}");
            foreach (string item in animations.Keys)
            {
                Debug.WriteLine(item);
            }
            return result;
        }

        static readonly IReadOnlyDictionary<string, string> latest2lce_blocks = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.latest2lce_blocks);
        static readonly IReadOnlyDictionary<string, string> latest2lce_items = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.latest2lce_items);
        private static IReadOnlyDictionary<string, string> GetVersionLookUpTable(AtlasResource.AtlasType atlasType)
        {
            return atlasType switch
            {
                AtlasResource.AtlasType.BlockAtlas => latest2lce_blocks,
                AtlasResource.AtlasType.ItemAtlas => latest2lce_items,
                _ => throw new Exception()
            };
        }

        private static string GetVersionFromFormat(int format) => _formatToVersion.TryGetValue(format, out IVersion versionRange) ? versionRange.ToString(" - ") : "unknown";

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
    }
}
