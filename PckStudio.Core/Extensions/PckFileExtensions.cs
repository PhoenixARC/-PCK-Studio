using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        {
            PckAsset asset = new PckAsset(assetPath, PckAssetType.TextureFile);
            asset.SetTexture(texture);
            pck.AddAsset(asset);
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
