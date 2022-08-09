using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PckStudio.Classes;
using PckStudio.Classes.Utils.grf;
using PckStudio.Classes.Utils;

namespace PckStudio.Classes.IO.GRF
{
    internal class GRFFileWriter : StreamDataWriter
    {
        private readonly GRFFile _grfFile;
        private List<string> LUT;

        private GRFFile.eCompressionType _compressionType;

        public static void Write(in Stream stream, GRFFile grfFile, GRFFile.eCompressionType compressionType)
        {
            new GRFFileWriter(grfFile, compressionType).WriteToStream(stream);
        }

        private GRFFileWriter(GRFFile grfFile, GRFFile.eCompressionType compressionType) : base(false)
        {
            _compressionType = compressionType;
            if (grfFile.IsWorld)
                throw new NotImplementedException("World grf saving is currently unsupported");
            _grfFile = grfFile;
            LUT = new List<string>();
            PrepareLookUpTable(_grfFile.Root, LUT);
        }

        private void PrepareLookUpTable(GRFFile.GameRule tag, List<string> LUT)
        {
            if (!LUT.Contains(tag.Name)) LUT.Add(tag.Name);
            tag.SubRules.ForEach(tag => PrepareLookUpTable(tag, LUT));
            foreach (var param in tag.Parameters)
                if (!LUT.Contains(param.Key)) LUT.Add(param.Key);
        }

        private void WriteToStream(Stream stream)
        {
            WriteHeader(stream);
            using (var uncompressed_stream = new MemoryStream())
            {
                WriteBody(uncompressed_stream);
                HandleCompression(stream, uncompressed_stream);
            }
        }

        private void HandleCompression(Stream destinationStream, MemoryStream sourceStream)
        {
            byte[] _buffer = sourceStream.ToArray();
            int _original_length = _buffer.Length;

            if (_compressionType >= GRFFile.eCompressionType.ZlibRle)
                _buffer = CompressRle(_buffer);
            if (_compressionType >= GRFFile.eCompressionType.Zlib)
            {
                _buffer = CompressZib(_buffer);
                WriteInt(destinationStream, _original_length);
                WriteInt(destinationStream, _buffer.Length);
            }
            if (_compressionType >= GRFFile.eCompressionType.ZlibRleCrc)
                MakeAndWriteCrc(destinationStream, _buffer);
            WriteBytes(destinationStream, _buffer);
            return;
        }

        private byte[] CompressZib(byte[] buffer)
        {
            byte[] result;
            var outputStream = new MemoryStream(); // Stream gets Disposed in DeflaterOutputStream
            using (var deflateStream = new DeflaterOutputStream(outputStream))
            {
                WriteBytes(deflateStream, buffer);
                deflateStream.Flush();
                deflateStream.Finish();
                outputStream.Position = 0;
                result = outputStream.ToArray();
            }
            return result;
        }

        private byte[] CompressRle(byte[] buffer) => Utils.RLE<byte>.Encode(buffer).ToArray();

        private void MakeAndWriteCrc(Stream stream, byte[] data)
        {
            uint crc = CRC32.CRC(data);
            if (crc != _grfFile.Crc) // no writting needed if there is no change
            {
                stream.Position = 3;
                WriteInt(stream, (int)crc);
                stream.Seek(0, SeekOrigin.End); // reset to the end of the stream
            }
        }

        private void WriteHeader(Stream stream)
        {
            WriteShort(stream, 1);
            if (_compressionType < GRFFile.eCompressionType.None ||
                _compressionType > GRFFile.eCompressionType.ZlibRleCrc)
                throw new ArgumentException(nameof(_compressionType));
            stream.WriteByte((byte)_compressionType);
            WriteInt(stream, _grfFile.Crc);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0); // <- used in world grf
        }

        private void WriteBody(Stream stream)
        {
            WriteTagLookUpTable(stream);
            SetString(stream, _grfFile.Root.Name);
            WriteInt(stream, _grfFile.Root.SubRules.Count);
            WriteGameRuleHierarchy(stream, _grfFile.Root);
        }

        private void WriteTagLookUpTable(Stream stream)
        {
            WriteInt(stream, LUT.Count);
            LUT.ForEach( s => WriteString(stream, s) );
        }

        private void WriteGameRuleHierarchy(Stream stream, GRFFile.GameRule rule)
        {
            SetString(stream, rule.Name);
            WriteInt(stream, rule.Parameters.Count);
            foreach (var param in rule.Parameters) WriteParameter(stream, param);
            WriteInt(stream, rule.SubRules.Count);
            rule.SubRules.ForEach(subrule => WriteGameRuleHierarchy(stream, subrule));
        }

        private void WriteParameter(Stream stream, KeyValuePair<string, string> param)
        {
            SetString(stream, param.Key);
            WriteString(stream, param.Value);
        }

        private void SetString(Stream stream, string s)
        {
            int i = LUT.IndexOf(s);
            if (i == -1) throw new Exception(nameof(s));
            WriteInt(stream, i);
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.ASCII);
        }
    }
}
