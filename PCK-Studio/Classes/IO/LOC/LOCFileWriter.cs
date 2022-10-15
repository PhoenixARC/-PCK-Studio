using PckStudio.Classes.FileTypes;
using System;
using System.IO;
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
            _ = _locfile ?? throw new ArgumentNullException(nameof(_locfile));
            WriteInt(stream, type);
            WriteInt(stream, _locfile.Languages.Count);
            if (type == 2) WriteLocKeys(stream);
            WriteLanguages(stream, type);
            WriteLanguageEntries(stream, type);
        }


        private void WriteLocKeys(Stream stream)
        {
            stream.WriteByte(0); // dont use stringIds(ints)
            WriteInt(stream, _locfile.LocKeys.Count);
            foreach (var key in _locfile.LocKeys.Keys)
                WriteString(stream, key);
        }

        private void WriteLanguages(Stream stream, int type)
        {
            foreach(var language in _locfile.Languages)
            {
                WriteString(stream, language);
                
                //Calculate the size of the language entry

                int size = 0;
                size += sizeof(int); // null long
                size += sizeof(byte); // null byte
                size += (sizeof(short) + Encoding.UTF8.GetByteCount(language)); // language name string
                size += sizeof(int); // key count

                foreach (var locKey in _locfile.LocKeys.Keys)
                {
                    if (type == 0) size += (2 + Encoding.UTF8.GetByteCount(locKey)); // loc key string
                    size += (2 + Encoding.UTF8.GetByteCount(_locfile.LocKeys[locKey][language])); // loc key string
                }

                WriteInt(stream, size);
            };
        }

        private void WriteLanguageEntries(Stream stream, int type)
        {
            foreach (var language in _locfile.Languages)
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
            };
        }

        private void WriteString(Stream stream, string s)
        {
            WriteShort(stream, Convert.ToInt16(Encoding.UTF8.GetByteCount(s)));
            WriteString(stream, s, Encoding.UTF8);
        }
    }
}
