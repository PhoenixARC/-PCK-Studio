using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PckStudio.Classes.Utils;

namespace PckStudio.Classes.IO.GRF
{
    internal class GRFFileReader
    {
        internal List<string> TagNames;
        internal GRFFile _file;
        public static GRFFile Read(Stream stream)
        {
            return new GRFFileReader().read(stream);
        }


        private GRFFile read(Stream stream)
        {
            stream = ReadHeader(stream);
            ReadTagNames(stream);
            ReadGRFTags(stream);
            return _file;
        }


        private Stream ReadHeader(Stream stream)
        {
            int x = ReadShort(stream);
            if (((x >> 31) | x) == 0)
            {
                ReadBytes(stream, 14); // 14 bools ?...
                return stream;
            }

            GRFFile.eCompressionType compression_type = (GRFFile.eCompressionType)stream.ReadByte();
            int crc = ReadInt(stream);
            int byte1 = stream.ReadByte();
            int byte2 = stream.ReadByte();
            int byte3 = stream.ReadByte();
            int byte4 = stream.ReadByte();
            if (byte4 > 0)
            {
                compression_type = (GRFFile.eCompressionType)byte4;
                // Custom enum
                compression_type &= GRFFile.eCompressionType.IsWolrdGrf;
            }
            _file = new GRFFile(compression_type, crc);

            if (compression_type == GRFFile.eCompressionType.None && byte4 == 0)
                return stream;

            int buf_size = ReadInt(stream);
            if (byte4 != 0)
            {
                stream = new MemoryStream(ReadBytes(stream, buf_size));
                buf_size = ReadInt(stream);
            }
            else
            {
                ReadInt(stream); // ignored cuz rest of data is compressed
            }
            stream = DecompressZLX(stream);
            if (compression_type > GRFFile.eCompressionType.Zlib)
            {
                byte[] data = RLE<byte>.Decode(ReadBytes(stream, buf_size)).ToArray();
                stream = new MemoryStream(data);
            }

            if (byte4 != 0)
                ReadBytes(stream, 23);

            return stream;
        }

        private Stream DecompressZLX(Stream compressedStream)
        {
            Stream outputstream = new MemoryStream();
            using (var inputStream = new InflaterInputStream(compressedStream))
            {
                inputStream.CopyTo(outputstream);
                outputstream.Position = 0;
            };
            return outputstream;
        }

        private void ReadTagNames(Stream stream)
        {
            int name_count = ReadInt(stream);
            TagNames = new List<string>(name_count);
            for (int i = 0; i < name_count; i++)
                TagNames.Add(ReadString(stream));
        }

        private void ReadGRFTags(Stream stream)
        {
            var NameAndCount = GetTagNameAndDetailCount(stream);
            _file.RootTag = new GRFFile.GRFTag(NameAndCount.Item1, null);
            _file.RootTag.Tags = ReadItemList(stream, NameAndCount.Item2, _file.RootTag);
        }

        internal List<GRFFile.GRFTag> ReadItemList(Stream stream, int count, GRFFile.GRFTag parent)
        {
            List<GRFFile.GRFTag> tags = new List<GRFFile.GRFTag>();
            for (int i = 0; i < count; i++)
            {
                var valuePair = GetTagNameAndDetailCount(stream);
                var tag = new GRFFile.GRFTag(valuePair.Item1, parent);
                for (var j = 0; j < valuePair.Item2; j++)
                {
                    var tuple = GetTagNameAndValue(stream);
                    tag.Parameters.Add(tuple.Item1, tuple.Item2);
                }
                tag.Tags = ReadItemList(stream, ReadInt(stream), tag);
                tags.Add(tag);
            }
            return tags;
        }

        internal string GetTagName(Stream stream) => TagNames[ReadInt(stream)];

        internal (string, int) GetTagNameAndDetailCount(Stream stream)
        {
            return new ValueTuple<string, int>(GetTagName(stream), ReadInt(stream));
        }

        internal (string, string) GetTagNameAndValue(Stream stream)
        {
            return new ValueTuple<string, string>(GetTagName(stream), ReadString(stream));
        }

        internal byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        internal int ReadInt(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        
        internal short ReadShort(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        internal string ReadString(Stream stream)
        {
            short stringLength = ReadShort(stream);
            byte[] buffer = ReadBytes(stream, stringLength);
            return Encoding.UTF8.GetString(buffer);
        }

    }
}
