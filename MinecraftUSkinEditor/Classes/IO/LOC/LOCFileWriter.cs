using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PckStudio.Classes.IO.LOC
{
    internal class LOCFileWriter : StreamDataWriter
    {
        internal LOCFile _locfile;
        public static void Write(Stream stream, LOCFile file, int type = 2)
        {
            new LOCFileWriter(file).WriteToStream(stream, type);
        }

        private LOCFileWriter(LOCFile file) : base(false)
        {
            _locfile = file;
        }

        private void WriteToStream(Stream stream, int type)
        {
            if (_locfile == null) throw new ArgumentNullException("Loc File is null");
            WriteInt(stream, type);
            WriteInt(stream, _locfile.languages.Count);
            if (type == 2) WriteLocKeys(stream);
            WriteLanguages(stream);
            WriteLanguageEntries(stream, type);
        }


        private void WriteLocKeys(Stream stream)
        {
            stream.WriteByte(0);
            WriteInt(stream, _locfile.keys.Count);
            foreach (var key in _locfile.keys.Keys)
                WriteString(stream, key);
        }

        private void WriteLanguages(Stream stream)
        {
            foreach (var language in _locfile.languages)
            {
                WriteString(stream, language);
                WriteInt(stream, 0);
            }
        }

        private void WriteLanguageEntries(Stream stream, int type)
        {
            foreach (var language in _locfile.languages)
            {
                WriteInt(stream, 0x1337);
                stream.WriteByte(0);
                WriteString(stream, language);
                WriteInt(stream, _locfile.keys.Keys.Count);
                foreach(var locKey in _locfile.keys.Keys)
                {
                    if (type == 0) WriteString(stream, locKey);
                    WriteString(stream, _locfile.keys[locKey][language]);
                }
            }
        }

        internal void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            WriteString(stream, s, Encoding.UTF8);
        }
    }
}
