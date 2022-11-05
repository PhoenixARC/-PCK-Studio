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
            return new TGAFileData(header, LoadImage(stream, header), LoadFooter(stream));
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
            Debug.WriteLine("ID length:         {0}", header.IdLength);
            Debug.WriteLine("Colourmap type:    {0}", header.Colormap.Type);
            Debug.WriteLine("Image type:        {0}", header.DataTypeCode);
            Debug.WriteLine("Colour map offset: {0}", header.Colormap.Origin);
            Debug.WriteLine("Colour map length: {0}", header.Colormap.Length);
            Debug.WriteLine("Colour map depth:  {0}", header.Colormap.Depth);
            Debug.WriteLine("X origin:          {0}", header.Origin.X);
            Debug.WriteLine("Y origin:          {0}", header.Origin.Y);
            Debug.WriteLine("Width:             {0}", header.Width);
            Debug.WriteLine("Height:            {0}", header.Height);
            Debug.WriteLine("Bits per pixel:    {0}", header.BitsPerPixel);
            Debug.WriteLine("Descriptor:        {0}", header.ImageDescriptor);

            if (header.DataTypeCode == TGADataTypeCode.NO_DATA)
                return null;

            Bitmap bitmap = new Bitmap(header.Width, header.Height);
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, header.Width, header.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
                );
            int formatSize = header.BitsPerPixel / 8;

            //if (header.DataTypeCode == TGADataTypeCode.RLE_RGB)
            //{             /* Compressed */
            //    byte[] p = ReadBytes(stream, 5);
            //    int j = p[0] & 0x7f;
            //    WritePixel((pixels[n]), &(p[1]), formatSize);
            //    if ((p[0] & 0x80) != 0)
            //    {         /* RLE chunk */
            //        for (int i = 0; i < j; i++)
            //        {
            //            MergeBytes(&(pixels[n]), &(p[1]), formatSize);
            //        }
            //    }
            //    else
            //    {                   /* Normal chunk */
            //        for (int i = 0; i < j; i++)
            //        {
            //            MergeBytes(&(pixels[n]), p, formatSize);
            //        }
            //    }
            //}

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
                            {
                                //WritePixel(pixelOffset, rle_data.Skip(x + y * formatSize).Take(formatSize).ToArray(), formatSize);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        private TGAFooter LoadFooter(Stream stream)
        {
            long origin = stream.Position;
            stream.Seek(-26, SeekOrigin.End);

            TGAFooter footer = new TGAFooter();

            footer.extensionDataOffset = ReadInt(stream); // optional
            footer.developerAreaDataOffset = ReadInt(stream); // optional

            string signature = ReadString(stream, 16, Encoding.ASCII);
            Debug.WriteLineIf(!signature.Equals("TRUEVISION-XFILE") || ReadShort(stream) != 0x002E,
                "Footer end invalid");
            
            stream.Seek(origin, SeekOrigin.Begin);

            Debug.WriteLine("Extension Data Offset:         {0:x}", footer.extensionDataOffset);
            Debug.WriteLine("Developer Area Data Offset:    {0:x}", footer.developerAreaDataOffset);
            return footer;
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
}
