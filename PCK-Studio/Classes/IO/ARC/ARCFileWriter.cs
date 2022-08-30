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

        public static void Write(Stream stream, ConsoleArchive archive)
        {
            new ARCFileWriter(archive).WriteToStream(stream);
        }

        public ARCFileWriter(ConsoleArchive archive) : base(true)
        {
            _archive = archive;
        }

        private void WriteToStream(Stream stream)
        {
            WriteInt(stream, _archive.Count);
            int currentOffset = 4 + _archive.Keys.ToArray().Sum(key => 10 + key.Length);
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

        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }
    }
}
