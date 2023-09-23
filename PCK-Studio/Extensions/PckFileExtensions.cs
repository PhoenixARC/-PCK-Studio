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
        internal static PckFileData CreateNewFileIf(this PckFile pck, bool condition, string filename, PckFileType filetype, IDataFormatWriter writer)
        {
            if (condition)
            {
                return pck.CreateNewFile(filename, filetype, writer);
            }
            return null;
        }

        internal static PckFileData CreateNewFile(this PckFile pck, string filename, PckFileType filetype, IDataFormatWriter writer)
        {
            var file = pck.CreateNewFile(filename, filetype);
            file.SetData(writer);
            return file;
        }
    }
}
