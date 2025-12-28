using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers;
using PckStudio.Core.Skin;

namespace PckStudio.Core.Extensions
{
    public static class PckFileExtensions
    {
        public static PckAsset AddTexture(this PckFile pck, string assetPath, Image texture)
            => pck.AddTexture(assetPath, texture, 0);
        public static PckAsset AddTexture(this PckFile pck, string assetPath, Image texture, int mipMapCount)
        {
            if (string.IsNullOrEmpty(assetPath) || texture is null)
                return default;

            PckAsset asset = pck.CreateNewAsset(assetPath, PckAssetType.TextureFile);
            asset.SetTexture(texture);
            string textureExtension = Path.GetExtension(assetPath);
            string dir = Path.GetDirectoryName(assetPath);
            string name = Path.GetFileNameWithoutExtension(assetPath);
            for (int i = 0; i < mipMapCount; i++)
            {
                int mipMapLevel = i + 2;
                string mippedPath = $"{dir}/{name}{PckAssetExtensions.MIPMAP_LEVEL}{mipMapLevel}{textureExtension}";
                Debug.WriteLine(mippedPath);
                if (pck.HasAsset(mippedPath, PckAssetType.TextureFile))
                    pck.RemoveAsset(pck.GetAsset(mippedPath, PckAssetType.TextureFile));
                Image mippedTexture = texture.CreateMipMap(mipMapLevel);

                PckAsset mipMappedAsset = pck.CreateNewAsset(mippedPath, PckAssetType.TextureFile);
                mipMappedAsset.SetTexture(mippedTexture);
            }
            return asset;
        }

        public static PckAsset AddSkin(this PckFile pck, string assetPathPrefix, Skin.Skin skin, LOCFile localisation)
        {
            string skinId = skin.Identifier.ToString("d08");

            PckAsset asset = new PckAsset(Path.Combine(assetPathPrefix, $"dlcskin{skinId}.png"), PckAssetType.SkinFile);

            asset.SetTexture(skin.Texture);

            asset.AddProperty("DISPLAYNAME", skin.MetaData.Name);

            bool canLocalize = localisation is not null;
            if (canLocalize)
            {
                string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
                asset.AddProperty("DISPLAYNAMEID", skinLocKey);
                localisation.SetLocEntry(skinLocKey, skin.MetaData.Name);
            }

            if (!string.IsNullOrEmpty(skin.MetaData.Theme))
            {
                asset.AddProperty("THEMENAME", skin.MetaData.Theme);

                if (canLocalize)
                {
                    string skinThemeLocKey = $"IDS_dlcskin{skinId}_THEMENAME";
                    asset.AddProperty("THEMENAMEID", skinThemeLocKey);
                    localisation.SetLocEntry(skinThemeLocKey, skin.MetaData.Theme);
                }
            }

            if (skin.HasCape)
            {
                asset.AddProperty("CAPEPATH", $"dlccape{skin.CapeId}.png");
            }

            asset.AddProperty("ANIM", skin.Anim);
            asset.AddProperty("GAME_FLAGS", "0x18");
            asset.AddProperty("FREE", "1");

            foreach (SkinBOX box in skin.Model.AdditionalBoxes)
            {
                asset.AddProperty(box.ToProperty());
            }
            foreach (SkinPartOffset offset in skin.Model.PartOffsets)
            {
                asset.AddProperty(offset.ToProperty());
            }

            pck.AddAsset(asset);
            return asset;
        }

        public static PckAsset CreateNewAssetIf(this PckFile pck, bool condition, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            if (condition)
            {
                return pck.CreateNewAsset(filename, filetype, writer);
            }
            return default;
        }

        public static PckAsset CreateNewAsset(this PckFile pck, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            PckAsset asset = pck.CreateNewAsset(filename, filetype);
            asset.SetData(writer);
            return asset;
        }

        public static bool HasDirectory(this PckFile pck, string directoryPath)
        {
            string sanitisedDirectoryPath = directoryPath.Replace('\\', '/');
            return pck.GetAssets().Any(asset => asset.Filename.StartsWith(sanitisedDirectoryPath));
        }

        public static IEnumerable<PckAsset> GetDirectoryContent(this PckFile pck, string directoryPath, PckAssetType assetType, bool includeSubDirectories = false)
            => GetDirectoryContent(pck, directoryPath, includeSubDirectories).Where(asset => asset.Type == assetType);
        public static IEnumerable<PckAsset> GetDirectoryContent(this PckFile pck, string directoryPath, bool includeSubDirectories = false)
        {
            string sanitisedDirectoryPath = directoryPath.Replace('\\', '/');
            if (!sanitisedDirectoryPath.EndsWith("/"))
                sanitisedDirectoryPath += "/";
            return pck.GetAssets()
                .Where(asset => asset.Filename.StartsWith(sanitisedDirectoryPath))
                .Where(asset => includeSubDirectories || !asset.Filename.Substring(sanitisedDirectoryPath.Length).Contains('/'));
        }

    }
}
