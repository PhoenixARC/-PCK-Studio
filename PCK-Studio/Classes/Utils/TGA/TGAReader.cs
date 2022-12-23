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
using System.Collections.Generic;
using System.Linq;

namespace PckStudio.Classes.Utils.TGA
{
    internal class TGAReader : StreamDataReader<TGAFileData>
    {
        public TGAReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }

        public TGAFileData Read(Stream stream) => ReadFromStream(stream);

        protected override TGAFileData ReadFromStream(Stream stream)
        {
            TGAHeader header = LoadHeader(stream);
            Bitmap image = LoadImage(stream, header);
            TGAFooter footer = LoadFooter(stream);
            TGAExtentionData extentionData = LoadExtentionData(stream, footer);
            return new TGAFileData(header, image, footer, extentionData);
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
            DebugLogHeader(header);

            if (header.DataTypeCode == TGADataTypeCode.NO_DATA)
                return null;

            Bitmap bitmap = new Bitmap(header.Width, header.Height);
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, header.Width, header.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
                );

            if (TGATypeCodeHandler.TryGetValue(header.DataTypeCode, out var func))
                func?.Invoke(stream, header, bitmapData);

            bitmap.UnlockBits(bitmapData);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        private static int GetColorChannelCount(TGAHeader header)
        {
            return header.BitsPerPixel / 8;
        }

        static readonly Dictionary<TGADataTypeCode, Action<Stream, TGAHeader, BitmapData>> TGATypeCodeHandler = new Dictionary<TGADataTypeCode, Action<Stream, TGAHeader, BitmapData>>()
        {
            [TGADataTypeCode.RGB] = TGAHandleRGB,
            [TGADataTypeCode.RLE_RGB] = TGAHandleRLERGB,
        };

        private static void TGAHandleRGB(Stream stream, TGAHeader header, BitmapData bitmapData)
        {
            int formatSize = GetColorChannelCount(header);
            for (int y = 0; y < header.Height; y++)
            {
                for (int x = 0; x < header.Width; x++)
                {
                    IntPtr pixelOffset = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                    WritePixel(pixelOffset, ReadBytes(stream, formatSize), formatSize);
                }
            }
        }
        
        private static void TGAHandleRLERGB(Stream stream, TGAHeader header, BitmapData bitmapData)
        {
            int formatSize = GetColorChannelCount(header);
            int x = 0, y = 0;

            while (header.Width * header.Height > x*y)
            {
                IntPtr pixelPosition = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                var flagValue = ReadBytes(stream, 1);
                if ((flagValue[0] & 0x80) != 0)
                {
                    byte[] pixelColor = ReadBytes(stream, formatSize);
                    int loopCount = (flagValue[0] & 0x7f);
                    for (int _ = 0; _ < loopCount; _++)
                    {
                        pixelPosition = bitmapData.Scan0 + 4 * x++ + bitmapData.Stride * y;
                        Debug.WriteLine("Writting pixel to {0}({1}, {2})", pixelPosition, x, y);
                        WritePixel(pixelPosition, pixelColor, formatSize);
                    }
                }
                else
                {
                    int loopCount = flagValue[0];
                    for (int _ = 0; _ < loopCount; _++)
                    {
                        pixelPosition = bitmapData.Scan0 + 4 * x++ + bitmapData.Stride * y;
                        Debug.WriteLine("Writting pixel to {0}({1}, {2})", pixelPosition, x, y);
                        WritePixel(pixelPosition, ReadBytes(stream, formatSize), formatSize);
                        x++;
                    }
                }
                if (x >= header.Width)
                {
                    x = 0;
                    y++;
                }
            }
        }

        private TGAFooter LoadFooter(Stream stream)
        {
            long origin = stream.Position;
            stream.Seek(-26, SeekOrigin.End);

            TGAFooter footer = new TGAFooter();

            footer.extensionDataOffset = ReadInt(stream); // optional
            footer.developerAreaDataOffset = ReadInt(stream); // optional
            Debug.WriteLine("Extension Data Offset:         {0:x}", footer.extensionDataOffset);
            Debug.WriteLine("Developer Area Data Offset:    {0:x}", footer.developerAreaDataOffset);

            string signature = ReadString(stream, 16, Encoding.ASCII);
            Debug.WriteLineIf(!signature.Equals("TRUEVISION-XFILE") || ReadShort(stream) != 0x002E,
                "Footer end invalid");

            stream.Seek(origin, SeekOrigin.Begin);
            return footer;
        }

