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
        private ConsoleArchive _archive;

        public static void Write(Stream stream, ConsoleArchive archive, bool useLittleEndian = false)
        {
            new ARCFileWriter(archive, useLittleEndian).WriteToStream(stream);
        }

        public ARCFileWriter(ConsoleArchive archive, bool useLittleEndian) : base(useLittleEndian)
        {
            _archive = archive;
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, _archive.Count);
            int currentOffset = 4 + _archive.Keys.Sum(key => 10 + key.Length);
            foreach (var pair in _archive)
            {
                int size = pair.Value.Length;
                WriteString(stream, pair.Key);
                WriteInt(stream, currentOffset);
                WriteInt(stream, size);
                currentOffset += size;
            }
            foreach (byte[] data in _archive.Values)
            {
                WriteBytes(stream, data);
            }
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.UTF8);
        }
    }
}
