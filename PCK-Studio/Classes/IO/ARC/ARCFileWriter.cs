using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.ARC
{
    internal class ARCFileWriter : StreamDataWriter
    {

        private ARCFileWriter() : base(true)
        {}

        public byte[] Build(ConsoleArchive ConsoleArc, string Filename)
        {
            MemoryStream f = new MemoryStream();
            WriteInt(f, ConsoleArc.Files.Count);
            foreach(ConsoleArchiveItem item in BuildTable(ConsoleArc))
            {
                WriteString(f, item.Name);
                WriteInt(f, item.Position);
                WriteInt(f, item.Size);
            }
            foreach (KeyValuePair<string, byte[]> pair in ConsoleArc.Files)
            {
                WriteBytes(f, pair.Value);
            }
            f.Close();
            f.Dispose();
            return f.ToArray();
        }

        private List<ConsoleArchiveItem> BuildTable(ConsoleArchive ConsoleArc)
        {
            List<ConsoleArchiveItem> l = new List<ConsoleArchiveItem>();
            int HeaderSize = 4;
            int currentFileOffset = 0;
            foreach(KeyValuePair<string, byte[]> pair in ConsoleArc.Files)
            {
                HeaderSize += pair.Key.Length;
                HeaderSize += 10;
                string name = pair.Key;
                int size = pair.Value.Length;
                int position = currentFileOffset;
                ConsoleArchiveItem citem = new ConsoleArchiveItem(name, size, position);
                l.Add(citem);
                currentFileOffset += size;
            }
            foreach (ConsoleArchiveItem item in l)
            {
                item.Position += HeaderSize;
            }
            return l;
        }
        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }
    }
}
