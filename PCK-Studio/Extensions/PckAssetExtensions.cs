using System;
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
using PckStudio.IO.TGA;
using PckStudio.Internal.Deserializer;
using PckStudio.Internal.Serializer;

namespace PckStudio.Extensions
{
    internal static class PckAssetExtensions
    {
        private const string MipMap = "MipMapLevel";

        internal static Image GetTexture(this PckAsset file)
        {
            if (file.Type != PckAssetType.SkinFile &&
                file.Type != PckAssetType.CapeFile &&
                file.Type != PckAssetType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }
            return file.GetDeserializedData(ImageDeserializer.DefaultDeserializer);
        }

        internal static T GetDeserializedData<T>(this PckAsset file, IPckAssetDeserializer<T> deserializer)
        {
            return deserializer.Deserialize(file);
        }

        internal static T GetData<T>(this PckAsset file, IDataFormatReader<T> formatReader) where T : class
        {
            using var ms = new MemoryStream(file.Data);
            return formatReader.FromStream(ms);
        }

        internal static void SetSerializedData<T>(this PckAsset file, T obj, IPckAssetSerializer<T> serializer)
        {
            serializer.Serialize(obj, ref file);
        }

        internal static void SetData(this PckAsset file, IDataFormatWriter formatWriter)
        {
            using (var stream = new MemoryStream())
            {
                formatWriter.WriteToStream(stream);
                file.SetData(stream.ToArray());
            }
        }

        internal static void SetTexture(this PckAsset file, Image image)
        {
            if (file.Type != PckAssetType.SkinFile &&
                file.Type != PckAssetType.CapeFile &&
                file.Type != PckAssetType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }
            file.SetSerializedData(image, ImageSerializer.DefaultSerializer);
        }

        internal static bool IsMipmappedFile(this PckAsset file)
        {
            // We only want to test the file name itself. ex: "terrainMipMapLevel2"
            string name = Path.GetFileNameWithoutExtension(file.Filename);

            // check if last character is a digit (0-9). If not return false
            if (!char.IsDigit(name[name.Length - 1]))
                return false;

            // If string does not end with MipMapLevel, then it's not MipMapped
            if (!name.Remove(name.Length - 1, 1).EndsWith(MipMap))
                return false;
            return true;
        }

        internal static string GetNormalPath(this PckAsset file)
        {
            if (!file.IsMipmappedFile())
                return file.Filename;
            string ext = Path.GetExtension(file.Filename);
            return file.Filename.Remove(file.Filename.Length - (MipMap.Length + 1) - ext.Length) + ext;
        }
    }
}
