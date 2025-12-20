using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static Image GetImage(this ZipArchiveEntry entry)
        {
            if (entry == null || (!entry.Name.EndsWith(".png") && !entry.Name.EndsWith(".jpg")))
                return null;

            using Stream stream = entry.Open();
            Image image = Image.FromStream(stream);
            stream.Dispose();
            return image;
        }

        public static IEnumerable<ZipArchiveEntry> GetDirectoryContent(this ZipArchive zip, string path, string extention = "")
        {
            return zip.Entries.Where(e => e.FullName.StartsWith(path) && e.Name.EndsWith(extention) && !e.Name.EndsWith("/") && !e.Name.EndsWith("\\"));
        }
    }
}
