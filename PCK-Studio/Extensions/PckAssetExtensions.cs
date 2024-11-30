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
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;
using PckStudio.Internal.Skin;

namespace PckStudio.Extensions
{
    internal static class PckAssetExtensions
    {
        private const string MipMap = "MipMapLevel";

        internal static PckAsset CreateNewAssetIf(this PckFile pck, bool condition, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            if (condition)
            {
                return pck.CreateNewAsset(filename, filetype, writer);
            }
            return default;
        }

        internal static PckAsset CreateNewAsset(this PckFile pck, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            PckAsset asset = pck.CreateNewAsset(filename, filetype);
            asset.SetData(writer);
            return asset;
        }

        internal static Image GetTexture(this PckAsset asset)
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
        /// <returns>Non-zero base number on success, otherwise 0</returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static int GetSkinId(this PckAsset asset)
        {
            if (asset.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("File is not a skin file");
            
            string filename = Path.GetFileNameWithoutExtension(asset.Filename);
            if (!filename.StartsWith("dlcskin"))
            {
                Trace.TraceWarning($"[{nameof(GetSkinId)}] File does not start with 'dlcskin'");
                return 0;
            }

            int skinId = 0;
            if (!int.TryParse(filename.Substring("dlcskin".Length), out skinId))
            {
                Trace.TraceWarning($"[{nameof(GetSkinId)}] Failed to parse Skin Id");
            }
            return skinId;
        }

        internal static Skin GetSkin(this PckAsset file)
        {
            if (file.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("File is not a skin file");

            int skinId = file.GetSkinId();

            string name = file.GetProperty("DISPLAYNAME");
            Image texture = file.GetTexture();
            SkinANIM anim = file.GetProperty("ANIM", SkinANIM.FromString);
            IEnumerable<SkinBOX> boxes = file.GetMultipleProperties("BOX").Select(kv => SkinBOX.FromString(kv.Value));
            IEnumerable<SkinPartOffset> offsets = file.GetMultipleProperties("OFFSET").Select(kv => SkinPartOffset.FromString(kv.Value));
            return new Skin(name, skinId, texture, anim, boxes, offsets);
        }

        internal static void SetSkin(this PckAsset asset, Skin skin, LOCFile localizationFile)
        {
            if (asset.Type != PckAssetType.SkinFile)
                throw new InvalidOperationException("Asset is not a skin file");

            asset.SetTexture(skin.Model.Texture);

            string skinId = skin.MetaData.Id.ToString("d08");

            // TODO: keep filepath 
            asset.Filename = $"dlcskin{skinId}.png";

            string skinLocKey = $"IDS_dlcskin{skinId}_DISPLAYNAME";
            asset.SetProperty("DISPLAYNAME", skin.MetaData.Name);
            asset.SetProperty("DISPLAYNAMEID", skinLocKey);
            localizationFile.AddLocKey(skinLocKey, skin.MetaData.Name);

            if (!string.IsNullOrEmpty(skin.MetaData.Theme))
            {
                asset.SetProperty("THEMENAME", skin.MetaData.Theme);
                asset.SetProperty("THEMENAMEID", $"IDS_dlcskin{skinId}_THEMENAME");
                localizationFile.AddLocKey($"IDS_dlcskin{skinId}_THEMENAME", skin.MetaData.Theme);
            }

            if (skin.HasCape)
            {
                asset.SetProperty("CAPEPATH", $"dlccape{skinId}.png");
            }

            asset.SetProperty("ANIM", skin.Model.ANIM.ToString());
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

        internal static T GetDeserializedData<T>(this PckAsset asset, IPckAssetDeserializer<T> deserializer)
        {
            return deserializer.Deserialize(asset);
        }

        internal static T GetData<T>(this PckAsset asset, IDataFormatReader<T> formatReader) where T : class
        {
            using var ms = new MemoryStream(asset.Data);
            return formatReader.FromStream(ms);
        }

        internal static void SetSerializedData<T>(this PckAsset asset, T obj, IPckAssetSerializer<T> serializer)
        {
            serializer.Serialize(obj, ref asset);
        }

        internal static void SetData(this PckAsset asset, IDataFormatWriter formatWriter)
        {
            using (var stream = new MemoryStream())
            {
                formatWriter.WriteToStream(stream);
                asset.SetData(stream.ToArray());
            }
        }

        internal static void SetTexture(this PckAsset asset, Image image)
        {
            if (asset.Type != PckAssetType.SkinFile &&
                asset.Type != PckAssetType.CapeFile &&
                asset.Type != PckAssetType.TextureFile)
            {
                throw new Exception("Asset is not suitable to contain image data.");
            }
            asset.SetSerializedData(image, ImageSerializer.DefaultSerializer);
        }

        internal static bool IsMipmappedFile(this PckAsset asset)
        {
            // We only want to test the file name itself. ex: "terrainMipMapLevel2"
            string name = Path.GetFileNameWithoutExtension(asset.Filename);

            // check if last character is a digit (0-9). If not return false
            if (!char.IsDigit(name[name.Length - 1]))
                return false;

            // If string does not end with MipMapLevel, then it's not MipMapped
            if (!name.Remove(name.Length - 1, 1).EndsWith(MipMap))
                return false;
            return true;
        }

        internal static string GetNormalPath(this PckAsset asset)
        {
            if (!asset.IsMipmappedFile())
                return asset.Filename;
            string ext = Path.GetExtension(asset.Filename);
            return asset.Filename.Remove(asset.Filename.Length - (MipMap.Length + 1) - ext.Length) + ext;
        }

        internal static void DeserializeProperties(this PckAsset asset, IEnumerable<string> serializedData)
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

        internal static IEnumerable<string> SerializeProperties(this PckAsset asset, string seperater = ":")
        {
            IReadOnlyList<KeyValuePair<string, string>> properties = asset.GetProperties();
            return properties.Select(property => property.Key + seperater + property.Value);
        }
    }
}
