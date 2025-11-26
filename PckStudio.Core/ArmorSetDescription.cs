using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Core.Extensions;
using PckStudio.Interfaces;

namespace PckStudio.Core
{
    public sealed class ArmorSetDescription
    {
        private static readonly Dictionary<string, ArmorSetDescription> _pathLookUp = new Dictionary<string, ArmorSetDescription>();
        private static readonly string _resourcePath = ResourceLocations.GetFromCategory(ResourceCategory.ArmorTextures).FullPath;
        public static ArmorSetDescription Empty = new ArmorSetDescription(string.Empty, 0, 0);

        public const string CLOTH = "cloth";
        public const string CHAIN = "chain";
        public const string IRON = "iron";
        public const string GOLD = "gold";
        public const string DIAMOND = "diamond";
        public const string TURTLE = "turtle";

        public static ArmorSetDescription Leather = new ArmorSetDescription(CLOTH, layerCount: 1);
        public static ArmorSetDescription Chain = new ArmorSetDescription(CHAIN);
        public static ArmorSetDescription Iron = new ArmorSetDescription(IRON);
        public static ArmorSetDescription Gold = new ArmorSetDescription(GOLD);
        public static ArmorSetDescription Diamond = new ArmorSetDescription(DIAMOND);
        public static ArmorSetDescription Turtle = new ArmorSetDescription(TURTLE, textureCount: 1);


        public string Name { get; }
        public int LayerCount { get; }
        public int TextureCount { get; }
        public int AssetCount { get; }
        public bool IsEmpty => this == Empty;

        private ArmorSetDescription(string name, int layerCount = 0, int textureCount = 2)
        {
            Name = name;
            if (string.IsNullOrEmpty(name) && layerCount == 0 && textureCount == 0)
                return;
            LayerCount = Math.Min(Math.Max(layerCount, 0), ('z' - 'b'));
            TextureCount = Math.Max(textureCount, 1);
            AssetCount = (LayerCount + 1) * TextureCount;
            foreach (var path in GetArmorNames())
            {
                _pathLookUp.Add(path, this);
            }
        }

        public static ArmorSetDescription GetFromAssetName(string name)
        {
            return _pathLookUp.ContainsKey(name ?? string.Empty) ? _pathLookUp[name] : Empty;
        }

        public string[] GetAssetPaths() => GetArmorNames().Select(name => Path.Combine($"{_resourcePath}/", name)).ToArray();

        public string[] GetArmorNames()
        {
            if (AssetCount <= 0)
                return Array.Empty<string>();
            string[] result = new string[AssetCount];
            for (int i = 0; i < TextureCount; i++)
            {
                string assetPath = $"{Name}_{i + 1}";
                result[i * (LayerCount + 1)] = assetPath;
                for (int j = 0; j < LayerCount; j++)
                {
                    result[i * TextureCount + 1 + j] = $"{assetPath}_{Convert.ToChar('b' + j)}";
                }
            }
            return result;
        }

        public ArmorSet GetArmorSet(ITryGet<string, Image> tryGetTexture)
        {
            Image baseTexture = default;
            Image overlayTexture = default;
            string[] assetPaths = GetAssetPaths();
            if (TextureCount < 2 && tryGetTexture.TryGet(assetPaths[0], out Image t0))
            {
                return new ArmorSet(Name, t0.Combine(t0, ImageLayoutDirection.Vertical), overlayTexture);
            }
            for (int i = 0; i < TextureCount / 2; i++)
            {
                if (tryGetTexture.TryGet(assetPaths[i + 0], out Image t1) &&
                    tryGetTexture.TryGet(assetPaths[i + 1], out Image t2))
                {
                    baseTexture = t1.Combine(t2, ImageLayoutDirection.Vertical);
                }
            }
            if (LayerCount > 0 &&
                tryGetTexture.TryGet(assetPaths[2], out Image t3) &&
                tryGetTexture.TryGet(assetPaths[3], out Image t4))
            {
                overlayTexture = t3.Combine(t4, ImageLayoutDirection.Vertical);
            }
            return new ArmorSet(Name, baseTexture, overlayTexture);
        }
    }
}
