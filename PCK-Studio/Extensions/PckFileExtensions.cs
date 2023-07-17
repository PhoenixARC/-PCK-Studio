using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;

namespace PckStudio.Extensions
{
    internal static class PckFileExtensions
    {
        private const string MipMap = "MipMapLevel";

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
