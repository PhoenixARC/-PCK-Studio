using PckStudio.Classes.IO.ARC;
using System.IO;

namespace PckStudio.Classes.Utils.ARC
{
    public static class ARCUtil
    {
        public static void Inject(Stream stream, (string filepath, byte[] data) entry)
        {
            var archive = ARCFileReader.Read(stream);
            stream.Seek(0, SeekOrigin.Begin);
            archive.Add(entry.filepath, entry.data);
            ARCFileWriter.Write(stream, archive);
        }
    }
}