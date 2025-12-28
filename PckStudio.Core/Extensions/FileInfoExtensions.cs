using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Workers;

namespace PckStudio.Core.Extensions
{
    public static class FileInfoExtensions
    {
        public static void Write(this FileInfo fileInfo, IDataFormatWriter formatWriter)
        {
            using (Stream stream = !fileInfo.Exists ? fileInfo.Create() : fileInfo.OpenWrite())
            {
                formatWriter.WriteToStream(stream);
            }
        }

        public static void Write(this FileInfo fileInfo, Image image, ImageFormat format)
        {
            using (Stream stream = !fileInfo.Exists ? fileInfo.Create() : fileInfo.OpenWrite())
            {
                image.Save(stream, format);
            }
        }

        public static void Write(this FileInfo fileInfo, byte[] data)
        {
            using (Stream stream = !fileInfo.Exists ? fileInfo.Create() : fileInfo.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
            }
        }

    }
}
