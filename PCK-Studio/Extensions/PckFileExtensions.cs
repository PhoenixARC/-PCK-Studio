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
        internal static PckAsset CreateNewAssetIf(this PckFile pck, bool condition, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            if (condition)
            {
                return pck.CreateNewAsset(filename, filetype, writer);
            }
            return null;
        }

        internal static PckAsset CreateNewAsset(this PckFile pck, string filename, PckAssetType filetype, IDataFormatWriter writer)
        {
            var asset = pck.CreateNewAsset(filename, filetype);
            asset.SetData(writer);
            return asset;
        }
    }
}
