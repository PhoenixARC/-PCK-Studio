using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO
{
    public class StreamDataReader
    {
        private static bool useLittleEndian;
        protected static bool IsUsingLittleEndian => useLittleEndian;
        protected StreamDataReader(bool littleEndian)
        {
            useLittleEndian = littleEndian;
        }

        protected static string ReadString(Stream stream, int length, Encoding encoding)
        {
            byte[] buffer = ReadBytes(stream, length << Convert.ToInt32(encoding is UnicodeEncoding));
            return encoding.GetString(buffer);
        }

        protected static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        protected static ushort ReadUShort(Stream stream) => (ushort)ReadShort(stream);
        protected static short ReadShort(Stream stream)
        {
            byte[] bytes = ReadBytes(stream, 2);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        protected static uint ReadUInt(Stream stream) => (uint)ReadInt(stream);
        protected static int ReadInt(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
        
        protected static ulong ReadULong(Stream stream) => (ulong)ReadLong(stream);
        protected static long ReadLong(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 8);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
