using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Conversion.Bedrock
{
    internal class InMemoryExportContext : IExportContext
    {
        private string _currentEntry;
        private Dictionary<string, byte[]> _buffer;

        public void Dispose()
        {
            _buffer.Clear();
        }

        public void PutNextEntry(string name)
        {
            _currentEntry = name;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _buffer[_currentEntry] = buffer.Skip(offset).Take(count).ToArray();
        }
    }
}
