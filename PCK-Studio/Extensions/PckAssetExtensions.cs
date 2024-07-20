﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using OMI.Workers;
using PckStudio.Interfaces;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;

namespace PckStudio.Extensions
{
    internal static class PckAssetExtensions
    {
        private const string MipMap = "MipMapLevel";

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

        internal static void DeserializePropertiesFromString(this PckAsset asset, string serializedData)
        {
            string[] lines = serializedData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                int idx = line.IndexOf(' ');
                if (idx == -1 || line.Length - 1 == idx)
                    continue;
                asset.AddProperty(line.Substring(0, idx).Replace(":", string.Empty), line.Substring(idx + 1));
            }
        }

        internal static string SerializePropertiesToString(this PckAsset asset)
        {
            StringBuilder builder = new StringBuilder(asset.PropertyCount * 20);
            foreach (var property in asset.GetProperties())
            {
                builder.AppendLine(property.Key + ": " + property.Value);
            }
            return builder.ToString();
        }
    }
}
