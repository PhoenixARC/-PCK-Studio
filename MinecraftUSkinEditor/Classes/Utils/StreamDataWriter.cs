using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.Utils
{
    internal class StreamDataWriter
    {
        private static bool useLittleEndian;
        protected static bool IsUsingLittleEndian => useLittleEndian;

        protected StreamDataWriter(bool littleEndian)
        {
            useLittleEndian = littleEndian;
        }

        protected static void WriteUShort(Stream stream, ushort value) => WriteShort(stream, (short)value);

        protected static void WriteShort(Stream stream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(bytes);
            WriteBytes(stream, bytes, 2);
        }

        protected static void WriteUInt(Stream stream, uint value) => WriteInt(stream, (int)value);
        protected static void WriteInt(Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 4);
        }
        
        protected static void WriteULong(Stream stream, ulong value) => WriteLong(stream, (long)value);
        protected static void WriteLong(Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 8);
        }

        protected static void WriteString(Stream stream, string s, Encoding encoding)
            => WriteBytes(stream, encoding.GetBytes(s));

        protected static void WriteBytes(Stream stream, byte[] bytes) => WriteBytes(stream, bytes, bytes.Length);
        protected static void WriteBytes(Stream stream, byte[] bytes, int count)
        {
            stream.Write(bytes, 0, count);
        }

    }
}
