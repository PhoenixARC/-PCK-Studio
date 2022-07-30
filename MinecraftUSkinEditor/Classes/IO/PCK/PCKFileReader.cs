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
        private PCKFile _file;
        private List<string> LUT;


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
            ReadLookUpTabel(stream);
            ReadFileEntries(stream);
            ReadFileContents(stream);
            return _file;
        }

        internal void ReadLookUpTabel(Stream stream)
        {
            int count = ReadInt(stream);
            LUT = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int index = ReadInt(stream);
                string value = ReadString(stream);
                LUT.Insert(index, value);
            }
            if (LUT.Contains("XMLVERSION"))
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
                _file.Files.Add(entry);
            }
        }
        internal void ReadFileContents(Stream stream)
        {
            _file.Files.ForEach( file => {
                int property_count = ReadInt(stream);
                for (; 0 < property_count; property_count--)
                {
                    string key = LUT[ReadInt(stream)];
                    string value = ReadString(stream);
                    file.properties.Add((key, value));
                }
                stream.Read(file.data, 0, file.size);
            });
        }
        internal string ReadString(Stream stream)
        {
            int len = ReadInt(stream);
            string s = ReadString(stream, len, IsUsingLittleEndian ?  Encoding.Unicode : Encoding.BigEndianUnicode);
            ReadInt(stream); // padding
            return s;
        }
    }
}
