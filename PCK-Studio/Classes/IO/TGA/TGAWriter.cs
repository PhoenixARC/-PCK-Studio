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
using OMI.Workers;
using OMI;

namespace PckStudio.IO.TGA
{
    internal class TGAWriter : IDataFormatWriter
    {
        private Bitmap _bitmap;

        public TGAWriter(Bitmap bitmap)
        {
            _bitmap = bitmap;
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

            byte[] pixel = new byte[4];
            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    IntPtr pixelOffset = bitmapData.Scan0 + 4 * x + bitmapData.Stride * y;
                    Marshal.Copy(pixelOffset, pixel, 0, 4);
                    writer.Write(pixel);
                }
            }

        }

        private void WriteFooter(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(0); // extensionDataOffset
            writer.Write(0); // developerAreaDataOffset
            writer.WriteString(TGAFooter.Signature);
            writer.Write((byte)0x2E);
            writer.Write((byte)0x00);
        }

        
        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                WriteHeader(writer);
                WriteImage(writer);
                WriteFooter(writer);
            }
        }

        public void WriteToFile(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }
    }
}
