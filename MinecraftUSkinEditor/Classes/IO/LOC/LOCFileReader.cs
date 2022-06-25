using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PckStudio.Classes.FileTypes;

namespace PckStudio.Classes.IO.LOC
{
    internal class LOCFileReader
    {
        internal LOCFile _file;

        public static LOCFile Read(Stream stream)
        {
            return new LOCFileReader().ReadFile(stream);
        }

        private LOCFileReader()
        {
            _file = new LOCFile();
        }

        private LOCFile ReadFile(Stream stream)
        {
            int loc_type = ReadInt(stream);
            int language_count = ReadInt(stream);
            List<string> keys = null;
            if (loc_type == 2) keys = ReadKeys(stream);
            for (int i = 0; i < language_count; i++)
            {
                string language = ReadString(stream);
                _file.languages.Add(language);
                ReadInt(stream); // padding ???
            }
            for (int i = 0; i < language_count; i++)
            {
                ReadInt(stream);
                stream.ReadByte();
                string language = ReadString(stream);
                if (!_file.languages.Contains(language))
                    throw new Exception("language not found");
                int count = ReadInt(stream);
                for (int j = 0; j < count; j++)
                {
                    string key = loc_type == 2 ? keys[j] : ReadString(stream);
                    string value = ReadString(stream);
                    if (_file.keys.ContainsKey(key))
                    {
                        _file.keys[key].Add(language, value);
                        continue;
                    }
                    var dict = new Dictionary<string, string>();
                    dict.Add(language, value);
                    _file.keys.Add(key, dict);
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

        internal short ReadShort(Stream stream)
        {
            byte[] bytes = new byte[2];
            stream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        internal int ReadInt(Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        internal string ReadString(Stream stream)
        {
            int length = ReadShort(stream);
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return Encoding.UTF8.GetString(buffer, 0, length);
        }
    }
}