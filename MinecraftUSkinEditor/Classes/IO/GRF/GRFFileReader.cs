using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            ReadBody(stream);
            return _file;
        }

        private Stream ReadHeader(Stream stream)
        {
            int x = ReadShort(stream);
            if (((x >> 31) | x) == 0)
            {
                // 14 bools ?...
                ReadBytes(stream, 14);
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
            }
            _file = new GRFFile(compression_type, crc, byte4 > 0);

            if (compression_type == GRFFile.eCompressionType.None && byte4 == 0)
                return stream;

            int buf_size = ReadInt(stream);
            var new_stream = stream;
            if (byte4 != 0)
            {
                new_stream = new MemoryStream(ReadBytes(stream, buf_size));
                buf_size = ReadInt(new_stream);
            }
            else
            {
                ReadInt(stream); // ignored cuz rest of data is compressed
            }
            var decompressed_stream = DecompressZLX(new_stream);
            new_stream.Dispose();
            if (compression_type > GRFFile.eCompressionType.Zlib)
            {
                byte[] data = ReadBytes(decompressed_stream, buf_size);
                byte[] decoded_data = RLE<byte>.Decode(data).ToArray();
                decompressed_stream.Dispose();
                decompressed_stream = new MemoryStream(decoded_data);
            }

            if (byte4 != 0)
                ReadBytes(decompressed_stream, 23);

            return decompressed_stream;
        }

        private void ReadBody(Stream stream)
        {
            ReadTagNames(stream);
            ReadGRFTags(stream);
        }

        private Stream DecompressZLX(Stream compressedStream)
        {
            Stream outputstream = new MemoryStream();
            using (var inputStream = new InflaterInputStream(compressedStream))
            {
                inputStream.IsStreamOwner = false;
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
            {
                string s = ReadString(stream);
                TagNames.Add(s);
                //Console.WriteLine(s);
            }
        }

        private void ReadGRFTags(Stream stream)
        {
            var NameAndCount = GetRuleNameAndCount(stream);
            _file.RootTag = new GRFFile.GRFTag(NameAndCount.Item1, null);
            _file.RootTag.Tags = ReadTags(stream, NameAndCount.Item2, _file.RootTag);
        }

        internal List<GRFFile.GRFTag> ReadTags(Stream stream, int count, GRFFile.GRFTag parent)
        {
            List<GRFFile.GRFTag> tags = new List<GRFFile.GRFTag>();
            for (int i = 0; i < count; i++)
            {
                var valuePair = GetRuleNameAndCount(stream);
                var tag = new GRFFile.GRFTag(valuePair.Item1, parent);
                for (int j = 0; j < valuePair.Item2; j++)
                {
                    var tuple = GetTagNameAndValue(stream);
                    tag.Parameters.Add(tuple.Item1, tuple.Item2);
                }
                tag.Tags = ReadTags(stream, ReadInt(stream), tag);
                tags.Add(tag);
            }
            return tags;
        }

        internal string GetTagName(Stream stream) => TagNames[ReadInt(stream)];

        internal (string, int) GetRuleNameAndCount(Stream stream)
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
            return Encoding.ASCII.GetString(buffer);
        }

    }
}
