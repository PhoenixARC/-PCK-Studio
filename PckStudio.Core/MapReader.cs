using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using OMI;

namespace PckStudio.Core
{
    public class MapReader
    {
        public static IDictionary<string, byte[]> OpenSave(Stream stream)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, ByteOrder.BigEndian);
            _ = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();
            byte[] buffer = new byte[bufferSize];
            using Stream decompStream = new InflaterInputStream(stream);
            decompStream.Read(buffer, 0, bufferSize);
            reader = new EndiannessAwareBinaryReader(new MemoryStream(buffer), System.Text.Encoding.BigEndianUnicode, ByteOrder.BigEndian);

            int fileStartOffset = reader.ReadInt32();
            int fileCount = reader.ReadInt32();
            reader.BaseStream.Seek(fileStartOffset, SeekOrigin.Begin);
            Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
            for (int i = 0; i < fileCount; i++)
            {
                string path = reader.ReadString(64).Replace('\\', '/');
                int size = reader.ReadInt32();
                int offset = reader.ReadInt32();
                long timestemp = reader.ReadInt64();

                long origin = reader.BaseStream.Position;
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                byte[] data = reader.ReadBytes(size);
                reader.BaseStream.Seek(origin, SeekOrigin.Begin);

                res.Add(path, data);
            }
            return res;
        }
    }
}
