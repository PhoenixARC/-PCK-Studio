using OMI.Workers.Archive;
using System.IO;

namespace PckStudio.Classes.Utils.ARC
{
    public static class ARCUtil
    {
        public static void Inject(Stream stream, (string filepath, byte[] data) entry)
        {
            var reader = new ARCFileReader();
            var archive = reader.FromStream(stream);
            archive.Add(entry.filepath, entry.data);
            var writer = new ARCFileWriter(archive);
            stream.Seek(0, SeekOrigin.Begin);
            writer.WriteToStream(stream);
        }
    }
}