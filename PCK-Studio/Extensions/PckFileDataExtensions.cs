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

namespace PckStudio.Extensions
{
    internal static class PckFileDataExtensions
    {
        private const string MipMap = "MipMapLevel";

        internal static void SetData(this PckFile.FileData file, IDataFormatWriter writer)
        {
            using (var stream = new MemoryStream())
            {
                writer.WriteToStream(stream);
                file.SetData(stream.ToArray());
            }
        }

        internal static void SetData(this PckFile.FileData file, Image image, ImageFormat imageFormat)
        {
            if (file.Filetype != PckFile.FileData.FileType.SkinFile &&
                file.Filetype != PckFile.FileData.FileType.CapeFile &&
                file.Filetype != PckFile.FileData.FileType.TextureFile)
            {
                Debug.WriteLine($"{file.Filename} is can't contain image data");
                return;
            }

            using (var stream = new MemoryStream())
            {
                image.Save(stream, imageFormat);
                file.SetData(stream.ToArray());
            }
        }

        internal static bool IsMipmappedFile(this PckFile.FileData file)
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

        internal static string GetNormalPath(this PckFile.FileData file)
        {
            if (!file.IsMipmappedFile())
                return file.Filename;
            string ext = Path.GetExtension(file.Filename);
            return file.Filename.Remove(file.Filename.Length - (MipMap.Length + 1) - ext.Length) + ext;
        }
    }
}
