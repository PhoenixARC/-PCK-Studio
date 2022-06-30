using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO.GRF
{
    public class GRFFileWriter
    {
        internal GRFFile _grfFile;
        public static void Write(Stream stream, GRFFile grfFile)
        {
            new GRFFileWriter(grfFile).write(stream);
        }

        private GRFFileWriter(GRFFile grfFile)
        {
            _grfFile = grfFile;
        }

        private void write(Stream stream)
        {
            BuildHeader(stream);
            WriteTagNames(stream);
        }


        private void BuildHeader(Stream stream)
        {
            WriteShort(stream, 1); // (x >> 31 | x) == 1
            stream.WriteByte((byte)_grfFile.CompressionType);
            WriteInt(stream, _grfFile.Crc);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0);
            stream.WriteByte(0);
        }

        private void WriteTagNames(Stream stream)
        {
            List<string> tagNames = new List<string>();
            GatherTagNames(_grfFile.RootTag, tagNames);
            WriteInt(stream, tagNames.Count);
            foreach (var s in tagNames)
            {
                WriteString(stream, s);
                Console.WriteLine(s);
            }
        }

        private void GatherTagNames(GRFFile.GRFTag tag, List<string> l)
        {
            if (!l.Contains(tag.Name)) l.Add(tag.Name);
            foreach (var subTag in tag.Tags)
                GatherTagNames(subTag, l);
            foreach (var subTag in tag.Parameters)
                if (!l.Contains(subTag.Key)) l.Add(subTag.Key);
        }

        internal void WriteInt(Stream stream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            WriteBytes(stream, bytes);
        }

        internal void WriteShort(Stream stream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            WriteBytes(stream, bytes);
        }

        internal void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteBytes(stream, Encoding.UTF8.GetBytes(s));
        }

        internal void WriteBytes(Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }


    }
}
