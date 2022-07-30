using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO
{
    internal class PCKFileWriter : StreamDataWriter
    {
        private PCKFile _pckfile;
        private List<string> LUT = new List<string>();
        private bool _isLittleEndian;

        public static void Write(Stream stream, PCKFile file, bool isLittleEndian)
        {
            new PCKFileWriter(file, isLittleEndian).WriteToStream(stream);
        }

        private PCKFileWriter(PCKFile file, bool isLittleEndian) : base(isLittleEndian)
        {
            _isLittleEndian = isLittleEndian;
            _pckfile = file;
            LUT = _pckfile.GatherMetaTags();
        }

        private void WriteToStream(Stream stream)
        {
            WriteInt(stream, _pckfile.type);
            WriteLookUpTable(stream);
            WriteFileEntries(stream);
            WriteFileContents(stream);
        }

        internal void WriteString(Stream stream, string s)
        {
            WriteInt(stream, s.Length);
            WriteString(stream, s, _isLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
            WriteInt(stream, 0); // padding
        }

        internal void WriteLookUpTable(Stream stream)
        {
            WriteInt(stream, LUT.Count);
            LUT.ForEach(entry =>
            {
                WriteInt(stream, LUT.IndexOf(entry));
                WriteString(stream, entry);
            });
            if (LUT.Contains("XMLVERSION"))
                WriteInt(stream, 0x1337); // :^)
        }

        internal void WriteFileEntries(Stream stream)
        {
            WriteInt(stream, _pckfile.Files.Count);
            foreach (var file in _pckfile.Files)
            {
                WriteInt(stream, file.size);
                WriteInt(stream, file.type);
                WriteString(stream, file.name);
            }
        }
        
        internal void WriteFileContents(Stream stream)
        {
            foreach (var file in _pckfile.Files)
            {
                WriteInt(stream, file.properties.Count);
                foreach (var property in file.properties)
                {
                    if (!LUT.Contains(property.Item1))
                        throw new Exception("Tag not in Look Up Table: " + property.Item1);
                    WriteInt(stream, LUT.IndexOf(property.Item1));
                    WriteString(stream, property.Item2);
                }
                WriteBytes(stream, file.data, file.size);
            }
        }
    }
}
