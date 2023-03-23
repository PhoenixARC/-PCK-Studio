/* Copyright (c) 2022-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using PckStudio.Classes.FileTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO
{
    public abstract class StreamDataReader<T>
    {
        private static bool useLittleEndian;
        protected static bool IsUsingLittleEndian => useLittleEndian;
        protected abstract T ReadFromStream(Stream stream);

        protected StreamDataReader(bool useLittleEndian)
        {
            StreamDataReader<T>.useLittleEndian = useLittleEndian;
        }

        protected static string ReadString(Stream stream, int length, Encoding encoding)
        {
            byte[] buffer = ReadBytes(stream, length << Convert.ToInt32(encoding is UnicodeEncoding));
            return encoding.GetString(buffer).TrimEnd('\0');
        }

        protected static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        protected static bool ReadBool(Stream stream)
        {
            return stream.ReadByte() != 0;
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
        
        protected static float ReadFloat(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }
    }
}
