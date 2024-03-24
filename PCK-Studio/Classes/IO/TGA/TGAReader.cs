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
using System.Collections.Generic;
using OMI.Workers;
using OMI;

namespace PckStudio.IO.TGA
{
    internal class TGAReader : IDataFormatReader<TGAFileData>, IDataFormatReader
    {
        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);

        public TGAFileData FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                using( var fs = File.OpenRead(filename) )
                {
                    return FromStream(fs);
                }
            }
            throw new FileNotFoundException(filename);
        }

        public TGAFileData FromStream(Stream stream)
        {
            using var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian);
            TGAHeader header = LoadHeader(reader);
            Image image = LoadImage(reader, header);
            TGAFooter footer = LoadFooter(reader);
            TGAExtentionData extentionData = LoadExtentionData(reader, footer);
            return new TGAFileData(header, image, footer, extentionData);
        }

        private static void TGA_HandleRGB(EndiannessAwareBinaryReader reader, TGAHeader header, BitmapData bitmapData)
        {
            int bytesPerPixel = header.BitsPerPixel / 8;

            byte[] data = reader.ReadBytes(header.Height * header.Width * bytesPerPixel);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
        }

        private static void TGA_HandleNoData(EndiannessAwareBinaryReader _, TGAHeader header, BitmapData bitmapData)
        {
            Random r = new Random();
            byte[] bytes = new byte[bitmapData.Width * bitmapData.Height * 4];
            r.NextBytes(bytes);
            Marshal.Copy(bytes, 0, bitmapData.Scan0, bytes.Length);
        }
        
        private static TGAHeader LoadHeader(EndiannessAwareBinaryReader reader)
        {
            var header = new TGAHeader();
            byte[] bytes = reader.ReadBytes(3);
            (var headerIdLength, header.Colormap.Type, header.DataTypeCode) = (bytes[0], bytes[1], (TGADataTypeCode)bytes[2]);
            header.Colormap.Origin = reader.ReadInt16();
            header.Colormap.Length = reader.ReadInt16();
            header.Colormap.Depth = reader.ReadByte();
            header.Origin.X = reader.ReadInt16();
            header.Origin.Y = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.BitsPerPixel = reader.ReadByte();
            header.ImageDescriptor = reader.ReadByte();
            header.Id = reader.ReadBytes(headerIdLength);
            DebugLogHeader(header);
            return header;
        }

        private static PixelFormat GetPixelFormat(int bytesPerPixel)
        {
            return bytesPerPixel switch
            {
                2 => PixelFormat.Format16bppArgb1555,
                3 => PixelFormat.Format24bppRgb,
                4 => PixelFormat.Format32bppArgb,
                _ => throw new NotSupportedException(nameof(bytesPerPixel))
            };
        }

        private static Image LoadImage(EndiannessAwareBinaryReader reader, TGAHeader header)
        {
            if (header.DataTypeCode != TGADataTypeCode.RGB)
                throw new NotSupportedException(nameof(header.DataTypeCode));

            Bitmap bitmap = new Bitmap(header.Width, header.Height);
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, header.Width, header.Height),
                ImageLockMode.WriteOnly,
                GetPixelFormat(header.BitsPerPixel >> 3));

            if (header.DataTypeCode == TGADataTypeCode.NO_DATA)
            {
                TGA_HandleNoData(reader, header, bitmapData);
                bitmap.UnlockBits(bitmapData);
                return bitmap;
            }

            TGA_HandleRGB(reader, header, bitmapData);
            bitmap.UnlockBits(bitmapData);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        private static TGAFooter LoadFooter(EndiannessAwareBinaryReader reader)
        {
            long origin = reader.BaseStream.Position;
            reader.BaseStream.Seek(-26, SeekOrigin.End);

            TGAFooter footer = new TGAFooter();

            footer.ExtensionDataOffset = reader.ReadInt32(); // optional
            footer.DeveloperAreaDataOffset = reader.ReadInt32(); // optional
            string signature = reader.ReadString(16);
            Debug.Assert(signature.Equals(TGAFooter.Signature) || reader.ReadInt16() != 0x002E,
                "Footer signature invalid");
            reader.BaseStream.Seek(origin, SeekOrigin.Begin);
            DebugLogFooter(footer);
            return footer;
        }

        private static TGAExtentionData LoadExtentionData(EndiannessAwareBinaryReader reader, TGAFooter footer)
        {
            if (footer.ExtensionDataOffset > 0)
            {
                reader.BaseStream.Seek(footer.ExtensionDataOffset, SeekOrigin.Begin);
                if (reader.ReadInt16() == TGAExtentionData.ExtensionSize)
                {
                    TGAExtentionData extentionData = new TGAExtentionData();
                    extentionData.AuthorName = reader.ReadString(41);
                    extentionData.AuthorComment = reader.ReadString(324);
                    short month = reader.ReadInt16();
                    short day = reader.ReadInt16();
                    short year = reader.ReadInt16();
                    short hour = reader.ReadInt16();
                    short minute = reader.ReadInt16();
                    short second = reader.ReadInt16();
                    extentionData.TimeStamp = new DateTime(year, month, day, hour, minute, second);
                    extentionData.JobID = reader.ReadString(41);
                    extentionData.JobTime = new TimeSpan(
                        hours: reader.ReadInt16(),
                        minutes: reader.ReadInt16(),
                        seconds: reader.ReadInt16()
                    );
                    extentionData.SoftwareID = reader.ReadString(41);
                    extentionData.SoftwareVersion = reader.ReadBytes(3);
                    extentionData.KeyColor = reader.ReadInt32();
                    extentionData.PixelAspectRatio = reader.ReadInt32();
                    extentionData.GammaValue = reader.ReadInt32();
                    extentionData.ColorCorrectionOffset = reader.ReadInt32();
                    extentionData.PostageStampOffset = reader.ReadInt32();
                    extentionData.ScanLineOffset = reader.ReadInt32();
                    extentionData.AttributesType = reader.ReadByte();
                    DebugLogExtentionData(extentionData);
                    return extentionData;
                }
            }
            return default;
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
            Debug.WriteLineIf(header.Id.Length > 0, $"ID:                {header.Id}", category: nameof(TGAReader));
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

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        private static void DebugLogFooter(TGAFooter footer)
        {
            Debug.WriteLine("-------Footer Data-------", category: nameof(TGAReader));
            Debug.WriteLine($"Extension Data Offset:         {footer.ExtensionDataOffset:x}", category: nameof(TGAReader));
            Debug.WriteLine($"Developer Area Data Offset:    {footer.DeveloperAreaDataOffset:x}", category: nameof(TGAReader));
            Debug.WriteLine("-----------------------", category: nameof(TGAReader));
        }
    }
}