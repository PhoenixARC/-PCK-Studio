using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Conversion.Bedrock
{
    internal class FileExportContext : IExportContext
    {
        Stream _currentEntry;
        string _filepath;

        public FileExportContext(string filepath)
        {
            if (filepath == null)
                throw new ArgumentNullException(nameof(filepath));

            if (!Directory.Exists(filepath))
                throw new DirectoryNotFoundException("Directory does not exist!");

            _filepath = filepath;
        }

        public void Dispose()
        {
            if (_currentEntry != null)
            {
                _currentEntry.Flush();
                _currentEntry.Close();
            }
        }

        public void PutNextEntry(string name)
        {
            if (_currentEntry != null)
            {
                _currentEntry.Flush();
                _currentEntry.Close();
            }
            string path = Path.Combine(_filepath, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path)); // to avoid subdirectory exceptions
            _currentEntry = File.OpenWrite(path);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (_currentEntry == null)
                throw new ArgumentNullException(nameof(_currentEntry));
            _currentEntry.Write(buffer, offset, count);
        }
    }
}
