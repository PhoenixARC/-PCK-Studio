using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PckStudio.Classes.IO;

namespace PckStudio.Classes.Utils.TGA
{
    internal static class TGA
    {
        private static TGAWriter _writer = new TGAWriter(true);
        private static TGAReader _reader = new TGAReader(true);

        // http://www.paulbourke.net/dataformats/tga/
        private ref struct TGAHeader
        {
            public byte IdLength;
            public TGADataTypeCode DataTypeCode;
            public (byte Type, short Origin/*Offset*/, short Length, byte Depth) Colormap;
            public (short X, short Y) Origin;
            public short Width;
            public short Height;
            public byte BitsPerPixel;
            public byte ImageDescriptor;
        }

        private ref struct TGAFooter
        {
            public byte[] extensionData;
            public byte[] developerAreaData;
        }

        private class TGAReader : StreamDataReader
        {
            public TGAReader(bool useLittleEndian) : base(useLittleEndian)
            {
            }

            public TGAHeader LoadHeader(Stream stream)
            {
                var header = new TGAHeader();
                byte[] bytes = ReadBytes(stream, 3);
                (header.IdLength, header.Colormap.Type, header.DataTypeCode) = (bytes[0], bytes[1], (TGADataTypeCode)bytes[2]);
                header.Colormap.Origin = ReadShort(stream);
                header.Colormap.Length = ReadShort(stream);
                header.Colormap.Depth = ReadBytes(stream, 1)[0];
                header.Origin.X = ReadShort(stream);
                header.Origin.Y = ReadShort(stream);
                header.Width = ReadShort(stream);
                header.Height = ReadShort(stream);
                header.BitsPerPixel = ReadBytes(stream, 1)[0];
                header.ImageDescriptor = ReadBytes(stream, 1)[0];
                return header;
            }

            public Bitmap LoadImage(Stream stream, TGAHeader header)
            {
                string idData = ReadString(stream, header.IdLength, Encoding.ASCII);
                Debug.WriteLineIf(header.IdLength > 0, $"Image ID: {idData}");
                Bitmap bitmap = new Bitmap(header.Width, header.Height);
                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, header.Width, header.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb
                    );
                int formatSize = header.BitsPerPixel / 8;
                for (int y = 0; y < header.Height; y++)
                {
                    for (int x = 0; x < header.Width; x++)
                    {
                        IntPtr pixelOffset = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                        switch (header.DataTypeCode)
                        {
                            case TGADataTypeCode.RGB:
                                WritePixel(pixelOffset, ReadBytes(stream, formatSize), formatSize);
                                break;
                            case TGADataTypeCode.RLE_RGB:
                                //{             /* Compressed */
                                //    //if (fread(p, 1, formatSize + 1, fptr) != formatSize + 1)
                                //    //{
                                //    //    Console.Error.WriteLine("Unexpected end of file at pixel %d\n", i);
                                //    //    return;
                                //    //}
                                //    int j = p[0] & 0x7f;
                                //    WritePixel(pixels[n], &(p[1]), formatSize);
                                //    n++;
                                //    if (p[0] & 0x80)
                                //    {         /* RLE chunk */
                                //        for (int i = 0; i < j; i++)
                                //        {
                                //            WritePixel(&(pixels[n]), &(p[1]), formatSize);
                                //            n++;
                                //        }
                                //    }
                                //    else
                                //    {                   /* Normal chunk */
                                //        for (int i = 0; i < j; i++)
                                //        {
                                //            if (fread(p, 1, formatSize, fptr) != formatSize)
                                //            {
                                //                Console.Error.WriteLine("Unexpected end of file at pixel %d\n", i);
                                //                return;
                                //            }
                                //            WritePixel(&(pixels[n]), p, formatSize);
                                //            n++;
                                //        }
                                //    }
                                //}
                                break;
                            default:
                                Debug.WriteLine("Type={0}", header.DataTypeCode);
                                break;
                        }
                    }
                }
                bitmap.UnlockBits(bitmapData);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return bitmap;
            }

            public TGAFooter LoadFooter(Stream stream)
            {
                /// <https://en.wikipedia.org/wiki/Truevision_TGA#File_footer_(optional)>
                TGAFooter footer = new TGAFooter();
                footer.extensionData = new byte[ReadInt(stream)];
                footer.developerAreaData = new byte[ReadInt(stream)];
                string signature = ReadString(stream, 16, Encoding.ASCII);
                Debug.Assert(signature.Equals("TRUEVISION-XFILE") || ReadShort(stream) == 0x002E,
                    "Footer end invalid");
                return footer;
            }

