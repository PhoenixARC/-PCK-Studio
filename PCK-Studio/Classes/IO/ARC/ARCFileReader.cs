using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.ARC
{
    internal class ARCFileReader : StreamDataReader<ConsoleArchive>
    {
        public static ConsoleArchive Read(Stream stream, bool useLittleEndian = false)
        {
            return new ARCFileReader(useLittleEndian).ReadFromStream(stream);
        }

        private ARCFileReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }

        protected override ConsoleArchive ReadFromStream(Stream stream)
        {
            ConsoleArchive _archive = new ConsoleArchive();
            int numberOfFiles = ReadInt(stream);
            for(int i = 0; i < numberOfFiles; i++)
            {
                string name = ReadString(stream);
                int pos = ReadInt(stream);
                int size = ReadInt(stream);
                _archive[name] = ReadBytesFromPosition(stream, size, pos);
            }
            return _archive;
        }

        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }

        private byte[] ReadBytesFromPosition(Stream stream, int size, int position)
        {
            long originalPOS = stream.Position;
            if (stream.Seek(position, SeekOrigin.Begin) != position) throw new Exception();
            byte[] data = ReadBytes(stream, size);
            if (stream.Seek(originalPOS, SeekOrigin.Begin) != originalPOS) throw new Exception();
            return data;
        }

    }
}
