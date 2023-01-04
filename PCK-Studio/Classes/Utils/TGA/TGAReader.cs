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

        private static void TGA_HandleRGB(Stream stream, TGAHeader header, BitmapData bitmapData)
        {
            int formatSize = header.BitsPerPixel / 8;
            for (int y = 0; y < header.Height; y++)
            {
                for (int x = 0; x < header.Width; x++)
                {
                    IntPtr pixelOffset = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                    WritePixel(pixelOffset, ReadBytes(stream, formatSize), formatSize);
                }
            }
        }

        private static void TGA_HandleRLE_RGB(Stream stream, TGAHeader header, BitmapData bitmapData)
        {
            int formatSize = header.BitsPerPixel / 8;

            int pixelOffset = 0;
            while (pixelOffset < header.Width * header.Height)
            {
                byte[] b = ReadBytes(stream, 1);
                bool isRleChunk = (b[0] & 0x80) != 0;
                int count = b[0] & 0x7f;
                byte[] rleColor = isRleChunk ? ReadBytes(stream, formatSize) : null;
                for (int i = 0; i < count; i++)
                {
                    if (pixelOffset > header.Width * header.Height)
                        throw new Exception($"Invalid pixel offset: {pixelOffset}");
                    WritePixel(bitmapData.Scan0 + pixelOffset + i * 4,
                        isRleChunk
                            ? rleColor
                            : ReadBytes(stream, formatSize),
                        formatSize);
                }
                pixelOffset += count * 4;
            }

        }

        private static void TGA_HandleNoData(Stream stream, TGAHeader header, BitmapData bitmapData)
        {
            Random r = new Random();
            byte[] bytes = new byte[bitmapData.Width * bitmapData.Height * 4];
            r.NextBytes(bytes);
            Marshal.Copy(bytes, 0, bitmapData.Scan0, bytes.Length);
        }

        private static readonly Dictionary<TGADataTypeCode, Action<Stream, TGAHeader, BitmapData>> TGADataTypeHandler = new()
        {
            [TGADataTypeCode.RGB] = TGA_HandleRGB,
            [TGADataTypeCode.RLE_RGB] = TGA_HandleRLE_RGB,
            [TGADataTypeCode.NO_DATA] = TGA_HandleNoData,
        };
        
        private static TGAHeader LoadHeader(Stream stream)
        {
            var header = new TGAHeader();
            byte[] bytes = ReadBytes(stream, 3);
            (var headerIdLength, header.Colormap.Type, header.DataTypeCode) = (bytes[0], bytes[1], (TGADataTypeCode)bytes[2]);
            header.Colormap.Origin = ReadShort(stream);
            header.Colormap.Length = ReadShort(stream);
            header.Colormap.Depth = ReadBytes(stream, 1)[0];
            header.Origin.X = ReadShort(stream);
            header.Origin.Y = ReadShort(stream);
            header.Width = ReadShort(stream);
            header.Height = ReadShort(stream);
            header.BitsPerPixel = ReadBytes(stream, 1)[0];
            header.ImageDescriptor = ReadBytes(stream, 1)[0];
            header.Id = ReadBytes(stream, headerIdLength);
            Debug.WriteLineIf(headerIdLength > 0, $"Image ID: {header.Id}");
            return header;
        }

        private static Bitmap LoadImage(Stream stream, TGAHeader header)
        {
            DebugLogHeader(header);

            //if (header.DataTypeCode == TGADataTypeCode.NO_DATA)
            //    return null;

            Bitmap bitmap = new Bitmap(header.Width, header.Height);
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, header.Width, header.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            if (TGADataTypeHandler.TryGetValue(header.DataTypeCode, out var func))
                func?.Invoke(stream, header, bitmapData);

            bitmap.UnlockBits(bitmapData);
            //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        private static TGAFooter LoadFooter(Stream stream)
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

        private static TGAExtentionData LoadExtentionData(Stream stream, TGAFooter footer)
        {
            if (footer.extensionDataOffset > 0)
            {
                stream.Seek(footer.extensionDataOffset, SeekOrigin.Begin);
                if (ReadShort(stream) == TGAExtentionData.ExtensionSize)
                {
                    TGAExtentionData extentionData = new TGAExtentionData();
                    extentionData.AuthorName = ReadString(stream, 41, Encoding.ASCII);
                    extentionData.AuthorComment = ReadString(stream, 324, Encoding.ASCII);
                    short month = ReadShort(stream);
                    short day = ReadShort(stream);
                    short year = ReadShort(stream);
                    short hour = ReadShort(stream);
                    short minute = ReadShort(stream);
                    short second = ReadShort(stream);
                    extentionData.TimeStamp = new DateTime(year, month, day, hour, minute, second);
                    extentionData.JobID = ReadString(stream, 41, Encoding.ASCII);
                    extentionData.JobTime = new TimeSpan(
                        hours: ReadShort(stream),
                        minutes: ReadShort(stream),
                        seconds: ReadShort(stream)
                    );
                    extentionData.SoftwareID = ReadString(stream, 41, Encoding.ASCII);
                    byte[] version = ReadBytes(stream, 3);
                    extentionData.SoftwareVersion = version;
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
        private static void DebugLogExtentionData(TGAExtentionData extentionData)
        {
            Debug.WriteLine("-------Extention Data-------", category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Author Name:             {0}", args: extentionData.AuthorName), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Author Comment:          {0}", args: extentionData.AuthorComment), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Time Stamp:              {0}", args: extentionData.TimeStamp), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Job ID:                  {0}", args: extentionData.JobID), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Job Time:                {0}", args: extentionData.JobTime), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("SoftwareID:              {0}", args: extentionData.SoftwareID), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Software Version:        {0}.{1}.{2}",   extentionData.SoftwareVersion[0], extentionData.SoftwareVersion[1], extentionData.SoftwareVersion[2]), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Key Color:               {0}", args: extentionData.KeyColor), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Pixel Aspect Ratio:      {0}", args: extentionData.PixelAspectRatio), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Gamma Value:             {0}", args: extentionData.GammaValue), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Color Correction Offset: {0}", args: extentionData.ColorCorrectionOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Postage Stamp Offset:    {0}", args: extentionData.PostageStampOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Scan Line Offset:        {0}", args: extentionData.ScanLineOffset), category: nameof(TGAReader));
            Debug.WriteLine(string.Format("Attributes Type:         {0}", args: extentionData.AttributesType), category: nameof(TGAReader));
            Debug.WriteLine("----------------------------", category: nameof(TGAReader));
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        private static void DebugLogHeader(TGAHeader header)
        {
            Debug.WriteLine("------Header Data------", category: nameof(TGAReader));
            Debug.WriteLine(string.Format("ID length:         {0}", args: header.Id.Length), category: nameof(TGAReader));
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
            Debug.WriteLine("-----------------------", category: nameof(TGAReader));
        }
    }
}