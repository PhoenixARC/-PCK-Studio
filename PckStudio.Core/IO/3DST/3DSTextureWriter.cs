using System;
using System.Drawing;
using System.IO;
using System.Text;
using OMI;
using OMI.Workers;

namespace PckStudio.Core.IO._3DST
{
    public class _3DSTextureWriter : IDataFormatWriter
    {
        private Image _image;
        private _3DSTextureFormat _format;
        public _3DSTextureWriter(Image image, _3DSTextureFormat format = _3DSTextureFormat.argb8)
        {
            _image = image;
            _format = format;
        }

        public void WriteToFile(string filename)
        {
            using(FileStream fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, ByteOrder.LittleEndian))
            {
                writer.WriteString("3DST"); // 0
                writer.Write(2); // 4 unknown
                writer.Write((int)_format); // 8
                writer.Write(_image.Width); // 12
                writer.Write(_image.Height); // 16
                writer.Write(0); // 20
                writer.Write(0); // 24
                writer.Write(0); // 28
                _image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                byte[] buffer = TextureCodec.Encode(new Bitmap(_image), _format);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
