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
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System;

namespace PckStudio.Classes.Utils.TGA
{
    /// <summary>
    /// Resources:
    ///  <http://www.paulbourke.net/dataformats/tga/>
    ///  <https://en.wikipedia.org/wiki/Truevision_TGA>
    /// </summary>
    public struct TGAHeader
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

    public struct TGAFooter
    {
        public int extensionDataOffset; 
        public int developerAreaDataOffset;
    }

    public struct TGATimeSpan
    {
        public short Year;
        public short Month;
        public short Day;
        public short Hour;
        public short Minute;
        public short Second;

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}({3}/{4}/{5})", Hour, Minute, Second, Day, Month, Year);
            //DateTime time = new DateTime(Year, Month, Day, Hour, Minute, Second);
            //return time.ToString();
        }
    }

    public struct TGAExtentionData
    {
        public const short ExtensionSize = 0x1EF;
        public string AuthorName;
        public string AuthorComment;
        public TGATimeSpan TimeStamp;
        public string JobID;
        public TGATimeSpan JobTime;
        public string SoftwareID;
        public int SoftwareVersion;
        public int KeyColor;
        public int PixelAspectRatio;
        public int GammaValue;
        public int ColorCorrectionOffset;
        public int PostageStampOffset;
        public int ScanLineOffset;
        public byte AttributesType;
    }

    public readonly struct TGAFileData
    {
        public TGAFileData(TGAHeader header, Bitmap bitmap, TGAFooter footer, TGAExtentionData extentionData)
        {
            if (bitmap.Width != header.Width || bitmap.Height != header.Height)
                throw new InvalidDataException("Header resolution doesn't match Image resolution");
            Header = header;
            Bitmap = bitmap;
            Footer = footer;
            ExtentionData = extentionData;
        }

        public readonly TGAHeader Header;
        public readonly Bitmap Bitmap;
        public readonly TGAFooter Footer;
        public readonly TGAExtentionData ExtentionData;
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

    public static class TGA
    {
        private static TGAWriter _writer = new TGAWriter(true);
        private static TGAReader _reader = new TGAReader(true);

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