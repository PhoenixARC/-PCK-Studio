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
        private TGADataTypeCode _format;

        public TGAWriter(Bitmap bitmap, TGADataTypeCode format)
        {
            _format = format;
            _bitmap = bitmap;
        }

        private void SaveHeader(EndiannessAwareBinaryWriter writer, TGAHeader header)
        {
            writer.Write(header.Id);
            writer.Write(new byte[]
            {
                header.Colormap.Type,
                (byte)header.DataTypeCode
            });
            writer.Write(header.Colormap.Origin);
            writer.Write(header.Colormap.Length);
            writer.Write(header.Colormap.Depth);
            writer.Write(header.Origin.X);
            writer.Write(header.Origin.Y);
            writer.Write(header.Width);
            writer.Write(header.Height);
            writer.Write(new byte[]
            {
                header.BitsPerPixel,
                header.ImageDescriptor,
            });
        }

        private void SaveImage(EndiannessAwareBinaryWriter writer, Bitmap bitmap, TGAHeader header)
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
                    switch (header.DataTypeCode)
                    {
                        case TGADataTypeCode.RGB:
                            if (header.BitsPerPixel == 32)
                            {
                                writer.Write(pixel);
                            }
                            break;
                        default:
                            throw new NotImplementedException(nameof(header.DataTypeCode));
                    }
                }
            }

        }

        private void SaveFooter(EndiannessAwareBinaryWriter writer)
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
                SaveHeader(writer, default);
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
