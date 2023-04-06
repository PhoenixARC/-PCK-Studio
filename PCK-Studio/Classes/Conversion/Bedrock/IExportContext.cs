using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Conversion.Bedrock
{
    internal interface IExportContext : IDisposable
    {
        void PutNextEntry(string name);
        void Write(byte[] buffer, int offset, int count);
    }
}
