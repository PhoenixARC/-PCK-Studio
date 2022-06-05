using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class COLFile
    {
        public struct ColorEntry
        {
            public string name;
            public uint rgb;
        }
        public struct ExtendedColorEntry
        {
            public string name;
            public uint argb;
            public uint rgb;
            public uint unk;
        }

        public List<ColorEntry> entries = new List<ColorEntry>();
        public List<ExtendedColorEntry> waterEntries = new List<ExtendedColorEntry>();
        public void Open(Stream stream)
        {
            int has_water_colors = ReadInt(stream);
            int color_entries = ReadInt(stream);
            for (int i = 0; i < color_entries; i++)
            {
                ColorEntry entry = new ColorEntry();
                entry.name = ReadString(stream);
                entry.rgb = ReadUint(stream);
                entries.Add(entry);
            }
            if (has_water_colors == 1)
            {
                int water_color_entries = ReadInt(stream);
                for (int i = 0; i < water_color_entries; i++)
                {
                    ExtendedColorEntry entry = new ExtendedColorEntry();
                    entry.name = ReadString(stream);
                    entry.argb = ReadUint(stream);
                    entry.rgb = ReadUint(stream);
                    entry.unk = ReadUint(stream);
                    waterEntries.Add(entry);
                }
            }
        }


        private string ReadString(Stream stream)
        {
            byte[] bytes = new byte[2];
            stream.Read(bytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            short strlen = BitConverter.ToInt16(bytes, 0);
            byte[] stringBuffer = new byte[strlen];
            stream.Read(stringBuffer, 0, strlen);
            return Encoding.UTF8.GetString(stringBuffer, 0, strlen);
        }

        private int ReadInt(Stream stream)
        {
            return (int)ReadUint(stream);
        }

        private uint ReadUint(Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private void WriteUint(Stream stream, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            stream.Write(bytes, 0, 4);
        }
        private void WriteInt(Stream stream, int value)
        {
            WriteUint(stream, (uint)value);
        }

        private void WriteString(Stream stream, string s)
        {
            byte[] bytes = BitConverter.GetBytes((short)s.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            byte[] stringBuffer = Encoding.UTF8.GetBytes(s);
            stream.Write(bytes, 0, 2);
            stream.Write(stringBuffer, 0, s.Length);
        }

        public void Save(Stream stream)
        {
            WriteInt(stream, Convert.ToInt32(waterEntries.Count > 0));
            WriteInt(stream, entries.Count);
            foreach (var colorEntry in entries)
            {
                WriteString(stream, colorEntry.name);
                WriteUint(stream, colorEntry.rgb);
            }
            if (waterEntries.Count > 0)
            {
                WriteInt(stream, waterEntries.Count);
                foreach (var colorEntry in waterEntries)
                {
                    WriteString(stream, colorEntry.name);
                    WriteUint(stream, colorEntry.rgb);
                    WriteUint(stream, colorEntry.argb);
                    WriteUint(stream, colorEntry.unk);
                }
            }
        }
    }
}
