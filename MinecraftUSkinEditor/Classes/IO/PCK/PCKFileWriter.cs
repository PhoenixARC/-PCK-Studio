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
        internal PCKFile _file;

        public static void Write(Stream stream, PCKFile file, bool isLittleEndian)
        {
            new PCKFileWriter(file, isLittleEndian).WriteToStream(stream);
        }

        private PCKFileWriter(PCKFile file, bool isLittleEndian) : base(isLittleEndian)
        {
            _file = file;
        }

        private void WriteToStream(Stream stream)
        {
            WriteInt(stream, _file.type);
            WriteMetaEntries(stream);
            WriteFileEntries(stream);
        }

        internal void WriteString(Stream stream, string s)
        {
            WriteInt(stream, s.Length);
            WriteString(stream, s, Encoding.BigEndianUnicode);
            WriteInt(stream, 0); // padding
        }

        internal void WriteMetaEntries(Stream stream)
        {
            WriteInt(stream, _file.meta_data.Count);
            _file.meta_data.ForEach(entry =>
            {
                WriteInt(stream, _file.meta_data.IndexOf(entry));
                WriteString(stream, entry);
            });
            if (_file.meta_data.Contains("XMLVERSION"))
                WriteInt(stream, 0x1337); // :^)
        }

        internal void WriteFileEntries(Stream stream)
        {
            WriteInt(stream, _file.file_entries.Count);
            foreach (var entry in _file.file_entries)
            {
                WriteInt(stream, entry.size);
                WriteInt(stream, entry.type);
                WriteString(stream, entry.name);
            }
            foreach (var entry in _file.file_entries)
            {
                WriteInt(stream, entry.properties.Count);
                foreach (var property in entry.properties)
                {
                    if (!_file.meta_data.Contains(property.Item1))
                        throw new Exception("Tag not in Meta: " + property.Item1);
                    WriteInt(stream, _file.meta_data.IndexOf(property.Item1));
                    WriteString(stream, property.Item2);
                }
                stream.Write(entry.data, 0, entry.size);
            }
        }

    }
}
