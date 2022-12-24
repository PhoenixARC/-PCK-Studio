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

        public TGAWriter(bool useLittleEndian) : base(useLittleEndian)
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
            WriteBytes(stream, new byte[]
            {
                    header.IdLength,
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
                        case TGADataTypeCode.RLE_RGB:
                            //if (header.BitsPerPixel == 32)
                            //{
                            //    // Write RLE Encoded Pixels

                            //    const int maxPacketLength = 128;
                            //    int iPacketStart = 0;
                            //    int iPacketEnd = 0;

                            //    Color[] arPixels = default;

                            //    while (iPacketStart < arPixels.Length)
                            //    {
                            //        Color c32PreviousPixel = arPixels[iPacketStart];

                            //        // Get current Packet Type
                            //        RLEPacketType packetType = EncodeToTGAExtension.PacketType(arPixels, iPacketStart);

                            //        // Find Packet End
                            //        int iReadEnd = Mathf.Min(iPacketStart + maxPacketLength, arPixels.Length);
                            //        for (iPacketEnd = iPacketStart + 1; iPacketEnd < iReadEnd; ++iPacketEnd)
                            //        {
                            //            bool bPreviousEqualsCurrent = EncodeToTGAExtension.Equals(arPixels[iPacketEnd - 1], arPixels[iPacketEnd]);

                            //            // Packet End if change in Packet Type or if max Packet-Size reached
                            //            if (packetType == RLEPacketType.RAW && bPreviousEqualsCurrent ||
                            //                packetType == RLEPacketType.RLE && !bPreviousEqualsCurrent)
                            //            {
                            //                break;
                            //            }
                            //        }

                            //        // Write Packet

                            //        int iPacketLength = iPacketEnd - iPacketStart;

                            //        switch (packetType)
                            //        {
                            //            case RLEPacketType.RLE:

                            //                // Add RLE-Bit to PacketLength
                            //                binaryWriter.Write((byte)((iPacketLength - 1) | (1 << 7)));

                            //                binaryWriter.Write(c32PreviousPixel.B);
                            //                binaryWriter.Write(c32PreviousPixel.G);
                            //                binaryWriter.Write(c32PreviousPixel.R);

                            //                if (iBytesPerPixel == iBytesPerPixelARGB32)
                            //                    binaryWriter.Write(c32PreviousPixel.A);

                            //                break;
                            //            case RLEPacketType.RAW:

                            //                WriteBytes((byte)(iPacketLength - 1));

                            //                for (int iPacketPosition = iPacketStart; iPacketPosition < iPacketEnd; ++iPacketPosition)
                            //                {
                            //                    Color c32Pixel = arPixels[iPacketPosition];
                            //                    binaryWriter.Write(c32Pixel.B);
                            //                    binaryWriter.Write(c32Pixel.G);
                            //                    binaryWriter.Write(c32Pixel.R);

                            //                    if (iBytesPerPixel == iBytesPerPixelARGB32)
                            //                        binaryWriter.Write(c32Pixel.A);
                            //                }

                            //                break;
                            //        }

                            //        iPacketStart = iPacketEnd;
                            //    }
                            //}
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