        private TGAExtentionData LoadExtentionData(Stream stream, TGAFooter footer)
        {
            if (footer.extensionDataOffset > 0)
            {
                stream.Seek(footer.extensionDataOffset, SeekOrigin.Begin);
                if (ReadShort(stream) == TGAExtentionData.ExtensionSize)
                {
                    TGAExtentionData extentionData = new TGAExtentionData();
                    extentionData.AuthorName = ReadString(stream, 41, Encoding.ASCII);
                    extentionData.AuthorComment = ReadString(stream, 324, Encoding.ASCII);
                    extentionData.TimeStamp = new TGATimeSpan()
                    {
                        Month = ReadShort(stream),
                        Day = ReadShort(stream),
                        Year = ReadShort(stream),
                        Hour = ReadShort(stream),
                        Minute = ReadShort(stream),
                        Second = ReadShort(stream),
                    };
                    extentionData.JobID = ReadString(stream, 41, Encoding.ASCII);
                    extentionData.JobTime = new TGATimeSpan()
                    {
                        Hour = ReadShort(stream),
                        Minute = ReadShort(stream),
                        Second = ReadShort(stream),
                        // Dummy value/Minimum valid time
                        Month = 1, Day = 1, Year = 1970,
                    };
                    extentionData.SoftwareID = ReadString(stream, 41, Encoding.ASCII);
                    byte[] version = ReadBytes(stream, 3);
                    extentionData.SoftwareVersion = version[2] << 16 | version[1] << 8 | version[0];
                    extentionData.KeyColor = ReadInt(stream);
                    extentionData.PixelAspectRatio = ReadInt(stream);
                    extentionData.GammaValue = ReadInt(stream);
                    extentionData.ColorCorrectionOffset = ReadInt(stream);
                    extentionData.PostageStampOffset = ReadInt(stream);
                    extentionData.ScanLineOffset = ReadInt(stream);
                    extentionData.AttributesType = (byte)stream.ReadByte();
                    DebugLogExtentionData(extentionData);

                    return extentionData;
                }
            }
            return default;
        }

        private static void WritePixel(IntPtr destination, byte[] data, int colorChannelCount)
        {
            if (colorChannelCount == 4) // 32-bit
            {
                Marshal.Copy(new byte[4]{
                        data[0],
                        data[1],
                        data[2],
                        data[3],
                    }, 0, destination, 4);
            }
            else if (colorChannelCount == 3) // 24-bit
            {
                Marshal.Copy(new byte[4]{
                        data[0],
                        data[1],
                        data[2],
                        byte.MaxValue,
                    }, 0, destination, 4);
            }
            else if (colorChannelCount == 2) // 16-bit
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

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        private static void DebugLogHeader(TGAHeader header)
        {
            Debug.WriteLine(string.Format("ID length:         {0}", args: header.IdLength), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Colourmap type:    {0}", args: header.Colormap.Type), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Image type:        {0}", args: header.DataTypeCode), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Colour map offset: {0}", args: header.Colormap.Origin), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Colour map length: {0}", args: header.Colormap.Length), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Colour map depth:  {0}", args: header.Colormap.Depth), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("X origin:          {0}", args: header.Origin.X), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Y origin:          {0}", args: header.Origin.Y), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Width:             {0}", args: header.Width), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Height:            {0}", args: header.Height), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Bits per pixel:    {0}", args: header.BitsPerPixel), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Descriptor:        {0}", args: header.ImageDescriptor), category: nameof(TGAReader));
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        private static void DebugLogExtentionData(TGAExtentionData extentionData)
        {
            Debug.WriteLine(string.Format("Author Name:             {0}", args: extentionData.AuthorName), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Author Comment:          {0}", args: extentionData.AuthorComment), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Time Stamp:              {0}", args: extentionData.TimeStamp), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Job ID:                  {0}", args: extentionData.JobID), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Job Time:                {0}", args: extentionData.JobTime), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("SoftwareID:              {0}", args: extentionData.SoftwareID), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Software Version:        {0}", args: extentionData.SoftwareVersion), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Key Color:               {0}", args: extentionData.KeyColor), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Pixel Aspect Ratio:      {0}", args: extentionData.PixelAspectRatio), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Gamma Value:             {0}", args: extentionData.GammaValue), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Color Correction Offset: {0}", args: extentionData.ColorCorrectionOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Postage Stamp Offset:    {0}", args: extentionData.PostageStampOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Scan Line Offset:        {0}", args: extentionData.ScanLineOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Attributes Type:         {0}", args: extentionData.AttributesType), category: nameof(TGAReader));
        }
    }
}