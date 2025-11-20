using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core
{
    public sealed class ArmorPieceDescription
    {
        private static readonly Dictionary<string, ArmorPieceDescription> _pathLookUp = new Dictionary<string, ArmorPieceDescription>();
        private static readonly string _resourcePath = ResourceLocations.GetFromCategory(ResourceCategory.ArmorTextures).FullPath;
        public static ArmorPieceDescription Empty = new ArmorPieceDescription(string.Empty, 0, 0);
        public static ArmorPieceDescription Leather = new ArmorPieceDescription("cloth", layerCount: 1);
        public static ArmorPieceDescription Chain = new ArmorPieceDescription("chain");
        public static ArmorPieceDescription Iron = new ArmorPieceDescription("iron");
        public static ArmorPieceDescription Gold = new ArmorPieceDescription("gold");
        public static ArmorPieceDescription Diamond = new ArmorPieceDescription("diamond");
        public static ArmorPieceDescription Turtle = new ArmorPieceDescription("turtle", textureCount: 1);


        public string Name { get; }
        public int LayerCount { get; }
        public int TextureCount { get; }
        public int AssetCount { get; }

        private ArmorPieceDescription(string name, int layerCount = 0, int textureCount = 2)
        {
            Name = name;
            if (string.IsNullOrEmpty(name) && layerCount == 0 && textureCount == 0)
                return;
            LayerCount = Math.Min(Math.Max(layerCount, 0), ('z' - 'b'));
            TextureCount = Math.Max(textureCount, 1);
            AssetCount = (LayerCount + 1) * TextureCount;
            foreach (var path in GetAssetPaths())
            {
                _pathLookUp.Add(path, this);
            }
        }

        public static ArmorPieceDescription GetFromAssetName(string name)
        {
            return _pathLookUp.ContainsKey(name ?? string.Empty) ? _pathLookUp[name] : Empty;
        }

        public string[] GetAssetPaths()
        {
            if (AssetCount <= 0)
                return Array.Empty<string>();
            string[] result = new string[AssetCount];
            for (int i = 0; i < TextureCount; i++)
            {
                string assetPath = Path.Combine($"{_resourcePath}/", $"{Name}_{i + 1}");
                result[i*(LayerCount+1)] = assetPath;
                for (int j = 0; j < LayerCount; j++)
                {
                    result[i * TextureCount + 1 + j] = $"{assetPath}_{Convert.ToChar('b' + j)}";
                }
            }
            return result;
        }
    }
}
