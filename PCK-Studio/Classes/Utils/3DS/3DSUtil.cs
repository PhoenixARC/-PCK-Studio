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
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return Encoding.ASCII.GetString(buffer);
        }

        private static int ReadInt32(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
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
                RenderBase.OTextureFormat otextureFormat = ReadInt32(stream) switch
                {
                    0 => RenderBase.OTextureFormat.rgba8,
                    1 => RenderBase.OTextureFormat.rgb8,
                    2 => RenderBase.OTextureFormat.rgba5551,
                    3 => RenderBase.OTextureFormat.rgb8,
                    4 => RenderBase.OTextureFormat.rgba4,
                    9 => RenderBase.OTextureFormat.la4,
                    _ => RenderBase.OTextureFormat.dontCare,
                };
                int width = ReadInt32(stream);
                int height = ReadInt32(stream);
                int bufferSize = CalcBufferSize(otextureFormat, width, height);
                stream.Seek(offset, SeekOrigin.Begin);
                byte[] buffer = new byte[bufferSize];
                stream.Read(buffer, 0, bufferSize);
                var img = TextureCodec.decode(buffer, width, height, otextureFormat);
                img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return img;
            }
            return null;
        }

        public static void SetImageTo3DST(Stream stream, Image source, RenderBase.OTextureFormat format = RenderBase.OTextureFormat.rgba8)
        {
            throw new NotImplementedException();
            //if (ReadString(stream, 4) == "3DST")
            //{
            //    int offset = 32;
            //    stream.Seek(8L, SeekOrigin.Begin);
            //    RenderBase.OTextureFormat otextureFormat = ReadInt32(stream) switch
            //    {
            //        0 => RenderBase.OTextureFormat.rgba8,
            //        1 => RenderBase.OTextureFormat.rgb8,
            //        2 => RenderBase.OTextureFormat.rgba5551,
            //        3 => RenderBase.OTextureFormat.rgb8,
            //        4 => RenderBase.OTextureFormat.rgba4,
            //        9 => RenderBase.OTextureFormat.la4,
            //        _ => RenderBase.OTextureFormat.dontCare,
            //    };
            //    int width = ReadInt32(stream);
            //    int height = ReadInt32(stream);
            //    int bufferSize = CalcBufferSize(otextureFormat, width, height);
            //    stream.Seek(offset, SeekOrigin.Begin);
            //    byte[] buffer = new byte[bufferSize];
            //    stream.Read(buffer, 0, bufferSize);
            //    var img = TextureCodec.decode(buffer, width, height, otextureFormat);
            //    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //}
        }
    }
}