            void WritePixel(IntPtr destination, byte[] data, int formatSize)
            {
                if (formatSize == 4) // 32-bit
                {
                    Marshal.Copy(new byte[4]{
                        data[0],
                        data[1],
                        data[2],
                        data[3],
                    }, 0, destination, 4);
                }
                else if (formatSize == 3) // 24-bit
                {
                    Marshal.Copy(new byte[4]{
                        data[0],
                        data[1],
                        data[2],
                        byte.MaxValue,
                    }, 0, destination, 4);
                }
                else if (formatSize == 2) // 16-bit
                {
                    // warning: could be incorrect
                    Marshal.Copy(new byte[4]{
                        (byte)((data[1] & 0x7c) << 1),
                        (byte)(((data[1] & 0x03) << 6) | ((data[0] & 0xe0) >> 2)),
                        (byte)((data[0] & 0x1f) << 3),
                        ((byte)(data[1] & 0x80)),
                    }, 0, destination, 4);

                }
            }
        }

        private class TGAWriter : StreamDataWriter
        {
            public TGAWriter(bool useLittleEndian) : base(useLittleEndian)
            {
            }

            public void SaveHeader(Stream stream, TGAHeader header)
            {
                WriteBytes(stream, new byte[]
                {
                    header.IdLength,
                    header.Colormap.Type,
                    (byte)header.DataTypeCode
                });
                WriteShort(stream, header.Colormap.Origin);
                WriteShort(stream, header.Colormap.Length);
                stream.WriteByte(header.Colormap.Depth);
                WriteShort(stream, header.Origin.X);
                WriteShort(stream, header.Origin.Y);
                WriteShort(stream, header.Width);
                WriteShort(stream, header.Height);
                WriteBytes(stream, new byte[]
                {
                    header.BitsPerPixel,
                    header.ImageDescriptor,
                });
            }

            public void SaveImage(Stream stream, Bitmap bitmap, TGAHeader header)
            {

            }
        }

        public enum TGADataTypeCode : byte
        {
            /// <summary>
            /// No image data included.
            /// </summary>
            NO_DATA = 0,
            /// <summary>
            /// Uncompressed, color-mapped images.
            /// </summary>
            COLORMAPPED = 1,
            /// <summary>
            /// Uncompressed, RGB images.
            /// </summary>
            RGB = 2,
            /// <summary>
            /// Uncompressed, black and white images.
            /// </summary>
            BLACK_WHITE = 3,
            /// <summary>
            /// Runlength encoded color-mapped images.
            /// </summary>
            RLE_COLORMAPPED = 9,
            /// <summary>
            /// Runlength encoded RGB images.
            /// </summary>
            RLE_RGB = 10,
            /// <summary>
            /// Compressed, black and white images.
            /// </summary>
            COMPRESSED_BLACK_WHITE = 11,
            /// <summary>
            /// Compressed color-mapped data, using Huffman, Delta, and runlength encoding.
            /// </summary>
            COMPRESSED_RLE_COLORMAPPED = 32,
            /// <summary>
            /// Compressed color-mapped data, using Huffman, Delta, and runlength encoding. 4-pass quadtree-type process.
            /// </summary>
            COMPRESSED_RLE_COLORMAPPED_4 = 33,
        }

        private static TGAHeader LoadHeader(Stream stream)
        {
            return _reader.LoadHeader(stream);
        }

        private static Bitmap LoadData(Stream stream, TGAHeader header)
        {
            return _reader.LoadImage(stream, header);
        }

        public static Bitmap FromStream(Stream stream)
        {
            TGAHeader header = LoadHeader(stream);
            Bitmap bitmap = LoadData(stream, header);
            _reader.LoadFooter(stream);
            return bitmap;
        }

        public static void Save(Stream stream, Bitmap bitmap, TGADataTypeCode format)
        {
            TGAHeader header = new TGAHeader()
            {
                IdLength = 0,
                DataTypeCode = format,
                Width = (short)bitmap.Width,
                Height = (short)bitmap.Height,
                BitsPerPixel = 32, // TODO !?
                /// 00 |             00 |               00 00
                ///    | pixel ordering | alpha_channel_depth
                ImageDescriptor = 0b0100,
                Origin = (0, 0),
                Colormap = (0, 0 ,0 ,0)
            };
            _writer.SaveHeader(stream, header);
            _writer.SaveImage(stream, bitmap, header);
        }
    }
}
