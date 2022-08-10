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
    internal class GRFFileReader : StreamDataReader
    {
        private IList<string> StringLookUpTable;
        private GRFFile _file;

        public static GRFFile Read(Stream stream)
        {
            return new GRFFileReader().ReadFromStream(stream);
        }

        private GRFFileReader() : base(false)
        { }

        private GRFFile ReadFromStream(Stream stream)
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
            _file = new GRFFile(crc, byte4 > 0);

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
            ReadStringLookUpTable(stream);
            string Name = GetString(stream);
            Console.WriteLine($"[{nameof(GRFFile)}] Root Name: {Name}");
            ReadGameRuleHierarchy(stream, _file.Root);
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

        private void ReadStringLookUpTable(Stream stream)
        {
            int name_count = ReadInt(stream);
            StringLookUpTable = new List<string>(name_count);
            for (int i = 0; i < name_count; i++)
            {
                string s = ReadString(stream);
                StringLookUpTable.Add(s);
            }
        }

        private void ReadGameRuleHierarchy(Stream stream, GRFFile.GameRule parent)
        {
            _ = parent ?? throw new ArgumentNullException(nameof(parent));
            int count = ReadInt(stream);
            for (int i = 0; i < count; i++)
            {
                (string Name, int Count) parameter = (GetString(stream), ReadInt(stream));
                var rule = parent.AddRule(parameter.Name);
                for (int j = 0; j < parameter.Count; j++)
                {
                    rule.Parameters.Add(GetString(stream), ReadString(stream));
                }
                ReadGameRuleHierarchy(stream, rule);
            }
        }

        private string GetString(Stream stream) => StringLookUpTable[ReadInt(stream)];

        private string ReadString(Stream stream)
        {
            short stringLength = ReadShort(stream);
            return ReadString(stream, stringLength, Encoding.ASCII);
        }
    }
}
