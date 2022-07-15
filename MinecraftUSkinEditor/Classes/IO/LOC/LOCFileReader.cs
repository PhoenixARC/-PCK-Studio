using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;
using PckStudio.Classes.Utils;

namespace PckStudio.Classes.IO.LOC
{
    internal class LOCFileReader : StreamDataReader
    {
        internal LOCFile _file;

        public static LOCFile Read(Stream stream)
        {
            return new LOCFileReader().ReadFile(stream);
        }

        private LOCFileReader() : base(false)
        {
            _file = new LOCFile();
        }

        private LOCFile ReadFile(Stream stream)
        {
            int loc_type = ReadInt(stream);
            int language_count = ReadInt(stream);
            bool lookUpKey = loc_type == 2;
            List<string> keys = lookUpKey ? ReadKeys(stream) : null;
            for (int i = 0; i < language_count; i++)
            {
                string language = ReadString(stream);
                _file.Languages.Add(language);
                ReadInt(stream); // padding ???
            }
            for (int i = 0; i < language_count; i++)
            {
                ReadInt(stream);
                stream.ReadByte();
                string language = ReadString(stream);
                if (!_file.Languages.Contains(language))
                    throw new Exception("language not found");
                int count = ReadInt(stream);
                for (int j = 0; j < count; j++)
                {
                    string key = lookUpKey ? keys[j] : ReadString(stream);
                    string value = ReadString(stream);
                    _file.SetLocEntry(key, language, value);
                }
            }
            return _file;
        }

        internal List<string> ReadKeys(Stream stream)
        {
            stream.ReadByte(); // unknown
            int keyCount = ReadInt(stream);
            List<string> keys = new List<string>(keyCount);
            for (int i = 0; i < keyCount; i++)
            {
                string key = ReadString(stream);
                keys.Add(key);
            }
            return keys;
        }

        internal string ReadString(Stream stream)
        {
            int length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }
    }
}