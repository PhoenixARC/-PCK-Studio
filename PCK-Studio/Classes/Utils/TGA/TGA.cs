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
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PckStudio.Classes.IO;

namespace PckStudio.Classes.Utils.TGA
{
    /// <summary>
    /// Resources:
    ///  <http://www.paulbourke.net/dataformats/tga/>
    ///  <https://en.wikipedia.org/wiki/Truevision_TGA#File_footer_(optional)>
    /// </summary>
    internal static class TGA
    {
        private static TGAWriter _writer = new TGAWriter(true);
        private static TGAReader _reader = new TGAReader(true);

        private struct TGAHeader
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

        private struct TGAFooter
        {
            public int extensionDataOffset;
            public int developerAreaDataOffset;
        }

        private readonly struct TGAFileData
        {
            public TGAFileData(TGAHeader header, Bitmap bitmap, TGAFooter footer)
            {
                Header = header;
                Bitmap = bitmap;
                Footer = footer;
            }

            public readonly TGAHeader Header;
            public readonly Bitmap Bitmap;
            public readonly TGAFooter Footer;
        }

        private class TGAReader : StreamDataReader<TGAFileData>
        {
            public TGAReader(bool useLittleEndian) : base(useLittleEndian)
            {
            }
            
            public TGAFileData Read(Stream stream)
            {
                return ReadFromStream(stream);
            }

            private TGAHeader LoadHeader(Stream stream)
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
            
            private Bitmap LoadImage(Stream stream, TGAHeader header)
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

            private TGAFooter LoadFooter(Stream stream)
            {
                long origin = stream.Position;
                stream.Seek(-26, SeekOrigin.End);
                TGAFooter footer = new TGAFooter();
                footer.extensionDataOffset = ReadInt(stream);
                footer.developerAreaDataOffset = ReadInt(stream);
                string signature = ReadString(stream, 16, Encoding.ASCII);
                Debug.WriteLineIf(!signature.Equals("TRUEVISION-XFILE") || ReadShort(stream) != 0x002E,
                    "Footer end invalid");
                stream.Seek(origin, SeekOrigin.Begin);
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

            protected override TGAFileData ReadFromStream(Stream stream)
            {
                TGAHeader header = _reader.LoadHeader(stream);
                return new TGAFileData(header, _reader.LoadImage(stream, header), _reader.LoadFooter(stream));
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
                Debug.Assert(bitmap.Width == header.Width || bitmap.Height == header.Height,
                    "Header resolution doesn't match Image resolution");

                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, header.Width, header.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                byte[] pixel = new byte[4];
                for (int y = 0; y < header.Height; y++)
                {
                    for (int x = 0; x < header.Width; x++)
                    {
                        IntPtr pixelOffset = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                        Marshal.Copy(pixelOffset, pixel, 0, 4);
                        switch(header.DataTypeCode)
                        {
                            case TGADataTypeCode.RGB:
                                if (header.BitsPerPixel == 32)
                                {
                                    WriteBytes(stream, pixel);
                                }
                                break;
                            default:
                                throw new NotImplementedException(nameof(header.DataTypeCode));
                        }
                    }
                }

            }

            protected override void WriteToStream(Stream stream)
            {
                throw new NotImplementedException();
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

        public static Bitmap FromStream(Stream stream)
        {
            TGAFileData tgaFile = _reader.Read(stream);
            return tgaFile.Bitmap;
        }

        public static void Save(Stream stream, Bitmap bitmap, TGADataTypeCode format)
        {
            TGAHeader header = new TGAHeader()
            {
                IdLength = 0,
                DataTypeCode = format,
                Width = (short)bitmap.Width,
                Height = (short)bitmap.Height,
                BitsPerPixel = (byte)Image.GetPixelFormatSize(bitmap.PixelFormat), // TODO !?
                ///    00    |       00       |        00 00
                /// not used | pixel ordering | alpha_channel_depth
                ImageDescriptor = 0b0100,
                Origin = (0, 0),
                Colormap = (0, 0 ,0 ,0)
            };
            _writer.SaveHeader(stream, header);
            _writer.SaveImage(stream, bitmap, header);
        }
    }
}
