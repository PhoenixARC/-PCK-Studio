using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Workers;
using PckStudio.Classes._3ds;
using OMI;

namespace PckStudio.Classes.IO._3DST
{
    internal class _3DSTextureReader : IDataFormatReader<Image>, IDataFormatReader
    {
        public Image FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                Image img = null;
                using (var fs = File.OpenRead(filename))
                {
                    img = FromStream(fs);
                }
                return img;
            }
            throw new FileNotFoundException(filename);
        }

        public Image FromStream(Stream stream)
        {
            Image img = null;
            using (var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                if (reader.ReadString(4) == "3DST")
                {
                    const int offset = 32;
                    reader.ReadInt32(); // unknown value
                    _3DSTextureFormat format = reader.ReadInt32() switch
                    {
                        0 => _3DSTextureFormat.argb8,
                        1 => _3DSTextureFormat.rgb8,
                        2 => _3DSTextureFormat.rgba5551,
                        3 => _3DSTextureFormat.rgb8,
                        4 => _3DSTextureFormat.rgba4,
                        9 => _3DSTextureFormat.la4,
                        _ => _3DSTextureFormat.dontCare,
                    };
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    int bufferSize = CalcBufferSize(format, width, height);
                    stream.Seek(offset, SeekOrigin.Begin);
                    byte[] buffer = new byte[bufferSize];
                    reader.Read(buffer, 0, bufferSize);
                    img = TextureCodec.Decode(buffer, width, height, format);
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
            }
            return img;
        }

        private static int CalcBufferSize(_3DSTextureFormat textureFormat, int width, int height)
        {
            switch (textureFormat)
            {
                case _3DSTextureFormat.argb8:
                    return width * height * 4;
                case _3DSTextureFormat.rgb8:
                    return width * height * 3;
                case _3DSTextureFormat.rgba5551:
                case _3DSTextureFormat.rgb565:
                case _3DSTextureFormat.rgba4:
                case _3DSTextureFormat.la8:
                case _3DSTextureFormat.hilo8:
                    return width * height * 2;
                case _3DSTextureFormat.l8:
                case _3DSTextureFormat.a8:
                case _3DSTextureFormat.la4:
                case _3DSTextureFormat.etc1a4:
                    return width * height;
                case _3DSTextureFormat.l4:
                case _3DSTextureFormat.a4:
                case _3DSTextureFormat.etc1:
                    return width * height >> 1;
                default:
                    throw new InvalidDataException("Invalid texture format on BCH!");
            }
        }

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);
    }
}
