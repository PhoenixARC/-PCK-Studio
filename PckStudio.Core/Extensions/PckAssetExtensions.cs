using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers;

using PckStudio.Interfaces;
using PckStudio.Core.Deserializer;
using PckStudio.Core.Serializer;
using PckStudio.Core.Skin;

namespace PckStudio.Core.Extensions
{
    public static class PckAssetExtensions
    {
        private const string MIPMAP_LEVEL = "MipMapLevel";

        public static Image GetTexture(this PckAsset asset)
        {
            if (asset.Type != PckAssetType.SkinFile &&
                asset.Type != PckAssetType.CapeFile &&
                asset.Type != PckAssetType.TextureFile)
            {
                throw new Exception("Asset is not suitable to contain image data.");
            }
            return asset.GetDeserializedData(ImageDeserializer.DefaultDeserializer);
        }

        /// <summary>
        /// Tries to get the skin id of the skin <paramref name="asset"/>
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>Positive number on success, otherwise -1</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static int GetId(this PckAsset asset)
        {
            if (asset.Type != PckAssetType.SkinFile && asset.Type != PckAssetType.CapeFile)
                throw new InvalidOperationException("Asset has no id.");

            string assetnamePrefix = asset.Type == PckAssetType.SkinFile ? "dlcskin" : "dlccpae";

            string assetPath = Path.GetFileNameWithoutExtension(asset.Filename);
            if (!assetPath.StartsWith(assetnamePrefix))
            {
                Trace.TraceWarning($"[{nameof(GetId)}] Asset name does not start with '{assetnamePrefix}'");
                return -1;
            }

            int id = 0;
            if (!int.TryParse(assetPath.Substring(assetnamePrefix.Length), out id))
            {
                Trace.TraceWarning($"[{nameof(GetId)}] Failed to parse Id");
            }
            return id;
        }

        public static Skin.Skin GetSkin(this PckAsset asset)
        {
            if (asset.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("Asset is not a skin.");

            int skinId = asset.GetId();

            string name = asset.GetProperty("DISPLAYNAME");
            string theme = asset.GetProperty("THEMENAME");
            Image texture = asset.GetTexture();
            SkinANIM anim = asset.GetProperty("ANIM", SkinANIM.FromString);
            IEnumerable<SkinBOX> boxes = asset.GetPropertyValues("BOX").Select(SkinBOX.FromString);
            IEnumerable<SkinPartOffset> offsets = asset.GetPropertyValues("OFFSET").Select(SkinPartOffset.FromString);
            return new Skin.Skin(name, theme, skinId, texture, anim, boxes, offsets);
        }

        public static void SetSkin(this PckAsset asset, Skin.Skin skin, LOCFile localisation)
        {
            if (asset.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("Asset is not a skin file");

            asset.SetTexture(skin.Texture);

            string skinId = skin.Identifier.ToString("d08");

            // TODO: keep filepath 
            asset.Filename = $"dlcskin{skinId}.png";

            asset.SetProperty("DISPLAYNAME", skin.MetaData.Name);

            bool canLocalize = localisation is not null;

            if (canLocalize)
            {
                string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
                asset.SetProperty("DISPLAYNAMEID", skinLocKey);
                localisation.SetLocEntry(skinLocKey, skin.MetaData.Name);
            }

            if (!string.IsNullOrEmpty(skin.MetaData.Theme))
            {
                asset.SetProperty("THEMENAME", skin.MetaData.Theme);

                if (canLocalize)
                {
                    string skinThemeLocKey = $"IDS_dlcskin{skinId}_THEMENAME";
                    asset.SetProperty("THEMENAMEID", skinThemeLocKey);
                    localisation.SetLocEntry(skinThemeLocKey, skin.MetaData.Theme);
                }
            }

            if (skin.HasCape)
            {
                asset.SetProperty("CAPEPATH", $"dlccape{skinId}.png");
            }

            asset.SetProperty("ANIM", skin.Anim.ToString());
            asset.SetProperty("GAME_FLAGS", "0x18");
            asset.SetProperty("FREE", "1");

            asset.RemoveProperties("BOX");
            asset.RemoveProperties("OFFSET");

            foreach (SkinBOX box in skin.Model.AdditionalBoxes)
            {
                asset.AddProperty(box.ToProperty());
            }
            foreach (SkinPartOffset offset in skin.Model.PartOffsets)
            {
                asset.AddProperty(offset.ToProperty());
            }
        }

        public static T GetDeserializedData<T>(this PckAsset asset, IPckAssetDeserializer<T> deserializer)
        {
            return deserializer.Deserialize(asset);
        }

        public static T GetData<T>(this PckAsset asset, IDataFormatReader<T> formatReader) where T : class
        {
            using var ms = new MemoryStream(asset.Data);
            return formatReader.FromStream(ms);
        }

        public static void SetSerializedData<T>(this PckAsset asset, T obj, IPckAssetSerializer<T> serializer)
        {
            serializer.Serialize(obj, ref asset);
        }

        public static void SetData(this PckAsset asset, IDataFormatWriter formatWriter)
        {
            using (var stream = new MemoryStream())
            {
                formatWriter.WriteToStream(stream);
                asset.SetData(stream.ToArray());
            }
        }

        public static void SetTexture(this PckAsset asset, Image image)
        {
            if (asset.Type != PckAssetType.SkinFile &&
                asset.Type != PckAssetType.CapeFile &&
                asset.Type != PckAssetType.TextureFile)
            {
                throw new Exception("Asset is not suitable to contain image data.");
            }
            asset.SetSerializedData(image, ImageSerializer.DefaultSerializer);
        }

        public static bool IsMipmappedFile(this PckAsset asset)
        {
            // We only want to test the file name itself. ex: "terrainMipMapLevel2"
            string name = Path.GetFileNameWithoutExtension(asset.Filename);

            // check if last character is a digit (0-9). If not return false
            if (!char.IsDigit(name[name.Length - 1]))
                return false;

            // If string does not end with MipMapLevel, then it's not MipMapped
            if (!name.Remove(name.Length - 1, 1).EndsWith(MIPMAP_LEVEL))
                return false;
            return true;
        }

        public static string GetNormalPath(this PckAsset asset)
        {
            if (!asset.IsMipmappedFile())
                return asset.Filename;
            string ext = Path.GetExtension(asset.Filename);
            return asset.Filename.Remove(asset.Filename.Length - (MIPMAP_LEVEL.Length + 1) - ext.Length) + ext;
        }

        public static void DeserializeProperties(this PckAsset asset, IEnumerable<string> serializedData)
        {
            IEnumerable<KeyValuePair<string, string>> lines = serializedData
                .Select(line => line.Split([' '], 2))
                .Where (keyValue => keyValue.Length == 2)
                .Select(keyValue => new KeyValuePair<string, string>(keyValue[0].Replace(":", ""), keyValue[1]));
            foreach (KeyValuePair<string, string> kv in lines)
            {
                asset.AddProperty(kv);
            }
        }

        public static IEnumerable<string> SerializeProperties(this PckAsset asset, string seperater = ":")
        {
            IReadOnlyList<KeyValuePair<string, string>> properties = asset.GetProperties();
            return properties.Select(property => property.Key + seperater + property.Value);
        }
    }
}
