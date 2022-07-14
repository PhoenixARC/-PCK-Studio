using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace PckStudio.Classes.IO
{
    internal class PCKFileReader : StreamDataReader
    {
        internal PCKFile _file;

        public static PCKFile Read(Stream stream, bool isLittleEndian)
        {
            return new PCKFileReader(isLittleEndian).ReadFileFromStream(stream);
        }

        private PCKFileReader(bool isLittleEndian) : base(isLittleEndian)
        {
        }

        private PCKFile ReadFileFromStream(Stream stream)
        {
            _file = new PCKFile(ReadInt(stream));
            ReadMetaData(stream);
            ReadFileEntries(stream);
            return _file;
        }

        internal void ReadMetaData(Stream stream)
        {
            int meta_entry_count = ReadInt(stream);
            _file.meta_data.Capacity = meta_entry_count;
            for (; 0 < meta_entry_count; meta_entry_count--)
            {
                int index = ReadInt(stream);
                string value = ReadString(stream);
                _file.meta_data.Insert(index, value);
            }
            if (_file.meta_data.Contains("XMLVERSION"))
                Console.WriteLine(ReadInt(stream)); // xml version num ??
        }

        internal void ReadFileEntries(Stream stream)
        {
            int file_entry_count = ReadInt(stream);
            for (; 0 < file_entry_count; file_entry_count--)
            {
                int file_size = ReadInt(stream);
                int file_type = ReadInt(stream);
                string name = ReadString(stream);
                var entry = new PCKFile.FileData(name, file_type, file_size);
                _file.file_entries.Add(entry);
            }
            foreach (var file_entry in _file.file_entries)
            {
                int property_count = ReadInt(stream);
                for (; 0 < property_count; property_count--)
                {
                    int index = ReadInt(stream);
                    string key = _file.meta_data[index];
                    string value = ReadString(stream);
                    file_entry.properties.Add(new ValueTuple<string, string>(key, value));
                }
                // file data buffer is only allocated when FileData is constructed with `dataSize`
                stream.Read(file_entry.data, 0, file_entry.size);
            }
        }
        internal string ReadString(Stream stream)
        {
            int len = ReadInt(stream);
            string s = ReadString(stream, len * 2, Encoding.BigEndianUnicode);
            ReadInt(stream); // padding
            return s;
        }
    }
}
