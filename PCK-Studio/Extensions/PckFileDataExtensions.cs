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

namespace PckStudio.Extensions
{
    internal static class PckFileDataExtensions
    {
        private const string MipMap = "MipMapLevel";

        private static Image EmptyImage = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        internal static Image GetTexture(this PckFileData file)
        {
            if (file.Filetype != PckFileType.SkinFile &&
                file.Filetype != PckFileType.CapeFile &&
                file.Filetype != PckFileType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }
            using (var stream = new MemoryStream(file.Data))
            {
                try
                {
                    if (Path.GetExtension(file.Filename) == ".tga")
                        return TGADeserializer.DeserializeFromStream(stream);
                    else
                        return Image.FromStream(stream);
                }
                catch(Exception ex)
                {
                    Trace.WriteLine($"Failed to read image from pck file data({file.Filename}).", category: nameof(PckFileDataExtensions) + "." + nameof(GetTexture));
                    Debug.WriteLine(ex.Message);
                    return EmptyImage;
                }
            }
        }

        internal static T Get<T>(this PckFileData file, IPckDeserializer<T> deserializer)
        {
            return deserializer.Deserialize(file);
        }

        internal static T Get<T>(this PckFileData file, IDataFormatReader<T> deserializer) where T : class
        {
            using var ms = new MemoryStream(file.Data);
            return deserializer.FromStream(ms);
        }

        internal static void SetData<T>(this PckFileData file, T obj, IPckFileSerializer<T> serializer)
        {
            serializer.Serialize(obj, ref file);
        }

        internal static void SetData(this PckFileData file, IDataFormatWriter writer)
        {
            using (var stream = new MemoryStream())
            {
                writer.WriteToStream(stream);
                file.SetData(stream.ToArray());
            }
        }

        internal static void SetData(this PckFileData file, Image image, ImageFormat imageFormat)
        {
            if (file.Filetype != PckFileType.SkinFile &&
                file.Filetype != PckFileType.CapeFile &&
                file.Filetype != PckFileType.TextureFile)
            {
                throw new Exception("File is not suitable to contain image data.");
            }

            using (var stream = new MemoryStream())
            {
                image.Save(stream, imageFormat);
                file.SetData(stream.ToArray());
            }
        }

        internal static bool IsMipmappedFile(this PckFileData file)
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

        internal static string GetNormalPath(this PckFileData file)
        {
            if (!file.IsMipmappedFile())
                return file.Filename;
            string ext = Path.GetExtension(file.Filename);
            return file.Filename.Remove(file.Filename.Length - (MipMap.Length + 1) - ext.Length) + ext;
        }
    }
}
