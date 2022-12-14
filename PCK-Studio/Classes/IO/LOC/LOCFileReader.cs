using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.LOC
{
    internal class LOCFileReader : StreamDataReader<LOCFile>
    {
        internal LOCFile _file;

        public static LOCFile Read(Stream stream)
        {
            return new LOCFileReader().ReadFromStream(stream);
        }

        private LOCFileReader() : base(false)
        {
            _file = new LOCFile();
        }

        protected override LOCFile ReadFromStream(Stream stream)
        {
            int loc_type = ReadInt(stream);
            int language_count = ReadInt(stream);
            bool lookUpKey = loc_type == 2;
            List<string> keys = lookUpKey ? ReadKeys(stream) : null;
            for (int i = 0; i < language_count; i++)
            {
                string language = ReadString(stream);
                ReadInt(stream); // unknown value
                _file.Languages.Add(language);
            }
            for (int i = 0; i < language_count; i++)
            {
                if (0 < ReadInt(stream))
                    stream.ReadByte();
                string language = ReadString(stream);
                if (!_file.Languages.Contains(language))
                    throw new KeyNotFoundException(nameof(language));
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

        private List<string> ReadKeys(Stream stream)
        {
            bool useUniqueIds = Convert.ToBoolean(stream.ReadByte());
            int keyCount = ReadInt(stream);
            List<string> keys = new List<string>(keyCount);
            for (int i = 0; i < keyCount; i++)
            {
                string key = useUniqueIds ? ReadInt(stream).ToString("X08") : ReadString(stream);
                keys.Add(key);
            }
            return keys;
        }

        private string ReadString(Stream stream)
        {
            int length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }
    }
}