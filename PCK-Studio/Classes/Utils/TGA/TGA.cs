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
        public byte[] Id;
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

    public struct TGAExtentionData
    {
        public const short ExtensionSize = 0x1EF;
        public string AuthorName;
        public string AuthorComment;
        public DateTime TimeStamp;
        public string JobID;
        public TimeSpan JobTime;
        public string SoftwareID;
        public byte[] SoftwareVersion;
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

        public void Save(string filename)
        {
            throw new NotImplementedException();
        }
        
        public void Save(string filename, TGADataTypeCode format)
        {
            throw new NotImplementedException();
        }
        
        public void Save(Stream stream, TGADataTypeCode format)
        {
            throw new NotImplementedException();
        }
    }

    public static class TGA
    {
        private static TGAWriter _writer = new TGAWriter();
        private static TGAReader _reader = new TGAReader();

        public static Bitmap FromFile(string filename)
        {
            if (Path.GetFileNameWithoutExtension(filename) != ".tga")
                throw new InvalidDataException("File does not end in .tga .");
            if (File.Exists(filename))
            {
                using var fs = File.OpenRead(filename);
                return FromStream(fs);
            }
            throw new FileNotFoundException(filename);
        }

        public static Bitmap FromStream(Stream stream)
        {
            return LoadTGAFileData(stream).Bitmap;
        }
        
        private static TGAFileData LoadTGAFileData(Stream stream)
        {
            return _reader.Read(stream);
        }

        public static void Save(Stream stream, Bitmap bitmap, TGADataTypeCode format)
        {
            TGAHeader header = new TGAHeader()
            {
                Id = Array.Empty<byte>(),
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