using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using OMI.Workers;

namespace PckStudio.Core.Extensions
{
    public static class ZipArchiveEntryExtensions
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

        public static IEnumerable<ZipArchiveEntry> GetDirectoryContent(this ZipArchive zip, string path, string extension = "", bool includeSubDirectories = false)
        {
            string sanitisedDirectoryPath = path.Replace('\\', '/');
            return zip.Entries
                .Where(e => e.FullName.StartsWith(sanitisedDirectoryPath))
                .Where(e => includeSubDirectories || (e.FullName.Substring(sanitisedDirectoryPath.Length).LastIndexOf('/') == 0 || e.FullName.Substring(sanitisedDirectoryPath.Length).LastIndexOf('/') == -1))
                .Where(e => string.IsNullOrWhiteSpace(extension) || e.FullName.EndsWith(extension));
        }

        public static ZipArchiveEntry WriteEntry(this ZipArchive zip, string name, byte[] data)
        {
            ZipArchiveEntry zipEntry = zip.CreateEntry(name, CompressionLevel.Fastest);
            Stream entryStream = zipEntry.Open();
            entryStream.Write(data, 0, data.Length);
            entryStream.Dispose();
            return zipEntry;
        }

        public static ZipArchiveEntry WriteEntry(this ZipArchive zip, string name, IDataFormatWriter writer)
        {
            ZipArchiveEntry zipEntry = zip.CreateEntry(name, CompressionLevel.Fastest);
            Stream entryStream = zipEntry.Open();
            writer.WriteToStream(entryStream);
            entryStream.Dispose();
            return zipEntry;
        }
    }
}
