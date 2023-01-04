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
using PckStudio.Classes.IO;
using System.Text;

namespace PckStudio.Classes.Utils.TGA
{
    internal class TGAWriter : StreamDataWriter
    {
        private Bitmap _bitmap;
        private TGADataTypeCode _format;

        public TGAWriter() : base(littleEndian: true)
        {
        }

        public void Write(Stream stream, Bitmap bitmap, TGADataTypeCode format)
        {
            _format = format;
            _bitmap = bitmap;
            WriteToStream(stream);
        }

        public void SaveHeader(Stream stream, TGAHeader header)
        {

            WriteBytes(stream, header.Id);
            WriteBytes(stream, new byte[]
            {
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
                    switch (header.DataTypeCode)
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

        private void SaveFooter(Stream stream)
        {
            WriteInt(stream, 0); // extensionDataOffset
            WriteInt(stream, 0); // developerAreaDataOffset
            WriteString(stream, "TRUEVISION-XFILE", Encoding.ASCII);
            WriteShort(stream, 0x002E);
        }

        protected override void WriteToStream(Stream stream)
        {
            SaveHeader(stream, default);
        }
    }
}
