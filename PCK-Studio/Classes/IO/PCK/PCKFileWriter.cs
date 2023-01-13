using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.PCK
{
    internal class PCKFileWriter : StreamDataWriter
    {
        private PCKFile _pckfile;
        private IList<string> _propertyList;

        public static void Write(Stream stream, PCKFile file, bool isLittleEndian, bool isSkinsPCK = false)
        {
            new PCKFileWriter(file, isLittleEndian, isSkinsPCK).WriteToStream(stream);
        }

        private PCKFileWriter(PCKFile file, bool isLittleEndian, bool isSkinsPCK) : base(isLittleEndian)
        {
            _pckfile = file;
            _propertyList = _pckfile.GetPropertyList();
            if (!_propertyList.Contains(PCKFile.XMLVersionString) && isSkinsPCK)
                _propertyList.Insert(0, PCKFile.XMLVersionString);
        }

        protected override void WriteToStream(Stream stream)
        {
            WriteInt(stream, _pckfile.type);
            WriteLookUpTable(stream);
            WriteFileEntries(stream);
            WriteFileContents(stream);
        }

        private void WriteString(Stream stream, string s)
        {
            WriteInt(stream, s.Length);
            WriteString(stream, s, IsUsingLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
            WriteInt(stream, 0); // padding
        }

        private void WriteLookUpTable(Stream stream)
        {
            WriteInt(stream, _propertyList.Count);
            foreach (var entry in _propertyList)
            {
                WriteInt(stream, _propertyList.IndexOf(entry));
                WriteString(stream, entry);
            };
            if (_propertyList.Contains(PCKFile.XMLVersionString))
                WriteInt(stream, 0x1337); // :^)
        }

        private void WriteFileEntries(Stream stream)
        {
            WriteInt(stream, _pckfile.Files.Count);
            foreach (var file in _pckfile.Files)
            {
                WriteInt(stream, file.Size);
                WriteInt(stream, (int)file.Filetype);
                WriteString(stream, file.Filename);
            }
        }

        private void WriteFileContents(Stream stream)
        {
            foreach (var file in _pckfile.Files)
            {
                WriteInt(stream, file.Properties.Count);
                foreach (var property in file.Properties)
                {
                    if (!_propertyList.Contains(property.Item1))
                        throw new Exception("Tag not in Look Up Table: " + property.Item1);
                    WriteInt(stream, _propertyList.IndexOf(property.Item1));
                    WriteString(stream, property.Item2);
                }
                WriteBytes(stream, file.Data, file.Size);
            }
        }
    }
}