using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.PCK
{
    internal class PCKFileReader : StreamDataReader<PCKFile>
    {
        private PCKFile _file;
        private IList<string> _propertyList;

        public static PCKFile Read(Stream stream, bool isLittleEndian)
        {
            return new PCKFileReader(isLittleEndian).ReadFromStream(stream);
        }

        private PCKFileReader(bool isLittleEndian) : base(isLittleEndian)
        {
        }

        protected override PCKFile ReadFromStream(Stream stream)
        {
            int pck_type = ReadInt(stream);
            if (pck_type > 0xf0_00_00) // 03 00 00 00 == true
                throw new OverflowException(nameof(pck_type));
            _file = new PCKFile(pck_type);
            ReadLookUpTable(stream);
            ReadFileEntries(stream);
            ReadFileContents(stream);
            return _file;
        }

        private void ReadLookUpTable(Stream stream)
        {
            int count = ReadInt(stream);
            _propertyList = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int index = ReadInt(stream);
                string value = ReadString(stream);
                _propertyList.Insert(index, value);
            }
            if (_propertyList.Contains(PCKFile.XMLVersionString))
                Console.WriteLine(ReadInt(stream)); // xml version num ??
        }

        private void ReadFileEntries(Stream stream)
        {
            int file_entry_count = ReadInt(stream);
            for (; 0 < file_entry_count; file_entry_count--)
            {
                int file_size = ReadInt(stream);
                var file_type = (PCKFile.FileData.FileType)ReadInt(stream);
                string file_path = ReadString(stream).Replace('\\', '/');
                var entry = new PCKFile.FileData(file_path, file_type, file_size);
                _file.Files.Add(entry);
            }
        }

        private void ReadFileContents(Stream stream)
        {
            foreach (var file in _file.Files)
            {
                int property_count = ReadInt(stream);
                for (; 0 < property_count; property_count--)
                {
                    string key = _propertyList[ReadInt(stream)];
                    string value = ReadString(stream);
                    file.properties.Add((key, value));
                }
                stream.Read(file.data, 0, file.size);
            };
        }

        private string ReadString(Stream stream)
        {
            int len = ReadInt(stream);
            string s = ReadString(stream, len, IsUsingLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
            ReadInt(stream); // padding
            return s;
        }
    }
}
