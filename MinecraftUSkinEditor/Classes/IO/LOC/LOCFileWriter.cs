using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PckStudio.Classes.IO.LOC
{
    internal class LOCFileWriter
    {
        internal LOCFile _file;
        public static void Write(Stream stream, LOCFile file, int type = 0)
        {
            new LOCFileWriter(file).WriteToStream(stream, type);
        }

        private LOCFileWriter(LOCFile file)
        {
            _file = file;
        }

        private void WriteToStream(Stream stream, int type)
        {
            WriteInt(stream, type);
            WriteInt(stream, _file.languages.Count);
            if (type == 2) WriteKeys(stream);

            WriteLanguages(stream);
        }


        internal void WriteKeys(Stream stream)
        {
            stream.WriteByte(0);
            // TODO: find all keys and write them
            //WriteInt(stream, )
        }

        internal void WriteLanguages(Stream stream)
        {
            foreach (var language in _file.languages.Keys)
            {
                WriteString(stream, language);
                WriteInt(stream, 0); // padding ???
            }
        }

        internal void WriteLanguageEntries(Stream stream)
        {

        }

        internal void WriteShort(Stream stream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }
        internal void WriteInt(Stream stream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        internal void WriteString(Stream stream, string s)
        {
            WriteShort(stream, (short)s.Length);
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
