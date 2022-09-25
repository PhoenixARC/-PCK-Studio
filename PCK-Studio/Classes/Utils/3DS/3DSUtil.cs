using Ohana3DS_Rebirth.Ohana;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace PckStudio.Classes.Utils
{
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

        public static int CalcBufferSize(RenderBase.OTextureFormat fmt, int w, int h)
        {
            switch (fmt)
            {
                case RenderBase.OTextureFormat.rgba8:
                    return w * h * 4;
                case RenderBase.OTextureFormat.rgb8:
                    return w * h * 3;
                case RenderBase.OTextureFormat.rgba5551:
                case RenderBase.OTextureFormat.rgb565:
                case RenderBase.OTextureFormat.rgba4:
                case RenderBase.OTextureFormat.la8:
                case RenderBase.OTextureFormat.hilo8:
                    return w * h * 2;
                case RenderBase.OTextureFormat.l8:
                case RenderBase.OTextureFormat.a8:
                case RenderBase.OTextureFormat.la4:
                case RenderBase.OTextureFormat.etc1a4:
                    return w * h;
                case RenderBase.OTextureFormat.l4:
                case RenderBase.OTextureFormat.a4:
                case RenderBase.OTextureFormat.etc1:
                    return w * h >> 1;
                default:
                    throw new InvalidDataException("Invalid texture format on BCH!");
            }
        }

        public static Image GetImageFrom3DST(Stream stream)
        {
            if (ReadString(stream, 4) == "3DST")
            {
                int offset = 32;
                stream.Seek(8L, SeekOrigin.Begin);
                RenderBase.OTextureFormat format = ReadInt32(stream) switch
                {
                    0 => RenderBase.OTextureFormat.rgba8,
                    1 => RenderBase.OTextureFormat.rgb8,
                    2 => RenderBase.OTextureFormat.rgba5551,
                    3 => RenderBase.OTextureFormat.rgb8,
                    4 => RenderBase.OTextureFormat.rgba4,
                    9 => RenderBase.OTextureFormat.la4,
                    _ => RenderBase.OTextureFormat.dontCare,
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

        public static void SetImageTo3DST(Stream stream, Image source, RenderBase.OTextureFormat format = RenderBase.OTextureFormat.rgba8)
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
            WriteInt32(stream, 0); // 32
            source.RotateFlip(RotateFlipType.RotateNoneFlipY);
            byte[] buffer = TextureCodec.Encode(new Bitmap(source), format);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
