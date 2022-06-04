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
        public struct COLEntry
        {
            public string name;
            public uint color;
        }

        List<byte> extradata = new List<byte>();
        public List<COLEntry> entries = new List<COLEntry>();
        public List<COLEntry> waterEntries = new List<COLEntry>();
        public void Open(Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            int has_water_colors = BitConverter.ToInt32(buffer, 0);
            int color_entries = BitConverter.ToInt32(buffer, 4);
            for (int i = 0; i < color_entries; i++)
            {
                COLEntry entry = new COLEntry();
                entry.name = ReadString(stream);
                entry.color = ReadUint(stream);
                entries.Add(entry);
            }
            if (has_water_colors == 1)
            {
                int water_color_entries = ReadInt(stream);
                for (int i = 0; i < water_color_entries; i++)
                {
                    COLEntry entry = new COLEntry();
                    entry.name = ReadString(stream);
                    entry.color = ReadUint(stream);
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
            int strlen = BitConverter.ToInt16(bytes, 0);
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
                WriteUint(stream, colorEntry.color);
            }
            if (waterEntries.Count > 0)
            {
                WriteInt(stream, waterEntries.Count);
                foreach (var colorEntry in waterEntries)
                {
                    WriteString(stream, colorEntry.name);
                    WriteUint(stream, colorEntry.color);
                }
            }
        }
    }
}
