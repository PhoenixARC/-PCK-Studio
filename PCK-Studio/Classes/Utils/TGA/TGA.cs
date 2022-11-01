using PckStudio.Classes.IO;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace PckStudio.Classes.Utils.TGA
{
    internal static class TGA
    {
        private static TGAHeader _header;
        private static Bitmap _bitmap;

        private static TGAWriter _writer = new TGAWriter(true);
        private static TGAReader _reader = new TGAReader(true);

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
                header.Origin.x = ReadShort(stream);
                header.Origin.y = ReadShort(stream);
                header.Width = ReadShort(stream);
                header.Height = ReadShort(stream);
                header.BitsPerPixel = ReadBytes(stream, 1)[0];
                header.ImageDescriptor = ReadBytes(stream, 1)[0];
                DebugLogHeader(header);
                return header;
            }

            [Conditional("DEBUG")]
            private void DebugLogHeader(TGAHeader header)
            {
                Debug.WriteLine("ID length:         {0}", header.IdLength);
                Debug.WriteLine("Colourmap type:    {0}", header.Colormap.Type);
                Debug.WriteLine("Image type:        {0}", header.DataTypeCode);
                Debug.WriteLine("Colour map offset: {0}", header.Colormap.Origin);
                Debug.WriteLine("Colour map length: {0}", header.Colormap.Length);
                Debug.WriteLine("Colour map depth:  {0}", header.Colormap.Depth);
                Debug.WriteLine("X origin:          {0}", header.Origin.x);
                Debug.WriteLine("Y origin:          {0}", header.Origin.y);
                Debug.WriteLine("Width:             {0}", header.Width);
                Debug.WriteLine("Height:            {0}", header.Height);
                Debug.WriteLine("Bits per pixel:    {0}", header.BitsPerPixel);
                Debug.WriteLine("Descriptor:        {0}", header.ImageDescriptor);
            }

            public Bitmap ReadImage(Stream stream, TGAHeader header)
            {
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

        public static void Reset()
        {
            _header = default;
            _bitmap = null;
        }

        public static void LoadHeader(Stream stream)
        {
            _header = _reader.LoadHeader(stream);
        }

        public static void LoadData(Stream stream)
        {
            if (_header.Equals(TGAHeader.Empty))
                throw new TGAException("no header loaded.");
            //string idData = ReadString(stream, _header.IdLength, Encoding.ASCII);
            //Debug.WriteLine(idData);
            stream.Read(new byte[_header.IdLength], 0, _header.IdLength);
            _bitmap = _reader.ReadImage(stream, _header);
        }

        public static void SaveHeader(Stream stream, Bitmap image, TGADataTypeCode format)
        {

        }

        private struct TGAHeader
        {
            public static readonly TGAHeader Empty = default(TGAHeader);

            public byte IdLength;
            public TGADataTypeCode DataTypeCode;
            public (byte Type, short Origin/*Offset*/, short Length, byte Depth) Colormap;
            public (short x, short y) Origin;
            public short Width;
            public short Height;
            public byte BitsPerPixel;
            public byte ImageDescriptor;
        }

        public static Bitmap FromStream(Stream stream)
        {
            LoadHeader(stream);
            LoadData(stream);
            return _bitmap;
        }

    }
}
