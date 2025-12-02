using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.Extensions
{
    internal static class ZipArchiveEntryExtensions
    {
        public static string ReadAllText(this ZipArchiveEntry entry)
        {
            if (entry == null)
                return string.Empty;
            using StreamReader reader = new StreamReader(entry.Open());
            return reader.ReadToEnd();
        }
    }
}
