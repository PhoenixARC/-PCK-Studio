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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.IO
{
    internal abstract class StreamDataWriter
    {
        private static bool useLittleEndian;
        protected static bool IsUsingLittleEndian => useLittleEndian;

        protected StreamDataWriter(bool littleEndian)
        {
            useLittleEndian = littleEndian;
        }

        protected abstract void WriteToStream(Stream stream);

        protected static void WriteBool(Stream stream, bool state)
        {
            stream.WriteByte((byte)(state ? 1 : 0));
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

        protected static void WriteFloat(Stream stream, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian && !useLittleEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 4);
        }
    }
}
