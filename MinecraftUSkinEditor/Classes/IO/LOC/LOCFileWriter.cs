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
            if (_locfile == null) throw new ArgumentNullException(nameof(_locfile));
            WriteInt(stream, type);
            WriteInt(stream, _locfile.Languages.Count);
            if (type == 2) WriteLocKeys(stream);
            WriteLanguages(stream);
            WriteLanguageEntries(stream, type);
        }


        private void WriteLocKeys(Stream stream)
        {
            stream.WriteByte(0); // dont use stringIds(ints)
            WriteInt(stream, _locfile.LocKeys.Count);
            foreach (var key in _locfile.LocKeys.Keys)
                WriteString(stream, key);
        }

        private void WriteLanguages(Stream stream)
        {
            _locfile.Languages.ForEach(language =>
            {
                WriteString(stream, language);
                WriteInt(stream, 0);
            });
        }

        private void WriteLanguageEntries(Stream stream, int type)
        {
            _locfile.Languages.ForEach(language =>
            {
                WriteInt(stream, 0x6D696B75); // :P
                stream.WriteByte(0); // <- only write when the previous written int was >0

                WriteString(stream, language);
                WriteInt(stream, _locfile.LocKeys.Keys.Count);
                foreach(var locKey in _locfile.LocKeys.Keys)
                {
                    if (type == 0) WriteString(stream, locKey);
                    WriteString(stream, _locfile.LocKeys[locKey][language]);
                }
            });
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, Convert.ToInt16(Encoding.UTF8.GetByteCount(s)));
            WriteString(stream, s, Encoding.UTF8);
        }
    }
}
