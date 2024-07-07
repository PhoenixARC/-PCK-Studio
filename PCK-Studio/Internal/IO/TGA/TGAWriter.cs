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
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using OMI;
using System.Windows.Forms;
using DiscordRPC;

namespace PckStudio.Internal.IO.TGA
{
    internal class TGAWriter
    {
        private Bitmap _bitmap;
        private int extensionDataOffset = 0;

        public TGAWriter()
        {
        }

        private void WriteHeader(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(new byte[]
            {
                0, // header.Id.Length
                0, // colormap type
                (byte)TGADataTypeCode.RGB
            });
            writer.Write(0); // Colormap.Origin
            writer.Write(0); // Colormap.Length
            writer.Write(0); // Colormap.Depth
            writer.Write(0); // Origin.X
            writer.Write(0); // Origin.Y
            writer.Write(_bitmap.Width);
            writer.Write(_bitmap.Height);
            writer.Write(32); // BitsPerPixel
            writer.Write(8);  // ImageDescriptor
        }

        private void WriteImage(EndiannessAwareBinaryWriter writer)
        {
            _bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData bitmapData = _bitmap.LockBits(
                new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            byte[] buffer = new byte[_bitmap.Width * _bitmap.Height * 4];
            Marshal.Copy(bitmapData.Scan0, buffer, 0, _bitmap.Width * _bitmap.Height * 4);
            writer.Write(buffer);
        }

        private void WriteFooter(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(extensionDataOffset); // extensionDataOffset
            writer.Write(0); // developerAreaDataOffset
            writer.WriteString(TGAFooter.Signature);
            writer.Write((byte)0x2E);
            writer.Write((byte)0x00);
        }

        private void WriteExtensionData(EndiannessAwareBinaryWriter writer)
        {
            extensionDataOffset = Convert.ToInt32(writer.BaseStream.Position);
            TGAExtentionData extentionData = TGAExtentionData.Create();
            writer.Write(TGAExtentionData.ExtensionSize);
            // Author Name
            writer.WriteString(extentionData.AuthorName, 41);
            // Author Comment
            writer.WriteString(extentionData.AuthorComment, 324);
            // Timestamp
            writer.Write((short)extentionData.TimeStamp.Month);
            writer.Write((short)extentionData.TimeStamp.Day);
            writer.Write((short)extentionData.TimeStamp.Year);
            writer.Write((short)extentionData.TimeStamp.Hour);
            writer.Write((short)extentionData.TimeStamp.Minute);
            writer.Write((short)extentionData.TimeStamp.Second);
            // Job id
            writer.WriteString(extentionData.JobID, 41);
            // Job time
            writer.Write((short)extentionData.JobTime.Hours);
            writer.Write((short)extentionData.JobTime.Minutes);
            writer.Write((short)extentionData.JobTime.Seconds);
            // Software Id
            writer.WriteString(extentionData.SoftwareID, 41);
            // Software version
            writer.Write(extentionData.SoftwareVersion, 0, 3);
            // Key color
            writer.Write(extentionData.KeyColor);
            // Pixel aspect ratio
            writer.Write(extentionData.PixelAspectRatio);
            // Gamma value
            writer.Write(extentionData.GammaValue);
            // Color correction offset
            writer.Write(extentionData.ColorCorrectionOffset);
            // Postage stamp offset
            writer.Write(extentionData.PostageStampOffset);
            // Scan line offset
            writer.Write(extentionData.ScanLineOffset);
            // Attributes type
            writer.Write(extentionData.AttributesType);
        }

        public void WriteToStream(Stream stream, Image image)
        {
            _bitmap = new Bitmap(image);
            using (var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                WriteHeader(writer);
                WriteImage(writer);
                WriteExtensionData(writer);
                WriteFooter(writer);
            }
        }

        public void WriteToFile(string filename, Image image)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs, image);
            }
        }
    }
}
