using System;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace PckStudio.Conversion.Bedrock
{
    internal class ZipExportContext : IExportContext
    {
        private ZipOutputStream _stream;

        public ZipExportContext(string filename, int level = Deflater.NO_COMPRESSION)
        {
            var fs = File.OpenWrite(filename);
            _stream = new ZipOutputStream(fs);
            _stream.SetLevel(level);
        }

        public void Dispose()
        {
            _stream?.Close();
            _stream = null;
        }

        public void PutNextEntry(string name)
        {
            _stream.PutNextEntry(new ZipEntry(name));
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}
