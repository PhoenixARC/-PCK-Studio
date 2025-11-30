using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PckStudio.Core.DLC;
using PckStudio.Core.Extensions;
using PckStudio.Core.Properties;

namespace PckStudio.Core.IO.Java
{
    public class ResourcePackImporter
    {
        internal const int TARGET_FORMAT_VERSION = 4;


        static readonly IReadOnlyDictionary<int, string> _formatToVersion = new Dictionary<int, string>()
        {
            [1] = "1.6.1 – 1.8.9",
            [2] = "1.9 – 1.10.2",
            [3] = "1.11 – 1.12.2",

            // ----- TARGET ----- //
            [4] = "1.13 – 1.14.4",
            // ------------------ //
            
            [5] = "1.15 – 1.16.1",
            [6] = "1.16.2 – 1.16.5",
            [7] = "1.17 – 1.17.1",
            [8] = "1.18 – 1.18.2",
            [9] = "1.19 – 1.19.2",
            [12] = "1.19.3",
            [13] = "1.19.4",
            [15] = "1.20 – 1.20.1",
            [18] = "1.20.2",
            [22] = "1.20.3 – 1.20.4",
            [32] = "1.20.5 – 1.20.6",
            [34] = "1.21 – 1.21.1",
            [42] = "1.21.2 – 1.21.3",
            [46] = "1.21.4",
            [55] = "1.21.5",
            [63] = "1.21.6",
            [64] = "1.21.7 – 1.21.8",
            [69] = "1.21.9 – 1.21.10",
        };

        public static DLCTexturePackage ImportTexturePack(string zipfilepath, LCEGameVersion gameVersion = default) => ImportTexturePack(new FileInfo(zipfilepath), gameVersion);
        public static DLCTexturePackage ImportTexturePack(FileInfo zipfile, LCEGameVersion gameVersion = default) => ImportTexturePack(new ZipArchive(zipfile.OpenRead()), gameVersion);
        public static DLCTexturePackage ImportTexturePack(ZipArchive zip, LCEGameVersion gameVersion = default)
        {
            StreamReader packmeta = new StreamReader(zip.GetEntry("pack.mcmeta").Open());
            int format = JsonConvert.DeserializeObject<McPackmeta>(packmeta.ReadToEnd()).Pack.Format;
            Debug.WriteLine($"Pack format: {format}");
            throw new NotImplementedException();
        }

        public static ImportResult<Atlas> ImportAtlas(ZipArchive zip, AtlasResource atlasResource, LCEGameVersion gameVersion = default)
        {
            StreamReader packmeta = new StreamReader(zip.GetEntry("pack.mcmeta").Open());
            int format = JsonConvert.DeserializeObject<McPackmeta>(packmeta.ReadToEnd()).Pack.Format;
            Debug.WriteLineIf(format == TARGET_FORMAT_VERSION, "Target format version... less work?");
            Debug.WriteLine($"Importing textures from resource pack of version: {GetVersionFromFormat(format)}(Format:{format})");

            Atlas atlas = Atlas.CreateDefault(atlasResource, gameVersion);
            string atlasPath = GetAtlasPathFromFormat(format, atlasResource.Type);
            string path = $"assets/minecraft/textures/{atlasPath}/";

            IReadOnlyDictionary<string, string> lookUpTable = GetVersionLookUpTable(format, atlasResource.Type);

            IReadOnlyDictionary<string, int> map =
                atlasResource.TilesInfo.enumerate()
                .ToDictionary(tileInfo => string.IsNullOrEmpty(tileInfo.value.InternalName) ? $"{Guid.NewGuid()}.{tileInfo.index}" : tileInfo.value.InternalName, it => it.index);

            IEnumerable<ZipArchiveEntry> entries = zip.Entries.Where(e => e.FullName.StartsWith(path) && !e.FullName.EndsWith("/"));

            IReadOnlyDictionary<string, ZipArchiveEntry> javaAnimations = entries.Where(e => e.FullName.EndsWith(".mcmeta")).ToDictionary(entry => entry.FullName);

            ImportResult<Atlas> result = new ImportResult<Atlas>(atlas, atlas.TileCount);
            Size maxTileSize = Size.Empty;
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
                if (javaAnimations.ContainsKey(t.FullName + ".mcmeta"))
                    img = img.GetArea(new Rectangle(Point.Empty, new Size(img.Width, img.Width)));

                if (img.Size.Width > maxTileSize.Width)
                    maxTileSize = new Size(img.Size.Width, img.Size.Width);
                atlas[i].Texture = img;
                result.SetMarked(i);
            }
            atlas.SetTileSize(maxTileSize);
            return result;
        }

        static readonly IReadOnlyDictionary<string, string> latest2lce_blocks = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.latest2lce_blocks);
        static readonly IReadOnlyDictionary<string, string> latest2lce_items = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.latest2lce_items);
        private static IReadOnlyDictionary<string, string> GetVersionLookUpTable(int format, AtlasResource.AtlasType atlasType)
        {
            _ = format;
            return atlasType switch
            {
                AtlasResource.AtlasType.BlockAtlas => latest2lce_blocks,
                AtlasResource.AtlasType.ItemAtlas => latest2lce_items,
                _ => throw new Exception()
            };
        }

        private static string GetVersionFromFormat(int format) => _formatToVersion.TryGetValue(format, out string version) ? version : "unknown";

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

        private static int GetColumnsFromGameVersion(LCEGameVersion gameVersion, AtlasResource.AtlasType atlasType)
        {
            return gameVersion switch
            {
                LCEGameVersion._1_13 when atlasType == AtlasResource.AtlasType.BlockAtlas => 34,
                LCEGameVersion._1_13 when atlasType == AtlasResource.AtlasType.ItemAtlas => 17,


                LCEGameVersion._1_14 when atlasType == AtlasResource.AtlasType.BlockAtlas => 39,
                LCEGameVersion._1_14 when atlasType == AtlasResource.AtlasType.ItemAtlas => 18,
                _ when atlasType == AtlasResource.AtlasType.PaintingAtlas => 16,
                _ => throw new ArgumentException(nameof(gameVersion))
            };
        }
    }
}
