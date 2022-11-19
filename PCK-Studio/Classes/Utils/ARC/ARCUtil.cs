using PckStudio.Classes.IO.ARC;
using System.IO;

namespace PckStudio.Classes.Utils.ARC
{
    public static class ARCUtil
    {
        public static void Inject(string filepath, (string filepath, byte[] data) entry)
        {
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite))
            {
                var archive = ARCFileReader.Read(fs);
                fs.Seek(0, SeekOrigin.Begin);
                archive.Add(entry.filepath, entry.data);
                ARCFileWriter.Write(fs, archive);
            }
        }
    }
}