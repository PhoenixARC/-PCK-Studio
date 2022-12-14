using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace PckStudio.Classes._3ds.Utils
{
    /// <summary>
    ///     Format of the texture used on the PICA200.
    /// </summary>
    public enum _3DSTextureFormat
    {
        argb8 = 0,
        rgb8 = 1,
        rgba5551 = 2,
        rgb565 = 3,
        rgba4 = 4,
        la8 = 5,
        hilo8 = 6,
        l8 = 7,
        a8 = 8,
        la4 = 9,
        l4 = 0xa,
        a4 = 0xb,
        etc1 = 0xc,
        etc1a4 = 0xd,
        dontCare
    }

    internal class _3DSUtil
    {
        private static string ReadString(Stream stream, int len)
        {
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return Encoding.ASCII.GetString(buffer);
        }

        private static int ReadInt32(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private static void WriteString(Stream stream, string s)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static void WriteInt32(Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }

        public static int CalcBufferSize(_3DSTextureFormat fmt, int w, int h)
        {
            switch (fmt)
            {
                case _3DSTextureFormat.argb8:
                    return w * h * 4;
                case _3DSTextureFormat.rgb8:
                    return w * h * 3;
                case _3DSTextureFormat.rgba5551:
                case _3DSTextureFormat.rgb565:
                case _3DSTextureFormat.rgba4:
                case _3DSTextureFormat.la8:
                case _3DSTextureFormat.hilo8:
                    return w * h * 2;
                case _3DSTextureFormat.l8:
                case _3DSTextureFormat.a8:
                case _3DSTextureFormat.la4:
                case _3DSTextureFormat.etc1a4:
                    return w * h;
                case _3DSTextureFormat.l4:
                case _3DSTextureFormat.a4:
                case _3DSTextureFormat.etc1:
                    return w * h >> 1;
                default:
                    throw new InvalidDataException("Invalid texture format on BCH!");
            }
        }

        public static Image GetImageFrom3DST(Stream stream)
        {
            if (ReadString(stream, 4) == "3DST")
            {
                const int offset = 32;
                stream.Seek(8L, SeekOrigin.Begin);
                _3DSTextureFormat format = ReadInt32(stream) switch
                {
                    0 => _3DSTextureFormat.argb8,
                    1 => _3DSTextureFormat.rgb8,
                    2 => _3DSTextureFormat.rgba5551,
                    3 => _3DSTextureFormat.rgb8,
                    4 => _3DSTextureFormat.rgba4,
                    9 => _3DSTextureFormat.la4,
                    _ => _3DSTextureFormat.dontCare,
                };
                int width  = ReadInt32(stream);
                int height = ReadInt32(stream);
                int bufferSize = CalcBufferSize(format, width, height);
                stream.Seek(offset, SeekOrigin.Begin);
                byte[] buffer = new byte[bufferSize];
                stream.Read(buffer, 0, bufferSize);
                var img = TextureCodec.Decode(buffer, width, height, format);
                img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return img;
            }
            return null;
        }

        public static void SetImageTo3DST(Stream stream, Image source, _3DSTextureFormat format = _3DSTextureFormat.argb8)
        {
            // TODO: fix Encoding
            WriteString(stream, "3DST"); // 0
            WriteInt32(stream, 2); // 4 unknown
            WriteInt32(stream, (int)format); // 8
            WriteInt32(stream, source.Width); // 12
            WriteInt32(stream, source.Height); // 16
            WriteInt32(stream, 0); // 20
            WriteInt32(stream, 0); // 24
            WriteInt32(stream, 0); // 28
            source.RotateFlip(RotateFlipType.RotateNoneFlipY);
            byte[] buffer = TextureCodec.Encode(new Bitmap(source), format);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
