using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using OMI.Workers;

namespace PckStudio.Extensions
{
    internal static class PckFileExtensions
    {
        internal static PckAsset CreateNewFileIf(this PckFile pck, bool condition, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            if (condition)
            {
                return pck.CreateNewFile(filename, filetype, writer);
            }
            return null;
        }

        internal static PckAsset CreateNewFile(this PckFile pck, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            var asset = pck.CreateNewFile(filename, filetype);
            asset.SetData(writer);
            return asset;
        }
    }
}
