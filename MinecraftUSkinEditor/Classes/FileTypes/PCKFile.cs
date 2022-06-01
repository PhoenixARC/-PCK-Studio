using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    using PCKProperties = Dictionary<string, string>;
    public class PCKFile
    {
        public int type { get; } = -1;
        public Dictionary<int, string> meta_data { get; } = new Dictionary<int, string>();
        public List<FileData> file_entries { get; set; } = new List<FileData>();
        public bool isLittleEndian { get; } = false;


        public class FileData
        {
            public enum EDLCType : int
            {
                DLCSkinFile,
		        DLCCapeFile,
		        DLCTextureFile,
		        DLCUIDataFile,
		        DLCLocalisationFile = 6,
		        DLCGameRulesFile,
		        DLCAudioFile,
		        DLCColourTableFile,
		        DLCGameRulesHeader,
		        DLCModelsFile = 12,
		        DLCBehavioursFile,
		        DLCMaterialFile,
            }

            public string name;
            public int type { get; }
            public byte[] data { get; set; } = null;
            public int size { get; set; }
            public PCKProperties properties { get; set; } = new PCKProperties();

            public FileData(string name, int type, int size)
            {
                this.type = type;
                this.name = name;
                this.size = size;
            }
            public FileData(int type)
            {
                name = "no_name";
                this.type = type;
            }
        }

        public PCKFile(Stream stream, bool isLittleEndian = false)
        {
            this.isLittleEndian = isLittleEndian;
            type = ReadInt(stream);
            ReadMetaData(stream);
            ReadFileEntries(stream);
        }

        public PCKFile(int type, bool isLittleEndian = false)
        {
            this.type = type;
            this.isLittleEndian = isLittleEndian;
        }

        internal void ReadMetaData(Stream stream)
        {
            int meta_entry_count = ReadInt(stream);
            bool has_xml_tag = false;
            for (; 0 < meta_entry_count; meta_entry_count--)
            {
                int index = ReadInt(stream);
                string value = ReadString(stream);
                if (value.Equals("XMLVERSION")) has_xml_tag = true;
                meta_data[index] = value;
                ReadInt(stream); // padding ????
            }
            if (has_xml_tag)
                Console.WriteLine(ReadInt(stream).ToString("X08"));
        }

        internal void ReadFileEntries(Stream stream)
        {
            int file_entry_count = ReadInt(stream);
            for (;0 < file_entry_count; file_entry_count--)
            {
                int file_size = ReadInt(stream);
                int file_type = ReadInt(stream);
                string name = ReadString(stream);
                file_entries.Add(new FileData(name, file_type, file_size));
                ReadInt(stream);
            }
            foreach (var file_entry in file_entries)
            {
                int property_count = ReadInt(stream);
                var properties = new PCKProperties();
                for(;0 < property_count; property_count--)
                {
                    int index = ReadInt(stream);
                    string key = meta_data[index];
                    string value = ReadString(stream);
                    ReadInt(stream); // padding ???
                    properties[key] = value;
                }
                file_entry.properties = properties;
                file_entry.data = new byte[file_entry.size];
                stream.Read(file_entry.data, 0, file_entry.size);
            }

        }

        internal string ReadString(Stream stream)
        {
            int len = ReadInt(stream);
            byte[] stringBuffer = new byte[len * 2];
            stream.Read(stringBuffer, 0, len * 2);
            return Encoding.BigEndianUnicode.GetString(stringBuffer, 0, len * 2);
        }

        internal int ReadInt(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            if (BitConverter.IsLittleEndian && !isLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        internal short ReadShort(Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            if (BitConverter.IsLittleEndian && !isLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        public override string ToString()
        {
            return $"type: {type}\nmeta entry count: {meta_data.Count}\nfile entry count: {file_entries.Count}";
        }

    }
}
