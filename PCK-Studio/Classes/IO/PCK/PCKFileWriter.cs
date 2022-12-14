﻿using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PckStudio.Classes.IO.PCK
{
    internal class PCKFileWriter : StreamDataWriter
    {
        private PCKFile _pckfile;
        private List<string> LUT = new List<string>();

        public static void Write(Stream stream, PCKFile file, bool isLittleEndian, bool isSkinsPCK = false)
        {
            new PCKFileWriter(file, isLittleEndian, isSkinsPCK).WriteToStream(stream);
        }

        private PCKFileWriter(PCKFile file, bool isLittleEndian, bool isSkinsPCK) : base(isLittleEndian)
        {
            _pckfile = file;
            LUT = _pckfile.GatherPropertiesList();
            if (!LUT.Contains("XMLVERSION") && isSkinsPCK) LUT.Insert(0, "XMLVERSION");
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
            WriteInt(stream, LUT.Count);
            LUT.ForEach(entry =>
            {
                WriteInt(stream, LUT.IndexOf(entry));
                WriteString(stream, entry);
            });
            if (LUT.Contains("XMLVERSION"))
                WriteInt(stream, 0x1337); // :^)
        }

        private void WriteFileEntries(Stream stream)
        {
            WriteInt(stream, _pckfile.Files.Count);
            foreach (var file in _pckfile.Files)
            {
                WriteInt(stream, file.size);
                WriteInt(stream, (int)file.filetype);
                WriteString(stream, file.filepath);
            }
        }

        private void WriteFileContents(Stream stream)
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