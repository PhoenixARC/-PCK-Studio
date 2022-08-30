using PckStudio.Classes.IO.ARC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.Utils.ARC
{
    public static class ARCUtil
    {
        public static void Inject(string filepath, (string filepath, byte[] data) entry)
        {
            using (var fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite))
            {
                var archive = ARCFileReader.Read(fs);
                archive.Add(entry.filepath, entry.data);
                ARCFileWriter.Write(fs, archive);
            }
        }
    }
}